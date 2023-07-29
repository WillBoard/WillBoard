using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.TranslationCreate
{
    public class TranslationCreateQueryHandler : IRequestHandler<TranslationCreateQuery, Result<TranslationCreateViewModel, InternalError>>
    {
        private readonly AccountManager _accountManager;

        public TranslationCreateQueryHandler(AccountManager accountManager)
        {
            _accountManager = accountManager;
        }

        public async Task<Result<TranslationCreateViewModel, InternalError>> Handle(TranslationCreateQuery request, CancellationToken cancellationToken)
        {
            var requestAccount = _accountManager.GetAccount();

            if (requestAccount.Type != AccountType.Administrator)
            {
                return Result<TranslationCreateViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var result = new TranslationCreateViewModel();

            return Result<TranslationCreateViewModel, InternalError>.ValueResult(result);
        }
    }
}