using Dapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WillBoard.Core.Entities;
using WillBoard.Core.Enums;
using WillBoard.Core.Interfaces.Repositories;

namespace WillBoard.Infrastructure.Repositories.MySql
{
    public class ConfigurationRepository : IConfigurationRepository
    {
        private readonly ISqlConnectionService _sqlConnectionService;

        public ConfigurationRepository(ISqlConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        public async Task CreateAsync(Configuration configuration)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "INSERT INTO `Configuration` (`Key`, `Value`, `Type`) VALUES (@Key, @Value, @Type);";

                dbConnection.Open();
                await dbConnection.ExecuteAsync(sql, new { Key = configuration.Key, Value = Convert.ToString(configuration.Value), Type = configuration.Type });
            }
        }

        public async Task<IEnumerable<Configuration>> ReadCollectionAsync()
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "SELECT * FROM `Configuration`;";

                dbConnection.Open();
                var result = await dbConnection.QueryAsync<Configuration>(sql);

                foreach (var element in result)
                {
                    switch (element.Type)
                    {
                        case ConfigurationType.Boolean:
                            if (bool.TryParse(element.Value, out bool booleanValue))
                            {
                                element.Value = booleanValue;
                            }
                            else
                            {
                                element.Value = default(bool);
                            }
                            break;

                        case ConfigurationType.Integer:
                            if (int.TryParse(element.Value, out int integerValue))
                            {
                                element.Value = integerValue;
                            }
                            else
                            {
                                element.Value = default(int);
                            }
                            break;
                    }
                }

                return result;
            }
        }

        public async Task UpdateAsync(Configuration configuration)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "UPDATE `Configuration` SET `Value` = @Value, `Type` = @Type WHERE `Key` = @Key;";

                dbConnection.Open();
                await dbConnection.ExecuteAsync(sql, new { Key = configuration.Key, Value = Convert.ToString(configuration.Value), Type = configuration.Type });
            }
        }

        public async Task DeleteAsync(string key)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "DELETE FROM `Configuration` WHERE `Key` = @Key;";

                dbConnection.Open();
                await dbConnection.ExecuteAsync(sql, new { Key = key });
            }
        }
    }
}