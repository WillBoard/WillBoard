using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WillBoard.Core.Entities;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Managers;
using WillBoard.Core.Interfaces.Repositories;

namespace WillBoard.Infrastructure.Caches.Memory
{
    public class BoardCache : IBoardCache
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IBoardRepository _boardRepository;
        private readonly ILockManager _lockManager;
        private readonly ICancellationTokenManager _cancellationTokenManager;

        public BoardCache(IMemoryCache memoryCache, IBoardRepository boardRepository, ILockManager lockManager, ICancellationTokenManager cancellationTokenManager)
        {
            _memoryCache = memoryCache;
            _boardRepository = boardRepository;
            _lockManager = lockManager;
            _cancellationTokenManager = cancellationTokenManager;
        }

        public async Task<Board> GetAsync(string boardId)
        {
            var boardCollection = await GetCollectionAsync();
            var board = boardCollection.Where(e => e.BoardId == boardId).FirstOrDefault();
            return board;
        }

        public async Task<IEnumerable<Board>> GetCollectionAsync()
        {
            if (_memoryCache.TryGetValue($"{nameof(Board)}_Collection", out IEnumerable<Board> boardCollection))
            {
                return boardCollection;
            }
            else
            {
                using (await _lockManager.GetBoardLockAsync($"Cache_GetCollection"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(Board)}_Collection", out boardCollection))
                    {
                        return boardCollection;
                    }

                    boardCollection = await _boardRepository.ReadCollectionAsync();

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.Normal,
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(60 * 24 * 7)
                    };

                    _memoryCache.Set($"{nameof(Board)}_Collection", boardCollection, memoryCacheEntryOptions);

                    return boardCollection;
                }
            }
        }

        public async Task RemoveCollectionAsync()
        {
            _memoryCache.Remove($"{nameof(Board)}_Collection");
        }
    }
}