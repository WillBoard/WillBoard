using WillBoard.Application.ViewModels;
using WillBoard.Core.Entities;

namespace WillBoard.Application.Administration.Queries.BoardBanDelete
{
    public class BoardBanDeleteViewModel : AdministrationViewModel
    {
        public Ban Ban { get; set; }
    }
}