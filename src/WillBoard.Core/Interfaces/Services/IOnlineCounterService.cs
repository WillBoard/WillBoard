using System.Numerics;
using WillBoard.Core.Enums;

namespace WillBoard.Core.Interfaces.Services
{
    public interface IOnlineCounterService
    {
        void AddOrUpdate(IpVersion ipVersion, BigInteger ipNumber);
        int CountAndAddOrUpdate(IpVersion ipVersion, BigInteger ipNumber);
        int Count();
    }
}