using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace ClimbingCompetition
{
    public interface IExceptionLogger
    {
        void WriteLog(Exception ex, bool needToShow);
        void WriteLog(string message, bool needToShow);
    }

    public static class CExceptionLogger
    {
        public static string LogFile
        {
            get { return ClimbingCompetition.Properties.Settings.Default.LogFile; }
            set
            {
                ClimbingCompetition.Properties.Settings lsSet = ClimbingCompetition.Properties.Settings.Default;
                lsSet.LogFile = value;
                lsSet.Save();
            }
        }
        public static void WriteLogStatic(string message, Form owner, bool needToShow)
        {
            try
            {
                string fileName = ClimbingCompetition.Properties.Settings.Default.LogFile;
                string msg = DateTime.Now.ToString("G") + ": " + message;
                if (fileName.Length < 1 || needToShow)
                {
                    Thread thr = new Thread(new ParameterizedThreadStart(delegate(object arg)
                    {
                        try
                        {
                            if (arg == null || !(arg is string))
                                return;
                            string s = (string)arg;
                            if (owner == null)
                                MessageBox.Show(s);
                            else if (owner.InvokeRequired)
                                owner.Invoke(new EventHandler(delegate { MessageBox.Show(owner, s, "Ошибка!"); }));
                            else
                                MessageBox.Show(owner, s, "Ошибка!");
                        }
                        catch { }
                    }));
                    thr.Start(msg);
                }
                if (!String.IsNullOrEmpty(fileName))
                {
                    StreamWriter swr = new StreamWriter(fileName, true);
                    try { swr.WriteLine(DateTime.Now.ToString("G") + ": " + message); }
                    finally { swr.Close(); }
                }
            }
            catch { }
        }
    }
}
