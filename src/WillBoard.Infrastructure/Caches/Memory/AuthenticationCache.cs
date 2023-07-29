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
    public class AuthenticationCache : IAuthenticationCache
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IAuthenticationRepository _authenticationRepository;
        private readonly ILockManager _lockManager;
        private readonly ICancellationTokenManager _cancellationTokenManager;

        public AuthenticationCache(IMemoryCache memoryCache, IAuthenticationRepository authenticationRepository, ILockManager lockManager, ICancellationTokenManager cancellationTokenManager)
        {
            _memoryCache = memoryCache;
            _authenticationRepository = authenticationRepository;
            _lockManager = lockManager;
            _cancellationTokenManager = cancellationTokenManager;
        }

        public async Task<Authentication> GetAsync(Guid accountId, Guid authenticationId)
        {
            if (_memoryCache.TryGetValue($"{nameof(Authentication)}_{accountId}_{authenticationId}", out Authentication authentication))
            {
                return authentication;
            }
            else
            {
                using (await _lockManager.GetAuthenticationLockAsync($"Cache_Get_{accountId}_{authenticationId}"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(Authentication)}_{accountId}_{authenticationId}", out authentication))
                    {
                        return authentication;
                    }

                    authentication = await _authenticationRepository.ReadAsync(authenticationId);

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.High,
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(15)
                    };

                    var cancellationTokenSource = _cancellationTokenManager.GetAuthenticationCancellationTokenSource($"Cache_Get_{accountId}");
                    memoryCacheEntryOptions.AddExpirationToken(new CancellationChangeToken(cancellationTokenSource.Token));

                    _memoryCache.Set($"{nameof(Authentication)}_{accountId}_{authenticationId}", authentication, memoryCacheEntryOptions);

                    return authentication;
                }
            }
        }

        public async Task RemoveAsync(Guid accountId, Guid authenticationId)
        {
            _memoryCache.Remove($"{nameof(Authentication)}_{accountId}_{authenticationId}");
        }

        public async Task PurgeAsync(Guid accountId)
        {
            _cancellationTokenManager.RemoveAuthenticationCancellationTokenSource($"Cache_Get_{accountId}");
        }

        public async Task<IEnumerable<Authentication>> GetCollectionAsync(Guid accountId, int skip, int take)
        {
            if (_memoryCache.TryGetValue($"{nameof(Authentication)}_Collection_{skip}_{take}", out IEnumerable<Authentication> authenticationCollection))
            {
                return authenticationCollection;
            }
            else
            {
                using (await _lockManager.GetAuthenticationLockAsync($"Cache_GetCollection_{accountId}_{skip}_{take}"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(Authentication)}_Collection_{accountId}_{skip}_{take}", out authenticationCollection))
                    {
                        return authenticationCollection;
                    }

                    authenticationCollection = await _authenticationRepository.ReadCollectionAsync(accountId, skip, take);

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.Normal,
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(5)
                    };

                    var cancellationTokenSource = _cancellationTokenManager.GetAuthenticationCancellationTokenSource($"Cache_GetCollection_{accountId}");
                    memoryCacheEntryOptions.AddExpirationToken(new CancellationChangeToken(cancellationTokenSource.Token));

                    _memoryCache.Set($"{nameof(Authentication)}_Collection_{accountId}_{skip}_{take}", authenticationCollection, memoryCacheEntryOptions);

                    return authenticationCollection;
                }
            }
        }

        public async Task PurgeCollectionAsync(Guid accountId)
        {
            _cancellationTokenManager.RemoveAuthenticationCancellationTokenSource($"Cache_GetCollection_{accountId}");
        }

        public async Task<int> GetCountAsync(Guid accountId)
        {
            if (_memoryCache.TryGetValue($"{nameof(Authentication)}_Count_{accountId}", out int count))
            {
                return count;
            }
            else
            {
                using (await _lockManager.GetAuthenticationLockAsync($"Cache_GetCount"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(Authentication)}_Count_{accountId}", out count))
                    {
                        return count;
                    }

                    count = await _authenticationRepository.ReadCountAsync(accountId);

                    _memoryCache.Set($"{nameof(Authentication)}_Count_{accountId}", count, new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.Normal,
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(5)
                    });

                    return count;
                }
            }
        }

        public async Task RemoveCountAsync(Guid accountId)
        {
            _memoryCache.Remove($"{nameof(Authentication)}_Count_{accountId}");
        }
    }
}