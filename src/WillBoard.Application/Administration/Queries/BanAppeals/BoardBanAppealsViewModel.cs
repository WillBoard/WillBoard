using System.Collections.Generic;
using WillBoard.Application.ViewModels;
using WillBoard.Core.Entities;

namespace WillBoard.Application.Administration.Queries.BanAppeals
{
    public class BanAppealsViewModel : AdministrationViewModel
    {
        public IEnumerable<BanAppeal> BanAppealCollection { get; set; }
        public int Page { get; set; }
        public int PageMax { get; set; }
    }
}