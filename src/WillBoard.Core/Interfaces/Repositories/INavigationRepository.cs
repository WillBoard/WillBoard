using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WillBoard.Core.Entities;

namespace WillBoard.Core.Interfaces.Repositories
{
    public interface INavigationRepository
    {
        Task CreateAsync(Navigation navigation);

        Task<IEnumerable<Navigation>> ReadCollectionAsync();

        Task UpdateAsync(Navigation navigation);

        Task DeleteAsync(Guid navigationId);
    }
}