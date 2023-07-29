using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.BanAppeals
{
    public class BanAppealsQuery : IRequest<Result<BanAppealsViewModel, InternalError>>
    {
        public string BoardId { get; set; }
        public int Page { get; set; }
    }
}