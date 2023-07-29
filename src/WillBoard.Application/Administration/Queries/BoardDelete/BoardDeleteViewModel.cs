using WillBoard.Application.ViewModels;

namespace WillBoard.Application.Administration.Queries.BoardDelete
{
    public class BoardDeleteViewModel : AdministrationViewModel
    {
        public WillBoard.Core.Entities.Board Board { get; set; }
    }
}