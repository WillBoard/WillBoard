using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Repositories;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.AccountInvitationReject
{
    public class AccountInvitationRejectCommandHandler : IRequestHandler<AccountInvitationRejectCommand, Status<InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IInvitationRepository _invitationRepository;
        private readonly IAccountCache _accountCache;
        private readonly IInvitationCache _invitationCache;

        public AccountInvitationRejectCommandHandler(AccountManager accountManager, IInvitationRepository invitationRepository, IAccountCache accountCache, IInvitationCache invitationCache)
        {
            _accountManager = accountManager;
            _invitationRepository = invitationRepository;
            _accountCache = accountCache;
            _invitationCache = invitationCache;
        }

        public async Task<Status<InternalError>> Handle(AccountInvitationRejectCommand request, CancellationToken cancellationToken)
        {
            var requestAccount = _accountManager.GetAccount();
            if (request.AccountId != requestAccount.AccountId && requestAccount.Type != AccountType.Administrator)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var account = await _accountCache.GetAsync(request.AccountId);

            if (account == null)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            var invitation = await _invitationCache.GetAccountAsync(account.AccountId, request.InvitationId);
            if (invitation == null)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            await _invitationRepository.DeleteAsync(invitation.InvitationId);

            await _invitationCache.PurgeAccountAsync(invitation.AccountId);
            await _invitationCache.PurgeAccountCollectionAsync(invitation.AccountId);
            await _invitationCache.RemoveAccountCountAsync(invitation.AccountId);

            await _invitationCache.PurgeBoardAsync(invitation.BoardId);
            await _invitationCache.PurgeBoardCollectionAsync(invitation.BoardId);
            await _invitationCache.RemoveBoardCountAsync(invitation.BoardId);

            return Status<InternalError>.SuccessStatus();
        }
    }
}