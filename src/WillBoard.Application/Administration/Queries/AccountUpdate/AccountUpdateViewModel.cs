using WillBoard.Application.ViewModels;

namespace WillBoard.Application.Administration.Queries.AccountUpdate
{
    public class AccountUpdateViewModel : AdministrationViewModel
    {
        public WillBoard.Core.Entities.Account Account { get; set; }
    }
}