using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using WillBoard.Core.Entities;
using WillBoard.Core.Enums;

namespace WillBoard.Core.Interfaces.Repositories
{
    public interface IBanRepository
    {
        Task CreateAsync(Ban ban);

        Task<Ban> ReadSystemAsync(Guid banId);
        Task<IEnumerable<Ban>> ReadSystemUnexpiredCollectionAsync(IpVersion ipVersion, BigInteger ipNumber);
        Task<IEnumerable<Ban>> ReadSystemCollectionAsync(int skip, int take);
        Task<int> ReadSystemCountAsync();

        Task<Ban> ReadBoardAsync(string boardId, Guid banId);
        Task<IEnumerable<Ban>> ReadBoardUnexpiredCollectionAsync(string boardId, IpVersion ipVersion, BigInteger ipNumber);
        Task<IEnumerable<Ban>> ReadBoardCollectionAsync(string boardId, int skip, int take);
        Task<int> ReadBoardCountAsync(string boardId);

        Task UpdateAsync(Ban ban);
        Task DeleteAsync(Guid banId);
    }
}