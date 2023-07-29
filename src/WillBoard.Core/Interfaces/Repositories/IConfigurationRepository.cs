using System.Collections.Generic;
using System.Threading.Tasks;
using WillBoard.Core.Entities;

namespace WillBoard.Core.Interfaces.Repositories
{
    public interface IConfigurationRepository
    {
        Task CreateAsync(Configuration configuration);

        Task<IEnumerable<Configuration>> ReadCollectionAsync();

        Task UpdateAsync(Configuration configuration);

        Task DeleteAsync(string key);
    }
}