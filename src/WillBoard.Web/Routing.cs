using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using WillBoard.Web.Models;

namespace WillBoard.Web
{
    public static class Routing
    {
        private static CompositeRouteConstraint _boardId = new CompositeRouteConstraint(new IRouteConstraint[] { new RegexRouteConstraint("^[a-z0-9]{1,32}$") });
        private static CompositeRouteConstraint _configurationKey = new CompositeRouteConstraint(new IRouteConstraint[] { new RegexRouteConstraint("^[a-z0-9]{1,128}$") });
        private static CompositeRouteConstraint _translationLanguage = new CompositeRouteConstraint(new IRouteConstraint[] { new RegexRouteConstraint("^[a-z0-9]{1,32}$") });
        private static CompositeRouteConstraint _translationKey = new CompositeRouteConstraint(new IRouteConstraint[] { new RegexRouteConstraint("^[a-z0-9]{1,128}$") });
        private static CompositeRouteConstraint _last = new CompositeRouteConstraint(new IRouteConstraint[] { new OptionalRouteConstraint(new IntRouteConstraint()) });
        private static CompositeRouteConstraint _int = new CompositeRouteConstraint(new IRouteConstraint[] { new IntRouteConstraint() });
        private static CompositeRouteConstraint _bool = new CompositeRouteConstraint(new IRouteConstraint[] { new BoolRouteConstraint() });
        private static CompositeRouteConstraint _guid = new CompositeRouteConstraint(new IRouteConstraint[] { new GuidRouteConstraint() });
        private static CompositeRouteConstraint _bigInteger = new CompositeRouteConstraint(new IRouteConstraint[] { new RegexRouteConstraint("^[0-9]{1,40}$") });

        public static IEndpointRouteBuilder Generate(IEndpointRouteBuilder endpointRouteBuilder)
        {
            GenerateApi(endpointRouteBuilder);
            GenerateAdministrationApi(endpointRouteBuilder);
            GenerateAdministration(endpointRouteBuilder);
            GenerateBoard(endpointRouteBuilder);
            GenerateApplication(endpointRouteBuilder);

            return endpointRouteBuilder;
        }

        private static IEndpointRouteBuilder GenerateApi(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute(
                name: "/api/captcha",
                pattern: "/api/captcha",
                defaults: new { controller = "Api", action = "Captcha" }
            ).WithMetadata(new EndpointResponse(EndpointContentType.JSON));

            endpointRouteBuilder.MapControllerRoute(
                name: "/api/captcha/wildcard",
                pattern: "/api/captcha/wildcard",
                defaults: new { controller = "Api", action = "CaptchaWildcard" }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "/api/{boardId}/verification/{thread}",
                pattern: "/api/{boardId}/verification/{thread}",
                defaults: new { controller = "Api", action = "GetVerification" },
                constraints: new { boardId = _boardId, thread = _bool }
            ).WithMetadata(new EndpointResponse(EndpointContentType.JSON));

            endpointRouteBuilder.MapControllerRoute(
                name: "/api/{boardId}/post",
                pattern: "/api/{boardId}/post",
                defaults: new { controller = "Api", action = "CreatePost" },
                constraints: new { boardId = _boardId }
            ).WithMetadata(new EndpointResponse(EndpointContentType.JSON));

            endpointRouteBuilder.MapControllerRoute(
                name: "/api/{boardId}/posts/{postId}",
                pattern: "/api/{boardId}/posts/{postId}",
                defaults: new { controller = "Api", action = "GetPost" },
                constraints: new { boardId = _boardId, postId = _int }
            ).WithMetadata(new EndpointResponse(EndpointContentType.JSON));

            endpointRouteBuilder.MapControllerRoute(
                name: "/api/{boardId}/replies/{threadId}/{last?}",
                pattern: "/api/{boardId}/replies/{threadId}/{last?}",
                defaults: new { controller = "Api", action = "GetReplies" },
                constraints: new { boardId = _boardId, threadId = _int, last = _last }
            ).WithMetadata(new EndpointResponse(EndpointContentType.JSON));

            endpointRouteBuilder.MapControllerRoute(
                name: "/api/{boardId}/synchronization",
                pattern: "/api/{boardId}/synchronization",
                defaults: new { controller = "Api", action = "Synchronization" },
                constraints: new { boardId = _boardId }
            ).WithMetadata(new EndpointResponse(EndpointContentType.SSE));

            endpointRouteBuilder.MapControllerRoute(
                name: "/api/{boardId}/report/{postId}/board",
                pattern: "/api/{boardId}/report/{postId}/board",
                defaults: new { controller = "Api", action = "ReportBoard" },
                constraints: new { boardId = _boardId }
            ).WithMetadata(new EndpointResponse(EndpointContentType.JSON));

            endpointRouteBuilder.MapControllerRoute(
                name: "/api/{boardId}/report/{postId}/system",
                pattern: "/api/{boardId}/report/{postId}/system",
                defaults: new { controller = "Api", action = "ReportSystem" },
                constraints: new { boardId = _boardId }
            ).WithMetadata(new EndpointResponse(EndpointContentType.JSON));

            endpointRouteBuilder.MapControllerRoute(
                name: "/api/{boardId}/delete/{postId}/post",
                pattern: "/api/{boardId}/delete/{postId}/post",
                defaults: new { controller = "Api", action = "DeletePost" },
                constraints: new { boardId = _boardId }
            ).WithMetadata(new EndpointResponse(EndpointContentType.JSON));

            endpointRouteBuilder.MapControllerRoute(
                name: "/api/{boardId}/delete/{postId}/file",
                pattern: "/api/{boardId}/delete/{postId}/file",
                defaults: new { controller = "Api", action = "DeleteFile" },
                constraints: new { boardId = _boardId }
            ).WithMetadata(new EndpointResponse(EndpointContentType.JSON));

            return endpointRouteBuilder;
        }

        private static IEndpointRouteBuilder GenerateAdministrationApi(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute(
                name: "/administration/api/{boardId}/posts/{postId}",
                pattern: "/administration/api/{boardId}/posts/{postId}",
                defaults: new { controller = "AdministrationApi", action = "GetPost" },
                constraints: new { boardId = _boardId, postId = _int }
            ).WithMetadata(new EndpointResponse(EndpointContentType.JSON));

            endpointRouteBuilder.MapControllerRoute(
                name: "/administration/api/{boardId}/replies/{threadId}/{last?}",
                pattern: "/administration/api/{boardId}/replies/{threadId}/{last?}",
                defaults: new { controller = "AdministrationApi", action = "GetReplies" },
                constraints: new { boardId = _boardId, threadId = _int, last = _last }
            ).WithMetadata(new EndpointResponse(EndpointContentType.JSON));

            endpointRouteBuilder.MapControllerRoute(
                name: "/administration/api/{boardId}/synchronization",
                pattern: "/administration/api/{boardId}/synchronization",
                defaults: new { controller = "AdministrationApi", action = "Synchronization" },
                constraints: new { boardId = _boardId }
            ).WithMetadata(new EndpointResponse(EndpointContentType.SSE));

            return endpointRouteBuilder;
        }

        private static IEndpointRouteBuilder GenerateAdministration(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute(
                name: "administration/login",
                pattern: "administration/login",
                defaults: new { controller = "Administration", action = "Login" }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/logout",
                pattern: "administration/logout",
                defaults: new { controller = "Administration", action = "Logout" }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/accounts/{page}",
                pattern: "administration/accounts/{page}",
                defaults: new { controller = "Administration", action = "Accounts", page = 1 },
                constraints: new { page = _int }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/account/{accountId}",
                pattern: "administration/account/{accountId}",
                defaults: new { controller = "Administration", action = "Account" },
                constraints: new { accountId = _guid }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/account/create",
                pattern: "administration/account/create",
                defaults: new { controller = "Administration", action = "AccountCreate" }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/account/{accountId}/update",
                pattern: "administration/account/{accountId}/update",
                defaults: new { controller = "Administration", action = "AccountUpdate" },
                constraints: new { accountId = _guid }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/account/{accountId}/password/change",
                pattern: "administration/account/{accountId}/password/change",
                defaults: new { controller = "Administration", action = "AccountPasswordChange" },
                constraints: new { accountId = _guid }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/account/{accountId}/authentications/{page}",
                pattern: "administration/account/{accountId}/authentications/{page}",
                defaults: new { controller = "Administration", action = "AccountAuthentications", page = 1 },
                constraints: new { accountId = _guid, page = _int }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/account/{accountId}/authentication/{authenticationId}/deactivate",
                pattern: "administration/account/{accountId}/authentication/{authenticationId}/deactivate",
                defaults: new { controller = "Administration", action = "AccountAuthenticationDeactivate" },
                constraints: new { accountId = _guid, authenticationId = _guid }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/account/{accountId}/invitations/{page}",
                pattern: "administration/account/{accountId}/invitations/{page}",
                defaults: new { controller = "Administration", action = "AccountInvitations", page = 1 },
                constraints: new { accountId = _guid, page = _int }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/account/{accountId}/invitation/{invitationId}/accept",
                pattern: "administration/account/{accountId}/invitation/{invitationId}/accept",
                defaults: new { controller = "Administration", action = "AccountInvitationAccept" },
                constraints: new { accountId = _guid, invitationId = _guid }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/account/{accountId}/invitation/{invitationId}/reject",
                pattern: "administration/account/{accountId}/invitation/{invitationId}/reject",
                defaults: new { controller = "Administration", action = "AccountInvitationReject" },
                constraints: new { accountId = _guid, invitationId = _guid }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/boards/{page}",
                pattern: "administration/boards/{page}",
                defaults: new { controller = "Administration", action = "Boards", page = 1 },
                constraints: new { page = _int }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/board/create",
                pattern: "administration/board/create",
                defaults: new { controller = "Administration", action = "BoardCreate" }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/board/{boardId}/update",
                pattern: "administration/board/{boardId}/update",
                defaults: new { controller = "Administration", action = "BoardUpdate" },
                constraints: new { boardId = _boardId }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/board/{boardId}/delete",
                pattern: "administration/board/{boardId}/delete",
                defaults: new { controller = "Administration", action = "BoardDelete" },
                constraints: new { boardId = _boardId }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/board/{boardId}/bans/{page}",
                pattern: "administration/board/{boardId}/bans/{page}",
                defaults: new { controller = "Administration", action = "BoardBans", page = 1 },
                constraints: new { boardId = _boardId, page = _int }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/board/{boardId}/ban/create",
                pattern: "administration/board/{boardId}/ban/create",
                defaults: new { controller = "Administration", action = "BoardBanCreate" },
                constraints: new { boardId = _boardId }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/board/{boardId}/ban/{banId}/update",
                pattern: "administration/board/{boardId}/ban/{banId}/update",
                defaults: new { controller = "Administration", action = "BoardBanUpdate" },
                constraints: new { boardId = _boardId, banId = _guid }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/board/{boardId}/ban/{banId}/delete",
                pattern: "administration/board/{boardId}/ban/{banId}/delete",
                defaults: new { controller = "Administration", action = "BoardBanDelete" },
                constraints: new { boardId = _boardId, banId = _guid }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/board/{boardId}/banappeals/{page}",
                pattern: "administration/board/{boardId}/banappeals/{page}",
                defaults: new { controller = "Administration", action = "BoardBanAppeals", page = 1 },
                constraints: new { boardId = _boardId, page = _int }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/board/{boardId}/banappeal/{banAppealId}/accept",
                pattern: "administration/board/{boardId}/banappeal/{banAppealId}/accept",
                defaults: new { controller = "Administration", action = "BoardBanAppealAccept" },
                constraints: new { boardId = _boardId, banAppealId = _guid }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/board/{boardId}/banappeal/{banAppealId}/reject",
                pattern: "administration/board/{boardId}/banappeal/{banAppealId}/reject",
                defaults: new { controller = "Administration", action = "BoardBanAppealReject" },
                constraints: new { boardId = _boardId, banAppealId = _guid }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/board/{boardId}/reports/{page}",
                pattern: "administration/board/{boardId}/reports/{page}",
                defaults: new { controller = "Administration", action = "BoardReports", page = 1 },
                constraints: new { boardId = _boardId, page = _int }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/board/{boardId}/report/{reportId}/delete",
                pattern: "administration/board/{boardId}/report/{reportId}/delete",
                defaults: new { controller = "Administration", action = "BoardReportDelete" },
                constraints: new { boardId = _boardId, reportId = _guid }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/board/{boardId}/invitations/{page}",
                pattern: "administration/board/{boardId}/invitations/{page}",
                defaults: new { controller = "Administration", action = "BoardInvitations", page = 1 },
                constraints: new { boardId = _boardId, page = _int }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/board/{boardId}/invitation/create",
                pattern: "administration/board/{boardId}/invitation/create",
                defaults: new { controller = "Administration", action = "BoardInvitationCreate" },
                constraints: new { boardId = _boardId }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/board/{boardId}/invitation/{invitationId}/delete",
                pattern: "administration/board/{boardId}/invitation/{invitationId}/delete",
                defaults: new { controller = "Administration", action = "BoardInvitationDelete" },
                constraints: new { boardId = _boardId, invitationId = _guid }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/board/{boardId}/authorizations/{page}",
                pattern: "administration/board/{boardId}/authorizations/{page}",
                defaults: new { controller = "Administration", action = "BoardAuthorizations", page = 1 },
                constraints: new { boardId = _boardId, page = _int }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/board/{boardId}/authorization/{authorizationId}/update",
                pattern: "administration/board/{boardId}/authorization/{authorizationId}/update",
                defaults: new { controller = "Administration", action = "BoardAuthorizationUpdate" },
                constraints: new { boardId = _boardId, authorizationId = _guid }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/board/{boardId}/authorization/{authorizationId}/delete",
                pattern: "administration/board/{boardId}/authorization/{authorizationId}/delete",
                defaults: new { controller = "Administration", action = "BoardAuthorizationDelete" },
                constraints: new { boardId = _boardId, authorizationId = _guid }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/board/{boardId}/post/{postId}/update",
                pattern: "administration/board/{boardId}/post/{postId}/update",
                defaults: new { controller = "Administration", action = "BoardPostUpdate" },
                constraints: new { boardId = _boardId, postId = _int }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/board/{boardId}/post/{postId}/delete",
                pattern: "administration/board/{boardId}/post/{postId}/delete",
                defaults: new { controller = "Administration", action = "BoardPostDelete" },
                constraints: new { boardId = _boardId, postId = _int }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/board/{boardId}/post/{postId}/delete-file",
                pattern: "administration/board/{boardId}/post/{postId}/delete-file",
                defaults: new { controller = "Administration", action = "BoardPostDeleteFile" },
                constraints: new { boardId = _boardId, postId = _int }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/board/{boardId}/thread/{postId}/reply-lock",
                pattern: "administration/board/{boardId}/thread/{postId}/reply-lock",
                defaults: new { controller = "Administration", action = "BoardThreadReplyLock" },
                constraints: new { boardId = _boardId, postId = _int }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/board/{boardId}/thread/{postId}/bump-lock",
                pattern: "administration/board/{boardId}/thread/{postId}/bump-lock",
                defaults: new { controller = "Administration", action = "BoardThreadBumpLock" },
                constraints: new { boardId = _boardId, postId = _int }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/board/{boardId}/thread/{postId}/pin",
                pattern: "administration/board/{boardId}/thread/{postId}/pin",
                defaults: new { controller = "Administration", action = "BoardThreadPin" },
                constraints: new { boardId = _boardId, postId = _int }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/board/{boardId}/thread/{postId}/excessive",
                pattern: "administration/board/{boardId}/thread/{postId}/excessive",
                defaults: new { controller = "Administration", action = "BoardThreadExcessive" },
                constraints: new { boardId = _boardId, postId = _int }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/board/{boardId}/thread/{postId}/copy",
                pattern: "administration/board/{boardId}/thread/{postId}/copy",
                defaults: new { controller = "Administration", action = "BoardThreadCopy" },
                constraints: new { boardId = _boardId, postId = _int }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/board/{boardId}/ip/{ipVersion}/{ipNumber}",
                pattern: "administration/board/{boardId}/ip/{ipVersion}/{ipNumber}",
                defaults: new { controller = "Administration", action = "BoardIp" },
                constraints: new { boardId = _boardId, ipVersion = _int, ipNumber = _bigInteger }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/board/{boardId}/ip/{ipVersion}/{ipNumber}/delete-posts",
                pattern: "administration/board/{boardId}/ip/{ipVersion}/{ipNumber}/delete-posts",
                defaults: new { controller = "Administration", action = "BoardIpDeletePosts" },
                constraints: new { boardId = _boardId, ipVersion = _int, ipNumber = _bigInteger }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/board/{boardId}/view/{page}",
                pattern: "administration/board/{boardId}/view/{page}",
                defaults: new { controller = "Administration", action = "BoardViewClassic", page = 1 },
                constraints: new { boardId = _boardId, page = _int }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/board/{boardId}/view/thread/{threadId}/{last?}",
                pattern: "administration/board/{boardId}/view/thread/{threadId}/{last?}",
                defaults: new { controller = "Administration", action = "BoardViewClassicThread" },
                constraints: new { boardId = _boardId, threadId = _int, last = _last }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/board/{boardId}/view/catalog",
                pattern: "administration/board/{boardId}/view/catalog",
                defaults: new { controller = "Administration", action = "BoardViewCatalog" },
                constraints: new { boardId = _boardId }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/board/{boardId}/view/search",
                pattern: "administration/board/{boardId}/view/search",
                defaults: new { controller = "Administration", action = "BoardViewSearch" },
                constraints: new { boardId = _boardId }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/bans/{page}",
                pattern: "administration/bans/{page}",
                defaults: new { controller = "Administration", action = "Bans", page = 1 },
                constraints: new { page = _int }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/ban/create",
                pattern: "administration/ban/create",
                defaults: new { controller = "Administration", action = "BanCreate" }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/ban/{banId}/update",
                pattern: "administration/ban/{banId}/update",
                defaults: new { controller = "Administration", action = "BanUpdate" },
                constraints: new { banId = _guid }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/ban/{banId}/delete",
                pattern: "administration/ban/{banId}/delete",
                defaults: new { controller = "Administration", action = "BanDelete" },
                constraints: new { banId = _guid }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/banappeals/{page}",
                pattern: "administration/anappeals/{page}",
                defaults: new { controller = "Administration", action = "BanAppeals", page = 1 },
                constraints: new { page = _int }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/banappeal/{banAppealId}/accept",
                pattern: "administration/banappeal/{banAppealId}/accept",
                defaults: new { controller = "Administration", action = "BanAppealAccept" },
                constraints: new { banAppealId = _guid }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/banappeal/{banAppealId}/reject",
                pattern: "administration/banappeal/{banAppealId}/reject",
                defaults: new { controller = "Administration", action = "BanAppealReject" },
                constraints: new { banAppealId = _guid }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/reports/{page}",
                pattern: "administration/reports/{page}",
                defaults: new { controller = "Administration", action = "Reports", page = 1 },
                constraints: new { page = _int }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/report/{reportId}/delete",
                pattern: "administration/report/{reportId}/delete",
                defaults: new { controller = "Administration", action = "ReportDelete" },
                constraints: new { reportId = _guid }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/navigations",
                pattern: "administration/navigations",
                defaults: new { controller = "Administration", action = "Navigations" }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/navigation/create",
                pattern: "administration/navigation/create",
                defaults: new { controller = "Administration", action = "NavigationCreate" }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/navigation/{navigationId}/update",
                pattern: "administration/navigation/{navigationId}/update",
                defaults: new { controller = "Administration", action = "NavigationUpdate" },
                constraints: new { navigationId = _guid }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/navigation/{navigationId}/delete",
                pattern: "administration/navigation/{navigationId}/delete",
                defaults: new { controller = "Administration", action = "NavigationDelete" },
                constraints: new { navigationId = _guid }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/configurations",
                pattern: "administration/configurations",
                defaults: new { controller = "Administration", action = "Configurations" }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/configuration/create",
                pattern: "administration/configuration/create",
                defaults: new { controller = "Administration", action = "ConfigurationCreate" }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/configuration/{key}/update",
                pattern: "administration/configuration/{key}/update",
                defaults: new { controller = "Administration", action = "ConfigurationUpdate" },
                constraints: new { key = _configurationKey }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/configuration/{key}/delete",
                pattern: "administration/configuration/{key}/delete",
                defaults: new { controller = "Administration", action = "ConfigurationDelete" },
                constraints: new { key = _configurationKey }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/translations",
                pattern: "administration/translations",
                defaults: new { controller = "Administration", action = "Translations" }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/translation/create",
                pattern: "administration/translation/create",
                defaults: new { controller = "Administration", action = "TranslationCreate" }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/translation/{language}/{key}/update",
                pattern: "administration/translation/{language}/{key}/update",
                defaults: new { controller = "Administration", action = "TranslationUpdate" },
                constraints: new { language = _translationLanguage, key = _translationKey }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/translation/{language}/{key}/delete",
                pattern: "administration/translation/{language}/{key}/delete",
                defaults: new { controller = "Administration", action = "TranslationDelete" },
                constraints: new { language = _translationLanguage, key = _translationKey }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/cache",
                pattern: "administration/cache",
                defaults: new { controller = "Administration", action = "Cache" }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "administration/cache/clear",
                pattern: "administration/cache/clear",
                defaults: new { controller = "Administration", action = "CacheClear" }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            return endpointRouteBuilder;
        }

        private static IEndpointRouteBuilder GenerateBoard(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute(
                  name: "{boardId}/report/{postId}",
                  pattern: "{boardId}/report/{postId}",
                  defaults: new { controller = "Board", action = "Report" },
                  constraints: new { boardId = _boardId, postId = _int }
              ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "{boardId}/report/{postId}/board",
                pattern: "{boardId}/report/{postId}/board",
                defaults: new { controller = "Board", action = "ReportBoard" },
                constraints: new { boardId = _boardId, postId = _int }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "{boardId}/report/{postId}/system",
                pattern: "{boardId}/report/{postId}/system",
                defaults: new { controller = "Board", action = "ReportSystem" },
                constraints: new { boardId = _boardId, postId = _int }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "{boardId}/delete/{postId}",
                pattern: "{boardId}/delete/{postId}",
                defaults: new { controller = "Board", action = "Delete" },
                constraints: new { boardId = _boardId, postId = _int }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "{boardId}/delete/{postId}/post",
                pattern: "{boardId}/delete/{postId}/post",
                defaults: new { controller = "Board", action = "DeletePost" },
                constraints: new { boardId = _boardId, postId = _int }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "{boardId}/delete/{postId}/file",
                pattern: "{boardId}/delete/{postId}/file",
                defaults: new { controller = "Board", action = "DeleteFile" },
                constraints: new { boardId = _boardId, postId = _int }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "{boardId}/login",
                pattern: "{boardId}/login",
                defaults: new { controller = "Board", action = "SecurePassword" },
                constraints: new { boardId = _boardId }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "{boardId}/ban",
                pattern: "{boardId}/ban",
                defaults: new { controller = "Board", action = "Ban" },
                constraints: new { boardId = _boardId }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "{boardId}/ban/{banId}/appeal/board",
                pattern: "{boardId}/ban/{banId}/appeal/board",
                defaults: new { controller = "Board", action = "BanAppealBoard" },
                constraints: new { boardId = _boardId }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "{boardId}/ban/{banId}/appeal/system",
                pattern: "{boardId}/ban/{banId}/appeal/system",
                defaults: new { controller = "Board", action = "BanAppealSystem" },
                constraints: new { boardId = _boardId }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "{boardId}/search",
                pattern: "{boardId}/search",
                defaults: new { controller = "Board", action = "Search" },
                constraints: new { boardId = _boardId }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "{boardId}/catalog",
                pattern: "{boardId}/catalog",
                defaults: new { controller = "Board", action = "Catalog" },
                constraints: new { boardId = _boardId }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "{boardId}/search",
                pattern: "{boardId}/search",
                defaults: new { controller = "Board", action = "Search" },
                constraints: new { boardId = _boardId }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "{boardId}/post",
                pattern: "{boardId}/post",
                defaults: new { controller = "Board", action = "CreatePost" },
                constraints: new { boardId = _boardId }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "{boardId}/thread/{threadId}/{last?}",
                pattern: "{boardId}/thread/{threadId}/{last?}",
                defaults: new { controller = "Board", action = "ClassicThread" },
                constraints: new { boardId = _boardId, threadId = _int, last = _last }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            endpointRouteBuilder.MapControllerRoute(
                name: "{boardId}/{page}",
                pattern: "{boardId}/{page}",
                defaults: new { controller = "Board", action = "Classic", page = 1 },
                constraints: new { boardId = _boardId, page = _int }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            return endpointRouteBuilder;
        }

        private static IEndpointRouteBuilder GenerateApplication(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute(
                name: "",
                pattern: "",
                defaults: new { controller = "Application", action = "Index" }
            ).WithMetadata(new EndpointResponse(EndpointContentType.HTML));

            return endpointRouteBuilder;
        }
    }
}