using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.Configurations
{
    public class ConfigurationsQueryHandler : IRequestHandler<ConfigurationsQuery, Result<ConfigurationsViewModel, InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IConfigurationCache _configurationCache;

        public ConfigurationsQueryHandler(AccountManager accountManager, IConfigurationCache configurationCache)
        {
            _accountManager = accountManager;
            _configurationCache = configurationCache;
        }

        public async Task<Result<ConfigurationsViewModel, InternalError>> Handle(ConfigurationsQuery request, CancellationToken cancellationToken)
        {
            var requestAccount = _accountManager.GetAccount();

            if (requestAccount.Type != AccountType.Administrator)
            {
                return Result<ConfigurationsViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var configurationCollection = await _configurationCache.GetCollectionAsync();

            var result = new ConfigurationsViewModel()
            {
                ConfigurationCollection = configurationCollection
            };

            return Result<ConfigurationsViewModel, InternalError>.ValueResult(result);
        }
    }
}