using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BoardCreate
{
    public class BoardCreateQueryHandler : IRequestHandler<BoardCreateQuery, Result<BoardCreateViewModel, InternalError>>
    {
        private readonly AccountManager _accountManager;

        public BoardCreateQueryHandler(AccountManager accountManager)
        {
            _accountManager = accountManager;
        }

        public async Task<Result<BoardCreateViewModel, InternalError>> Handle(BoardCreateQuery request, CancellationToken cancellationToken)
        {
            var requestAccount = _accountManager.GetAccount();

            if (requestAccount.Type != AccountType.Administrator)
            {
                return Result<BoardCreateViewModel, InternalError>.ErrorResult(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var result = new BoardCreateViewModel();

            return Result<BoardCreateViewModel, InternalError>.ValueResult(result);
        }
    }
}