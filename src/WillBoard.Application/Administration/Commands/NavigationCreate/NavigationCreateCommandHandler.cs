using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Entities;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Repositories;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.NavigationCreate
{
    public class NavigationCreateCommandHandler : IRequestHandler<NavigationCreateCommand, Status<InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly INavigationRepository _navigationRepository;
        private readonly INavigationCache _navigationCache;

        public NavigationCreateCommandHandler(AccountManager accountManager, INavigationRepository navigationRepository, INavigationCache navigationCache)
        {
            _accountManager = accountManager;
            _navigationRepository = navigationRepository;
            _navigationCache = navigationCache;
        }

        public async Task<Status<InternalError>> Handle(NavigationCreateCommand request, CancellationToken cancellationToken)
        {
            var requestAccount = _accountManager.GetAccount();

            if (requestAccount.Type != AccountType.Administrator)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var navigation = new Navigation()
            {
                NavigationId = Guid.NewGuid(),
                Priority = request.Priority,
                Icon = request.Icon,
                Name = request.Name,
                Url = request.Url,
                Tab = request.NewTab
            };

            await _navigationRepository.CreateAsync(navigation);

            await _navigationCache.RemoveCollectionAsync();

            return Status<InternalError>.SuccessStatus();
        }
    }
}