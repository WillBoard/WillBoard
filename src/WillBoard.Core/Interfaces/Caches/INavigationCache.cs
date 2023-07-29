using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WillBoard.Core.Entities;

namespace WillBoard.Core.Interfaces.Caches
{
    public interface INavigationCache
    {
        Task<Navigation> GetAsync(Guid navigationId);
        Task<IEnumerable<Navigation>> GetCollectionAsync();
        Task RemoveCollectionAsync();
    }
}