using System.Collections.Generic;
using WillBoard.Application.ViewModels;
using WillBoard.Core.Entities;

namespace WillBoard.Application.Administration.Queries.BoardInvitations
{
    public class BoardInvitationsViewModel : AdministrationViewModel
    {
        public WillBoard.Core.Entities.Board Board { get; set; }
        public IEnumerable<Invitation> InvitationCollection { get; set; }
        public int Page { get; set; }
        public int PageMax { get; set; }
    }
}