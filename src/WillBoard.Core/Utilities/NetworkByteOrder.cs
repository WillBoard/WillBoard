using System;
using System.Buffers.Binary;
using System.Runtime.InteropServices;

namespace WillBoard.Core.Utilities
{
    // Network Byte Order = Big Endian
    public static class NetworkByteOrder
    {
        public static void WriteUInt16(Span<byte> buffer, UInt16 value)
        {
            BinaryPrimitives.WriteUInt16BigEndian(buffer, value);
        }

        public static UInt16 ReadUInt16(Span<byte> buffer)
        {
            return BinaryPrimitives.ReadUInt16BigEndian(buffer);
        }

        public static void WriteUInt32(Span<byte> buffer, UInt32 value)
        {
            BinaryPrimitives.WriteUInt32BigEndian(buffer, value);
        }

        public static UInt32 ReadUInt32(Span<byte> buffer)
        {
            return BinaryPrimitives.ReadUInt32BigEndian(buffer);
        }

        public static void WriteUInt64(Span<byte> buffer, UInt32 value)
        {
            BinaryPrimitives.WriteUInt64BigEndian(buffer, value);
        }

        public static UInt64 ReadUInt64(Span<byte> buffer)
        {
            return BinaryPrimitives.ReadUInt64BigEndian(buffer);
        }

        public static void WriteUInt128(Span<byte> buffer, UInt128 value)
        {
            BinaryPrimitives.WriteUInt128BigEndian(buffer, value);
        }

        public static UInt128 ReadUInt128(Span<byte> buffer)
        {
            return BinaryPrimitives.ReadUInt128BigEndian(buffer);
        }

        public static void WriteUuid(Span<byte> buffer, Guid value)
        {
            Span<byte> guidBytes = stackalloc byte[16];
            MemoryMarshal.Write(guidBytes, ref value);

            if (BitConverter.IsLittleEndian)
            {
                buffer[0] = guidBytes[3];
                buffer[1] = guidBytes[2];
                buffer[2] = guidBytes[1];
                buffer[3] = guidBytes[0];

                buffer[4] = guidBytes[5];
                buffer[5] = guidBytes[4];

                buffer[6] = guidBytes[7];
                buffer[7] = guidBytes[6];
            }
            else
            {
                buffer[0] = guidBytes[0];
                buffer[1] = guidBytes[1];
                buffer[2] = guidBytes[2];
                buffer[3] = guidBytes[3];

                buffer[4] = guidBytes[4];
                buffer[5] = guidBytes[5];

                buffer[6] = guidBytes[6];
                buffer[7] = guidBytes[7];
            }

            buffer[8] = guidBytes[8];
            buffer[9] = guidBytes[9];
            buffer[10] = guidBytes[10];
            buffer[11] = guidBytes[11];
            buffer[12] = guidBytes[12];
            buffer[13] = guidBytes[13];
            buffer[14] = guidBytes[14];
            buffer[15] = guidBytes[15];
        }

        public static Guid ReadUuid(Span<byte> buffer)
        {
            uint a = ReadUInt32(buffer.Slice(0, 4));
            ushort b = ReadUInt16(buffer.Slice(4, 2));
            ushort c = ReadUInt16(buffer.Slice(6, 2));

            var guid = new Guid(
                    a,
                    b,
                    c,
                    buffer[8],
                    buffer[9],
                    buffer[10],
                    buffer[11],
                    buffer[12],
                    buffer[13],
                    buffer[14],
                    buffer[15]
                );

            return guid;
        }
    }
}