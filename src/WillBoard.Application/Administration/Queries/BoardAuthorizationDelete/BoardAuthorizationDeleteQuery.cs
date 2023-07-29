using MediatR;
using System;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BoardAuthorizationDelete
{
    public class BoardAuthorizationDeleteQuery : IRequest<Result<BoardAuthorizationDeleteViewModel, InternalError>>
    {
        public string BoardId { get; set; }
        public Guid AuthorizationId { get; set; }
    }
}