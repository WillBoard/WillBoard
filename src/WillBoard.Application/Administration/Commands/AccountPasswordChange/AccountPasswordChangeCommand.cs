using MediatR;
using System;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.AccountPasswordChange
{
    public class AccountPasswordChangeCommand : IRequest<Status<InternalError>>
    {
        public Guid AccountId { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string NewPasswordConfirmation { get; set; }
    }
}