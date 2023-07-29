using System;
using System.Numerics;
using WillBoard.Core.Classes;
using WillBoard.Core.Enums;

namespace WillBoard.Core.Entities
{
    public class Board
    {
        public string BoardId { get; set; }
        public string Name { get; set; }

        public bool Availability { get; set; }

        public BoardType Type { get; set; }
        public string[] TypeGroupCollection { get; set; }

        public BoardAccessibility Accessibility { get; set; }
        public string AccessibilityPassword { get; set; }
        public UInt32[] AccessibilityIpVersion4NumberCollection { get; set; }
        public BigInteger[] AccessibilityIpVersion6NumberCollection { get; set; }

        public BoardVisibility Visibility { get; set; }

        public string Anonymous { get; set; }
        public string Language { get; set; }
        public string TimeZone { get; set; }

        public bool OnlineCounterAvailability { get; set; }

        public CssTheme[] CssThemeCollection { get; set; }

        public string CssInline { get; set; }
        public string[] CssExternalCollection { get; set; }

        public string JsInline { get; set; }
        public string[] JsExternalCollection { get; set; }

        public string InformationAside { get; set; }
        public string InformationHeader { get; set; }
        public string InformationFooter { get; set; }

        public MarkupCustom[] MarkupStaticCustomCollection { get; set; }
        public MarkupCustom[] MarkupDynamicCustomCollection { get; set; }

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
        public string[] ReplyFieldFileTypeCollection { get; set; }
        public int ReplyFieldFileImageWidthMin { get; set; }
        public int ReplyFieldFileImageWidthMax { get; set; }
        public int ReplyFieldFileImageHeightMin { get; set; }
        public int ReplyFieldFileImageHeightMax { get; set; }
        public int ReplyFieldFileVideoWidthMin { get; set; }
        public int ReplyFieldFileVideoWidthMax { get; set; }
        public int ReplyFieldFileVideoHeightMin { get; set; }
        public int ReplyFieldFileVideoHeightMax { get; set; }
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
        public string[] ThreadFieldFileTypeCollection { get; set; }
        public int ThreadFieldFileImageWidthMin { get; set; }
        public int ThreadFieldFileImageWidthMax { get; set; }
        public int ThreadFieldFileImageHeightMin { get; set; }
        public int ThreadFieldFileImageHeightMax { get; set; }
        public int ThreadFieldFileVideoWidthMin { get; set; }
        public int ThreadFieldFileVideoWidthMax { get; set; }
        public int ThreadFieldFileVideoHeightMin { get; set; }
        public int ThreadFieldFileVideoHeightMax { get; set; }
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
        public string[] CountryIpVersion4BlockCollection { get; set; }
        public UInt32[] CountryIpVersion4ExclusionIpNumberCollection { get; set; }

        public bool CountryIpVersion6BlockAvailability { get; set; }
        public string[] CountryIpVersion6BlockCollection { get; set; }
        public BigInteger[] CountryIpVersion6ExclusionIpNumberCollection { get; set; }

        public bool BadIpVersion4BlockAvailability { get; set; }
        public UInt32[] BadIpVersion4BlockExclusionIpNumberCollection { get; set; }

        public bool BadIpVersion6BlockAvailability { get; set; }
        public BigInteger[] BadIpVersion6BlockExclusionIpNumberCollection { get; set; }

        public bool DnsBlockListIpVersion4Availability { get; set; }
        public bool DnsBlockListIpVersion4Cache { get; set; }
        public BlockList[] DnsBlockListIpVersion4Collection { get; set; }
        public UInt32[] DnsBlockListIpVersion4ExclusionIpNumberCollection { get; set; }

        public bool DnsBlockListIpVersion6Availability { get; set; }
        public bool DnsBlockListIpVersion6Cache { get; set; }
        public BlockList[] DnsBlockListIpVersion6Collection { get; set; }
        public BigInteger[] DnsBlockListIpVersion6ExclusionIpNumberCollection { get; set; }

        public bool ApiBlockListIpVersion4Availability { get; set; }
        public bool ApiBlockListIpVersion4Cache { get; set; }
        public BlockList[] ApiBlockListIpVersion4Collection { get; set; }
        public UInt32[] ApiBlockListIpVersion4ExclusionIpNumberCollection { get; set; }

        public bool ApiBlockListIpVersion6Availability { get; set; }
        public bool ApiBlockListIpVersion6Cache { get; set; }
        public BlockList[] ApiBlockListIpVersion6Collection { get; set; }
        public BigInteger[] ApiBlockListIpVersion6ExclusionIpNumberCollection { get; set; }

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

        public Board()
        {
        }

        public Board(string boardId)
        {
            BoardId = boardId;
            Name = "";

            Availability = false;

            Type = BoardType.Standard;
            TypeGroupCollection = Array.Empty<string>();

            Accessibility = BoardAccessibility.Public;
            AccessibilityPassword = "";
            AccessibilityIpVersion4NumberCollection = Array.Empty<UInt32>();
            AccessibilityIpVersion6NumberCollection = Array.Empty<BigInteger>();

            Visibility = BoardVisibility.Hidden;

            Anonymous = "Anonymous";
            Language = "en";
            TimeZone = TimeZoneInfo.Utc.Id;

            OnlineCounterAvailability = false;

            CssThemeCollection = new CssTheme[]
            {
                new CssTheme()
                {
                    ThemeId = "main",
                    Name = "Main",
                    Path = ""
                },
                new CssTheme()
                {
                    ThemeId = "dark",
                    Name = "Dark",
                    Path = "/css/dark.css"
                },
            };

            CssInline = "";
            CssExternalCollection = Array.Empty<string>();

            JsInline = "";
            JsExternalCollection = Array.Empty<string>();

            InformationAside = "";
            InformationHeader = "";
            InformationFooter = "";

            MarkupStaticCustomCollection = Array.Empty<MarkupCustom>();
            MarkupDynamicCustomCollection = Array.Empty<MarkupCustom>();

            UserIdRequirement = false;
            CountryRequirement = false;

            PostAvailability = false;

            PageMax = 10;
            PageThreadMax = 10;

            ThreadReplyPreviewMax = 3;
            ThreadPinReplyPreviewMax = 1;

            ThreadBumpLockReplyMax = 2000;
            ThreadBumpLockTimeMax = 7776000;

            ThreadExcessiveTimeMax = 10800;

            ReplyFieldNameAvailability = false;
            ReplyFieldNameRequirement = false;
            ReplyFieldNameLengthMin = 0;
            ReplyFieldNameLengthMax = 32;
            ThreadFieldNameAvailability = false;
            ThreadFieldNameRequirement = false;
            ThreadFieldNameLengthMin = 0;
            ThreadFieldNameLengthMax = 32;

            ReplyFieldEmailAvailability = false;
            ReplyFieldEmailRequirement = false;
            ReplyFieldEmailLengthMin = 0;
            ReplyFieldEmailLengthMax = 32;
            ThreadFieldEmailAvailability = false;
            ThreadFieldEmailRequirement = false;
            ThreadFieldEmailLengthMin = 0;
            ThreadFieldEmailLengthMax = 32;

            ReplyFieldSubjectAvailability = false;
            ReplyFieldSubjectRequirement = false;
            ReplyFieldSubjectLengthMin = 0;
            ReplyFieldSubjectLengthMax = 32;
            ThreadFieldSubjectAvailability = false;
            ThreadFieldSubjectRequirement = false;
            ThreadFieldSubjectLengthMin = 0;
            ThreadFieldSubjectLengthMax = 32;

            ReplyFieldMessageAvailability = true;
            ReplyFieldMessageRequirement = true;
            ReplyFieldMessageLengthMin = 0;
            ReplyFieldMessageLengthMax = 5000;
            ReplyFieldMessageLineMax = 50;
            ReplyFieldMessageLinkMax = 10;
            ReplyFieldMessageMentionMax = 10;
            ThreadFieldMessageAvailability = true;
            ThreadFieldMessageRequirement = true;
            ThreadFieldMessageLengthMin = 0;
            ThreadFieldMessageLengthMax = 5000;
            ThreadFieldMessageLineMax = 50;
            ThreadFieldMessageLinkMax = 10;
            ThreadFieldMessageMentionMax = 10;

            ReplyFieldFileAvailability = true;
            ReplyFieldFileRequirement = false;
            ReplyFieldFileSizeMin = 1;
            ReplyFieldFileSizeMax = 10485760;
            ReplyFieldFileOriginality = true;
            ReplyFieldFileTypeCollection = Array.Empty<string>();
            ReplyFieldFileImageWidthMin = 1;
            ReplyFieldFileImageWidthMax = 10000;
            ReplyFieldFileImageHeightMin = 1;
            ReplyFieldFileImageHeightMax = 10000;
            ReplyFieldFileVideoWidthMin = 1;
            ReplyFieldFileVideoWidthMax = 10000;
            ReplyFieldFileVideoHeightMin = 1;
            ReplyFieldFileVideoHeightMax = 10000;
            ReplyFieldFileVideoDurationMin = 1;
            ReplyFieldFileVideoDurationMax = 3600;
            ReplyFieldFileAudioDurationMin = 1;
            ReplyFieldFileAudioDurationMax = 3600;
            ReplyFilePreviewWidthMax = 128;
            ReplyFilePreviewHeightMax = 128;
            ThreadFieldFileAvailability = true;
            ThreadFieldFileRequirement = true;
            ThreadFieldFileSizeMin = 1;
            ThreadFieldFileSizeMax = 10485760;
            ThreadFieldFileOriginality = true;
            ThreadFieldFileTypeCollection = Array.Empty<string>();
            ThreadFieldFileImageWidthMin = 1;
            ThreadFieldFileImageWidthMax = 10000;
            ThreadFieldFileImageHeightMin = 1;
            ThreadFieldFileImageHeightMax = 10000;
            ThreadFieldFileVideoWidthMin = 1;
            ThreadFieldFileVideoWidthMax = 10000;
            ThreadFieldFileVideoHeightMin = 1;
            ThreadFieldFileVideoHeightMax = 10000;
            ThreadFieldFileVideoDurationMin = 1;
            ThreadFieldFileVideoDurationMax = 3600;
            ThreadFieldFileAudioDurationMin = 1;
            ThreadFieldFileAudioDurationMax = 3600;
            ThreadFilePreviewWidthMax = 256;
            ThreadFilePreviewHeightMax = 256;

            ReplyFieldOptionSpoilerAvailability = false;
            ReplyFieldOptionSageAvailability = false;

            ThreadFieldOptionSpoilerAvailability = false;
            ThreadFieldOptionSageAvailability = false;
            ThreadFieldOptionUserIdAvailability = false;
            ThreadFieldOptionCountryAvailability = false;

            ReplyTimeMin = 15;
            ThreadTimeMin = 600;

            ReplyFieldPasswordAvailability = true;
            ReplyFieldPasswordRequirement = true;
            ReplyFieldPasswordLengthMin = 6;
            ReplyFieldPasswordLengthMax = 64;
            ThreadFieldPasswordAvailability = true;
            ThreadFieldPasswordRequirement = true;
            ThreadFieldPasswordLengthMin = 6;
            ThreadFieldPasswordLengthMax = 64;

            ReplyDeleteAvailability = true;
            ReplyDeleteTimeMin = 1;
            ReplyDeleteTimeMax = 1440;
            ThreadDeleteAvailability = true;
            ThreadDeleteTimeMin = 1;
            ThreadDeleteTimeMax = 1440;

            ReplyFileDeleteAvailability = true;
            ReplyFileDeleteTimeMin = 1;
            ReplyFileDeleteTimeMax = 1440;
            ThreadFileDeleteAvailability = true;
            ThreadFileDeleteTimeMin = 1;
            ThreadFileDeleteTimeMax = 1440;

            ReportBoardAvailability = true;
            ReportBoardLengthMin = 6;
            ReportBoardLengthMax = 64;
            ReportBoardIpMax = 20;
            ReportBoardTimeMin = 15;

            CountryIpVersion4BlockAvailability = false;
            CountryIpVersion4BlockCollection = Array.Empty<string>();
            CountryIpVersion4ExclusionIpNumberCollection = Array.Empty<UInt32>();

            CountryIpVersion6BlockAvailability = false;
            CountryIpVersion6BlockCollection = Array.Empty<string>();
            CountryIpVersion6ExclusionIpNumberCollection = Array.Empty<BigInteger>();

            BadIpVersion4BlockAvailability = false;
            BadIpVersion4BlockExclusionIpNumberCollection = Array.Empty<UInt32>();

            BadIpVersion6BlockAvailability = false;
            BadIpVersion6BlockExclusionIpNumberCollection = Array.Empty<BigInteger>();

            DnsBlockListIpVersion4Availability = false;
            DnsBlockListIpVersion4Collection = Array.Empty<BlockList>();
            DnsBlockListIpVersion4ExclusionIpNumberCollection = Array.Empty<UInt32>();

            DnsBlockListIpVersion6Availability = false;
            DnsBlockListIpVersion6Collection = Array.Empty<BlockList>();
            DnsBlockListIpVersion6ExclusionIpNumberCollection = Array.Empty<BigInteger>();

            ApiBlockListIpVersion4Availability = false;
            ApiBlockListIpVersion4Collection = Array.Empty<BlockList>();
            ApiBlockListIpVersion4ExclusionIpNumberCollection = Array.Empty<UInt32>();

            ApiBlockListIpVersion6Availability = false;
            ApiBlockListIpVersion6Collection = Array.Empty<BlockList>();
            ApiBlockListIpVersion6ExclusionIpNumberCollection = Array.Empty<BigInteger>();

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