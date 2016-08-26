using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;

namespace ListShow
{
    static class Program
    {
        static bool exceptionOccured = false;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try { Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException); }
            catch { }
            try { Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException); }
            catch { }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ShowForm());

        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            if (!exceptionOccured)
                exceptionOccured = true;
            string exMessage;
            if (e != null && e.Exception != null)
                exMessage = e.Exception.ToString();
            else
                exMessage = "";
            if (MessageBox.Show("Необработанная ошибка:\r\n" + exMessage +
                "\r\nЕсли такая ошибка будет появляться в дальнейшем, обратитесь к разработчику.\r\n" +
                "Продолжить работу?", "Необработанное исключение", MessageBoxButtons.YesNo, MessageBoxIcon.Error)
                == DialogResult.No)
                if (exceptionOccured)
                    try { Process.GetCurrentProcess().Kill(); }
                    catch (Exception ex) { MessageBox.Show("Невозможно завершить процесс:\r\n" + ex.Message); }
                else
                    try { Application.Exit(); }
                    catch
                    {
                        try { Process.GetCurrentProcess().Kill(); }
                        catch (Exception ex2) { MessageBox.Show("Невозможно завершить процесс:\r\n" + ex2.Message); }
                    }
        }
    }
}