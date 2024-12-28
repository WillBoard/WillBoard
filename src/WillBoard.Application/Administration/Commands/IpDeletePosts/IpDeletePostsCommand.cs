using MediatR;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.IpDeletePosts
{
    public class IpDeletePostsCommand : IRequest<Status<InternalError>>
    {
        public IpVersion IpVersion { get; set; }
        public string IpNumber { get; set; }
    }
}