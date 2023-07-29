using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using WillBoard.Application.AdministrationApi.Queries.GetPost;
using WillBoard.Application.AdministrationApi.Queries.GetReplies;
using WillBoard.Core.Classes;
using WillBoard.Core.Consts;
using WillBoard.Core.Interfaces.Services;
using WillBoard.Core.Managers;
using WillBoard.Infrastructure.Services;
using WillBoard.Web.Filters;
using WillBoard.Web.Models;
using WillBoard.Web.Services;

namespace WillBoard.Web.Controllers
{
    public class AdministrationApiController : Controller
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        private readonly ErrorService _errorService;
        private readonly IpManager _ipManager;
        private readonly AccountManager _accountManager;
        private readonly BoardManager _boardManager;
        private readonly IClassicCaptchaService _classicCaptchaService;
        private readonly ISynchronizationService _synchronizationService;
        private readonly JsonService _jsonService;

        public AdministrationApiController(
            ILogger<AdministrationApiController> logger,
            IMediator mediator,
            ErrorService errorService,
            IpManager ipManager,
            AccountManager accountManager,
            BoardManager boardManager,
            IClassicCaptchaService classicCaptchaService,
            ISynchronizationService synchronizationService,
            JsonService jsonService)
        {
            _logger = logger;
            _mediator = mediator;
            _errorService = errorService;
            _ipManager = ipManager;
            _accountManager = accountManager;
            _boardManager = boardManager;
            _classicCaptchaService = classicCaptchaService;
            _synchronizationService = synchronizationService;
            _jsonService = jsonService;
        }

        [HttpGet]
        [AuthenticationFilter]
        [BoardFilter]
        public async Task<IActionResult> GetPostAsync([FromRoute] string boardId, [FromRoute] int postId)
        {
            var query = new GetPostQuery()
            {
                BoardId = boardId,
                PostId = postId
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.BoardErrorAsync(result.Error, EndpointContentType.JSON);
            }

            var json = _jsonService.SerializeData(result.Value);

            return Content(json, ContentType.ApplicationJson, Encoding.UTF8);
        }

        [HttpGet]
        [AuthenticationFilter]
        [BoardFilter]
        public async Task<IActionResult> GetRepliesAsync([FromRoute] string boardId, [FromRoute] int threadId, [FromRoute] int? last)
        {
            var query = new GetRepliesQuery()
            {
                BoardId = boardId,
                ThreadId = threadId,
                Last = last
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.BoardErrorAsync(result.Error, EndpointContentType.JSON);
            }

            var json = _jsonService.SerializeData(result.Value);

            return Content(json, ContentType.ApplicationJson, Encoding.UTF8);
        }

        [HttpGet]
        [AuthenticationFilter]
        [BoardFilter]
        public async Task SynchronizationAsync([FromRoute] string boardId)
        {
            var board = _boardManager.GetBoard();

            if (!board.SynchronizationBoardAvailability && !board.SynchronizationThreadAvailability)
            {
                HttpContext.Response.StatusCode = 404;
                return;
            }

            if (!_accountManager.CheckPermission(board.BoardId, e => e.PermissionBoardView))
            {
                HttpContext.Response.StatusCode = 403;
                return;
            }

            HttpContext.Response.ContentType = ContentType.TextEventStream;

            using (var subscription = _synchronizationService.Subscribe<AdministrationSynchronizationMessage>(_ipManager.GetIpVersion(), _ipManager.GetIpNumber(), board.BoardId))
            {
                try
                {
                    await HttpContext.Response.WriteAsync($"event:open{Environment.NewLine}{Environment.NewLine}");
                    await HttpContext.Response.Body.FlushAsync();

                    do
                    {
                        var synchronizationMessage = await subscription.WaitForData();
                        if (synchronizationMessage != null)
                        {
                            var synchronizationMessageEvent = _jsonService.SerializeSynchronizationMessage(synchronizationMessage.Event, synchronizationMessage.Data);
                            await HttpContext.Response.WriteAsync(synchronizationMessageEvent);
                            await HttpContext.Response.Body.FlushAsync();
                        }
                    }
                    while (!HttpContext.RequestAborted.IsCancellationRequested);

                    _synchronizationService.Unsubscribe(subscription);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, $"Exception occured during {0} method.", nameof(SynchronizationAsync));
                    _synchronizationService.Unsubscribe(subscription);
                }
            }
        }
    }
}