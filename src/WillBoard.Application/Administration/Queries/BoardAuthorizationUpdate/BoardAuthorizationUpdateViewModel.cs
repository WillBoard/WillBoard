using WillBoard.Application.ViewModels;
using WillBoard.Core.Entities;

namespace WillBoard.Application.Administration.Queries.BoardAuthorizationUpdate
{
    public class BoardAuthorizationUpdateViewModel : AdministrationViewModel
    {
        public Authorization Authorization { get; set; }
    }
}