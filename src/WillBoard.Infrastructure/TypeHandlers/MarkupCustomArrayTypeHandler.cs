using Dapper;
using System;
using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using WillBoard.Core.Classes;
using WillBoard.Core.Converters;

namespace WillBoard.Infrastructure.TypeHandlers
{
    public class MarkupCustomArrayTypeHandler : SqlMapper.TypeHandler<MarkupCustom[]>
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

        public override MarkupCustom[] Parse(object value)
        {
            if (value == null)
            {
                return Array.Empty<MarkupCustom>();
            }

            return JsonSerializer.Deserialize<MarkupCustom[]>(value.ToString(), JsonSerializerOptions);
        }

        public override void SetValue(IDbDataParameter parameter, MarkupCustom[] value)
        {
            parameter.Value = JsonSerializer.Serialize(value, JsonSerializerOptions);
        }
    }
}