using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using WillBoard.Application.ViewModels;
using WillBoard.Core.Classes;
using WillBoard.Core.Consts;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Infrastructure.Services;
using WillBoard.Web.Models;

namespace WillBoard.Web.Middlewares
{
    public class StatusCodeHandlerMiddleware
    {
        private static readonly ActionDescriptor EmptyActionDescriptor = new ActionDescriptor();
        private static readonly RouteData EmptyRouteData = new RouteData();

        private readonly RequestDelegate _next;
        private readonly JsonService _jsonService;

        public StatusCodeHandlerMiddleware(RequestDelegate next, JsonService jsonService)
        {
            _next = next;
            _jsonService = jsonService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            if (context.Response.HasStarted
                || context.Response.StatusCode < 400
                || context.Response.StatusCode >= 600
                || context.Response.ContentLength.HasValue
                || !string.IsNullOrEmpty(context.Response.ContentType))
            {
                return;
            }

            await StatusCodeHandlerAsync(context);
        }

        private Task StatusCodeHandlerAsync(HttpContext context)
        {
            var code = context.Response.StatusCode;

            var endpoint = context.GetEndpoint();

            var endpointResponse = endpoint?.Metadata?.GetMetadata<EndpointResponse>();

            if (endpointResponse?.ContentType == EndpointContentType.JSON)
            {
                context.Response.StatusCode = code;
                context.Response.ContentType = ContentType.ApplicationJson;
                return context.Response.WriteAsync(_jsonService.SerializeError(new ExternalError(code, "", "")));
            }
            else
            {
                var executor = context.RequestServices.GetService<IActionResultExecutor<ViewResult>>();

                if (executor == null)
                {
                    return Task.CompletedTask;
                }

                var routeData = context.GetRouteData() ?? EmptyRouteData;

                var actionContext = new ActionContext(context, routeData, EmptyActionDescriptor);

                var model = new ApplicationErrorViewModel()
                {
                    Title = "Error",
                    ViewType = ViewType.Application,
                    Error = new ExternalError(code, "", "")
                };

                var result = new ViewResult()
                {
                    StatusCode = code,
                    ViewName = "/Views/Application/Error.cshtml",
                    ViewData = new ViewDataDictionary(metadataProvider: new EmptyModelMetadataProvider(), modelState: new ModelStateDictionary())
                    {
                        Model = model
                    },
                    TempData = new MemoryTempDataDictionary()
                };

                return executor.ExecuteAsync(actionContext, result);
            }
        }
    }
}