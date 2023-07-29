using System.Collections.Generic;
using WillBoard.Application.ViewModels;
using WillBoard.Core.Entities;

namespace WillBoard.Application.Administration.Queries.BoardBans
{
    public class BoardBansViewModel : AdministrationViewModel
    {
        public WillBoard.Core.Entities.Board Board { get; set; }
        public IEnumerable<Ban> BanCollection { get; set; }
        public int Page { get; set; }
        public int PageMax { get; set; }
    }
}