using System;
using System.Globalization;

namespace WillBoard.Core.Utilities
{
    public class FormatUtility
    {
        private static readonly string[] SizeSuffixArray = new string[]
        {
            "B", "KiB", "MiB", "GiB", "TiB", "PiB", "EiB", "ZiB", "YiB"
        };

        private static readonly NumberFormatInfo SizeNumberFormatInfo = new NumberFormatInfo()
        {
            NumberDecimalSeparator = "."
        };

        public static string FormatDataSize(long value)
        {
            if (value < 0)
            {
                return string.Format("-{0}", FormatDataSize(-value));
            }
            if (value == 0)
            {
                return string.Format("0.00 {0}", SizeSuffixArray[0]);
            }

            var mag = (int)Math.Log(value, 1024);
            var adjustedSize = (decimal)value / (1L << (mag * 10));

            return string.Format("{0} {1}", adjustedSize.ToString("0.00", SizeNumberFormatInfo), SizeSuffixArray[mag]);
        }
    }
}