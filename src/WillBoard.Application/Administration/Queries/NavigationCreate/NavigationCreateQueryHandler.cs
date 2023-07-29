using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.NavigationCreate
{
    public class NavigationCreateQueryHandler : IRequestHandler<NavigationCreateQuery, Result<NavigationCreateViewModel, InternalError>>
    {
        private readonly AccountManager _accountManager;

        public NavigationCreateQueryHandler(AccountManager accountManager)
        {
            _accountManager = accountManager;
        }

        public async Task<Result<NavigationCreateViewModel, InternalError>> Handle(NavigationCreateQuery request, CancellationToken cancellationToken)
        {
            var requestAccount = _accountManager.GetAccount();

            if (requestAccount.Type != AccountType.Administrator)
            {
                return Result<NavigationCreateViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var result = new NavigationCreateViewModel();

            return Result<NavigationCreateViewModel, InternalError>.ValueResult(result);
        }
    }
}