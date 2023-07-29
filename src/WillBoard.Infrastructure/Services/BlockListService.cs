using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Numerics;
using System.Threading.Tasks;
using WillBoard.Core.Classes;
using WillBoard.Core.Interfaces.Services;
using WillBoard.Core.Utilities;

namespace WillBoard.Infrastructure.Services
{
    public class BlockListService : IBlockListService
    {
        private readonly ILogger _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public BlockListService(ILogger<BlockListService> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<bool> CheckDnsBlockListIpVersion4Async(uint ipNumber, IEnumerable<BlockList> blockListCollection)
        {
            var ipAddress = IpConversion.IpVersion4NumberToIpVersion4AddressString(ipNumber);

            var charCollection = ipAddress.Split('.').Reverse();
            var dnsIp = string.Join(".", charCollection);

            foreach (var blockList in blockListCollection)
            {
                try
                {
                    var hostAddressCollection = await Dns.GetHostAddressesAsync($"{dnsIp}.{blockList.Address}");
                    if (hostAddressCollection != null)
                    {
                        foreach (var response in blockList.ResponseCollection)
                        {
                            if (hostAddressCollection.Any(e => e.ToString() == response))
                            {
                                return true;
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogWarning(exception, "Exception occured during {0} method with IP address {1} and host name {2}.", nameof(CheckDnsBlockListIpVersion4Async), ipAddress.ToString(), $"{dnsIp}.{blockList.Address}");
                }
            }

            return false;
        }

        public async Task<bool> CheckDnsBlockListIpVersion6Async(BigInteger ipNumber, IEnumerable<BlockList> blockListCollection)
        {
            var ipAddress = IpConversion.IpVersion6NumberToIpVersion6AddressString(ipNumber, false);

            var charCollection = ipAddress.Replace(":", "").ToCharArray().Reverse();
            var dnsIp = string.Join(".", charCollection);

            foreach (var blockList in blockListCollection)
            {
                try
                {
                    var hostAddressCollection = await Dns.GetHostAddressesAsync($"{dnsIp}.{blockList.Address}");
                    if (hostAddressCollection != null)
                    {
                        foreach (var response in blockList.ResponseCollection)
                        {
                            if (hostAddressCollection.Any(e => e.ToString() == response))
                            {
                                return true;
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogWarning(exception, "Exception occured during {0} method with IP address {1} and host name {2}.", nameof(CheckDnsBlockListIpVersion6Async), ipAddress, $"{dnsIp}.{blockList.Address}");
                }
            }

            return false;
        }

        public async Task<bool> CheckApiBlockListIpVersion4Async(uint ipNumber, IEnumerable<BlockList> blockListCollection)
        {
            var ipAddress = IpConversion.IpVersion4NumberToIpVersion4AddressString(ipNumber);

            foreach (var blockList in blockListCollection)
            {
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, string.Format(blockList.Address, ipAddress));
                    var client = _httpClientFactory.CreateClient();
                    client.Timeout = TimeSpan.FromSeconds(2.5);

                    var response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        if (blockList.ResponseCollection.Any(e => e == content))
                        {
                            return true;
                        }
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogWarning(exception, "Exception occured during {0} method with IP address {1}.", nameof(CheckApiBlockListIpVersion4Async), ipAddress);
                }
            }

            return false;
        }

        public async Task<bool> CheckApiBlockListIpVersion6Async(BigInteger ipNumber, IEnumerable<BlockList> blockListCollection)
        {
            var ipAddress = IpConversion.IpVersion6NumberToIpVersion6AddressString(ipNumber, false);

            foreach (var blockList in blockListCollection)
            {
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, string.Format(blockList.Address, ipAddress));
                    var client = _httpClientFactory.CreateClient();
                    client.Timeout = TimeSpan.FromSeconds(2.5);

                    var response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        if (blockList.ResponseCollection.Any(e => e == content))
                        {
                            return true;
                        }
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogWarning(exception, "Exception occured during {0} method with IP address {1}.", nameof(CheckApiBlockListIpVersion6Async), ipAddress);
                }
            }

            return false;
        }
    }
}