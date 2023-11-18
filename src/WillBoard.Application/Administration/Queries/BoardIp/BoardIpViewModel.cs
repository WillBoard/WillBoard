using System;
using System.Collections.Generic;
using WillBoard.Application.ViewModels;
using WillBoard.Core.Entities;
using WillBoard.Core.Enums;

namespace WillBoard.Application.Administration.Queries.BoardIp
{
    public class BoardIpViewModel : AdministrationViewModel
    {
        public IEnumerable<Post> PostCollection { get; set; }
        public IpVersion IpVersion { get; set; }
        public UInt128 IpNumber { get; set; }
        public string Dns { get; set; }
        public string Country { get; set; }
    }
}