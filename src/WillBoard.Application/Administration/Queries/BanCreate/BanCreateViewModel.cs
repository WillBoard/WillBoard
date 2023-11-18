using System;
using WillBoard.Application.ViewModels;
using WillBoard.Core.Enums;

namespace WillBoard.Application.Administration.Queries.BanCreate
{
    public class BanCreateViewModel : AdministrationViewModel
    {
        public IpVersion IPVersion { get; set; }
        public UInt128 IPNumber { get; set; }
    }
}