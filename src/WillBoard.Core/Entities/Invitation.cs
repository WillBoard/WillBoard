using System;

namespace WillBoard.Core.Entities
{
    public class Invitation
    {
        public Guid InvitationId { get; set; }

        public Guid AccountId { get; set; }
        public string BoardId { get; set; }

        public DateTime Creation { get; set; }

        public string Message { get; set; }

        public Invitation()
        {
        }
    }
}