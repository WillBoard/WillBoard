using MediatR;
using System.Collections.Generic;
using WillBoard.Application.DataModels;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Api.Queries.GetReplies
{
    public class GetRepliesQuery : IRequest<Result<IEnumerable<PostDataModel>, InternalError>>
    {
        public string BoardId { get; set; }
        public int ThreadId { get; set; }
        public int? Last { get; set; }
    }
}