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

namespace WillBoard.Application.Administration.Commands.BanAppealReject
{
    public class BanAppealRejectCommandHandler : IRequestHandler<BanAppealRejectCommand, Status<InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IBanAppealRepository _banAppealRepository;
        private readonly IBanAppealCache _banAppealCache;

        public BanAppealRejectCommandHandler(AccountManager accountManager, IBanAppealRepository banAppealRepository, IBanAppealCache banAppealCache)
        {
            _accountManager = accountManager;
            _banAppealRepository = banAppealRepository;
            _banAppealCache = banAppealCache;
        }

        public async Task<Status<InternalError>> Handle(BanAppealRejectCommand request, CancellationToken cancellationToken)
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

            await _banAppealRepository.DeleteAsync(banAppeal.BanAppealId);

            await _banAppealCache.RemoveSystemAsync(banAppeal.BanAppealId);
            await _banAppealCache.RemoveSystemBanCollectionAsync(banAppeal.BanId);
            await _banAppealCache.PurgeSystemCollectionAsync();
            await _banAppealCache.RemoveSystemCountAsync();

            return Status<InternalError>.SuccessStatus();
        }
    }
}