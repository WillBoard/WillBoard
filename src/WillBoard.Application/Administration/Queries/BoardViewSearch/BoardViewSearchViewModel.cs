using System.Collections.Generic;
using WillBoard.Application.ViewModels;
using WillBoard.Core.Entities;

namespace WillBoard.Application.Administration.Queries.BoardViewSearch
{
    public class BoardViewSearchViewModel : BoardViewModel
    {
        public int? PostId { get; set; }
        public int? ThreadId { get; set; }
        public string Message { get; set; }
        public string File { get; set; }
        public string Type { get; set; }
        public string Order { get; set; }
        public IEnumerable<Post> SearchCollection { get; set; }
    }
}