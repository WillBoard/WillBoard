using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BoardAuthorizations
{
    public class BoardAuthorizationsQueryHandler : IRequestHandler<BoardAuthorizationsQuery, Result<BoardAuthorizationsViewModel, InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IBoardCache _boardCache;
        private readonly IAuthorizationCache _authorizationCache;

        public BoardAuthorizationsQueryHandler(AccountManager accountManager, IBoardCache boardCache, IAuthorizationCache authorizationCache)
        {
            _accountManager = accountManager;
            _boardCache = boardCache;
            _authorizationCache = authorizationCache;
        }

        public async Task<Result<BoardAuthorizationsViewModel, InternalError>> Handle(BoardAuthorizationsQuery request, CancellationToken cancellationToken)
        {
            var board = await _boardCache.GetAsync(request.BoardId);

            if (board == null)
            {
                return Result<BoardAuthorizationsViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            if (!_accountManager.CheckPermission(request.BoardId, e => e.PermissionAuthorizationRead))
            {
                return Result<BoardAuthorizationsViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var authorizationCount = await _authorizationCache.GetBoardCountAsync(board.BoardId);
            var pageMax = (authorizationCount / 100) + 1;

            if (request.Page > pageMax)
            {
                return Result<BoardAuthorizationsViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            var invitationCollection = await _authorizationCache.GetBoardCollectionAsync(board.BoardId, (request.Page - 1) * 100, 100);

            var result = new BoardAuthorizationsViewModel()
            {
                Board = board,
                AuthorizationCollection = invitationCollection,
                Page = request.Page,
                PageMax = pageMax
            };

            return Result<BoardAuthorizationsViewModel, InternalError>.ValueResult(result);
        }
    }
}