using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using System;
using System.Numerics;
using System.Threading.Tasks;
using WillBoard.Core.Entities;
using WillBoard.Core.Enums;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Managers;
using WillBoard.Core.Interfaces.Repositories;

namespace WillBoard.Infrastructure.Caches.Memory
{
    public class BadIpCache : IBadIpCache
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IBadIpRepository _badIPRepository;
        private readonly ILockManager _lockManager;
        private readonly ICancellationTokenManager _cancellationTokenManager;

        public BadIpCache(IMemoryCache memoryCache, IBadIpRepository badIPRepository, ILockManager lockManager, ICancellationTokenManager cancellationTokenManager)
        {
            _memoryCache = memoryCache;
            _badIPRepository = badIPRepository;
            _lockManager = lockManager;
            _cancellationTokenManager = cancellationTokenManager;
        }

        public async Task<BadIp> GetAsync(IpVersion ipVersion, BigInteger ipNumber)
        {
            if (_memoryCache.TryGetValue($"{nameof(BadIp)}_{ipVersion}_{ipNumber}", out BadIp badIP))
            {
                return badIP;
            }
            else
            {
                using (await _lockManager.GetBadIpLockAsync($"Cache_Get_{ipVersion}_{ipNumber}"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(BadIp)}_{ipVersion}_{ipNumber}", out badIP))
                    {
                        return badIP;
                    }

                    badIP = await _badIPRepository.ReadAsync(ipVersion, ipNumber);

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.High,
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddHours(3)
                    };

                    var cancellationTokenSource = _cancellationTokenManager.GetBadIpCancellationTokenSource("Cache_Get");
                    memoryCacheEntryOptions.AddExpirationToken(new CancellationChangeToken(cancellationTokenSource.Token));

                    _memoryCache.Set($"{nameof(BadIp)}_{ipVersion}_{ipNumber}", badIP, memoryCacheEntryOptions);

                    return badIP;
                }
            }
        }

        public async Task RemoveAsync(IpVersion ipVersion, BigInteger ipNumber)
        {
            _memoryCache.Remove($"{nameof(BadIp)}_{ipVersion}_{ipNumber}");
        }

        public async Task PurgeAsync()
        {
            _cancellationTokenManager.RemoveBadIpCancellationTokenSource("Cache_Get");
        }
    }
}