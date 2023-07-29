using MediatR;
using System;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BoardAuthorizationUpdate
{
    public class BoardAuthorizationUpdateQuery : IRequest<Result<BoardAuthorizationUpdateViewModel, InternalError>>
    {
        public string BoardId { get; set; }
        public Guid AuthorizationId { get; set; }
    }
}