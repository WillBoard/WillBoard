using System.Globalization;
using System.Numerics;

namespace WillBoard.Core.Utilities
{
    public static class IpRange
    {
        public static uint[] IpVersion4NumberWithCidrToRange(uint ipNumber, byte cidr)
        {
            byte maskbits = cidr;
            uint mask = 0xffffffff;
            mask <<= (32 - maskbits);

            uint ipNumberStart = ipNumber & mask;
            uint ipNumberEnd = ipNumber | (mask ^ 0xffffffff);

            return new uint[] { ipNumberStart, ipNumberEnd };
        }

        public static BigInteger[] IpVersion6NumberWithCidrToRange(BigInteger ipNumber, byte cidr)
        {
            byte maskbits = cidr;
            BigInteger mask = BigInteger.Parse("00FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF", NumberStyles.HexNumber);
            mask <<= (128 - maskbits);

            BigInteger ipNumberStart = ipNumber & mask;
            BigInteger ipNumberEnd = ipNumber + ((new BigInteger(1) << (int)(128 - cidr)) - 1);

            return new BigInteger[] { ipNumberStart, ipNumberEnd };
        }
    }
}