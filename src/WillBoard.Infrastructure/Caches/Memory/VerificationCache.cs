using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WillBoard.Core.Entities;
using WillBoard.Core.Enums;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Managers;
using WillBoard.Core.Interfaces.Repositories;

namespace WillBoard.Infrastructure.Caches.Memory
{
    public class VerificationCache : IVerificationCache
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IVerificationRepository _verificationRepository;
        private readonly ILockManager _lockManager;
        private readonly ICancellationTokenManager _cancellationTokenManager;

        public VerificationCache(IMemoryCache memoryCache, IVerificationRepository verificationRepository, ILockManager lockManager, ICancellationTokenManager cancellationTokenManager)
        {
            _memoryCache = memoryCache;
            _verificationRepository = verificationRepository;
            _lockManager = lockManager;
            _cancellationTokenManager = cancellationTokenManager;
        }

        public async Task<IEnumerable<Verification>> GetSystemUnexpiredCollectionAsync(IpVersion ipVersion, UInt128 ipNumber)
        {
            if (_memoryCache.TryGetValue($"{nameof(Verification)}_SystemUnexpiredCollection_{ipVersion}_{ipNumber}", out IEnumerable<Verification> verificationCollection))
            {
                return verificationCollection;
            }
            else
            {
                using (await _lockManager.GetVerificationLockAsync($"Cache_GetSystemUnexpiredCollection_{ipVersion}_{ipNumber}"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(Verification)}_SystemUnexpiredCollection_{ipVersion}_{ipNumber}", out verificationCollection))
                    {
                        return verificationCollection;
                    }

                    verificationCollection = await _verificationRepository.ReadSystemUnexpiredCollectionAsync(ipVersion, ipNumber);

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.High,
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(15)
                    };

                    var cancellationTokenSource = _cancellationTokenManager.GetVerificationCancellationTokenSource("Cache_GetSystemUnexpiredCollection");
                    memoryCacheEntryOptions.AddExpirationToken(new CancellationChangeToken(cancellationTokenSource.Token));

                    _memoryCache.Set($"{nameof(Verification)}_SystemUnexpiredCollection_{ipVersion}_{ipNumber}", verificationCollection, memoryCacheEntryOptions);

                    return verificationCollection;
                }
            }
        }

        public async Task RemoveSystemUnexpiredCollectionAsync(IpVersion ipVersion, UInt128 ipNumber)
        {
            _memoryCache.Remove($"{nameof(Verification)}_SystemUnexpiredCollection_{ipVersion}_{ipNumber}");
        }

        public async Task PurgeSystemUnexpiredCollectionAsync()
        {
            _cancellationTokenManager.RemoveVerificationCancellationTokenSource("Cache_GetSystemUnexpiredCollection");
        }

        public async Task<IEnumerable<Verification>> GetBoardUnexpiredCollectionAsync(string boardId, IpVersion ipVersion, UInt128 ipNumber)
        {
            if (_memoryCache.TryGetValue($"{nameof(Verification)}_BoardUnexpiredCollection_{boardId}_{ipVersion}_{ipNumber}", out IEnumerable<Verification> verificationCollection))
            {
                return verificationCollection;
            }
            else
            {
                using (await _lockManager.GetVerificationLockAsync($"Cache_GetBoardUnexpiredCollection_{boardId}_{ipVersion}_{ipNumber}"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(Verification)}_BoardUnexpiredCollection_{boardId}_{ipVersion}_{ipNumber}", out verificationCollection))
                    {
                        return verificationCollection;
                    }

                    verificationCollection = await _verificationRepository.ReadBoardUnexpiredCollectionAsync(boardId, ipVersion, ipNumber);

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.High,
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(15)
                    };

                    var cancellationTokenSource = _cancellationTokenManager.GetVerificationCancellationTokenSource($"Cache_GetBoardUnexpiredCollection_{boardId}");
                    memoryCacheEntryOptions.AddExpirationToken(new CancellationChangeToken(cancellationTokenSource.Token));

                    _memoryCache.Set($"{nameof(Verification)}_BoardUnexpiredCollection_{boardId}_{ipVersion}_{ipNumber}", verificationCollection, memoryCacheEntryOptions);

                    return verificationCollection;
                }
            }
        }

        public async Task RemoveBoardUnexpiredCollectionAsync(string boardId, IpVersion ipVersion, UInt128 ipNumber)
        {
            _memoryCache.Remove($"{nameof(Verification)}_BoardUnexpiredCollection_{boardId}_{ipVersion}_{ipNumber}");
        }

        public async Task PurgeBoardUnexpiredCollectionAsync(string boardId)
        {
            _cancellationTokenManager.RemoveVerificationCancellationTokenSource($"Cache_GetBoardUnexpiredCollection_{boardId}");
        }
    }
}