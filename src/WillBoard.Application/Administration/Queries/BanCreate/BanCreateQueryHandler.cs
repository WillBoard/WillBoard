using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BanCreate
{
    public class BanCreateQueryHandler : IRequestHandler<BanCreateQuery, Result<BanCreateViewModel, InternalError>>
    {
        private readonly AccountManager _accountManager;

        public BanCreateQueryHandler(AccountManager accountManager)
        {
            _accountManager = accountManager;
        }

        public async Task<Result<BanCreateViewModel, InternalError>> Handle(BanCreateQuery request, CancellationToken cancellationToken)
        {
            var requestAccount = _accountManager.GetAccount();

            if (requestAccount.Type != AccountType.Administrator)
            {
                return Result<BanCreateViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            UInt128.TryParse(request.IPNumber, out UInt128 ipNumber);

            var result = new BanCreateViewModel()
            {
                IPVersion = request.IPVersion,
                IPNumber = ipNumber
            };

            return Result<BanCreateViewModel, InternalError>.ValueResult(result);
        }
    }
}