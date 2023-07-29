using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Security.Cryptography;
using WillBoard.Core.Interfaces.Services;
using WillBoard.Core.Utilities;

namespace WillBoard.Infrastructure.Services
{
    // Based on Identity password hasher (https://github.com/dotnet/aspnetcore/blob/main/src/Identity/Extensions.Core/src/PasswordHasher.cs)
    public class PasswordService : IPasswordService
    {
        public string HashPassword(string password)
        {
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            KeyDerivationPrf prf = KeyDerivationPrf.HMACSHA256;
            int iterCount = 10000;
            int saltSize = 128 / 8;
            int numBytesRequested = 256 / 8;

            byte[] salt = new byte[saltSize];
            rng.GetBytes(salt);
            byte[] subkey = KeyDerivation.Pbkdf2(password, salt, prf, iterCount, numBytesRequested);

            Span<byte> outputBytes = new byte[12 + salt.Length + subkey.Length];
            NetworkByteOrder.WriteUInt32(outputBytes.Slice(0, 4), (uint)prf);
            NetworkByteOrder.WriteUInt32(outputBytes.Slice(4, 4), (uint)iterCount);
            NetworkByteOrder.WriteUInt32(outputBytes.Slice(8, 4), (uint)saltSize);
            salt.CopyTo(outputBytes.Slice(12, salt.Length));
            subkey.CopyTo(outputBytes.Slice(12 + saltSize, subkey.Length));
            return Convert.ToBase64String(outputBytes);
        }

        public bool VerifyPassword(string hashedPassword, string password)
        {
            try
            {
                Span<byte> decodedHashedPassword = Convert.FromBase64String(hashedPassword);

                KeyDerivationPrf prf = (KeyDerivationPrf)NetworkByteOrder.ReadUInt32(decodedHashedPassword.Slice(0, 4));
                int iterCount = (int)NetworkByteOrder.ReadUInt32(decodedHashedPassword.Slice(4, 4));
                int saltLength = (int)NetworkByteOrder.ReadUInt32(decodedHashedPassword.Slice(8, 4));

                if (saltLength < 128 / 8)
                {
                    return false;
                }

                byte[] salt = new byte[saltLength];
                decodedHashedPassword.Slice(12, salt.Length).CopyTo(salt);

                int subkeyLength = decodedHashedPassword.Length - 12 - salt.Length;
                if (subkeyLength < 128 / 8)
                {
                    return false;
                }

                byte[] expectedSubkey = new byte[subkeyLength];
                decodedHashedPassword.Slice(12 + salt.Length, expectedSubkey.Length).CopyTo(expectedSubkey);

                byte[] actualSubkey = KeyDerivation.Pbkdf2(password, salt, prf, iterCount, subkeyLength);
                return CryptographicOperations.FixedTimeEquals(actualSubkey, expectedSubkey);
            }
            catch
            {
                return false;
            }
        }
    }
}