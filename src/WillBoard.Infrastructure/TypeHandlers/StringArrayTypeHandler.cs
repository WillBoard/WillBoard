using Dapper;
using System;
using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using WillBoard.Core.Converters;

namespace WillBoard.Infrastructure.TypeHandlers
{
    public class StringArrayTypeHandler : SqlMapper.TypeHandler<string[]>
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters =
            {
                new DateTimeJsonConverter(),
                new BigIntegerJsonConverter()
            }
        };

        public override string[] Parse(object value)
        {
            if (value == null)
            {
                return Array.Empty<string>();
            }

            return JsonSerializer.Deserialize<string[]>(value.ToString(), JsonSerializerOptions);
        }

        public override void SetValue(IDbDataParameter parameter, string[] value)
        {
            parameter.Value = JsonSerializer.Serialize(value, JsonSerializerOptions);
        }
    }
}