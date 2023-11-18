using System;
using WillBoard.Core.Enums;

namespace WillBoard.Core.Interfaces.Services
{
    public interface IOnlineCounterService
    {
        void AddOrUpdate(IpVersion ipVersion, UInt128 ipNumber);
        int CountAndAddOrUpdate(IpVersion ipVersion, UInt128 ipNumber);
        int Count();
    }
}