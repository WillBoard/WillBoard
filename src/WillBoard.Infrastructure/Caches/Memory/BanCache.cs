using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using WillBoard.Core.Entities;
using WillBoard.Core.Enums;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Managers;
using WillBoard.Core.Interfaces.Repositories;

namespace WillBoard.Infrastructure.Caches.Memory
{
    public class BanCache : IBanCache
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IBanRepository _banRepository;
        private readonly ILockManager _lockManager;
        private readonly ICancellationTokenManager _cancellationTokenManager;

        public BanCache(IMemoryCache memoryCache, IBanRepository banRepository, ILockManager lockManager, ICancellationTokenManager cancellationTokenManager)
        {
            _memoryCache = memoryCache;
            _banRepository = banRepository;
            _lockManager = lockManager;
            _cancellationTokenManager = cancellationTokenManager;
        }

        public async Task<Ban> GetSystemAsync(Guid banId)
        {
            if (_memoryCache.TryGetValue($"{nameof(Ban)}_System_{banId}", out Ban ban))
            {
                return ban;
            }
            else
            {
                using (await _lockManager.GetBanLockAsync($"Cache_GetSystem_{banId}"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(Ban)}_System_{banId}", out ban))
                    {
                        return ban;
                    }

                    ban = await _banRepository.ReadSystemAsync(banId);

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.Normal,
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(15)
                    };

                    var cancellationTokenSource = _cancellationTokenManager.GetBanCancellationTokenSource("Cache_GetSystem");
                    memoryCacheEntryOptions.AddExpirationToken(new CancellationChangeToken(cancellationTokenSource.Token));

                    _memoryCache.Set($"{nameof(Ban)}_System_{banId}", ban, memoryCacheEntryOptions);

                    return ban;
                }
            }
        }

        public async Task RemoveSystemAsync(Guid banId)
        {
            _memoryCache.Remove($"{nameof(Ban)}_System_{banId}");
        }

        public async Task PurgeSystemAsync()
        {
            _cancellationTokenManager.RemoveBanCancellationTokenSource("Cache_GetSystem");
        }

        public async Task<IEnumerable<Ban>> GetSystemUnexpiredCollectionAsync(IpVersion ipVersion, BigInteger ipNumber)
        {
            if (_memoryCache.TryGetValue($"{nameof(Ban)}_SystemUnexpiredCollection_{ipVersion}_{ipNumber}", out IEnumerable<Ban> banCollection))
            {
                return banCollection;
            }
            else
            {
                using (await _lockManager.GetBanLockAsync($"Cache_GetSystemUnexpiredCollection_{ipVersion}_{ipNumber}"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(Ban)}_SystemUnexpiredCollection_{ipVersion}_{ipNumber}", out banCollection))
                    {
                        return banCollection;
                    }

                    banCollection = await _banRepository.ReadSystemUnexpiredCollectionAsync(ipVersion, ipNumber);

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.High,
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddHours(6)
                    };

                    var cancellationTokenSource = _cancellationTokenManager.GetBanCancellationTokenSource("Cache_GetSystemUnexpiredCollection");
                    memoryCacheEntryOptions.AddExpirationToken(new CancellationChangeToken(cancellationTokenSource.Token));

                    _memoryCache.Set($"{nameof(Ban)}_SystemUnexpiredCollection_{ipVersion}_{ipNumber}", banCollection, memoryCacheEntryOptions);

                    return banCollection;
                }
            }
        }

        public async Task PurgeSystemUnexpiredCollectionAsync()
        {
            _cancellationTokenManager.RemoveBanCancellationTokenSource("Cache_GetSystemUnexpiredCollection");
        }

        public async Task<IEnumerable<Ban>> GetSystemCollectionAsync(int skip, int take)
        {
            if (_memoryCache.TryGetValue($"{nameof(Ban)}_SystemCollection_{skip}_{take}", out IEnumerable<Ban> banCollection))
            {
                return banCollection;
            }
            else
            {
                using (await _lockManager.GetBanLockAsync($"Cache_GetSystemCollection_{skip}_{take}"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(Ban)}_SystemCollection_{skip}_{take}", out banCollection))
                    {
                        return banCollection;
                    }

                    banCollection = await _banRepository.ReadSystemCollectionAsync(skip, take);

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.Normal,
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(5)
                    };

                    var cancellationTokenSource = _cancellationTokenManager.GetBanCancellationTokenSource("Cache_GetSystemCollection");
                    memoryCacheEntryOptions.AddExpirationToken(new CancellationChangeToken(cancellationTokenSource.Token));

                    _memoryCache.Set($"{nameof(Ban)}_SystemCollection_{skip}_{take}", banCollection, memoryCacheEntryOptions);

                    return banCollection;
                }
            }
        }

        public async Task PurgeSystemCollectionAsync()
        {
            _cancellationTokenManager.RemoveBanCancellationTokenSource("Cache_GetSystemCollection");
        }

        public async Task<int> GetSystemCountAsync()
        {
            if (_memoryCache.TryGetValue($"{nameof(Ban)}_SystemCount", out int count))
            {
                return count;
            }
            else
            {
                using (await _lockManager.GetBanLockAsync($"Cache_GetSystemCount"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(Ban)}_SystemCount", out count))
                    {
                        return count;
                    }

                    count = await _banRepository.ReadSystemCountAsync();

                    _memoryCache.Set($"{nameof(Ban)}_SystemCount", count, new MemoryCacheEntryOptions()
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
            _memoryCache.Remove($"{nameof(Ban)}_SystemCount");
        }

        public async Task<Ban> GetBoardAsync(string boardId, Guid banId)
        {
            if (_memoryCache.TryGetValue($"{nameof(Ban)}_Board_{boardId}_{banId}", out Ban ban))
            {
                return ban;
            }
            else
            {
                using (await _lockManager.GetBanLockAsync($"Cache_GetBoard_{boardId}_{banId}"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(Ban)}_Board_{boardId}_{banId}", out ban))
                    {
                        return ban;
                    }

                    ban = await _banRepository.ReadBoardAsync(boardId, banId);

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.Normal,
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(15)
                    };

                    var cancellationTokenSource = _cancellationTokenManager.GetBanCancellationTokenSource($"Cache_GetBoard_{boardId}");
                    memoryCacheEntryOptions.AddExpirationToken(new CancellationChangeToken(cancellationTokenSource.Token));

                    _memoryCache.Set($"{nameof(Ban)}_Board_{boardId}_{banId}", ban, memoryCacheEntryOptions);

                    return ban;
                }
            }
        }

        public async Task RemoveBoardAsync(string boardId, Guid banId)
        {
            _memoryCache.Remove($"{nameof(Ban)}_Board_{boardId}_{banId}");
        }

        public async Task PurgeBoardAsync(string boardId)
        {
            _cancellationTokenManager.RemoveBanCancellationTokenSource($"Cache_GetBoard_{boardId}");
        }

        public async Task<IEnumerable<Ban>> GetBoardUnexpiredCollectionAsync(string boardId, IpVersion ipVersion, BigInteger ipNumber)
        {
            if (_memoryCache.TryGetValue($"{nameof(Ban)}_BoardUnexpiredCollection_{boardId}_{ipVersion}_{ipNumber}", out IEnumerable<Ban> banCollection))
            {
                return banCollection;
            }
            else
            {
                using (await _lockManager.GetBanLockAsync($"Cache_GetBoardUnexpiredCollection_{boardId}_{ipVersion}_{ipNumber}"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(Ban)}_BoardUnexpiredCollection_{boardId}_{ipVersion}_{ipNumber}", out banCollection))
                    {
                        return banCollection;
                    }

                    banCollection = await _banRepository.ReadBoardUnexpiredCollectionAsync(boardId, ipVersion, ipNumber);

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.High,
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddHours(6)
                    };

                    var cancellationTokenSource = _cancellationTokenManager.GetBanCancellationTokenSource($"Cache_GetBoardUnexpiredCollection_{boardId}");
                    memoryCacheEntryOptions.AddExpirationToken(new CancellationChangeToken(cancellationTokenSource.Token));

                    _memoryCache.Set($"{nameof(Ban)}_BoardUnexpiredCollection_{boardId}_{ipVersion}_{ipNumber}", banCollection, memoryCacheEntryOptions);

                    return banCollection;
                }
            }
        }

        public async Task PurgeBoardUnexpiredCollectionAsync(string boardId)
        {
            _cancellationTokenManager.RemoveBanCancellationTokenSource($"Cache_GetBoardUnexpiredCollection_{boardId}");
        }

        public async Task<IEnumerable<Ban>> GetBoardCollectionAsync(string boardId, int skip, int take)
        {
            if (_memoryCache.TryGetValue($"{nameof(Ban)}_BoardCollection_{boardId}_{skip}_{take}", out IEnumerable<Ban> banCollection))
            {
                return banCollection;
            }
            else
            {
                using (await _lockManager.GetBanLockAsync($"Cache_GetBoardCollection_{boardId}_{skip}_{take}"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(Ban)}_BoardCollection_{boardId}_{skip}_{take}", out banCollection))
                    {
                        return banCollection;
                    }

                    banCollection = await _banRepository.ReadBoardCollectionAsync(boardId, skip, take);

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.Normal,
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(5)
                    };

                    var cancellationTokenSource = _cancellationTokenManager.GetBanCancellationTokenSource($"Cache_GetBoardCollection_{boardId}");
                    memoryCacheEntryOptions.AddExpirationToken(new CancellationChangeToken(cancellationTokenSource.Token));

                    _memoryCache.Set($"{nameof(Ban)}_BoardCollection_{boardId}_{skip}_{take}", banCollection, memoryCacheEntryOptions);

                    return banCollection;
                }
            }
        }

        public async Task PurgeBoardCollectionAsync(string boardId)
        {
            _cancellationTokenManager.RemoveBanCancellationTokenSource($"Cache_GetBoardCollection_{boardId}");
        }

        public async Task<int> GetBoardCountAsync(string boardId)
        {
            if (_memoryCache.TryGetValue($"{nameof(Ban)}_BoardCount_{boardId}", out int count))
            {
                return count;
            }
            else
            {
                using (await _lockManager.GetBanLockAsync($"Cache_GetBoardCount_{boardId}"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(Ban)}_BoardCount_{boardId}", out count))
                    {
                        return count;
                    }

                    count = await _banRepository.ReadBoardCountAsync(boardId);

                    _memoryCache.Set($"{nameof(Ban)}_BoardCount_{boardId}", count, new MemoryCacheEntryOptions()
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
            _memoryCache.Remove($"{nameof(Ban)}_BoardCount_{boardId}");
        }
    }
}