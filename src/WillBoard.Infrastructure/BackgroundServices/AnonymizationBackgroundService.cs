using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Entities;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Managers;
using WillBoard.Core.Interfaces.Repositories;
using WillBoard.Core.Interfaces.Services;

namespace WillBoard.Infrastructure.BackgroundServices
{
    public class AnonymizationBackgroundService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly ILockManager _lockManager;
        private readonly IPostRepository _postRepository;
        private readonly IBoardCache _boardCache;
        private readonly IPostCache _postCache;
        private readonly ISynchronizationService _synchronizationService;
        private readonly IStorageService _storageService;
        private readonly IPostService _postService;

        public AnonymizationBackgroundService(
            ILogger<AnonymizationBackgroundService> logger,
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
            _logger.LogInformation($"{nameof(ExecuteAsync)} in {nameof(AnonymizationBackgroundService)} started.");

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(86_400_000, cancellationToken);

                    var boardCollection = await _boardCache.GetCollectionAsync();
                    foreach (var board in boardCollection)
                    {
                        if (!board.AnonymizationAvailability)
                        {
                            continue;
                        }

                        await AnonymizeBoardAsync(board);
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, $"Exception occured during {nameof(ExecuteAsync)} in {nameof(AnonymizationBackgroundService)}.");
                }
            }

            _logger.LogWarning($"{nameof(ExecuteAsync)} in {nameof(AnonymizationBackgroundService)} finished.");
        }

        private async Task AnonymizeBoardAsync(Board board)
        {
            try
            {
                using (await _lockManager.GetPostLockAsync($"Database_{board.BoardId}"))
                {
                    var postCollection = await _postRepository.ReadUnanonymizedCollectionAsync(board.BoardId, board.AnonymizationTimeMax);

                    if (postCollection.Any())
                    {
                        await _postRepository.AnonymizeCollectionAsync(postCollection);
                    }

                    await _postCache.PurgeAdaptedCollectionAsync(board.BoardId);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Exception occured during {nameof(AnonymizeBoardAsync)} of {board.BoardId} board.");
            }
        }
    }
}