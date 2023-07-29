using System.Collections.Generic;
using System.Threading.Tasks;
using WillBoard.Core.Entities;

namespace WillBoard.Core.Interfaces.Caches
{
    public interface IConfigurationCache
    {
        Task<Configuration> GetAsync(string configurationKey);
        Task<IEnumerable<Configuration>> GetCollectionAsync();
        Task RemoveCollectionAsync();
    }
}