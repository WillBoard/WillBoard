using System.Numerics;
using WillBoard.Application.ViewModels;
using WillBoard.Core.Enums;

namespace WillBoard.Application.Administration.Queries.BoardIpDeletePosts
{
    public class BoardIpDeletePostsViewModel : AdministrationViewModel
    {
        public string BoardId { get; set; }
        public IpVersion IpVersion { get; set; }
        public BigInteger IpNumber { get; set; }
    }
}