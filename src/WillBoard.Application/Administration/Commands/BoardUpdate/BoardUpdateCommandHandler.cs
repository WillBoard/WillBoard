using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WillBoard.Core.Consts;
using WillBoard.Core.Errors;
using WillBoard.Core.Interfaces.Caches;
using WillBoard.Core.Interfaces.Repositories;
using WillBoard.Core.Managers;
using WillBoard.Core.Results;
using WillBoard.Core.Utilities;

namespace WillBoard.Application.Administration.Commands.BoardUpdate
{
    public class BoardUpdateCommandHandler : IRequestHandler<BoardUpdateCommand, Status<InternalError>>
    {
        private readonly AccountManager _accountManager;
        private readonly IBoardRepository _boardRepository;
        private readonly IBoardCache _boardCache;
        private readonly IPostCache _postCache;
        private readonly IBlockListCache _blockListCache;

        public BoardUpdateCommandHandler(AccountManager accountManager, IBoardRepository boardRepository, IBoardCache boardCache, IPostCache postCache, IBlockListCache blockListCache)
        {
            _accountManager = accountManager;
            _boardRepository = boardRepository;
            _boardCache = boardCache;
            _postCache = postCache;
            _blockListCache = blockListCache;
        }

        public async Task<Status<InternalError>> Handle(BoardUpdateCommand request, CancellationToken cancellationToken)
        {
            var board = await _boardCache.GetAsync(request.BoardId);
            if (board == null)
            {
                return Status<InternalError>.ErrorStatus(new InternalError(404, TranslationKey.ErrorNotFound));
            }

            if (!_accountManager.CheckPermission(request.BoardId, e => e.PermissionBoardUpdate))
            {
                return Status<InternalError>.ErrorStatus(new InternalError(403, TranslationKey.ErrorForbidden));
            }

            var boardUpdate = new Core.Entities.Board(request.BoardId)
            {
                Name = request.Name,

                Availability = request.Availability,

                Type = request.Type,
                TypeGroupCollection = ArrayConversion.DeserializeString(request.TypeGroupCollection),

                Accessibility = request.Accessibility,
                AccessibilityPassword = request.AccessibilityPassword,
                AccessibilityIpVersion4NumberCollection = ArrayConversion.DeserializeUInt32(request.AccessibilityIpVersion4NumberCollection),
                AccessibilityIpVersion6NumberCollection = ArrayConversion.DeserializeBigInteger(request.AccessibilityIpVersion6NumberCollection),

                Visibility = request.Visibility,

                Anonymous = request.Anonymous,
                Language = request.Language,
                TimeZone = request.TimeZone,

                OnlineCounterAvailability = request.OnlineCounterAvailability,

                CssThemeCollection = ArrayConversion.DeserializeCssTheme(request.CssThemeCollection),

                CssInline = request.CssInline,
                CssExternalCollection = ArrayConversion.DeserializeString(request.CssExternalCollection),

                JsInline = request.JsInline,
                JsExternalCollection = ArrayConversion.DeserializeString(request.JsExternalCollection),

                InformationAside = request.InformationAside,
                InformationHeader = request.InformationHeader,
                InformationFooter = request.InformationFooter,

                MarkupStaticCustomCollection = ArrayConversion.DeserializeMarkupCustom(request.MarkupStaticCustomCollection),
                MarkupDynamicCustomCollection = ArrayConversion.DeserializeMarkupCustom(request.MarkupDynamicCustomCollection),

                UserIdRequirement = request.UserIdRequirement,
                CountryRequirement = request.CountryRequirement,

                PageMax = request.PageMax,
                PageThreadMax = request.PageThreadMax,

                PostAvailability = request.PostAvailability,

                ThreadReplyPreviewMax = request.ThreadReplyPreviewMax,
                ThreadPinReplyPreviewMax = request.ThreadPinReplyPreviewMax,

                ThreadBumpLockReplyMax = request.ThreadBumpLockReplyMax,
                ThreadBumpLockTimeMax = request.ThreadBumpLockTimeMax,

                ThreadExcessiveTimeMax = request.ThreadExcessiveTimeMax,

                ReplyFieldNameAvailability = request.ReplyFieldNameAvailability,
                ReplyFieldNameRequirement = request.ReplyFieldNameRequirement,
                ReplyFieldNameLengthMin = request.ReplyFieldNameLengthMin,
                ReplyFieldNameLengthMax = request.ReplyFieldNameLengthMax,
                ThreadFieldNameAvailability = request.ThreadFieldNameAvailability,
                ThreadFieldNameRequirement = request.ThreadFieldNameRequirement,
                ThreadFieldNameLengthMin = request.ThreadFieldNameLengthMin,
                ThreadFieldNameLengthMax = request.ThreadFieldNameLengthMax,

                ReplyFieldEmailAvailability = request.ReplyFieldEmailAvailability,
                ReplyFieldEmailRequirement = request.ReplyFieldEmailRequirement,
                ReplyFieldEmailLengthMin = request.ReplyFieldEmailLengthMin,
                ReplyFieldEmailLengthMax = request.ReplyFieldEmailLengthMax,
                ThreadFieldEmailAvailability = request.ThreadFieldEmailAvailability,
                ThreadFieldEmailRequirement = request.ThreadFieldEmailRequirement,
                ThreadFieldEmailLengthMin = request.ThreadFieldEmailLengthMin,
                ThreadFieldEmailLengthMax = request.ThreadFieldEmailLengthMax,

                ReplyFieldSubjectAvailability = request.ReplyFieldSubjectAvailability,
                ReplyFieldSubjectRequirement = request.ReplyFieldSubjectRequirement,
                ReplyFieldSubjectLengthMin = request.ReplyFieldSubjectLengthMin,
                ReplyFieldSubjectLengthMax = request.ReplyFieldSubjectLengthMax,
                ThreadFieldSubjectAvailability = request.ThreadFieldSubjectAvailability,
                ThreadFieldSubjectRequirement = request.ThreadFieldSubjectRequirement,
                ThreadFieldSubjectLengthMin = request.ThreadFieldSubjectLengthMin,
                ThreadFieldSubjectLengthMax = request.ThreadFieldSubjectLengthMax,

                ReplyFieldMessageAvailability = request.ReplyFieldMessageAvailability,
                ReplyFieldMessageRequirement = request.ReplyFieldMessageRequirement,
                ReplyFieldMessageLengthMin = request.ReplyFieldMessageLengthMin,
                ReplyFieldMessageLengthMax = request.ReplyFieldMessageLengthMax,
                ReplyFieldMessageLineMax = request.ReplyFieldMessageLineMax,
                ReplyFieldMessageLinkMax = request.ReplyFieldMessageLinkMax,
                ReplyFieldMessageMentionMax = request.ReplyFieldMessageMentionMax,
                ThreadFieldMessageAvailability = request.ThreadFieldMessageAvailability,
                ThreadFieldMessageRequirement = request.ThreadFieldMessageRequirement,
                ThreadFieldMessageLengthMin = request.ThreadFieldMessageLengthMin,
                ThreadFieldMessageLengthMax = request.ThreadFieldMessageLengthMax,
                ThreadFieldMessageLineMax = request.ThreadFieldMessageLineMax,
                ThreadFieldMessageLinkMax = request.ThreadFieldMessageLinkMax,
                ThreadFieldMessageMentionMax = request.ThreadFieldMessageMentionMax,

                ReplyFieldFileAvailability = request.ReplyFieldFileAvailability,
                ReplyFieldFileRequirement = request.ReplyFieldFileRequirement,
                ReplyFieldFileSizeMin = request.ReplyFieldFileSizeMin,
                ReplyFieldFileSizeMax = request.ReplyFieldFileSizeMax,
                ReplyFieldFileOriginality = request.ReplyFieldFileOriginality,
                ReplyFieldFileTypeCollection = ArrayConversion.DeserializeString(request.ReplyFieldFileTypeCollection),
                ReplyFieldFileImageWidthMin = request.ReplyFieldFileImageWidthMin,
                ReplyFieldFileImageWidthMax = request.ReplyFieldFileImageWidthMax,
                ReplyFieldFileImageHeightMin = request.ReplyFieldFileImageHeightMin,
                ReplyFieldFileImageHeightMax = request.ReplyFieldFileImageHeightMax,
                ReplyFieldFileVideoWidthMin = request.ReplyFieldFileVideoWidthMin,
                ReplyFieldFileVideoWidthMax = request.ReplyFieldFileVideoWidthMax,
                ReplyFieldFileVideoHeightMax = request.ReplyFieldFileVideoHeightMax,
                ReplyFieldFileVideoHeightMin = request.ReplyFieldFileVideoHeightMin,
                ReplyFieldFileVideoDurationMin = request.ReplyFieldFileVideoDurationMin,
                ReplyFieldFileVideoDurationMax = request.ReplyFieldFileVideoDurationMax,
                ReplyFieldFileAudioDurationMin = request.ReplyFieldFileAudioDurationMin,
                ReplyFieldFileAudioDurationMax = request.ReplyFieldFileAudioDurationMax,
                ReplyFilePreviewWidthMax = request.ReplyFilePreviewWidthMax,
                ReplyFilePreviewHeightMax = request.ReplyFilePreviewHeightMax,
                ThreadFieldFileAvailability = request.ThreadFieldFileAvailability,
                ThreadFieldFileRequirement = request.ThreadFieldFileRequirement,
                ThreadFieldFileSizeMin = request.ThreadFieldFileSizeMin,
                ThreadFieldFileSizeMax = request.ThreadFieldFileSizeMax,
                ThreadFieldFileOriginality = request.ThreadFieldFileOriginality,
                ThreadFieldFileTypeCollection = ArrayConversion.DeserializeString(request.ThreadFieldFileTypeCollection),
                ThreadFieldFileImageWidthMin = request.ThreadFieldFileImageWidthMin,
                ThreadFieldFileImageWidthMax = request.ThreadFieldFileImageWidthMax,
                ThreadFieldFileImageHeightMin = request.ThreadFieldFileImageHeightMin,
                ThreadFieldFileImageHeightMax = request.ThreadFieldFileImageHeightMax,
                ThreadFieldFileVideoWidthMin = request.ThreadFieldFileVideoWidthMin,
                ThreadFieldFileVideoWidthMax = request.ThreadFieldFileVideoWidthMax,
                ThreadFieldFileVideoHeightMax = request.ThreadFieldFileVideoHeightMax,
                ThreadFieldFileVideoHeightMin = request.ThreadFieldFileVideoHeightMin,
                ThreadFieldFileVideoDurationMin = request.ThreadFieldFileVideoDurationMin,
                ThreadFieldFileVideoDurationMax = request.ThreadFieldFileVideoDurationMax,
                ThreadFieldFileAudioDurationMin = request.ThreadFieldFileAudioDurationMin,
                ThreadFieldFileAudioDurationMax = request.ThreadFieldFileAudioDurationMax,
                ThreadFilePreviewWidthMax = request.ThreadFilePreviewWidthMax,
                ThreadFilePreviewHeightMax = request.ThreadFilePreviewHeightMax,

                ReplyFieldOptionSpoilerAvailability = request.ReplyFieldOptionSpoilerAvailability,
                ReplyFieldOptionSageAvailability = request.ReplyFieldOptionSageAvailability,

                ThreadFieldOptionSpoilerAvailability = request.ThreadFieldOptionSpoilerAvailability,
                ThreadFieldOptionSageAvailability = request.ThreadFieldOptionSageAvailability,
                ThreadFieldOptionUserIdAvailability = request.ThreadFieldOptionUserIdAvailability,
                ThreadFieldOptionCountryAvailability = request.ThreadFieldOptionCountryAvailability,

                ReplyFieldPasswordAvailability = request.ReplyFieldPasswordAvailability,
                ReplyFieldPasswordRequirement = request.ReplyFieldPasswordRequirement,
                ReplyFieldPasswordLengthMin = request.ReplyFieldPasswordLengthMin,
                ReplyFieldPasswordLengthMax = request.ReplyFieldPasswordLengthMax,
                ThreadFieldPasswordAvailability = request.ThreadFieldPasswordAvailability,
                ThreadFieldPasswordRequirement = request.ThreadFieldPasswordRequirement,
                ThreadFieldPasswordLengthMin = request.ThreadFieldPasswordLengthMin,
                ThreadFieldPasswordLengthMax = request.ThreadFieldPasswordLengthMax,

                ReplyTimeMin = request.ReplyTimeMin,
                ThreadTimeMin = request.ThreadTimeMin,

                ReplyDeleteAvailability = request.ReplyDeleteAvailability,
                ReplyDeleteTimeMin = request.ReplyDeleteTimeMin,
                ReplyDeleteTimeMax = request.ReplyDeleteTimeMax,
                ThreadDeleteAvailability = request.ThreadDeleteAvailability,
                ThreadDeleteTimeMin = request.ThreadDeleteTimeMin,
                ThreadDeleteTimeMax = request.ThreadDeleteTimeMax,

                ReplyFileDeleteAvailability = request.ReplyFileDeleteAvailability,
                ReplyFileDeleteTimeMin = request.ReplyFileDeleteTimeMin,
                ReplyFileDeleteTimeMax = request.ReplyFileDeleteTimeMax,
                ThreadFileDeleteAvailability = request.ThreadFileDeleteAvailability,
                ThreadFileDeleteTimeMin = request.ThreadFileDeleteTimeMin,
                ThreadFileDeleteTimeMax = request.ThreadFileDeleteTimeMax,

                ReportBoardAvailability = request.ReportBoardAvailability,
                ReportBoardLengthMin = request.ReportBoardLengthMin,
                ReportBoardLengthMax = request.ReportBoardLengthMax,
                ReportBoardIpMax = request.ReportBoardIpMax,
                ReportBoardTimeMin = request.ReportBoardTimeMin,

                CountryIpVersion4BlockAvailability = request.CountryIpVersion4BlockAvailability,
                CountryIpVersion4BlockCollection = ArrayConversion.DeserializeString(request.CountryIpVersion4BlockCollection),
                CountryIpVersion4ExclusionIpNumberCollection = ArrayConversion.DeserializeUInt32(request.CountryIpVersion4ExclusionIpNumberCollection),

                CountryIpVersion6BlockAvailability = request.CountryIpVersion6BlockAvailability,
                CountryIpVersion6BlockCollection = ArrayConversion.DeserializeString(request.CountryIpVersion6BlockCollection),
                CountryIpVersion6ExclusionIpNumberCollection = ArrayConversion.DeserializeBigInteger(request.CountryIpVersion6ExclusionIpNumberCollection),

                BadIpVersion4BlockAvailability = request.BadIpVersion4BlockAvailability,
                BadIpVersion4BlockExclusionIpNumberCollection = ArrayConversion.DeserializeUInt32(request.BadIpVersion4BlockExclusionIpNumberCollection),

                BadIpVersion6BlockAvailability = request.BadIpVersion6BlockAvailability,
                BadIpVersion6BlockExclusionIpNumberCollection = ArrayConversion.DeserializeBigInteger(request.BadIpVersion6BlockExclusionIpNumberCollection),

                DnsBlockListIpVersion4Availability = request.DnsBlockListIpVersion4Availability,
                DnsBlockListIpVersion4Cache = request.DnsBlockListIpVersion4Cache,
                DnsBlockListIpVersion4Collection = ArrayConversion.DeserializeBlockList(request.DnsBlockListIpVersion4Collection),
                DnsBlockListIpVersion4ExclusionIpNumberCollection = ArrayConversion.DeserializeUInt32(request.DnsBlockListIpVersion4ExclusionIpNumberCollection),

                DnsBlockListIpVersion6Availability = request.DnsBlockListIpVersion6Availability,
                DnsBlockListIpVersion6Cache = request.DnsBlockListIpVersion6Cache,
                DnsBlockListIpVersion6Collection = ArrayConversion.DeserializeBlockList(request.DnsBlockListIpVersion6Collection),
                DnsBlockListIpVersion6ExclusionIpNumberCollection = ArrayConversion.DeserializeBigInteger(request.DnsBlockListIpVersion6ExclusionIpNumberCollection),

                ApiBlockListIpVersion4Availability = request.ApiBlockListIpVersion4Availability,
                ApiBlockListIpVersion4Cache = request.ApiBlockListIpVersion4Cache,
                ApiBlockListIpVersion4Collection = ArrayConversion.DeserializeBlockList(request.ApiBlockListIpVersion4Collection),
                ApiBlockListIpVersion4ExclusionIpNumberCollection = ArrayConversion.DeserializeUInt32(request.ApiBlockListIpVersion4ExclusionIpNumberCollection),

                ApiBlockListIpVersion6Availability = request.ApiBlockListIpVersion6Availability,
                ApiBlockListIpVersion6Cache = request.ApiBlockListIpVersion6Cache,
                ApiBlockListIpVersion6Collection = ArrayConversion.DeserializeBlockList(request.ApiBlockListIpVersion6Collection),
                ApiBlockListIpVersion6ExclusionIpNumberCollection = ArrayConversion.DeserializeBigInteger(request.ApiBlockListIpVersion6ExclusionIpNumberCollection),

                FieldVerificationType = request.FieldVerificationType,

                VerificationPublicKey = request.VerificationPublicKey,
                VerificationSecretKey = request.VerificationSecretKey,

                ReplyFieldVerificationMode = request.ReplyFieldVerificationMode,
                ReplyFieldVerificationLocalTime = request.ReplyFieldVerificationLocalTime,
                ThreadFieldVerificationMode = request.ThreadFieldVerificationMode,
                ThreadFieldVerificationLocalTime = request.ThreadFieldVerificationLocalTime,

                SynchronizationThreadAvailability = request.SynchronizationThreadAvailability,
                SynchronizationBoardAvailability = request.SynchronizationBoardAvailability,

                SearchAvailability = request.SearchAvailability,
                CatalogAvailability = request.CatalogAvailability,

                AnonymizationAvailability = request.AnonymizationAvailability,
                AnonymizationTimeMax = request.AnonymizationTimeMax,
            };

            await _boardRepository.UpdateAsync(boardUpdate);

            await _boardCache.RemoveCollectionAsync();
            await _postCache.PurgeAdaptedCollectionAsync(board.BoardId);
            await _blockListCache.PurgeBoardDnsBlockListIpVersion4Async(board.BoardId);
            await _blockListCache.PurgeBoardDnsBlockListIpVersion6Async(board.BoardId);
            await _blockListCache.PurgeBoardApiBlockListIpVersion4Async(board.BoardId);
            await _blockListCache.PurgeBoardApiBlockListIpVersion6Async(board.BoardId);

            return Status<InternalError>.SuccessStatus();
        }
    }
}