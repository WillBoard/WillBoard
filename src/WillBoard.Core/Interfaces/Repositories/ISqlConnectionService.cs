using System.Data;

namespace WillBoard.Core.Interfaces.Repositories
{
    public interface ISqlConnectionService
    {
        public IDbConnection Connection { get; }
    }
}