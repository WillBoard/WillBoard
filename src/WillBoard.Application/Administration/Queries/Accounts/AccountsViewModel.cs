using System.Collections.Generic;
using WillBoard.Application.ViewModels;

namespace WillBoard.Application.Administration.Queries.Accounts
{
    public class AccountsViewModel : AdministrationViewModel
    {
        public IEnumerable<WillBoard.Core.Entities.Account> AccountCollection { get; set; }
        public int Page { get; set; }
        public int PageMax { get; set; }
    }
}