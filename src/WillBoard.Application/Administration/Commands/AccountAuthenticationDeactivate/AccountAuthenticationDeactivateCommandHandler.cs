using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Entities;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Providers;
using WillBoard.Core.Interfaces.Repositories;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.AccountAuthenticationDeactivate
{
    public class AccountAuthenticationDeactivateCommandHandler : IRequestHandler<AccountAuthenticationDeactivateCommand, Status<InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IAuthenticationRepository _authenticationRepository;
        private readonly IAccountCache _accountCache;
        private readonly IAuthenticationCache _authenticationCache;

        public AccountAuthenticationDeactivateCommandHandler(AccountManager accountManager, IDateTimeProvider dateTimeProvider, IAuthenticationRepository authenticationRepository, IAccountCache accountCache, IAuthenticationCache authenticationCache)
        {
            _accountManager = accountManager;
            _dateTimeProvider = dateTimeProvider;
            _authenticationRepository = authenticationRepository;
            _accountCache = accountCache;
            _authenticationCache = authenticationCache;
        }

        public async Task<Status<InternalError>> Handle(AccountAuthenticationDeactivateCommand request, CancellationToken cancellationToken)
        {
            var requestAccount = _accountManager.GetAccount();
            if (request.AccountId != requestAccount.AccountId && requestAccount.Type != AccountType.Administrator)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var account = await _accountCache.GetAsync(request.AccountId);

            if (account == null)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            var authentication = await _authenticationCache.GetAsync(account.AccountId, request.AuthenticationId);

            if (authentication == null)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            if (authentication.Expiration < _dateTimeProvider.UtcNow)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorBadRequest));
            }

            var exiredAuthentication = new Authentication(authentication);
            exiredAuthentication.Expiration = _dateTimeProvider.UtcNow;

            await _authenticationRepository.UpdateAsync(exiredAuthentication);

            await _authenticationCache.PurgeAsync(account.AccountId);
            await _authenticationCache.PurgeCollectionAsync(account.AccountId);

            return Status<InternalError>.SuccessStatus();
        }
    }
}