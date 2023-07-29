using System;

namespace WillBoard.Core.Utilities
{
    public static class IdentifierUtility
    {
        public static string Generate(int characterCount)
        {
            var bitCount = 6 * characterCount;
            var byteCount = (int)Math.Ceiling(bitCount / 8f);

            Span<byte> buffer = stackalloc byte[byteCount];
            Random.Shared.NextBytes(buffer);

            var base64 = Convert.ToBase64String(buffer);
            base64 = base64.Replace('+', '-').Replace('/', '_');
            return base64.Substring(0, characterCount);
        }
    }
}