using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WillBoard.Core.Entities;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Managers;
using WillBoard.Core.Interfaces.Repositories;

namespace WillBoard.Infrastructure.Caches.Memory
{
    public class ConfigurationCache : IConfigurationCache
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IConfigurationRepository _configurationRepository;
        private readonly ILockManager _lockManager;
        private readonly ICancellationTokenManager _cancellationTokenManager;

        public ConfigurationCache(IMemoryCache memoryCache, IConfigurationRepository configurationRepository, ILockManager lockManager, ICancellationTokenManager cancellationTokenManager)
        {
            _memoryCache = memoryCache;
            _configurationRepository = configurationRepository;
            _lockManager = lockManager;
            _cancellationTokenManager = cancellationTokenManager;
        }

        public async Task<Configuration> GetAsync(string configurationKey)
        {
            var configurationCollection = await GetCollectionAsync();
            var configuration = configurationCollection.Where(e => e.Key == configurationKey).FirstOrDefault();
            return configuration;
        }

        public async Task<IEnumerable<Configuration>> GetCollectionAsync()
        {
            if (_memoryCache.TryGetValue($"{nameof(Configuration)}_Collection", out IEnumerable<Configuration> configurationCollection))
            {
                return configurationCollection;
            }
            else
            {
                using (await _lockManager.GetConfigurationLockAsync($"Cache_GetCollection"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(Configuration)}_Collection", out configurationCollection))
                    {
                        return configurationCollection;
                    }

                    configurationCollection = await _configurationRepository.ReadCollectionAsync();

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.NeverRemove,
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddDays(7)
                    };

                    _memoryCache.Set($"{nameof(Configuration)}_Collection", configurationCollection, memoryCacheEntryOptions);

                    return configurationCollection;
                }
            }
        }

        public async Task RemoveCollectionAsync()
        {
            _memoryCache.Remove($"{nameof(Configuration)}_Collection");
        }
    }
}