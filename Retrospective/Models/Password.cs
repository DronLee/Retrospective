using System;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Retrospective.Models
{
    /// <summary>
    /// Класс для работы с паролем.
    /// </summary>
    internal class Password
    {
        /// <summary>
        /// Некая "соль", добавляемая к строке при получении хэша.
        /// </summary>
        private static byte[] _salt = {200, 34, 34, 145, 98};

        /// <summary>
        /// Количество итераций для получения хэша.
        /// </summary>
        private const int _iterationCount = 10000;

        /// <summary>
        /// Получение хэша строки.
        /// </summary>
        /// <param name="value">Строка, из которой будет получен хэш.</param>
        /// <returns>Хэш.</returns>
        internal static string GetHash(string value)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: value,
                salt: _salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: _iterationCount, 
                numBytesRequested: 256));
        }
    }
}