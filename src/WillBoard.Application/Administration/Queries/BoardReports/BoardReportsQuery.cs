using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BoardReports
{
    public class BoardReportsQuery : IRequest<Result<BoardReportsViewModel, InternalError>>
    {
        public string BoardId { get; set; }
        public int Page { get; set; }
    }
}