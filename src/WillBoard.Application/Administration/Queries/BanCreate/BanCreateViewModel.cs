using System.Numerics;
using WillBoard.Application.ViewModels;
using WillBoard.Core.Enums;

namespace WillBoard.Application.Administration.Queries.BanCreate
{
    public class BanCreateViewModel : AdministrationViewModel
    {
        public IpVersion IPVersion { get; set; }
        public BigInteger IPNumber { get; set; }
    }
}