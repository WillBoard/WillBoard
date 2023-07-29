using MediatR;
using WillBoard.Application.DataModels;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Api.Queries.GetPost
{
    public class GetPostQuery : IRequest<Result<PostDataModel, InternalError>>
    {
        public string BoardId { get; set; }
        public int PostId { get; set; }
    }
}