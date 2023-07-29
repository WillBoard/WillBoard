using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Board.Commands.ReportSystem
{
    public class ReportSystemCommand : IRequest<Status<InternalError>>
    {
        public string BoardId { get; set; }
        public int PostId { get; set; }
        public string Reason { get; set; }
    }
}