using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WillBoard.Core.Entities;

namespace WillBoard.Core.Interfaces.Caches
{
    public interface IBanAppealCache
    {
        Task<BanAppeal> GetSystemAsync(Guid banAppealId);
        Task RemoveSystemAsync(Guid banAppealId);
        Task PurgeSystemAsync();

        Task<IEnumerable<BanAppeal>> GetSystemBanCollectionAsync(Guid banId);
        Task RemoveSystemBanCollectionAsync(Guid banId);
        Task PurgeSystemBanCollectionAsync();

        Task<IEnumerable<BanAppeal>> GetSystemCollectionAsync(int skip, int take);
        Task PurgeSystemCollectionAsync();

        Task<int> GetSystemCountAsync();
        Task RemoveSystemCountAsync();

        Task<BanAppeal> GetBoardAsync(string boardId, Guid banAppealId);
        Task RemoveBoardAsync(string boardId, Guid banAppealId);
        Task PurgeBoardAsync(string boardId);

        Task<IEnumerable<BanAppeal>> GetBoardBanCollectionAsync(string boardId, Guid banId);
        Task RemoveBoardBanCollectionAsync(string boardId, Guid banId);
        Task PurgeBoardBanCollectionAsync(string boardId);

        Task<IEnumerable<BanAppeal>> GetBoardCollectionAsync(string boardId, int skip, int take);
        Task PurgeBoardCollectionAsync(string boardId);

        Task<int> GetBoardCountAsync(string boardId);
        Task RemoveBoardCountAsync(string boardId);
    }
}