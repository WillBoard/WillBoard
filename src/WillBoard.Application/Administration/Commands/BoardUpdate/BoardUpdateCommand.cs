using MediatR;
using WillBoard.Core.Enums;
using WillBoard.Core.Errors;
using WillBoard.Core.Results;

namespace WillBoard.Application.Administration.Commands.BoardUpdate
{
    public class BoardUpdateCommand : IRequest<Status<InternalError>>
    {
        public string BoardId { get; set; }
        public string Name { get; set; }

        public bool Availability { get; set; }

        public BoardType Type { get; set; }
        public string TypeGroupCollection { get; set; }

        public BoardAccessibility Accessibility { get; set; }
        public string AccessibilityPassword { get; set; }
        public string AccessibilityIpVersion4NumberCollection { get; set; }
        public string AccessibilityIpVersion6NumberCollection { get; set; }

        public BoardVisibility Visibility { get; set; }

        public string Anonymous { get; set; }
        public string Language { get; set; }
        public string TimeZone { get; set; }

        public bool OnlineCounterAvailability { get; set; }

        public string CssThemeCollection { get; set; }

        public string CssInline { get; set; }
        public string CssExternalCollection { get; set; }

        public string JsInline { get; set; }
        public string JsExternalCollection { get; set; }

        public string InformationAside { get; set; }
        public string InformationHeader { get; set; }
        public string InformationFooter { get; set; }

        public string MarkupStaticCustomCollection { get; set; }
        public string MarkupDynamicCustomCollection { get; set; }

        public bool UserIdRequirement { get; set; }
        public bool CountryRequirement { get; set; }

        public int PageMax { get; set; }
        public int PageThreadMax { get; set; }

        public bool PostAvailability { get; set; }

        public int ThreadReplyPreviewMax { get; set; }
        public int ThreadPinReplyPreviewMax { get; set; }

        public int ThreadBumpLockReplyMax { get; set; }
        public int ThreadBumpLockTimeMax { get; set; }

        public int ThreadExcessiveTimeMax { get; set; }

        public bool ReplyFieldNameAvailability { get; set; }
        public bool ReplyFieldNameRequirement { get; set; }
        public int ReplyFieldNameLengthMin { get; set; }
        public int ReplyFieldNameLengthMax { get; set; }
        public bool ThreadFieldNameAvailability { get; set; }
        public bool ThreadFieldNameRequirement { get; set; }
        public int ThreadFieldNameLengthMin { get; set; }
        public int ThreadFieldNameLengthMax { get; set; }

        public bool ReplyFieldEmailAvailability { get; set; }
        public bool ReplyFieldEmailRequirement { get; set; }
        public int ReplyFieldEmailLengthMin { get; set; }
        public int ReplyFieldEmailLengthMax { get; set; }
        public bool ThreadFieldEmailAvailability { get; set; }
        public bool ThreadFieldEmailRequirement { get; set; }
        public int ThreadFieldEmailLengthMin { get; set; }
        public int ThreadFieldEmailLengthMax { get; set; }

        public bool ReplyFieldSubjectAvailability { get; set; }
        public bool ReplyFieldSubjectRequirement { get; set; }
        public int ReplyFieldSubjectLengthMin { get; set; }
        public int ReplyFieldSubjectLengthMax { get; set; }
        public bool ThreadFieldSubjectAvailability { get; set; }
        public bool ThreadFieldSubjectRequirement { get; set; }
        public int ThreadFieldSubjectLengthMin { get; set; }
        public int ThreadFieldSubjectLengthMax { get; set; }

        public bool ReplyFieldMessageAvailability { get; set; }
        public bool ReplyFieldMessageRequirement { get; set; }
        public int ReplyFieldMessageLengthMin { get; set; }
        public int ReplyFieldMessageLengthMax { get; set; }
        public int ReplyFieldMessageLineMax { get; set; }
        public int ReplyFieldMessageLinkMax { get; set; }
        public int ReplyFieldMessageMentionMax { get; set; }
        public bool ThreadFieldMessageAvailability { get; set; }
        public bool ThreadFieldMessageRequirement { get; set; }
        public int ThreadFieldMessageLengthMin { get; set; }
        public int ThreadFieldMessageLengthMax { get; set; }
        public int ThreadFieldMessageLineMax { get; set; }
        public int ThreadFieldMessageLinkMax { get; set; }
        public int ThreadFieldMessageMentionMax { get; set; }

        public bool ReplyFieldFileAvailability { get; set; }
        public bool ReplyFieldFileRequirement { get; set; }
        public long ReplyFieldFileSizeMin { get; set; }
        public long ReplyFieldFileSizeMax { get; set; }
        public bool ReplyFieldFileOriginality { get; set; }
        public string ReplyFieldFileTypeCollection { get; set; }
        public int ReplyFieldFileImageWidthMin { get; set; }
        public int ReplyFieldFileImageWidthMax { get; set; }
        public int ReplyFieldFileImageHeightMin { get; set; }
        public int ReplyFieldFileImageHeightMax { get; set; }
        public int ReplyFieldFileVideoWidthMin { get; set; }
        public int ReplyFieldFileVideoWidthMax { get; set; }
        public int ReplyFieldFileVideoHeightMax { get; set; }
        public int ReplyFieldFileVideoHeightMin { get; set; }
        public int ReplyFieldFileVideoDurationMin { get; set; }
        public int ReplyFieldFileVideoDurationMax { get; set; }
        public int ReplyFieldFileAudioDurationMin { get; set; }
        public int ReplyFieldFileAudioDurationMax { get; set; }
        public int ReplyFilePreviewWidthMax { get; set; }
        public int ReplyFilePreviewHeightMax { get; set; }
        public bool ThreadFieldFileAvailability { get; set; }
        public bool ThreadFieldFileRequirement { get; set; }
        public long ThreadFieldFileSizeMin { get; set; }
        public long ThreadFieldFileSizeMax { get; set; }
        public bool ThreadFieldFileOriginality { get; set; }
        public string ThreadFieldFileTypeCollection { get; set; }
        public int ThreadFieldFileImageWidthMin { get; set; }
        public int ThreadFieldFileImageWidthMax { get; set; }
        public int ThreadFieldFileImageHeightMin { get; set; }
        public int ThreadFieldFileImageHeightMax { get; set; }
        public int ThreadFieldFileVideoWidthMin { get; set; }
        public int ThreadFieldFileVideoWidthMax { get; set; }
        public int ThreadFieldFileVideoHeightMax { get; set; }
        public int ThreadFieldFileVideoHeightMin { get; set; }
        public int ThreadFieldFileVideoDurationMin { get; set; }
        public int ThreadFieldFileVideoDurationMax { get; set; }
        public int ThreadFieldFileAudioDurationMin { get; set; }
        public int ThreadFieldFileAudioDurationMax { get; set; }
        public int ThreadFilePreviewWidthMax { get; set; }
        public int ThreadFilePreviewHeightMax { get; set; }

        public bool ReplyFieldOptionSpoilerAvailability { get; set; }
        public bool ReplyFieldOptionSageAvailability { get; set; }

        public bool ThreadFieldOptionSpoilerAvailability { get; set; }
        public bool ThreadFieldOptionSageAvailability { get; set; }
        public bool ThreadFieldOptionUserIdAvailability { get; set; }
        public bool ThreadFieldOptionCountryAvailability { get; set; }

        public bool ReplyFieldPasswordAvailability { get; set; }
        public bool ReplyFieldPasswordRequirement { get; set; }
        public int ReplyFieldPasswordLengthMin { get; set; }
        public int ReplyFieldPasswordLengthMax { get; set; }
        public bool ThreadFieldPasswordAvailability { get; set; }
        public bool ThreadFieldPasswordRequirement { get; set; }
        public int ThreadFieldPasswordLengthMin { get; set; }
        public int ThreadFieldPasswordLengthMax { get; set; }

        public int ReplyTimeMin { get; set; }
        public int ThreadTimeMin { get; set; }

        public bool ReplyDeleteAvailability { get; set; }
        public int ReplyDeleteTimeMin { get; set; }
        public int ReplyDeleteTimeMax { get; set; }
        public bool ThreadDeleteAvailability { get; set; }
        public int ThreadDeleteTimeMin { get; set; }
        public int ThreadDeleteTimeMax { get; set; }

        public bool ReplyFileDeleteAvailability { get; set; }
        public int ReplyFileDeleteTimeMin { get; set; }
        public int ReplyFileDeleteTimeMax { get; set; }
        public bool ThreadFileDeleteAvailability { get; set; }
        public int ThreadFileDeleteTimeMin { get; set; }
        public int ThreadFileDeleteTimeMax { get; set; }

        public bool ReportBoardAvailability { get; set; }
        public int ReportBoardLengthMin { get; set; }
        public int ReportBoardLengthMax { get; set; }
        public int ReportBoardIpMax { get; set; }
        public int ReportBoardTimeMin { get; set; }

        public bool CountryIpVersion4BlockAvailability { get; set; }
        public string CountryIpVersion4BlockCollection { get; set; }
        public string CountryIpVersion4ExclusionIpNumberCollection { get; set; }

        public bool CountryIpVersion6BlockAvailability { get; set; }
        public string CountryIpVersion6BlockCollection { get; set; }
        public string CountryIpVersion6ExclusionIpNumberCollection { get; set; }

        public bool BadIpVersion4BlockAvailability { get; set; }
        public string BadIpVersion4BlockExclusionIpNumberCollection { get; set; }

        public bool BadIpVersion6BlockAvailability { get; set; }
        public string BadIpVersion6BlockExclusionIpNumberCollection { get; set; }

        public bool DnsBlockListIpVersion4Availability { get; set; }
        public bool DnsBlockListIpVersion4Cache { get; set; }
        public string DnsBlockListIpVersion4Collection { get; set; }
        public string DnsBlockListIpVersion4ExclusionIpNumberCollection { get; set; }

        public bool DnsBlockListIpVersion6Availability { get; set; }
        public bool DnsBlockListIpVersion6Cache { get; set; }
        public string DnsBlockListIpVersion6Collection { get; set; }
        public string DnsBlockListIpVersion6ExclusionIpNumberCollection { get; set; }

        public bool ApiBlockListIpVersion4Availability { get; set; }
        public bool ApiBlockListIpVersion4Cache { get; set; }
        public string ApiBlockListIpVersion4Collection { get; set; }
        public string ApiBlockListIpVersion4ExclusionIpNumberCollection { get; set; }

        public bool ApiBlockListIpVersion6Availability { get; set; }
        public bool ApiBlockListIpVersion6Cache { get; set; }
        public string ApiBlockListIpVersion6Collection { get; set; }
        public string ApiBlockListIpVersion6ExclusionIpNumberCollection { get; set; }

        public VerificationType FieldVerificationType { get; set; }

        public string VerificationPublicKey { get; set; }
        public string VerificationSecretKey { get; set; }

        public VerificationMode ReplyFieldVerificationMode { get; set; }
        public int ReplyFieldVerificationLocalTime { get; set; }
        public VerificationMode ThreadFieldVerificationMode { get; set; }
        public int ThreadFieldVerificationLocalTime { get; set; }

        public bool SynchronizationThreadAvailability { get; set; }
        public bool SynchronizationBoardAvailability { get; set; }

        public bool SearchAvailability { get; set; }
        public bool CatalogAvailability { get; set; }

        public bool AnonymizationAvailability { get; set; }
        public int AnonymizationTimeMax { get; set; }

        public BoardUpdateCommand()
        {
            BoardId = "";
            Name = "";

            Availability = false;
            PostAvailability = false;

            Type = BoardType.Standard;
            TypeGroupCollection = "";

            Accessibility = BoardAccessibility.Public;
            AccessibilityPassword = "";
            AccessibilityIpVersion4NumberCollection = "";
            AccessibilityIpVersion6NumberCollection = "";

            Visibility = BoardVisibility.Visible;

            Anonymous = "";
            Language = "";
            TimeZone = "";

            OnlineCounterAvailability = false;

            CssThemeCollection = "";

            CssInline = "";
            CssExternalCollection = "";

            JsInline = "";
            JsExternalCollection = "";

            InformationAside = "";
            InformationHeader = "";
            InformationFooter = "";

            MarkupStaticCustomCollection = "";
            MarkupDynamicCustomCollection = "";

            UserIdRequirement = false;
            CountryRequirement = false;

            PageMax = 0;
            PageThreadMax = 0;

            ThreadReplyPreviewMax = 0;
            ThreadPinReplyPreviewMax = 0;

            ThreadBumpLockReplyMax = 0;
            ThreadBumpLockTimeMax = 0;
            ThreadExcessiveTimeMax = 0;

            ReplyFieldNameAvailability = false;
            ReplyFieldNameRequirement = false;
            ReplyFieldNameLengthMin = 0;
            ReplyFieldNameLengthMax = 0;
            ThreadFieldNameAvailability = false;
            ThreadFieldNameRequirement = false;
            ThreadFieldNameLengthMin = 0;
            ThreadFieldNameLengthMax = 0;

            ReplyFieldEmailAvailability = false;
            ReplyFieldEmailRequirement = false;
            ReplyFieldEmailLengthMin = 0;
            ReplyFieldEmailLengthMax = 0;
            ThreadFieldEmailAvailability = false;
            ThreadFieldEmailRequirement = false;
            ThreadFieldEmailLengthMin = 0;
            ThreadFieldEmailLengthMax = 0;

            ReplyFieldSubjectAvailability = false;
            ReplyFieldSubjectRequirement = false;
            ReplyFieldSubjectLengthMin = 0;
            ReplyFieldSubjectLengthMax = 0;
            ThreadFieldSubjectAvailability = false;
            ThreadFieldSubjectRequirement = false;
            ThreadFieldSubjectLengthMin = 0;
            ThreadFieldSubjectLengthMax = 0;

            ReplyFieldMessageAvailability = false;
            ReplyFieldMessageRequirement = false;
            ReplyFieldMessageLengthMin = 0;
            ReplyFieldMessageLengthMax = 0;
            ReplyFieldMessageLineMax = 0;
            ReplyFieldMessageLinkMax = 0;
            ReplyFieldMessageMentionMax = 0;
            ThreadFieldMessageAvailability = false;
            ThreadFieldMessageRequirement = false;
            ThreadFieldMessageLengthMin = 0;
            ThreadFieldMessageLengthMax = 0;
            ThreadFieldMessageLineMax = 0;
            ThreadFieldMessageLinkMax = 0;
            ThreadFieldMessageMentionMax = 0;

            ReplyFieldFileAvailability = false;
            ReplyFieldFileRequirement = false;
            ReplyFieldFileSizeMin = 0;
            ReplyFieldFileSizeMax = 0;
            ReplyFieldFileOriginality = false;
            ReplyFieldFileTypeCollection = "";
            ReplyFieldFileImageWidthMin = 0;
            ReplyFieldFileImageWidthMax = 0;
            ReplyFieldFileImageHeightMin = 0;
            ReplyFieldFileImageHeightMax = 0;
            ReplyFieldFileVideoWidthMin = 0;
            ReplyFieldFileVideoWidthMax = 0;
            ReplyFieldFileVideoHeightMax = 0;
            ReplyFieldFileVideoHeightMin = 0;
            ReplyFieldFileVideoDurationMin = 0;
            ReplyFieldFileVideoDurationMax = 0;
            ReplyFieldFileAudioDurationMin = 0;
            ReplyFieldFileAudioDurationMax = 0;
            ReplyFilePreviewWidthMax = 0;
            ReplyFilePreviewHeightMax = 0;
            ThreadFieldFileAvailability = false;
            ThreadFieldFileRequirement = false;
            ThreadFieldFileSizeMin = 0;
            ThreadFieldFileSizeMax = 0;
            ThreadFieldFileOriginality = false;
            ThreadFieldFileTypeCollection = "";
            ThreadFieldFileImageWidthMin = 0;
            ThreadFieldFileImageWidthMax = 0;
            ThreadFieldFileImageHeightMin = 0;
            ThreadFieldFileImageHeightMax = 0;
            ThreadFieldFileVideoWidthMin = 0;
            ThreadFieldFileVideoWidthMax = 0;
            ThreadFieldFileVideoHeightMax = 0;
            ThreadFieldFileVideoHeightMin = 0;
            ThreadFieldFileVideoDurationMin = 0;
            ThreadFieldFileVideoDurationMax = 0;
            ThreadFieldFileAudioDurationMin = 0;
            ThreadFieldFileAudioDurationMax = 0;
            ThreadFilePreviewWidthMax = 0;
            ThreadFilePreviewHeightMax = 0;

            ReplyFieldOptionSpoilerAvailability = false;
            ReplyFieldOptionSageAvailability = false;

            ThreadFieldOptionSpoilerAvailability = false;
            ThreadFieldOptionSageAvailability = false;
            ThreadFieldOptionUserIdAvailability = false;
            ThreadFieldOptionCountryAvailability = false;

            ReplyFieldPasswordAvailability = false;
            ReplyFieldPasswordRequirement = false;
            ReplyFieldPasswordLengthMin = 0;
            ReplyFieldPasswordLengthMax = 0;
            ThreadFieldPasswordAvailability = false;
            ThreadFieldPasswordRequirement = false;
            ThreadFieldPasswordLengthMin = 0;
            ThreadFieldPasswordLengthMax = 0;

            ReplyTimeMin = 0;
            ThreadTimeMin = 0;

            ReplyDeleteAvailability = false;
            ReplyDeleteTimeMin = 0;
            ReplyDeleteTimeMax = 0;
            ThreadDeleteAvailability = false;
            ThreadDeleteTimeMin = 0;
            ThreadDeleteTimeMax = 0;

            ReplyFileDeleteAvailability = false;
            ReplyFileDeleteTimeMin = 0;
            ReplyFileDeleteTimeMax = 0;
            ThreadFileDeleteAvailability = false;
            ThreadFileDeleteTimeMin = 0;
            ThreadFileDeleteTimeMax = 0;

            ReportBoardAvailability = false;
            ReportBoardLengthMin = 0;
            ReportBoardLengthMax = 0;
            ReportBoardIpMax = 0;
            ReportBoardTimeMin = 0;

            CountryIpVersion4BlockAvailability = false;
            CountryIpVersion4BlockCollection = "";
            CountryIpVersion4ExclusionIpNumberCollection = "";

            CountryIpVersion6BlockAvailability = false;
            CountryIpVersion6BlockCollection = "";
            CountryIpVersion6ExclusionIpNumberCollection = "";

            BadIpVersion4BlockAvailability = false;
            BadIpVersion4BlockExclusionIpNumberCollection = "";

            BadIpVersion6BlockAvailability = false;
            BadIpVersion6BlockExclusionIpNumberCollection = "";

            DnsBlockListIpVersion4Availability = false;
            DnsBlockListIpVersion4Cache = false;
            DnsBlockListIpVersion4Collection = "";
            DnsBlockListIpVersion4ExclusionIpNumberCollection = "";

            DnsBlockListIpVersion6Availability = false;
            DnsBlockListIpVersion6Cache = false;
            DnsBlockListIpVersion6Collection = "";
            DnsBlockListIpVersion6ExclusionIpNumberCollection = "";

            ApiBlockListIpVersion4Availability = false;
            ApiBlockListIpVersion4Cache = false;
            ApiBlockListIpVersion4Collection = "";
            ApiBlockListIpVersion4ExclusionIpNumberCollection = "";

            ApiBlockListIpVersion6Availability = false;
            ApiBlockListIpVersion6Cache = false;
            ApiBlockListIpVersion6Collection = "";
            ApiBlockListIpVersion6ExclusionIpNumberCollection = "";

            FieldVerificationType = VerificationType.None;

            VerificationPublicKey = "";
            VerificationSecretKey = "";

            ReplyFieldVerificationMode = VerificationMode.None;
            ReplyFieldVerificationLocalTime = 0;
            ThreadFieldVerificationMode = VerificationMode.None;
            ThreadFieldVerificationLocalTime = 0;

            SynchronizationThreadAvailability = false;
            SynchronizationBoardAvailability = false;

            SearchAvailability = false;
            CatalogAvailability = false;

            AnonymizationAvailability = false;
            AnonymizationTimeMax = 0;
        }
    }
}