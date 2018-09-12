using System;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Retrospective.Models
{
    public class Password
    {
        private static byte[] salt = {200, 34, 34, 145, 98};
        private const int iterationCount = 10000;

        public static string GetHash(string value)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: value,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: iterationCount, 
                numBytesRequested: 256));
        }
    }
}