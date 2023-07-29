using MediatR;
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

namespace WillBoard.Application.Administration.Commands.AccountUpdate
{
    public class AccountUpdateCommandHandler : IRequestHandler<AccountUpdateCommand, Status<InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountCache _accountCache;

        public AccountUpdateCommandHandler(AccountManager accountManager, IAccountRepository accountRepository, IAccountCache accountCache)
        {
            _accountManager = accountManager;
            _accountRepository = accountRepository;
            _accountCache = accountCache;
        }

        public async Task<Status<InternalError>> Handle(AccountUpdateCommand request, CancellationToken cancellationToken)
        {
            var requestAccount = _accountManager.GetAccount();
            if (requestAccount.Type != AccountType.Administrator)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var account = await _accountCache.GetAsync(request.AccountId);

            if (account == null)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            var updatedAccount = new Account(account);
            updatedAccount.Type = request.Type;
            updatedAccount.Active = request.Active;

            await _accountRepository.UpdateAsync(updatedAccount);

            await _accountCache.RemoveAsync(account.AccountId);
            await _accountCache.PurgeCollectionAsync();

            return Status<InternalError>.SuccessStatus();
        }
    }
}