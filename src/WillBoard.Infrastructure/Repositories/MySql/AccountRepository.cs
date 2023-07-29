using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using WillBoard.Core.Entities;
using WillBoard.Core.Interfaces.Repositories;

namespace WillBoard.Infrastructure.Repositories.MySql
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ISqlConnectionService _sqlConnectionService;

        public AccountRepository(ISqlConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        public async Task CreateAsync(Account account)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "INSERT INTO `Account` (`AccountId`, `Creation`, `Type`, `Active`, `Password`) VALUES (@AccountId, @Creation, @Type, @Active, @Password);";

                dbConnection.Open();
                await dbConnection.ExecuteAsync(sql, account);
            }
        }

        public async Task<Account> ReadAsync(Guid accountId)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "SELECT * FROM `Account` WHERE `AccountId` = @AccountId LIMIT 1;";

                dbConnection.Open();
                return await dbConnection.QuerySingleOrDefaultAsync<Account>(sql, new { AccountId = accountId });
            }
        }

        public async Task<IEnumerable<Account>> ReadCollectionAsync(int skip, int take)
        {
            using (IDbConnection dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "SELECT * FROM `Account` ORDER BY `Creation` DESC OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;";

                dbConnection.Open();
                return await dbConnection.QueryAsync<Account>(sql, new { Skip = skip, Take = take });
            }
        }

        public async Task<int> ReadCountAsync()
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "SELECT COUNT(`AccountId`) FROM `Account`;";

                dbConnection.Open();
                return await dbConnection.ExecuteScalarAsync<int>(sql);
            }
        }

        public async Task UpdateAsync(Account account)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "UPDATE `Account` SET `Type` = @Type, `Active` = @Active, `Password` = @Password WHERE `AccountId` = @AccountId;";

                dbConnection.Open();
                await dbConnection.ExecuteAsync(sql, account);
            }
        }

        public async Task DeleteAsync(Guid accountId)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "DELETE FROM `Account` WHERE `AccountId` = @AccountId;";

                dbConnection.Open();
                await dbConnection.ExecuteAsync(sql, new { AccountId = accountId });
            }
        }
    }
}