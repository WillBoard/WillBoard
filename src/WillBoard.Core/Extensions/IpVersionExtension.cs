using System.Net.Sockets;
using WillBoard.Core.Enums;

namespace WillBoard.Core.Extensions
{
    public static class IpVersionExtension
    {
        public static AddressFamily CheckAddressFamily(this IpVersion ipAddress)
        {
            switch (ipAddress)
            {
                case IpVersion.IpVersion4:
                    return AddressFamily.InterNetwork;

                case IpVersion.IpVersion6:
                    return AddressFamily.InterNetworkV6;

                default:
                    return AddressFamily.Unspecified;
            }
        }
    }
}