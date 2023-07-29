using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Entities;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Repositories;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.TranslationCreate
{
    public class TranslationCreateCommandHandler : IRequestHandler<TranslationCreateCommand, Status<InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly ITranslationRepository _translationRepository;
        private readonly ITranslationCache _translationCache;

        public TranslationCreateCommandHandler(AccountManager accountManager, ITranslationRepository translationRepository, ITranslationCache translationCache)
        {
            _accountManager = accountManager;
            _translationRepository = translationRepository;
            _translationCache = translationCache;
        }

        public async Task<Status<InternalError>> Handle(TranslationCreateCommand request, CancellationToken cancellationToken)
        {
            var requestAccount = _accountManager.GetAccount();

            if (requestAccount.Type != AccountType.Administrator)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            if (string.IsNullOrEmpty(request.Language))
            {
                return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorBadRequest));
            }

            if (string.IsNullOrEmpty(request.Key))
            {
                return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorBadRequest));
            }

            var translationCollection = await _translationCache.GetCollectionAsync();
            var currentTranslation = translationCollection.FirstOrDefault(e => e.Language == request.Language && e.Key == request.Key);

            if (currentTranslation != null)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorBadRequest));
            }

            var translation = new Translation()
            {
                Language = request.Language,
                Key = request.Key,
                Value = request.Value
            };

            await _translationRepository.CreateAsync(translation);

            await _translationCache.RemoveDictionaryAsync(translation.Language);
            await _translationCache.RemoveCollectionAsync();

            return Status<InternalError>.SuccessStatus();
        }
    }
}