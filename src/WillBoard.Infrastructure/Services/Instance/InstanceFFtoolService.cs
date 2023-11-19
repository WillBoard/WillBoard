using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WillBoard.Core.Classes;
using WillBoard.Core.Enums;
using WillBoard.Core.Interfaces.Services;
using WillBoard.Core.Results;

namespace WillBoard.Infrastructure.Services.Instance
{
    public class InstanceFFtoolService : IFFtoolService
    {
        private readonly ILogger _logger;
        private readonly IHostEnvironment _hostEnvironment;

        public InstanceFFtoolService(ILogger<IpService> logger, IHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<Result<FFinformation, string>> FFprobeAsync(Stream inputStream)
        {
            var outputArguments = "-hide_banner -loglevel fatal -show_format -show_streams -show_error -print_format json";

            var result = await FFprocessAsync(GetFFprobePath(), inputStream, outputArguments);

            if (!result.Success)
            {
                return Result<FFinformation, string>.ErrorResult(result.Error);
            }

            var ffinformation = new FFinformation();
            using (var document = JsonDocument.Parse(result.Value))
            {
                var ffinformationFormat = new FFinformationFormat();
                var format = document.RootElement.GetProperty("format");
                ffinformationFormat.Name = format.GetProperty("format_name").GetString();
                ffinformationFormat.Duration = double.Parse(format.GetProperty("duration").GetString(), CultureInfo.InvariantCulture);
                ffinformation.Format = ffinformationFormat;

                var streamCollection = new List<FFinformationStream>();
                foreach (var stream in document.RootElement.GetProperty("streams").EnumerateArray())
                {
                    var ffinformationStream = new FFinformationStream();
                    ffinformationStream.Index = stream.GetProperty("index").GetInt32();
                    ffinformationStream.CodecName = stream.GetProperty("codec_name").GetString();
                    ffinformationStream.CodecType = stream.GetProperty("codec_type").GetString() switch
                    {
                        "video" => FFinformationStreamCodecType.Video,
                        "audio" => FFinformationStreamCodecType.Audio,
                        "data" => FFinformationStreamCodecType.Data,
                        "subtitle" => FFinformationStreamCodecType.Subtitle,
                        "attachment" => FFinformationStreamCodecType.Attachment,
                        _ => FFinformationStreamCodecType.Unknown
                    };
                    if (stream.TryGetProperty("width", out JsonElement width))
                    {
                        ffinformationStream.Width = width.GetInt32();
                    }
                    if (stream.TryGetProperty("height", out JsonElement height))
                    {
                        ffinformationStream.Height = height.GetInt32();
                    }
                    streamCollection.Add(ffinformationStream);
                }

                ffinformation.StreamCollection = streamCollection;
            }

            return Result<FFinformation, string>.ValueResult(ffinformation);
        }

        public async Task<Result<string, string>> FFprobeAsync(Stream inputStream, string outputArguments)
        {
            return await FFprocessAsync(GetFFprobePath(), inputStream, outputArguments);
        }

        public async Task<Result<string, string>> FFmpegAsync(Stream inputStream, string outputArguments)
        {
            return await FFprocessAsync(GetFFmpegPath(), inputStream, outputArguments);
        }

        private async Task<Result<string, string>> FFprocessAsync(string fileName, Stream inputStream, string outputArguments)
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = fileName;

                // In future can be used named pipe https://github.com/dotnet/runtime/issues/28979
                // process.StartInfo.Arguments = $"-i pipe:{Guid.NewGuid} {outputArguments}";

                process.StartInfo.Arguments = $"-i - {outputArguments}";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;
                process.EnableRaisingEvents = true;

                var outputBuilder = new StringBuilder();
                var outputCloseEvent = new TaskCompletionSource<bool>();

                process.OutputDataReceived += (s, e) =>
                {
                    if (e.Data == null)
                    {
                        outputCloseEvent.SetResult(true);
                    }
                    else
                    {
                        outputBuilder.AppendLine(e.Data);
                    }
                };

                var errorBuilder = new StringBuilder();
                var errorCloseEvent = new TaskCompletionSource<bool>();

                process.ErrorDataReceived += (s, e) =>
                {
                    if (e.Data == null)
                    {
                        errorCloseEvent.SetResult(true);
                    }
                    else
                    {
                        errorBuilder.AppendLine(e.Data);
                    }
                };

                var isStarted = false;

                try
                {
                    isStarted = process.Start();
                }
                catch (Exception exception)
                {
                    _logger.LogWarning(exception, "Exception occurred during {0} method with {1}.", nameof(FFprocessAsync), fileName);

                    isStarted = false;
                }

                if (!isStarted)
                {
                    throw new Exception("FFtool process did not start.");
                }

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                try
                {
                    using (var binaryReader = new BinaryReader(inputStream))
                    using (var binaryWriter = new BinaryWriter(process.StandardInput.BaseStream))
                    {
                        byte[] buffer = new byte[1024];
                        int readSize = binaryReader.Read(buffer, 0, buffer.Length);

                        while (readSize != 0 && !process.HasExited)
                        {
                            binaryWriter.Write(buffer, 0, readSize);
                            binaryWriter.Flush();
                            readSize = binaryReader.Read(buffer, 0, buffer.Length);
                        }

                        binaryWriter.Close();
                        binaryReader.Close();

                        process.StandardInput.BaseStream.Close();
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "Exception occurred during {0} method with {1}.", nameof(FFprocessAsync), fileName);

                    // Linux produces "Broken Pipe" exception, but it seems working, when is ignored.
                    // As long as the problem is not solved, the exception in Linux will be discarded.
                    if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        throw;
                    }
                }

                await process.WaitForExitAsync();

                await Task.WhenAll(outputCloseEvent.Task, errorCloseEvent.Task);

                if (errorBuilder.Length > 0)
                {
                    return Result<string, string>.ErrorResult(errorBuilder.ToString());
                }

                return Result<string, string>.ValueResult(outputBuilder.ToString());
            }
        }

        private string GetFFprobePath()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return _hostEnvironment.ContentRootPath + @"/Assets/FFprobe/ffprobe-win-64.exe";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return _hostEnvironment.ContentRootPath + @"/Assets/FFprobe/ffprobe-linux-64";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return _hostEnvironment.ContentRootPath + @"/Assets/FFprobe/ffprobe-osx-64";
            }
            throw new NotSupportedException($@"OS platform is not supported");
        }

        private string GetFFmpegPath()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return _hostEnvironment.ContentRootPath + @"/Assets/FFmpeg/ffmpeg-win-64.exe";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return _hostEnvironment.ContentRootPath + @"/Assets/FFmpeg/ffmpeg-linux-64";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return _hostEnvironment.ContentRootPath + @"/Assets/FFmpeg/ffmpeg-osx-64";
            }
            throw new NotSupportedException($@"OS platform is not supported");
        }
    }
}