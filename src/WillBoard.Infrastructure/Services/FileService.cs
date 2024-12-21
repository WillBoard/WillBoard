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
                if (file[0] == 66 && file[1] == 77)
                {
                    mime = "image/bmp";
                }
            }
            else if (extension == "gif")
            {
                if (file[0] == 71 && file[1] == 73 && file[2] == 70 && file[3] == 56 && file[4] == 55 && file[5] == 97 ||
                    file[0] == 71 && file[1] == 73 && file[2] == 70 && file[3] == 56 && file[4] == 57 && file[5] == 97)
                {
                    mime = "image/gif";
                }
            }
            else if (extension == "jpg" || extension == "jpeg")
            {
                if (file[0] == 255 && file[1] == 216 && file[2] == 255 && file[3] == 219 ||
                    file[0] == 255 && file[1] == 216 && file[2] == 255 && file[3] == 226 ||
                    file[0] == 255 && file[1] == 216 && file[2] == 255 && file[3] == 227 ||
                    file[0] == 255 && file[1] == 216 && file[2] == 255 && file[3] == 224 && file[6] == 112 && file[7] == 70 && file[8] == 73 && file[9] == 70 && file[10] == 0 && file[11] == 1 ||
                    file[0] == 255 && file[1] == 216 && file[2] == 255 && file[3] == 224 && file[6] == 74 && file[7] == 70 && file[8] == 73 && file[9] == 70 && file[10] == 0 && file[11] == 1 ||
                    file[0] == 255 && file[1] == 216 && file[2] == 255 && file[3] == 225 && file[6] == 69 && file[7] == 120 && file[8] == 105 && file[9] == 102 && file[10] == 0 && file[11] == 0 ||
                    file[0] == 255 && file[1] == 216 && file[2] == 255 && file[3] == 232 && file[6] == 83 && file[7] == 80 && file[8] == 73 && file[9] == 70 && file[10] == 70 && file[11] == 0)
                {
                    mime = "image/jpeg";
                }
            }
            else if (extension == "png")
            {
                if (file[0] == 137 && file[1] == 80 && file[2] == 78 && file[3] == 71 && file[4] == 13 && file[5] == 10 && file[6] == 26 && file[7] == 10)
                {
                    mime = "image/png";
                }
            }
            else if (extension == "webp")
            {
                if (file[0] == 82 && file[1] == 73 && file[2] == 70 && file[3] == 70 && file[6] == 87 && file[7] == 69 && file[8] == 66 && file[9] == 80)
                {
                    mime = "image/webp";
                }
            }
            else if (extension == "psd")
            {
                if (file[0] == 56 && file[1] == 66 && file[2] == 80 && file[3] == 83)
                {
                    mime = "image/x-photoshop";
                }
            }
            else if (extension == "mid" || extension == "midi")
            {
                if (file[0] == 77 && file[1] == 84 && file[2] == 104 && file[3] == 100)
                {
                    mime = "audio/midi";
                }
            }
            else if (extension == "mp3")
            {
                if (file[0] == 255 && file[1] == 251 || file[0] == 73 && file[1] == 68 && file[2] == 51)
                {
                    mime = "audio/mpeg";
                }
            }
            else if (extension == "flac")
            {
                if (file[0] == 102 && file[1] == 76 && file[2] == 97 && file[3] == 67)
                {
                    mime = "audio/flac";
                }
            }
            else if (extension == "wav")
            {
                if (file[0] == 82 && file[1] == 73 && file[2] == 70 && file[3] == 70 && file[6] == 87 && file[7] == 65 && file[8] == 86 && file[9] == 69)
                {
                    mime = "audio/wav";
                }
            }
            else if (extension == "ogg" || extension == "oga" || extension == "ogv")
            {
                if (file[0] == 79 && file[1] == 103 && file[2] == 80 && file[3] == 83)
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
                if (file[0] == 26 && file[1] == 69 && file[2] == 223 && file[3] == 163)
                {
                    mime = "video/webm";
                }
            }
            else if (extension == "mp4")
            {
                if (file[4] == 102 && file[5] == 116 && file[6] == 121 && file[7] == 112 && file[8] == 109 && file[9] == 112 && file[10] == 52 && file[11] == 50 ||
                    file[4] == 102 && file[5] == 116 && file[6] == 121 && file[7] == 112 && file[8] == 105 && file[9] == 115 && file[10] == 111 && file[11] == 109 ||
                    file[4] == 102 && file[5] == 116 && file[6] == 121 && file[7] == 112 && file[8] == 77 && file[9] == 83 && file[10] == 78 && file[11] == 86)
                {
                    mime = "video/mp4";
                }
            }
            else if (extension == "avi")
            {
                if (file[0] == 82 && file[1] == 73 && file[2] == 70 && file[3] == 70 && file[6] == 65 && file[7] == 86 && file[8] == 73 && file[9] == 32)
                {
                    mime = "video/avi";
                }
            }
            else if (extension == "wasm")
            {
                if (file[0] == 0 && file[1] == 97 && file[2] == 115 && file[3] == 109)
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