// <copyright file="SelectTeamType.cs">
// Copyright В© 2016 All Rights Reserved
// This file is part of ClimbingCompetition.
//
//  ClimbingCompetition is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  ClimbingCompetition is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with ClimbingCompetition.  If not, see <http://www.gnu.org/licenses/>.
//
// (Р­С‚РѕС‚ С„Р°Р№Р» вЂ” С‡Р°СЃС‚СЊ ClimbingCompetition.
// 
// ClimbingCompetition - СЃРІРѕР±РѕРґРЅР°СЏ РїСЂРѕРіСЂР°РјРјР°: РІС‹ РјРѕР¶РµС‚Рµ РїРµСЂРµСЂР°СЃРїСЂРѕСЃС‚СЂР°РЅСЏС‚СЊ РµРµ Рё/РёР»Рё
// РёР·РјРµРЅСЏС‚СЊ РµРµ РЅР° СѓСЃР»РѕРІРёСЏС… РЎС‚Р°РЅРґР°СЂС‚РЅРѕР№ РѕР±С‰РµСЃС‚РІРµРЅРЅРѕР№ Р»РёС†РµРЅР·РёРё GNU РІ С‚РѕРј РІРёРґРµ,
// РІ РєР°РєРѕРј РѕРЅР° Р±С‹Р»Р° РѕРїСѓР±Р»РёРєРѕРІР°РЅР° Р¤РѕРЅРґРѕРј СЃРІРѕР±РѕРґРЅРѕРіРѕ РїСЂРѕРіСЂР°РјРјРЅРѕРіРѕ РѕР±РµСЃРїРµС‡РµРЅРёСЏ;
// Р»РёР±Рѕ РІРµСЂСЃРёРё 3 Р»РёС†РµРЅР·РёРё, Р»РёР±Рѕ (РїРѕ РІР°С€РµРјСѓ РІС‹Р±РѕСЂСѓ) Р»СЋР±РѕР№ Р±РѕР»РµРµ РїРѕР·РґРЅРµР№
// РІРµСЂСЃРёРё.
// 
// ClimbingCompetition СЂР°СЃРїСЂРѕСЃС‚СЂР°РЅСЏРµС‚СЃСЏ РІ РЅР°РґРµР¶РґРµ, С‡С‚Рѕ РѕРЅР° Р±СѓРґРµС‚ РїРѕР»РµР·РЅРѕР№,
// РЅРѕ Р‘Р•Р—Рћ Р’РЎРЇРљРРҐ Р“РђР РђРќРўРР™; РґР°Р¶Рµ Р±РµР· РЅРµСЏРІРЅРѕР№ РіР°СЂР°РЅС‚РёРё РўРћР’РђР РќРћР“Рћ Р’РР”Рђ
// РёР»Рё РџР РР“РћР”РќРћРЎРўР Р”Р›РЇ РћРџР Р•Р”Р•Р›Р•РќРќР«РҐ Р¦Р•Р›Р•Р™. РџРѕРґСЂРѕР±РЅРµРµ СЃРј. РІ РЎС‚Р°РЅРґР°СЂС‚РЅРѕР№
// РѕР±С‰РµСЃС‚РІРµРЅРЅРѕР№ Р»РёС†РµРЅР·РёРё GNU.
// 
// Р’С‹ РґРѕР»Р¶РЅС‹ Р±С‹Р»Рё РїРѕР»СѓС‡РёС‚СЊ РєРѕРїРёСЋ РЎС‚Р°РЅРґР°СЂС‚РЅРѕР№ РѕР±С‰РµСЃС‚РІРµРЅРЅРѕР№ Р»РёС†РµРЅР·РёРё GNU
// РІРјРµСЃС‚Рµ СЃ СЌС‚РѕР№ РїСЂРѕРіСЂР°РјРјРѕР№. Р•СЃР»Рё СЌС‚Рѕ РЅРµ С‚Р°Рє, СЃРј. <http://www.gnu.org/licenses/>.)
// </copyright>
// <author>Ivan Kaurov</author>
// <date>30.08.2016 23:51</date>

using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ClimbingCompetition
{
	/// <summary>
	/// Форма для указания способа подсчёта командных результатов
	/// </summary>
	public class SelectTeamType : BaseForm
	{
		private System.Windows.Forms.RadioButton rbGroup;
		private System.Windows.Forms.RadioButton radioButton1;
		private System.Windows.Forms.TextBox nM;
		private System.Windows.Forms.TextBox nF;
		private System.Windows.Forms.TextBox nG;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
        private TeamResult tRes;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
        int list_id;
        private TextBox tbCoef;
        private Label label3;
        //SqlConnection cn;
        string style;

		public static TeamResult selectResultsType(SqlConnection cn, int list_id, string competitionTitle) 
		{
			SelectTeamType tm = new SelectTeamType(cn, list_id, competitionTitle);
			tm.ShowDialog();
			return tm.tRes;
		}

		public SelectTeamType(SqlConnection baseCon, int list_id, string competitionTitle) : base(baseCon, competitionTitle)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			rbGroup.Checked = true;
            this.list_id = list_id;
            //this.cn = cn;
            //if (this.cn.State != System.Data.ConnectionState.Open)
            //    cn.Open();
            SqlCommand cmd = new SqlCommand("SELECT style FROM lists(NOLOCK) WHERE iid = " + list_id.ToString(), cn);
            style = cmd.ExecuteScalar().ToString();
            switch(style){
                case "Командные - Тр":
                    this.tRes.lead = true;
                    this.tRes.speed  = this.tRes.boulder = false;
                    break;
                case "Командные - Б":
                    this.tRes.boulder = true;
                    this.tRes.speed = this.tRes.lead = false;
                    break;
                case "Командные - Ск":
                    this.tRes.speed = true;
                    this.tRes.boulder = this.tRes.lead = false;
                    break;
#if !FULL
                case "Командные":
                    goto case "Командные - Ск";
#endif
                default:
                    cmd.CommandText = "SELECT COUNT(*) FROM lists(NOLOCK) WHERE style = 'Трудность' AND round = 'Итоговый протокол'";
                    tRes.lead = (Convert.ToInt32(cmd.ExecuteScalar()) > 0);
                    cmd.CommandText = "SELECT COUNT(*) FROM lists(NOLOCK) WHERE style = 'Скорость' AND round = 'Итоговый протокол'";
                    tRes.speed = (Convert.ToInt32(cmd.ExecuteScalar()) > 0);
                    cmd.CommandText = "SELECT COUNT(*) FROM lists(NOLOCK) WHERE style = 'Боулдеринг' AND round = 'Итоговый протокол'";
                    tRes.boulder = (Convert.ToInt32(cmd.ExecuteScalar()) > 0);
                    break;
            }
            double coef2ndTeam = 1.0;
            try
            {
                SortingClass.CheckColumn("lists_hdr2", "coef2ndTeam", "FLOAT NOT NULL DEFAULT 1.0", cn);
                cmd.CommandText = "SELECT use_group, male, female, coef2ndTeam FROM lists_hdr2(NOLOCK) WHERE list_id = " + list_id.ToString();
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        bool b = Convert.ToBoolean(rdr[0]);
                        rbGroup.Checked = b;
                        radioButton1.Checked = !b;
                        int nMale = Convert.ToInt32(rdr[1]);
                        if (b)
                        {
                            nM.Text = nMale.ToString();
                            nF.Text = rdr[2].ToString();
                        }
                        else
                            nG.Text = nMale.ToString();
                        coef2ndTeam = Convert.ToDouble(rdr["coef2ndTeam"]);
                    }
                }
            }
            catch
            {
                try
                {
                    cmd.CommandText = "CREATE TABLE [dbo].[lists_hdr2] (" +
    "	[list_id] [int] NOT NULL ," +
    "	[use_group] [bit] NOT NULL ," +
    "	[male] [int] NOT NULL ," +
    "	[female] [int] NULL ," +
    "   [coef2ndTeam] FLOAT NOT NULL DEFAULT 1.0," +
    "	CONSTRAINT [PK_lists_hdr2] PRIMARY KEY  CLUSTERED " +
    "	(" +
    "		[list_id]" +
    "	)  ON [PRIMARY] ," +
    "	CONSTRAINT [FK_lists_hdr2_lists] FOREIGN KEY " +
    "	(" +
    "		[list_id]" +
    "	) REFERENCES [dbo].[lists] (" +
    "		[iid]" +
    "	) ON DELETE CASCADE  ON UPDATE CASCADE" +
    ")";
                    cmd.ExecuteNonQuery();
                }
                catch { }
            }
            tRes.Qf2nd = coef2ndTeam;
            tbCoef.Text = coef2ndTeam.ToString("0.0#");
			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.rbGroup = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.nM = new System.Windows.Forms.TextBox();
            this.nF = new System.Windows.Forms.TextBox();
            this.nG = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tbCoef = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // rbGroup
            // 
            this.rbGroup.Location = new System.Drawing.Point(16, 8);
            this.rbGroup.Name = "rbGroup";
            this.rbGroup.Size = new System.Drawing.Size(160, 24);
            this.rbGroup.TabIndex = 0;
            this.rbGroup.TabStop = true;
            this.rbGroup.Text = "N лучших по группе/виду";
            this.rbGroup.CheckedChanged += new System.EventHandler(this.rbGroup_CheckedChanged);
            // 
            // radioButton1
            // 
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(16, 96);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(160, 24);
            this.radioButton1.TabIndex = 1;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "N лучших по виду";
            // 
            // nM
            // 
            this.nM.Location = new System.Drawing.Point(192, 8);
            this.nM.Name = "nM";
            this.nM.Size = new System.Drawing.Size(64, 20);
            this.nM.TabIndex = 2;
            // 
            // nF
            // 
            this.nF.Location = new System.Drawing.Point(192, 40);
            this.nF.Name = "nF";
            this.nF.Size = new System.Drawing.Size(64, 20);
            this.nF.TabIndex = 3;
            // 
            // nG
            // 
            this.nG.Location = new System.Drawing.Point(192, 96);
            this.nG.Name = "nG";
            this.nG.Size = new System.Drawing.Size(64, 20);
            this.nG.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(264, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "N в мужских группах";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(264, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(120, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "N в женских группах";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(12, 162);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(96, 32);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(160, 162);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(96, 32);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // tbCoef
            // 
            this.tbCoef.Location = new System.Drawing.Point(192, 136);
            this.tbCoef.Name = "tbCoef";
            this.tbCoef.Size = new System.Drawing.Size(64, 20);
            this.tbCoef.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 143);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(168, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Коэффициент для 2ой команды";
            // 
            // SelectTeamType
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(384, 217);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbCoef);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nM);
            this.Controls.Add(this.nF);
            this.Controls.Add(this.nG);
            this.Controls.Add(this.rbGroup);
            this.Controls.Add(this.radioButton1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnCancel);
            this.Name = "SelectTeamType";
            this.Text = "Подсчёт командных результатов";
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

        private void btnOK_Click(object sender, System.EventArgs e)
        {
            double d;
            if (!double.TryParse(tbCoef.Text.Trim(), out d))
            {
                tbCoef.Text = tbCoef.Text.Trim().Replace(',', '.');
                if (!double.TryParse(tbCoef.Text, out d))
                {
                    tbCoef.Text = tbCoef.Text.Remove('.', ',');
                    if (!double.TryParse(tbCoef.Text, out d))
                    {
                        MessageBox.Show("Коэффициент указан неправильно");
                        return;
                    }
                }
            }
            tRes.Qf2nd = d;
            if (rbGroup.Checked)
            {
                int nMale, nFemale;
                try
                {
                    nMale = Convert.ToInt32(nM.Text);
                    nFemale = Convert.ToInt32(nF.Text);
                }
                catch
                {
                    MessageBox.Show("Неверный ввод");
                    return;
                }
                tRes.type = TeamResultsType.Group;
                tRes.nFemale = nFemale;
                tRes.nMale = nMale;
            }
            else
            {
                int n;
                try
                { n = Convert.ToInt32(nG.Text); }
                catch
                {
                    MessageBox.Show("Неверный ввод");
                    return;
                }
                tRes.type = TeamResultsType.Style;
                tRes.nMale = n;
                tRes.nFemale = 0;
            }
            try
            {
                if (cn.State != System.Data.ConnectionState.Open)
                    cn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.CommandText = "DELETE FROM lists_hdr2 WHERE list_id = " + list_id.ToString();
                cmd.ExecuteNonQuery();
                cmd.CommandText = "INSERT INTO lists_hdr2(list_id, use_group, male, female,coef2ndTeam) VALUES (" +
                    list_id.ToString() + ", @ug, " + tRes.nMale + ", " + tRes.nFemale + ",@c2team)";
                
                cmd.Parameters.Add("@ug", System.Data.SqlDbType.Bit);
                cmd.Parameters[0].Value = (tRes.type == TeamResultsType.Group);
                cmd.Parameters.Add("@c2team", System.Data.SqlDbType.Float);
                cmd.Parameters[1].Value = tRes.Qf2nd;
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            this.Close();
        }

		private void rbGroup_CheckedChanged(object sender, System.EventArgs e)
		{
			if(rbGroup.Checked) 
			{
				nM.Enabled = true;
				nF.Enabled = true;
				nG.Enabled = false;
			}
			else
			{
				nM.Enabled = false;
				nF.Enabled = false;
				nG.Enabled = true;
			}
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			tRes.type = TeamResultsType.Cancel;
			tRes.nFemale = 0;
			tRes.nMale = 0;
			this.Close();
		}
	}

	public enum TeamResultsType{Group,Style,Cancel}
	public struct TeamResult 
	{
		public TeamResultsType type;
		public int nMale;
		public int nFemale;
		public bool lead;
		public bool speed;
        public bool boulder;
        public double Qf2nd;
	}
}
