using System.Collections.Generic;
using WillBoard.Application.ViewModels;

namespace WillBoard.Application.Administration.Queries.BoardBanAppeals
{
    public class BoardBanAppealsViewModel : AdministrationViewModel
    {
        public WillBoard.Core.Entities.Board Board { get; set; }
        public IEnumerable<Core.Entities.BanAppeal> BanAppealCollection { get; set; }
        public int Page { get; set; }
        public int PageMax { get; set; }
    }
}