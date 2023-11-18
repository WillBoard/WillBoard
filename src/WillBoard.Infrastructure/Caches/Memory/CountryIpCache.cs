using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using System;
using System.Threading.Tasks;
using WillBoard.Core.Entities;
using WillBoard.Core.Enums;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Managers;
using WillBoard.Core.Interfaces.Repositories;

namespace WillBoard.Infrastructure.Caches.Memory
{
    public class CountryIpCache : ICountryIpCache
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ICountryIpRepository _countryIPRepository;
        private readonly ILockManager _lockManager;
        private readonly ICancellationTokenManager _cancellationTokenManager;

        public CountryIpCache(IMemoryCache memoryCache, ICountryIpRepository countryIPRepository, ILockManager lockManager, ICancellationTokenManager cancellationTokenManager)
        {
            _memoryCache = memoryCache;
            _countryIPRepository = countryIPRepository;
            _lockManager = lockManager;
            _cancellationTokenManager = cancellationTokenManager;
        }

        public async Task<CountryIp> GetAsync(IpVersion ipVersion, UInt128 ipNumber)
        {
            if (_memoryCache.TryGetValue($"{nameof(CountryIp)}_{ipVersion}_{ipNumber}", out CountryIp countryIP))
            {
                return countryIP;
            }
            else
            {
                using (await _lockManager.GetCountryIpLockAsync($"Cache_Get_{ipVersion}_{ipNumber}"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(CountryIp)}_{ipVersion}_{ipNumber}", out countryIP))
                    {
                        return countryIP;
                    }

                    countryIP = await _countryIPRepository.ReadAsync(ipVersion, ipNumber);

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.High,
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddHours(3)
                    };

                    var cancellationTokenSource = _cancellationTokenManager.GetCountryIpCancellationTokenSource("Cache_Get");
                    memoryCacheEntryOptions.AddExpirationToken(new CancellationChangeToken(cancellationTokenSource.Token));

                    _memoryCache.Set($"{nameof(CountryIp)}_{ipVersion}_{ipNumber}", countryIP, memoryCacheEntryOptions);

                    return countryIP;
                }
            }
        }

        public async Task RemoveAsync(IpVersion ipVersion, UInt128 ipNumber)
        {
            _memoryCache.Remove($"{nameof(CountryIp)}_{ipVersion}_{ipNumber}");
        }

        public async Task PurgeAsync()
        {
            _cancellationTokenManager.RemoveCountryIpCancellationTokenSource("Cache_Get");
        }
    }
}