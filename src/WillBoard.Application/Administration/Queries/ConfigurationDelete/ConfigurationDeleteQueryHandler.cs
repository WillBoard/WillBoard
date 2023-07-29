using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.ConfigurationDelete
{
    public class ConfigurationDeleteQueryHandler : IRequestHandler<ConfigurationDeleteQuery, Result<ConfigurationDeleteViewModel, InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IConfigurationCache _configurationCache;

        public ConfigurationDeleteQueryHandler(AccountManager accountManager, IConfigurationCache configurationCache)
        {
            _accountManager = accountManager;
            _configurationCache = configurationCache;
        }

        public async Task<Result<ConfigurationDeleteViewModel, InternalError>> Handle(ConfigurationDeleteQuery request, CancellationToken cancellationToken)
        {
            var requestAccount = _accountManager.GetAccount();

            if (requestAccount.Type != AccountType.Administrator)
            {
                return Result<ConfigurationDeleteViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var configuration = await _configurationCache.GetAsync(request.Key);

            if (configuration == null)
            {
                return Result<ConfigurationDeleteViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            var result = new ConfigurationDeleteViewModel()
            {
                Configuration = configuration
            };

            return Result<ConfigurationDeleteViewModel, InternalError>.ValueResult(result);
        }
    }
}