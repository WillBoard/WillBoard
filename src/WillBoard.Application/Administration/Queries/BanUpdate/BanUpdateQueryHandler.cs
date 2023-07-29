using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BanUpdate
{
    public class BanUpdateQueryHandler : IRequestHandler<BanUpdateQuery, Result<BanUpdateViewModel, InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IBanCache _banCache;

        public BanUpdateQueryHandler(AccountManager accountManager, IBanCache banCache)
        {
            _accountManager = accountManager;
            _banCache = banCache;
        }

        public async Task<Result<BanUpdateViewModel, InternalError>> Handle(BanUpdateQuery request, CancellationToken cancellationToken)
        {
            var requestAccount = _accountManager.GetAccount();

            if (requestAccount.Type != AccountType.Administrator)
            {
                return Result<BanUpdateViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var ban = await _banCache.GetSystemAsync(request.BanId);

            if (ban == null || ban.BoardId != null)
            {
                return Result<BanUpdateViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            var result = new BanUpdateViewModel()
            {
                Ban = ban
            };

            return Result<BanUpdateViewModel, InternalError>.ValueResult(result);
        }
    }
}