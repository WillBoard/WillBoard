using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Repositories;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.BoardInvitationDelete
{
    public class BoardInvitationDeleteCommandHandler : IRequestHandler<BoardInvitationDeleteCommand, Status<InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IInvitationRepository _invitationRepository;
        private readonly IBoardCache _boardCache;
        private readonly IInvitationCache _invitationCache;

        public BoardInvitationDeleteCommandHandler(AccountManager accountManager, IInvitationRepository invitationRepository, IBoardCache boardCache, IInvitationCache invitationCache)
        {
            _accountManager = accountManager;
            _invitationRepository = invitationRepository;
            _boardCache = boardCache;
            _invitationCache = invitationCache;
        }

        public async Task<Status<InternalError>> Handle(BoardInvitationDeleteCommand request, CancellationToken cancellationToken)
        {
            var board = await _boardCache.GetAsync(request.BoardId);
            if (board == null)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            if (!_accountManager.CheckPermission(request.BoardId, e => e.PermissionInvitationDelete))
            {
                return Status<InternalError>.ErrorStatus(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var invitation = await _invitationCache.GetBoardAsync(board.BoardId, request.InvitationId);
            if (invitation == null)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            await _invitationRepository.DeleteAsync(invitation.InvitationId);

            await _invitationCache.RemoveAccountAsync(invitation.AccountId, request.InvitationId);
            await _invitationCache.PurgeAccountCollectionAsync(invitation.AccountId);
            await _invitationCache.RemoveAccountCountAsync(invitation.AccountId);

            await _invitationCache.RemoveBoardAsync(board.BoardId, request.InvitationId);
            await _invitationCache.PurgeBoardCollectionAsync(board.BoardId);
            await _invitationCache.RemoveBoardCountAsync(board.BoardId);

            return Status<InternalError>.SuccessStatus();
        }
    }
}