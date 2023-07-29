using System.Collections.Generic;
using WillBoard.Application.ViewModels;
using WillBoard.Core.Entities;

namespace WillBoard.Application.Board.Queries.Ban
{
    public class BanViewModel : BoardViewModel
    {
        public IDictionary<WillBoard.Core.Entities.Ban, BanAppeal> BanDictionary { get; set; }
    }
}