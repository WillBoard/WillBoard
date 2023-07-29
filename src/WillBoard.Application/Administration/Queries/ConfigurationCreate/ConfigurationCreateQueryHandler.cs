using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.ConfigurationCreate
{
    public class ConfigurationCreateQueryHandler : IRequestHandler<ConfigurationCreateQuery, Result<ConfigurationCreateViewModel, InternalError>>
    {
        private readonly AccountManager _accountManager;

        public ConfigurationCreateQueryHandler(AccountManager accountManager)
        {
            _accountManager = accountManager;
        }

        public async Task<Result<ConfigurationCreateViewModel, InternalError>> Handle(ConfigurationCreateQuery request, CancellationToken cancellationToken)
        {
            var requestAccount = _accountManager.GetAccount();

            if (requestAccount.Type != AccountType.Administrator)
            {
                return Result<ConfigurationCreateViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var result = new ConfigurationCreateViewModel();

            return Result<ConfigurationCreateViewModel, InternalError>.ValueResult(result);
        }
    }
}