using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Enums;
using WillBoard.Core.Interfaces.Services;

namespace WillBoard.Core.Classes
{
    public abstract class SynchronizationSubscription : IDisposable
    {
        public Guid Id { get; set; }
        public DateTime Creation { get; set; }
        public IpVersion IpVersion { get; set; }
        public UInt128 IpNumber { get; set; }

        public abstract void Notify(object data);
        public abstract void Dispose();
    }

    public class SynchronizationSubscription<T> : SynchronizationSubscription where T : class
    {
        private readonly ISynchronizationService _synchronizationService;
        private readonly object _deliveryQueueLock = new object();
        private readonly Queue<T> _deliveryQueue = new Queue<T>();
        private readonly SemaphoreSlim _signal = new SemaphoreSlim(0, int.MaxValue);

        public SynchronizationSubscription(ISynchronizationService synchronizationService)
        {
            _synchronizationService = synchronizationService;
        }

        public override void Notify(object data)
        {
            T genericData = data as T;

            if (genericData != null)
            {
                Notify(genericData);
            }
        }

        public void Notify(T data)
        {
            lock (_deliveryQueueLock)
            {
                _deliveryQueue.Enqueue(data);
            }

            _signal.Release();
        }

        public async Task<T> WaitForData()
        {
            await _signal.WaitAsync();

            lock (_deliveryQueueLock)
            {
                return _deliveryQueue.Dequeue();
            }
        }

        public override void Dispose()
        {
            _synchronizationService.Unsubscribe(this);
            _signal.Dispose();
        }
    }

    public class SynchronizationMessage
    {
        public SynchronizationEvent Event { get; set; }
        public object Data { get; set; }
    }

    public class AdministrationSynchronizationMessage
    {
        public SynchronizationEvent Event { get; set; }
        public object Data { get; set; }
    }
}