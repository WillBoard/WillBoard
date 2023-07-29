using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Classes;
using WillBoard.Core.Entities;
using WillBoard.Core.Enums;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Managers;
using WillBoard.Core.Interfaces.Repositories;
using WillBoard.Core.Interfaces.Services;

namespace WillBoard.Infrastructure.BackgroundServices
{
    public class ExcessiveBackgroundService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly ILockManager _lockManager;
        private readonly IPostRepository _postRepository;
        private readonly IBoardCache _boardCache;
        private readonly IPostCache _postCache;
        private readonly ISynchronizationService _synchronizationService;
        private readonly IStorageService _storageService;
        private readonly IPostService _postService;

        public ExcessiveBackgroundService(
            ILogger<ExcessiveBackgroundService> logger,
            ILockManager lockManager,
            IPostRepository postRepository,
            IBoardCache boardCache,
            IPostCache postCache,
            ISynchronizationService synchronizationService,
            IStorageService storageService,
            IPostService postService)
        {
            _logger = logger;
            _lockManager = lockManager;
            _postRepository = postRepository;
            _boardCache = boardCache;
            _postCache = postCache;
            _synchronizationService = synchronizationService;
            _storageService = storageService;
            _postService = postService;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(ExecuteAsync)} in {nameof(ExcessiveBackgroundService)} started.");

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(900_000, cancellationToken);

                    var boardCollection = await _boardCache.GetCollectionAsync();
                    foreach (var board in boardCollection)
                    {
                        await DeleteExpiredExcessiveAsync(board);
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, $"Exception occured during {nameof(ExecuteAsync)} in {nameof(ExcessiveBackgroundService)}.");
                }
            }

            _logger.LogWarning($"{nameof(ExecuteAsync)} in {nameof(ExcessiveBackgroundService)} finished.");
        }

        private async Task DeleteExpiredExcessiveAsync(Board board)
        {
            try
            {
                var postExcessiveCollection = Enumerable.Empty<Post>();

                using (await _lockManager.GetPostLockAsync($"Database_{board.BoardId}"))
                {
                    postExcessiveCollection = await _postRepository.ReadExpiredExcessiveCollectionAsync(board.BoardId, board.ThreadExcessiveTimeMax);

                    if (postExcessiveCollection.Any())
                    {
                        await _postService.DeleteCollectionAsync(postExcessiveCollection);
                    }

                    foreach (var postExcessive in postExcessiveCollection)
                    {
                        _storageService.DeleteSourceFile(board.BoardId, postExcessive.FileName);
                        _storageService.DeletePreviewFile(board.BoardId, postExcessive.FilePreviewName);

                        await _postCache.RemoveAdaptedAsync(board, postExcessive.PostId);
                    }
                }

                foreach (var postExcessive in postExcessiveCollection)
                {
                    var synchronizationMessageExcessive = new SynchronizationMessage()
                    {
                        Event = SynchronizationEvent.Delete,
                        Data = new
                        {
                            boardId = postExcessive.BoardId,
                            postId = postExcessive.PostId,
                            threadId = postExcessive.ThreadId
                        }
                    };

                    var administrationSynchronizationMessageExcessive = new AdministrationSynchronizationMessage()
                    {
                        Event = SynchronizationEvent.Delete,
                        Data = new
                        {
                            boardId = postExcessive.BoardId,
                            postId = postExcessive.PostId,
                            threadId = postExcessive.ThreadId
                        }
                    };

                    _synchronizationService.Notify(synchronizationMessageExcessive, board.BoardId);
                    _synchronizationService.Notify(administrationSynchronizationMessageExcessive, board.BoardId);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Exception occured during {nameof(DeleteExpiredExcessiveAsync)} of {board.BoardId} board.");
            }
        }
    }
}