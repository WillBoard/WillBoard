using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.Cache
{
    public class CacheQueryHandler : IRequestHandler<CacheQuery, Result<CacheViewModel, InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IBoardCache _boardCache;

        public CacheQueryHandler(AccountManager accountManager, IBoardCache boardCache)
        {
            _accountManager = accountManager;
            _boardCache = boardCache;
        }

        public async Task<Result<CacheViewModel, InternalError>> Handle(CacheQuery request, CancellationToken cancellationToken)
        {
            var requestAccount = _accountManager.GetAccount();

            if (requestAccount.Type != AccountType.Administrator)
            {
                return Result<CacheViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var boardCollection = await _boardCache.GetCollectionAsync();

            var result = new CacheViewModel()
            {
                BoardCollection = boardCollection
            };

            return Result<CacheViewModel, InternalError>.ValueResult(result);
        }
    }
}