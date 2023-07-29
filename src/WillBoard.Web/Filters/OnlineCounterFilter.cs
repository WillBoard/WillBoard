using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;
using WillBoard.Core.Interfaces.Services;
using WillBoard.Core.Managers;

namespace WillBoard.Web.Filters
{
    public class OnlineCounterFilterAttribute : TypeFilterAttribute
    {
        public OnlineCounterFilterAttribute() : base(typeof(OnlineCounterFilter))
        {
        }

        private class OnlineCounterFilter : IAsyncActionFilter
        {
            private readonly IpManager _ipManager;
            private readonly IOnlineCounterService _counterService;

            public OnlineCounterFilter(IpManager ipManager, IOnlineCounterService counterService)
            {
                _ipManager = ipManager;
                _counterService = counterService;
            }

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                _counterService.AddOrUpdate(_ipManager.GetIpVersion(), _ipManager.GetIpNumber());

                await next();
            }
        }
    }
}