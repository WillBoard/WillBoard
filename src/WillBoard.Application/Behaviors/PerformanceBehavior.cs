using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
using Microsoft.Extensions.Logging;

namespace WillBoard.Application.Behaviors
{
    public class PerformanceBehavior<TMessage, TResponse> : IPipelineBehavior<TMessage, TResponse> where TMessage : IMessage
    {
        private readonly ILogger _logger;

        public PerformanceBehavior(ILogger<PerformanceBehavior<TMessage, TResponse>> logger)
        {
            _logger = logger;
        }

        public async ValueTask<TResponse> Handle(TMessage message, MessageHandlerDelegate<TMessage, TResponse> next, CancellationToken cancellationToken)
        {
            var startTime = Stopwatch.GetTimestamp();

            var response = await next(message, cancellationToken);

            var timeSpanElapsed = Stopwatch.GetElapsedTime(startTime);

            _logger.LogInformation("Handled {0}. The execution took: {1}.", typeof(TMessage).Name, timeSpanElapsed.ToString(@"mm\:ss\.ffffff"));

            return response;
        }
    }
}
