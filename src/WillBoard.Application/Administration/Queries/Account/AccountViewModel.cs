using WillBoard.Application.ViewModels;

namespace WillBoard.Application.Administration.Queries.Account
{
    public class AccountViewModel : AdministrationViewModel
    {
        public WillBoard.Core.Entities.Account Account { get; set; }
    }
}