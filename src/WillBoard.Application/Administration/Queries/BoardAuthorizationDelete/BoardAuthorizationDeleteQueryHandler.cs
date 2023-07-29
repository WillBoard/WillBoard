using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BoardAuthorizationDelete
{
    public class BoardAuthorizationDeleteQueryHandler : IRequestHandler<BoardAuthorizationDeleteQuery, Result<BoardAuthorizationDeleteViewModel, InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IBoardCache _boardCache;
        private readonly IAuthorizationCache _authorizationCache;

        public BoardAuthorizationDeleteQueryHandler(AccountManager accountManager, IBoardCache boardCache, IAuthorizationCache authorizationCache)
        {
            _accountManager = accountManager;
            _boardCache = boardCache;
            _authorizationCache = authorizationCache;
        }

        public async Task<Result<BoardAuthorizationDeleteViewModel, InternalError>> Handle(BoardAuthorizationDeleteQuery request, CancellationToken cancellationToken)
        {
            var board = await _boardCache.GetAsync(request.BoardId);

            if (board == null)
            {
                return Result<BoardAuthorizationDeleteViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            if (!_accountManager.CheckPermission(request.BoardId, e => e.PermissionAuthorizationDelete))
            {
                return Result<BoardAuthorizationDeleteViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var authorization = await _authorizationCache.GetBoardAsync(request.BoardId, request.AuthorizationId);

            if (authorization == null || authorization.BoardId != request.BoardId)
            {
                return Result<BoardAuthorizationDeleteViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            var result = new BoardAuthorizationDeleteViewModel()
            {
                Authorization = authorization
            };

            return Result<BoardAuthorizationDeleteViewModel, InternalError>.ValueResult(result);
        }
    }
}