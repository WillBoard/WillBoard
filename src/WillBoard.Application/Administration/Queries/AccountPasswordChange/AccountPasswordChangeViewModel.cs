using WillBoard.Application.ViewModels;

namespace WillBoard.Application.Administration.Queries.AccountPasswordChange
{
    public class AccountPasswordChangeViewModel : AdministrationViewModel
    {
        public WillBoard.Core.Entities.Account Account { get; set; }
    }
}