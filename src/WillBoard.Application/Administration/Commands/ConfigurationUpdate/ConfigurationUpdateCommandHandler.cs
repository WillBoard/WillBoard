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

namespace WillBoard.Application.Administration.Commands.ConfigurationUpdate
{
    public class ConfigurationUpdateCommandHandler : IRequestHandler<ConfigurationUpdateCommand, Status<InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IConfigurationRepository _configurationRepository;
        private readonly IConfigurationCache _configurationCache;

        public ConfigurationUpdateCommandHandler(AccountManager accountManager, IConfigurationRepository configurationRepository, IConfigurationCache configurationCache)
        {
            _accountManager = accountManager;
            _configurationRepository = configurationRepository;
            _configurationCache = configurationCache;
        }

        public async Task<Status<InternalError>> Handle(ConfigurationUpdateCommand request, CancellationToken cancellationToken)
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

            var configurationUpdate = new Configuration()
            {
                Key = configuration.Key,
                Type = request.Type
            };

            if (request.Type == ConfigurationType.Boolean)
            {
                if (bool.TryParse(request.Value, out bool booleanValue))
                {
                    configurationUpdate.Value = booleanValue;
                }
            }

            if (request.Type == ConfigurationType.String)
            {
                configurationUpdate.Value = request.Value;
            }

            if (request.Type == ConfigurationType.Integer)
            {
                if (int.TryParse(request.Value, out int integerValue))
                {
                    configurationUpdate.Value = integerValue;
                }
            }

            await _configurationRepository.UpdateAsync(configurationUpdate);

            await _configurationCache.RemoveCollectionAsync();

            return Status<InternalError>.SuccessStatus();
        }
    }
}