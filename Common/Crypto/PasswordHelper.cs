using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crypto
{
    public static class PasswordHelper
    {
        private static readonly char[] nonAN = { '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '\"', '\'', '?', '/', '\\', '|' };

        private static Random random = new Random();

        public static string GeneratePassword(
            int minLength = 8,
            int maxLength = 30)
        {
            if (minLength < 0)
                minLength = 0;
            if (maxLength < minLength)
                maxLength = minLength;

            int passwordLength = random.Next(minLength, maxLength + 1);
            if (passwordLength < 1)
                return string.Empty;

            var result = new StringBuilder();
            while(result.Length < passwordLength)
            {
                bool upper = (random.Next() % 2) == 0;
                result.Append((char)random.Next(upper ? 'A' : 'a', 1 + (upper ? 'Z' : 'z')));
            }

            return result.ToString();
        }
    }
}
