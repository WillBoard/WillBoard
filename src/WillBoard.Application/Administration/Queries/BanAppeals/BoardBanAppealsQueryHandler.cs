using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BanAppeals
{
    public class BanAppealsQueryHandler : IRequestHandler<BanAppealsQuery, Result<BanAppealsViewModel, InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IBanAppealCache _banAppealCache;

        public BanAppealsQueryHandler(AccountManager accountManager, IBanAppealCache banAppealCache)
        {
            _accountManager = accountManager;
            _banAppealCache = banAppealCache;
        }

        public async Task<Result<BanAppealsViewModel, InternalError>> Handle(BanAppealsQuery request, CancellationToken cancellationToken)
        {
            var requestAccount = _accountManager.GetAccount();

            if (requestAccount.Type != AccountType.Administrator)
            {
                return Result<BanAppealsViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var banAppealCount = await _banAppealCache.GetSystemCountAsync();
            var pageMax = (banAppealCount / 100) + 1;

            if (request.Page > pageMax)
            {
                return Result<BanAppealsViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            var banAppealCollection = await _banAppealCache.GetSystemCollectionAsync((request.Page - 1) * 100, 100);

            var result = new BanAppealsViewModel()
            {
                BanAppealCollection = banAppealCollection,
                Page = request.Page,
                PageMax = pageMax
            };

            return Result<BanAppealsViewModel, InternalError>.ValueResult(result);
        }
    }
}