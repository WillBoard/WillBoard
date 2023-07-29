using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.Navigations
{
    public class NavigationsQueryHandler : IRequestHandler<NavigationsQuery, Result<NavigationsViewModel, InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly INavigationCache _navigationCache;

        public NavigationsQueryHandler(AccountManager accountManager, INavigationCache navigationCache)
        {
            _accountManager = accountManager;
            _navigationCache = navigationCache;
        }

        public async Task<Result<NavigationsViewModel, InternalError>> Handle(NavigationsQuery request, CancellationToken cancellationToken)
        {
            var requestAccount = _accountManager.GetAccount();

            if (requestAccount.Type != AccountType.Administrator)
            {
                return Result<NavigationsViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var navigationCollection = await _navigationCache.GetCollectionAsync();

            var result = new NavigationsViewModel()
            {
                NavigationCollection = navigationCollection
            };

            return Result<NavigationsViewModel, InternalError>.ValueResult(result);
        }
    }
}