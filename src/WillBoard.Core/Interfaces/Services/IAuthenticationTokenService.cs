using System;

namespace WillBoard.Core.Interfaces.Services
{
    public interface IAuthenticationTokenService
    {
        string Encode(Guid acountId, Guid guid);
        void Decode(string encodedString, out Guid accountId, out Guid guid);
        bool TryDecode(string encodedString, out Guid accountId, out Guid guid);
    }
}