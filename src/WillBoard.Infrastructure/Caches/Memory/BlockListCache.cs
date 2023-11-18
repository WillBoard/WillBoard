using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WillBoard.Core.Classes;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Managers;
using WillBoard.Core.Interfaces.Services;

namespace WillBoard.Infrastructure.Caches.Memory
{
    public class BlockListCache : IBlockListCache
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IBlockListService _blockListService;
        private readonly ILockManager _lockManager;
        private readonly ICancellationTokenManager _cancellationTokenManager;

        public BlockListCache(IMemoryCache memoryCache, IBlockListService blockListService, ILockManager lockManager, ICancellationTokenManager cancellationTokenManager)
        {
            _memoryCache = memoryCache;
            _blockListService = blockListService;
            _lockManager = lockManager;
            _cancellationTokenManager = cancellationTokenManager;
        }

        public async Task<bool> GetBoardDnsBlockListIpVersion4Async(string boardId, UInt32 ipNumber, IEnumerable<BlockList> blockListCollection)
        {
            if (_memoryCache.TryGetValue($"{nameof(BlockList)}_BoardDnsBlockListIpVersion4_{boardId}_{ipNumber}", out bool blockListResult))
            {
                return blockListResult;
            }
            else
            {
                using (await _lockManager.GetBlockListLockAsync($"Cache_GetBoardDnsBlockListIpVersion4_{boardId}_{ipNumber}"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(BlockList)}_BoardDnsBlockListIpVersion4_{boardId}_{ipNumber}", out blockListResult))
                    {
                        return blockListResult;
                    }

                    blockListResult = await _blockListService.CheckDnsBlockListIpVersion4Async(ipNumber, blockListCollection);

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.High,
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddHours(12)
                    };

                    var cancellationTokenSource = _cancellationTokenManager.GetBlockListCancellationTokenSource("Cache_GetBoardDnsBlockListIpVersion4");
                    memoryCacheEntryOptions.AddExpirationToken(new CancellationChangeToken(cancellationTokenSource.Token));

                    _memoryCache.Set($"{nameof(BlockList)}_BoardDnsBlockListIpVersion4_{boardId}_{ipNumber}", blockListResult, memoryCacheEntryOptions);

                    return blockListResult;
                }
            }
        }

        public async Task RemoveBoardDnsBlockListIpVersion4Async(string boardId, UInt32 ipNumber)
        {
            _memoryCache.Remove($"{nameof(BlockList)}_BoardDnsBlockListIpVersion4_{boardId}_{ipNumber}");
        }

        public async Task PurgeBoardDnsBlockListIpVersion4Async(string boardId)
        {
            _cancellationTokenManager.RemoveBlockListCancellationTokenSource("Cache_GetBoardDnsBlockListIpVersion4");
        }

        public async Task<bool> GetBoardDnsBlockListIpVersion6Async(string boardId, UInt128 ipNumber, IEnumerable<BlockList> blockListCollection)
        {
            if (_memoryCache.TryGetValue($"{nameof(BlockList)}_BoardDnsBlockListIpVersion6_{boardId}_{ipNumber}", out bool blockListResult))
            {
                return blockListResult;
            }
            else
            {
                using (await _lockManager.GetBlockListLockAsync($"Cache_GetBoardDnsBlockListIpVersion6_{boardId}_{ipNumber}"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(BlockList)}_BoardDnsBlockListIpVersion6_{boardId}_{ipNumber}", out blockListResult))
                    {
                        return blockListResult;
                    }

                    blockListResult = await _blockListService.CheckDnsBlockListIpVersion6Async(ipNumber, blockListCollection);

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.High,
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddHours(12)
                    };

                    var cancellationTokenSource = _cancellationTokenManager.GetBlockListCancellationTokenSource("Cache_GetBoardDnsBlockListIpVersion6");
                    memoryCacheEntryOptions.AddExpirationToken(new CancellationChangeToken(cancellationTokenSource.Token));

                    _memoryCache.Set($"{nameof(BlockList)}_BoardDnsBlockListIpVersion6_{boardId}_{ipNumber}", blockListResult, memoryCacheEntryOptions);

                    return blockListResult;
                }
            }
        }

        public async Task RemoveBoardDnsBlockListIpVersion6Async(string boardId, UInt128 ipNumber)
        {
            _memoryCache.Remove($"{nameof(BlockList)}_BoardDnsBlockListIpVersion6_{boardId}_{ipNumber}");
        }

        public async Task PurgeBoardDnsBlockListIpVersion6Async(string boardId)
        {
            _cancellationTokenManager.RemoveBlockListCancellationTokenSource("Cache_GetBoardDnsBlockListIpVersion6");
        }

        public async Task<bool> GetBoardApiBlockListIpVersion4Async(string boardId, UInt32 ipNumber, IEnumerable<BlockList> blockListCollection)
        {
            if (_memoryCache.TryGetValue($"{nameof(BlockList)}_BoardApiBlockListIpVersion4_{boardId}_{ipNumber}", out bool blockListResult))
            {
                return blockListResult;
            }
            else
            {
                using (await _lockManager.GetBlockListLockAsync($"Cache_GetBoardApiBlockListIpVersion4_{boardId}_{ipNumber}"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(BlockList)}_BoardApiBlockListIpVersion4_{boardId}_{ipNumber}", out blockListResult))
                    {
                        return blockListResult;
                    }

                    blockListResult = await _blockListService.CheckApiBlockListIpVersion4Async(ipNumber, blockListCollection);

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.High,
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddHours(12)
                    };

                    var cancellationTokenSource = _cancellationTokenManager.GetBlockListCancellationTokenSource("Cache_GetBoardApiBlockListIpVersion4");
                    memoryCacheEntryOptions.AddExpirationToken(new CancellationChangeToken(cancellationTokenSource.Token));

                    _memoryCache.Set($"{nameof(BlockList)}_BoardApiBlockListIpVersion4_{boardId}_{ipNumber}", blockListResult, memoryCacheEntryOptions);

                    return blockListResult;
                }
            }
        }

        public async Task RemoveBoardApiBlockListIpVersion4Async(string boardId, UInt32 ipNumber)
        {
            _memoryCache.Remove($"{nameof(BlockList)}_BoardApiBlockListIpVersion4_{boardId}_{ipNumber}");
        }

        public async Task PurgeBoardApiBlockListIpVersion4Async(string boardId)
        {
            _cancellationTokenManager.RemoveBlockListCancellationTokenSource("Cache_GetBoardApiBlockListIpVersion4");
        }

        public async Task<bool> GetBoardApiBlockListIpVersion6Async(string boardId, UInt128 ipNumber, IEnumerable<BlockList> blockListCollection)
        {
            if (_memoryCache.TryGetValue($"{nameof(BlockList)}_BoardApiBlockListIpVersion6_{boardId}_{ipNumber}", out bool blockListResult))
            {
                return blockListResult;
            }
            else
            {
                using (await _lockManager.GetBlockListLockAsync($"Cache_GetBoardApiBlockListIpVersion6_{boardId}_{ipNumber}"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(BlockList)}_BoardApiBlockListIpVersion6_{boardId}_{ipNumber}", out blockListResult))
                    {
                        return blockListResult;
                    }

                    blockListResult = await _blockListService.CheckApiBlockListIpVersion6Async(ipNumber, blockListCollection);

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.High,
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddHours(12)
                    };

                    var cancellationTokenSource = _cancellationTokenManager.GetBlockListCancellationTokenSource("Cache_GetBoardApiBlockListIpVersion6");
                    memoryCacheEntryOptions.AddExpirationToken(new CancellationChangeToken(cancellationTokenSource.Token));

                    _memoryCache.Set($"{nameof(BlockList)}_BoardApiBlockListIpVersion6_{boardId}_{ipNumber}", blockListResult, memoryCacheEntryOptions);

                    return blockListResult;
                }
            }
        }

        public async Task RemoveBoardApiBlockListIpVersion6Async(string boardId, UInt128 ipNumber)
        {
            _memoryCache.Remove($"{nameof(BlockList)}_BoardApiBlockListIpVersion6_{boardId}_{ipNumber}");
        }

        public async Task PurgeBoardApiBlockListIpVersion6Async(string boardId)
        {
            _cancellationTokenManager.RemoveBlockListCancellationTokenSource("Cache_GetBoardApiBlockListIpVersion6");
        }
    }
}