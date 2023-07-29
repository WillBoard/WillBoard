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

namespace WillBoard.Application.Administration.Commands.ConfigurationDelete
{
    public class ConfigurationDeleteCommandHandler : IRequestHandler<ConfigurationDeleteCommand, Status<InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IConfigurationRepository _configurationRepository;
        private readonly IConfigurationCache _configurationCache;

        public ConfigurationDeleteCommandHandler(AccountManager accountManager, IConfigurationRepository configurationRepository, IConfigurationCache configurationCache)
        {
            _accountManager = accountManager;
            _configurationRepository = configurationRepository;
            _configurationCache = configurationCache;
        }

        public async Task<Status<InternalError>> Handle(ConfigurationDeleteCommand request, CancellationToken cancellationToken)
        {
            var requestAccount = _accountManager.GetAccount();

            if (requestAccount.Type != AccountType.Administrator)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var configuration = await _configurationCache.GetAsync(request.Key);

            if (configuration == null)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            await _configurationRepository.DeleteAsync(configuration.Key);

            await _configurationCache.RemoveCollectionAsync();

            return Status<InternalError>.SuccessStatus();
        }
    }
}