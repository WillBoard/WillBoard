using System;
using System.Buffers.Text;
using System.Text;
using WillBoard.Core.Interfaces.Services;
using WillBoard.Core.Utilities;

namespace WillBoard.Infrastructure.Services
{
    public class AuthenticationTokenService : IAuthenticationTokenService
    {
        public string Encode(Guid acountId, Guid guid)
        {
            Span<byte> value = stackalloc byte[32];

            NetworkByteOrder.WriteUuid(value.Slice(0, 16), acountId);
            NetworkByteOrder.WriteUuid(value.Slice(16, 16), guid);

            Span<byte> encoded = stackalloc byte[44];
            Base64.EncodeToUtf8(value, encoded, out _, out _);

            var encodedString = Encoding.UTF8.GetString(encoded);

            return encodedString;
        }

        public void Decode(string encodedString, out Guid accountId, out Guid guid)
        {
            Span<byte> encoded = stackalloc byte[44];
            Encoding.UTF8.GetBytes(encodedString, encoded);

            Span<byte> value = stackalloc byte[32];
            Base64.DecodeFromUtf8(encoded, value, out _, out _);

            accountId = NetworkByteOrder.ReadUuid(value.Slice(0, 16));
            guid = NetworkByteOrder.ReadUuid(value.Slice(16, 16));
        }

        public bool TryDecode(string encodedString, out Guid accountId, out Guid guid)
        {
            try
            {
                Decode(encodedString, out accountId, out guid);
                return true;
            }
            catch
            {
                accountId = default(Guid);
                guid = default(Guid);
                return false;
            }
        }
    }
}