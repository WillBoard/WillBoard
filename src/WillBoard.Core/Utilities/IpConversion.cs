﻿using System;
using System.Net;
using System.Numerics;
using WillBoard.Core.Enums;

namespace WillBoard.Core.Utilities
{
    public static class IpConversion
    {
        public static UInt32 IpVersion4AddressToIpVersion4Number(IPAddress ipAddress)
        {
            Span<byte> ipAddressByteSpan = ipAddress.GetAddressBytes();

            return NetworkByteOrder.ReadUInt32(ipAddressByteSpan);
        }

        public static UInt128 IpVersion6AddressToIpVersion6Number(IPAddress ipAddress)
        {
            Span<byte> ipAddressByteSpan = ipAddress.GetAddressBytes();

            return NetworkByteOrder.ReadUInt128(ipAddressByteSpan);
        }

        public static IPAddress IpVersion4NumberToIpVersion4Address(UInt32 ipNumber)
        {
            Span<byte> ipAddressByteSpan = stackalloc byte[4];

            NetworkByteOrder.WriteUInt32(ipAddressByteSpan, ipNumber);

            return new IPAddress(ipAddressByteSpan);
        }

        public static IPAddress IpVersion6NumberToIpVersion6Address(UInt128 ipNumber)
        {
            Span<byte> ipAddressByteSpan = stackalloc byte[16];

            NetworkByteOrder.WriteUInt128(ipAddressByteSpan, ipNumber);

            return new IPAddress(ipAddressByteSpan);
        }

        // IP Address = w.x.y.z
        // To reverse IP number to IP address,
        // w = int (IP Number / 16777216 ) % 256
        // x = int (IP Number / 65536    ) % 256
        // y = int (IP Number / 256      ) % 256
        // z = int (IP Number            ) % 256
        // where % is the modulus operator and int returns the integer part of the division.
        public static string IpVersion4NumberToIpVersion4AddressString(UInt32 ipNumber)
        {
            return ((ipNumber / 16777216) % 256) + "." + ((ipNumber / 65536) % 256) + "." + ((ipNumber / 256) % 256) + "." + (ipNumber % 256);
        }

        // IP Address = a:b:c:d:e:f:g:h
        // To reverse IP number to IP address,
        // a = int(IP Number / (65536 ^ 7)) % 65536
        // b = int (IP Number / (65536^6) ) % 65536
        // c = int (IP Number / (65536^5) ) % 65536
        // d = int (IP Number / (65536^4) ) % 65536
        // e = int (IP Number / (65536^3) ) % 65536
        // f = int (IP Number / (65536^2) ) % 65536
        // g = int (IP Number / 65536 ) % 65536
        // h = IP Number % 65536
        // where % is the modulus operator and int returns the integer part of the division.
        // NOTE: All parts need to be converted into hexadecimal to be part of the IPv6 address.
        // More about IPv6 address formats https://www.ibm.com/docs/en/i/7.2?topic=concepts-ipv6-address-formats
        public static string IpVersion6NumberToIpVersion6AddressString(UInt128 ipNumber, bool omitLeadingZeros = true)
        {
            var format = omitLeadingZeros ? "X" : "X4";
            return ((int)((ipNumber / BigInteger.Pow(65536, 7)) % 65536)).ToString(format) + ":" + ((int)((ipNumber / BigInteger.Pow(65536, 6)) % 65536)).ToString(format) + ":" + ((int)((ipNumber / BigInteger.Pow(65536, 5)) % 65536)).ToString(format) + ":" + ((int)((ipNumber / BigInteger.Pow(65536, 4)) % 65536)).ToString(format) + ":" + ((int)((ipNumber / BigInteger.Pow(65536, 3)) % 65536)).ToString(format) + ":" + ((int)((ipNumber / BigInteger.Pow(65536, 2)) % 65536)).ToString(format) + ":" + ((int)((ipNumber / 65536) % 65536)).ToString(format) + ":" + ((int)(ipNumber % 65536)).ToString(format);
        }

        public static IPAddress IpNumberToIpAddress(IpVersion ipVersion, UInt128 ipNumber)
        {
            switch (ipVersion)
            {
                case IpVersion.IpVersion4:
                    return IpVersion4NumberToIpVersion4Address((UInt32)ipNumber);

                case IpVersion.IpVersion6:
                    return IpVersion6NumberToIpVersion6Address(ipNumber);

                default:
                    return null;
            }
        }

        public static string IpNumberToIpAddressString(IpVersion ipVersion, UInt128 ipNumber)
        {
            switch (ipVersion)
            {
                case IpVersion.IpVersion4:
                    return IpVersion4NumberToIpVersion4AddressString((UInt32)ipNumber);

                case IpVersion.IpVersion6:
                    return IpVersion6NumberToIpVersion6AddressString(ipNumber);

                default:
                    return "-";
            }
        }
    }
}