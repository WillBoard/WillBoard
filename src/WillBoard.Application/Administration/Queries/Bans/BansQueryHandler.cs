using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.Bans
{
    public class BansQueryHandler : IRequestHandler<BansQuery, Result<BansViewModel, InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IBanCache _banCache;

        public BansQueryHandler(AccountManager accountManager, IBanCache banCache)
        {
            _accountManager = accountManager;
            _banCache = banCache;
        }

        public async Task<Result<BansViewModel, InternalError>> Handle(BansQuery request, CancellationToken cancellationToken)
        {
            var requestAccount = _accountManager.GetAccount();

            if (requestAccount.Type != AccountType.Administrator)
            {
                return Result<BansViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var banCount = await _banCache.GetSystemCountAsync();
            var pageMax = (banCount / 100) + 1;

            if (request.Page > pageMax)
            {
                return Result<BansViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            var banCollection = await _banCache.GetSystemCollectionAsync((request.Page - 1) * 100, 100);

            var result = new BansViewModel()
            {
                BanCollection = banCollection,
                Page = request.Page,
                PageMax = pageMax
            };

            return Result<BansViewModel, InternalError>.ValueResult(result);
        }
    }
}