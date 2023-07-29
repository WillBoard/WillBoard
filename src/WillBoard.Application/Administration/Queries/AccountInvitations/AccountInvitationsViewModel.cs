using System.Collections.Generic;
using WillBoard.Application.ViewModels;
using WillBoard.Core.Entities;

namespace WillBoard.Application.Administration.Queries.AccountInvitations
{
    public class AccountInvitationsViewModel : AdministrationViewModel
    {
        public WillBoard.Core.Entities.Account Account { get; set; }
        public IEnumerable<Invitation> InvitationCollection { get; set; }
        public int Page { get; set; }
        public int PageMax { get; set; }
    }
}