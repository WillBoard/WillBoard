using System.Collections.Generic;
using WillBoard.Application.ViewModels;
using WillBoard.Core.Entities;

namespace WillBoard.Application.Administration.Queries.AccountAuthentications
{
    public class AccountAuthenticationsViewModel : AdministrationViewModel
    {
        public WillBoard.Core.Entities.Account Account { get; set; }
        public IEnumerable<Authentication> AuthenticationCollection { get; set; }
        public int Page { get; set; }
        public int PageMax { get; set; }
    }
}