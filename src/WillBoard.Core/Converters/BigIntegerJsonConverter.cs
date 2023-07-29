using System;
using System.Globalization;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WillBoard.Core.Converters
{
    public class BigIntegerJsonConverter : JsonConverter<BigInteger>
    {
        public override BigInteger Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.Number)
            {
                throw new JsonException(string.Format("Found token type {0} but expected token type {1}.", reader.TokenType, JsonTokenType.Number));
            }

            return BigInteger.Parse(Encoding.UTF8.GetString(reader.ValueSpan));
        }

        public override void Write(Utf8JsonWriter writer, BigInteger value, JsonSerializerOptions options)
        {
            var number = value.ToString(NumberFormatInfo.InvariantInfo);
            writer.WriteRawValue(number);
        }
    }
}