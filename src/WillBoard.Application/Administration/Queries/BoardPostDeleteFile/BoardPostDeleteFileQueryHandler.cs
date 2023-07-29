using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BoardPostDeleteFile
{
    public class BoardPostDeleteFileQueryHandler : IRequestHandler<BoardPostDeleteFileQuery, Result<BoardPostDeleteFileViewModel, InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IBoardCache _boardCache;
        private readonly IPostCache _postCache;

        public BoardPostDeleteFileQueryHandler(AccountManager accountManager, IBoardCache boardCache, IPostCache postCache)
        {
            _accountManager = accountManager;
            _boardCache = boardCache;
            _postCache = postCache;
        }

        public async Task<Result<BoardPostDeleteFileViewModel, InternalError>> Handle(BoardPostDeleteFileQuery request, CancellationToken cancellationToken)
        {
            var board = await _boardCache.GetAsync(request.BoardId);

            if (board == null)
            {
                return Result<BoardPostDeleteFileViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            if (!_accountManager.CheckPermission(request.BoardId, e => e.PermissionPostEdit))
            {
                return Result<BoardPostDeleteFileViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var post = await _postCache.GetAdaptedAsync(board, request.PostId);

            if (post == null)
            {
                return Result<BoardPostDeleteFileViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            if (post.File == false || post.FileDeleted == true)
            {
                return Result<BoardPostDeleteFileViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            var result = new BoardPostDeleteFileViewModel()
            {
                Post = post
            };

            return Result<BoardPostDeleteFileViewModel, InternalError>.ValueResult(result);
        }
    }
}