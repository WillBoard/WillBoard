using WillBoard.Application.ViewModels;
using WillBoard.Core.Entities;

namespace WillBoard.Application.Administration.Queries.BoardAuthorizationDelete
{
    public class BoardAuthorizationDeleteViewModel : AdministrationViewModel
    {
        public Authorization Authorization { get; set; }
    }
}