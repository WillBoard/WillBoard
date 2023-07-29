using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using WillBoard.Core.Classes;
using WillBoard.Core.Enums;
using WillBoard.Core.Interfaces.Providers;
using WillBoard.Core.Interfaces.Services;

namespace WillBoard.Infrastructure.Services.Instance
{
    public class InstanceSynchronizationService : ISynchronizationService
    {
        private readonly ConcurrentDictionary<SynchronizationSubscription, string[]> _synchronizationSubscriptionCollection = new ConcurrentDictionary<SynchronizationSubscription, string[]>();

        private readonly IOnlineCounterService _onlineCounterService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public InstanceSynchronizationService(IOnlineCounterService onlineCounterService, IDateTimeProvider dateTimeProvider)
        {
            _onlineCounterService = onlineCounterService;
            _dateTimeProvider = dateTimeProvider;

            // Using Thread for long running work instead of Task
            // https://github.com/davidfowl/AspNetCoreDiagnosticScenarios/blob/master/AsyncGuidance.md#avoid-using-taskrun-for-long-running-work-that-blocks-the-thread
            var thread = new Thread(Heartbeat)
            {
                IsBackground = true
            };

            thread.Start();
        }

        private void Heartbeat()
        {
            while (true)
            {
                foreach (var synchronizationSubscription in _synchronizationSubscriptionCollection)
                {
                    _onlineCounterService.AddOrUpdate(synchronizationSubscription.Key.IpVersion, synchronizationSubscription.Key.IpNumber);
                }

                var online = _onlineCounterService.Count();

                var synchronizationMessage = new SynchronizationMessage()
                {
                    Event = SynchronizationEvent.Heartbeat,
                    Data = new { online }
                };

                var administrationSynchronizationMessage = new AdministrationSynchronizationMessage()
                {
                    Event = SynchronizationEvent.Heartbeat,
                    Data = new { online }
                };

                Notify(synchronizationMessage, "heartbeat");
                Notify(administrationSynchronizationMessage, "heartbeat");

                Thread.Sleep(30_000);
            }
        }

        public void Notify<T>(T synchronizationMessage, params string[] synchronizationNameCollection) where T : class
        {
            if (synchronizationNameCollection == null)
            {
                throw new ArgumentNullException(nameof(synchronizationNameCollection));
            }

            foreach (var synchronizationName in synchronizationNameCollection)
            {
                if (synchronizationName == "heartbeat")
                {
                    foreach (var subscription in _synchronizationSubscriptionCollection)
                    {
                        subscription.Key.Notify(synchronizationMessage);
                    }
                }
                else
                {
                    foreach (var subscription in _synchronizationSubscriptionCollection.Where(k => k.Value.Any(v => v == synchronizationName)))
                    {
                        subscription.Key.Notify(synchronizationMessage);
                    }
                }
            }
        }

        public SynchronizationSubscription<T> Subscribe<T>(IpVersion ipVersion, BigInteger ipNumber, params string[] synchronizationNameCollection) where T : class
        {
            if (synchronizationNameCollection == null)
            {
                throw new ArgumentNullException(nameof(synchronizationNameCollection));
            }

            var subscription = new SynchronizationSubscription<T>(this)
            {
                Id = Guid.NewGuid(),
                Creation = _dateTimeProvider.UtcNow,
                IpVersion = ipVersion,
                IpNumber = ipNumber
            };

            _synchronizationSubscriptionCollection.TryAdd(subscription, synchronizationNameCollection);

            return subscription;
        }

        public void Unsubscribe(SynchronizationSubscription synchronizationSubscription)
        {
            _synchronizationSubscriptionCollection.TryRemove(synchronizationSubscription, out string[] _);
        }

        public IDictionary<SynchronizationSubscription, string[]> GetSynchronizationSubscriptionCollection()
        {
            return _synchronizationSubscriptionCollection;
        }
    }
}