using Microsoft.AspNetCore.Http;
using SkiaSharp;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Entities;
using WillBoard.Core.Enums;
using WillBoard.Core.Interfaces.Services;
using WillBoard.Core.Results;

namespace WillBoard.Infrastructure.Services
{
    public class FileService : IFileService
    {
        private readonly IStorageService _storageService;
        private readonly IFFtoolService _fftoolService;

        public FileService(IStorageService storageService, IFFtoolService fftoolService)
        {
            _storageService = storageService;
            _fftoolService = fftoolService;
        }

        public byte[] GetMD5(in IFormFile file)
        {
            using (var md5 = MD5.Create())
            {
                return md5.ComputeHash(file.OpenReadStream());
            }
        }

        public async Task<Status<string>> AddFileAsync(Board board, Post post, IFormFile file)
        {
            post.File = true;

            var fileName = post.Creation.ToString("yyMMddHHmmss") + Random.Shared.Next(0, 999999).ToString("000000");

            post.FileNameOriginal = file.FileName;

            var fileNameExtension = Path.GetExtension(post.FileNameOriginal);
            if (string.IsNullOrEmpty(fileNameExtension))
            {
                post.FileName = $"{fileName}";
            }
            else
            {
                post.FileName = $"{fileName}.{GetFileNameExtension(post.FileNameOriginal)}";
            }

            post.FileSize = file.Length;
            post.FilePreviewName = fileName + ".png";
            post.FilePreview = true;

            var sourcePath = _storageService.GetSourceFilePath(board.BoardId, post.FileName);
            var previewPath = _storageService.GetPreviewFilePath(board.BoardId, post.FilePreviewName);

            var previewWidthMax = post.IsThread() ? board.ThreadFilePreviewWidthMax : board.ReplyFilePreviewWidthMax;
            var previewHeightMax = post.IsThread() ? board.ThreadFilePreviewHeightMax : board.ReplyFilePreviewHeightMax;

            if (post.FileMimeType == "image/jpeg" || post.FileMimeType == "image/png" || post.FileMimeType == "image/gif" || post.FileMimeType == "image/bmp")
            {
                using (var inputStream = file.OpenReadStream())
                {
                    var inputData = SKData.Create(inputStream);
                    var inputImage = SKImage.FromEncodedData(inputData);

                    if (inputImage == null)
                    {
                        return Status<string>.ErrorStatus(post.IsThread() ? TranslationKey.ErrorThreadFieldFile : TranslationKey.ErrorReplyFieldFile);
                    }

                    if (post.IsThread())
                    {
                        if (inputImage.Width < board.ThreadFieldFileImageWidthMin)
                        {
                            return Status<string>.ErrorStatus(TranslationKey.ErrorThreadFieldFileImageWidthMin);
                        }
                        if (inputImage.Width > board.ThreadFieldFileImageWidthMax)
                        {
                            return Status<string>.ErrorStatus(TranslationKey.ErrorThreadFieldFileImageWidthMax);
                        }
                        if (inputImage.Height < board.ThreadFieldFileImageHeightMin)
                        {
                            return Status<string>.ErrorStatus(TranslationKey.ErrorThreadFieldFileImageHeightMin);
                        }
                        if (inputImage.Height > board.ThreadFieldFileImageHeightMax)
                        {
                            return Status<string>.ErrorStatus(TranslationKey.ErrorThreadFieldFileImageHeightMax);
                        }
                    }
                    else
                    {
                        if (inputImage.Width < board.ReplyFieldFileImageWidthMin)
                        {
                            return Status<string>.ErrorStatus(TranslationKey.ErrorReplyFieldFileImageWidthMin);
                        }
                        if (inputImage.Width > board.ReplyFieldFileImageWidthMax)
                        {
                            return Status<string>.ErrorStatus(TranslationKey.ErrorReplyFieldFileImageWidthMax);
                        }
                        if (inputImage.Height < board.ReplyFieldFileImageHeightMin)
                        {
                            return Status<string>.ErrorStatus(TranslationKey.ErrorReplyFieldFileImageHeightMin);
                        }
                        if (inputImage.Height > board.ReplyFieldFileImageHeightMax)
                        {
                            return Status<string>.ErrorStatus(TranslationKey.ErrorReplyFieldFileImageHeightMax);
                        }
                    }

                    post.FileWidth = inputImage.Width;
                    post.FileHeight = inputImage.Height;

                    CalculatePreviewDemensions(inputImage.Width, inputImage.Height, previewWidthMax, previewHeightMax, out int previewWidth, out int previewHeight);

                    post.FilePreviewWidth = previewWidth;
                    post.FilePreviewHeight = previewHeight;

                    var outputImageInfo = new SKImageInfo(previewWidth, previewHeight, SKColorType.Rgba8888, SKAlphaType.Premul);
                    using (var outputImage = SKImage.Create(outputImageInfo))
                    {
                        var outputPixmap = outputImage.PeekPixels();
                        var outputSamplingOptions = new SKSamplingOptions(SKCubicResampler.Mitchell);
                        var resized = inputImage.ScalePixels(outputPixmap, outputSamplingOptions, SKImageCachingHint.Disallow);

                        inputImage.Dispose();

                        if (!resized)
                        {
                            return Status<string>.ErrorStatus(post.IsThread() ? TranslationKey.ErrorThreadFieldFile : TranslationKey.ErrorReplyFieldFile);
                        }

                        using (var outputData = outputImage.Encode(SKEncodedImageFormat.Png, 100))
                        using (var outputStream = File.OpenWrite(previewPath))
                        {
                            outputData.SaveTo(outputStream);
                        }
                    }

                    using (var fileStream = new FileStream(sourcePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    return Status<string>.SuccessStatus();
                }
            }

            if (post.FileMimeType == "video/webm" || post.FileMimeType == "audio/webm" || post.FileMimeType == "video/mp4" || post.FileMimeType == "audio/mpeg")
            {
                using (var inputStream = file.OpenReadStream())
                {
                    var result = await _fftoolService.FFprobeAsync(inputStream);

                    if (!result.Success)
                    {
                        return Status<string>.ErrorStatus(post.IsThread() ? TranslationKey.ErrorThreadFieldFile : TranslationKey.ErrorReplyFieldFile);
                    }

                    if (result.Value.Format.Name != "matroska,webm" || result.Value.Format.Name != "mov,mp4,m4a,3gp,3g2,mj2")
                    {
                        // Uncomment when want to limit to only one video stream per file
                        //var videoCollection = result.Value.StreamCollection.Where(stream => stream.CodecType == FFinformationStreamCodecType.Video);
                        //var videoCollectionCount = videoCollection.Count();
                        //if (videoCollectionCount > 1)
                        //{
                        //    return Status<string>.ErrorStatus();
                        //}

                        var video = result.Value.StreamCollection.FirstOrDefault(stream => stream.CodecType == FFinformationStreamCodecType.Video);
                        if (video != null)
                        {
                            if (post.IsThread())
                            {
                                if (result.Value.Format.Duration < board.ThreadFieldFileVideoDurationMin)
                                {
                                    return Status<string>.ErrorStatus(TranslationKey.ErrorThreadFieldFileVideoDurationMin);
                                }
                                if (result.Value.Format.Duration > board.ThreadFieldFileVideoDurationMax)
                                {
                                    return Status<string>.ErrorStatus(TranslationKey.ErrorThreadFieldFileVideoDurationMax);
                                }
                                if (video.Width < board.ThreadFieldFileVideoWidthMin)
                                {
                                    return Status<string>.ErrorStatus(TranslationKey.ErrorThreadFieldFileVideoWidthMin);
                                }
                                if (video.Width > board.ThreadFieldFileVideoWidthMax)
                                {
                                    return Status<string>.ErrorStatus(TranslationKey.ErrorThreadFieldFileVideoWidthMax);
                                }
                                if (video.Height < board.ThreadFieldFileVideoHeightMin)
                                {
                                    return Status<string>.ErrorStatus(TranslationKey.ErrorThreadFieldFileVideoHeightMin);
                                }
                                if (video.Height > board.ThreadFieldFileVideoHeightMax)
                                {
                                    return Status<string>.ErrorStatus(TranslationKey.ErrorThreadFieldFileVideoHeightMax);
                                }
                            }
                            else
                            {
                                if (result.Value.Format.Duration < board.ReplyFieldFileVideoDurationMin)
                                {
                                    return Status<string>.ErrorStatus(TranslationKey.ErrorReplyFieldFileVideoDurationMin);
                                }
                                if (result.Value.Format.Duration > board.ReplyFieldFileVideoDurationMax)
                                {
                                    return Status<string>.ErrorStatus(TranslationKey.ErrorReplyFieldFileVideoDurationMax);
                                }
                                if (video.Width < board.ReplyFieldFileVideoWidthMin)
                                {
                                    return Status<string>.ErrorStatus(TranslationKey.ErrorReplyFieldFileVideoWidthMin);
                                }
                                if (video.Width > board.ReplyFieldFileVideoWidthMax)
                                {
                                    return Status<string>.ErrorStatus(TranslationKey.ErrorReplyFieldFileVideoWidthMax);
                                }
                                if (video.Height < board.ReplyFieldFileVideoHeightMin)
                                {
                                    return Status<string>.ErrorStatus(TranslationKey.ErrorReplyFieldFileVideoHeightMin);
                                }
                                if (video.Height > board.ReplyFieldFileVideoHeightMax)
                                {
                                    return Status<string>.ErrorStatus(TranslationKey.ErrorReplyFieldFileVideoHeightMax);
                                }
                            }

                            post.FileWidth = video.Width;
                            post.FileHeight = video.Height;
                            post.FileDuration = result.Value.Format.Duration;

                            CalculatePreviewDemensions(video.Width, video.Height, previewWidthMax, previewHeightMax, out int previewWidth, out int previewHeight);

                            post.FilePreviewWidth = previewWidth;
                            post.FilePreviewHeight = previewHeight;

                            var previewResult = await GeneratePreviewVideoAsync(file, previewPath, previewWidth, previewHeight);

                            if (!previewResult)
                            {
                                return Status<string>.ErrorStatus(post.IsThread() ? TranslationKey.ErrorThreadFieldFile : TranslationKey.ErrorReplyFieldFile);
                            }

                            using (var stream = new FileStream(sourcePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            return Status<string>.SuccessStatus();
                        }
                    }

                    if (result.Value.Format.Name != "matroska,webm" || result.Value.Format.Name != "mp3")
                    {
                        var audio = result.Value.StreamCollection.FirstOrDefault(stream => stream.CodecType == FFinformationStreamCodecType.Audio);
                        if (audio != null)
                        {
                            if (post.IsThread())
                            {
                                if (result.Value.Format.Duration < board.ThreadFieldFileAudioDurationMin)
                                {
                                    return Status<string>.ErrorStatus(TranslationKey.ErrorThreadFieldFileAudioDurationMin);
                                }
                                if (result.Value.Format.Duration > board.ThreadFieldFileAudioDurationMax)
                                {
                                    return Status<string>.ErrorStatus(TranslationKey.ErrorThreadFieldFileAudioDurationMax);
                                }
                            }
                            else
                            {
                                if (result.Value.Format.Duration < board.ReplyFieldFileAudioDurationMin)
                                {
                                    return Status<string>.ErrorStatus(TranslationKey.ErrorReplyFieldFileVideoDurationMin);
                                }
                                if (result.Value.Format.Duration > board.ReplyFieldFileAudioDurationMax)
                                {
                                    return Status<string>.ErrorStatus(TranslationKey.ErrorReplyFieldFileAudioDurationMax);
                                }
                            }

                            post.FilePreviewWidth = previewWidthMax;
                            post.FilePreviewHeight = previewHeightMax;
                            post.FileDuration = result.Value.Format.Duration;

                            if (post.FileMimeType == "video/webm")
                            {
                                post.FileMimeType = "audio/webm";
                            }

                            var previewResult = await GeneratePreviewAudioAsync(file, previewPath, previewWidthMax, previewHeightMax);

                            if (!previewResult)
                            {
                                return Status<string>.ErrorStatus(post.IsThread() ? TranslationKey.ErrorThreadFieldFile : TranslationKey.ErrorReplyFieldFile);
                            }

                            using (var stream = new FileStream(sourcePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            return Status<string>.SuccessStatus();
                        }
                    }
                }
            }

            return Status<string>.ErrorStatus(post.IsThread() ? TranslationKey.ErrorThreadFieldFile : TranslationKey.ErrorReplyFieldFile);
        }

        private void CalculatePreviewDemensions(int width, int height, int widthMax, int heightMax, out int previewWidth, out int previewHeight)
        {
            if (width > widthMax || height > heightMax)
            {
                float scaleWidth = (float)widthMax / (float)width;
                float scaleHeight = (float)heightMax / (float)height;
                float scale = Math.Min(scaleWidth, scaleHeight);
                previewWidth = (int)(width * scale);
                previewHeight = (int)(height * scale);
            }
            else
            {
                previewWidth = width;
                previewHeight = height;
            }
        }

        private async Task<bool> GeneratePreviewVideoAsync(IFormFile formFile, string previewPath, int previewWidthMax, int previewHeightMax)
        {
            using (var inputStream = formFile.OpenReadStream())
            {
                var outputArguments = $"-loglevel error -vframes 1 -filter:v scale=\"{previewWidthMax}:{previewHeightMax}\" \"{previewPath}\"";

                var result = await _fftoolService.FFmpegAsync(inputStream, outputArguments);

                if (!result.Success)
                {
                    _storageService.DeleteFile(previewPath);
                    return false;
                }

                return true;
            }
        }

        private async Task<bool> GeneratePreviewAudioAsync(IFormFile formFile, string previewPath, int previewWidthMax, int previewHeightMax)
        {
            using (var inputStream = formFile.OpenReadStream())
            {
                var outputArguments = $"-loglevel error -filter_complex \"aformat=channel_layouts=mono,showwavespic=s={previewWidthMax}x{previewHeightMax}:colors=000000\" -frames:v 1 \"{previewPath}\"";

                var result = await _fftoolService.FFmpegAsync(inputStream, outputArguments);

                if (!result.Success)
                {
                    _storageService.DeleteFile(previewPath);
                    return false;
                }

                return true;
            }
        }

        public string GetMimeType(in IFormFile formFile)
        {
            using (var stream = formFile.OpenReadStream())
            {
                return GetMimeType(stream, formFile.FileName);
            }
        }

        public string GetMimeType(Stream stream, string fileName)
        {
            Span<byte> buffer = stackalloc byte[128];

            stream.ReadExactly(buffer);

            return GetMimeType(buffer, fileName);
        }

        public string GetMimeType(ReadOnlySpan<byte> file, string fileName)
        {
            var mime = "application/octet-stream";

            var extension = GetFileNameExtension(fileName);

            if (extension == "bmp")
            {
                if (file[0] == 0x42 && file[1] == 0x4D)
                {
                    mime = "image/bmp";
                }
            }
            else if (extension == "gif")
            {
                if (file[0] == 0x47 && file[1] == 0x49 && file[2] == 0x46 && file[3] == 0x38 && file[4] == 0x37 && file[5] == 0x61 ||
                    file[0] == 0x47 && file[1] == 0x49 && file[2] == 0x46 && file[3] == 0x38 && file[4] == 0x39 && file[5] == 0x61)
                {
                    mime = "image/gif";
                }
            }
            else if (extension == "jpg" || extension == "jpeg")
            {
                if (file[0] == 0xFF && file[1] == 0xD8 && file[2] == 0xFF && file[3] == 0xDB ||
                    file[0] == 0xFF && file[1] == 0xD8 && file[2] == 0xFF && file[3] == 0xE2 ||
                    file[0] == 0xFF && file[1] == 0xD8 && file[2] == 0xFF && file[3] == 0xE3 ||
                    file[0] == 0xFF && file[1] == 0xD8 && file[2] == 0xFF && file[3] == 0xE0 && file[6] == 0x70 && file[7] == 0x46 && file[8] == 0x49 && file[9] == 0x46 && file[10] == 0x00 && file[11] == 0x01 ||
                    file[0] == 0xFF && file[1] == 0xD8 && file[2] == 0xFF && file[3] == 0xE0 && file[6] == 0x4A && file[7] == 0x46 && file[8] == 0x49 && file[9] == 0x46 && file[10] == 0x00 && file[11] == 0x01 ||
                    file[0] == 0xFF && file[1] == 0xD8 && file[2] == 0xFF && file[3] == 0xE1 && file[6] == 0x45 && file[7] == 0x78 && file[8] == 0x69 && file[9] == 0x66 && file[10] == 0x00 && file[11] == 0x00 ||
                    file[0] == 0xFF && file[1] == 0xD8 && file[2] == 0xFF && file[3] == 0xE8 && file[6] == 0x53 && file[7] == 0x50 && file[8] == 0x49 && file[9] == 0x46 && file[10] == 0x46 && file[11] == 0x00)
                {
                    mime = "image/jpeg";
                }
            }
            else if (extension == "png")
            {
                if (file[0] == 0x89 && file[1] == 0x50 && file[2] == 0x4E && file[3] == 0x47 && file[4] == 0x0D && file[5] == 0x0A && file[6] == 0x1A && file[7] == 0x0A)
                {
                    mime = "image/png";
                }
            }
            else if (extension == "webp")
            {
                if (file[0] == 0x52 && file[1] == 0x49 && file[2] == 0x46 && file[3] == 0x46 && file[6] == 0x57 && file[7] == 0x45 && file[8] == 0x42 && file[9] == 0x50)
                {
                    mime = "image/webp";
                }
            }
            else if (extension == "psd")
            {
                if (file[0] == 0x38 && file[1] == 0x42 && file[2] == 0x50 && file[3] == 0x53)
                {
                    mime = "image/x-photoshop";
                }
            }
            else if (extension == "mid" || extension == "midi")
            {
                if (file[0] == 0x4D && file[1] == 0x54 && file[2] == 0x68 && file[3] == 0x64)
                {
                    mime = "audio/midi";
                }
            }
            else if (extension == "mp3")
            {
                if (file[0] == 0xFF && file[1] == 0xFB || file[0] == 0x49 && file[1] == 0x44 && file[2] == 0x33)
                {
                    mime = "audio/mpeg";
                }
            }
            else if (extension == "flac")
            {
                if (file[0] == 0x66 && file[1] == 0x4C && file[2] == 0x61 && file[3] == 0x43)
                {
                    mime = "audio/flac";
                }
            }
            else if (extension == "wav")
            {
                if (file[0] == 0x52 && file[1] == 0x49 && file[2] == 0x46 && file[3] == 0x46 && file[6] == 0x57 && file[7] == 0x41 && file[8] == 0x56 && file[9] == 0x45)
                {
                    mime = "audio/wav";
                }
            }
            else if (extension == "ogg" || extension == "oga" || extension == "ogv")
            {
                if (file[0] == 0x4F && file[1] == 0x67 && file[2] == 0x50 && file[3] == 0x53)
                {
                    if (extension == "ogg")
                    {
                        mime = "audio/ogg";
                    }
                    else if (extension == "oga")
                    {
                        mime = "audio/ogg";
                    }
                    else if (extension == "ogv")
                    {
                        mime = "video/ogg";
                    }
                }
            }
            else if (extension == "webm")
            {
                if (file[0] == 0x1A && file[1] == 0x45 && file[2] == 0xDF && file[3] == 0xA3)
                {
                    mime = "video/webm";
                }
            }
            else if (extension == "mp4")
            {
                if (file[4] == 0x66 && file[5] == 0x74 && file[6] == 0x79 && file[7] == 0x70 && file[8] == 0x6D && file[9] == 0x70 && file[10] == 0x34 && file[11] == 0x32 ||
                    file[4] == 0x66 && file[5] == 0x74 && file[6] == 0x79 && file[7] == 0x70 && file[8] == 0x69 && file[9] == 0x73 && file[10] == 0x6F && file[11] == 0x6D ||
                    file[4] == 0x66 && file[5] == 0x74 && file[6] == 0x79 && file[7] == 0x70 && file[8] == 0x4D && file[9] == 0x53 && file[10] == 0x4E && file[11] == 0x56)
                {
                    mime = "video/mp4";
                }
            }
            else if (extension == "avi")
            {
                if (file[0] == 0x52 && file[1] == 0x49 && file[2] == 0x46 && file[3] == 0x46 && file[6] == 0x41 && file[7] == 0x56 && file[8] == 0x49 && file[9] == 0x20)
                {
                    mime = "video/avi";
                }
            }
            else if (extension == "wasm")
            {
                if (file[0] == 0x00 && file[1] == 0x61 && file[2] == 0x73 && file[3] == 0x6D)
                {
                    mime = "application/wasm";
                }
            }

            return mime;
        }

        private string GetFileNameExtension(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return string.Empty;
            }

            var dotIndex = fileName.LastIndexOf(".");
            if (dotIndex == -1)
            {
                return string.Empty;
            }

            return fileName.Substring(dotIndex).Replace(".", "").ToLowerInvariant();
        }
    }
}