using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WillBoard.Core.Entities;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Managers;
using WillBoard.Core.Interfaces.Repositories;

namespace WillBoard.Infrastructure.Caches.Memory
{
    public class BanAppealCache : IBanAppealCache
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IBanAppealRepository _banAppealRepository;
        private readonly ILockManager _lockManager;
        private readonly ICancellationTokenManager _cancellationTokenManager;

        public BanAppealCache(IMemoryCache memoryCache, IBanAppealRepository banAppealRepository, ILockManager lockManager, ICancellationTokenManager cancellationTokenManager)
        {
            _memoryCache = memoryCache;
            _banAppealRepository = banAppealRepository;
            _lockManager = lockManager;
            _cancellationTokenManager = cancellationTokenManager;
        }

        public async Task<BanAppeal> GetSystemAsync(Guid banAppealId)
        {
            if (_memoryCache.TryGetValue($"{nameof(BanAppeal)}_System_{banAppealId}", out BanAppeal banAppeal))
            {
                return banAppeal;
            }
            else
            {
                using (await _lockManager.GetBanAppealLockAsync($"Cache_GetSystem_{banAppealId}"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(BanAppeal)}_System_{banAppealId}", out banAppeal))
                    {
                        return banAppeal;
                    }

                    banAppeal = await _banAppealRepository.ReadSystemAsync(banAppealId);

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.Normal,
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(15)
                    };

                    var cancellationTokenSource = _cancellationTokenManager.GetBanAppealCancellationTokenSource("Cache_GetSystem");
                    memoryCacheEntryOptions.AddExpirationToken(new CancellationChangeToken(cancellationTokenSource.Token));

                    _memoryCache.Set($"{nameof(BanAppeal)}_System_{banAppealId}", banAppeal, memoryCacheEntryOptions);

                    return banAppeal;
                }
            }
        }

        public async Task RemoveSystemAsync(Guid banAppealId)
        {
            _memoryCache.Remove($"{nameof(BanAppeal)}_System_{banAppealId}");
        }

        public async Task PurgeSystemAsync()
        {
            _cancellationTokenManager.RemoveBanAppealCancellationTokenSource("Cache_GetSystem");
        }

        public async Task<IEnumerable<BanAppeal>> GetSystemBanCollectionAsync(Guid banId)
        {
            if (_memoryCache.TryGetValue($"{nameof(BanAppeal)}_SystemBanCollection_{banId}", out IEnumerable<BanAppeal> banAppeal))
            {
                return banAppeal;
            }
            else
            {
                using (await _lockManager.GetBanAppealLockAsync($"Cache_GetSystemBanCollection_{banId}"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(BanAppeal)}_SystemBanCollection_{banId}", out banAppeal))
                    {
                        return banAppeal;
                    }

                    banAppeal = await _banAppealRepository.ReadSystemBanCollectionAsync(banId);

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.Normal,
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(15)
                    };

                    var cancellationTokenSource = _cancellationTokenManager.GetBanAppealCancellationTokenSource("Cache_GetSystemBanCollection");
                    memoryCacheEntryOptions.AddExpirationToken(new CancellationChangeToken(cancellationTokenSource.Token));

                    _memoryCache.Set($"{nameof(BanAppeal)}_SystemBanCollection_{banId}", banAppeal, memoryCacheEntryOptions);

                    return banAppeal;
                }
            }
        }

        public async Task RemoveSystemBanCollectionAsync(Guid banId)
        {
            _memoryCache.Remove($"{nameof(BanAppeal)}_SystemBanCollection_{banId}");
        }

        public async Task PurgeSystemBanCollectionAsync()
        {
            _cancellationTokenManager.RemoveBanAppealCancellationTokenSource("Cache_GetSystemBanCollection");
        }

        public async Task<IEnumerable<BanAppeal>> GetSystemCollectionAsync(int skip, int take)
        {
            if (_memoryCache.TryGetValue($"{nameof(BanAppeal)}_SystemCollection_{skip}_{take}", out IEnumerable<BanAppeal> banAppealCollection))
            {
                return banAppealCollection;
            }
            else
            {
                using (await _lockManager.GetBanAppealLockAsync($"Cache_GetSystemCollection_{skip}_{take}"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(BanAppeal)}_SystemCollection_{skip}_{take}", out banAppealCollection))
                    {
                        return banAppealCollection;
                    }

                    banAppealCollection = await _banAppealRepository.ReadSystemCollectionAsync(skip, take);

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.Normal,
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(5)
                    };

                    var cancellationTokenSource = _cancellationTokenManager.GetBanAppealCancellationTokenSource("Cache_GetSystemCollection");
                    memoryCacheEntryOptions.AddExpirationToken(new CancellationChangeToken(cancellationTokenSource.Token));

                    _memoryCache.Set($"{nameof(BanAppeal)}_SystemCollection_{skip}_{take}", banAppealCollection, memoryCacheEntryOptions);

                    return banAppealCollection;
                }
            }
        }

        public async Task PurgeSystemCollectionAsync()
        {
            _cancellationTokenManager.RemoveBanAppealCancellationTokenSource("Cache_GetSystemCollection");
        }

        public async Task<int> GetSystemCountAsync()
        {
            if (_memoryCache.TryGetValue($"{nameof(BanAppeal)}_SystemCount", out int count))
            {
                return count;
            }
            else
            {
                using (await _lockManager.GetBanAppealLockAsync($"Cache_GetSystemCount"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(BanAppeal)}_SystemCount", out count))
                    {
                        return count;
                    }

                    count = await _banAppealRepository.ReadSystemCountAsync();

                    _memoryCache.Set($"{nameof(BanAppeal)}_SystemCount", count, new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.Normal,
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(5)
                    });

                    return count;
                }
            }
        }

        public async Task RemoveSystemCountAsync()
        {
            _memoryCache.Remove($"{nameof(BanAppeal)}_SystemCount");
        }

        public async Task<BanAppeal> GetBoardAsync(string boardId, Guid banAppealId)
        {
            if (_memoryCache.TryGetValue($"{nameof(BanAppeal)}_Board_{boardId}_{banAppealId}", out BanAppeal banAppeal))
            {
                return banAppeal;
            }
            else
            {
                using (await _lockManager.GetBanAppealLockAsync($"Cache_GetBoard_{boardId}_{banAppealId}"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(BanAppeal)}_Board_{boardId}_{banAppealId}", out banAppeal))
                    {
                        return banAppeal;
                    }

                    banAppeal = await _banAppealRepository.ReadBoardAsync(boardId, banAppealId);

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.Normal,
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(15)
                    };

                    var cancellationTokenSource = _cancellationTokenManager.GetBanAppealCancellationTokenSource($"Cache_GetBoard_{boardId}");
                    memoryCacheEntryOptions.AddExpirationToken(new CancellationChangeToken(cancellationTokenSource.Token));

                    _memoryCache.Set($"{nameof(BanAppeal)}_Board_{boardId}_{banAppealId}", banAppeal, memoryCacheEntryOptions);

                    return banAppeal;
                }
            }
        }

        public async Task RemoveBoardAsync(string boardId, Guid banAppealId)
        {
            _memoryCache.Remove($"{nameof(BanAppeal)}_Board_{boardId}_{banAppealId}");
        }

        public async Task PurgeBoardAsync(string boardId)
        {
            _cancellationTokenManager.RemoveBanAppealCancellationTokenSource($"Cache_GetBoard_{boardId}");
        }

        public async Task<IEnumerable<BanAppeal>> GetBoardBanCollectionAsync(string boardId, Guid banId)
        {
            if (_memoryCache.TryGetValue($"{nameof(BanAppeal)}_BoardBanCollection_{boardId}_{banId}", out IEnumerable<BanAppeal> banAppeal))
            {
                return banAppeal;
            }
            else
            {
                using (await _lockManager.GetBanAppealLockAsync($"Cache_GetBoardBanCollection_{boardId}_{banId}"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(BanAppeal)}_BoardBanCollection_{boardId}_{banId}", out banAppeal))
                    {
                        return banAppeal;
                    }

                    banAppeal = await _banAppealRepository.ReadBoardBanCollectionAsync(boardId, banId);

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.Normal,
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(15)
                    };

                    var cancellationTokenSource = _cancellationTokenManager.GetBanAppealCancellationTokenSource($"Cache_GetBoardBanCollection_{boardId}");
                    memoryCacheEntryOptions.AddExpirationToken(new CancellationChangeToken(cancellationTokenSource.Token));

                    _memoryCache.Set($"{nameof(BanAppeal)}_BoardBanCollection_{boardId}_{banId}", banAppeal, memoryCacheEntryOptions);

                    return banAppeal;
                }
            }
        }

        public async Task RemoveBoardBanCollectionAsync(string boardId, Guid banId)
        {
            _memoryCache.Remove($"{nameof(BanAppeal)}_BoardBanCollection_{boardId}_{banId}");
        }

        public async Task PurgeBoardBanCollectionAsync(string boardId)
        {
            _cancellationTokenManager.RemoveBanAppealCancellationTokenSource($"Cache_GetBoardBanCollection_{boardId}");
        }

        public async Task<IEnumerable<BanAppeal>> GetBoardCollectionAsync(string boardId, int skip, int take)
        {
            if (_memoryCache.TryGetValue($"{nameof(BanAppeal)}_BoardCollection_{boardId}_{skip}_{take}", out IEnumerable<BanAppeal> banAppealCollection))
            {
                return banAppealCollection;
            }
            else
            {
                using (await _lockManager.GetBanAppealLockAsync($"Cache_GetBoardCollection_{boardId}_{skip}_{take}"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(BanAppeal)}_BoardCollection_{boardId}_{skip}_{take}", out banAppealCollection))
                    {
                        return banAppealCollection;
                    }

                    banAppealCollection = await _banAppealRepository.ReadBoardCollectionAsync(boardId, skip, take);

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.Normal,
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(5)
                    };

                    var cancellationTokenSource = _cancellationTokenManager.GetBanAppealCancellationTokenSource($"Cache_GetBoardCollection_{boardId}");
                    memoryCacheEntryOptions.AddExpirationToken(new CancellationChangeToken(cancellationTokenSource.Token));

                    _memoryCache.Set($"{nameof(BanAppeal)}_BoardCollection_{boardId}_{skip}_{take}", banAppealCollection, memoryCacheEntryOptions);

                    return banAppealCollection;
                }
            }
        }

        public async Task PurgeBoardCollectionAsync(string boardId)
        {
            _cancellationTokenManager.RemoveBanAppealCancellationTokenSource($"Cache_GetBoardCollection_{boardId}");
        }

        public async Task<int> GetBoardCountAsync(string boardId)
        {
            if (_memoryCache.TryGetValue($"{nameof(BanAppeal)}_BoardCount_{boardId}", out int count))
            {
                return count;
            }
            else
            {
                using (await _lockManager.GetBanAppealLockAsync($"Cache_GetBoardCount_{boardId}"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(BanAppeal)}_BoardCount_{boardId}", out count))
                    {
                        return count;
                    }

                    count = await _banAppealRepository.ReadBoardCountAsync(boardId);

                    _memoryCache.Set($"{nameof(BanAppeal)}_BoardCount_{boardId}", count, new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.Normal,
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(5)
                    });

                    return count;
                }
            }
        }

        public async Task RemoveBoardCountAsync(string boardId)
        {
            _memoryCache.Remove($"{nameof(BanAppeal)}_BoardCount_{boardId}");
        }
    }
}