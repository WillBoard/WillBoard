using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WillBoard.Core.Entities;

namespace WillBoard.Core.Interfaces.Caches
{
    public interface IAuthenticationCache
    {
        Task<Authentication> GetAsync(Guid accountId, Guid authenticationId);
        Task RemoveAsync(Guid accountId, Guid authenticationId);
        Task PurgeAsync(Guid accountId);

        Task<IEnumerable<Authentication>> GetCollectionAsync(Guid accountId, int skip, int take);
        Task PurgeCollectionAsync(Guid accountId);

        Task<int> GetCountAsync(Guid accountId);
        Task RemoveCountAsync(Guid accountId);
    }
}