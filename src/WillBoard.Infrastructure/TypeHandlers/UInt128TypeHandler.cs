using Dapper;
using System;
using System.Data;

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
            parameter.Value = value.ToString();
        }
    }
}