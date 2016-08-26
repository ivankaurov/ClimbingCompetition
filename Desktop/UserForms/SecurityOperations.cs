using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace ClimbingCompetition
{
    public sealed class SecurityOperations : System.Windows.Forms.Form
    {
        private System.Windows.Forms.RadioButton rbUseDSA;
        private System.Windows.Forms.RadioButton rbPassword;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private SqlConnection cn;
        private OpenFileDialog openKey;
        private static readonly Encoding currentEncoding = Encoding.Unicode;
        private readonly bool ownConnection;
        private SecurityOperations(SqlConnection _cn, bool show, bool newCn)
        {
            if (newCn)
            {
                this.ownConnection = true;
                this.cn = new SqlConnection(_cn.ConnectionString);
                this.cn.Open();
            }
            else
            {
                this.ownConnection = false;
                this.cn = _cn;
            }

            SortingClass.CheckColumn("CompetitionData", "signature", "image null", cn);
            SortingClass.CheckColumn("CompetitionData", "opwd", "image null", cn);
            SortingClass.CheckColumn("CompetitionData", "touse", "smallint not null default 0", cn);
            if (show)
                InitializeComponent();
        }

        private AuthType AuthenticationType
        {
            get { return (rbUseDSA.Checked ? AuthType.Signature : AuthType.Password); }
            set
            {
                if (value == AuthType.Signature)
                    rbUseDSA.Checked = true;
                else
                    rbPassword.Checked = true;
            }
        }

        private SqlCommand CreateCommand()
        {
            if (cn.State != System.Data.ConnectionState.Open)
                cn.Open();
            return new SqlCommand { Connection = cn };
        }
        private AuthType AuthTypeFromDB
        {
            get
            {
                SqlCommand cmd = CreateCommand();
                cmd.CommandText = "SELECT touse FROM CompetitionData(nolock)";
                var res = cmd.ExecuteScalar();
                if (res == null)
                    return AuthType.Password;
                return (AuthType)Convert.ToInt16(res);
            }
        }
        private void SaveAuthType(AuthType value, SqlTransaction tran = null)
        {
            var cmd = CreateCommand();
            cmd.Transaction = tran;
            cmd.CommandText = String.Format("UPDATE CompetitionData SET touse={0}", (short)value);
            cmd.ExecuteNonQuery();
        }

        private String PasswordFromDB
        {
            get
            {
                SqlCommand cmd = CreateCommand();
                cmd.CommandText = "SELECT opwd FROM CompetitionData(nolock)";
                var res = cmd.ExecuteScalar() as byte[];
                if (res == null)
                    return String.Empty;
                return currentEncoding.GetString(res);
            }
        }
        private void SavePassword(String value, SqlTransaction tran = null)
        {
            var cmd = CreateCommand();
            cmd.Transaction = tran;
            cmd.CommandText = "UPDATE CompetitionData SET opwd=@opwd";
            cmd.Parameters.Add("@opwd", System.Data.SqlDbType.Image).Value = currentEncoding.GetBytes(value ?? String.Empty);
            cmd.ExecuteNonQuery();
        }

        private byte[] GetKeyFromDB(SqlTransaction tran = null)
        {
            var cmd = CreateCommand();
            cmd.Transaction = tran;
            cmd.CommandText = "SELECT signature FROM CompetitionData(nolock)";
            var res = cmd.ExecuteScalar() as byte[];
            if (res == null || res.Length < 1)
                return null;
            else
                return res;
        }

        private bool SaveKey(SqlTransaction tran = null)
        {
            var oldKey = GetKeyFromDB(tran);
            if (oldKey != null)
            {
                switch (MessageBox.Show(this, "Заменить существующий ключ подписи?", String.Empty, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                {
                    case System.Windows.Forms.DialogResult.Cancel:
                        return false;
                    case System.Windows.Forms.DialogResult.No:
                        return true;
                }
            }
            if (openKey.ShowDialog(this) == System.Windows.Forms.DialogResult.Cancel)
                return false;
            if (String.IsNullOrEmpty(openKey.FileName))
                return false;
            byte[] buffer = new byte[1024];
            int n;
            try
            {
                using (MemoryStream mstr = new MemoryStream())
                {
                    using (var f = File.Open(openKey.FileName, FileMode.Open, FileAccess.Read))
                    {
                        while ((n = f.Read(buffer, 0, buffer.Length)) > 0)
                            mstr.Write(buffer, 0, n);
                    }
                    buffer = mstr.ToArray();
                }
            }
            catch (DirectoryNotFoundException)
            {
                MessageBox.Show(this, "Путь не найден");
                return false;
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show(this, String.Format("Файл {0} не найден", openKey.FileName));
                return false;
            }
            catch (IOException ex)
            {
                MessageBox.Show(this, String.Format("Ошибка открытия файла {1}:{0}{2}", Environment.NewLine, openKey.FileName, ex.Message));
                return false;
            }

            var cmd = CreateCommand();
            cmd.Transaction = tran;
            cmd.CommandText = "UPDATE CompetitionData SET signature=@signature";
            cmd.Parameters.Add("@signature", System.Data.SqlDbType.Image).Value = buffer;
            cmd.ExecuteNonQuery();
            return true;
        }

        private void ResetKey(SqlTransaction tran = null)
        {
            var cmd = CreateCommand();
            cmd.Transaction = tran;
            cmd.CommandText = "UPDATE CompetitionData SET signature=NULL";
            cmd.ExecuteNonQuery();
        }
        

        private void InitializeComponent()
        {
            this.rbUseDSA = new System.Windows.Forms.RadioButton();
            this.rbPassword = new System.Windows.Forms.RadioButton();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.openKey = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // rbUseDSA
            // 
            this.rbUseDSA.AutoSize = true;
            this.rbUseDSA.Location = new System.Drawing.Point(8, 33);
            this.rbUseDSA.Name = "rbUseDSA";
            this.rbUseDSA.Size = new System.Drawing.Size(48, 17);
            this.rbUseDSA.TabIndex = 0;
            this.rbUseDSA.Text = "ЭЦП";
            this.rbUseDSA.UseVisualStyleBackColor = true;
            // 
            // rbPassword
            // 
            this.rbPassword.AutoSize = true;
            this.rbPassword.Checked = true;
            this.rbPassword.Location = new System.Drawing.Point(8, 10);
            this.rbPassword.Name = "rbPassword";
            this.rbPassword.Size = new System.Drawing.Size(63, 17);
            this.rbPassword.TabIndex = 0;
            this.rbPassword.TabStop = true;
            this.rbPassword.Text = "Пароль";
            this.rbPassword.UseVisualStyleBackColor = true;
            this.rbPassword.CheckedChanged += new System.EventHandler(this.rbPassword_CheckedChanged);
            // 
            // tbPassword
            // 
            this.tbPassword.Location = new System.Drawing.Point(77, 9);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.PasswordChar = '*';
            this.tbPassword.Size = new System.Drawing.Size(100, 20);
            this.tbPassword.TabIndex = 1;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(8, 65);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(102, 65);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // openKey
            // 
            this.openKey.Filter = "Файлы ключа (*.key)|*.key|Все файлы (*.*)|*.8";
            this.openKey.Title = "Выбор файла ключа";
            // 
            // SecurityOperations
            // 
            this.AcceptButton = this.btnOK;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(184, 110);
            this.ControlBox = false;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.tbPassword);
            this.Controls.Add(this.rbPassword);
            this.Controls.Add(this.rbUseDSA);
            this.Name = "SecurityOperations";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Настройка безопасности";
            this.Load += new System.EventHandler(this.SecurityOperations_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && ownConnection && cn != null)
            {
                cn.Dispose();
                cn = null;
            }
            base.Dispose(disposing);
        }

        private void rbPassword_CheckedChanged(object sender, EventArgs e)
        {
            tbPassword.Enabled = rbPassword.Checked;
        }

        private void SecurityOperations_Load(object sender, EventArgs e)
        {
            this.AuthenticationType = AuthTypeFromDB;
            this.tbPassword.Text = PasswordFromDB;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (cn.State != System.Data.ConnectionState.Open)
                cn.Open();
            SqlTransaction tran = cn.BeginTransaction();
            try
            {
                var at = AuthenticationType;
                SaveAuthType(at, tran);
                switch (at)
                {
                    case AuthType.Signature:
                        SavePassword(String.Empty, tran);
                        if (!SaveKey(tran))
                        {
                            tran.Rollback();
                            return;
                        }
                        break;
                    case AuthType.Password:
                        if (String.IsNullOrEmpty(tbPassword.Text))
                        {
                            MessageBox.Show(this, "Пароль не введён");
                            tran.Rollback();
                            return;
                        }
                        SavePassword(tbPassword.Text, tran);
                        ResetKey(tran);
                        break;
                }
                tran.Commit();
            }
            catch
            {
                tran.Rollback();
                throw;
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private byte[] SignBytes(byte[] src)
        {
            var keyInfo = GetKeyFromDB();
            if (keyInfo == null)
                throw new WebEncryptionException("Ключ для ЭЦП не найден");
            String keyXml = Encoding.Unicode.GetString(keyInfo);

            /*BinaryFormatter fmt = new BinaryFormatter();
            DSAParameters algParams;
            using (MemoryStream mstr = new MemoryStream(keyInfo))
            {
                try { algParams = (DSAParameters)fmt.Deserialize(mstr); }
                catch (SerializationException ex) { throw new WebEncryptionException("Ключ поврежден", ex); }
            }*/
            using (DSACryptoServiceProvider provider = new DSACryptoServiceProvider())
            {
                try { provider.FromXmlString(keyXml); }
                catch (CryptographicException ex) { throw new WebEncryptionException("Ключ поврежден", ex); }
                try { return provider.SignData(src); }
                catch (CryptographicException ex) { throw new WebEncryptionException("Ошибка подписи данных", ex); }
            }
        }

        public static bool SetSecurityData(SqlConnection cn, IWin32Window owner = null)
        {
            SecurityOperations secOp = new SecurityOperations(cn, true, true);
            DialogResult dgRes;
            if (owner == null)
                dgRes = secOp.ShowDialog();
            else
                dgRes = secOp.ShowDialog(owner);
            return (dgRes == DialogResult.OK);
        }

        public static byte[] GetKey(SqlConnection cn)
        {
            return new SecurityOperations(cn, false, false).GetKeyFromDB();
        }

        public static String GetPassword(SqlConnection cn)
        {
            return new SecurityOperations(cn, false, false).PasswordFromDB;
        }

        public static AuthType GetAuthType(SqlConnection cn)
        {
            return new SecurityOperations(cn, false, false).AuthTypeFromDB;
        }

        public static XmlApiClient.SignBytes CreateSigningDelegate(SqlConnection cn)
        {
            if (GetAuthType(cn) != AuthType.Signature)
                return null;
            return (a => new SecurityOperations(cn, false, false).SignBytes(a));
        }
    }

    [Serializable]
    public class WebEncryptionException : ApplicationException
    {
        public WebEncryptionException()
        {
        }

        public WebEncryptionException(String message) : base(message) { }

        public WebEncryptionException(String message, Exception innerException) : base(message, innerException) { }
    }

    public enum AuthType : short { Password = 0, Signature = 1 }
}
