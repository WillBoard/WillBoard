using System;
using System.Collections.Concurrent;
using System.Linq;
using WillBoard.Core.Enums;
using WillBoard.Core.Interfaces.Providers;
using WillBoard.Core.Interfaces.Services;

namespace WillBoard.Infrastructure.Services.Instance
{
    public class InstanceOnlineCounterService : IOnlineCounterService
    {
        private readonly ConcurrentDictionary<string, DateTime> _visitorDictionary = new ConcurrentDictionary<string, DateTime>();

        private readonly IDateTimeProvider _dateTimeProvider;

        public InstanceOnlineCounterService(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public int CountAndAddOrUpdate(IpVersion ipVersion, UInt128 ipNumber)
        {
            AddOrUpdate(ipVersion, ipNumber);

            return Count();
        }

        public int Count()
        {
            foreach (var visitor in _visitorDictionary.Where(visitor => visitor.Value < _dateTimeProvider.UtcNow.AddMinutes(-15)))
            {
                _visitorDictionary.TryRemove(visitor.Key, out DateTime value);
            }

            return _visitorDictionary.Count;
        }

        public void AddOrUpdate(IpVersion ipVersion, UInt128 ipNumber)
        {
            _visitorDictionary.AddOrUpdate($"{ipVersion}_{ipNumber}", _dateTimeProvider.UtcNow, (key, value) => _dateTimeProvider.UtcNow);
        }
    }
}