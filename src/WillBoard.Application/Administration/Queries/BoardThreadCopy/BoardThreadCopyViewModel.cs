using System.Collections.Generic;
using WillBoard.Application.ViewModels;
using WillBoard.Core.Entities;

namespace WillBoard.Application.Administration.Queries.BoardThreadCopy
{
    public class BoardThreadCopyViewModel : AdministrationViewModel
    {
        public Post Post { get; set; }
        public IEnumerable<WillBoard.Core.Entities.Board> BoardCollection { get; set; }
    }
}