using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using WillBoard.Core.Enums;
using WillBoard.Core.Extensions;
using WillBoard.Core.Managers;
using WillBoard.Core.Utilities;

namespace WillBoard.Web.Middlewares
{
    public class IpNumberMiddleware
    {
        private readonly RequestDelegate _next;

        public IpNumberMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context, IpManager ipManager)
        {
            var ipAddress = context.Connection.RemoteIpAddress;
            ipManager.SetIpAddress(ipAddress);

            var ipVersion = ipAddress.CheckIpVersion();
            ipManager.SetIpVersion(ipVersion);

            switch (ipVersion)
            {
                case IpVersion.IpVersion4:
                    ipManager.SetIpNumber(IpConversion.IpVersion4AddressToIpVersion4Number(ipAddress));
                    break;

                case IpVersion.IpVersion6:
                    ipManager.SetIpNumber(IpConversion.IpVersion6AddressToIpVersion6Number(ipAddress));
                    break;
            }

            return _next(context);
        }
    }
}