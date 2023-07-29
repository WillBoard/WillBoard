using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Providers;
using WillBoard.Core.Interfaces.Repositories;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Status<InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IAuthenticationRepository _authenticationRepository;
        private readonly IAuthenticationCache _authenticationCache;

        public LogoutCommandHandler(AccountManager accountManager, IDateTimeProvider dateTimeProvider, IAuthenticationRepository authenticationRepository, IAuthenticationCache authenticationCache)
        {
            _accountManager = accountManager;
            _dateTimeProvider = dateTimeProvider;
            _authenticationRepository = authenticationRepository;
            _authenticationCache = authenticationCache;
        }

        public async Task<Status<InternalError>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var account = _accountManager.GetAccount();
            var authentication = _accountManager.GetAuthentication();

            authentication.Expiration = _dateTimeProvider.UtcNow;

            await _authenticationRepository.UpdateAsync(authentication);
            await _authenticationCache.PurgeCollectionAsync(account.AccountId);

            return Status<InternalError>.SuccessStatus();
        }
    }
}