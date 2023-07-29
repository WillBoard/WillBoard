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
    public class AccountCache : IAccountCache
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IAccountRepository _accountRepository;
        private readonly ILockManager _lockManager;
        private readonly ICancellationTokenManager _cancellationTokenManager;

        public AccountCache(IMemoryCache memoryCache, IAccountRepository accountRepository, ILockManager lockManager, ICancellationTokenManager cancellationTokenManager)
        {
            _memoryCache = memoryCache;
            _accountRepository = accountRepository;
            _lockManager = lockManager;
            _cancellationTokenManager = cancellationTokenManager;
        }

        public async Task<Account> GetAsync(Guid accountId)
        {
            if (_memoryCache.TryGetValue($"{nameof(Account)}_{accountId}", out Account account))
            {
                return account;
            }
            else
            {
                using (await _lockManager.GetAccountLockAsync($"Cache_Get_{accountId}"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(Account)}_{accountId}", out account))
                    {
                        return account;
                    }

                    account = await _accountRepository.ReadAsync(accountId);

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.High,
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(15)
                    };

                    var cancellationTokenSource = _cancellationTokenManager.GetAccountCancellationTokenSource("Cache_Get");
                    memoryCacheEntryOptions.AddExpirationToken(new CancellationChangeToken(cancellationTokenSource.Token));

                    _memoryCache.Set($"Account_{accountId}", account, memoryCacheEntryOptions);

                    return account;
                }
            }
        }

        public async Task RemoveAsync(Guid accountId)
        {
            _memoryCache.Remove($"{nameof(Account)}_{accountId}");
        }

        public async Task PurgeAsync()
        {
            _cancellationTokenManager.RemoveAccountCancellationTokenSource("Cache_Get");
        }

        public async Task<IEnumerable<Account>> GetCollectionAsync(int skip, int take)
        {
            if (_memoryCache.TryGetValue($"{nameof(Account)}_Collection_{skip}_{take}", out IEnumerable<Account> accountCollection))
            {
                return accountCollection;
            }
            else
            {
                using (await _lockManager.GetAccountLockAsync($"Cache_GetCollection_{skip}_{take}"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(Account)}_Collection_{skip}_{take}", out accountCollection))
                    {
                        return accountCollection;
                    }

                    accountCollection = await _accountRepository.ReadCollectionAsync(skip, take);

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.Normal,
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(5)
                    };

                    var cancellationTokenSource = _cancellationTokenManager.GetAccountCancellationTokenSource("Cache_GetCollection");
                    memoryCacheEntryOptions.AddExpirationToken(new CancellationChangeToken(cancellationTokenSource.Token));

                    _memoryCache.Set($"{nameof(Account)}_Collection_{skip}_{take}", accountCollection, memoryCacheEntryOptions);

                    return accountCollection;
                }
            }
        }

        public async Task PurgeCollectionAsync()
        {
            _cancellationTokenManager.RemoveAccountCancellationTokenSource("Cache_GetCollection");
        }

        public async Task<int> GetCountAsync()
        {
            if (_memoryCache.TryGetValue($"{nameof(Account)}_Count", out int count))
            {
                return count;
            }
            else
            {
                using (await _lockManager.GetAccountLockAsync($"Cache_GetCount"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(Account)}_Count", out count))
                    {
                        return count;
                    }

                    count = await _accountRepository.ReadCountAsync();

                    _memoryCache.Set($"{nameof(Account)}_Count", count, new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.Normal,
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(5)
                    });

                    return count;
                }
            }
        }

        public async Task RemoveCountAsync()
        {
            _memoryCache.Remove($"{nameof(Account)}_Count");
        }
    }
}