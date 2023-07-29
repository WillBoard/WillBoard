using System.Collections.Generic;
using WillBoard.Application.ViewModels;
using WillBoard.Core.Entities;

namespace WillBoard.Application.Board.Queries.Catalog
{
    public class CatalogViewModel : BoardViewModel
    {
        public IEnumerable<Post> ThreadCollection { get; set; }
    }
}