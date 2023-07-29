using System.Collections.Generic;
using WillBoard.Application.ViewModels;
using WillBoard.Core.Entities;

namespace WillBoard.Application.Board.Queries.Classic
{
    public class ClassicViewModel : BoardViewModel
    {
        public IDictionary<Post, Post[]> PostDictionary { get; set; }
        public int PageCurrent { get; set; }
        public int PageMax { get; set; }
    }
}