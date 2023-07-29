using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Board.Commands.ReportBoard
{
    public class ReportBoardCommand : IRequest<Status<InternalError>>
    {
        public string BoardId { get; set; }
        public int PostId { get; set; }
        public string Reason { get; set; }
    }
}