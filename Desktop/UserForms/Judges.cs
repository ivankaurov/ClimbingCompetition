using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using Excel = Microsoft.Office.Interop.Excel;
using Word = Microsoft.Office.Interop.Word;
using System.IO;
using System.Threading;

namespace ClimbingCompetition
{
    /// <summary>
    /// Форма - список судей
    /// </summary>
    public partial class Judges : BaseForm
    {
        private Thread thrLoad = null;
        SqlConnection remote = null;
        SqlDataAdapter da, daPos;
        DataTable dt = new DataTable(), dtPos = new DataTable();
        public static readonly List<string> posLst = new List<string>();
        public static readonly List<string> posToBadge = new List<string>();
        private bool IsEditJudge
        {
            get { return tbSurname.Enabled; }
            set
            {
                if (IsEditPos && !IsNewJudge)
                    return;
                tbSurname.Enabled = tbName.Enabled = tbPatronimic.Enabled =
                    tbCat.Enabled = tbCity.Enabled = btnLoadPhoto.Enabled = btnDelPhoto.Enabled = value;
                btnPrintBadges.Enabled = rbGroupJudge.Enabled = rbGroupPos.Enabled = btnPosListEdit.Enabled =
                    btnPrintOneB.Enabled = btnExcelExport.Enabled = btnAdd.Enabled =
                    btnLoadFRinet.Enabled = btnLoadToInet.Enabled = !value;
                if (value)
                {
                    btnEditSubmit.Text = "Подтвердить";
                    btnDel.Text = "Отменить";
                }
                else
                {
                    btnEditSubmit.Text = "Правка";
                    btnDel.Text = "Удалить";
                }
                if (!value)
                    IsEditPos = false;
                btnAddPos.Enabled = btnDelPos.Enabled = btnEditPos.Enabled = value && lblIID.Text.Length > 0;
            }
        }
        private bool IsEditPos
        {
            get { return cbPos.Enabled; }
            set
            {

                if (value)
                {
                    btnEditPos.Text = "Подтвердить";
                    btnDelPos.Text = "Отменить";
                }
                else
                {
                    btnEditPos.Text = "Правка должности";
                    btnDelPos.Text = "Удалить должность";
                }
                btnEditSubmit.Enabled = btnDel.Enabled = !value;
                cbPos.Enabled = value;
                tbPos.Enabled = false;
                if (value)
                    btnAddPos.Enabled = false;
                else
                    btnAddPos.Enabled = IsEditJudge;
            }
        }
        private bool isEdit { get { return IsEditJudge || IsEditPos; } }

        static Judges()
        {
            posLst.Add("Главный судья");
            posLst.Add("Главный секретарь");
            posLst.Add("Зам. гл.судьи по виду");
            posLst.Add("Зам. гл.судьи по трассам");
            posLst.Add("Зам. гл.судьи по безопасности");
            posLst.Add("Врач соревнований");
            posLst.Add("Постановщик трасс");
            posLst.Add("Секретарь");
            posLst.Add("Судья на трассе");
            posLst.Add("Судья при участниках");
            posLst.Add("Страховщик");
            posLst.Add("Председатель мандатной комиссии");
            posLst.Add("Представитель проводящей организации");
            posLst.Add("Представитель ФСР");

            posToBadge.Add("Представитель ФСР");
            posToBadge.Add("Представитель проводящей организации");
            posToBadge.Add("Главный судья");
            posToBadge.Add("Зам. гл.судьи по виду");
            posToBadge.Add("Зам. гл.судьи по трассам");
            posToBadge.Add("Зам. гл.судьи по безопасности");
            posToBadge.Add("Главный секретарь");
            posToBadge.Add("Постановщик трасс");
            posToBadge.Add("Врач соревнований");
            posToBadge.Add("Секретарь");
        }

        public static void InitPosList(SqlConnection cn, SqlTransaction tran)
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = @"IF NOT EXISTS(SELECT * FROM positions(NOLOCK) WHERE name = @name)
                                    INSERT INTO positions(iid, name, forBadge) VALUES (@iid, @name, @prn)";
            cmd.Parameters.Add("@name", SqlDbType.VarChar, 255);
            cmd.Parameters.Add("@iid", SqlDbType.Int);
            cmd.Parameters.Add("@prn", SqlDbType.Bit);
            bool needToCommit;
            if (tran == null)
            {
                cmd.Transaction = cn.BeginTransaction();
                needToCommit = true;
            }
            else
            {
                cmd.Transaction = tran;
                needToCommit = false;
            }
            try
            {
                foreach (string str in posLst)
                {
                    cmd.Parameters[0].Value = str;
                    cmd.Parameters[1].Value = StaticClass.GetNextIID("positions", cn, "iid", cmd.Transaction);
                    cmd.Parameters[2].Value = (posToBadge.IndexOf(str) > -1);
                    cmd.ExecuteNonQuery();
                }
                if (needToCommit)
                    cmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                if (needToCommit)
                    try { cmd.Transaction.Rollback(); }
                    catch { }
                throw ex;
            }
        }

        public Judges(SqlConnection baseCon, string competitionTitle)
            : base(baseCon, competitionTitle)
        {
            InitializeComponent();

            try
            {
                SqlCommand cmd = new SqlCommand("SELECT ISNULL(remoteString,'') rs FROM CompetitionData(NOLOCK)", this.cn);
                string remS = cmd.ExecuteScalar().ToString();
                if (remS.Length > 0)
                    remote = new SqlConnection(remS);
            }
            catch { remote = null; }
            //btnLoadFRinet.Visible = btnLoadToInet.Visible = (remote != null);

            IsEditJudge = false;
            IsEditPos = false;

            
            //this.cn = cn;
            CheckTable(this.cn);
            da = new SqlDataAdapter(new SqlCommand("  SELECT iid, surname Фамилия, name Имя, patronimic Отчество, " +
                                                   "         city Город, category Категория " +
                                                   "    FROM judges(NOLOCK) " +
                                                   "ORDER BY surname, name, patronimic, city", cn));
            daPos = new SqlDataAdapter(new SqlCommand(@"
                  SELECT p.name Должность, jp.iid
                    FROM positions p(NOLOCK)
                    JOIN JudgePos jp(NOLOCK) ON jp.pos_id = p.iid
                   WHERE jp.judge_id = @jid
                ORDER BY p.name", this.cn));
            daPos.SelectCommand.Parameters.Add("@jid", SqlDbType.Int);
            dg.DataSource = dt;
            dgPos.DataSource = dtPos;
            try { SortingClass.CheckColumn("Judges", "photo", "image", this.cn); }
            catch { }
            try { SortingClass.CheckColumn("Judges", "changed", "BIT NOT NULL DEFAULT 1", this.cn); }
            catch { }
            try { SortingClass.CheckColumn("positions", "changed", "BIT NOT NULL DEFAULT 1", this.cn); }
            catch { }
            try { SortingClass.CheckColumn("JudgePos", "changed", "BIT NOT NULL DEFAULT 1", this.cn); }
            catch { }
#if !DEBUG
            btnLoadFRinet.Visible = btnLoadToInet.Visible = false;
#endif
            RefreshData();
        }

        public static void CheckTable(SqlConnection cn)
        {
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                SqlCommand cmd = new SqlCommand();
                SqlDataReader rdr;
                cmd.Connection = cn;
                cmd.CommandText = "SELECT COUNT(*) FROM sysobjects(NOLOCK) WHERE name = 'JudgePos' AND type = 'U'";
                if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                {
                    cmd.CommandText = @"ALTER VIEW judgeView AS
                 SELECT j.iid, j.name, j.surname, j.patronimic, city, category, ISNULL(p.name,'') pos
                        FROM judges j(NOLOCK)
                   LEFT JOIN JudgePos jp(NOLOCK) ON jp.judge_id = j.iid
                   LEFT JOIN positions p(NOLOCK) ON p.iid = jp.pos_id";
                    cmd.ExecuteNonQuery();
                    if(!SortingClass.CheckColumn("positions", "forBadge", "BIT NOT NULL DEFAULT 0", cn))
                        InitPosList(cn, null);
                    return;
                }
                cmd.Transaction = cn.BeginTransaction();
                try
                {

                    cmd.CommandText = @"
                    CREATE TABLE positions(
                      iid  INT PRIMARY KEY,
                      name VARCHAR(255) NOT NULL,
                  forBadge BIT NOT NULL DEFAULT 0
                    )
                    CREATE TABLE JudgePos(
                      iid BIGINT PRIMARY KEY,
                      judge_id INT NOT NULL FOREIGN KEY REFERENCES judges(iid),
                      pos_id INT NOT NULL FOREIGN KEY REFERENCES positions(iid),
                    )";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "SELECT DISTINCT pos FROM judges(NOLOCK) ORDER BY pos";
                    List<string> posList = new List<string>();
                    rdr = cmd.ExecuteReader();
                    try
                    {
                        while (rdr.Read())
                            posList.Add(rdr["pos"].ToString());
                    }
                    finally { rdr.Close(); }
                    int i = 0;
                    cmd.CommandText = "INSERT INTO positions (iid, name) VALUES (@id, @name)";
                    cmd.Parameters.Add("@id", SqlDbType.Int);
                    cmd.Parameters.Add("@name", SqlDbType.VarChar, 255);
                    foreach (string str in posList)
                    {
                        i++;
                        cmd.Parameters[0].Value = i;
                        cmd.Parameters[1].Value = str;
                        cmd.ExecuteNonQuery();
                    }
                    cmd.Parameters.Clear();
                    cmd.CommandText = @"
                    DECLARE @id BIGINT, @jid INT, @pid INT, @pos VARCHAR(255)
                    DECLARE crsT CURSOR FOR SELECT iid, pos FROM judges(NOLOCK)
                    SET @id = 0
                    OPEN crsT
                    FETCH NEXT FROM crsT INTO @jid, @pos
                    WHILE @@FETCH_STATUS = 0 BEGIN
                      SET @id = @id + 1
                      SELECT @pid = iid FROM positions(NOLOCK) WHERE name = @pos
                      INSERT INTO JudgePos (iid, judge_id, pos_id) VALUES(@id, @jid, @pid)
                      FETCH NEXT FROM crsT INTO @jid, @pos
                    END
                    CLOSE crsT
                    DEALLOCATE crsT";
                    cmd.ExecuteNonQuery();
                    //cmd.CommandText = "ALTER TABLE judges DROP COLUMN pos";
                    //cmd.ExecuteNonQuery();

                    cmd.CommandText = @"
                    CREATE VIEW judgeView AS
                      SELECT j.iid, j.name, j.surname, j.patronimic, city, category, ISNULL(p.name,'') pos
                        FROM judges j(NOLOCK)
                   LEFT JOIN JudgePos jp(NOLOCK) ON jp.judge_id = j.iid
                   LEFT JOIN positions p(NOLOCK) ON p.iid = jp.pos_id";
                    cmd.ExecuteNonQuery();
                    InitPosList(cn, cmd.Transaction);
                    cmd.Transaction.Commit();
                }
                catch (Exception ex)
                {
                    if (cmd.Transaction != null)
                        cmd.Transaction.Rollback();
                    throw ex;
                }
            }
            catch (Exception ex) { MessageBox.Show("Ошибка создания структуры\r\n" + ex.Message); }
        }

        void RefreshData()
        {
            dt.Clear();
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    try
                    {
                        int id;
                        id = Convert.ToInt32(dt.Rows[0]["iid"]);
                        if (IsEditJudge && lblIID.Text.Length > 0)
                            int.TryParse(lblIID.Text, out id);
                        daPos.SelectCommand.Parameters[0].Value = id;
                        dtPos.Clear();
                        daPos.Fill(dtPos);
                    }
                    catch { }

                }
            }
            catch (Exception ex) { MessageBox.Show("Ошибка загрузки данных:\r\n" + ex.Message); }
        }

        private void Judges_Load(object sender, EventArgs e)
        {

        }

        private void dg_SelectionChanged(object sender, EventArgs e)
        {
            if (isEdit)
                return;
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                lblIID.Text = dg.CurrentRow.Cells[0].Value.ToString();
                tbCity.Text = dg.CurrentRow.Cells[4].Value.ToString();
                tbSurname.Text = dg.CurrentRow.Cells[1].Value.ToString();
                tbName.Text = dg.CurrentRow.Cells[2].Value.ToString();
                tbPatronimic.Text = dg.CurrentRow.Cells[3].Value.ToString();
                tbCat.Text = dg.CurrentRow.Cells[5].Value.ToString();
                int jId;
                if (int.TryParse(lblIID.Text, out jId))
                {
                    daPos.SelectCommand.Parameters[0].Value = jId;
                    dtPos.Clear();
                    daPos.Fill(dtPos);
                    try { dgPos.Columns["iid"].Visible = false; }
                    catch { }

                    dsClimbingTableAdapters.judgeDataTableAdapter jta = new ClimbingCompetition.dsClimbingTableAdapters.judgeDataTableAdapter();
                    jta.Connection = cn;
                    foreach (dsClimbing.judgeDataRow row in jta.GetDataByIid(jId))
                    {
                        if (row.IsphotoNull())
                            pbPhoto.Image = null;
                        else
                            try { pbPhoto.Image = ImageWorker.GetFromBytes(row.photo); }
                            catch { pbPhoto.Image = null; }
                        break;
                    }
                }
                System.Threading.Thread thr = new System.Threading.Thread(cbPosFill);
                thr.Start();
            }
            catch
            {
                ClearData();
            }
        }

        private void ClearData()
        {
            lblIID.Text = tbCity.Text = tbName.Text = tbPatronimic.Text = tbPos.Text = tbSurname.Text = tbCat.Text = "";
            cbPos.SelectedIndex = -1;
            cbPos.Text = "Должность";
            dtPos.Clear();
            pbPhoto.Image = null;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (isEdit)
                return;
            ClearData();
            IsEditJudge = true;
            IsNewJudge = true;
        }

        private void btnEditSubmit_Click(object sender, EventArgs e)
        {
            if (!IsEditJudge)
            {
                if (lblIID.Text.Length < 1)
                    return;
            }
            else
            {
                if (tbSurname.Text == "")
                {
                    MessageBox.Show("Введите фамилию");
                    return;
                }
                //if (cbPos.SelectedIndex < 0 || (cbPos.SelectedItem.ToString() == "Другое:" && tbPos.Text == ""))
                //{
                //    MessageBox.Show("Введите должность");
                //    return;
                //}
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.Transaction = cn.BeginTransaction();
                try
                {
                    int iid;
                    bool isNew;
                    try
                    {
                        iid = Convert.ToInt32(lblIID.Text);
                        isNew = false;
                    }
                    catch
                    {
                        iid = (int)StaticClass.GetNextIID("judges", cn, "iid", cmd.Transaction);
                        isNew = true;
                    }
                    if (isNew)
                        cmd.CommandText = "INSERT INTO judges (iid, surname, name, patronimic, city, category,photo) " +
                            "VALUES (" + iid.ToString() + ", @surname, @name, @patronimic, @city, @cat, @photo)";
                    else
                        cmd.CommandText = "UPDATE judges SET surname = @surname, name = @name, patronimic = " +
                            "@patronimic, city = @city, category = @cat, photo=@photo WHERE iid = " + iid.ToString();
                    cmd.Parameters.Add("@surname", SqlDbType.VarChar);
                    cmd.Parameters.Add("@name", SqlDbType.VarChar);
                    cmd.Parameters.Add("@patronimic", SqlDbType.VarChar);
                    cmd.Parameters.Add("@city", SqlDbType.VarChar);
                    cmd.Parameters.Add("@cat", SqlDbType.VarChar);
                    cmd.Parameters.Add("@photo", SqlDbType.Image);


                    //string pos;
                    //if (cbPos.SelectedItem.ToString() == "Другое:")
                    //    pos = tbPos.Text;
                    //else
                    //    pos = cbPos.SelectedItem.ToString();

                    cmd.Parameters[0].Value = tbSurname.Text;
                    cmd.Parameters[1].Value = tbName.Text;
                    cmd.Parameters[2].Value = tbPatronimic.Text;
                    cmd.Parameters[3].Value = tbCity.Text;
                    cmd.Parameters[4].Value = tbCat.Text;
                    if (pbPhoto.Image == null)
                        cmd.Parameters[5].Value = DBNull.Value;
                    else
                        try { cmd.Parameters[5].Value = ImageWorker.GetBytesFromImage(pbPhoto.Image); }
                        catch (Exception exi)
                        {
                            cmd.Parameters[5].Value = DBNull.Value;
                            MessageBox.Show("Ошибка сохранения фото:\r\n" + exi.Message);
                        }

                    cmd.ExecuteNonQuery();

                    if (!AddFirstPos(cmd, iid))
                    {
                        cmd.Transaction.Rollback();
                        return;
                    }

                    cmd.Transaction.Commit();
                    try { RefreshData(); }
                    catch { }
                    IsNewJudge = false;
                }
                catch (Exception ex)
                {
                    if (cmd.Transaction != null)
                        try { cmd.Transaction.Rollback(); }
                        catch { }
                    if (MessageBox.Show("Ошибка добавления судьи:\r\n" + ex.Message +
                        "\r\nПродолжить правку?",
                        "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        return;
                }
            }
            if (IsEditJudge = !IsEditJudge)
                IsNewJudge = false;

        }

        private void cbPos_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (IsEditPos)
                    tbPos.Enabled = cbPos.Text == "Другое:";
            }
            catch { }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (IsEditJudge)
            {
                IsEditJudge = false;
                return;
            }
            int iid;
            if (lblIID.Text == "")
            {
                MessageBox.Show("Судья не выбран");
                return;
            }
            try
            {
                iid = Convert.ToInt32(lblIID.Text);
                if (MessageBox.Show(
                    "Вы уверены, что хотите удалить судью " + tbSurname.Text + " " + tbName.Text + " " + tbPatronimic.Text + "?",
                    "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.Transaction = cn.BeginTransaction();

                try
                {
                    cmd.CommandText = "UPDATE lists SET judge_id = NULL WHERE judge_id = " + iid.ToString();
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE lists SET routesetter_id = NULL WHERE routesetter_id = " + iid.ToString();
                    cmd.ExecuteNonQuery();
                    List<int> jPos = new List<int>();
                    cmd.CommandText = "SELECT pos_id FROM JudgePos(NOLOCK) WHERE judge_id = " + iid.ToString();
                    SqlDataReader dr = cmd.ExecuteReader();
                    try
                    {
                        while (dr.Read())
                            jPos.Add(Convert.ToInt32(dr["pos_id"]));
                    }
                    finally { dr.Close(); }
                    cmd.CommandText = "DELETE FROM JudgePos WHERE judge_id = " + iid.ToString();
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "DELETE FROM judges WHERE iid = " + iid.ToString();
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "SELECT COUNT(*) FROM JudgePos(NOLOCK) WHERE pos_id=@pid";
                    cmd.Parameters.Add("@pid", SqlDbType.Int);
                    for (int i = 0; i < jPos.Count; i++)
                    {
                        cmd.Parameters[0].Value = jPos[i];
                        int nTmp = Convert.ToInt32(cmd.ExecuteScalar());
                        if (nTmp > 0)
                        {
                            jPos.RemoveAt(i);
                            i--;
                        }
                    }
                    if (jPos.Count > 0)
                    {
                        cmd.CommandText = "DELETE FROM positions WHERE iid=@pid";
                        foreach (int pid in jPos)
                        {
                            cmd.Parameters[0].Value = pid;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    cmd.Transaction.Commit();
                }
                catch (Exception ex)
                {
                    try { cmd.Transaction.Rollback(); }
                    catch { }
                    throw ex;
                }
            }
            catch (Exception ex) { MessageBox.Show("Удаление не удалось.\r\n" + ex.Message); }
            try { RefreshData(); }
            catch { }
        }

        private void cbPosFill()
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                cmd.CommandText = "SELECT name FROM positions(NOLOCK) ";
                if (posLst.Count > 0)
                {
                    cmd.CommandText += " WHERE name NOT IN(";
                    int i = 0;
                    foreach (string str in posLst)
                    {
                        i++;
                        cmd.CommandText += "@p" + i.ToString() + ",";
                        cmd.Parameters.Add("@p" + i.ToString(), SqlDbType.VarChar, 255);
                        cmd.Parameters[cmd.Parameters.Count - 1].Value = str;
                    }
                    cmd.CommandText = cmd.CommandText.Substring(0, cmd.CommandText.Length - 1) + ") ";
                }
                cmd.CommandText += " ORDER BY name";
                cbPos.Invoke(new EventHandler(delegate
                {
                    try
                    {
                        cbPos.Items.Clear();
                        foreach (string str in posLst)
                            cbPos.Items.Add(str);
                        SqlDataReader rdr = cmd.ExecuteReader();
                        try
                        {
                            while (rdr.Read())
                                cbPos.Items.Add(rdr["name"].ToString());
                        }
                        finally { rdr.Close(); }
                        cbPos.Items.Add("Другое:");
                    }
                    catch { }
                }));
            }
            catch { }
        }

        private void cbPos_DropDown(object sender, EventArgs e)
        {
            System.Threading.Thread thr = new System.Threading.Thread(cbPosFill);
            thr.Start();
        }

        private void dgPos_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (IsEditPos)
                    return;
                string pos = dgPos.CurrentRow.Cells[0].Value.ToString();
                if (cbPos.Items.IndexOf(pos) > -1)
                {
                    cbPos.SelectedIndex = cbPos.Items.IndexOf(pos);
                    tbPos.Text = "";
                }
                else
                {
                    cbPos.SelectedIndex = cbPos.Items.Count - 1;
                    tbPos.Text = pos;
                }
                try { lblPosID.Text = dgPos.CurrentRow.Cells["iid"].Value.ToString(); }
                catch { lblPosID.Text = ""; }
            }
            catch { }
        }

        private void btnAddPos_Click(object sender, EventArgs e)
        {
            IsEditPos = true;
            cbPos.Text = "Выберите должность";
            lblPosID.Text = "";
        }

        private void btnEditPos_Click(object sender, EventArgs e)
        {
            if (!IsEditPos)
            {
                IsEditPos = true;
                return;
            }
            ModifyPos();
        }

        private bool ModifyPos()
        {
            string pos;
            if (cbPos.Text.ToLower().IndexOf("выберите") == 0 || cbPos.Text.ToLower().IndexOf("должность") == 0)
                pos = "";
            else if (cbPos.Text.ToLower().IndexOf("другое") == 0)
                pos = tbPos.Text;
            else
                pos = cbPos.Text;
            if (pos.Length < 1)
            {
                MessageBox.Show("Должность не выбрана");
                return false;
            }
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Transaction = cn.BeginTransaction();
                try
                {
                    cmd.Connection = cn;
                    cmd.Parameters.Add("@p", SqlDbType.VarChar, 255);
                    cmd.Parameters[0].Value = pos;
                    cmd.CommandText = "SELECT iid FROM positions(NOLOCK) WHERE name=@p";
                    int posID;
                    object o = cmd.ExecuteScalar();
                    if (o == DBNull.Value || o == null)
                    {
                        posID = (int)StaticClass.GetNextIID("positions", cn, "iid", cmd.Transaction);
                        cmd.CommandText = "INSERT INTO positions(iid, name) VALUES (" +
                            posID.ToString() + ", @p)";
                        cmd.ExecuteNonQuery();
                    }
                    else
                        posID = Convert.ToInt32(o);
                    cmd.CommandText = "SELECT COUNT(*) FROM JudgePos(NOLOCK) WHERE judge_id = " + lblIID.Text + " AND pos_id = " + posID.ToString();
                    if (Convert.ToInt32(cmd.ExecuteScalar()) < 1)
                    {
                        if (lblPosID.Text.Length < 1)
                        {
                            long jpID = StaticClass.GetNextIID("JudgePos", cn, "iid", cmd.Transaction);
                            cmd.CommandText = "INSERT INTO JudgePos(iid, judge_id, pos_id) VALUES (" +
                                jpID.ToString() + ", " + lblIID.Text + ", " + posID.ToString() + ")";
                        }
                        else
                            cmd.CommandText = "UPDATE JudgePos SET pos_id=" + posID.ToString() + " WHERE iid=" + lblPosID.Text;
                        cmd.ExecuteNonQuery();
                    }
                    cmd.Transaction.Commit();
                    IsEditPos = false;
                    try { RefreshData(); }
                    catch { }
                    return true;
                }
                catch (Exception ex)
                {
                    try
                    {
                        if (cmd.Transaction != null)
                            cmd.Transaction.Rollback();
                    }
                    catch { }
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка добавления/изменения должности\r\n" + ex.Message);
            }
            return false;
        }

        private void btnDelPos_Click(object sender, EventArgs e)
        {
            if (IsEditPos)
            {
                IsEditPos = false;
                RefreshData();
                return;
            }
            if (lblPosID.Text.Length < 1)
            {
                MessageBox.Show("Выберите должность для удаления");
                return;
            }
            try
            {
                if (MessageBox.Show("Вы действительно хотите удалить должность '" + cbPos.Text + "'?",
                    "Удалить должность", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    return;
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.Transaction = cn.BeginTransaction();
                try
                {
                    cmd.CommandText = "SELECT pos_id FROM JudgePos j(NOLOCK) WHERE j.iid = " + lblPosID.Text;
                    int posID = Convert.ToInt32(cmd.ExecuteScalar());

                    cmd.CommandText = "DELETE FROM JudgePos WHERE iid=" + lblPosID.Text;
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "SELECT COUNT(*) FROM JudgePos WHERE pos_id = " + posID.ToString();
                    int cnt = Convert.ToInt32(cmd.ExecuteScalar());
                    if (cnt < 1)
                    {
                        cmd.CommandText = "DELETE FROM positions WHERE iid = " + posID.ToString();
                        cmd.ExecuteNonQuery();
                    }
                    cmd.Transaction.Commit();
                }
                catch (Exception ex)
                {
                    if (cmd.Transaction != null)
                        try { cmd.Transaction.Rollback(); }
                        catch { }
                    throw ex;
                }
                RefreshData();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка удаления должности\r\n" + ex.Message);
            }
        }

        private void JudgeList()
        {
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                SqlDataAdapter daR = new SqlDataAdapter();
                daR.SelectCommand = new SqlCommand();
                daR.SelectCommand.Connection = cn;
                daR.SelectCommand.CommandText = @"
   SELECT iid, surname+
          CASE WHEN name <> '' THEN ' ' + name ELSE '' END+
          CASE WHEN patronimic <> '' THEN ' ' + patronimic ELSE '' END [Фамилия,Имя],
          city Город, category Категория, pos Должность
     FROM judgeView(NOLOCK)
 ORDER BY surname, name, patronimic, city, category, pos, iid";
                DataTable dtR = new DataTable();
                daR.Fill(dtR);

                Excel.Workbook wb;
                Excel.Worksheet ws;
                Excel.Application xlApp;
                if (!StaticClass.LaunchExcel(out xlApp, out wb, out ws, false, cn))
                    return;
                try
                {
                    int jPos = 0, jStartRow = 0;
                    int startRow = 3;
                    for (int i = 1; i < dtR.Columns.Count; i++)
                        ws.Cells[startRow, i + 1] = dtR.Columns[i].ColumnName;
                    ws.Cells[startRow, 1] = "№ п/п";
                    int currentRow = startRow;
                    int curJudge = -1;
                    foreach (DataRow dr in dtR.Rows)
                    {
                        currentRow++;
                        if (curJudge != Convert.ToInt32(dr["iid"]))
                        {
                            curJudge = Convert.ToInt32(dr["iid"]);
                            if (jStartRow > 0)
                                for (int i = 1; i <= 4; i++)
                                    ws.get_Range(ws.Cells[jStartRow, i], ws.Cells[currentRow - 1, i]).Merge(Type.Missing);
                            jPos++;
                            jStartRow = currentRow;
                            ws.Cells[currentRow, 1] = jPos;
                            ws.Cells[currentRow, 2] = dr["Фамилия,Имя"].ToString();
                            ws.Cells[currentRow, 3] = dr["Город"].ToString();
                            ws.Cells[currentRow, 4] = dr["Категория"].ToString();
                        }
                        ws.Cells[currentRow, 5] = dr["Должность"].ToString();
                    }
                    if (jStartRow > 0)
                        for (int i = 1; i <= 4; i++)
                            ws.get_Range(ws.Cells[jStartRow, i], ws.Cells[currentRow, i]).Merge(Type.Missing);
                    Excel.Range r;
                    r = ws.get_Range(ws.Cells[startRow, 1], ws.Cells[currentRow, 5]);
                    r.Style = "StyleLA";
                    r = ws.get_Range(ws.Cells[startRow, 1], ws.Cells[startRow, 5]);
                    r.Font.Bold = true;
                    r = ws.get_Range(ws.Cells[startRow, 1], ws.Cells[currentRow, 5]);
                    r.Columns.AutoFit();
                    r = ws.get_Range(ws.Cells[1, 1], ws.Cells[1, 5]);
                    r.Style = "CompTitle";
                    ws.Cells[1, 1] = StaticClass.ParseCompTitle(competitionTitle)[0];
                    r.Merge(Type.Missing);
                    r.Rows.AutoFit();
                    r = ws.get_Range(ws.Cells[2, 1], ws.Cells[2, 5]);
                    r.Style = "Title";
                    r.Merge(Type.Missing);
                    ws.Cells[2, 1] = "Список судей";
                    try { ws.Name = "Список судей"; }
                    catch { }
                }
                finally { StaticClass.SetExcelVisible(xlApp); }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка экспорта списка судей\r\n" + ex.Message);
            }
        }

        private void JudgeListPos()
        {
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                SqlDataAdapter daR = new SqlDataAdapter();
                daR.SelectCommand = new SqlCommand();
                daR.SelectCommand.Connection = cn;
                daR.SelectCommand.CommandText = @"
                      SELECT j.iid, ISNULL(p.name,'') Должность, j.surname +
                             CASE WHEN j.name <> '' THEN ' ' + j.name ELSE '' END+
                             CASE WHEN j.patronimic <> '' THEN ' ' + j.patronimic ELSE '' END [Фамилия,Имя],
                             j.city Город, j.category Категория
                        FROM judges j(NOLOCK)
                   LEFT JOIN JudgePos jp(NOLOCK) ON jp.judge_id = j.iid
                   LEFT JOIN positions p(NOLOCK) ON p.iid = jp.pos_id
                    ORDER BY p.iid, j.surname, j.name, j.patronimic, j.city, j.category, j.iid";

                DataTable dtR = new DataTable();
                daR.Fill(dtR);

                Excel.Workbook wb;
                Excel.Worksheet ws;
                Excel.Application xlApp;
                if (!StaticClass.LaunchExcel(out xlApp, out wb, out ws, false, cn))
                    return;
                try
                {
                    int startRow = 3;
                    for (int i = 1; i < dtR.Columns.Count; i++)
                        ws.Cells[startRow, i + 1] = dtR.Columns[i].ColumnName;
                    ws.Cells[startRow, 1] = "№ п/п";
                    int currentRow = startRow;
                    foreach (DataRow dr in dtR.Rows)
                    {
                        currentRow++;
                        ws.Cells[currentRow, 1] = currentRow - startRow;
                        ws.Cells[currentRow, 2] = dr["Должность"].ToString();
                        ws.Cells[currentRow, 3] = dr["Фамилия,Имя"].ToString();
                        ws.Cells[currentRow, 4] = dr["Город"].ToString();
                        ws.Cells[currentRow, 5] = dr["Категория"].ToString();
                    }
                    Excel.Range r;
                    r = ws.get_Range(ws.Cells[startRow, 1], ws.Cells[currentRow, 5]);
                    r.Style = "StyleLA";
                    r = ws.get_Range(ws.Cells[startRow, 1], ws.Cells[startRow, 5]);
                    r.Font.Bold = true;
                    r = ws.get_Range(ws.Cells[startRow, 1], ws.Cells[currentRow, 5]);
                    r.Columns.AutoFit();
                    r = ws.get_Range(ws.Cells[1, 1], ws.Cells[1, 5]);
                    r.Style = "CompTitle";
                    ws.Cells[1, 1] = StaticClass.ParseCompTitle(competitionTitle)[0];
                    r.Merge(Type.Missing);
                    r.Rows.AutoFit();
                    r = ws.get_Range(ws.Cells[2, 1], ws.Cells[2, 5]);
                    r.Style = "Title";
                    r.Merge(Type.Missing);
                    ws.Cells[2, 1] = "Список судей";
                    try { ws.Name = "Список судей по должностям"; }
                    catch { }
                }
                finally { StaticClass.SetExcelVisible(xlApp); }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка экспорта списка судей\r\n" + ex.Message);
            }
        }

        private void btnExcelExport_Click(object sender, EventArgs e)
        {
            if (rbGroupJudge.Checked)
                JudgeList();
            else
                JudgeListPos();
        }


        private bool isNewJudge = false;
        protected bool IsNewJudge
        {
            get { return (isEdit && isNewJudge); }
            set
            {
                if (isEdit)
                {
                    cbPos.Enabled = value;
                    if (value)
                        cbPos.Text = "Выберите должность";
                    else
                        tbPos.Enabled = false;
                }
                isNewJudge = value;
            }
        }

        bool AddFirstPos(System.Data.SqlClient.SqlCommand cmd, int judgeID)
        {
            if (!IsNewJudge)
                return true;
            if (cbPos.Text.Length < 1 ||
                (cbPos.Text.ToLower().IndexOf("другое") == 0 && tbPos.Text.Length < 1) ||
                cbPos.Text.ToLower().IndexOf("выберите") == 0)
            {
                if (MessageBox.Show("Должность не введена. Продолжить?", "", MessageBoxButtons.YesNo)
                    == DialogResult.No)
                    return false;
                else
                    return true;
            }
            string pos;
            if (cbPos.Text.ToLower().IndexOf("другое") == 0)
                pos = tbPos.Text;
            else
                pos = cbPos.Text;
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@pos", System.Data.SqlDbType.VarChar, 255);
            cmd.Parameters[0].Value = pos;
            cmd.CommandText = "SELECT iid FROM positions(NOLOCK) WHERE name=@pos";
            object oT = cmd.ExecuteScalar();
            int posID;
            if (oT == DBNull.Value || oT == null)
            {
                posID = (int)StaticClass.GetNextIID("positions", cn, "iid", cmd.Transaction);
                cmd.CommandText = "INSERT INTO positions (iid,name) VALUES (" + posID.ToString() + ",@pos)";
                cmd.ExecuteNonQuery();
            }
            else
                posID = Convert.ToInt32(oT);
            long jpID = StaticClass.GetNextIID("JudgePos", cn, "iid", cmd.Transaction);
            cmd.CommandText = "INSERT INTO JudgePos(iid, judge_id, pos_id) VALUES (" +
                jpID.ToString() + ", " + judgeID.ToString() + ", " + posID.ToString() + ")";
            cmd.ExecuteNonQuery();
            return true;
        }

        private const string PhotoFileName = "BadgeRep.jpg";

        private void btnPrintBadges_Click(object sender, EventArgs e)
        {
            if (sender == btnPrintOneB && lblIID.Text.Length < 1)
                return;

            pictMut.WaitOne();
            try
            {
                appSettings aSet = appSettings.Default;
                string left = aSet.BadgeLeft, right = aSet.BadgeRight;
                do
                {
                    if (left.Length == 0)
                    {
                        if ((left = SourceSelect.GetLogo("СЛЕВА")) == SourceSelect.CANCEL)
                            return;
                    }
                    if (right.Length == 0)
                    {
                        if ((right = SourceSelect.GetLogo("СПРАВА")) == SourceSelect.CANCEL)
                            return;
                    }
                    switch (MessageBox.Show(this, "Проверьте логотипы:\r\nСЛЕВА: " + left.ToString() +
                        "\r\nСПРАВА: " + right.ToString(), "Проверьте логотипы", MessageBoxButtons.YesNoCancel,
                         MessageBoxIcon.Information))
                    {
                        case DialogResult.Yes:
                            aSet.BadgeLeft = left;
                            aSet.BadgeRight = right;
                            aSet.Save();
                            break;
                        case DialogResult.No:
                            left = right = "";
                            break;
                        case DialogResult.Cancel:
                            return;
                    }
                } while (left.Length * right.Length == 0);
                pictLeft = left;
                pictRight = right;

                if (cn.State != ConnectionState.Open)
                    cn.Open();
                dsClimbingTableAdapters.judgeDataTableAdapter jta = new ClimbingCompetition.dsClimbingTableAdapters.judgeDataTableAdapter();
                jta.Connection = cn;
                if (sender == btnPrintBadges)
                    dataToPrint = jta.GetDataForBadges();
                else
                {
                    int jid;
                    if (!int.TryParse(lblIID.Text, out jid))
                        return;
                    dataToPrint = jta.GetDataForBadgesByIid(jid);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка создание отчета:\r\n" + ex.Message);
                return;
            }
            finally { pictMut.ReleaseMutex(); }
            try { (new WordReportCreationBar(CreateBadges)).ShowDialog(); }
            catch (Exception ex) { MessageBox.Show("Ошибка создание отчета:\r\n" + ex.Message); }
        }

        private enum DirectionMove { Left, Right, Up, Down };

        private static Word.Selection MoveSelectionHome(Word.Selection s)
        {
            object xnt = Word.WdMovementType.wdMove, unit = Word.WdUnits.wdStory;
            s.HomeKey(ref unit, ref xnt);
            return s;
        }

        private static Word.Selection MoveSelectionToEnd(Word.Selection s, bool extend)
        {
            object xnt, unit = Word.WdUnits.wdStory;
            if (extend)
                xnt = Word.WdMovementType.wdExtend;
            else
                xnt = Word.WdMovementType.wdMove;
            s.EndKey(ref unit, ref xnt);
            return s;
        }

        string pictLeft = SourceSelect.NOLOGO, pictRight = SourceSelect.NOLOGO;
        System.Threading.Mutex pictMut = new System.Threading.Mutex();
        dsClimbing.judgeDataDataTable dataToPrint = new dsClimbing.judgeDataDataTable();

        private static Word.Selection MoveSelection(Word.Selection s, DirectionMove dir, Word.WdUnits unit, int count, bool extend)
        {
            object objExtend;
            if (extend)
                objExtend = Word.WdMovementType.wdExtend;
            else
                objExtend = Word.WdMovementType.wdMove;
            object wdUnit = unit, wdCount = count;
            if (unit == Word.WdUnits.wdStory)
            {
                s.HomeKey(ref wdUnit, ref objExtend);
                return s;
            }
            switch (dir)
            {
                case DirectionMove.Up:
                    s.MoveUp(ref wdUnit, ref wdCount, ref objExtend);
                    break;
                case DirectionMove.Down:
                    s.MoveDown(ref wdUnit, ref wdCount, ref objExtend);
                    break;
                case DirectionMove.Left:
                    s.MoveLeft(ref wdUnit, ref wdCount, ref objExtend);
                    break;
                case DirectionMove.Right:
                    s.MoveRight(ref wdUnit, ref wdCount, ref objExtend);
                    break;
            }
            return s;
        }

        private static Word.Selection AddPicture(Word.Selection s, string fileName)
        {
            object saveWithFile = true, linkToFile = false, range = Type.Missing;
            Word.InlineShape ish = s.InlineShapes.AddPicture(fileName, ref linkToFile, ref saveWithFile, ref range);
            ish.LockAspectRatio = Microsoft.Office.Core.MsoTriState.msoTrue;
            if (ish.Height > 49.05f)
                ish.Height = 49.05f;
            return s;
        }

        private static Word.Selection DeleteUnit(Word.Selection s, Word.WdUnits unit, int count)
        {
            object oUnit = unit, oCount = count;
            s.Delete(ref oUnit, ref oCount);
            return s;
        }

        Word.Application CreateBadges(BackgroundWorker bw)
        {
            pictMut.WaitOne();
            try
            {

                Word.Document doc;
                Word.Application app = null;
                string dir = System.Threading.Thread.GetDomain().BaseDirectory;
                string photoFile = dir + PhotoFileName;
                if (StaticClass.LaunchWord(dir + "Templates\\badges.dot", out doc, out app))
                {
#if DEBUG
                    app.Visible = true;
#endif
                    try
                    {
                        bw.ReportProgress(15);
                        Word.Selection s = app.Selection;
                        s = MoveSelectionHome(s);
                        s = MoveSelection(s, DirectionMove.Down, Word.WdUnits.wdLine, 2, false);
                        s = MoveSelection(s, DirectionMove.Left, Word.WdUnits.wdCharacter, 1, false);
                        if (pictLeft != SourceSelect.NOLOGO)
                        {
                            if (pictLeft == SourceSelect.PHOTO)
                                s.TypeText("$[PHOTO]");
                            else
                            {
                                s = AddPicture(s, pictLeft);
                                s = MoveSelection(s, DirectionMove.Left, Word.WdUnits.wdCharacter, 1, true);
                                s.Copy();
                            }
                        }
                        s = MoveSelection(s, DirectionMove.Right, Word.WdUnits.wdCell, 3, false);
                        if (pictLeft != SourceSelect.NOLOGO)
                        {
                            if (pictLeft == SourceSelect.PHOTO)
                                s.TypeText("$[PHOTO]");
                            else
                                s.PasteAndFormat(Microsoft.Office.Interop.Word.WdRecoveryType.wdPasteDefault);
                        }
                        s = MoveSelection(s, DirectionMove.Left, Word.WdUnits.wdCell, 1, false);
                        if (pictRight != SourceSelect.NOLOGO)
                        {
                            if (pictRight == SourceSelect.PHOTO)
                                s.TypeText("$[PHOTO]");
                            else
                            {
                                s = AddPicture(s, pictRight);
                                s = MoveSelection(s, DirectionMove.Left, Word.WdUnits.wdCharacter, 1, true);
                                s.Copy();
                            }
                            s = MoveSelection(s, DirectionMove.Right, Word.WdUnits.wdCell, 3, false);
                            if (pictRight == SourceSelect.PHOTO)
                                s.TypeText("$[PHOTO]");
                            else
                                s.PasteAndFormat(Microsoft.Office.Interop.Word.WdRecoveryType.wdPasteDefault);
                        }

                        s = MoveSelectionHome(s);

                        WordReportCreationBar.ReplaceLabel(doc, app, "COMP_TITLE", StaticClass.ParseCompTitle(competitionTitle)[0], true);
                        WordReportCreationBar.ReplaceLabel(doc, app, "DATE", StaticClass.ParseCompTitle(competitionTitle)[2], true);
                        WordReportCreationBar.ReplaceLabel(doc, app, "PLACE", StaticClass.ParseCompTitle(competitionTitle)[1], true);

                        s = MoveSelectionHome(s);
                        s = MoveSelectionToEnd(s, true);
                        s.Copy();
                        dsClimbing.judgeDataRow row;
                        if (dataToPrint.Rows.Count > 0)
                        {
                            row = (dsClimbing.judgeDataRow)dataToPrint.Rows[0];
                            WordReportCreationBar.ReplaceLabel(doc, app, "NAME1", row.full_name, true);
                            WordReportCreationBar.ReplaceLabel(doc, app, "POS1", row.pos, true);
                            s = AddJudgePicture(photoFile, s, row);
                        }
                        if (dataToPrint.Rows.Count > 1)
                        {
                            row = (dsClimbing.judgeDataRow)dataToPrint.Rows[1];
                            WordReportCreationBar.ReplaceLabel(doc, app, "NAME2", row.full_name, true);
                            WordReportCreationBar.ReplaceLabel(doc, app, "POS2", row.pos, true);
                            s = AddJudgePicture(photoFile, s, row);
                        }
                        int cur = 25;
                        bw.ReportProgress(cur);
                        int step = (100 - cur) / dataToPrint.Rows.Count;

                        for (int i = 2; i < dataToPrint.Rows.Count; i++)
                        {
                            row = (dsClimbing.judgeDataRow)dataToPrint.Rows[i];
                            if (i % 2 == 0)
                            {
                                s = MoveSelectionToEnd(s, false);
                                //s.TypeParagraph();
                                s.PasteAndFormat(Word.WdRecoveryType.wdPasteDefault);
                                s = MoveSelection(s, DirectionMove.Up, Word.WdUnits.wdLine, 1, false);
                                s = DeleteUnit(s, Word.WdUnits.wdCharacter, 1);
                                s = MoveSelectionToEnd(s, false);
                                s = MoveSelection(s, DirectionMove.Up, Word.WdUnits.wdParagraph, 16, false);
                                //s = DeleteUnit(s, Word.WdUnits.wdCharacter, 1);
                                WordReportCreationBar.ReplaceLabel(doc, app, "NAME1", row.full_name, true);
                                WordReportCreationBar.ReplaceLabel(doc, app, "POS1", row.pos, true);
                                s = AddJudgePicture(photoFile, s, row);
                            }
                            else
                            {
                                WordReportCreationBar.ReplaceLabel(doc, app, "NAME2", row.full_name, true);
                                WordReportCreationBar.ReplaceLabel(doc, app, "POS2", row.pos, true);
                                s = AddJudgePicture(photoFile, s, row);
                            }
                            cur += step;
                            bw.ReportProgress(cur);
                        }
                        WordReportCreationBar.ReplaceLabel(doc, app, "NAME1", "", true);
                        WordReportCreationBar.ReplaceLabel(doc, app, "POS1", "", true);
                        WordReportCreationBar.ReplaceLabel(doc, app, "NAME2", "", true);
                        WordReportCreationBar.ReplaceLabel(doc, app, "POS2", "", true);
                        WordReportCreationBar.ReplaceLabel(doc, app, "PHOTO", "", true);
                        s = MoveSelectionHome(s);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка загрузки данных:\r\n" + ex.Message);
                    }
                }
                try
                {
                    if (File.Exists(photoFile))
                        File.Delete(photoFile);
                }
                catch { }
                bw.ReportProgress(100);

                return app;
            }
            finally { pictMut.ReleaseMutex(); }
        }

        private Word.Selection DeleteSelected(Word.Selection s)
        {
            object unit = Word.WdUnits.wdCharacter;
            object count = 1;
            s.Delete(ref unit, ref count);
            return s;
        }

        private Word.Selection AddJudgePicture(string photoFile, Word.Selection s, dsClimbing.judgeDataRow row)
        {
            try
            {
                if (pictLeft == SourceSelect.PHOTO || pictRight == SourceSelect.PHOTO)
                {
                    if (File.Exists(photoFile))
                        File.Delete(photoFile);
                    if (!row.IsphotoNull())
                    {
                        Image img = ImageWorker.GetFromBytes(row.photo);
                        Bitmap bmp = new Bitmap(img);
                        bmp.Save(photoFile, ImageFormat.Jpeg);
                    }
                    if (pictLeft == SourceSelect.PHOTO)
                    {
                        s = WordReportCreationBar.MoveToLabel(s, "PHOTO");
                        s = DeleteSelected(s);
                        if (!row.IsphotoNull())
                            s = AddPicture(s, photoFile);
                        //if (row.IsphotoNull())
                        //    s.TypeText(" ");
                        //else
                        //    s = AddPicture(s, photoFile);
                    }
                    if (pictRight == SourceSelect.PHOTO)
                    {
                        s = WordReportCreationBar.MoveToLabel(s, "PHOTO");
                        s = DeleteSelected(s);
                        if (!row.IsphotoNull())
                            s = AddPicture(s, photoFile);
                    }
                }
            }
            catch { }
            return s;
        }

        private void btnDelPhoto_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Вы уверены, что хотите удалить фото у судьи " + tbSurname.Text + " " + tbName.Text + "?",
                "Удалить фото", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;
            pbPhoto.Image = null;
        }

        private void btnLoadPhoto_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Выберите фото";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.Cancel)
                return;
            try
            {
                Image img = Image.FromFile(ofd.FileName);
                pbPhoto.Image = img;
            }
            catch (Exception ex) { MessageBox.Show(this, "Ошибка загрузки фото:\r\n" + ex.Message); }
        }

        private void btnLoadToInet_Click(object sender, EventArgs e)
        {
            if (remote == null)
                return;
            if (thrLoad != null && thrLoad.IsAlive)
            {
                if (MessageBox.Show(this, "Загрузка списка судей уже запущена. Прервать?", "Прервать загрузку?", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    == DialogResult.No)
                    return;
                else
                    thrLoad.Abort();
            }
            bool loadPhoto = (MessageBox.Show(this, "Загрузить фото?", "Загрузить фото?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                 DialogResult.Yes);
            thrLoad = new Thread(LoadToInet);
            thrLoad.Start(new LoadArg(cn.ConnectionString, remote.ConnectionString, loadPhoto));
        }

        private class LoadArg
        {
            public LoadArg(string local, string remote, bool loadPhoto)
            {
                this.local = local;
                this.remote = remote;
                this.loadPhoto = loadPhoto;
            }
            public string local { get; private set; }
            public string remote { get; private set; }
            public bool loadPhoto { get; private set; }
        }

        private static void LoadToInet(object obj)
        {
            SqlConnection local = null, remote = null;
            try
            {
                if (!(obj is LoadArg))
                    return;
                LoadArg la = (LoadArg)obj;
                local = new SqlConnection(la.local);
                local.Open();
                remote = new SqlConnection(la.remote);
                //StaticClass.RefreshJudgesList(true, true, la.loadPhoto, local, remote);
            }
            catch (ThreadAbortException) { }
            catch (Exception ex) { MessageBox.Show("Ошибка загрузки списка судей:\r\n" + ex.Message); }
            finally
            {
                try
                {
                    if (local != null && local.State != ConnectionState.Closed)
                        local.Close();
                }
                catch { }
                try
                {
                    if (remote != null && remote.State != ConnectionState.Closed)
                        remote.Close();
                }
                catch { }
            }
        }

        private void LoadFromInet()
        {
            SqlTransaction tran = null;
            try
            {
                if (remote == null)
                    return;
                if (remote.State != ConnectionState.Open)
                    remote.Open();
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                tran = cn.BeginTransaction();
                List<int> insJudges = new List<int>(), insPos = new List<int>(), insJPos = new List<int>();

                dsClimbingTableAdapters.ONLjudgesTableAdapter jta = new ClimbingCompetition.dsClimbingTableAdapters.ONLjudgesTableAdapter();
                jta.Connection = remote;
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.Parameters.Add("@iid", SqlDbType.Int);
                cmd.Parameters.Add("@surname", SqlDbType.VarChar, 255);
                cmd.Parameters.Add("@name", SqlDbType.VarChar, 255);
                cmd.Parameters.Add("@patronimic", SqlDbType.VarChar, 255);
                cmd.Parameters.Add("@city", SqlDbType.VarChar, 255);
                cmd.Parameters.Add("@category", SqlDbType.VarChar, 50);
                cmd.Parameters.Add("@photo", SqlDbType.Image);
                string sUpdate = "UPDATE Judges SET name=@name,surname=@surname,patronimic=@patronimic,city=@city,category=@category,photo=@photo,changed=0 WHERE iid=@iid";
                string sInsert = "INSERT INTO Judges(iid, name,surname,patronimic,city,category,photo,changed) " +
                    " VALUES (@iid, @name, @surname, @patronimic, @city,@category,@photo,0)";
                cmd.Transaction = tran;
                SqlCommand cmdCheck = new SqlCommand();
                cmdCheck.Connection = cn;
                cmdCheck.CommandText = "SELECT ISNULL(COUNT(*),0) cnt FROM Judges(NOLOCK) WHERE iid=@iid";
                cmdCheck.Parameters.Add("@iid", SqlDbType.Int);
                cmdCheck.Transaction = tran;
                foreach (dsClimbing.ONLjudgesRow row in jta.GetData().Rows)
                {
                    cmdCheck.Parameters[0].Value = row.iid;
                    if (Convert.ToInt32(cmdCheck.ExecuteScalar()) > 0)
                        cmd.CommandText = sUpdate;
                    else
                        cmd.CommandText = sInsert;
                    cmd.Parameters[0].Value = row.iid;
                    cmd.Parameters[1].Value = row.surname;
                    cmd.Parameters[2].Value = row.name;
                    cmd.Parameters[3].Value = row.patronimic;
                    cmd.Parameters[4].Value = row.city;
                    cmd.Parameters[5].Value = row.category;
                    if (row.IsphotoNull())
                        cmd.Parameters[6].Value = DBNull.Value;
                    else
                        cmd.Parameters[6].Value = row.photo;
                    cmd.ExecuteNonQuery();
                    insJudges.Add(row.iid);
                }

                cmd.Parameters.Clear();
                cmd.Parameters.Add("@iid", SqlDbType.Int);
                cmd.Parameters.Add("@name", SqlDbType.VarChar, 255);
                sInsert = "INSERT INTO positions(iid,name) VALUES (@iid, @name)";
                sUpdate = "UPDATE positions SET name=@name WHERE iid=@iid";
                cmdCheck.CommandText = "SELECT ISNULL(COUNT(*),0) cnt FROM positions WHERE iid=@iid";
                SqlDataAdapter da = new SqlDataAdapter(new SqlCommand("SELECT iid,name FROM ONLpositions(NOLOCK)", remote));
                DataTable dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    cmdCheck.Parameters[0].Value = Convert.ToInt32(row["iid"]);
                    if (Convert.ToInt32(cmdCheck.ExecuteScalar()) > 0)
                        cmd.CommandText = sUpdate;
                    else
                        cmd.CommandText = sInsert;
                    cmd.Parameters[0].Value = cmdCheck.Parameters[0].Value;
                    cmd.Parameters[1].Value = row["name"].ToString();
                    cmd.ExecuteNonQuery();
                    insPos.Add((int)cmdCheck.Parameters[0].Value);
                }

                cmd.Parameters.Clear();
                cmd.Parameters.Add("@iid", SqlDbType.Int);
                cmd.Parameters.Add("@judge_id", SqlDbType.Int);
                cmd.Parameters.Add("@pos_id", SqlDbType.Int);
                sInsert = "INSERT INTO JudgePos(iid,judge_id,pos_id) VALUES (@iid, @judge_id,@pos_id)";
                sUpdate = "UPDATE JudgePos SET judge_id=@judge_id,pos_id=@pos_id WHERE iid=@iid";
                cmdCheck.CommandText = "SELECT ISNULL(COUNT(*),0) cnt FROM JudgePos WHERE iid=@iid";
                da.SelectCommand.CommandText = "SELECT iid,judge_id, pos_id FROM ONLjudgePos(NOLOCK)";
                dt.Rows.Clear();
                dt.Columns.Clear();
                dt.Clear();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                {
                    cmdCheck.Parameters[0].Value = Convert.ToInt32(row["iid"]);
                    if (Convert.ToInt32(cmdCheck.ExecuteScalar()) > 0)
                        cmd.CommandText = sUpdate;
                    else
                        cmd.CommandText = sInsert;
                    cmd.Parameters[0].Value = cmdCheck.Parameters[0].Value;
                    cmd.Parameters[1].Value = Convert.ToInt32(row["judge_id"]);
                    cmd.Parameters[2].Value = Convert.ToInt32(row["pos_id"]);
                    cmd.ExecuteNonQuery();
                    insJPos.Add((int)cmdCheck.Parameters[0].Value);
                }
                tran.Commit();
                if (MessageBox.Show(this, "Загрузка завершена. Удалить остальных судей?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                    == DialogResult.Yes)
                {
                    tran = cn.BeginTransaction();
                    cmd.Parameters.Clear();
                    cmd.Transaction = tran;
                    int cnt = 0;
                    if (insJPos.Count > 0)
                    {
                        cmd.CommandText = "DELETE FROM JudgePos WHERE iid NOT IN(";
                        foreach (int i in insJPos)
                            cmd.CommandText += i.ToString() + ",";
                        cmd.CommandText = cmd.CommandText.Substring(0, cmd.CommandText.Length - 1) + ")";
                        cnt += cmd.ExecuteNonQuery();
                    }
                    if (insJudges.Count > 0)
                    {
                        string sIn = " NOT IN(";
                        foreach (int i in insJudges)
                            sIn += i.ToString() + ",";
                        sIn = sIn.Substring(0, sIn.Length - 1) + ")";
                        cmd.CommandText = "DELETE FROM JudgePos WHERE judge_id" + sIn;
                        cnt += cmd.ExecuteNonQuery();
                        cmd.CommandText = "DELETE FROM Judges WHERE iid" + sIn;
                        cnt += cmd.ExecuteNonQuery();
                    }
                    if (insPos.Count > 0)
                    {
                        string sIn = " NOT IN(";
                        foreach (int i in insPos)
                            sIn += i.ToString() + ",";
                        sIn = sIn.Substring(0, sIn.Length - 1) + ")";
                        cmd.CommandText = "DELETE FROM JudgePos WHERE pos_id" + sIn;
                        cnt += cmd.ExecuteNonQuery();
                        cmd.CommandText = "DELETE FROM positions WHERE iid" + sIn;
                        cnt += cmd.ExecuteNonQuery();
                    }
                    if (cnt > 0)
                    {
                        if (MessageBox.Show(this, "Удалить " + cnt.ToString() + " записей?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                            == DialogResult.Yes)
                            tran.Commit();
                        else
                            tran.Rollback();
                    }
                    else
                        tran.Rollback();
                }
            }
            catch (ThreadAbortException)
            {
                try
                {
                    if (tran != null)
                        tran.Rollback();
                }
                catch { }
            }
            catch (Exception ex)
            {
                try
                {
                    if (tran != null)
                        tran.Rollback();
                }
                catch { }
                MessageBox.Show(this, "Ошибка загрузки списка судей:\r\n" + ex.Message);
            }
            finally
            {
                try
                {
                    if (remote != null && remote.State != ConnectionState.Closed)
                        remote.Close();
                }
                catch { }
                RefreshData();
            }
        }

        private void btnLoadFRinet_Click(object sender, EventArgs e)
        {
            LoadFromInet();
        }

        private void btnPosListEdit_Click(object sender, EventArgs e)
        {
            PositionsEditForm pef = new PositionsEditForm(this.cn, this.competitionTitle);
            pef.ShowDialog(this);
        }
    }
}