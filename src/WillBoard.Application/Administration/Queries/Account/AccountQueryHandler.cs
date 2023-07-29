using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.Account
{
    public class AccountQueryHandler : IRequestHandler<AccountQuery, Result<AccountViewModel, InternalError>>
    {
        private readonly IAccountCache _accountCache;
        private readonly AccountManager _accountManager;

        public AccountQueryHandler(IAccountCache accountCache, AccountManager accountManager)
        {
            _accountCache = accountCache;
            _accountManager = accountManager;
        }

        public async Task<Result<AccountViewModel, InternalError>> Handle(AccountQuery request, CancellationToken cancellationToken)
        {
            var requestAccount = _accountManager.GetAccount();

            if (request.AccountId != requestAccount.AccountId && requestAccount.Type != AccountType.Administrator)
            {
                return Result<AccountViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var account = await _accountCache.GetAsync(request.AccountId);

            if (account == null)
            {
                return Result<AccountViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            var result = new AccountViewModel()
            {
                Account = account
            };

            return Result<AccountViewModel, InternalError>.ValueResult(result);
        }
    }
}