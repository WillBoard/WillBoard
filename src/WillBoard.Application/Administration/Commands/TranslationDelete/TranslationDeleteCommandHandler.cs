using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Repositories;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.TranslationDelete
{
    public class TranslationDeleteCommandHandler : IRequestHandler<TranslationDeleteCommand, Status<InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly ITranslationRepository _translationRepository;
        private readonly ITranslationCache _translationCache;

        public TranslationDeleteCommandHandler(AccountManager accountManager, ITranslationRepository translationRepository, ITranslationCache translationCache)
        {
            _accountManager = accountManager;
            _translationRepository = translationRepository;
            _translationCache = translationCache;
        }

        public async Task<Status<InternalError>> Handle(TranslationDeleteCommand request, CancellationToken cancellationToken)
        {
            var requestAccount = _accountManager.GetAccount();

            if (requestAccount.Type != AccountType.Administrator)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var translationCollection = await _translationCache.GetCollectionAsync();
            var translation = translationCollection.FirstOrDefault(e => e.Language == request.Language && e.Key == request.Key);

            if (translation == null)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            await _translationRepository.DeleteAsync(translation.Language, translation.Key);

            await _translationCache.RemoveDictionaryAsync(translation.Language);
            await _translationCache.RemoveCollectionAsync();

            return Status<InternalError>.SuccessStatus();
        }
    }
}