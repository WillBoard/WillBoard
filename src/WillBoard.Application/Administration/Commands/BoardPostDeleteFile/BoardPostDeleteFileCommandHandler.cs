using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Repositories;
using WillBoard.Core.Interfaces.Services;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.BoardPostDeleteFile
{
    public class BoardPostDeleteFileCommandHandler : IRequestHandler<BoardPostDeleteFileCommand, Status<InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IPostRepository _postRepository;
        private readonly IBoardCache _boardCache;
        private readonly IPostCache _postCache;
        private readonly IStorageService _storageService;

        public BoardPostDeleteFileCommandHandler(AccountManager accountManager, IPostRepository postRepository, IBoardCache boardCache, IPostCache postCache, IStorageService storageService)
        {
            _accountManager = accountManager;
            _postRepository = postRepository;
            _boardCache = boardCache;
            _postCache = postCache;
            _storageService = storageService;
        }

        public async Task<Status<InternalError>> Handle(BoardPostDeleteFileCommand request, CancellationToken cancellationToken)
        {
            var board = await _boardCache.GetAsync(request.BoardId);

            if (board == null)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            if (!_accountManager.CheckPermission(request.BoardId, e => e.PermissionPostDelete))
            {
                return Status<InternalError>.ErrorStatus(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var post = await _postCache.GetAdaptedAsync(board, request.PostId);

            if (post == null)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            if (post.File == false || post.FileDeleted == true)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(403, TranslationKey.ErrorNotFoundDeleteFile));
            }

            await _postRepository.UpdateFileDeleted(board.BoardId, post.PostId, true);

            _storageService.DeleteSourceFile(board.BoardId, post.FileName);
            _storageService.DeletePreviewFile(board.BoardId, post.FilePreviewName);

            await _postCache.UpdateAdaptedFileDeletedAsync(board, post.PostId, true);

            return Status<InternalError>.SuccessStatus();
        }
    }
}