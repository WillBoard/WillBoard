using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WillBoard.Core.Entities;
using WillBoard.Core.Interfaces.Repositories;

namespace WillBoard.Infrastructure.Repositories.MySql
{
    public class TranslationRepository : ITranslationRepository
    {
        private readonly ISqlConnectionService _sqlConnectionService;

        public TranslationRepository(ISqlConnectionService sqlConnectionService)
        {
            _sqlConnectionService = sqlConnectionService;
        }

        public async Task CreateAsync(Translation translation)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "INSERT INTO `Translation` (`Language`, `Key`, `Value`) VALUES (@Language, @Key, @Value);";

                dbConnection.Open();
                await dbConnection.ExecuteAsync(sql, translation);
            }
        }

        public async Task<IDictionary<string, string>> ReadDictionaryAsync(string language)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "SELECT * FROM `Translation` WHERE `Language` = @Language;";

                dbConnection.Open();
                var collection = await dbConnection.QueryAsync<Translation>(sql, new { Language = language });

                var dictionary = collection.ToDictionary(x => x.Key, x => x.Value);

                return dictionary;
            }
        }

        public async Task<IEnumerable<Translation>> ReadCollectionAsync()
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "SELECT * FROM `Translation`;";

                dbConnection.Open();
                return await dbConnection.QueryAsync<Translation>(sql);
            }
        }

        public async Task UpdateAsync(Translation translation)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "UPDATE `Translation` SET `Value` = @Value WHERE `Language` = @Language AND `Key` = @Key;";

                dbConnection.Open();
                await dbConnection.ExecuteAsync(sql, translation);
            }
        }

        public async Task DeleteAsync(string language, string key)
        {
            using (var dbConnection = _sqlConnectionService.Connection)
            {
                var sql = "DELETE FROM `Translation` WHERE `Language` = @Language AND `Key` = @Key;";

                dbConnection.Open();
                await dbConnection.ExecuteAsync(sql, new { Language = language, Key = key });
            }
        }
    }
}