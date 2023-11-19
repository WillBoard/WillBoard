using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using WillBoard.Application.Api.Queries.GetPost;
using WillBoard.Application.Api.Queries.GetReplies;
using WillBoard.Application.Api.Queries.GetVerification;
using WillBoard.Application.Board.Commands.CreatePost;
using WillBoard.Application.Board.Commands.DeleteFile;
using WillBoard.Application.Board.Commands.DeletePost;
using WillBoard.Application.Board.Commands.ReportBoard;
using WillBoard.Application.Board.Commands.ReportSystem;
using WillBoard.Core.Classes;
using WillBoard.Core.Consts;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Extensions;
using WillBoard.Core.Interfaces.Services;
using WillBoard.Core.Managers;
using WillBoard.Infrastructure.Services;
using WillBoard.Web.Filters;
using WillBoard.Web.Models;
using WillBoard.Web.Services;

namespace WillBoard.Web.Controllers
{
    public class ApiController : Controller
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        private readonly IpManager _ipManager;
        private readonly BoardManager _boardManager;
        private readonly ISynchronizationService _synchronizationService;
        private readonly IClassicCaptchaService _classicCaptchaService;
        private readonly JsonService _jsonService;
        private readonly ErrorService _errorService;

        public ApiController(
            ILogger<ApiController> logger,
            IMediator mediator,
            IpManager ipManager,
            BoardManager boardManager,
            ISynchronizationService synchronizationService,
            IClassicCaptchaService classicCaptchaService,
            JsonService jsonService,
            ErrorService errorService)
        {
            _logger = logger;
            _mediator = mediator;
            _ipManager = ipManager;
            _boardManager = boardManager;
            _synchronizationService = synchronizationService;
            _classicCaptchaService = classicCaptchaService;
            _jsonService = jsonService;
            _errorService = errorService;
        }

        [HttpGet]
        public IActionResult Captcha()
        {
            ReadOnlySpan<byte> image = _classicCaptchaService.GenerateCaptha(out string captchaValue);
            string captchaKey = _classicCaptchaService.AddCaptcha(captchaValue, out DateTime captchaStart, out DateTime captchaEnd);
            string imageBase64Data = Convert.ToBase64String(image);

            var data = new
            {
                captchaKey = captchaKey,
                captchaImage = string.Format("data:image/png;base64,{0}", imageBase64Data),
                captchaStart = captchaStart.ToIso8601String(),
                captchaEnd = captchaEnd.ToIso8601String()
            };

            var json = _jsonService.SerializeData(data);

            return Content(json, ContentType.ApplicationJson, Encoding.UTF8);
        }

        [HttpGet]
        public IActionResult CaptchaWildcard()
        {
            ReadOnlySpan<byte> image = _classicCaptchaService.GenerateCaptha(out string captchaValue);
            string captchaKey = _classicCaptchaService.AddCaptcha(captchaValue, out DateTime captchaStart, out DateTime captchaEnd);
            string imageBase64Data = Convert.ToBase64String(image);

            string html =
$@"<!doctype html>
<html>
<head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <link rel=""stylesheet"" href=""/css/captcha-wildcard.css"">
</style>
</head>
<body>
    <form enctype=""multipart/form-data"" action=""/api/captcha/wildcard"" method=""post"">
        <input type=""hidden"" name=""captchaKey"" value=""{captchaKey}"" autocomplete=""off"">
        <img src=""data:image/png;base64,{imageBase64Data}"">
        <input type=""text"" name=""captchaValue"" autocomplete=""off"" placeholder=""Captcha"">
        <button type=""submit"">Verify</button>
        <p>This captcha is valid for 5 minutes <small></small><small>({captchaStart.ToRfc3339String()} UTC - {captchaEnd.ToRfc3339String()} UTC)</small></p>
    </form>
</body>
</html>";

            return Content(html, ContentType.TextHtml, Encoding.UTF8);
        }

        [HttpPost]
        public IActionResult CaptchaWildcard(string captchaKey, string captchaValue)
        {
            if (_classicCaptchaService.Verify(captchaKey, captchaValue))
            {
                string captchaWildcard = _classicCaptchaService.AddWildcard(out DateTime captchaStart, out DateTime captchaEnd);

                string html =
$@"<!doctype html>
<html>
<head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <link rel=""stylesheet"" href=""/css/captcha-wildcard.css"">
</head>
<body>
    <p>Copy this code and paste it below</p>
    <textarea>{captchaWildcard}</textarea>
    <p>This code is valid for 5 minutes<small>({captchaStart.ToRfc3339String()} UTC - {captchaEnd.ToRfc3339String()} UTC)</small></p>
</body>
</html>";

                return Content(html, ContentType.TextHtml, Encoding.UTF8);
            }

            return RedirectToAction("CaptchaWildcard");
        }

        [HttpGet]
        [BoardFilter]
        public async Task<IActionResult> GetVerificationAsync([FromRoute] string boardId, [FromRoute] bool thread)
        {
            var query = new GetVerificationQuery()
            {
                BoardId = boardId,
                Thread = thread
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                return await _errorService.BoardErrorAsync(result.Error, EndpointContentType.JSON);
            }

            var json = _jsonService.SerializeData(result.Value);

            return Content(json, ContentType.ApplicationJson, Encoding.UTF8);
        }

        [HttpPost]
        [BoardFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> CreatePostAsync([FromRoute] string boardId, [FromForm] CreatePostCommand command)
        {
            if (boardId != command.BoardId)
            {
                return await _errorService.BoardErrorAsync(new InternalError(400, TranslationKey.ErrorBadRequest), EndpointContentType.JSON);
            }

            var userAgentHeader = Request.Headers["User-Agent"].ToString();
            command.UserAgent = userAgentHeader.Substring(0, Math.Min(userAgentHeader.Length, 256));

            var board = _boardManager.GetBoard();

            if (board.FieldVerificationType == VerificationType.ReCaptcha)
            {
                command.VerificationValue = Request.Form["g-recaptcha-response"].ToString();
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.BoardErrorAsync(result.Error, EndpointContentType.JSON);
            }

            var json = _jsonService.SerializeData(result.Value);

            return Content(json, ContentType.ApplicationJson, Encoding.UTF8);
        }

        [HttpGet]
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

        [HttpPost]
        [BoardFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> ReportBoardAsync([FromRoute] string boardId, [FromRoute] int postId, [FromForm] ReportBoardCommand command)
        {
            if (boardId != command.BoardId || postId != command.PostId)
            {
                return await _errorService.BoardErrorAsync(new InternalError(400, TranslationKey.ErrorBadRequest), EndpointContentType.JSON);
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.BoardErrorAsync(result.Error, EndpointContentType.JSON);
            }

            return NoContent();
        }

        [HttpPost]
        [BoardFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> ReportSystemAsync([FromRoute] string boardId, [FromRoute] int postId, [FromForm] ReportSystemCommand command)
        {
            if (boardId != command.BoardId || postId != command.PostId)
            {
                return await _errorService.BoardErrorAsync(new InternalError(400, TranslationKey.ErrorBadRequest), EndpointContentType.JSON);
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.BoardErrorAsync(result.Error, EndpointContentType.JSON);
            }

            return NoContent();
        }

        [HttpPost]
        [BoardFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> DeletePostAsync([FromRoute] string boardId, [FromRoute] int postId, [FromForm] DeletePostCommand command)
        {
            if (boardId != command.BoardId || postId != command.PostId)
            {
                return await _errorService.BoardErrorAsync(new InternalError(400, TranslationKey.ErrorBadRequest), EndpointContentType.JSON);
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.BoardErrorAsync(result.Error, EndpointContentType.JSON);
            }

            return NoContent();
        }

        [HttpPost]
        [BoardFilter]
        [CsrfFilter(Csrf.Check)]
        public async Task<IActionResult> DeleteFileAsync([FromRoute] string boardId, [FromRoute] int postId, [FromForm] DeleteFileCommand command)
        {
            if (boardId != command.BoardId || postId != command.PostId)
            {
                return await _errorService.BoardErrorAsync(new InternalError(400, TranslationKey.ErrorBadRequest), EndpointContentType.JSON);
            }

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return await _errorService.BoardErrorAsync(result.Error, EndpointContentType.JSON);
            }

            return NoContent();
        }

        [HttpGet]
        [BoardFilter]
        public async Task SynchronizationAsync([FromRoute] string boardId)
        {
            var board = _boardManager.GetBoard();

            if (!board.SynchronizationBoardAvailability && !board.SynchronizationThreadAvailability)
            {
                HttpContext.Response.StatusCode = 404;
                return;
            }

            HttpContext.Response.ContentType = ContentType.TextEventStream;

            using (var subscription = _synchronizationService.Subscribe<SynchronizationMessage>(_ipManager.GetIpVersion(), _ipManager.GetIpNumber(), board.BoardId))
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
                    _logger.LogError(exception, $"Exception occurred during {0} method.", nameof(SynchronizationAsync));
                    _synchronizationService.Unsubscribe(subscription);
                }
            }
        }
    }
}