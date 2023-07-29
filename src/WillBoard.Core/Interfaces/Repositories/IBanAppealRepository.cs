using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WillBoard.Core.Entities;

namespace WillBoard.Core.Interfaces.Repositories
{
    public interface IBanAppealRepository
    {
        Task CreateAsync(BanAppeal banAppeal);

        Task<BanAppeal> ReadSystemAsync(Guid banAppealId);
        Task<IEnumerable<BanAppeal>> ReadSystemBanCollectionAsync(Guid banId);
        Task<IEnumerable<BanAppeal>> ReadSystemCollectionAsync(int skip, int take);
        Task<int> ReadSystemCountAsync();

        Task<BanAppeal> ReadBoardAsync(string boardId, Guid banAppealId);
        Task<IEnumerable<BanAppeal>> ReadBoardBanCollectionAsync(string boardId, Guid banId);
        Task<IEnumerable<BanAppeal>> ReadBoardCollectionAsync(string boardId, int skip, int take);
        Task<int> ReadBoardCountAsync(string boardId);

        Task UpdateAsync(BanAppeal banBanAppeal);

        Task DeleteAsync(Guid banAppealId);
    }
}