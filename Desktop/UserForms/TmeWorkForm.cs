using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace ClimbingCompetition
{
    /// <summary>
    /// Форма отображается при выполнении операции резервного копирования БД и т.п. Тупо идёт секундомер
    /// </summary>
    public partial class TmeWorkForm : Form
    {
        AccountForm.TimeConsumingWork func;
        Thread thr = null;
        int min = 0, sec = 0;
        bool autoHide;
        public TmeWorkForm(string caption, AccountForm.TimeConsumingWork func, bool autoHide)
        {
            this.autoHide = autoHide;
            InitializeComponent();
            this.Text = caption;
            this.func = func;
            thr = new Thread(ThreadFunc);
            try { thr.TrySetApartmentState(ApartmentState.STA); }
            catch { }
            thr.Start();
        }

        private void ThreadFunc()
        {
            func();
            timer.Stop();
            if (!autoHide)
                MessageBox.Show("Процесс завершён");
            try
            {
                this.Invoke(new EventHandler(delegate
                {
                    try { this.Close(); }
                    catch { }
                }));
            }
            catch { }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (sec == 59)
            {
                min++;
                sec = 0;
            }
            else
                sec++;
            label2.Invoke(new EventHandler(delegate
            {
                label2.Text = min.ToString("00") + ":" + sec.ToString("00");
            }));
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                if (thr != null)
                    thr.Abort();
                timer.Stop();
                MessageBox.Show("Процесс отменён");
                this.Invoke(new EventHandler(delegate { this.Close(); }));
            }
            catch (Exception ex)
            { MessageBox.Show("Отмена невозможна:\r\n" + ex.Message); }
        }
    }
}
