using MySqlConnector;
using System.Data;
using WillBoard.Core.Interfaces.Repositories;
using WillBoard.Core.Interfaces.Services;

namespace WillBoard.Infrastructure.Repositories
{
    public class MySqlConnectionService : ISqlConnectionService
    {
        private readonly IConfigurationService _configurationService;

        public MySqlConnectionService(IConfigurationService instanceService)
        {
            _configurationService = instanceService;
        }

        public IDbConnection Connection
        {
            get
            {
                return new MySqlConnection(_configurationService.Configuration.Database.ConnectionString);
            }
        }
    }
}