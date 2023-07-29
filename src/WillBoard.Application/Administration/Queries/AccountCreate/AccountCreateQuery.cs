using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.AccountCreate
{
    public class AccountCreateQuery : IRequest<Result<AccountCreateViewModel, InternalError>>
    {
    }
}