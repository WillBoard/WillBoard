using MediatR;
using System;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BoardBanDelete
{
    public class BoardBanDeleteQuery : IRequest<Result<BoardBanDeleteViewModel, InternalError>>
    {
        public string BoardId { get; set; }
        public Guid BanId { get; set; }
    }
}