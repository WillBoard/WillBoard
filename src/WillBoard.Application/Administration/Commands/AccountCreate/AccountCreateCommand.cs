using MediatR;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.AccountCreate
{
    public class AccountCreateCommand : IRequest<Status<InternalError>>
    {
        public string Password { get; set; }
        public AccountType Type { get; set; }
        public bool Active { get; set; }
    }
}