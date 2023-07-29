using Dapper;
using System;
using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using WillBoard.Core.Classes;
using WillBoard.Core.Converters;

namespace WillBoard.Infrastructure.TypeHandlers
{
    public class CssThemeArrayTypeHandler : SqlMapper.TypeHandler<CssTheme[]>
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

        public override CssTheme[] Parse(object value)
        {
            if (value == null)
            {
                return Array.Empty<CssTheme>();
            }

            return JsonSerializer.Deserialize<CssTheme[]>(value.ToString(), JsonSerializerOptions);
        }

        public override void SetValue(IDbDataParameter parameter, CssTheme[] value)
        {
            parameter.Value = JsonSerializer.Serialize(value, JsonSerializerOptions);
        }
    }
}