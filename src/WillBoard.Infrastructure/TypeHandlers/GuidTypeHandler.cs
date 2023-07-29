using Dapper;
using System;
using System.Data;
using WillBoard.Core.Utilities;

namespace WillBoard.Infrastructure.TypeHandlers
{
    // Read UUID as GUID
    // Write GUID as UUID
    // https://www.ietf.org/rfc/rfc4122.txt
    public class GuidTypeHandler : SqlMapper.TypeHandler<Guid>
    {
        public override Guid Parse(object value)
        {
            var guid = NetworkByteOrder.ReadUuid((byte[])value);
            return guid;
        }

        public override void SetValue(IDbDataParameter parameter, Guid value)
        {
            byte[] guid = new byte[16];
            NetworkByteOrder.WriteUuid(guid, value);
            parameter.Value = guid;
        }
    }
}