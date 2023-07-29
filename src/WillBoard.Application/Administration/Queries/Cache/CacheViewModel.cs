using System.Collections.Generic;
using WillBoard.Application.ViewModels;

namespace WillBoard.Application.Administration.Queries.Cache
{
    public class CacheViewModel : AdministrationViewModel
    {
        public IEnumerable<WillBoard.Core.Entities.Board> BoardCollection { get; set; }
    }
}