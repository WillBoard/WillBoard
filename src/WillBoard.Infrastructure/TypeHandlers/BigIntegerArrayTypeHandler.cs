using Dapper;
using System;
using System.Data;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;
using WillBoard.Core.Converters;

namespace WillBoard.Infrastructure.TypeHandlers
{
    public class BigIntegerArrayTypeHandler : SqlMapper.TypeHandler<BigInteger[]>
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

        public override BigInteger[] Parse(object value)
        {
            if (value == null)
            {
                return Array.Empty<BigInteger>();
            }

            return JsonSerializer.Deserialize<BigInteger[]>(value.ToString(), JsonSerializerOptions);
        }

        public override void SetValue(IDbDataParameter parameter, BigInteger[] value)
        {
            parameter.Value = JsonSerializer.Serialize(value, JsonSerializerOptions);
        }
    }
}