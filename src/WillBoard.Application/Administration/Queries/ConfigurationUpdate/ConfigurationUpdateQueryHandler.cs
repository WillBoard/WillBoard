using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.ConfigurationUpdate
{
    public class ConfigurationUpdateQueryHandler : IRequestHandler<ConfigurationUpdateQuery, Result<ConfigurationUpdateViewModel, InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IConfigurationCache _configurationCache;

        public ConfigurationUpdateQueryHandler(AccountManager accountManager, IConfigurationCache configurationCache)
        {
            _accountManager = accountManager;
            _configurationCache = configurationCache;
        }

        public async Task<Result<ConfigurationUpdateViewModel, InternalError>> Handle(ConfigurationUpdateQuery request, CancellationToken cancellationToken)
        {
            var requestAccount = _accountManager.GetAccount();

            if (requestAccount.Type != AccountType.Administrator)
            {
                return Result<ConfigurationUpdateViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var configuration = await _configurationCache.GetAsync(request.Key);

            if (configuration == null)
            {
                return Result<ConfigurationUpdateViewModel, InternalError>.ErrorResult(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            var result = new ConfigurationUpdateViewModel()
            {
                Configuration = configuration
            };

            return Result<ConfigurationUpdateViewModel, InternalError>.ValueResult(result);
        }
    }
}