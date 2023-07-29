using System.Collections.Generic;
using WillBoard.Application.ViewModels;
using WillBoard.Core.Entities;

namespace WillBoard.Application.Administration.Queries.Bans
{
    public class BansViewModel : AdministrationViewModel
    {
        public IEnumerable<Ban> BanCollection { get; set; }
        public int Page { get; set; }
        public int PageMax { get; set; }
    }
}