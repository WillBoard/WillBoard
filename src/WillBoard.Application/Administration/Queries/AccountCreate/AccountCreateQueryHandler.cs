using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.AccountCreate
{
    public class AccountCreateQueryHandler : IRequestHandler<AccountCreateQuery, Result<AccountCreateViewModel, InternalError>>
    {
        private readonly AccountManager _accountManager;

        public AccountCreateQueryHandler(AccountManager accountManager)
        {
            _accountManager = accountManager;
        }

        public async Task<Result<AccountCreateViewModel, InternalError>> Handle(AccountCreateQuery request, CancellationToken cancellationToken)
        {
            var requestAccount = _accountManager.GetAccount();

            if (requestAccount.Type != AccountType.Administrator)
            {
                return Result<AccountCreateViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var result = new AccountCreateViewModel();

            return Result<AccountCreateViewModel, InternalError>.ValueResult(result);
        }
    }
}