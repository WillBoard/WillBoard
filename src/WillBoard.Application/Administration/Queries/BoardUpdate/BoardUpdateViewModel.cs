using WillBoard.Application.ViewModels;

namespace WillBoard.Application.Administration.Queries.BoardUpdate
{
    public class BoardUpdateViewModel : AdministrationViewModel
    {
        public WillBoard.Core.Entities.Board Board { get; set; }
    }
}