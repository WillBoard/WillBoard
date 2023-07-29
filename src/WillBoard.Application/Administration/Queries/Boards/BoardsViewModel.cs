using System.Collections.Generic;
using WillBoard.Application.ViewModels;

namespace WillBoard.Application.Administration.Queries.Boards
{
    public class BoardsViewModel : AdministrationViewModel
    {
        public IEnumerable<WillBoard.Core.Entities.Board> BoardCollection { get; set; }
        public int Page { get; set; }
        public int PageMax { get; set; }
    }
}