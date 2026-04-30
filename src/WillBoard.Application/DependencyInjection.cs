using Mediator;
using Microsoft.Extensions.DependencyInjection;
using WillBoard.Application.Behaviors;

namespace WillBoard.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediator(options =>
            {
                options.Namespace = "WillBoard.Mediator";
                options.ServiceLifetime = ServiceLifetime.Transient;
            });

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));

            return services;
        }
    }
}
