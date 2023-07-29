using System.Collections.Generic;
using System.Threading.Tasks;
using WillBoard.Core.Entities;

namespace WillBoard.Core.Interfaces.Caches
{
    public interface ITranslationCache
    {
        Task<string> GetAsync(string language, string key);
        Task<IDictionary<string, string>> GetDictionaryAsync(string language);
        Task RemoveDictionaryAsync(string language);

        Task<IEnumerable<Translation>> GetCollectionAsync();
        Task RemoveCollectionAsync();
    }
}