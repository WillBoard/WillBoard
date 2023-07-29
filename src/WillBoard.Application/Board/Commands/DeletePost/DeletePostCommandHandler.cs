using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Classes;
using WillBoard.Core.Consts;
using WillBoard.Core.Entities;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Managers;
using WillBoard.Core.Interfaces.Providers;
using WillBoard.Core.Interfaces.Repositories;
using WillBoard.Core.Interfaces.Services;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Board.Commands.DeletePost
{
    public class DeletePostCommandHandler : IRequestHandler<DeletePostCommand, Status<InternalError>>
    {
        private readonly BoardManager _boardManager;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IPostRepository _postRepository;
        private readonly IPostCache _postCache;
        private readonly IPostService _postService;
        private readonly ILockManager _lockManager;
        private readonly ISynchronizationService _synchronizationService;
        private readonly IStorageService _storageService;

        public DeletePostCommandHandler(
            BoardManager boardManager,
            IDateTimeProvider dateTimeProvider,
            IPostRepository postRepository,
            IPostCache postCache,
            IPostService postService,
            ILockManager lockManager,
            ISynchronizationService synchronizationService,
            IStorageService storageService)
        {
            _boardManager = boardManager;
            _dateTimeProvider = dateTimeProvider;
            _postRepository = postRepository;
            _postCache = postCache;
            _postService = postService;
            _lockManager = lockManager;
            _synchronizationService = synchronizationService;
            _storageService = storageService;
        }

        public async Task<Status<InternalError>> Handle(DeletePostCommand request, CancellationToken cancellationToken)
        {
            var board = _boardManager.GetBoard();

            var adaptedPost = await _postCache.GetAdaptedAsync(board, request.PostId);

            var adaptedValidation = Validate(request, board, adaptedPost);

            if (!adaptedValidation.Success)
            {
                return adaptedValidation;
            }

            var postDeleteCollection = new List<Post>();

            using (await _lockManager.GetPostLockAsync($"Database_{board.BoardId}"))
            {
                var post = await _postRepository.ReadAsync(board.BoardId, request.PostId);

                var validation = Validate(request, board, post);

                if (!validation.Success)
                {
                    return validation;
                }

                if (post.ThreadId == null)
                {
                    var replyCollection = await _postRepository.ReadThreadReplyCollectionAsync(board.BoardId, post.PostId);
                    postDeleteCollection.Add(post);
                    postDeleteCollection.AddRange(replyCollection);
                }
                else
                {
                    postDeleteCollection.Add(post);
                }

                await _postService.DeleteCollectionAsync(postDeleteCollection);

                foreach (var postDelete in postDeleteCollection)
                {
                    if (post.File == true && post.FileDeleted == false)
                    {
                        _storageService.DeleteSourceFile(board.BoardId, postDelete.FileName);
                        _storageService.DeletePreviewFile(board.BoardId, postDelete.FilePreviewName);
                    }

                    await _postCache.RemoveAdaptedAsync(board, postDelete.PostId);
                }
            }

            if (board.SynchronizationBoardAvailability || board.SynchronizationThreadAvailability)
            {
                foreach (var postDelete in postDeleteCollection)
                {
                    var synchronizationMessage = new SynchronizationMessage()
                    {
                        Event = SynchronizationEvent.Delete,
                        Data = new
                        {
                            boardId = postDelete.BoardId,
                            postId = postDelete.PostId,
                            threadId = postDelete.ThreadId
                        }
                    };

                    var administrationSynchronizationMessage = new AdministrationSynchronizationMessage()
                    {
                        Event = SynchronizationEvent.Delete,
                        Data = new
                        {
                            boardId = postDelete.BoardId,
                            postId = postDelete.PostId,
                            threadId = postDelete.ThreadId
                        }
                    };

                    _synchronizationService.Notify(synchronizationMessage, board.BoardId);
                    _synchronizationService.Notify(administrationSynchronizationMessage, board.BoardId);
                }
            }

            return Status<InternalError>.SuccessStatus();
        }

        public Status<InternalError> Validate(DeletePostCommand request, Core.Entities.Board board, Post post)
        {
            if (post == null)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            if (post.ThreadId == null)
            {
                if (board.ThreadDeleteAvailability == false)
                {
                    return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorThreadDeleteAvailability));
                }

                if (_dateTimeProvider.UtcNow < post.Creation.AddSeconds(board.ThreadDeleteTimeMin))
                {
                    return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorThreadDeleteTimeMin));
                }

                if (_dateTimeProvider.UtcNow > post.Creation.AddSeconds(board.ThreadDeleteTimeMax))
                {
                    return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorThreadDeleteTimeMax));
                }
            }
            else
            {
                if (board.ReplyDeleteAvailability == false)
                {
                    return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.EroorReplyDeleteAvailability));
                }

                if (_dateTimeProvider.UtcNow < post.Creation.AddSeconds(board.ReplyDeleteTimeMin))
                {
                    return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReplyDeleteTimeMin));
                }

                if (_dateTimeProvider.UtcNow > post.Creation.AddSeconds(board.ReplyDeleteTimeMax))
                {
                    return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReplyDeleteTimeMax));
                }
            }

            if (post.Password != request.Password)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorPassword));
            }

            return Status<InternalError>.SuccessStatus();
        }
    }
}