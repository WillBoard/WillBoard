using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BoardThreadPin
{
    public class BoardThreadPinQueryHandler : IRequestHandler<BoardThreadPinQuery, Result<BoardThreadPinViewModel, InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IBoardCache _boardCache;
        private readonly IPostCache _postCache;

        public BoardThreadPinQueryHandler(AccountManager accountManager, IBoardCache boardCache, IPostCache postCache)
        {
            _accountManager = accountManager;
            _boardCache = boardCache;
            _postCache = postCache;
        }

        public async Task<Result<BoardThreadPinViewModel, InternalError>> Handle(BoardThreadPinQuery request, CancellationToken cancellationToken)
        {
            var board = await _boardCache.GetAsync(request.BoardId);

            if (board == null)
            {
                return Result<BoardThreadPinViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            if (!_accountManager.CheckPermission(request.BoardId, e => e.PermissionThreadPin))
            {
                return Result<BoardThreadPinViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var post = await _postCache.GetAdaptedAsync(board, request.PostId);

            if (post == null || post.ThreadId != null)
            {
                return Result<BoardThreadPinViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            var result = new BoardThreadPinViewModel()
            {
                Post = post
            };

            return Result<BoardThreadPinViewModel, InternalError>.ValueResult(result);
        }
    }
}