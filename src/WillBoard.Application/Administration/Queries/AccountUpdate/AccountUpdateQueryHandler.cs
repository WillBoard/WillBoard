using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.AccountUpdate
{
    public class AccountUpdateQueryHandler : IRequestHandler<AccountUpdateQuery, Result<AccountUpdateViewModel, InternalError>>
    {
        private readonly IAccountCache _accountCache;
        private readonly AccountManager _accountManager;

        public AccountUpdateQueryHandler(IAccountCache accountCache, AccountManager accountManager)
        {
            _accountCache = accountCache;
            _accountManager = accountManager;
        }

        public async Task<Result<AccountUpdateViewModel, InternalError>> Handle(AccountUpdateQuery request, CancellationToken cancellationToken)
        {
            var requestAccount = _accountManager.GetAccount();

            if (requestAccount.Type != AccountType.Administrator)
            {
                return Result<AccountUpdateViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var account = await _accountCache.GetAsync(request.AccountId);

            if (account == null)
            {
                return Result<AccountUpdateViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            var result = new AccountUpdateViewModel()
            {
                Account = account
            };

            return Result<AccountUpdateViewModel, InternalError>.ValueResult(result);
        }
    }
}