using System;
using WillBoard.Application.ViewModels;
using WillBoard.Core.Enums;

namespace WillBoard.Application.Administration.Queries.IpDeletePosts
{
    public class IpDeletePostsViewModel : AdministrationViewModel
    {
        public IpVersion IpVersion { get; set; }
        public UInt128 IpNumber { get; set; }
    }
}