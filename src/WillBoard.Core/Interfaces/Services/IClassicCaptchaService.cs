using System;

namespace WillBoard.Core.Interfaces.Services
{
    public interface IClassicCaptchaService
    {
        bool Verify(string captchaKey, string captchaValue);
        string AddWildcard(out DateTime captchaStart, out DateTime captchaEnd);
        string AddCaptcha(string captchaValue, out DateTime captchaStart, out DateTime captchaEnd);
        ReadOnlySpan<byte> GenerateCaptha(out string captchaValue);
    }
}