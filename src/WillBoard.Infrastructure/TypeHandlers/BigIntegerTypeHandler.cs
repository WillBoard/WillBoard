using Dapper;
using System.Data;
using System.Numerics;

namespace WillBoard.Infrastructure.TypeHandlers
{
    public class BigIntegerTypeHandler : SqlMapper.TypeHandler<BigInteger>
    {
        public override BigInteger Parse(object value)
        {
            return BigInteger.Parse((string)value);
        }

        public override void SetValue(IDbDataParameter parameter, BigInteger value)
        {
            parameter.Value = value.ToString();
        }
    }
}