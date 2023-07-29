using MediatR;
using WillBoard.Application.ViewModels;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Board.Queries.Delete
{
    public class DeleteQuery : IRequest<Result<PostViewModel, InternalError>>
    {
        public string BoardId { get; set; }
        public int PostId { get; set; }
    }
}