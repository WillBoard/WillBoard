using System;

namespace WillBoard.Core.Utilities
{
    public static class IpRange
    {
        public static UInt32[] IpVersion4NumberWithCidrToRange(UInt32 ipNumber, byte cidr)
        {
            byte maskBits = cidr;
            var mask = UInt32.MaxValue;
            mask <<= (32 - maskBits);

            UInt32 ipNumberStart = ipNumber & mask;
            UInt32 ipNumberEnd = ipNumber | (mask ^ UInt32.MaxValue);

            return new UInt32[] { ipNumberStart, ipNumberEnd };
        }

        public static UInt128[] IpVersion6NumberWithCidrToRange(UInt128 ipNumber, byte cidr)
        {
            byte maskBits = cidr;
            var mask = UInt128.MaxValue;
            mask <<= (128 - maskBits);

            UInt128 ipNumberStart = ipNumber & mask;
            UInt128 ipNumberEnd = ipNumber | (mask ^ UInt128.MaxValue);

            return new UInt128[] { ipNumberStart, ipNumberEnd };
        }
    }
}