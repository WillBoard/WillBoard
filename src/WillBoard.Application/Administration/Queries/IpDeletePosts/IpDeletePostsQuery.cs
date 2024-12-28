using MediatR;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Queries.IpDeletePosts
{
    public class IpDeletePostsQuery : IRequest<Result<IpDeletePostsViewModel, InternalError>>
    {
        public IpVersion IpVersion { get; set; }
        public string IpNumber { get; set; }
    }
}