using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.NavigationUpdate
{
    public class NavigationUpdateQueryHandler : IRequestHandler<NavigationUpdateQuery, Result<NavigationUpdateViewModel, InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly INavigationCache _navigationCache;

        public NavigationUpdateQueryHandler(AccountManager accountManager, INavigationCache navigationCache)
        {
            _accountManager = accountManager;
            _navigationCache = navigationCache;
        }

        public async Task<Result<NavigationUpdateViewModel, InternalError>> Handle(NavigationUpdateQuery request, CancellationToken cancellationToken)
        {
            var requestAccount = _accountManager.GetAccount();

            if (requestAccount.Type != AccountType.Administrator)
            {
                return Result<NavigationUpdateViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var navigation = await _navigationCache.GetAsync(request.NavigationId);

            if (navigation == null)
            {
                return Result<NavigationUpdateViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            var result = new NavigationUpdateViewModel()
            {
                Navigation = navigation
            };

            return Result<NavigationUpdateViewModel, InternalError>.ValueResult(result);
        }
    }
}