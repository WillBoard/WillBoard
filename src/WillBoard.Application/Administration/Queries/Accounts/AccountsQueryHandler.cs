using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.Accounts
{
    public class AccountsQueryHandler : IRequestHandler<AccountsQuery, Result<AccountsViewModel, InternalError>>
    {
        private readonly IAccountCache _accountCache;
        private readonly AccountManager _accountManager;

        public AccountsQueryHandler(IAccountCache accountCache, AccountManager accountManager)
        {
            _accountCache = accountCache;
            _accountManager = accountManager;
        }

        public async Task<Result<AccountsViewModel, InternalError>> Handle(AccountsQuery request, CancellationToken cancellationToken)
        {
            var requestAccount = _accountManager.GetAccount();

            if (requestAccount.Type != AccountType.Administrator)
            {
                return Result<AccountsViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var accountCount = await _accountCache.GetCountAsync();
            var pageMax = (accountCount / 100) + 1;

            if (request.Page > pageMax)
            {
                return Result<AccountsViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }
            var accountCollection = await _accountCache.GetCollectionAsync((request.Page - 1) * 100, 100);

            var result = new AccountsViewModel()
            {
                AccountCollection = accountCollection,
                Page = request.Page,
                PageMax = pageMax
            };

            return Result<AccountsViewModel, InternalError>.ValueResult(result);
        }
    }
}