using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WillBoard.Core.Entities;

namespace WillBoard.Core.Interfaces.Repositories
{
    public interface IAuthenticationRepository
    {
        Task CreateAsync(Authentication authentication);
        Task<Authentication> ReadAsync(Guid authenticationId);

        Task<IEnumerable<Authentication>> ReadUnexpiredCollectionAsync(Guid accountId);
        Task<IEnumerable<Authentication>> ReadCollectionAsync(Guid accountId, int skip, int take);
        Task<int> ReadCountAsync(Guid accountId);

        Task UpdateAsync(Authentication authentication);
        Task DeleteAsync(Guid authenticationId);
    }
}