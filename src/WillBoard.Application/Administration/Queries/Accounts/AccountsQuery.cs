using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.Accounts
{
    public class AccountsQuery : IRequest<Result<AccountsViewModel, InternalError>>
    {
        public int Page { get; set; }
    }
}