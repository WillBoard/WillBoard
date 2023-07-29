using System;
using System.Collections.Generic;
using System.Linq;
using WillBoard.Core.Entities;

namespace WillBoard.Core.Managers
{
    public class AccountManager
    {
        private Account _account;
        private Authentication _authentication;
        private IEnumerable<Authorization> _authorizationCollection;

        public AccountManager()
        {
        }

        public void SetAccount(Account account)
        {
            _account = account;
        }

        public Account GetAccount()
        {
            return _account;
        }

        public void SetAuthentication(Authentication authentication)
        {
            _authentication = authentication;
        }

        public Authentication GetAuthentication()
        {
            return _authentication;
        }

        public void SetAuthorizationCollection(IEnumerable<Authorization> authorizationCollection)
        {
            _authorizationCollection = authorizationCollection;
        }

        public IEnumerable<Authorization> GetAuthorizationCollection()
        {
            return _authorizationCollection;
        }

        public bool CheckPermission(string boardId, Func<Authorization, bool> propertySelector)
        {
            if (_account == null)
            {
                return false;
            }

            if (_account.Type == Enums.AccountType.Administrator)
            {
                return true;
            }

            if (_authorizationCollection == null)
            {
                return false;
            }

            var authorization = _authorizationCollection.FirstOrDefault(e => e.BoardId == boardId);

            if (authorization == null)
            {
                return false;
            }

            return propertySelector(authorization);
        }

        public bool CheckPermission(string boardId)
        {
            if (_account == null)
            {
                return false;
            }

            if (_account.Type == Enums.AccountType.Administrator)
            {
                return true;
            }

            if (_authorizationCollection == null)
            {
                return false;
            }

            var authorization = _authorizationCollection.FirstOrDefault(e => e.BoardId == boardId);

            if (authorization == null)
            {
                return false;
            }

            return true;
        }
    }
}