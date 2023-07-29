using System.Collections.Generic;
using System.Data;
using System.Numerics;
using System.Threading.Tasks;
using WillBoard.Core.Entities;
using WillBoard.Core.Enums;

namespace WillBoard.Core.Interfaces.Repositories
{
    public interface ICountryIpRepository
    {
        Task CreateCollectionAsync(IEnumerable<CountryIp> countryIpCollection);
        Task CreateCollectionAsync(IEnumerable<CountryIp> countryIpCollection, IDbConnection dbConnection, IDbTransaction transaction);

        Task<CountryIp> ReadAsync(IpVersion ipVersion, BigInteger ipNumber);

        Task TruncateAsync();
        Task TruncateAsync(IDbConnection dbConnection, IDbTransaction transaction);
    }
}