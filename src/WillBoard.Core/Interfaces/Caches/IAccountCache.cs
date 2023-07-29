using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WillBoard.Core.Entities;

namespace WillBoard.Core.Interfaces.Caches
{
    public interface IAccountCache
    {
        Task<Account> GetAsync(Guid accountId);
        Task RemoveAsync(Guid accountId);
        Task PurgeAsync();

        Task<IEnumerable<Account>> GetCollectionAsync(int skip, int take);
        Task PurgeCollectionAsync();

        Task<int> GetCountAsync();
        Task RemoveCountAsync();
    }
}