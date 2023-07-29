using WillBoard.Application.ViewModels;
using WillBoard.Core.Entities;

namespace WillBoard.Application.Administration.Queries.BanDelete
{
    public class BanDeleteViewModel : AdministrationViewModel
    {
        public Ban Ban { get; set; }
    }
}