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
using WillBoard.Core.Interfaces.Services;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.AccountPasswordChange
{
    public class AccountPasswordChangeCommandHandler : IRequestHandler<AccountPasswordChangeCommand, Status<InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IAuthenticationRepository _authenticationRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountCache _accountCache;
        private readonly IAuthenticationCache _authenticationCache;
        private readonly IPasswordService _passwordService;

        public AccountPasswordChangeCommandHandler(AccountManager accountManager, IDateTimeProvider dateTimeProvider, IAuthenticationRepository authenticationRepository, IAccountRepository accountRepository, IAccountCache accountCache, IAuthenticationCache authenticationCache, IPasswordService passwordService)
        {
            _accountManager = accountManager;
            _dateTimeProvider = dateTimeProvider;
            _authenticationRepository = authenticationRepository;
            _accountRepository = accountRepository;
            _accountCache = accountCache;
            _authenticationCache = authenticationCache;
            _passwordService = passwordService;
        }

        public async Task<Status<InternalError>> Handle(AccountPasswordChangeCommand request, CancellationToken cancellationToken)
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

            if (request.NewPassword == null || request.NewPassword.Length < 8 || request.NewPassword.Length > 64)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorNewPassword));
            }

            if (request.NewPassword != request.NewPasswordConfirmation)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorNewPasswordConfirmation));
            }

            if (request.CurrentPassword == null || request.CurrentPassword.Length < 8 || request.CurrentPassword.Length > 64)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorCurrentPassword));
            }

            if (!_passwordService.VerifyPassword(account.Password, request.CurrentPassword))
            {
                return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorCurrentPassword));
            }

            var password = _passwordService.HashPassword(request.NewPassword);

            var expiriation = _dateTimeProvider.UtcNow;

            var authenticationCollection = await _authenticationRepository.ReadUnexpiredCollectionAsync(account.AccountId);

            foreach (var authentication in authenticationCollection)
            {
                var updatedAuthentication = new Authentication(authentication);
                updatedAuthentication.Expiration = expiriation;
                await _authenticationRepository.UpdateAsync(updatedAuthentication);
            }

            await _authenticationCache.PurgeAsync(account.AccountId);
            await _authenticationCache.PurgeCollectionAsync(account.AccountId);

            var updatedAccount = new Account(account);
            updatedAccount.Password = password;

            await _accountRepository.UpdateAsync(updatedAccount);

            await _accountCache.RemoveAsync(account.AccountId);
            await _accountCache.PurgeCollectionAsync();

            return Status<InternalError>.SuccessStatus();
        }
    }
}