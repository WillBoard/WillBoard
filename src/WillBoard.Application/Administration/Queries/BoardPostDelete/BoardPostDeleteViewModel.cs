using WillBoard.Application.ViewModels;
using WillBoard.Core.Entities;

namespace WillBoard.Application.Administration.Queries.BoardPostDelete
{
    public class BoardPostDeleteViewModel : AdministrationViewModel
    {
        public Post Post { get; set; }
    }
}