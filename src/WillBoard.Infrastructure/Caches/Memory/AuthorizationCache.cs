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
    public class AuthorizationCache : IAuthorizationCache
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IAuthorizationRepository _authorizationRepository;
        private readonly ILockManager _lockManager;
        private readonly ICancellationTokenManager _cancellationTokenManager;

        public AuthorizationCache(IMemoryCache memoryCache, IAuthorizationRepository authorizationRepository, ILockManager lockManager, ICancellationTokenManager cancellationTokenManager)
        {
            _memoryCache = memoryCache;
            _authorizationRepository = authorizationRepository;
            _lockManager = lockManager;
            _cancellationTokenManager = cancellationTokenManager;
        }

        public async Task<IEnumerable<Authorization>> GetAccountCollectionAsync(Guid accountId)
        {
            if (_memoryCache.TryGetValue($"{nameof(Authorization)}_AccountCollection_{accountId}", out IEnumerable<Authorization> authorizationCollection))
            {
                return authorizationCollection;
            }
            else
            {
                using (await _lockManager.GetAuthorizationLockAsync($"Cache_GetAccountCollection_{accountId}"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(Authorization)}_AccountCollection_{accountId}", out authorizationCollection))
                    {
                        return authorizationCollection;
                    }

                    authorizationCollection = await _authorizationRepository.ReadAccountCollectionAsync(accountId);

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.Normal,
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(5)
                    };

                    var cancellationTokenSource = _cancellationTokenManager.GetAuthorizationCancellationTokenSource($"Cache_AccountCollection");
                    memoryCacheEntryOptions.AddExpirationToken(new CancellationChangeToken(cancellationTokenSource.Token));

                    _memoryCache.Set($"{nameof(Authorization)}_AccountCollection_{accountId}", authorizationCollection, memoryCacheEntryOptions);

                    return authorizationCollection;
                }
            }
        }

        public async Task RemoveAccountCollectionAsync(Guid accountId)
        {
            _memoryCache.Remove($"{nameof(Authorization)}_AccountCollection_{accountId}");
        }

        public async Task PurgeAccountCollectionAsync()
        {
            _cancellationTokenManager.RemoveAuthorizationCancellationTokenSource($"Cache_AccountCollection");
        }

        public async Task<Authorization> GetBoardAsync(string boardId, Guid authorizationId)
        {
            if (_memoryCache.TryGetValue($"{nameof(Authorization)}_Board_{boardId}_{authorizationId}", out Authorization authorization))
            {
                return authorization;
            }
            else
            {
                using (await _lockManager.GetAuthorizationLockAsync($"Cache_GetBoard_{boardId}_{authorizationId}"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(Authorization)}_Board_{boardId}_{authorizationId}", out authorization))
                    {
                        return authorization;
                    }

                    authorization = await _authorizationRepository.ReadBoardAsync(boardId, authorizationId);

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.Normal,
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(5)
                    };

                    var cancellationTokenSource = _cancellationTokenManager.GetAuthorizationCancellationTokenSource($"Cache_GetBoard_{boardId}");
                    memoryCacheEntryOptions.AddExpirationToken(new CancellationChangeToken(cancellationTokenSource.Token));

                    _memoryCache.Set($"{nameof(Authorization)}_Board_{boardId}_{authorizationId}", authorization, memoryCacheEntryOptions);

                    return authorization;
                }
            }
        }

        public async Task RemoveBoardAsync(string boardId, Guid authorizationId)
        {
            _memoryCache.Remove($"{nameof(Authorization)}_Board_{boardId}_{authorizationId}");
        }

        public async Task PurgeBoardAsync(string boardId)
        {
            _cancellationTokenManager.RemoveAuthorizationCancellationTokenSource($"Cache_GetBoard_{boardId}");
        }

        public async Task<IEnumerable<Authorization>> GetBoardCollectionAsync(string boardId, int skip, int take)
        {
            if (_memoryCache.TryGetValue($"{nameof(Authorization)}_BoardCollection_{skip}_{take}", out IEnumerable<Authorization> authorizationCollection))
            {
                return authorizationCollection;
            }
            else
            {
                using (await _lockManager.GetAuthorizationLockAsync($"Cache_GetBoardCollection_{boardId}_{skip}_{take}"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(Authorization)}_BoardCollection_{boardId}_{skip}_{take}", out authorizationCollection))
                    {
                        return authorizationCollection;
                    }

                    authorizationCollection = await _authorizationRepository.ReadBoardCollectionAsync(boardId, skip, take);

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.Normal,
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(5)
                    };

                    var cancellationTokenSource = _cancellationTokenManager.GetAuthorizationCancellationTokenSource($"Cache_GetBoardCollection_{boardId}");
                    memoryCacheEntryOptions.AddExpirationToken(new CancellationChangeToken(cancellationTokenSource.Token));

                    _memoryCache.Set($"{nameof(Authorization)}_BoardCollection_{boardId}_{skip}_{take}", authorizationCollection, memoryCacheEntryOptions);

                    return authorizationCollection;
                }
            }
        }

        public async Task PurgeBoardCollectionAsync(string boardId)
        {
            _cancellationTokenManager.RemoveAuthorizationCancellationTokenSource($"Cache_GetBoardCollection_{boardId}");
        }

        public async Task<int> GetBoardCountAsync(string boardId)
        {
            if (_memoryCache.TryGetValue($"{nameof(Authorization)}_BoardCount_{boardId}", out int count))
            {
                return count;
            }
            else
            {
                using (await _lockManager.GetAuthorizationLockAsync($"Cache_GetBoardCount"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(Authorization)}_BoardCount_{boardId}", out count))
                    {
                        return count;
                    }

                    count = await _authorizationRepository.ReadBoardCountAsync(boardId);

                    _memoryCache.Set($"{nameof(Authorization)}_BoardCount_{boardId}", count, new MemoryCacheEntryOptions()
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
            _memoryCache.Remove($"{nameof(Authorization)}_BoardCount_{boardId}");
        }
    }
}