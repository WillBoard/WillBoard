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
    public class NavigationCache : INavigationCache
    {
        private readonly IMemoryCache _memoryCache;
        private readonly INavigationRepository _navigationRepository;
        private readonly ILockManager _lockManager;
        private readonly ICancellationTokenManager _cancellationTokenManager;

        public NavigationCache(IMemoryCache memoryCache, INavigationRepository navigationRepository, ILockManager lockManager, ICancellationTokenManager cancellationTokenManager)
        {
            _memoryCache = memoryCache;
            _navigationRepository = navigationRepository;
            _lockManager = lockManager;
            _cancellationTokenManager = cancellationTokenManager;
        }

        public async Task<Navigation> GetAsync(Guid navigationId)
        {
            var navigationCollection = await GetCollectionAsync();
            var navigation = navigationCollection.Where(e => e.NavigationId == navigationId).FirstOrDefault();
            return navigation;
        }

        public async Task<IEnumerable<Navigation>> GetCollectionAsync()
        {
            if (_memoryCache.TryGetValue($"{nameof(Navigation)}_Collection", out IEnumerable<Navigation> navigationCollection))
            {
                return navigationCollection;
            }
            else
            {
                using (await _lockManager.GetNavigationLockAsync($"Cache_GetCollection"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(Navigation)}_Collection", out navigationCollection))
                    {
                        return navigationCollection;
                    }

                    navigationCollection = await _navigationRepository.ReadCollectionAsync();

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.NeverRemove,
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddDays(7)
                    };

                    _memoryCache.Set($"{nameof(Navigation)}_Collection", navigationCollection, memoryCacheEntryOptions);

                    return navigationCollection;
                }
            }
        }

        public async Task RemoveCollectionAsync()
        {
            _memoryCache.Remove($"{nameof(Navigation)}_Collection");
        }
    }
}