using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BoardThreadExcessive
{
    public class BoardThreadExcessiveQueryHandler : IRequestHandler<BoardThreadExcessiveQuery, Result<BoardThreadExcessiveViewModel, InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IBoardCache _boardCache;
        private readonly IPostCache _postCache;

        public BoardThreadExcessiveQueryHandler(AccountManager accountManager, IBoardCache boardCache, IPostCache postCache)
        {
            _accountManager = accountManager;
            _boardCache = boardCache;
            _postCache = postCache;
        }

        public async Task<Result<BoardThreadExcessiveViewModel, InternalError>> Handle(BoardThreadExcessiveQuery request, CancellationToken cancellationToken)
        {
            var board = await _boardCache.GetAsync(request.BoardId);

            if (board == null)
            {
                return Result<BoardThreadExcessiveViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            if (!_accountManager.CheckPermission(request.BoardId, e => e.PermissionThreadExcessive))
            {
                return Result<BoardThreadExcessiveViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var post = await _postCache.GetAdaptedAsync(board, request.PostId);

            if (post == null || post.ThreadId != null)
            {
                return Result<BoardThreadExcessiveViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            var result = new BoardThreadExcessiveViewModel()
            {
                Post = post
            };

            return Result<BoardThreadExcessiveViewModel, InternalError>.ValueResult(result);
        }
    }
}