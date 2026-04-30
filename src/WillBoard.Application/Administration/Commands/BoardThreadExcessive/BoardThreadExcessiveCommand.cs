using System;
using Mediator;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.BoardThreadExcessive
{
    public class BoardThreadExcessiveCommand : IRequest<Status<InternalError>>
    {
        public string BoardId { get; set; }
        public int PostId { get; set; }
        public DateTime? Excessive { get; set; }
    }
}
