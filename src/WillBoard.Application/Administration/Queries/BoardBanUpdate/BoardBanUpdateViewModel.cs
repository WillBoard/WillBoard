using WillBoard.Application.ViewModels;
using WillBoard.Core.Entities;

namespace WillBoard.Application.Administration.Queries.BoardBanUpdate
{
    public class BoardBanUpdateViewModel : AdministrationViewModel
    {
        public Ban Ban { get; set; }
    }
}