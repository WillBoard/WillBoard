using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.Reports
{
    public class ReportsQuery : IRequest<Result<ReportsViewModel, InternalError>>
    {
        public int Page { get; set; }
    }
}