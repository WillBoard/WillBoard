using MediatR;
using System;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.BoardInvitationCreate
{
    public class BoardInvitationCreateCommand : IRequest<Status<InternalError>>
    {
        public string BoardId { get; set; }
        public Guid AccountId { get; set; }
        public string Message { get; set; }
    }
}