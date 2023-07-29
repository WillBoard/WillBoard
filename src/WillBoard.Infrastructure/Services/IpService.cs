using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Numerics;
using System.Threading.Tasks;
using WillBoard.Core.Classes;
using WillBoard.Core.Enums;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Services;

namespace WillBoard.Infrastructure.Services
{
    public class IpService : IIpService
    {
        private readonly ILogger _logger;
        private readonly IBadIpCache _badIpCache;
        private readonly ICountryIpCache _countryIpCache;
        private readonly IBlockListCache _blockListCache;
        private readonly IBlockListService _blockListService;

        public IpService(ILogger<IpService> logger, IBadIpCache badIpCache, ICountryIpCache countryIpCache, IBlockListCache blockListCache, IBlockListService blockListService)
        {
            _logger = logger;
            _badIpCache = badIpCache;
            _countryIpCache = countryIpCache;
            _blockListCache = blockListCache;
            _blockListService = blockListService;
        }

        public async Task<string> GetDnsHostNameAsync(IPAddress ipAddress)
        {
            try
            {
                var dns = await Dns.GetHostEntryAsync(ipAddress);
                return dns.HostName;
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, "Exception occured during {0} method with IP address {1}.", nameof(GetDnsHostNameAsync), ipAddress.ToString());
                return ipAddress.ToString();
            }
        }

        public async Task<string> GetCountryIpAsync(IpVersion ipVersion, BigInteger ipNumber)
        {
            var countryIp = await _countryIpCache.GetAsync(ipVersion, ipNumber);
            if (countryIp == null)
            {
                return "ZZ";
            }
            else
            {
                return countryIp.CountryCode;
            }
        }

        public async Task<bool> GetBadIpAsync(IpVersion ipVersion, BigInteger ipNumber)
        {
            var badIp = await _badIpCache.GetAsync(ipVersion, ipNumber);
            if (badIp == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<bool> GetDnsBlockListIpVersion4Async(string boardId, uint ipNumber, IEnumerable<BlockList> blockListCollection, bool cache)
        {
            if (cache)
            {
                return await _blockListCache.GetBoardDnsBlockListIpVersion4Async(boardId, ipNumber, blockListCollection);
            }

            return await _blockListService.CheckDnsBlockListIpVersion4Async(ipNumber, blockListCollection);
        }

        public async Task<bool> GetDnsBlockListIpVersion6Async(string boardId, BigInteger ipNumber, IEnumerable<BlockList> blockListCollection, bool cache)
        {
            if (cache)
            {
                return await _blockListCache.GetBoardDnsBlockListIpVersion6Async(boardId, ipNumber, blockListCollection);
            }

            return await _blockListService.CheckDnsBlockListIpVersion6Async(ipNumber, blockListCollection);
        }

        public async Task<bool> GetApiBlockListIpVersion4Async(string boardId, uint ipNumber, IEnumerable<BlockList> blockListCollection, bool cache)
        {
            if (cache)
            {
                return await _blockListCache.GetBoardApiBlockListIpVersion4Async(boardId, ipNumber, blockListCollection);
            }

            return await _blockListService.CheckApiBlockListIpVersion4Async(ipNumber, blockListCollection);
        }

        public async Task<bool> GetApiBlockListIpVersion6Async(string boardId, BigInteger ipNumber, IEnumerable<BlockList> blockListCollection, bool cache)
        {
            if (cache)
            {
                return await _blockListCache.GetBoardApiBlockListIpVersion6Async(boardId, ipNumber, blockListCollection);
            }

            return await _blockListService.CheckApiBlockListIpVersion6Async(ipNumber, blockListCollection);
        }
    }
}