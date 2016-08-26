using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace ClimbingCompetition
{
    public static class PasswordWorkingClass
    {
        private static readonly Encoding PasswordEncoding = Encoding.Unicode;
        private static readonly byte[] bPWDKey = { 0xAB, 0x56, 0x61, 0x02, 0xF1, 0xA2, 0x3B, 0x70 };
        private static readonly byte[] bIV = { 0xBB, 0xC1, 0x54, 0x60, 0x01, 0x5B, 0x34, 0x7A };
        public static byte[] EncryptPassword(string password)
        {
            return EncryptPassword(password, bIV, bPWDKey);
        }
        public static byte[] EncryptPassword(string password, byte[] IV, byte[] key)
        {
            using (DESCryptoServiceProvider csp = new DESCryptoServiceProvider())
            {
                csp.IV = IV;
                csp.Key = key;
                using (MemoryStream mstr = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(
                        mstr, csp.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        byte[] b = PasswordEncoding.GetBytes(password);
                        cs.Write(b, 0, b.Length);
                        cs.Flush();
                        return mstr.ToArray();
                    }
                }
            }
        }

        public static string DecryptPassword(byte[] src)
        {
            return DecryptPassword(src, bIV, bPWDKey);
        }
        public static string DecryptPassword(byte[] src, byte[] IV, byte[] key)
        {
            using (DESCryptoServiceProvider dsp = new DESCryptoServiceProvider())
            {
                dsp.IV = IV;
                dsp.Key = key;
                using (MemoryStream mstr = new MemoryStream(src))
                {
                    using (CryptoStream cs = new CryptoStream(mstr,
                        dsp.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        byte[] buffer = new byte[1024];
                        List<byte> bufferFull = new List<byte>();
                        int n;
                        while ((n = cs.Read(buffer, 0, buffer.Length)) > 0)
                            for (int i = 0; i < n; i++)
                                bufferFull.Add(buffer[i]);
                        return PasswordEncoding.GetString(bufferFull.ToArray());
                    }
                }
            }
        }

        private static readonly char[] csLetters ={'a','b','c','d','e','f','g','h','i','j','k','l','m',
                                             'n','o','p','q','r','s','t','u','v','w','x','y',
                                             'z','а','б','в','г','д','е','v','ё','ж','з','и',
                                             'й','к','л','м','н','о','п','р','с','т','у','ф',
                                             'х','ц','ч','ш','щ','ъ','ы','ь','э','ю','я'};
        private static readonly char[] ccLetters ={'A','B','C','D','E','F','G','H','I','J','K',
                                              'L','M','N','O','P','Q','R','S','T','U','V',
                                              'W','X','Y','Z','А','Б','В','Г','Д','Е','Ё',
                                              'Ж','З','И','Й','К','Л','М','Н','О','П','Р',
                                              'С','Т','У','Ф','Х','Ц','Ч','Ш','Щ','Ъ','Ы',
                                              'Ь','Э','Ю','Я'};
        private static readonly char[] cDigits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };

        public static bool CheckStrength(string pwd)
        {
            bool res;
            if (pwd.Length < 8)
                return false;
            res = (pwd.IndexOfAny(ccLetters) >= 0);
            res = res && (pwd.IndexOfAny(csLetters) >= 0);
            res = res && (pwd.IndexOfAny(cDigits) >= 0);
            return res;
        }

        private static Random rnd = new Random();

        private enum CharType { CAPITAL, LOW, DIGIT, ALL };

        private static char getChar(CharType t)
        {
            List<char> src = new List<char>();
            int i;
            if (t == CharType.CAPITAL || t == CharType.ALL)
                for (i = 0; i < 26; i++)
                    src.Add(ccLetters[i]);
            if (t == CharType.LOW || t == CharType.ALL)
                for (i = 0; i < 26; i++)
                    src.Add(csLetters[i]);
            if (t == CharType.DIGIT || t == CharType.ALL)
                foreach (char c in cDigits)
                    src.Add(c);
            int n = rnd.Next(src.Count);
            return src[n];

        }

        public static string generatePassword(int length)
        {
            if (length < 8)
                throw new ArgumentOutOfRangeException("length");
            string password;
            int attempts = 0;
            do
            {
                password = "";
                for (int i = 0; i < length; i++)
                    password += getChar(CharType.ALL);
                attempts++;
            } while (!CheckStrength(password) && (attempts < 100));
            if (!CheckStrength(password))
                password = password.Substring(0, (length - 3)) + getChar(CharType.LOW) + getChar(CharType.DIGIT) + getChar(CharType.CAPITAL);
            return password;
        }
    }
}
