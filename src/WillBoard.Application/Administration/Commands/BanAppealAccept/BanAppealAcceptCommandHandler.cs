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

namespace WillBoard.Application.Administration.Commands.BanAppealAccept
{
    public class BanAppealAcceptCommandHandler : IRequestHandler<BanAppealAcceptCommand, Status<InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IBanRepository _banRepository;
        private readonly IBanCache _banCache;
        private readonly IBanAppealCache _banAppealCache;

        public BanAppealAcceptCommandHandler(AccountManager accountManager, IBanRepository banRepository, IBanCache banCache, IBanAppealCache banAppealCache)
        {
            _accountManager = accountManager;
            _banRepository = banRepository;
            _banCache = banCache;
            _banAppealCache = banAppealCache;
        }

        public async Task<Status<InternalError>> Handle(BanAppealAcceptCommand request, CancellationToken cancellationToken)
        {
            var requestAccount = _accountManager.GetAccount();

            if (requestAccount.Type != AccountType.Administrator)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var banAppeal = await _banAppealCache.GetSystemAsync(request.BanAppealId);

            if (banAppeal == null)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            await _banRepository.DeleteAsync(banAppeal.BanId);

            await _banCache.RemoveSystemAsync(banAppeal.BanId);
            await _banCache.PurgeSystemCollectionAsync();
            await _banCache.PurgeSystemUnexpiredCollectionAsync();
            await _banCache.RemoveSystemCountAsync();

            await _banAppealCache.RemoveSystemAsync(banAppeal.BanAppealId);
            await _banAppealCache.RemoveSystemBanCollectionAsync(banAppeal.BanId);
            await _banAppealCache.PurgeSystemCollectionAsync();
            await _banAppealCache.RemoveSystemCountAsync();

            return Status<InternalError>.SuccessStatus();
        }
    }
}