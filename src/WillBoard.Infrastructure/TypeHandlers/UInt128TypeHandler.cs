using System;
using System.Data;
using System.Numerics;
using Dapper;

namespace WillBoard.Infrastructure.TypeHandlers
{
    public class UInt128TypeHandler : SqlMapper.TypeHandler<UInt128>
    {
        public override UInt128 Parse(object value)
        {
            return UInt128.Parse((string)value);
        }

        public override void SetValue(IDbDataParameter parameter, UInt128 value)
        {
            parameter.Value = (BigInteger)value;
        }
    }
}
