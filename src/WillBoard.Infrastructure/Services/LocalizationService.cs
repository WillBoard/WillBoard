using System.Threading.Tasks;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Services;

namespace WillBoard.Infrastructure.Services
{
    public class LocalizationService : ILocalizationService
    {
        private readonly ITranslationCache _translationCache;

        public LocalizationService(ITranslationCache translationCache)
        {
            _translationCache = translationCache;
        }

        public async Task<string> GetLocalizationAsync(string language, string key)
        {
            var value = await _translationCache.GetAsync(language, key);

            if (value is not null)
            {
                return value;
            }

            return key;
        }

        public async Task<string> GetLocalizationAsync(string language, string key, params object[] arguments)
        {
            var output = await GetLocalizationAsync(language, key);

            if (arguments.Length == 0)
            {
                return output;
            }

            return string.Format(output, arguments);
        }
    }
}