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
    public class InvitationCache : IInvitationCache
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IInvitationRepository _invitationRepository;
        private readonly ILockManager _lockManager;
        private readonly ICancellationTokenManager _cancellationTokenManager;

        public InvitationCache(IMemoryCache memoryCache, IInvitationRepository invitationRepository, ILockManager lockManager, ICancellationTokenManager cancellationTokenManager)
        {
            _memoryCache = memoryCache;
            _invitationRepository = invitationRepository;
            _lockManager = lockManager;
            _cancellationTokenManager = cancellationTokenManager;
        }

        public async Task<Invitation> GetAccountAsync(Guid accountId, Guid invitationId)
        {
            if (_memoryCache.TryGetValue($"{nameof(Invitation)}_Account_{accountId}_{invitationId}", out Invitation invitation))
            {
                return invitation;
            }
            else
            {
                using (await _lockManager.GetInvitationLockAsync($"Cache_GetAccount_{accountId}_{invitationId}"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(Invitation)}_Account_{accountId}_{invitationId}", out invitation))
                    {
                        return invitation;
                    }

                    invitation = await _invitationRepository.ReadAccountAsync(accountId, invitationId);

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.Normal,
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(15)
                    };

                    var idCancellationTokenSource = _cancellationTokenManager.GetInvitationCancellationTokenSource($"Cache_GetAccount_{accountId}");
                    memoryCacheEntryOptions.AddExpirationToken(new CancellationChangeToken(idCancellationTokenSource.Token));

                    var cancellationTokenSource = _cancellationTokenManager.GetInvitationCancellationTokenSource($"Cache_GetAccount");
                    memoryCacheEntryOptions.AddExpirationToken(new CancellationChangeToken(cancellationTokenSource.Token));

                    _memoryCache.Set($"{nameof(Invitation)}_Account_{accountId}_{invitationId}", invitation, memoryCacheEntryOptions);

                    return invitation;
                }
            }
        }

        public async Task RemoveAccountAsync(Guid accountId, Guid invitationId)
        {
            _memoryCache.Remove($"{nameof(Invitation)}_Account_{accountId}_{invitationId}");
        }

        public async Task PurgeAccountAsync(Guid accountId)
        {
            _cancellationTokenManager.RemoveInvitationCancellationTokenSource($"Cache_GetAccount_{accountId}");
        }

        public async Task PurgeAccountAsync()
        {
            _cancellationTokenManager.RemoveInvitationCancellationTokenSource($"Cache_GetAccount");
        }

        public async Task<IEnumerable<Invitation>> GetAccountCollectionAsync(Guid accountId, int skip, int take)
        {
            if (_memoryCache.TryGetValue($"{nameof(Invitation)}_AccountCollection_{accountId}_{skip}_{take}", out IEnumerable<Invitation> invitationCollection))
            {
                return invitationCollection;
            }
            else
            {
                using (await _lockManager.GetInvitationLockAsync($"Cache_GetAccountCollection_{accountId}_{skip}_{take}"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(Invitation)}_AccountCollection_{accountId}_{skip}_{take}", out invitationCollection))
                    {
                        return invitationCollection;
                    }

                    invitationCollection = await _invitationRepository.ReadAccountCollectionAsync(accountId, skip, take);

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.Normal,
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(5)
                    };

                    var idCancellationTokenSource = _cancellationTokenManager.GetInvitationCancellationTokenSource($"Cache_GetAccountCollection_{accountId}");
                    memoryCacheEntryOptions.AddExpirationToken(new CancellationChangeToken(idCancellationTokenSource.Token));

                    var cancellationTokenSource = _cancellationTokenManager.GetInvitationCancellationTokenSource($"Cache_GetAccountCollection");
                    memoryCacheEntryOptions.AddExpirationToken(new CancellationChangeToken(cancellationTokenSource.Token));

                    _memoryCache.Set($"{nameof(Invitation)}_AccountCollection_{skip}_{take}", invitationCollection, memoryCacheEntryOptions);

                    return invitationCollection;
                }
            }
        }

        public async Task PurgeAccountCollectionAsync(Guid accountId)
        {
            _cancellationTokenManager.RemoveInvitationCancellationTokenSource($"Cache_GetAccountCollection_{accountId}");
        }

        public async Task PurgeAccountCollectionAsync()
        {
            _cancellationTokenManager.RemoveInvitationCancellationTokenSource($"Cache_GetAccountCollection");
        }

        public async Task<int> GetAccountCountAsync(Guid accountId)
        {
            if (_memoryCache.TryGetValue($"{nameof(Invitation)}_AccountCount_{accountId}", out int count))
            {
                return count;
            }
            else
            {
                using (await _lockManager.GetInvitationLockAsync($"Cache_GetAccountCount"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(Invitation)}_AccountCount_{accountId}", out count))
                    {
                        return count;
                    }

                    count = await _invitationRepository.ReadAccountCountAsync(accountId);

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.Normal,
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(5)
                    };

                    var cancellationTokenSource = _cancellationTokenManager.GetInvitationCancellationTokenSource("Cache_GetAccountCount");
                    memoryCacheEntryOptions.AddExpirationToken(new CancellationChangeToken(cancellationTokenSource.Token));

                    _memoryCache.Set($"{nameof(Invitation)}_AccountCount_{accountId}", count, memoryCacheEntryOptions);

                    return count;
                }
            }
        }

        public async Task RemoveAccountCountAsync(Guid accountId)
        {
            _memoryCache.Remove($"{nameof(Invitation)}_AccountCount_{accountId}");
        }

        public async Task PurgeAccountCountAsync()
        {
            _cancellationTokenManager.RemoveInvitationCancellationTokenSource($"Cache_GetAccountCount");
        }

        public async Task<Invitation> GetBoardAsync(string boardId, Guid invitationId)
        {
            if (_memoryCache.TryGetValue($"{nameof(Invitation)}_Board_{boardId}_{invitationId}", out Invitation invitation))
            {
                return invitation;
            }
            else
            {
                using (await _lockManager.GetInvitationLockAsync($"Cache_GetBoard_{boardId}_{invitationId}"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(Invitation)}_Board_{boardId}_{invitationId}", out invitation))
                    {
                        return invitation;
                    }

                    invitation = await _invitationRepository.ReadBoardAsync(boardId, invitationId);

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.Normal,
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(15)
                    };

                    var cancellationTokenSource = _cancellationTokenManager.GetInvitationCancellationTokenSource($"Cache_GetBoard_{boardId}");
                    memoryCacheEntryOptions.AddExpirationToken(new CancellationChangeToken(cancellationTokenSource.Token));

                    _memoryCache.Set($"{nameof(Invitation)}_Board_{boardId}_{invitationId}", invitation, memoryCacheEntryOptions);

                    return invitation;
                }
            }
        }

        public async Task RemoveBoardAsync(string boardId, Guid invitationId)
        {
            _memoryCache.Remove($"{nameof(Invitation)}_Board_{boardId}_{invitationId}");
        }

        public async Task PurgeBoardAsync(string boardId)
        {
            _cancellationTokenManager.RemoveInvitationCancellationTokenSource($"Cache_GetBoard_{boardId}");
        }

        public async Task<IEnumerable<Invitation>> GetBoardCollectionAsync(string boardId, int skip, int take)
        {
            if (_memoryCache.TryGetValue($"{nameof(Invitation)}_BoardCollection_{boardId}_{skip}_{take}", out IEnumerable<Invitation> invitationCollection))
            {
                return invitationCollection;
            }
            else
            {
                using (await _lockManager.GetInvitationLockAsync($"Cache_GetBoardCollection_{boardId}_{skip}_{take}"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(Invitation)}_BoardCollection_{boardId}_{skip}_{take}", out invitationCollection))
                    {
                        return invitationCollection;
                    }

                    invitationCollection = await _invitationRepository.ReadBoardCollectionAsync(boardId, skip, take);

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.Normal,
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(5)
                    };

                    var cancellationTokenSource = _cancellationTokenManager.GetInvitationCancellationTokenSource($"Cache_GetBoardCollection_{boardId}");
                    memoryCacheEntryOptions.AddExpirationToken(new CancellationChangeToken(cancellationTokenSource.Token));

                    _memoryCache.Set($"{nameof(Invitation)}_BoardCollection_{skip}_{take}", invitationCollection, memoryCacheEntryOptions);

                    return invitationCollection;
                }
            }
        }

        public async Task PurgeBoardCollectionAsync(string boardId)
        {
            _cancellationTokenManager.RemoveInvitationCancellationTokenSource($"Cache_GetBoardCollection_{boardId}");
        }

        public async Task<int> GetBoardCountAsync(string boardId)
        {
            if (_memoryCache.TryGetValue($"{nameof(Invitation)}_BoardCount_{boardId}", out int count))
            {
                return count;
            }
            else
            {
                using (await _lockManager.GetInvitationLockAsync($"Cache_GetBoardCount"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(Invitation)}_BoardCount_{boardId}", out count))
                    {
                        return count;
                    }

                    count = await _invitationRepository.ReadBoardCountAsync(boardId);

                    _memoryCache.Set($"{nameof(Invitation)}_BoardCount_{boardId}", count, new MemoryCacheEntryOptions()
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
            _memoryCache.Remove($"{nameof(Invitation)}_BoardCount_{boardId}");
        }
    }
}