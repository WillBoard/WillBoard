using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.NavigationDelete
{
    public class NavigationDeleteQueryHandler : IRequestHandler<NavigationDeleteQuery, Result<NavigationDeleteViewModel, InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly INavigationCache _navigationCache;

        public NavigationDeleteQueryHandler(AccountManager accountManager, INavigationCache navigationCache)
        {
            _accountManager = accountManager;
            _navigationCache = navigationCache;
        }

        public async Task<Result<NavigationDeleteViewModel, InternalError>> Handle(NavigationDeleteQuery request, CancellationToken cancellationToken)
        {
            var requestAccount = _accountManager.GetAccount();

            if (requestAccount.Type != AccountType.Administrator)
            {
                return Result<NavigationDeleteViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var navigation = await _navigationCache.GetAsync(request.NavigationId);

            if (navigation == null)
            {
                return Result<NavigationDeleteViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            var result = new NavigationDeleteViewModel()
            {
                Navigation = navigation
            };

            return Result<NavigationDeleteViewModel, InternalError>.ValueResult(result);
        }
    }
}