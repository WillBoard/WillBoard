using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using WillBoard.Application.ViewModels;
using WillBoard.Core.Classes;
using WillBoard.Core.Consts;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Exceptions;
using WillBoard.Infrastructure.Services;
using WillBoard.Web.Models;

namespace WillBoard.Web.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private static readonly ActionDescriptor EmptyActionDescriptor = new ActionDescriptor();
        private static readonly RouteData EmptyRouteData = new RouteData();

        private readonly RequestDelegate _next;
        private readonly JsonService _jsonService;

        public ExceptionHandlerMiddleware(RequestDelegate next, JsonService jsonService)
        {
            _next = next;
            _jsonService = jsonService;
        }

        public async Task InvokeAsync(HttpContext context, ILogger<ExceptionHandlerMiddleware> logger)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Exception occurred during request.");
                await ExceptionHandlerAsync(context, exception);
            }
        }

        private Task ExceptionHandlerAsync(HttpContext context, Exception exception)
        {
            var status = 400;
            var code = "";
            var message = "";

            var exceptionType = exception.GetType();
            switch (exception)
            {
                case ErrorException e when exceptionType == typeof(ErrorException):
                    status = e.Status;
                    code = e.Code;
                    message = e.Message;
                    break;

                case Exception e when exceptionType == typeof(Exception):
                    status = 500;
                    break;

                default:
                    status = 500;
                    break;
            }

            var endpoint = context.GetEndpoint();

            if (endpoint == null)
            {
                return Task.CompletedTask;
            }

            var endpointResponse = endpoint.Metadata.GetMetadata<EndpointResponse>();

            if (endpointResponse == null)
            {
                return Task.CompletedTask;
            }

            if (endpointResponse.ContentType == EndpointContentType.JSON)
            {
                context.Response.StatusCode = status;
                context.Response.ContentType = ContentType.ApplicationJson;
                return context.Response.WriteAsync(_jsonService.SerializeError(new ExternalError(status, code, message)));
            }
            else if (endpointResponse.ContentType == EndpointContentType.SSE)
            {
                return context.Response.WriteAsync($"event:error{Environment.NewLine}");
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
                    Error = new ExternalError(status, code, message)
                };

                var result = new ViewResult()
                {
                    StatusCode = status,
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