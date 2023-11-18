using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using WillBoard.Core.Classes;
using WillBoard.Core.Converters;

namespace WillBoard.Core.Utilities
{
    public static class ArrayConversion
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters =
            {
                new DateTimeJsonConverter(),
            }
        };

        public static string[] DeserializeString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return Array.Empty<string>();
            }

            return JsonSerializer.Deserialize<string[]>(value, JsonSerializerOptions);
        }

        public static string SerializeString(string[] value)
        {
            return JsonSerializer.Serialize(value, JsonSerializerOptions);
        }

        public static UInt32[] DeserializeUInt32(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return Array.Empty<UInt32>();
            }

            return JsonSerializer.Deserialize<UInt32[]>(value, JsonSerializerOptions);
        }

        public static string SerializeUInt32(UInt32[] value)
        {
            return JsonSerializer.Serialize(value, JsonSerializerOptions);
        }

        public static BlockList[] DeserializeBlockList(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return Array.Empty<BlockList>();
            }

            return JsonSerializer.Deserialize<BlockList[]>(value, JsonSerializerOptions);
        }

        public static UInt128[] DeserializeUInt128(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return Array.Empty<UInt128>();
            }

            return JsonSerializer.Deserialize<UInt128[]>(value, JsonSerializerOptions);
        }

        public static string SerializeUInt128(UInt128[] value)
        {
            return JsonSerializer.Serialize(value, JsonSerializerOptions);
        }

        public static string SerializeBlockList(BlockList[] value)
        {
            return JsonSerializer.Serialize(value, JsonSerializerOptions);
        }

        public static CssTheme[] DeserializeCssTheme(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return Array.Empty<CssTheme>();
            }

            return JsonSerializer.Deserialize<CssTheme[]>(value, JsonSerializerOptions);
        }

        public static string SerializeCssTheme(CssTheme[] value)
        {
            return JsonSerializer.Serialize(value, JsonSerializerOptions);
        }

        public static MarkupCustom[] DeserializeMarkupCustom(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return Array.Empty<MarkupCustom>();
            }

            return JsonSerializer.Deserialize<MarkupCustom[]>(value, JsonSerializerOptions);
        }

        public static string SerializeMarkupCustom(MarkupCustom[] value)
        {
            return JsonSerializer.Serialize(value, JsonSerializerOptions);
        }
    }
}