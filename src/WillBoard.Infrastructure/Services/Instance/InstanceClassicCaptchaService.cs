using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using SkiaSharp;
using System;
using System.Linq;
using WillBoard.Core.Interfaces.Providers;
using WillBoard.Core.Interfaces.Services;

namespace WillBoard.Infrastructure.Services.Instance
{
    public class InstanceClassicCaptchaService : IClassicCaptchaService
    {
        private readonly string _chars = "abcdefghijklmnopqrstuvwxyz0123456789";
        private readonly SKImageInfo _captchaImageInfo = new SKImageInfo(256, 64, SKColorType.Bgra8888, SKAlphaType.Premul);
        private readonly SKTypeface _typeface;
        private readonly SKImage _backgroundImage;

        private readonly IHostEnvironment _hostEnvironment;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IMemoryCache _memoryCache;

        public InstanceClassicCaptchaService(IHostEnvironment hostEnvironment, IDateTimeProvider dateTimeProvider, IMemoryCache memoryCache)
        {
            _hostEnvironment = hostEnvironment;
            _dateTimeProvider = dateTimeProvider;
            _memoryCache = memoryCache;

            _typeface = SKTypeface.FromFile(hostEnvironment.ContentRootPath + "/Assets/Captcha/cascadia-code-regular.ttf");
            _backgroundImage = SKImage.FromEncodedData(hostEnvironment.ContentRootPath + "/Assets/Captcha/background.png");
        }

        public bool Verify(string captchaKey, string captchaValue)
        {
            if (_memoryCache.TryGetValue("ClassicCAPTCHA_" + captchaKey, out string result))
            {
                if (result == captchaValue)
                {
                    _memoryCache.Remove("ClassicCAPTCHA_" + captchaKey);
                    return true;
                }
            }

            _memoryCache.Remove("ClassicCAPTCHA_" + captchaKey);
            return false;
        }

        public string AddWildcard(out DateTime captchaStart, out DateTime captchaEnd)
        {
            var wildcard = Guid.NewGuid().ToString("N");
            captchaStart = _dateTimeProvider.UtcNow;
            captchaEnd = captchaStart.AddMinutes(5);

            _memoryCache.Set("ClassicCAPTCHA_" + wildcard, wildcard, new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = _dateTimeProvider.UtcNow.AddMinutes(5),
                Priority = CacheItemPriority.NeverRemove
            });

            return wildcard;
        }

        public string AddCaptcha(string captchaValue, out DateTime captchaStart, out DateTime captchaEnd)
        {
            var captchaKey = Guid.NewGuid().ToString("N");
            captchaStart = _dateTimeProvider.UtcNow;
            captchaEnd = captchaStart.AddMinutes(5);

            _memoryCache.Set("ClassicCAPTCHA_" + captchaKey, captchaValue, new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = captchaEnd,
                Priority = CacheItemPriority.NeverRemove
            });

            return captchaKey;
        }

        public ReadOnlySpan<byte> GenerateCaptha(out string captchaValue)
        {
            captchaValue = new string(Enumerable.Repeat(_chars, Random.Shared.Next(5, 6)).Select(s => s[Random.Shared.Next(s.Length)]).ToArray());

            using (var surface = SKSurface.Create(_captchaImageInfo))
            {
                var canvas = surface.Canvas;
                canvas.Clear(SKColors.Black);

                canvas.DrawImage(_backgroundImage, new SKPoint(0, 0));

                using (var textPaint = new SKPaint())
                {
                    textPaint.TextSize = Random.Shared.Next(22, 26);
                    textPaint.IsAntialias = true;
                    textPaint.Typeface = _typeface;

                    float xCenter = _captchaImageInfo.Width / 2;
                    float yCenter = _captchaImageInfo.Height / 2;

                    var textBounds = new SKRect();
                    textPaint.MeasureText(captchaValue, ref textBounds);
                    float xText = xCenter - textBounds.MidX;
                    float yText = yCenter - textBounds.MidY;

                    textPaint.Shader = SKShader.CreateLinearGradient(
                                new SKPoint(textBounds.Left, textBounds.Top),
                                new SKPoint(textBounds.Right, textBounds.Bottom),
                                new SKColor[] { new SKColor((byte)Random.Shared.Next(64, 128), (byte)Random.Shared.Next(64, 128), (byte)Random.Shared.Next(64, 128), (byte)Random.Shared.Next(128, 160)),
                                                new SKColor((byte)Random.Shared.Next(64, 128), (byte)Random.Shared.Next(64, 128), (byte)Random.Shared.Next(64, 128), (byte)Random.Shared.Next(128, 160)) },
                                null,
                                SKShaderTileMode.Repeat);

                    canvas.Translate(xCenter, yCenter);
                    var xDegrees = (double)Random.Shared.Next(-20, 20);
                    var yDegrees = (double)Random.Shared.Next(-20, 20);
                    canvas.Skew((float)Math.Tan(Math.PI * xDegrees / 180), (float)Math.Tan(Math.PI * yDegrees / 180));
                    canvas.Translate(-xCenter, -yCenter);

                    canvas.DrawText(captchaValue, xText + Random.Shared.Next(-60, 60), yText, textPaint);
                }

                canvas.ResetMatrix();

                using (var linePaint = new SKPaint())
                {
                    linePaint.Style = SKPaintStyle.Stroke;
                    linePaint.Color = new SKColor((byte)Random.Shared.Next(0, 256), (byte)Random.Shared.Next(0, 256), (byte)Random.Shared.Next(0, 256));
                    linePaint.StrokeWidth = Random.Shared.Next(1, 2);
                    linePaint.IsAntialias = true;

                    canvas.DrawLine(Random.Shared.Next(-32, 0), Random.Shared.Next(-32, 96), Random.Shared.Next(256, 288), Random.Shared.Next(-32, 96), linePaint);
                }

                using (var linePaint = new SKPaint())
                {
                    linePaint.Style = SKPaintStyle.Stroke;
                    linePaint.Color = new SKColor((byte)Random.Shared.Next(0, 256), (byte)Random.Shared.Next(0, 256), (byte)Random.Shared.Next(0, 256));
                    linePaint.StrokeWidth = Random.Shared.Next(1, 2);
                    linePaint.IsAntialias = true;

                    canvas.DrawLine(Random.Shared.Next(-32, 0), Random.Shared.Next(-32, 96), Random.Shared.Next(256, 288), Random.Shared.Next(-32, 96), linePaint);
                }

                using (var linePaint = new SKPaint())
                {
                    linePaint.Style = SKPaintStyle.Stroke;
                    linePaint.Color = new SKColor((byte)Random.Shared.Next(0, 256), (byte)Random.Shared.Next(0, 256), (byte)Random.Shared.Next(0, 256));
                    linePaint.StrokeWidth = Random.Shared.Next(1, 2);
                    linePaint.IsAntialias = true;

                    canvas.DrawLine(Random.Shared.Next(-32, 0), Random.Shared.Next(-32, 96), Random.Shared.Next(256, 288), Random.Shared.Next(-32, 96), linePaint);
                }

                using (var noisePaint = new SKPaint())
                {
                    noisePaint.Shader = SKShader.CreatePerlinNoiseTurbulence((float)Random.Shared.NextDouble(), (float)Random.Shared.NextDouble(), 1, (float)Random.Shared.NextDouble(), new SKPointI(256, 64));
                    canvas.DrawRect(new SKRect(0, 0, 256, 64), noisePaint);
                }

                var imageOutput = surface.Snapshot();

                using (SKData dataOutput = imageOutput.Encode(SKEncodedImageFormat.Png, 100))
                {
                    return new ReadOnlySpan<byte>(dataOutput.ToArray());
                }
            }
        }
    }
}