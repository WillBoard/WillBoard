using System;
using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BoardThreadExcessive
{
    public class BoardThreadExcessiveQuery : IRequest<Result<BoardThreadExcessiveViewModel, InternalError>>
    {
        public string BoardId { get; set; }
        public int PostId { get; set; }
        public DateTime? Excessive { get; set; }
    }
}
