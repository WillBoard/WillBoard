using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using WillBoard.Core.Entities;
using WillBoard.Core.Enums;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Managers;
using WillBoard.Core.Interfaces.Repositories;

namespace WillBoard.Infrastructure.Caches.Memory
{
    public class ReportCache : IReportCache
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IReportRepository _reportRepository;
        private readonly ILockManager _lockManager;
        private readonly ICancellationTokenManager _cancellationTokenManager;

        public ReportCache(IMemoryCache memoryCache, IReportRepository reportRepository, ILockManager lockManager, ICancellationTokenManager cancellationTokenManager)
        {
            _memoryCache = memoryCache;
            _reportRepository = reportRepository;
            _lockManager = lockManager;
            _cancellationTokenManager = cancellationTokenManager;
        }

        public async Task<Report> GetSystemAsync(Guid reportId)
        {
            if (_memoryCache.TryGetValue($"{nameof(Report)}_System_{reportId}", out Report report))
            {
                return report;
            }
            else
            {
                using (await _lockManager.GetReportLockAsync($"Cache_GetSystem_{reportId}"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(Report)}_System_{reportId}", out report))
                    {
                        return report;
                    }

                    report = await _reportRepository.ReadSystemAsync(reportId);

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.Normal,
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(15)
                    };

                    var cancellationTokenSource = _cancellationTokenManager.GetReportCancellationTokenSource("Cache_GetSystem");
                    memoryCacheEntryOptions.AddExpirationToken(new CancellationChangeToken(cancellationTokenSource.Token));

                    _memoryCache.Set($"{nameof(Report)}_System_{reportId}", report, memoryCacheEntryOptions);

                    return report;
                }
            }
        }

        public async Task RemoveSystemAsync(Guid reportId)
        {
            _memoryCache.Remove($"{nameof(Report)}_System_{reportId}");
        }

        public async Task PurgeSystemAsync()
        {
            _cancellationTokenManager.RemoveReportCancellationTokenSource("Cache_GetSystem");
        }

        public async Task<IEnumerable<Report>> GetSystemIpCollectionAsync(IpVersion ipVersion, BigInteger ipNumber)
        {
            if (_memoryCache.TryGetValue($"{nameof(Report)}_SystemIpCollection_{ipVersion}_{ipNumber}", out IEnumerable<Report> reportCollection))
            {
                return reportCollection;
            }
            else
            {
                using (await _lockManager.GetReportLockAsync($"Cache_GetSystemIpCollection_{ipVersion}_{ipNumber}"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(Report)}_SystemIpCollection_{ipVersion}_{ipNumber}", out reportCollection))
                    {
                        return reportCollection;
                    }

                    reportCollection = await _reportRepository.ReadSystemIPCollectionAsync(ipVersion, ipNumber);

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.Normal,
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(5)
                    };

                    var cancellationTokenSource = _cancellationTokenManager.GetReportCancellationTokenSource("Cache_GetSystemIpCollection");
                    memoryCacheEntryOptions.AddExpirationToken(new CancellationChangeToken(cancellationTokenSource.Token));

                    _memoryCache.Set($"{nameof(Report)}_SystemIpCollection_{ipVersion}_{ipNumber}", reportCollection, memoryCacheEntryOptions);

                    return reportCollection;
                }
            }
        }

        public async Task RemoveSystemIpCollectionAsync(IpVersion ipVersion, BigInteger ipNumber)
        {
            _memoryCache.Remove($"{nameof(Report)}_SystemIpCollection_{ipVersion}_{ipNumber}");
        }

        public async Task PurgeSystemIpCollectionAsync()
        {
            _cancellationTokenManager.RemoveReportCancellationTokenSource("Cache_GetSystemIpCollection");
        }

        public async Task<IEnumerable<Report>> GetSystemCollectionAsync(int skip, int take)
        {
            if (_memoryCache.TryGetValue($"{nameof(Report)}_SystemCollection_{skip}_{take}", out IEnumerable<Report> reportCollection))
            {
                return reportCollection;
            }
            else
            {
                using (await _lockManager.GetReportLockAsync($"Cache_GetSystemCollection_{skip}_{take}"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(Report)}_SystemCollection_{skip}_{take}", out reportCollection))
                    {
                        return reportCollection;
                    }

                    reportCollection = await _reportRepository.ReadSystemCollectionAsync(skip, take);

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.Normal,
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(5)
                    };

                    var cancellationTokenSource = _cancellationTokenManager.GetReportCancellationTokenSource("Cache_GetSystemCollection");
                    memoryCacheEntryOptions.AddExpirationToken(new CancellationChangeToken(cancellationTokenSource.Token));

                    _memoryCache.Set($"{nameof(Report)}_SystemCollection_{skip}_{take}", reportCollection, memoryCacheEntryOptions);

                    return reportCollection;
                }
            }
        }

        public async Task PurgeSystemCollectionAsync()
        {
            _cancellationTokenManager.RemoveReportCancellationTokenSource("Cache_GetSystemCollection");
        }

        public async Task<int> GetSystemCountAsync()
        {
            if (_memoryCache.TryGetValue($"{nameof(Report)}_SystemCount", out int count))
            {
                return count;
            }
            else
            {
                using (await _lockManager.GetReportLockAsync($"Cache_GetSystemCount"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(Report)}_SystemCount", out count))
                    {
                        return count;
                    }

                    count = await _reportRepository.ReadSystemCountAsync();

                    _memoryCache.Set($"{nameof(Report)}_SystemCount", count, new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.Normal,
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(5)
                    });

                    return count;
                }
            }
        }

        public async Task RemoveSystemCountAsync()
        {
            _memoryCache.Remove($"{nameof(Report)}_SystemCount");
        }

        public async Task<Report> GetBoardAsync(string boardId, Guid reportId)
        {
            if (_memoryCache.TryGetValue($"{nameof(Report)}_Board_{boardId}_{reportId}", out Report report))
            {
                return report;
            }
            else
            {
                using (await _lockManager.GetReportLockAsync($"Cache_GetBoard_{boardId}_{reportId}"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(Report)}_{boardId}_Board_{reportId}", out report))
                    {
                        return report;
                    }

                    report = await _reportRepository.ReadBoardAsync(boardId, reportId);

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.Normal,
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(15)
                    };

                    var cancellationTokenSource = _cancellationTokenManager.GetReportCancellationTokenSource($"Cache_GetBoard_{boardId}");
                    memoryCacheEntryOptions.AddExpirationToken(new CancellationChangeToken(cancellationTokenSource.Token));

                    _memoryCache.Set($"{nameof(Report)}_Board_{boardId}_{reportId}", report, memoryCacheEntryOptions);

                    return report;
                }
            }
        }

        public async Task RemoveBoardAsync(string boardId, Guid reportId)
        {
            _memoryCache.Remove($"{nameof(Report)}_Board_{boardId}_{reportId}");
        }

        public async Task PurgeBoardAsync(string boardId)
        {
            _cancellationTokenManager.RemoveReportCancellationTokenSource($"Cache_GetBoard_{boardId}");
        }

        public async Task<IEnumerable<Report>> GetBoardIpCollectionAsync(string boardId, IpVersion ipVersion, BigInteger ipNumber)
        {
            if (_memoryCache.TryGetValue($"{nameof(Report)}_BoardIpCollection_{boardId}_{ipVersion}_{ipNumber}", out IEnumerable<Report> reportCollection))
            {
                return reportCollection;
            }
            else
            {
                using (await _lockManager.GetReportLockAsync($"Cache_GetBoardIpCollection_{boardId}_{ipVersion}_{ipNumber}"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(Report)}_BoardIpCollection_{boardId}_{ipVersion}_{ipNumber}", out reportCollection))
                    {
                        return reportCollection;
                    }

                    reportCollection = await _reportRepository.ReadBoardIPCollectionAsync(boardId, ipVersion, ipNumber);

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.Normal,
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(5)
                    };

                    var cancellationTokenSource = _cancellationTokenManager.GetReportCancellationTokenSource($"Cache_GetBoardIpCollection_{boardId}");
                    memoryCacheEntryOptions.AddExpirationToken(new CancellationChangeToken(cancellationTokenSource.Token));

                    _memoryCache.Set($"{nameof(Report)}_BoardIpCollection_{boardId}_{ipVersion}_{ipNumber}", reportCollection, memoryCacheEntryOptions);

                    return reportCollection;
                }
            }
        }

        public async Task RemoveBoardIpCollectionAsync(string boardId, IpVersion ipVersion, BigInteger ipNumber)
        {
            _memoryCache.Remove($"{nameof(Report)}_BoardIpCollection_{boardId}_{ipVersion}_{ipNumber}");
        }

        public async Task PurgeBoardIpCollectionAsync(string boardId)
        {
            _cancellationTokenManager.RemoveReportCancellationTokenSource($"Cache_GetBoardIpCollection_{boardId}");
        }

        public async Task<IEnumerable<Report>> GetBoardCollectionAsync(string boardId, int skip, int take)
        {
            if (_memoryCache.TryGetValue($"{nameof(Report)}_BoardCollection_{boardId}_{skip}_{take}", out IEnumerable<Report> reportCollection))
            {
                return reportCollection;
            }
            else
            {
                using (await _lockManager.GetReportLockAsync($"Cache_GetBoardCollection_{boardId}_{skip}_{take}"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(Report)}_BoardCollection_{boardId}_{skip}_{take}", out reportCollection))
                    {
                        return reportCollection;
                    }

                    reportCollection = await _reportRepository.ReadBoardCollectionAsync(boardId, skip, take);

                    var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.Normal,
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(5)
                    };

                    var cancellationTokenSource = _cancellationTokenManager.GetReportCancellationTokenSource($"Cache_BoardCollection_{boardId}");
                    memoryCacheEntryOptions.AddExpirationToken(new CancellationChangeToken(cancellationTokenSource.Token));

                    _memoryCache.Set($"{nameof(Report)}_BoardCollection_{boardId}_{skip}_{take}", reportCollection, memoryCacheEntryOptions);

                    return reportCollection;
                }
            }
        }

        public async Task PurgeBoardCollectionAsync(string boardId)
        {
            _cancellationTokenManager.RemoveReportCancellationTokenSource($"Cache_BoardCollection_{boardId}");
        }

        public async Task<int> GetBoardCountAsync(string boardId)
        {
            if (_memoryCache.TryGetValue($"{nameof(Report)}_BoardCount_{boardId}", out int count))
            {
                return count;
            }
            else
            {
                using (await _lockManager.GetReportLockAsync($"Cache_GetBoardCount_{boardId}"))
                {
                    if (_memoryCache.TryGetValue($"{nameof(Report)}_BoardCount_{boardId}", out count))
                    {
                        return count;
                    }

                    count = await _reportRepository.ReadBoardCountAsync(boardId);

                    _memoryCache.Set($"{nameof(Report)}_BoardCount_{boardId}", count, new MemoryCacheEntryOptions()
                    {
                        Priority = CacheItemPriority.Normal,
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(5)
                    });

                    return count;
                }
            }
        }

        public async Task RemoveBoardCountAsync(string boardId)
        {
            _memoryCache.Remove($"{nameof(Report)}_BoardCount_{boardId}");
        }
    }
}