using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Crypto
{
    public static class Hashing
    {
        private static Random rnd = new Random();
        private static Encoding defaultEncoding = Encoding.Unicode;
        
        public static byte[] GetBytes(this String value, Byte[] salt = null)
        {
            if (salt == null || salt.Length < 1)
                return defaultEncoding.GetBytes(value ?? String.Empty);
            List<Byte> data = new List<byte>(value.GetBytes());
            data.AddRange(salt);
            return data.ToArray();
        }

        public static String GetString(this byte[] byteValue)
        {
            return defaultEncoding.GetString(byteValue);
        }

        public static byte[] ComputeHash(this byte[] value)
        {
            using (var provider = new SHA512CryptoServiceProvider())
            {
                return provider.ComputeHash(value);
            }
        }

        public static byte[] ComputeHash(this String value, byte[] salt = null)
        {
            return value.GetBytes(salt).ComputeHash();
        }

        public static string ComputeHashString(this string value, byte[] salt = null)
        {
            return Encoding.ASCII.GetString(value.ComputeHash(salt));
        }

        public static bool CheckHash(this byte[] openValue, byte[] hashValue)
        {
            if (openValue == null && hashValue == null)
                return true;
            else if (openValue == null || hashValue == null)
                return false;
            return openValue.ComputeHash()
                            .SequenceEqual(hashValue);
        }

        public static bool CheckHash(this String value, byte[] hashValue, byte[] salt = null)
        {
            return value.GetBytes(salt).CheckHash(hashValue);
        }

        public static byte[] GenerateSalt(int length = 20)
        {
            if (length < 1)
                throw new ArgumentOutOfRangeException("length");
            byte[] res = new byte[length];
            lock (rnd)
            {
                rnd.NextBytes(res);
            }
            return res;
        }
    }
}
