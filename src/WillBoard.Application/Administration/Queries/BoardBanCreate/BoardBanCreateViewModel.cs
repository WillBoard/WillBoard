using System;
using System.Numerics;
using WillBoard.Application.ViewModels;
using WillBoard.Core.Enums;

namespace WillBoard.Application.Administration.Queries.BoardBanCreate
{
    public class BoardBanCreateViewModel : AdministrationViewModel
    {
        public string BoardId { get; set; }
        public IpVersion IPVersion { get; set; }
        public UInt128 IPNumber { get; set; }
    }
}