
#if !DEBUG
//#define CONSTANTTRIAL
//#define TIMETRIAL
#endif



#if DEBUG && TRIALTEST
#if !CONSTANTTRIAL
#define CONSTANTTRIAL
#endif
#if !TIMETRIAL
#define TIMETRIAL
#endif
#endif
#if CONSTANTTRIAL || TIMETRIAL
#define HASTRIAL
//#define CRYPTOTRIAL
#endif

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Windows.Forms;
using Microsoft.Win32;
namespace ClimbingCompetition
{
    /// <summary>
    /// Выполняется проверка демо-версии программы
    /// </summary>
    public static class ValidationClass
    {
        public static bool IsTrial
        {
            get
            {
#if HASTRIAL
                return true;
#else
                return false;
#endif
            }
        }

#if TIMETRIAL
        const int TRIAL_TIME = 130;
#endif
#if CONSTANTTRIAL  
        static readonly DateTime LAST_TRIAL = new DateTime(2014, 03, 15);
#endif
#if HASTRIAL
        private static String KeyFileName = "ApiDownlload.dll";
        private static String TrialFileName="ApiUplload.dll";
        private static String RegBranch = "Software\\Microsoft\\Office\\3.0\\Common";
        private static String RegKey = "{4F4679E0-2C17-46DF-8594-C514AA67A2AC}";
        private static String RegParam = "{7696D4EB-D2D3-4D8B-925F-F18229D03345}";
        private enum CheckResult { Allow, Expired, CryptoViolation, TimeViolation, FileViolation }

        [Serializable]
        internal sealed class TrialData
        {
            private DateTime entryCreated;
            public DateTime EntryCreated
            {
                get { return entryCreated.ToUniversalTime(); }
                private set { entryCreated = value.ToUniversalTime(); }
            }

            private DateTime lastStart;
            public DateTime LastStart
            {
                get { return lastStart.ToUniversalTime(); }
                set { lastStart = value.ToUniversalTime(); }
            }

            public TrialData(DateTime entryCreated)
            {
                this.EntryCreated = entryCreated;
            }
        }

        private static TrialData CheckFileData(String fileName, String keyFileName)
        {
            if (!File.Exists(fileName))
                return new TrialData(DateTime.Now) { LastStart = DateTime.Now };
            byte[] buffer = new byte[1024];
            using (MemoryStream mstr = new MemoryStream())
            {
                using (var f = File.Open(fileName, FileMode.Open, FileAccess.Read))
                {
                    int n;
                    while ((n = f.Read(buffer, 0, buffer.Length)) > 0)
                        mstr.Write(buffer, 0, n);
                }
                return (TrialData)ClmEncryptor.DecryptObject(keyFileName, mstr.ToArray());
            }
        }
        private static void SaveToFile(TrialData data, String fileName, String keyFileName)
        {
            var encryptedObject = ClmEncryptor.EncryptObject(keyFileName, data);
            using (var f = File.Open(fileName, FileMode.Create, FileAccess.Write))
            {
                f.Write(encryptedObject, 0, encryptedObject.Length);
            }
        }

        private static TrialData CheckRegistryData(String regBranch, String regKey, String regParam, String keyFileName)
        {
            object obj;
            using (var rk = Registry.CurrentUser.CreateSubKey(String.Format("{0}{1}{2}",
                regBranch,
                regBranch.EndsWith("\\", StringComparison.Ordinal) ? String.Empty : "\\",
                regKey)))
            {
                obj = rk.GetValue(regParam);
            }
            if (obj == null)
                return new TrialData(DateTime.Now) { LastStart = DateTime.Now };
            byte[] encryptedObject = obj as byte[];
            if (encryptedObject == null)
                throw new CryptographicException("Unexpected data type");
            return (TrialData)ClmEncryptor.DecryptObject(keyFileName, encryptedObject);
        }
        private static void SaveToRegistry(TrialData data, String regBranch, String regKey, String regParam, String keyFileName)
        {
            byte[] encryptedObject = ClmEncryptor.EncryptObject(keyFileName, data);
            using (var rk = Registry.CurrentUser.CreateSubKey(String.Format("{0}{1}{2}",
                regBranch,
                regBranch.EndsWith("\\", StringComparison.Ordinal) ? String.Empty : "\\",
                regKey)))
            {
                rk.SetValue(regParam, encryptedObject, RegistryValueKind.Binary);
            }
        }

        private static CheckResult CheckTrialStats(out int remainingTime)
        {
            remainingTime = -1;
            try
            {
                List<TrialData> retreivedData = new List<TrialData>{
                CheckFileData(TrialFileName, KeyFileName),
                CheckRegistryData(RegBranch, RegKey, RegParam, KeyFileName)};

                retreivedData.Sort((a, b) => a.EntryCreated.CompareTo(b.EntryCreated));
                DateTime installationDate = retreivedData[0].EntryCreated;

                retreivedData.Sort((a, b) => b.LastStart.CompareTo(a.LastStart));
                DateTime lastStart = retreivedData[0].LastStart;

                DateTime uncNow = DateTime.UtcNow;
#if CONSTANTTRIAL
                if (uncNow.Date > LAST_TRIAL.Date)
                    return CheckResult.Expired;
                if (remainingTime < 0)
                    remainingTime = (LAST_TRIAL.Date - uncNow.Date).Days;

#endif
#if TIMETRIAL
                if (uncNow < lastStart)
                    return CheckResult.TimeViolation;
                var lastDay = installationDate.Date.AddDays(TRIAL_TIME);
                if (uncNow.Date > lastDay)
                    return CheckResult.Expired;
                int rmDays = (lastDay.Date - uncNow.Date).Days;
                if (remainingTime < 0 || rmDays < remainingTime)
                    remainingTime = rmDays;

#endif
                TrialData newData = new TrialData(installationDate) { LastStart = uncNow };
                if (File.Exists(KeyFileName))
                    File.Delete(KeyFileName);
                SaveToFile(newData, TrialFileName, KeyFileName);
                SaveToRegistry(newData, RegBranch, RegKey, RegParam, KeyFileName);
            }
            catch (FileNotFoundException) { return CheckResult.FileViolation; }
            catch (CryptographicException) { return CheckResult.CryptoViolation; }
            catch (SerializationException) { return CheckResult.CryptoViolation; }

            return CheckResult.Allow;
        }
#endif
        public static bool AllowExecute()
        {
            int n;
            return AllowExecute(out n, false);
        }

        public static bool AllowExecute(out int TimeRemaining, bool showDaysMessage)
        {
#if !HASTRIAL
            TimeRemaining = -1;
            return true;
#else
            var cRes = CheckTrialStats(out TimeRemaining);
            if (cRes == CheckResult.Allow)
            {
                if (showDaysMessage && TimeRemaining >= 0)
                    MessageBox.Show(String.Format("Пробная версия. Осталось {0} дней", TimeRemaining));
                return true;
            }
            String sMsg;
            switch (cRes)
            {
                case CheckResult.CryptoViolation:
                    sMsg = "Не хорошо ломать шифр";
                    break;
                case CheckResult.FileViolation:
                    sMsg = "Вы куда-то потеряли ценный файлик";
                    break;
                case CheckResult.TimeViolation:
                    sMsg = "Жульничать с переводом времени плохо";
                    break;
                default:
                    sMsg = "Пробный период истек";
                    break;
            }
            MessageBox.Show(sMsg, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
#endif
        } 
    }
#if HASTRIAL
#if CRYPTOTRIAL
    [Serializable]
    internal sealed class DESParams
    {
        public byte[] IV { get; set; }
        public byte[] Key { get; set; }

        public DESParams() { }
        public DESParams(DESCryptoServiceProvider provider)
        {
            this.IV = provider.IV;
            this.Key = provider.Key;
        }

        public void SaveToFile(String fileName)
        {
            BinaryFormatter fmt = new BinaryFormatter();
            using (var f = File.Open(fileName, FileMode.Create, FileAccess.Write))
            {
                fmt.Serialize(f, this);
            }
        }

        public static DESParams LoadFromFile(String fileName)
        {
            if (!File.Exists(fileName))
                return null;
            BinaryFormatter fmt = new BinaryFormatter();
            using (var f = File.Open(fileName, FileMode.Open, FileAccess.Read))
            {
                return (DESParams)fmt.Deserialize(f);
            }
        }
    }
#endif

    internal static class ClmEncryptor
    {
#if CRYPTOTRIAL
        private static DESCryptoServiceProvider CreateProvider(String fileName)
        {
            DESParams dsp = DESParams.LoadFromFile(fileName);
            var provider = new DESCryptoServiceProvider();
            if (dsp == null)
            {
                provider.GenerateIV();
                provider.GenerateKey();
                dsp = new DESParams(provider);
                dsp.SaveToFile(fileName);
            }
            else
            {
                provider.IV = dsp.IV;
                provider.Key = dsp.Key;
            }
            return provider;
        }
#endif

        public static byte[] EncryptData(String keyFileName, byte[] rawData)
        {
            return DoCryptoTransform(keyFileName, rawData, p => p.CreateEncryptor(), CryptoStreamMode.Write);
        }

        public static byte[] DecryptData(String keyFileName, byte[] encryptedData)
        {
            return DoCryptoTransform(keyFileName, encryptedData, p => p.CreateDecryptor(), CryptoStreamMode.Write);
        }

        public static byte[] EncryptObject(String keyFileName, object o)
        {
            BinaryFormatter fmt = new BinaryFormatter();
            byte[] buffer;
            using (MemoryStream mstr = new MemoryStream())
            {
                fmt.Serialize(mstr, o);
                buffer = mstr.ToArray();
            }

            return EncryptData(keyFileName, buffer);
        }

        public static Object DecryptObject(String keyFileName, byte[] encryptedData)
        {
            var objData = DecryptData(keyFileName, encryptedData);
            BinaryFormatter fmt = new BinaryFormatter();
            using (MemoryStream mstr = new MemoryStream(objData))
            {
                return fmt.Deserialize(mstr);
            }
        }

        private delegate ICryptoTransform CreateTransform(DES provider);
        private static byte[] DoCryptoTransform(String keyFileName, byte[] sourceData, CreateTransform transformFunc, CryptoStreamMode mode)
        {
#if CRYPTOTRIAL
            using (MemoryStream mstr = new MemoryStream())
            {
                using (var provider = CreateProvider(keyFileName))
                {
                    using (var cryptoStream = new CryptoStream(mstr, transformFunc(provider), mode))
                    {
                        cryptoStream.Write(sourceData, 0, sourceData.Length);
                    }
                }
                return mstr.ToArray();
            }
#else
            return sourceData;
#endif
        }
    }
#endif
}
