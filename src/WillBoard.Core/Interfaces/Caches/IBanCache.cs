using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using WillBoard.Core.Entities;
using WillBoard.Core.Enums;

namespace WillBoard.Core.Interfaces.Caches
{
    public interface IBanCache
    {
        Task<Ban> GetSystemAsync(Guid banId);
        Task RemoveSystemAsync(Guid banId);
        Task PurgeSystemAsync();

        Task<IEnumerable<Ban>> GetSystemUnexpiredCollectionAsync(IpVersion ipVersion, BigInteger ipNumber);
        Task PurgeSystemUnexpiredCollectionAsync();

        Task<IEnumerable<Ban>> GetSystemCollectionAsync(int skip, int take);
        Task PurgeSystemCollectionAsync();

        Task<int> GetSystemCountAsync();
        Task RemoveSystemCountAsync();

        Task<Ban> GetBoardAsync(string boardId, Guid banId);
        Task RemoveBoardAsync(string boardId, Guid banId);
        Task PurgeBoardAsync(string boardId);

        Task<IEnumerable<Ban>> GetBoardUnexpiredCollectionAsync(string boardId, IpVersion ipVersion, BigInteger ipNumber);
        Task PurgeBoardUnexpiredCollectionAsync(string boardId);

        Task<IEnumerable<Ban>> GetBoardCollectionAsync(string boardId, int skip, int take);
        Task PurgeBoardCollectionAsync(string boardId);

        Task<int> GetBoardCountAsync(string boardId);
        Task RemoveBoardCountAsync(string boardId);
    }
}