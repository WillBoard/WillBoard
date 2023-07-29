using System;
using WillBoard.Core.Enums;

namespace WillBoard.Core.Entities
{
    public class Account
    {
        public Guid AccountId { get; set; }
        public DateTime Creation { get; set; }
        public AccountType Type { get; set; }
        public bool Active { get; set; }
        public string Password { get; set; }

        public Account()
        {
        }

        public Account(Account account)
        {
            AccountId = account.AccountId;
            Creation = account.Creation;
            Type = account.Type;
            Active = account.Active;
            Password = account.Password;
        }
    }
}