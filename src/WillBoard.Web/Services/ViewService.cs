using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WillBoard.Application.Administration.Queries.BoardViewClassic;
using WillBoard.Application.Board.Queries.Classic;
using WillBoard.Application.ViewModels;
using WillBoard.Core.Consts;
using WillBoard.Core.Interfaces.Services;
using WillBoard.Core.Managers;

namespace WillBoard.Web.Services
{
    public class ViewService
    {
        private readonly AccountManager _accountManager;
        private readonly BoardManager _boardManager;
        private readonly ILocalizationService _localizationService;

        private readonly JsonWriterOptions _jsonWriterOptions = new JsonWriterOptions
        {
            Indented = true
        };

        public ViewService(AccountManager accountManager, BoardManager boardManager, ILocalizationService localizationService)
        {
            _accountManager = accountManager;
            _boardManager = boardManager;
            _localizationService = localizationService;
        }

        public string GenerateViewConfiguration(BoardViewModel model)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream, _jsonWriterOptions))
                {
                    writer.WriteStartObject();
                    writer.WriteNumber("viewType", ((uint)model.ViewType));
                    writer.WriteNumber("boardViewType", ((uint)model.BoardViewType));

                    var classicViewModel = model as ClassicViewModel;
                    if (classicViewModel is not null)
                    {
                        writer.WriteNumber("pageCurrent", ((uint)classicViewModel.PageCurrent));
                    }

                    var boardViewClassicViewModel = model as BoardViewClassicViewModel;
                    if (boardViewClassicViewModel is not null)
                    {
                        writer.WriteNumber("pageCurrent", ((uint)boardViewClassicViewModel.PageCurrent));
                    }

                    writer.WriteEndObject();
                }

                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        public string GenerateBoardConfiguration()
        {
            var board = _boardManager.GetBoard();

            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream, _jsonWriterOptions))
                {
                    writer.WriteStartObject();
                    writer.WriteString("boardId", board.BoardId);
                    writer.WriteString("name", board.Name);
                    writer.WriteBoolean("onlineCounterAvailability", board.OnlineCounterAvailability);

                    writer.WriteStartObject("cssThemeCollection");
                    foreach (var theme in board.CssThemeCollection)
                    {
                        writer.WriteStartObject(theme.ThemeId);
                        writer.WriteString("name", theme.Name);
                        writer.WriteString("path", theme.Path);
                        writer.WriteEndObject();
                    }
                    writer.WriteEndObject();

                    writer.WriteNumber("threadFilePreviewHeightMax", board.ThreadFilePreviewHeightMax);
                    writer.WriteNumber("threadFilePreviewWidthMax", board.ThreadFilePreviewWidthMax);
                    writer.WriteNumber("replyFilePreviewHeightMax", board.ReplyFilePreviewHeightMax);
                    writer.WriteNumber("replyFilePreviewWidthMax", board.ReplyFilePreviewWidthMax);

                    writer.WriteNumber("fieldVerificationType", (int)board.FieldVerificationType);

                    writer.WriteNumber("threadReplyPreviewMax", board.ThreadReplyPreviewMax);
                    writer.WriteNumber("threadPinReplyPreviewMax", board.ThreadPinReplyPreviewMax);

                    writer.WriteBoolean("synchronizationBoard", board.SynchronizationBoardAvailability);
                    writer.WriteBoolean("synchronizationThread", board.SynchronizationThreadAvailability);

                    writer.WriteNumber("threadTimeMin", board.ThreadTimeMin);
                    writer.WriteNumber("replyTimeMin", board.ReplyTimeMin);

                    writer.WriteStartObject("fieldCollection");

                    writer.WriteStartArray("thread");

                    writer.WriteStartObject();
                    writer.WriteString("name", "name");
                    writer.WriteBoolean("availability", board.ThreadFieldNameAvailability);
                    if (board.ThreadFieldNameAvailability)
                    {
                        writer.WriteBoolean("requirement", board.ThreadFieldNameRequirement);
                        writer.WriteNumber("lengthMin", board.ThreadFieldNameLengthMin);
                        writer.WriteNumber("lengthMax", board.ThreadFieldNameLengthMax);
                    }
                    writer.WriteEndObject();

                    writer.WriteStartObject();
                    writer.WriteString("name", "email");
                    writer.WriteBoolean("availability", board.ThreadFieldEmailAvailability);
                    if (board.ThreadFieldEmailAvailability)
                    {
                        writer.WriteBoolean("requirement", board.ThreadFieldEmailRequirement);
                        writer.WriteNumber("lengthMin", board.ThreadFieldEmailLengthMin);
                        writer.WriteNumber("lengthMax", board.ThreadFieldEmailLengthMax);
                    }
                    writer.WriteEndObject();

                    writer.WriteStartObject();
                    writer.WriteString("name", "subject");
                    writer.WriteBoolean("availability", board.ThreadFieldSubjectAvailability);
                    if (board.ThreadFieldSubjectAvailability)
                    {
                        writer.WriteBoolean("requirement", board.ThreadFieldSubjectRequirement);
                        writer.WriteNumber("lengthMin", board.ThreadFieldSubjectLengthMin);
                        writer.WriteNumber("lengthMax", board.ThreadFieldSubjectLengthMax);
                    }
                    writer.WriteEndObject();

                    writer.WriteStartObject();
                    writer.WriteString("name", "message");
                    writer.WriteBoolean("availability", board.ThreadFieldMessageAvailability);
                    if (board.ThreadFieldMessageAvailability)
                    {
                        writer.WriteBoolean("requirement", board.ThreadFieldMessageRequirement);
                        writer.WriteNumber("lengthMin", board.ThreadFieldMessageLengthMin);
                        writer.WriteNumber("lengthMax", board.ThreadFieldMessageLengthMax);
                    }
                    writer.WriteEndObject();

                    writer.WriteStartObject();
                    writer.WriteString("name", "file");
                    writer.WriteBoolean("availability", board.ThreadFieldFileAvailability);
                    if (board.ThreadFieldMessageAvailability)
                    {
                        writer.WriteBoolean("requirement", board.ThreadFieldFileRequirement);
                        writer.WriteString("typeCollection", string.Join(", ", board.ThreadFieldFileTypeCollection));
                    }
                    writer.WriteEndObject();

                    writer.WriteStartObject();
                    writer.WriteString("name", "password");
                    writer.WriteBoolean("availability", board.ThreadFieldPasswordAvailability);
                    if (board.ThreadFieldPasswordAvailability)
                    {
                        writer.WriteBoolean("requirement", board.ThreadFieldPasswordRequirement);
                        writer.WriteNumber("lengthMin", board.ThreadFieldPasswordLengthMin);
                        writer.WriteNumber("lengthMax", board.ThreadFieldPasswordLengthMax);
                    }
                    writer.WriteEndObject();

                    writer.WriteEndArray();

                    writer.WriteStartArray("reply");

                    writer.WriteStartObject();
                    writer.WriteString("name", "name");
                    writer.WriteBoolean("availability", board.ReplyFieldNameAvailability);
                    if (board.ReplyFieldNameAvailability)
                    {
                        writer.WriteBoolean("requirement", board.ReplyFieldNameRequirement);
                        writer.WriteNumber("lengthMin", board.ReplyFieldNameLengthMin);
                        writer.WriteNumber("lengthMax", board.ReplyFieldNameLengthMax);
                    }
                    writer.WriteEndObject();

                    writer.WriteStartObject();
                    writer.WriteString("name", "email");
                    writer.WriteBoolean("availability", board.ReplyFieldEmailAvailability);
                    if (board.ReplyFieldEmailAvailability)
                    {
                        writer.WriteBoolean("requirement", board.ReplyFieldEmailRequirement);
                        writer.WriteNumber("lengthMin", board.ReplyFieldEmailLengthMin);
                        writer.WriteNumber("lengthMax", board.ReplyFieldEmailLengthMax);
                    }
                    writer.WriteEndObject();

                    writer.WriteStartObject();
                    writer.WriteString("name", "subject");
                    writer.WriteBoolean("availability", board.ReplyFieldSubjectAvailability);
                    if (board.ReplyFieldSubjectAvailability)
                    {
                        writer.WriteBoolean("requirement", board.ReplyFieldSubjectRequirement);
                        writer.WriteNumber("lengthMin", board.ReplyFieldSubjectLengthMin);
                        writer.WriteNumber("lengthMax", board.ReplyFieldSubjectLengthMax);
                    }
                    writer.WriteEndObject();

                    writer.WriteStartObject();
                    writer.WriteString("name", "message");
                    writer.WriteBoolean("availability", board.ReplyFieldMessageAvailability);
                    if (board.ReplyFieldMessageAvailability)
                    {
                        writer.WriteBoolean("requirement", board.ReplyFieldMessageRequirement);
                        writer.WriteNumber("lengthMin", board.ReplyFieldMessageLengthMin);
                        writer.WriteNumber("lengthMax", board.ReplyFieldMessageLengthMax);
                    }
                    writer.WriteEndObject();

                    writer.WriteStartObject();
                    writer.WriteString("name", "file");
                    writer.WriteBoolean("availability", board.ReplyFieldFileAvailability);
                    if (board.ReplyFieldMessageAvailability)
                    {
                        writer.WriteBoolean("requirement", board.ReplyFieldFileRequirement);
                        writer.WriteString("typeCollection", string.Join(", ", board.ReplyFieldFileTypeCollection));
                    }
                    writer.WriteEndObject();

                    writer.WriteStartObject();
                    writer.WriteString("name", "password");
                    writer.WriteBoolean("availability", board.ReplyFieldPasswordAvailability);
                    if (board.ReplyFieldPasswordAvailability)
                    {
                        writer.WriteBoolean("requirement", board.ReplyFieldPasswordRequirement);
                        writer.WriteNumber("lengthMin", board.ReplyFieldPasswordLengthMin);
                        writer.WriteNumber("lengthMax", board.ReplyFieldPasswordLengthMax);
                    }
                    writer.WriteEndObject();

                    writer.WriteEndArray();

                    writer.WriteEndObject();

                    writer.WriteEndObject();
                }

                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        public async Task<string> GenerateLocalization()
        {
            var board = _boardManager.GetBoard();

            var newThread = await _localizationService.GetLocalizationAsync(board.Language, TranslationKey.NewThread);
            var reply = await _localizationService.GetLocalizationAsync(board.Language, TranslationKey.Reply);
            var replyIn = await _localizationService.GetLocalizationAsync(board.Language, TranslationKey.ReplyIn);
            var hide = await _localizationService.GetLocalizationAsync(board.Language, TranslationKey.Hide);
            var show = await _localizationService.GetLocalizationAsync(board.Language, TranslationKey.Show);
            var delete = await _localizationService.GetLocalizationAsync(board.Language, TranslationKey.Delete);
            var post = await _localizationService.GetLocalizationAsync(board.Language, TranslationKey.Post);
            var file = await _localizationService.GetLocalizationAsync(board.Language, TranslationKey.File);
            var replyOmitted = await _localizationService.GetLocalizationAsync(board.Language, TranslationKey.ReplyOmitted);
            var replyExpanded = await _localizationService.GetLocalizationAsync(board.Language, TranslationKey.ReplyExpanded);
            var repliesOmitted = await _localizationService.GetLocalizationAsync(board.Language, TranslationKey.RepliesOmitted);
            var repliesExpanded = await _localizationService.GetLocalizationAsync(board.Language, TranslationKey.RepliesExpanded);
            var curb = await _localizationService.GetLocalizationAsync(board.Language, TranslationKey.Curb);
            var uncurb = await _localizationService.GetLocalizationAsync(board.Language, TranslationKey.Uncurb);
            var options = await _localizationService.GetLocalizationAsync(board.Language, TranslationKey.Options);
            var fileDeleted = await _localizationService.GetLocalizationAsync(board.Language, TranslationKey.FileDeleted);
            var report = await _localizationService.GetLocalizationAsync(board.Language, TranslationKey.Report);
            var yes = await _localizationService.GetLocalizationAsync(board.Language, TranslationKey.Yes);
            var no = await _localizationService.GetLocalizationAsync(board.Language, TranslationKey.No);
            var name = await _localizationService.GetLocalizationAsync(board.Language, TranslationKey.Name);
            var subject = await _localizationService.GetLocalizationAsync(board.Language, TranslationKey.Subject);
            var email = await _localizationService.GetLocalizationAsync(board.Language, TranslationKey.Email);
            var password = await _localizationService.GetLocalizationAsync(board.Language, TranslationKey.ErrorPassword);
            var message = await _localizationService.GetLocalizationAsync(board.Language, TranslationKey.Message);
            var captcha = await _localizationService.GetLocalizationAsync(board.Language, TranslationKey.Captcha);
            var hidePreview = await _localizationService.GetLocalizationAsync(board.Language, TranslationKey.HidePreview);
            var unhidePreview = await _localizationService.GetLocalizationAsync(board.Language, TranslationKey.UnhidePreview);
            var longContentOmitted = await _localizationService.GetLocalizationAsync(board.Language, TranslationKey.LongContentOmitted);
            var longContentExpanded = await _localizationService.GetLocalizationAsync(board.Language, TranslationKey.LongContentExpanded);
            var watch = await _localizationService.GetLocalizationAsync(board.Language, TranslationKey.Watch);
            var unwatch = await _localizationService.GetLocalizationAsync(board.Language, TranslationKey.Unwatch);

            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream, _jsonWriterOptions))
                {
                    writer.WriteStartObject();
                    writer.WriteString(nameof(newThread), newThread);
                    writer.WriteString(nameof(reply), reply);
                    writer.WriteString(nameof(replyIn), replyIn);
                    writer.WriteString(nameof(hide), hide);
                    writer.WriteString(nameof(show), show);
                    writer.WriteString(nameof(delete), delete);
                    writer.WriteString(nameof(post), post);
                    writer.WriteString(nameof(file), file);
                    writer.WriteString(nameof(replyOmitted), replyOmitted);
                    writer.WriteString(nameof(replyExpanded), replyExpanded);
                    writer.WriteString(nameof(repliesOmitted), repliesOmitted);
                    writer.WriteString(nameof(repliesExpanded), repliesExpanded);
                    writer.WriteString(nameof(curb), curb);
                    writer.WriteString(nameof(uncurb), uncurb);
                    writer.WriteString(nameof(options), options);
                    writer.WriteString(nameof(fileDeleted), fileDeleted);
                    writer.WriteString(nameof(report), report);
                    writer.WriteString(nameof(yes), yes);
                    writer.WriteString(nameof(no), no);
                    writer.WriteString(nameof(name), name);
                    writer.WriteString(nameof(subject), subject);
                    writer.WriteString(nameof(email), email);
                    writer.WriteString(nameof(password), password);
                    writer.WriteString(nameof(message), message);
                    writer.WriteString(nameof(captcha), captcha);
                    writer.WriteString(nameof(hidePreview), hidePreview);
                    writer.WriteString(nameof(unhidePreview), unhidePreview);
                    writer.WriteString(nameof(longContentOmitted), longContentOmitted);
                    writer.WriteString(nameof(longContentExpanded), longContentExpanded);
                    writer.WriteString(nameof(watch), watch);
                    writer.WriteString(nameof(unwatch), unwatch);
                    writer.WriteEndObject();
                }

                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        public async Task<string> GenerateAccount()
        {
            var account = _accountManager.GetAccount();

            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream, _jsonWriterOptions))
                {
                    writer.WriteStartObject();

                    writer.WriteString("accountId", account.AccountId);
                    writer.WriteNumber("type", (int)account.Type);

                    writer.WriteEndObject();
                }

                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        public async Task<string> GenerateAuthorization()
        {
            var account = _accountManager.GetAccount();
            var board = _boardManager.GetBoard();

            var permissionReportRead = _accountManager.CheckPermission(board.BoardId, e => e.PermissionReportRead);
            var permissionReportDelete = _accountManager.CheckPermission(board.BoardId, e => e.PermissionReportDelete);

            var permissionBanRead = _accountManager.CheckPermission(board.BoardId, e => e.PermissionBanRead);
            var permissionBanCreate = _accountManager.CheckPermission(board.BoardId, e => e.PermissionBanCreate);
            var permissionBanUpdate = _accountManager.CheckPermission(board.BoardId, e => e.PermissionBanUpdate);
            var permissionBanDelete = _accountManager.CheckPermission(board.BoardId, e => e.PermissionBanDelete);

            var permissionBanAppealRead = _accountManager.CheckPermission(board.BoardId, e => e.PermissionBanAppealRead);
            var permissionBanAppealAccept = _accountManager.CheckPermission(board.BoardId, e => e.PermissionBanAppealAccept);
            var permissionBanAppealReject = _accountManager.CheckPermission(board.BoardId, e => e.PermissionBanAppealReject);

            var permissionIpRead = _accountManager.CheckPermission(board.BoardId, e => e.PermissionIpRead);
            var permissionIpDeletePosts = _accountManager.CheckPermission(board.BoardId, e => e.PermissionIpDeletePosts);

            var permissionPostEdit = _accountManager.CheckPermission(board.BoardId, e => e.PermissionPostEdit);
            var permissionPostDelete = _accountManager.CheckPermission(board.BoardId, e => e.PermissionPostDelete);
            var permissionPostDeleteFile = _accountManager.CheckPermission(board.BoardId, e => e.PermissionPostDeleteFile);

            var permissionThreadReplyLock = _accountManager.CheckPermission(board.BoardId, e => e.PermissionThreadReplyLock);
            var permissionThreadBumpLock = _accountManager.CheckPermission(board.BoardId, e => e.PermissionThreadBumpLock);
            var permissionThreadExcessive = _accountManager.CheckPermission(board.BoardId, e => e.PermissionThreadExcessive);
            var permissionThreadPin = _accountManager.CheckPermission(board.BoardId, e => e.PermissionThreadPin);
            var permissionThreadCopy = _accountManager.CheckPermission(board.BoardId, e => e.PermissionThreadCopy);

            var permissionAuthorizationRead = _accountManager.CheckPermission(board.BoardId, e => e.PermissionAuthorizationRead);
            var permissionAuthorizationUpdate = _accountManager.CheckPermission(board.BoardId, e => e.PermissionAuthorizationUpdate);
            var permissionAuthorizationDelete = _accountManager.CheckPermission(board.BoardId, e => e.PermissionAuthorizationDelete);

            var permissionInvitationRead = _accountManager.CheckPermission(board.BoardId, e => e.PermissionInvitationRead);
            var permissionInvitationCreate = _accountManager.CheckPermission(board.BoardId, e => e.PermissionInvitationCreate);
            var permissionInvitationUpdate = _accountManager.CheckPermission(board.BoardId, e => e.PermissionInvitationUpdate);
            var permissionInvitationDelete = _accountManager.CheckPermission(board.BoardId, e => e.PermissionInvitationDelete);

            var permissionBoardView = _accountManager.CheckPermission(board.BoardId, e => e.PermissionBoardView);
            var permissionBoardUpdate = _accountManager.CheckPermission(board.BoardId, e => e.PermissionBoardUpdate);
            var permissionBoardDelete = _accountManager.CheckPermission(board.BoardId, e => e.PermissionBoardDelete);

            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream, _jsonWriterOptions))
                {
                    writer.WriteStartObject();

                    writer.WriteBoolean(nameof(permissionReportRead), permissionReportRead);
                    writer.WriteBoolean(nameof(permissionReportDelete), permissionReportDelete);

                    writer.WriteBoolean(nameof(permissionBanRead), permissionBanRead);
                    writer.WriteBoolean(nameof(permissionBanCreate), permissionBanCreate);
                    writer.WriteBoolean(nameof(permissionBanUpdate), permissionBanUpdate);
                    writer.WriteBoolean(nameof(permissionBanDelete), permissionBanDelete);

                    writer.WriteBoolean(nameof(permissionBanAppealRead), permissionBanAppealRead);
                    writer.WriteBoolean(nameof(permissionBanAppealAccept), permissionBanAppealAccept);
                    writer.WriteBoolean(nameof(permissionBanAppealReject), permissionBanAppealReject);

                    writer.WriteBoolean(nameof(permissionIpRead), permissionIpRead);
                    writer.WriteBoolean(nameof(permissionIpDeletePosts), permissionIpDeletePosts);

                    writer.WriteBoolean(nameof(permissionPostEdit), permissionPostEdit);
                    writer.WriteBoolean(nameof(permissionPostDelete), permissionPostDelete);
                    writer.WriteBoolean(nameof(permissionPostDeleteFile), permissionPostDeleteFile);

                    writer.WriteBoolean(nameof(permissionThreadReplyLock), permissionThreadReplyLock);
                    writer.WriteBoolean(nameof(permissionThreadBumpLock), permissionThreadBumpLock);
                    writer.WriteBoolean(nameof(permissionThreadExcessive), permissionThreadExcessive);
                    writer.WriteBoolean(nameof(permissionThreadPin), permissionThreadPin);
                    writer.WriteBoolean(nameof(permissionThreadCopy), permissionThreadCopy);

                    writer.WriteBoolean(nameof(permissionAuthorizationRead), permissionAuthorizationRead);
                    writer.WriteBoolean(nameof(permissionAuthorizationUpdate), permissionAuthorizationUpdate);
                    writer.WriteBoolean(nameof(permissionAuthorizationDelete), permissionAuthorizationDelete);

                    writer.WriteBoolean(nameof(permissionInvitationRead), permissionInvitationRead);
                    writer.WriteBoolean(nameof(permissionInvitationCreate), permissionInvitationCreate);
                    writer.WriteBoolean(nameof(permissionInvitationUpdate), permissionInvitationUpdate);
                    writer.WriteBoolean(nameof(permissionInvitationDelete), permissionInvitationDelete);

                    writer.WriteBoolean(nameof(permissionBoardView), permissionBoardView);
                    writer.WriteBoolean(nameof(permissionBoardUpdate), permissionBoardUpdate);
                    writer.WriteBoolean(nameof(permissionBoardDelete), permissionBoardDelete);

                    writer.WriteEndObject();
                }

                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
    }
}