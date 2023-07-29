using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BoardInvitationCreate
{
    public class BoardInvitationCreateQueryHandler : IRequestHandler<BoardInvitationCreateQuery, Result<BoardInvitationCreateViewModel, InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IBoardCache _boardCache;

        public BoardInvitationCreateQueryHandler(AccountManager accountManager, IBoardCache boardCache)
        {
            _accountManager = accountManager;
            _boardCache = boardCache;
        }

        public async Task<Result<BoardInvitationCreateViewModel, InternalError>> Handle(BoardInvitationCreateQuery request, CancellationToken cancellationToken)
        {
            var board = await _boardCache.GetAsync(request.BoardId);

            if (board == null)
            {
                return Result<BoardInvitationCreateViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            if (!_accountManager.CheckPermission(request.BoardId, e => e.PermissionInvitationCreate))
            {
                return Result<BoardInvitationCreateViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var result = new BoardInvitationCreateViewModel()
            {
                Board = board
            };

            return Result<BoardInvitationCreateViewModel, InternalError>.ValueResult(result);
        }
    }
}