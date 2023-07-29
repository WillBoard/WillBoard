using MediatR;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.BoardIpDeletePosts
{
    public class BoardIpDeletePostsCommand : IRequest<Status<InternalError>>
    {
        public string BoardId { get; set; }
        public IpVersion IpVersion { get; set; }
        public string IpNumber { get; set; }
    }
}