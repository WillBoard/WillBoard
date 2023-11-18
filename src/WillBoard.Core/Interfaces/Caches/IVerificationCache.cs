using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WillBoard.Core.Entities;
using WillBoard.Core.Enums;

namespace WillBoard.Core.Interfaces.Caches
{
    public interface IVerificationCache
    {
        Task<IEnumerable<Verification>> GetSystemUnexpiredCollectionAsync(IpVersion ipVersion, UInt128 ipNumber);
        Task RemoveSystemUnexpiredCollectionAsync(IpVersion ipVersion, UInt128 ipNumber);
        Task PurgeSystemUnexpiredCollectionAsync();

        Task<IEnumerable<Verification>> GetBoardUnexpiredCollectionAsync(string boardId, IpVersion ipVersion, UInt128 ipNumber);
        Task RemoveBoardUnexpiredCollectionAsync(string boardId, IpVersion ipVersion, UInt128 ipNumber);
        Task PurgeBoardUnexpiredCollectionAsync(string boardId);
    }
}