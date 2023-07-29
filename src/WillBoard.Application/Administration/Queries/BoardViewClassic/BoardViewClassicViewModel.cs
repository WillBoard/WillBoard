using System.Collections.Generic;
using WillBoard.Application.ViewModels;
using WillBoard.Core.Entities;

namespace WillBoard.Application.Administration.Queries.BoardViewClassic
{
    public class BoardViewClassicViewModel : BoardViewModel
    {
        public IDictionary<Post, Post[]> PostDictionary { get; set; }
        public int PageCurrent { get; set; }
        public int PageMax { get; set; }
    }
}