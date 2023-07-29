using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using WillBoard.Core.Extensions;

namespace WillBoard.Core.Converters
{
    public class DateTimeJsonConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var date = reader.GetString();
            return date.ParseUtcIso8601String();
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            var date = value.ToIso8601String();
            writer.WriteStringValue(date);
        }
    }
}