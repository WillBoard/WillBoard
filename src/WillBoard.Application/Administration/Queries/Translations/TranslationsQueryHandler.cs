using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.Translations
{
    public class TranslationsQueryHandler : IRequestHandler<TranslationsQuery, Result<TranslationsViewModel, InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly ITranslationCache _translationCache;

        public TranslationsQueryHandler(AccountManager accountManager, ITranslationCache translationCache)
        {
            _accountManager = accountManager;
            _translationCache = translationCache;
        }

        public async Task<Result<TranslationsViewModel, InternalError>> Handle(TranslationsQuery request, CancellationToken cancellationToken)
        {
            var requestAccount = _accountManager.GetAccount();

            if (requestAccount.Type != AccountType.Administrator)
            {
                return Result<TranslationsViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var translationCollection = await _translationCache.GetCollectionAsync();

            var result = new TranslationsViewModel()
            {
                TranslationCollection = translationCollection
            };

            return Result<TranslationsViewModel, InternalError>.ValueResult(result);
        }
    }
}