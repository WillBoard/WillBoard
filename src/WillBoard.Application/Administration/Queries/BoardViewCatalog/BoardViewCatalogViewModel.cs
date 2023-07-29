using System.Collections.Generic;
using WillBoard.Application.ViewModels;
using WillBoard.Core.Entities;

namespace WillBoard.Application.Administration.Queries.BoardViewCatalog
{
    public class BoardViewCatalogViewModel : BoardViewModel
    {
        public IEnumerable<Post> ThreadCollection { get; set; }
    }
}