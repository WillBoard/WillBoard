using System;
using Mediator;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BoardBanUpdate
{
    public class BoardBanUpdateQuery : IRequest<Result<BoardBanUpdateViewModel, InternalError>>
    {
        public string BoardId { get; set; }
        public Guid BanId { get; set; }
    }
}
