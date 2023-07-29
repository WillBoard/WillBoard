using Dapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WillBoard.Core.Entities;
using WillBoard.Core.Interfaces.Repositories;

namespace WillBoard.Infrastructure.Repositories.MySql
{
    public class NavigationRepository : INavigationRepository
    {
        private readonly ISqlConnectionService _sqlConnectionService;

        public NavigationRepository(ISqlConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        public async Task CreateAsync(Navigation navigation)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "INSERT INTO `Navigation` (`NavigationId`, `Priority`, `Icon`, `Name`, `Url`, `Tab`) VALUES (@NavigationId, @Priority, @Icon, @Name, @Url, @Tab);";

                dbConnection.Open();
                await dbConnection.ExecuteAsync(sql, navigation);
            }
        }

        public async Task<IEnumerable<Navigation>> ReadCollectionAsync()
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "SELECT * FROM `Navigation` ORDER BY `Priority` ASC;";

                dbConnection.Open();
                return await dbConnection.QueryAsync<Navigation>(sql);
            }
        }

        public async Task UpdateAsync(Navigation navigation)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "UPDATE `Navigation` SET `Priority` = @Priority, `Icon` = @Icon, `Name` = @Name, `Url` = @Url , `Tab` = @Tab WHERE `NavigationId` = @NavigationId;";

                dbConnection.Open();
                await dbConnection.ExecuteAsync(sql, navigation);
            }
        }

        public async Task DeleteAsync(Guid navigationId)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "DELETE FROM `Navigation` WHERE `NavigationId` = @NavigationId;";

                dbConnection.Open();
                await dbConnection.ExecuteAsync(sql, new { NavigationId = navigationId });
            }
        }
    }
}