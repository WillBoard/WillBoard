using Dapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WillBoard.Core.Entities;
using WillBoard.Core.Interfaces.Repositories;

namespace WillBoard.Infrastructure.Repositories.MySql
{
    public class LogRepository : ILogRepository
    {
        private readonly ISqlConnectionService _sqlConnectionService;

        public LogRepository(ISqlConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        public void CreateLog(Log log)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "INSERT INTO `Log` (`LogId`, `Creation`, `Message`) VALUES (@LogId, @Creation, @Message);";

                dbConnection.Open();
                dbConnection.Execute(sql, log);
            }
        }

        public IEnumerable<Log> ReadLogCollection(DateTime dateTimeFrom, DateTime dateTimeTo)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "SELECT * FROM `Log` WHERE `Creation` >= @DateTimeFrom AND `Creation` <= @DateTimeTo;";

                dbConnection.Open();
                return dbConnection.Query<Log>(sql, new { DateTimeFrom = dateTimeFrom, DateTimeTo = dateTimeTo });
            }
        }

        public async Task CreateLogAsync(Log log)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "INSERT INTO `Log` (`LogId`, `Creation`, `Message`) VALUES (@LogId, @Creation, @Message);";

                dbConnection.Open();
                await dbConnection.ExecuteAsync(sql, log);
            }
        }

        public async Task<IEnumerable<Log>> ReadLogCollectionAsync(DateTime dateTimeFrom, DateTime dateTimeTo)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "SELECT * FROM `Log` WHERE `Creation` >= @DateTimeFrom AND `Creation` <= @DateTimeTo;";

                dbConnection.Open();
                return await dbConnection.QueryAsync<Log>(sql, new { DateTimeFrom = dateTimeFrom, DateTimeTo = dateTimeTo });
            }
        }
    }
}