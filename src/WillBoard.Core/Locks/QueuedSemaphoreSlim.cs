using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace WillBoard.Core.Locks
{
    public class QueuedSemaphoreSlim
    {
        private readonly SemaphoreSlim _semaphoreSlim;
        private readonly ConcurrentQueue<TaskCompletionSource<bool>> _concurrentQueue = new ConcurrentQueue<TaskCompletionSource<bool>>();

        public QueuedSemaphoreSlim(int initialCount)
        {
            _semaphoreSlim = new SemaphoreSlim(initialCount);
        }

        public QueuedSemaphoreSlim(int initialCount, int maxCount)
        {
            _semaphoreSlim = new SemaphoreSlim(initialCount, maxCount);
        }

        public Task WaitAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            _concurrentQueue.Enqueue(taskCompletionSource);

            _semaphoreSlim.WaitAsync().ContinueWith(t =>
            {
                if (_concurrentQueue.TryDequeue(out var item))
                {
                    item.SetResult(true);
                }
            });

            return taskCompletionSource.Task;
        }

        public void Release()
        {
            _semaphoreSlim.Release();
        }
    }
}