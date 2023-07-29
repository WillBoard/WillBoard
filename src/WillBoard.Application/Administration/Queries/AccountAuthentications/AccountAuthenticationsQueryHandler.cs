using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.AccountAuthentications
{
    public class AccountAuthenticationsQueryHandler : IRequestHandler<AccountAuthenticationsQuery, Result<AccountAuthenticationsViewModel, InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IAccountCache _accountCache;
        private readonly IAuthenticationCache _authenticationCache;

        public AccountAuthenticationsQueryHandler(AccountManager accountManager, IAccountCache accountCache, IAuthenticationCache authenticationCache)
        {
            _accountManager = accountManager;
            _accountCache = accountCache;
            _authenticationCache = authenticationCache;
        }

        public async Task<Result<AccountAuthenticationsViewModel, InternalError>> Handle(AccountAuthenticationsQuery request, CancellationToken cancellationToken)
        {
            var requestAccount = _accountManager.GetAccount();

            if (request.AccountId != requestAccount.AccountId && requestAccount.Type != AccountType.Administrator)
            {
                return Result<AccountAuthenticationsViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var account = await _accountCache.GetAsync(request.AccountId);

            if (account == null)
            {
                return Result<AccountAuthenticationsViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            var accountCount = await _authenticationCache.GetCountAsync(account.AccountId);
            var pageMax = (accountCount / 100) + 1;

            if (request.Page > pageMax)
            {
                return Result<AccountAuthenticationsViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            var authenticationCollection = await _authenticationCache.GetCollectionAsync(account.AccountId, (request.Page - 1) * 100, 100);

            var result = new AccountAuthenticationsViewModel()
            {
                Account = account,
                AuthenticationCollection = authenticationCollection,
                Page = request.Page,
                PageMax = pageMax
            };

            return Result<AccountAuthenticationsViewModel, InternalError>.ValueResult(result);
        }
    }
}