using System.Net;
using System.Net.Sockets;
using WillBoard.Core.Enums;

namespace WillBoard.Core.Extensions
{
    public static class IpAddressExtension
    {
        public static IpVersion CheckIpVersion(this IPAddress ipAddress)
        {
            switch (ipAddress.AddressFamily)
            {
                case AddressFamily.InterNetwork:
                    return IpVersion.IpVersion4;

                case AddressFamily.InterNetworkV6:
                    return IpVersion.IpVersion6;

                default:
                    return IpVersion.None;
            }
        }
    }
}