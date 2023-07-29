using MediatR;
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

namespace WillBoard.Application.Administration.Commands.ConfigurationCreate
{
    public class ConfigurationCreateCommandHandler : IRequestHandler<ConfigurationCreateCommand, Status<InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IConfigurationRepository _configurationRepository;
        private readonly IConfigurationCache _configurationCache;

        public ConfigurationCreateCommandHandler(AccountManager accountManager, IConfigurationRepository configurationRepository, IConfigurationCache configurationCache)
        {
            _accountManager = accountManager;
            _configurationRepository = configurationRepository;
            _configurationCache = configurationCache;
        }

        public async Task<Status<InternalError>> Handle(ConfigurationCreateCommand request, CancellationToken cancellationToken)
        {
            var requestAccount = _accountManager.GetAccount();

            if (requestAccount.Type != AccountType.Administrator)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            if (string.IsNullOrEmpty(request.Key))
            {
                return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorBadRequest));
            }

            var currentConfiguration = await _configurationCache.GetAsync(request.Key);

            if (currentConfiguration != null)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(400, TranslationKey.ErrorBadRequest));
            }

            var configuration = new Configuration()
            {
                Key = request.Key,
                Type = request.Type
            };

            if (request.Type == ConfigurationType.Boolean)
            {
                if (bool.TryParse(request.Value, out bool booleanValue))
                {
                    configuration.Value = booleanValue;
                }
            }

            if (request.Type == ConfigurationType.String)
            {
                configuration.Value = request.Value;
            }

            if (request.Type == ConfigurationType.Integer)
            {
                if (int.TryParse(request.Value, out int integerValue))
                {
                    configuration.Value = integerValue;
                }
            }

            await _configurationRepository.CreateAsync(configuration);

            await _configurationCache.RemoveCollectionAsync();

            return Status<InternalError>.SuccessStatus();
        }
    }
}