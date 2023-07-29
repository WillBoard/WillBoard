using WillBoard.Application.ViewModels;
using WillBoard.Core.Entities;

namespace WillBoard.Application.Administration.Queries.BoardPostUpdate
{
    public class BoardPostUpdateViewModel : AdministrationViewModel
    {
        public Post Post { get; set; }
    }
}