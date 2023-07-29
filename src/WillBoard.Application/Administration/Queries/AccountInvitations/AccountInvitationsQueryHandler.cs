using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.AccountInvitations
{
    public class AccountInvitationsQueryHandler : IRequestHandler<AccountInvitationsQuery, Result<AccountInvitationsViewModel, InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IAccountCache _accountCache;
        private readonly IInvitationCache _invitationCache;

        public AccountInvitationsQueryHandler(AccountManager accountManager, IAccountCache accountCache, IInvitationCache invitationCache)
        {
            _accountManager = accountManager;
            _accountCache = accountCache;
            _invitationCache = invitationCache;
        }

        public async Task<Result<AccountInvitationsViewModel, InternalError>> Handle(AccountInvitationsQuery request, CancellationToken cancellationToken)
        {
            var requestAccount = _accountManager.GetAccount();
            if (request.AccountId != requestAccount.AccountId && requestAccount.Type != AccountType.Administrator)
            {
                return Result<AccountInvitationsViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var account = await _accountCache.GetAsync(request.AccountId);

            if (account == null)
            {
                return Result<AccountInvitationsViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            var invitationCount = await _invitationCache.GetAccountCountAsync(account.AccountId);
            var pageMax = (invitationCount / 100) + 1;

            if (request.Page > pageMax)
            {
                return Result<AccountInvitationsViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            var invitationCollection = await _invitationCache.GetAccountCollectionAsync(account.AccountId, (request.Page - 1) * 100, 100);

            var result = new AccountInvitationsViewModel()
            {
                Account = account,
                InvitationCollection = invitationCollection,
                Page = request.Page,
                PageMax = pageMax
            };

            return Result<AccountInvitationsViewModel, InternalError>.ValueResult(result);
        }
    }
}