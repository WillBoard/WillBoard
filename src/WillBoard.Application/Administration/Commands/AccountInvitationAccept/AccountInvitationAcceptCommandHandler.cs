using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Entities;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Providers;
using WillBoard.Core.Interfaces.Repositories;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.AccountInvitationAccept
{
    public class AccountInvitationAcceptCommandHandler : IRequestHandler<AccountInvitationAcceptCommand, Status<InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IInvitationRepository _invitationRepository;
        private readonly IAuthorizationRepository _authorizationRepository;
        private readonly IAccountCache _accountCache;
        private readonly IInvitationCache _invitationCache;
        private readonly IAuthorizationCache _authorizationCache;

        public AccountInvitationAcceptCommandHandler(AccountManager accountManager, IDateTimeProvider dateTimeProvider, IInvitationRepository invitationRepository, IAuthorizationRepository authorizationRepository, IAccountCache accountCache, IInvitationCache invitationCache, IAuthorizationCache authorizationCache)
        {
            _accountManager = accountManager;
            _dateTimeProvider = dateTimeProvider;
            _invitationRepository = invitationRepository;
            _authorizationRepository = authorizationRepository;
            _accountCache = accountCache;
            _invitationCache = invitationCache;
            _authorizationCache = authorizationCache;
        }

        public async Task<Status<InternalError>> Handle(AccountInvitationAcceptCommand request, CancellationToken cancellationToken)
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

            var authorization = new Authorization()
            {
                AuthorizationId = Guid.NewGuid(),
                AccountId = invitation.AccountId,
                BoardId = invitation.BoardId,
                Creation = _dateTimeProvider.UtcNow,
                Name = ""
            };

            await _authorizationRepository.CreateAsync(authorization);

            await _invitationCache.PurgeAccountAsync(invitation.AccountId);
            await _invitationCache.PurgeAccountCollectionAsync(invitation.AccountId);
            await _invitationCache.RemoveAccountCountAsync(invitation.AccountId);

            await _invitationCache.PurgeBoardAsync(invitation.BoardId);
            await _invitationCache.PurgeBoardCollectionAsync(invitation.BoardId);
            await _invitationCache.RemoveBoardCountAsync(invitation.BoardId);

            await _authorizationCache.PurgeBoardAsync(authorization.BoardId);
            await _authorizationCache.PurgeBoardCollectionAsync(authorization.BoardId);
            await _authorizationCache.RemoveBoardCountAsync(authorization.BoardId);

            return Status<InternalError>.SuccessStatus();
        }
    }
}