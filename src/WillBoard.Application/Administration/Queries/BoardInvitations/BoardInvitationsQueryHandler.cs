using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BoardInvitations
{
    public class BoardInvitationsQueryHandler : IRequestHandler<BoardInvitationsQuery, Result<BoardInvitationsViewModel, InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IBoardCache _boardCache;
        private readonly IInvitationCache _invitationCache;

        public BoardInvitationsQueryHandler(AccountManager accountManager, IBoardCache boardCache, IInvitationCache invitationCache)
        {
            _accountManager = accountManager;
            _boardCache = boardCache;
            _invitationCache = invitationCache;
        }

        public async Task<Result<BoardInvitationsViewModel, InternalError>> Handle(BoardInvitationsQuery request, CancellationToken cancellationToken)
        {
            var board = await _boardCache.GetAsync(request.BoardId);

            if (board == null)
            {
                return Result<BoardInvitationsViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            if (!_accountManager.CheckPermission(request.BoardId, e => e.PermissionInvitationRead))
            {
                return Result<BoardInvitationsViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var invitationCount = await _invitationCache.GetBoardCountAsync(board.BoardId);
            var pageMax = (invitationCount / 100) + 1;

            if (request.Page > pageMax)
            {
                return Result<BoardInvitationsViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            var invitationCollection = await _invitationCache.GetBoardCollectionAsync(board.BoardId, (request.Page - 1) * 100, 100);

            var result = new BoardInvitationsViewModel()
            {
                Board = board,
                InvitationCollection = invitationCollection,
                Page = request.Page,
                PageMax = pageMax
            };

            return Result<BoardInvitationsViewModel, InternalError>.ValueResult(result);
        }
    }
}