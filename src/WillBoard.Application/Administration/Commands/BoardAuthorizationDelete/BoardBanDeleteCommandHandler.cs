using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Repositories;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.BoardAuthorizationDelete
{
    public class BoardAuthorizationDeleteCommandHandler : IRequestHandler<BoardAuthorizationDeleteCommand, Status<InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IAuthorizationRepository _authorizationRepository;
        private readonly IBoardCache _boardCache;
        private readonly IAuthorizationCache _authorizationCache;

        public BoardAuthorizationDeleteCommandHandler(AccountManager accountManager, IAuthorizationRepository authorizationRepository, IBoardCache boardCache, IAuthorizationCache authorizationCache)
        {
            _accountManager = accountManager;
            _authorizationRepository = authorizationRepository;
            _boardCache = boardCache;
            _authorizationCache = authorizationCache;
        }

        public async Task<Status<InternalError>> Handle(BoardAuthorizationDeleteCommand request, CancellationToken cancellationToken)
        {
            var board = await _boardCache.GetAsync(request.BoardId);
            if (board == null)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            if (!_accountManager.CheckPermission(request.BoardId, e => e.PermissionAuthorizationDelete))
            {
                return Status<InternalError>.ErrorStatus(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var authorization = await _authorizationCache.GetBoardAsync(request.BoardId, request.AuthorizationId);
            if (authorization == null || authorization.BoardId != request.BoardId)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            await _authorizationRepository.DeleteAsync(authorization.AuthorizationId);

            await _authorizationCache.RemoveAccountCollectionAsync(authorization.AccountId);
            await _authorizationCache.RemoveBoardAsync(board.BoardId, authorization.AuthorizationId);
            await _authorizationCache.PurgeBoardCollectionAsync(board.BoardId);

            return Status<InternalError>.SuccessStatus();
        }
    }
}