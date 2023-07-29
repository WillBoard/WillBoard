using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using WillBoard.Core.Entities;
using WillBoard.Core.Enums;

namespace WillBoard.Core.Interfaces.Caches
{
    public interface IVerificationCache
    {
        Task<IEnumerable<Verification>> GetSystemUnexpiredCollectionAsync(IpVersion ipVersion, BigInteger ipNumber);
        Task RemoveSystemUnexpiredCollectionAsync(IpVersion ipVersion, BigInteger ipNumber);
        Task PurgeSystemUnexpiredCollectionAsync();

        Task<IEnumerable<Verification>> GetBoardUnexpiredCollectionAsync(string boardId, IpVersion ipVersion, BigInteger ipNumber);
        Task RemoveBoardUnexpiredCollectionAsync(string boardId, IpVersion ipVersion, BigInteger ipNumber);
        Task PurgeBoardUnexpiredCollectionAsync(string boardId);
    }
}