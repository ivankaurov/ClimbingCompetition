using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ClimbingCompetition
{
    public sealed partial class SetAdminPassword : Form
    {
        private string oldPwd;
        private bool oldExists;
        private bool cancel;
        private IWin32Window owner;
        private SetAdminPassword(string oldPwd, bool oldExists, IWin32Window owner)
        {
            InitializeComponent();
            this.oldExists = oldExists;
            this.oldPwd = oldPwd;
            if (this.oldExists)
                this.Text = "Смена пароля администратора";
            else
                this.Text = "Введите пароль администратора";
            this.lblOld.Visible = this.lblOld.Enabled =
                this.tbOld.Visible = this.tbOld.Enabled = oldExists;
            this.cancel = false;
            this.owner = owner;
        }

        

        private void btnOK_Click(object sender, EventArgs e)
        {
            if(oldExists)
                if (tbOld.Text != oldPwd)
                {
                    MessageBox.Show(owner, "Старый пароль введён неправильно.");
                    return;
                }
            if (tbConf.Text != tbNew.Text)
            {
                MessageBox.Show(owner, "Пароли не совпадают.");
                return;
            }
            if (!PasswordWorkingClass.CheckStrength(tbNew.Text))
            {
                MessageBox.Show(owner, "Пароль не соответствует требованиям безопасности.\r\n" +
                    "Длина пароля должна быть не менее 8 символов,\r\n" +
                    "пароль должен содержать не менее одной заглавной буквы, " +
                    "не менее одной строчной буквы и не менее одной цифры.");
                return;
            }
            this.cancel = false;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.cancel = true;
            this.Close();
        }

        private static bool CreateNewPassword(string old, bool oldExists, IWin32Window owner, out string newPassword)
        {
            SetAdminPassword sap = new SetAdminPassword(old, oldExists, owner);
            sap.ShowDialog(owner);
            bool res = !sap.cancel;
            if (res)
                newPassword = sap.tbNew.Text;
            else
                newPassword = "";
            return res;
        }

        public static bool ChangePassword(string oldPassword, IWin32Window owner, out string newPassword)
        {
            return CreateNewPassword(oldPassword, true, owner, out newPassword);
        }

        public static bool CreateNewPassword(IWin32Window owner, out string newPassword)
        {
            return CreateNewPassword("", false, owner, out newPassword);
        }
    }
}
