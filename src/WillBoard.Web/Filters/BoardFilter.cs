using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Linq;
using System.Threading.Tasks;
using WillBoard.Application.ViewModels;
using WillBoard.Core.Consts;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Managers;
using WillBoard.Web.Models;
using WillBoard.Web.Services;

namespace WillBoard.Web.Filters
{
    public class BoardFilterAttribute : TypeFilterAttribute
    {
        public BoardFilterAttribute() : base(typeof(BoardFilter))
        {
        }

        private class BoardFilter : IAsyncActionFilter
        {
            private readonly IpManager _ipManager;
            private readonly BoardManager _boardManager;
            private readonly IBoardCache _boardCache;
            private readonly ErrorService _errorService;

            public BoardFilter(IpManager ipManager, BoardManager boardManager, IBoardCache boardCache, ErrorService errorService)
            {
                _ipManager = ipManager;
                _boardManager = boardManager;
                _boardCache = boardCache;
                _errorService = errorService;
            }

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                var endpoint = context.HttpContext.GetEndpoint();
                var endpointResponse = endpoint?.Metadata?.GetMetadata<EndpointResponse>();
                var contentType = endpointResponse?.ContentType ?? EndpointContentType.HTML;

                if (!context.RouteData.Values.TryGetValue("boardId", out var routeDataValue))
                {
                    context.Result = await _errorService.ApplicationErrorAsync(new InternalError(404, TranslationKey.ErrorNotFound), contentType);
                    return;
                }

                var boardId = routeDataValue as string;

                if (boardId is null)
                {
                    context.Result = await _errorService.ApplicationErrorAsync(new InternalError(404, TranslationKey.ErrorNotFound), contentType);
                    return;
                }

                var board = await _boardCache.GetAsync(boardId);

                if (board is null || board.Availability is false)
                {
                    context.Result = await _errorService.ApplicationErrorAsync(new InternalError(404, TranslationKey.ErrorNotFound), contentType);
                    return;
                }

                _boardManager.SetBoard(board);

                if (board.Accessibility is BoardAccessibility.Password)
                {
                    var cookiePassword = context.HttpContext.Request.Cookies["password_" + board.BoardId];
                    if (board.AccessibilityPassword != cookiePassword)
                    {
                        context.HttpContext.Response.Cookies.Delete("password_" + board.BoardId);

                        if (contentType is EndpointContentType.JSON)
                        {
                            context.Result = await _errorService.ApplicationErrorAsync(new InternalError(403, TranslationKey.ErrorForbidden), EndpointContentType.JSON);
                            return;
                        }
                        else
                        {
                            context.Result = new ViewResult
                            {
                                ViewName = "/Views/Board/SecurePassword.cshtml",
                                ViewData = new ViewDataDictionary(metadataProvider: new EmptyModelMetadataProvider(), modelState: new ModelStateDictionary())
                                {
                                    Model = new BoardViewModel()
                                    {
                                        Title = "Login",
                                    }
                                },
                                TempData = null
                            };
                            return;
                        }
                    }
                }

                if (board.Accessibility is BoardAccessibility.Ip)
                {
                    var ipVersion = _ipManager.GetIpVersion();
                    var ipNumber = _ipManager.GetIpNumber();

                    if (ipVersion == IpVersion.None || (ipVersion == IpVersion.IpVersion4 && !board.AccessibilityIpVersion4NumberCollection.Any(x => x == ipNumber)) || (ipVersion == IpVersion.IpVersion6 && !board.AccessibilityIpVersion6NumberCollection.Any(x => x == ipNumber)))
                    {
                        context.Result = await _errorService.ApplicationErrorAsync(new InternalError(404, TranslationKey.ErrorNotFound), EndpointContentType.HTML);
                        return;
                    }
                }

                await next();
            }
        }
    }
}