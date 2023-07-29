using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BanDelete
{
    public class BanDeleteQueryHandler : IRequestHandler<BanDeleteQuery, Result<BanDeleteViewModel, InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IBanCache _banCache;

        public BanDeleteQueryHandler(AccountManager accountManager, IBanCache banCache)
        {
            _accountManager = accountManager;
            _banCache = banCache;
        }

        public async Task<Result<BanDeleteViewModel, InternalError>> Handle(BanDeleteQuery request, CancellationToken cancellationToken)
        {
            var requestAccount = _accountManager.GetAccount();

            if (requestAccount.Type != AccountType.Administrator)
            {
                return Result<BanDeleteViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var ban = await _banCache.GetSystemAsync(request.BanId);

            if (ban == null || ban.BoardId != null)
            {
                return Result<BanDeleteViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            var result = new BanDeleteViewModel()
            {
                Ban = ban
            };

            return Result<BanDeleteViewModel, InternalError>.ValueResult(result);
        }
    }
}