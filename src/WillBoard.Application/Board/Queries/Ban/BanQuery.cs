using MediatR;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Board.Queries.Ban
{
    public class BanQuery : IRequest<Result<BanViewModel, InternalError>>
    {
        public string BoardId { get; set; }
    }
}