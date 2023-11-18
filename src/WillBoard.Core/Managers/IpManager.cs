using System;
using System.Net;
using WillBoard.Core.Enums;

namespace WillBoard.Core.Managers
{
    public class IpManager
    {
        private IpVersion _ipVersion;
        private UInt128 _ipNumber;
        private IPAddress _ipAddress;

        public IpManager()
        {
        }

        public void SetIpVersion(IpVersion ipVersion)
        {
            _ipVersion = ipVersion;
        }

        public IpVersion GetIpVersion()
        {
            return _ipVersion;
        }

        public void SetIpNumber(UInt128 ipNumber)
        {
            _ipNumber = ipNumber;
        }

        public UInt128 GetIpNumber()
        {
            return _ipNumber;
        }

        public void SetIpAddress(IPAddress ipAddress)
        {
            _ipAddress = ipAddress;
        }

        public IPAddress GetIpAddress()
        {
            return _ipAddress;
        }
    }
}