using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Entities;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Managers;
using WillBoard.Core.Interfaces.Providers;
using WillBoard.Core.Interfaces.Repositories;
using WillBoard.Core.Interfaces.Services;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Board.Commands.DeleteFile
{
    public class DeleteFileCommandHandler : IRequestHandler<DeleteFileCommand, Status<InternalError>>
    {
        private readonly BoardManager _boardManager;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IPostRepository _postRepository;
        private readonly IPostCache _postCache;
        private readonly ILockManager _lockManager;
        private readonly IStorageService _storageService;

        public DeleteFileCommandHandler(BoardManager boardManager, IDateTimeProvider dateTimeProvider, IPostRepository postRepository, IPostCache postCache, ILockManager lockManager, IStorageService storageService)
        {
            _boardManager = boardManager;
            _dateTimeProvider = dateTimeProvider;
            _postRepository = postRepository;
            _postCache = postCache;
            _lockManager = lockManager;
            _storageService = storageService;
        }

        public async Task<Status<InternalError>> Handle(DeleteFileCommand request, CancellationToken cancellationToken)
        {
            var board = _boardManager.GetBoard();

            var adaptedPost = await _postCache.GetAdaptedAsync(board, request.PostId);

            var adaptedValidation = Validate(request, board, adaptedPost);

            if (!adaptedValidation.Success)
            {
                return adaptedValidation;
            }

            using (await _lockManager.GetPostLockAsync($"Database_{board.BoardId}"))
            {
                var post = await _postRepository.ReadAsync(board.BoardId, request.PostId);

                var validation = Validate(request, board, post);

                if (!validation.Success)
                {
                    return validation;
                }

                await _postRepository.UpdateFileDeleted(board.BoardId, adaptedPost.PostId, true);

                _storageService.DeleteSourceFile(board.BoardId, adaptedPost.FileName);
                _storageService.DeletePreviewFile(board.BoardId, adaptedPost.FilePreviewName);

                await _postCache.UpdateAdaptedFileDeletedAsync(board, adaptedPost.PostId, true);

                return Status<InternalError>.SuccessStatus();
            }
        }

        public Status<InternalError> Validate(DeleteFileCommand request, Core.Entities.Board board, Post post)
        {
            if (post == null)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            if (post.File == false || post.FileDeleted == true)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorNotFoundDeleteFile));
            }

            if (post.IsThread())
            {
                if (board.ThreadFileDeleteAvailability == false)
                {
                    return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorThreadFileDeleteAvailability));
                }

                if (_dateTimeProvider.UtcNow < post.Creation.AddSeconds(board.ThreadFileDeleteTimeMin))
                {
                    return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorThreadFileDeleteTimeMin));
                }

                if (_dateTimeProvider.UtcNow > post.Creation.AddSeconds(board.ThreadFileDeleteTimeMax))
                {
                    return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorThreadFileDeleteTimeMax));
                }
            }
            else
            {
                if (board.ReplyFileDeleteAvailability == false)
                {
                    return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReplyFileDeleteAvailability));
                }

                if (_dateTimeProvider.UtcNow < post.Creation.AddSeconds(board.ReplyFileDeleteTimeMin))
                {
                    return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReplyFileDeleteTimeMin));
                }

                if (_dateTimeProvider.UtcNow > post.Creation.AddSeconds(board.ReplyFileDeleteTimeMax))
                {
                    return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorReplyFileDeleteTimeMax));
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