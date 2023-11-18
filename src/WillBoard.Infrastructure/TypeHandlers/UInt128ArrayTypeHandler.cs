using Dapper;
using System;
using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using WillBoard.Core.Converters;

namespace WillBoard.Infrastructure.TypeHandlers
{
    public class UInt128ArrayTypeHandler : SqlMapper.TypeHandler<UInt128[]>
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

        public override UInt128[] Parse(object value)
        {
            if (value == null)
            {
                return Array.Empty<UInt128>();
            }

            return JsonSerializer.Deserialize<UInt128[]>(value.ToString(), JsonSerializerOptions);
        }

        public override void SetValue(IDbDataParameter parameter, UInt128[] value)
        {
            parameter.Value = JsonSerializer.Serialize(value, JsonSerializerOptions);
        }
    }
}