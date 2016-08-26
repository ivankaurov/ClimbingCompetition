using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

namespace ClimbingCompetition
{
    /// <summary>
    /// Форма для настройки СОМ порта для подключения скоростной системы
    /// </summary>
    public partial class SerialPortConfig : Form
    {
        private string portName;
        private SerialPort sp = null;
        private SerialPortConfig(string portName)
        {
            InitializeComponent();
            this.Text = "Настройка порта " + portName;
            this.portName = portName;
            cmbBaudRate.SelectedIndex = appSettings.Default.cmbBaudRate;
            cmbDataBits.SelectedIndex = appSettings.Default.cmbDataBits;
            cmbParity.SelectedIndex = appSettings.Default.cmbParity;
            cmbStopBits.SelectedIndex = appSettings.Default.cmbStopBits;
            cmbHandShake.SelectedIndex = appSettings.Default.cmdHandShake;
            cbShowInfo.Checked = appSettings.Default.cbShowInfo;
            tbAutoClose.Text = appSettings.Default.AutoCloseTimer.ToString();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            sp = new SerialPort(portName);
            int tmp;
            if (cmbBaudRate.SelectedIndex > 0)
            {
                if (int.TryParse(cmbBaudRate.Text, out tmp))
                    sp.BaudRate = tmp;
                else
                {
                    MessageBox.Show("Скорость введена не верно");
                    return;
                }
            }
            if (cmbDataBits.SelectedIndex > 0)
            {
                if (int.TryParse(cmbDataBits.Text, out tmp))
                    sp.DataBits = tmp;
                else
                {
                    MessageBox.Show("Биты данных введены не верно");
                    return;
                }
            }
            if (cmbHandShake.SelectedIndex > 0)
            {
                try
                {
                    Handshake hsh = (Handshake)Enum.Parse(typeof(Handshake), cmbHandShake.Text, true);
                    sp.Handshake = hsh;
                }
                catch
                {
                    MessageBox.Show("Handshake протокол выбран неправильно;");
                    return;
                }
            }
            if (cmbStopBits.SelectedIndex > 0)
            {
                StopBits sBits;
                switch (cmbStopBits.Text)
                {
                    case "(нет)":
                        sBits = StopBits.None;
                        break;
                    case "1":
                        sBits = StopBits.One;
                        break;
                    case "1.5":
                        sBits = StopBits.OnePointFive;
                        break;
                    case "2":
                        sBits = StopBits.Two;
                        break;
                    default:
                        MessageBox.Show("Стоповые биты введены неправильно");
                        return;
                }
                sp.StopBits = sBits;
            }
            if (cmbParity.SelectedIndex > 0)
            {
                Parity p;
                switch (cmbParity.Text)
                {
                    case "(нет)":
                        p = Parity.None;
                        break;
                    case "Чёт":
                        p = Parity.Even;
                        break;
                    case "Нечёт":
                        p = Parity.Odd;
                        break;
                    case "Маркер":
                        p = Parity.Mark;
                        break;
                    case "Пробел":
                        p = Parity.Space;
                        break;
                    default:
                        MessageBox.Show("Чётность введена неправильно");
                        return;
                }
                sp.Parity = p;
            }
            int autoClose;
            if (!int.TryParse(tbAutoClose.Text, out autoClose))
            {
                MessageBox.Show("Время закрытия окна введено неверно");
                return;
            }
            if (autoClose < 0 || (autoClose > 0 && autoClose < 5))
            {
                MessageBox.Show("Время закрытия окна должно быть более 4 секунд");
                return;
            }
            appSettings aset = appSettings.Default;
            aset.cmbBaudRate = cmbBaudRate.SelectedIndex;
            aset.cmbDataBits = cmbDataBits.SelectedIndex;
            aset.cmbParity = cmbParity.SelectedIndex;
            aset.cmbStopBits = cmbStopBits.SelectedIndex;
            aset.cmdHandShake = cmbHandShake.SelectedIndex;
            aset.systemPort = sp;
            aset.cbShowInfo = cbShowInfo.Checked;
            aset.AutoCloseTimer = autoClose;
            aset.Save();
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            sp = null;
            this.Close();
        }

        public static SerialPort ConfigurePort(string portName, IWin32Window owner)
        {
            SerialPortConfig spc = new SerialPortConfig(portName);
            spc.ShowDialog(owner);
            return spc.sp;
        }
    }
}
