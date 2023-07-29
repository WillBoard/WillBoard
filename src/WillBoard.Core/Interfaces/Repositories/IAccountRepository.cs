using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WillBoard.Core.Entities;

namespace WillBoard.Core.Interfaces.Repositories
{
    public interface IAccountRepository
    {
        Task CreateAsync(Account account);

        Task<Account> ReadAsync(Guid accountId);
        Task<IEnumerable<Account>> ReadCollectionAsync(int skip, int take);
        Task<int> ReadCountAsync();

        Task UpdateAsync(Account account);

        Task DeleteAsync(Guid accountId);
    }
}