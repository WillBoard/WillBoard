using MediatR;
using System;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.AccountAuthenticationDeactivate
{
    public class AccountAuthenticationDeactivateCommand : IRequest<Status<InternalError>>
    {
        public Guid AccountId { get; set; }
        public Guid AuthenticationId { get; set; }
    }
}