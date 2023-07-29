using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.TranslationDelete
{
    public class TranslationDeleteQueryHandler : IRequestHandler<TranslationDeleteQuery, Result<TranslationDeleteViewModel, InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly ITranslationCache _translationCache;

        public TranslationDeleteQueryHandler(AccountManager accountManager, ITranslationCache translationCache)
        {
            _accountManager = accountManager;
            _translationCache = translationCache;
        }

        public async Task<Result<TranslationDeleteViewModel, InternalError>> Handle(TranslationDeleteQuery request, CancellationToken cancellationToken)
        {
            var requestAccount = _accountManager.GetAccount();

            if (requestAccount.Type != AccountType.Administrator)
            {
                return Result<TranslationDeleteViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var translationCollection = await _translationCache.GetCollectionAsync();
            var translation = translationCollection.FirstOrDefault(e => e.Language == request.Language && e.Key == request.Key);

            if (translation == null)
            {
                return Result<TranslationDeleteViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            var result = new TranslationDeleteViewModel()
            {
                Translation = translation
            };

            return Result<TranslationDeleteViewModel, InternalError>.ValueResult(result);
        }
    }
}