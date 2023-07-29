using MediatR;
using System;
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

namespace WillBoard.Application.Administration.Commands.AccountCreate
{
    public class AccountCreateCommandHandler : IRequestHandler<AccountCreateCommand, Status<InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountCache _accountCache;
        private readonly IPasswordService _passwordService;

        public AccountCreateCommandHandler(AccountManager accountManager, IDateTimeProvider dateTimeProvider, IAccountRepository accountRepository, IAccountCache accountCache, IPasswordService passwordService)
        {
            _accountManager = accountManager;
            _dateTimeProvider = dateTimeProvider;
            _accountRepository = accountRepository;
            _accountCache = accountCache;
            _passwordService = passwordService;
        }

        public async Task<Status<InternalError>> Handle(AccountCreateCommand request, CancellationToken cancellationToken)
        {
            var requestAccount = _accountManager.GetAccount();

            if (requestAccount.Type != AccountType.Administrator)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            if (request.Password == null || request.Password.Length < 8 || request.Password.Length > 64)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorNewPassword));
            }

            var account = new Account()
            {
                AccountId = Guid.NewGuid(),
                Creation = _dateTimeProvider.UtcNow,
                Type = request.Type,
                Active = request.Active,
                Password = _passwordService.HashPassword(request.Password)
            };

            await _accountRepository.CreateAsync(account);

            await _accountCache.RemoveAsync(account.AccountId);
            await _accountCache.PurgeCollectionAsync();
            await _accountCache.RemoveCountAsync();

            return Status<InternalError>.SuccessStatus();
        }
    }
}