using Dapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WillBoard.Core.Entities;
using WillBoard.Core.Interfaces.Providers;
using WillBoard.Core.Interfaces.Repositories;

namespace WillBoard.Infrastructure.Repositories.MySql
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly ISqlConnectionService _sqlConnectionService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public AuthenticationRepository(ISqlConnectionService sqlConnectionService, IDateTimeProvider dateTimeProvider)
        {
            _sqlConnectionService = sqlConnectionService;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task CreateAsync(Authentication authentication)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "INSERT INTO `Authentication` (`AuthenticationId`, `AccountId`, `IpVersion`, `IpNumber`, `Creation`, `Expiration`, `Name`) VALUES (@AuthenticationId, @AccountId, @IpVersion, @IpNumber, @Creation, @Expiration, @Name);";

                dbConnection.Open();
                await dbConnection.ExecuteAsync(sql, authentication);
            }
        }

        public async Task<Authentication> ReadAsync(Guid authenticationId)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "SELECT * FROM `Authentication` WHERE `AuthenticationId` = @AuthenticationId LIMIT 1;";

                dbConnection.Open();
                return await dbConnection.QuerySingleOrDefaultAsync<Authentication>(sql, new { AuthenticationId = authenticationId });
            }
        }

        public async Task<IEnumerable<Authentication>> ReadUnexpiredCollectionAsync(Guid accountId)
        {
            var expirationFrom = _dateTimeProvider.UtcNow;

            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "SELECT * FROM `Authentication` WHERE `AccountId` = @AccountId AND `Expiration` >= @ExpirationFrom;";

                dbConnection.Open();
                return await dbConnection.QueryAsync<Authentication>(sql, new { AccountId = accountId, ExpirationFrom = expirationFrom });
            }
        }

        public async Task<IEnumerable<Authentication>> ReadCollectionAsync(Guid accountId, int skip, int take)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "SELECT * FROM `Authentication` WHERE `AccountId` = @AccountId ORDER BY `Creation` DESC OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;";

                dbConnection.Open();
                return await dbConnection.QueryAsync<Authentication>(sql, new { AccountId = accountId, Skip = skip, Take = take });
            }
        }

        public async Task<int> ReadCountAsync(Guid accountId)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "SELECT COUNT(`AuthenticationId`) FROM `Authentication` WHERE `AccountId` = @AccountId;";

                dbConnection.Open();
                return await dbConnection.ExecuteScalarAsync<int>(sql, new { AccountId = accountId });
            }
        }

        public async Task UpdateAsync(Authentication authentication)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "UPDATE `Authentication` SET `IpVersion` = @IpVersion, `IpNumber` = @IpNumber, `Expiration` = @Expiration, `Name` = @Name WHERE `AuthenticationId` = @AuthenticationId;";

                dbConnection.Open();
                await dbConnection.ExecuteAsync(sql, authentication);
            }
        }

        public async Task DeleteAsync(Guid authenticationId)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "DELETE FROM `Authentication` WHERE `AuthenticationId` = @AuthenticationId;";

                dbConnection.Open();
                await dbConnection.ExecuteAsync(sql, new { AuthenticationId = authenticationId });
            }
        }
    }
}