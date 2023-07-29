using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Errors;
using WillBoard.Web.Models;
using WillBoard.Web.Services;

namespace WillBoard.Web.Filters
{
    public class CsrfFilterAttribute : TypeFilterAttribute
    {
        public CsrfFilterAttribute(Csrf csrf) : base(typeof(CsrfFilter))
        {
            Arguments = new object[] { csrf };
        }

        private class CsrfFilter : IAsyncActionFilter
        {
            private readonly Csrf _csrf;
            private readonly ErrorService _errorService;

            public CsrfFilter(Csrf csrf, ErrorService errorService)
            {
                _csrf = csrf;
                _errorService = errorService;
            }

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                if (_csrf == Csrf.Set)
                {
                    var controller = context.Controller as Controller;
                    if (controller == null)
                    {
                        return;
                    }

                    if (Guid.TryParseExact(context.HttpContext.Request.Cookies["csrf"], "N", out Guid guidOutput))
                    {
                        controller.ViewData["csrf"] = guidOutput.ToString("N");
                    }
                    else
                    {
                        var csrf = Guid.NewGuid().ToString("N");
                        CookieOptions options = new CookieOptions
                        {
                            HttpOnly = true,
                            SameSite = SameSiteMode.Strict
                        };
                        context.HttpContext.Response.Cookies.Append("csrf", csrf, options);
                        controller.ViewData["csrf"] = csrf;
                    }
                }

                if (_csrf == Csrf.Check)
                {
                    var endpoint = context.HttpContext.GetEndpoint();

                    if (endpoint == null)
                    {
                        return;
                    }

                    var endpointResponse = endpoint.Metadata.GetMetadata<EndpointResponse>();

                    if (endpointResponse == null)
                    {
                        return;
                    }

                    var cookieCSRF = context.HttpContext.Request.Cookies["csrf"];

                    if (string.IsNullOrEmpty(cookieCSRF))
                    {
                        context.Result = await _errorService.ApplicationErrorAsync(new InternalError(400, TranslationKey.ErrorCsrf), endpointResponse.ContentType);
                        return;
                    }

                    if (!context.HttpContext.Request.HasFormContentType)
                    {
                        context.Result = await _errorService.ApplicationErrorAsync(new InternalError(400, TranslationKey.ErrorCsrf), endpointResponse.ContentType);
                        return;
                    }

                    var formCsrf = context.HttpContext.Request.Form["csrf"];

                    if (string.IsNullOrEmpty(formCsrf) || cookieCSRF != formCsrf)
                    {
                        context.Result = await _errorService.ApplicationErrorAsync(new InternalError(400, TranslationKey.ErrorCsrf), endpointResponse.ContentType);
                        return;
                    }
                }

                await next();
            }
        }
    }

    public enum Csrf
    {
        Set,
        Check
    }
}