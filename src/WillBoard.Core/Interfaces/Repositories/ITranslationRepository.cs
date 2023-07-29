using System.Collections.Generic;
using System.Threading.Tasks;
using WillBoard.Core.Entities;

namespace WillBoard.Core.Interfaces.Repositories
{
    public interface ITranslationRepository
    {
        Task CreateAsync(Translation translation);

        Task<IDictionary<string, string>> ReadDictionaryAsync(string language);
        Task<IEnumerable<Translation>> ReadCollectionAsync();

        Task UpdateAsync(Translation translation);

        Task DeleteAsync(string language, string key);
    }
}