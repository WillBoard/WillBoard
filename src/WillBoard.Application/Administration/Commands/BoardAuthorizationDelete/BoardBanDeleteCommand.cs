using MediatR;
using System;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.BoardAuthorizationDelete
{
    public class BoardAuthorizationDeleteCommand : IRequest<Status<InternalError>>
    {
        public string BoardId { get; set; }
        public Guid AuthorizationId { get; set; }
    }
}