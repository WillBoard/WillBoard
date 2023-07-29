using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Repositories;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.BanDelete
{
    public class BanDeleteCommandHandler : IRequestHandler<BanDeleteCommand, Status<InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IBanRepository _banRepository;
        private readonly IBanCache _banCache;
        private readonly IBanAppealCache _banAppealCache;

        public BanDeleteCommandHandler(AccountManager accountManager, IBanRepository banRepository, IBanCache banCache, IBanAppealCache banAppealCache)
        {
            _accountManager = accountManager;
            _banRepository = banRepository;
            _banCache = banCache;
            _banAppealCache = banAppealCache;
        }

        public async Task<Status<InternalError>> Handle(BanDeleteCommand request, CancellationToken cancellationToken)
        {
            var requestAccount = _accountManager.GetAccount();

            if (requestAccount.Type != AccountType.Administrator)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var ban = await _banCache.GetSystemAsync(request.BanId);
            if (ban == null || ban.BoardId != null)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            await _banRepository.DeleteAsync(ban.BanId);

            await _banCache.RemoveSystemAsync(ban.BanId);
            await _banCache.PurgeSystemUnexpiredCollectionAsync();
            await _banCache.PurgeSystemCollectionAsync();
            await _banCache.GetSystemCountAsync();

            await _banAppealCache.RemoveSystemAsync(ban.BanId);
            await _banAppealCache.PurgeSystemCollectionAsync();
            await _banAppealCache.GetSystemCountAsync();

            return Status<InternalError>.SuccessStatus();
        }
    }
}