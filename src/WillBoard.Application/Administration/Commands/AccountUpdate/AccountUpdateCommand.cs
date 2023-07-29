using MediatR;
using System;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.AccountUpdate
{
    public class AccountUpdateCommand : IRequest<Status<InternalError>>
    {
        public Guid AccountId { get; set; }
        public AccountType Type { get; set; }
        public bool Active { get; set; }
    }
}