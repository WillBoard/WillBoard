using System;

namespace WillBoard.Core.Extensions
{
    public static class DateTimeExtension
    {
        public static string ToRfc3339String(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static string ToUtcRfc3339String(this DateTime dateTime)
        {
            if (dateTime.Kind != DateTimeKind.Utc)
            {
                dateTime = TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.Utc);
            }
            return dateTime.ToRfc3339String();
        }

        public static string ToIso8601String(this DateTime dateTime)
        {
            if (dateTime.Kind != DateTimeKind.Utc)
            {
                return dateTime.ToString("yyyy-MM-ddTHH:mm:ss");
            }
            else
            {
                return dateTime.ToString("yyyy-MM-ddTHH:mm:ssZ");
            }
        }

        public static string ToUtcIso8601String(this DateTime dateTime)
        {
            if (dateTime.Kind != DateTimeKind.Utc)
            {
                dateTime = TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.Utc);
            }
            return dateTime.ToIso8601String();
        }

        public static DateTime ParseUtcIso8601String(this string value)
        {
            DateTime dateTime = DateTime.ParseExact(value, "yyyy-MM-ddTHH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture);
            dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
            return dateTime;
        }

        public static DateTime ParseUtcRfc3339String(this string value)
        {
            DateTime dateTime = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
            return dateTime;
        }
    }
}