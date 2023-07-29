using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WillBoard.Core.Entities;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Managers;
using WillBoard.Core.Interfaces.Repositories;

namespace WillBoard.Infrastructure.Caches.Memory
{
    public class TranslationCache : ITranslationCache
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ITranslationRepository _translationRepository;
        private readonly ILockManager _lockManager;
        private readonly ICancellationTokenManager _cancellationTokenManager;

        public TranslationCache(IMemoryCache memoryCache, ITranslationRepository translationRepository, ILockManager lockManager, ICancellationTokenManager cancellationTokenManager)
        {
            _memoryCache = memoryCache;
            _translationRepository = translationRepository;
            _lockManager = lockManager;
            _cancellationTokenManager = cancellationTokenManager;
        }

        public async Task<string> GetAsync(string language, string key)
        {
            var translationDictionary = await GetDictionaryAsync(language);

            if (translationDictionary.TryGetValue(key, out string value))
            {
                return value;
            }

            return null;
        }

        public async Task<IDictionary<string, string>> GetDictionaryAsync(string language)
        {
            if (_memoryCache.TryGetValue($"{nameof(Translation)}_Dictionary_{language}", out IDictionary<string, string> translationDictionary))
            {
                return translationDictionary;
            }
            else
            {
                using (await _lockManager.GetTranslationLockAsync($"Cache_GetDictionary_{language}"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(Translation)}_Dictionary_{language}", out translationDictionary))
                    {
                        return translationDictionary;
                    }

                    translationDictionary = await _translationRepository.ReadDictionaryAsync(language);

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.NeverRemove,
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddDays(1)
                    };

                    _memoryCache.Set($"{nameof(Translation)}_Dictionary_{language}", translationDictionary, memoryCacheEntryOptions);

                    return translationDictionary;
                }
            }
        }

        public async Task RemoveDictionaryAsync(string language)
        {
            _memoryCache.Remove($"{nameof(Translation)}_Dictionary_{language}");
        }

        public async Task<IEnumerable<Translation>> GetCollectionAsync()
        {
            if (_memoryCache.TryGetValue($"{nameof(Translation)}_Collection", out IEnumerable<Translation> translationCollection))
            {
                return translationCollection;
            }
            else
            {
                using (await _lockManager.GetTranslationLockAsync($"Cache_GetCollection"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(Translation)}_Collection", out translationCollection))
                    {
                        return translationCollection;
                    }

                    translationCollection = await _translationRepository.ReadCollectionAsync();

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.Normal,
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(5)
                    };

                    _memoryCache.Set($"{nameof(Translation)}_Collection", translationCollection, memoryCacheEntryOptions);

                    return translationCollection;
                }
            }
        }

        public async Task RemoveCollectionAsync()
        {
            _memoryCache.Remove($"{nameof(Translation)}_Collection");
        }
    }
}