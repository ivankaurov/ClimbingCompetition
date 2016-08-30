// <copyright file="SetConnectionString.cs">
// Copyright © 2016 All Rights Reserved
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
// (Этот файл — часть ClimbingCompetition.
// 
// ClimbingCompetition - свободная программа: вы можете перераспространять ее и/или
// изменять ее на условиях Стандартной общественной лицензии GNU в том виде,
// в каком она была опубликована Фондом свободного программного обеспечения;
// либо версии 3 лицензии, либо (по вашему выбору) любой более поздней
// версии.
// 
// ClimbingCompetition распространяется в надежде, что она будет полезной,
// но БЕЗО ВСЯКИХ ГАРАНТИЙ; даже без неявной гарантии ТОВАРНОГО ВИДА
// или ПРИГОДНОСТИ ДЛЯ ОПРЕДЕЛЕННЫХ ЦЕЛЕЙ. Подробнее см. в Стандартной
// общественной лицензии GNU.
// 
// Вы должны были получить копию Стандартной общественной лицензии GNU
// вместе с этой программой. Если это не так, см. <http://www.gnu.org/licenses/>.)
// </copyright>
// <author>Ivan Kaurov</author>
// <date>30.08.2016 23:51</date>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using ClimbingCompetition.WebServices;
using System.Xml.Serialization;
using XmlApiClient;
using System.Xml;
using XmlApiData;
using System.Net;

namespace ClimbingCompetition
{
    /// <summary>
    /// Форма для настройки соединения с удалённой БД для онлайн трансляции
    /// </summary>
    public partial class SetConnectionString : BaseForm
    {
        //SqlConnection cn;
        public SetConnectionString(string str, string competitionTitle, SqlConnection baseCnLocal)
            : base(baseCnLocal, competitionTitle, null)
        {
            //cn = baseCnLocal;
            //if (cn.State != ConnectionState.Open)
            //    cn.Open();
            if (SortingClass.CheckColumn("CompetitionData", "remoteString", "VARCHAR(8000)", cn))
            {
                SqlCommand cmL = new SqlCommand();
                cmL.Connection = cn;
                cmL.CommandText = "SELECT C.length" +
                                  "  FROM syscolumns C(nolock)" +
                                  "  JOIN sysobjects O(nolock) on O.id = C.id" +
                                  " WHERE O.name = 'CompetitionData'" +
                                  "   AND C.name = 'remoteString'";
                object oLength = cmL.ExecuteScalar();
                int nLength;
                if(oLength!= null && int.TryParse(oLength.ToString(), out nLength))
                    if (nLength < 8000 && nLength > 0)
                    {
                        cmL.CommandText = "ALTER TABLE CompetitionData ALTER COLUMN remoteString VARCHAR(8000)";
                        cmL.ExecuteNonQuery();
                    }
            }
            SortingClass.CheckColumn("CompetitionData", "remoteTime", "INT", cn);
            //               SELECT C.name, O.type 
            // FROM syscolumns C(NOLOCK)
            // JOIN sysobjects O(NOLOCK) ON O.id = C.id
            //WHERE O.name = 'CompetitionData'
            //  AND O.type = 'U'
            InitializeComponent();
            //tbHeight.Text = str;

            this.competitionTitle = competitionTitle;
            try
            {
                DbData d = this.DataFromDb;
                textBox1.Text = d.connectionString;
                if (d.time > -1)
                    comboBox1.SelectedIndex = d.time;
            }
            catch { textBox1.Text = str; }
            conStr = str;
            rbCString_CheckedChanged(rbCString, new EventArgs());

            //rbUseService.Enabled = gbUseWebServices.Enabled = false;
        }


        string conStr;
        int timerInt;

        public int TimerInt
        {
            get { return timerInt; }
            set { timerInt = value; }
        }
        public string GetConnectionString { get { return conStr; } }

        SqlConnection remote = new SqlConnection();

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!SetCString())
                return;
            if (comboBox1.SelectedIndex < 0)
            {
                MessageBox.Show("Интервал обновления не выбран");
                return;
            }
            if (comboBox1.SelectedIndex == 0)
                timerInt = 30;
            else
            {
                timerInt = int.Parse(comboBox1.Text.Substring(0,
                    comboBox1.Text.IndexOf(' '))) * 60;
            }
            conStr = textBox1.Text;
            var cWrp = GetClientWrapper(textBox1.Text);
            if (cWrp != null)
            {
                var client = cWrp.CreateClient(cn);
                try
                {
                    if (!client.ValidateCompetition())
                        if (MessageBox.Show(this,
                            String.Format("Ошибка проверки подлинности пользователя.{0}" +
                            "Необходимо настроить аутентификацию.{0}" +
                            "Сохранить подключение без настройки аутентификации?", Environment.NewLine),
                            String.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                            == System.Windows.Forms.DialogResult.No)
                            return;
                }
                catch (WebException ex)
                {
                    StaticClass.ShowExceptionMessageBox("Ошибка проверки подлинности", ex, this);
                    return;
                }
            }
            try
            {
                //Data Source=toshiba-kaurov\sql2000;Initial Catalog=testDB;User ID=sa;password=sa
                DbData d;
                d.connectionString = conStr;
                d.time = comboBox1.SelectedIndex;
                this.DataFromDb = d;
            }
            catch { }
            if (conStr.Length > 0 && cWrp == null)
            {
                try
                {
                    remote.ConnectionString = conStr;
                    if (remote.State != ConnectionState.Open)
                        remote.Open();
                    if (cn.State != ConnectionState.Open)
                        cn.Open();
                    SqlCommand cmdLocal = new SqlCommand();
                    cmdLocal.Connection = cn;
                    cmdLocal.CommandText = "SELECT NAME FROM CompetitionData(NOLOCK)";

                    string cName;
                    try { cName = cmdLocal.ExecuteScalar().ToString(); }
                    catch { cName = ""; }

                    cmdLocal.Connection = remote;
                    cmdLocal.CommandText = "UPDATE ONLData SET table_name = '" + cName + "'";
                    cmdLocal.ExecuteNonQuery();
                }
                catch { }
            }

            this.Close();
        }

        struct DbData
        {
            public string connectionString;
            public int time;
        }

        DbData DataFromDb
        {
            get
            {
                DbData retVal;
                retVal.time = -1;
                retVal.connectionString = "";
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                cmd.CommandText = "SELECT ISNULL(remoteString,'') rS, ISNULL(remoteTime,-1) rT FROM CompetitionData(NOLOCK)";
                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    retVal.connectionString = rdr[0].ToString();
                    retVal.time = Convert.ToInt32(rdr[1]);
                }
                rdr.Close();
                return retVal;
            }
            set
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                cmd.CommandText = "UPDATE CompetitionData SET remoteString=@cs, remoteTime=@rt";
                cmd.Parameters.Add("@cs", SqlDbType.VarChar, 8000);
                cmd.Parameters[0].Value = value.connectionString;

                cmd.Parameters.Add("@rt", SqlDbType.Int);
                cmd.Parameters[1].Value = value.time;

                cmd.ExecuteNonQuery();
            }
        }
                
        private void btnCancel_Click(object sender, EventArgs e)
        {
            conStr = "";
            this.Close();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            try
            {
                if (!SetCString())
                    return;
                try { remote.Close(); }
                finally { remote.ConnectionString = textBox1.Text; }

                if (remote.State != ConnectionState.Open)
                    remote.Open();
                remote.Close();
                btnCreateDB.Enabled = btnChangeAdmPassword.Enabled = true;
                MessageBox.Show("Соединение успешно открыто.");
            }
            catch (Exception ex)
            { MessageBox.Show("Ошибка соединения.\r\n\r\nДанные ошибки:\r\n" + ex.Message); }
        }

        private static void AddAdmin(SqlConnection cn, SqlTransaction tran, string password)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            if (tran == null)
                cmd.Transaction = cn.BeginTransaction();
            else
                cmd.Transaction = tran;
            try
            {
                cmd.CommandText = "IF NOT EXISTS(SELECT * FROM ONLroles(NOLOCK) WHERE iid='ADM')" +
                    "INSERT INTO ONLroles(iid,role_name) VALUES ('ADM','Администратор')";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "IF NOT EXISTS(SELECT * FROM ONLroles(NOLOCK) WHERE iid='USR')" +
                    "INSERT INTO ONLroles(iid,role_name) VALUES ('USR','Пользователь')";
                cmd.ExecuteNonQuery();

                cmd.CommandText = @"
                     IF EXISTS(SELECT * FROM ONLusers(NOLOCK) WHERE iid = 'AAA')
                        UPDATE ONLusers SET name='Администратор', password=@pwd
                           WHERE iid = 'AAA'
                     ELSE
                        INSERT INTO ONLusers (iid, name, password, creationDate)
                           VALUES('AAA','Администратор', @pwd, @crd)";
                cmd.Parameters.Add("@pwd", SqlDbType.VarChar, 8000);
                cmd.Parameters[0].Value = password;
                cmd.Parameters.Add("@crd", SqlDbType.DateTime);
                cmd.Parameters[1].Value = DateTime.Now.ToUniversalTime();
                cmd.ExecuteNonQuery();

                long nextId;
                cmd.CommandText = @"SELECT ISNULL(MAX(iid),0) + 1 iid
                                      FROM ONLuserRoles(NOLOCK)";
                object oTmp = cmd.ExecuteScalar();
                if (oTmp is long)
                    nextId = (long)oTmp;
                else if (oTmp == null || oTmp == DBNull.Value)
                    nextId = 1;
                else
                    nextId = Convert.ToInt64(oTmp);

                cmd.CommandText = @"
                    IF NOT EXISTS(SELECT * FROM ONLuserRoles(NOLOCK)
                                   WHERE user_id = 'AAA' AND role_id = 'ADM')
                       INSERT INTO ONLuserRoles(iid, user_id, role_id)
                           VALUES(" + nextId.ToString() + ",'AAA','ADM')";
                cmd.ExecuteNonQuery();
                if (tran == null)
                    cmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                if (tran == null)
                    cmd.Transaction.Rollback();
                throw ex;
            }
        }

        private void btnCreateDB_Click(object sender, EventArgs e)
        {
            if (!SetCString())
                return;
            try
            {
                if (MessageBox.Show("Внимание! Все данные, хранящиеся в выбранной БД будут безвозвратно удалены! Продолжить?", "Создать БД", MessageBoxButtons.YesNo, MessageBoxIcon.Hand) == DialogResult.No)
                    return;
                string newAdminPassword;
                if (!SetAdminPassword.CreateNewPassword(this, out newAdminPassword))
                    return;
                if (remote.State != ConnectionState.Open)
                    remote.Open();
                if (cn.State != ConnectionState.Open)
                    cn.Open();

                

                SqlCommand cmdLocal = new SqlCommand();
                cmdLocal.Connection = cn;
                cmdLocal.CommandText = "SELECT SHORT_NAME FROM CompetitionData(NOLOCK)";
                var cRules = SortingClass.GetCompRules(cn, false);
                string cName;
                try { cName = cmdLocal.ExecuteScalar().ToString(); }
                catch { cName = ""; }

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = remote;
                cmd.Transaction = null;
                try
                {
                    cmd.CommandText = " ALTER DATABASE " + cmd.Connection.Database + " COLLATE Cyrillic_General_CI_AS ";
                    cmd.ExecuteNonQuery();

                    remote = new SqlConnection(cmd.Connection.ConnectionString);
                    cmd.Connection.Close();
                    remote.Open();
                    cmd.Connection = remote;

                    cmd.Transaction = remote.BeginTransaction();

                    cmd.CommandText = @"IF EXISTS(select name FROM sysobjects
                                             WHERE name = 'ONLrankings' AND type='U')
                                DROP TABLE ONLrankings";
                    cmd.ExecuteNonQuery();
                    
                    cmd.CommandText = @"IF EXISTS(select name FROM sysobjects
                                             WHERE name = 'ONLclimbersUnc' AND type='U')
                                DROP TABLE ONLclimbersUnc";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"IF EXISTS(select name FROM sysobjects
                                             WHERE name = 'ONLoperations' AND type='U')
                                DROP TABLE ONLoperations";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"IF EXISTS(select name FROM sysobjects
                                               WHERE name = 'ONLuserRoles' AND type = 'U')
                                DROP TABLE ONLuserRoles";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"IF EXISTS(select name FROM sysobjects
                                               WHERE name = 'ONLroles' AND type = 'U')
                                DROP TABLE ONLroles";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"IF EXISTS(select name FROM sysobjects
                                               WHERE name = 'ONLusers' AND type = 'U')
                                DROP TABLE ONLusers";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"IF EXISTS(SELECT name FROM sysobjects
                                               WHERE name = 'ONLdata' AND type = 'U')
                                DROP TABLE ONLdata";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"IF EXISTS(SELECT name FROM sysobjects
                                               WHERE name = 'ONLlead' AND type = 'U')
                                DROP TABLE ONLlead";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"IF EXISTS(SELECT name FROM sysobjects
                                               WHERE name = 'ONLspeed' AND type = 'U')
                                DROP TABLE ONLspeed";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"IF EXISTS(SELECT name FROM sysobjects
                                               WHERE name = 'ONLboulderRoutes' AND type = 'U')
                                DROP TABLE ONLboulderRoutes";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"IF EXISTS(SELECT name FROM sysobjects
                                               WHERE name = 'ONLboulder' AND type = 'U')
                                DROP TABLE ONLboulder";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"IF EXISTS(SELECT name FROM sysobjects
                                               WHERE name = 'ONLflash' AND type = 'U')
                                DROP TABLE ONLflash";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"IF EXISTS(SELECT name FROM sysobjects
                                               WHERE name = 'ONLlistdata' AND type = 'U')
                                DROP TABLE ONLlistdata";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"IF EXISTS(SELECT name FROM sysobjects
                                               WHERE name = 'ONLlists' AND type = 'U')
                                DROP TABLE ONLlists";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"IF EXISTS(SELECT name FROM sysobjects
                                               WHERE name = 'ONLclimbers' AND type = 'U')
                                DROP TABLE ONLclimbers";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"IF EXISTS(SELECT name FROM sysobjects
                                               WHERE name = 'ONLclimbers' AND type = 'U')
                                DROP TABLE ONLclimbersUnc";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"IF EXISTS(SELECT name FROM sysobjects
                                               WHERE name = 'ONLteams' AND type = 'U')
                                DROP TABLE ONLteams";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"IF EXISTS(SELECT name FROM sysobjects
                                               WHERE name = 'ONLgroups' AND type = 'U')
                                DROP TABLE ONLgroups";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"IF EXISTS(select name FROM sysobjects
                                             WHERE name = 'ONLqfList' AND type='U')
                                DROP TABLE ONLqfList";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"IF EXISTS(SELECT name FROM sysobjects
                                               WHERE name = 'ONLjudgePos' AND type = 'U')
                                DROP TABLE ONLjudgePos";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"IF EXISTS(SELECT name FROM sysobjects
                                               WHERE name = 'ONLjudges' AND type = 'U')
                                DROP TABLE ONLjudges";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"IF EXISTS(SELECT name FROM sysobjects
                                               WHERE name = 'ONLpositions' AND type = 'U')
                                DROP TABLE ONLpositions";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"IF EXISTS(SELECT name FROM sysobjects
                                               WHERE name = 'InsertTeamGroup' AND type = 'P')
                                DROP PROCEDURE InsertTeamGroup";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"IF EXISTS(SELECT name FROM sysobjects
                                               WHERE name = 'InsertClimber' AND type = 'P')
                                DROP PROCEDURE InsertClimber";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"IF EXISTS(SELECT name FROM sysobjects
                                               WHERE name = 'InsertList' AND type = 'P')
                                DROP PROCEDURE InsertList";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"IF EXISTS(SELECT name FROM sysobjects
                                               WHERE name = 'InsertLead' AND type = 'P')
                                DROP PROCEDURE InsertLead";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"IF EXISTS(SELECT name FROM sysobjects
                                               WHERE name = 'InsertBoulder' AND type = 'P')
                                DROP PROCEDURE InsertBoulder";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"IF EXISTS(SELECT name FROM sysobjects
                                               WHERE name = 'InsertBoulderRoute' AND type = 'P')
                                DROP PROCEDURE InsertBoulderRoute";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"IF EXISTS(SELECT name FROM sysobjects
                                               WHERE name = 'InsertSpeed' AND type = 'P')
                                DROP PROCEDURE InsertSpeed";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"IF EXISTS(SELECT name FROM sysobjects
                                               WHERE name = 'InsertJudgePos' AND type = 'P')
                                DROP PROCEDURE InsertJudgePos";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"IF EXISTS(SELECT name FROM sysobjects
                                               WHERE name = 'InsertJudge' AND type = 'P')
                                DROP PROCEDURE InsertJudge";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"IF EXISTS(SELECT name FROM sysobjects
                                               WHERE name = 'InsertPos' AND type = 'P')
                                DROP PROCEDURE InsertPos";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"IF EXISTS(SELECT name FROM sysobjects
                                               WHERE name = 'InsertInitialData' AND type = 'P')
                                DROP PROCEDURE InsertInitialData";
                    cmd.ExecuteNonQuery();



                    cmd.CommandText = "CREATE TABLE [dbo].[ONLData] (" +
        "[table_name] [varchar] (255) COLLATE Cyrillic_General_CI_AS NULL, " +
        "[COL_NAME_R] [int] NULL, " +
        "[lastUpd] [DateTime] NULL"+
    ")";
                    cmd.ExecuteNonQuery();

                    //dt.Cells[1, 1] = StaticClass.ParseCompTitle(competitionTitle)[0];
                    cmd.CommandText = "INSERT INTO ONLData (table_name,COL_NAME_R) VALUES ('" +
                        cName + "'," + ((int)cRules).ToString() + ")";
                    try { cmd.ExecuteNonQuery(); }
                    catch { }

                    cmd.CommandText = "CREATE TABLE dbo.ONLqfList (" +
                                      " iid INT NOT NULL PRIMARY KEY," +
                                      "  qf VARCHAR(4) COLLATE Cyrillic_General_CI_AS NOT NULL" +
                                      ")";
                    cmd.ExecuteNonQuery();

                    cmdLocal.CommandText = "SELECT iid,qf FROM qfList(NOLOCK) ORDER BY iid";
                    cmd.CommandText = "INSERT INTO dbo.ONLqfList(iid,qf) VALUES (@iid,@qf)";
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@iid", SqlDbType.Int);
                    cmd.Parameters.Add("@qf", SqlDbType.VarChar, 4);
                    SqlDataReader drq = cmdLocal.ExecuteReader();
                    try
                    {
                        while (drq.Read())
                        {
                            cmd.Parameters[0].Value = drq["iid"];
                            cmd.Parameters[1].Value = drq["qf"];
                            cmd.ExecuteNonQuery();
                        }
                    }
                    finally { drq.Close(); }
                    cmd.Parameters.Clear();

                    cmd.CommandText = "      CREATE TABLE dbo.ONLGroups (" +
                                      "         iid INT NOT NULL PRIMARY KEY," +
                                      "        name VARCHAR(255) COLLATE Cyrillic_General_CI_AS NULL," +
                                      "     oldYear INT NOT NULL DEFAULT 0," +
                                      "   youngYear INT NOT NULL DEFAULT " + DateTime.Now.Year.ToString() + "," +
                                      "       minQf INT NOT NULL FOREIGN KEY REFERENCES ONLqfList(iid) ON DELETE CASCADE ON UPDATE CASCADE," +
                                      "genderFemale BIT NOT NULL DEFAULT 0)";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "CREATE TABLE dbo.ONLteams (" +
                                      "   iid INT NOT NULL PRIMARY KEY," +
                                      "  name VARCHAR(255)  COLLATE Cyrillic_General_CI_AS NULL," +
                                      "   pos INT NOT NULL DEFAULT " + int.MaxValue.ToString() +
                                      ")";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "  CREATE TABLE dbo.ONLclimbers (" +
                                      "     iid INT NOT NULL PRIMARY KEY," +
                                      " surname VARCHAR(255)  COLLATE Cyrillic_General_CI_AS NULL," +
                                      "    name VARCHAR(255)  COLLATE Cyrillic_General_CI_AS NULL," +
                                      "     age INT NULL," +
                                      "genderFemale BIT NOT NULL DEFAULT 0," +
                                      "      qf VARCHAR(50) NULL," +
                                      " team_id INT NULL FOREIGN KEY REFERENCES ONLteams(iid) ON DELETE CASCADE ON UPDATE CASCADE," +
                                      "group_id INT NULL FOREIGN KEY REFERENCES ONLgroups(iid) ON DELETE CASCADE ON UPDATE CASCADE," +
                                      "lead BIT NOT NULL DEFAULT 0," +
                                      "speed BIT NOT NULL DEFAULT 0," +
                                      "boulder BIT NOT NULL DEFAULT 0," +
                                      "rankingLead INT NULL," +
                                      "rankingSpeed INT NULL," +
                                      "rankingBoulder INT NULL," +
                                      "      vk BIT NOT NULL DEFAULT 0," +
                                      "nopoints BIT NOT NULL DEFAULT 0," +
                                      "   photo IMAGE NULL," +
                                      "phLoaded BIT NOT NULL DEFAULT 0," +
                                      "is_del BIT NOT NULL DEFAULT 0," +
                                      "sys_date_update DATETIME NOT NULL DEFAULT CONVERT(DATETIME,'01.01.2000',104)," +
                                      "appl_type VARCHAR(MAX) NOT NULL DEFAULT ''," +
                                      "is_changeble BIT NOT NULL DEFAULT 0," +
                                      "queue_pos INT NOT NULL DEFAULT 0" +
                                      ")";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "     CREATE TABLE dbo.ONLjudges (" +
                                      "        iid INT NOT NULL PRIMARY KEY," +
                                      "    surname VARCHAR(255)  COLLATE Cyrillic_General_CI_AS NOT NULL," +
                                      "       name VARCHAR(255)  COLLATE Cyrillic_General_CI_AS NOT NULL DEFAULT ''," +
                                      " patronimic VARCHAR(255)  COLLATE Cyrillic_General_CI_AS NOT NULL DEFAULT ''," +
                                      "   category VARCHAR(50)  COLLATE Cyrillic_General_CI_AS NOT NULL DEFAULT ''," +
                                      "       city VARCHAR(255)  COLLATE Cyrillic_General_CI_AS NOT NULL DEFAULT ''," +
                                      "      photo IMAGE NULL, " +
                                      "photoLoaded BIT NOT NULL DEFAULT 0)";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "     CREATE TABLE dbo.ONLpositions (" +
                                      "        iid INT NOT NULL PRIMARY KEY," +
                                      "       name VARCHAR(255)  COLLATE Cyrillic_General_CI_AS NOT NULL DEFAULT '')";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "     CREATE TABLE dbo.ONLjudgePos (" +
                                      "        iid INT NOT NULL PRIMARY KEY," +
                                      "   judge_id INT NOT NULL FOREIGN KEY REFERENCES ONLjudges(iid) ON DELETE CASCADE ON UPDATE CASCADE," +
                                      "     pos_id INT NOT NULL FOREIGN KEY REFERENCES ONLpositions(iid) ON DELETE CASCADE ON UPDATE CASCADE)";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "     CREATE TABLE dbo.ONLlists (" +
                                      "        iid INT NOT NULL PRIMARY KEY," +
                                      "      style VARCHAR(50)  COLLATE Cyrillic_General_CI_AS NULL," +
                                      "   group_id INT NULL FOREIGN KEY REFERENCES ONLgroups(iid)," +
                                      "      round VARCHAR(255)  COLLATE Cyrillic_General_CI_AS NULL," +
                                      "routeNumber INT NULL," +
                                      "       live BIT NOT NULL DEFAULT 0," +
                                      "      quote INT NOT NULL DEFAULT 0," +
                                      "  prevRound INT NULL," +
                                      " iid_parent INT NULL," +
                                      " start_time VARCHAR(255)  COLLATE Cyrillic_General_CI_AS NOT NULL DEFAULT ''," +
                                      " roundFinished BIT NOT NULL DEFAULT 0, " +
                                      "    lastUpd DATETIME NULL " +
                                      ")";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "     CREATE TABLE dbo.ONLlistdata (" +
                                      "        iid INT NOT NULL FOREIGN KEY REFERENCES ONLlists(iid) ON DELETE CASCADE ON UPDATE CASCADE," +
                                      "   iid_line BIGINT NOT NULL PRIMARY KEY ," +
                                      " climber_id INT NOT NULL FOREIGN KEY REFERENCES ONLclimbers(iid) ON DELETE CASCADE ON UPDATE CASCADE," +
                                      "      start INT NULL," +
                                      "        res BIGINT NULL," +
                                      "      preQf BIT NOT NULL DEFAULT 0," +
                                      "        pos INT NULL" +
                                      ")";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "CREATE TABLE dbo.ONLlead (" +
                                      "        idPK BIGINT NOT NULL PRIMARY KEY, " +
                                      "    iid_line BIGINT NOT NULL UNIQUE FOREIGN KEY REFERENCES ONLlistdata(iid_line) ON DELETE CASCADE ON UPDATE CASCADE," +
                                      "         res VARCHAR(50) COLLATE Cyrillic_General_CI_AS NULL" +
                                      ")";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "CREATE TABLE dbo.ONLspeed (" +
                                      "        idPK BIGINT NOT NULL PRIMARY KEY, " +
                                      "    iid_line BIGINT NOT NULL UNIQUE FOREIGN KEY REFERENCES ONLlistdata(iid_line) ON DELETE CASCADE ON UPDATE CASCADE," +
                                      "          r1 VARCHAR(50) COLLATE Cyrillic_General_CI_AS NULL ," +
                                      "          r2 VARCHAR(50) COLLATE Cyrillic_General_CI_AS NULL ," +
                                      "         res VARCHAR(50) COLLATE Cyrillic_General_CI_AS NULL," +
                                      "          qf VARCHAR(50) COLLATE Cyrillic_General_CI_AS NOT NULL DEFAULT ''" +
                                      ")";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "CREATE TABLE dbo.ONLBoulder (" +
                        "        idPK BIGINT NOT NULL PRIMARY KEY, " +
                        "    iid_line BIGINT NOT NULL UNIQUE FOREIGN KEY REFERENCES ONLlistdata(iid_line) ON DELETE CASCADE ON UPDATE CASCADE," +
                        "	[T]  [int] NULL ," +
                        "	[Ta] [int] NULL ," +
                        "	[B]  [int] NULL ," +
                        "	[Ba] [int] NULL," +
                        "   [nya] [bit] NOT NULL DEFAULT 0, " +
                        "   [disq] [bit] NOT NULL DEFAULT 0" +
                        ")";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "CREATE TABLE dbo.ONLboulderRoutes (" +
                        "   iid_line BIGINT NOT NULL PRIMARY KEY," +
                        " iid_parent BIGINT NOT NULL FOREIGN KEY REFERENCES ONLBoulder (idPK) ON DELETE CASCADE ON UPDATE CASCADE," +
                        "     routeN    INT NOT NULL," +
                        "       topA    INT NULL," +
                        "     bonusA    INT NULL" +
                        ")";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "CREATE TABLE dbo.ONLusers (" +
                        "iid VARCHAR(3)  COLLATE Cyrillic_General_CI_AS PRIMARY KEY," +
                        "name VARCHAR(255)  COLLATE Cyrillic_General_CI_AS NOT NULL," +
                        "password VARCHAR(8000)  COLLATE Cyrillic_General_CI_AS NOT NULL," +
                        "team_id INT NULL FOREIGN KEY REFERENCES ONLteams(iid) ON DELETE CASCADE ON UPDATE CASCADE," +
                        "email VARCHAR(255)  COLLATE Cyrillic_General_CI_AS NULL," +
                        "chief VARCHAR(255)  COLLATE Cyrillic_General_CI_AS NULL," +
                        "creationDate DATETIME NULL," +
                        "loginDate DATETIME NULL," +
                        "lastForgPassword DATETIME NULL," +
                        "messageToSend VARCHAR(MAX) COLLATE Cyrillic_General_CI_AS NOT NULL DEFAULT ''," +
                        "NotifSent VARCHAR(MAX) COLLATE Cyrillic_General_CI_AS NOT NULL DEFAULT ''" +
                        ")";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "CREATE TABLE dbo.ONLroles (" +
                        "iid VARCHAR(3) COLLATE Cyrillic_General_CI_AS PRIMARY KEY," +
                        "role_name VARCHAR(255)  COLLATE Cyrillic_General_CI_AS NOT NULL)";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "CREATE TABLE dbo.ONLuserRoles (" +
                        "iid BIGINT NOT NULL PRIMARY KEY," +
                        "user_id VARCHAR(3)  COLLATE Cyrillic_General_CI_AS NOT NULL FOREIGN KEY REFERENCES ONLusers(iid) ON DELETE CASCADE ON UPDATE CASCADE," +
                        "role_id VARCHAR(3)  COLLATE Cyrillic_General_CI_AS NOT NULL FOREIGN KEY REFERENCES ONLroles(iid) ON DELETE CASCADE ON UPDATE CASCADE" +
                        ")";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "CREATE TABLE dbo.ONLoperations (" +
                        "iid BIGINT NOT NULL PRIMARY KEY," +
                        "user_id VARCHAR(3)  COLLATE Cyrillic_General_CI_AS NOT NULL FOREIGN KEY REFERENCES ONLusers(iid) ON DELETE CASCADE ON UPDATE CASCADE," +
                        "op_date DATETIME NOT NULL," +
                        "state INT NOT NULL DEFAULT 0" +
                        ")";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "  CREATE TABLE dbo.ONLclimbersUnc (" +
                                      "     iid INT NOT NULL PRIMARY KEY," +
                                      " surname VARCHAR(255)  COLLATE Cyrillic_General_CI_AS NULL," +
                                      "    name VARCHAR(255)  COLLATE Cyrillic_General_CI_AS NULL," +
                                      "     age INT NULL," +
                                      "genderFemale BIT NOT NULL DEFAULT 0," +
                                      "      qf VARCHAR(50) COLLATE Cyrillic_General_CI_AS  NULL," +
                                      " team_id INT NULL FOREIGN KEY REFERENCES ONLteams(iid)," +
                                      "group_id INT NULL FOREIGN KEY REFERENCES ONLgroups(iid)," +
                                      "lead BIT NOT NULL DEFAULT 0," +
                                      "speed BIT NOT NULL DEFAULT 0," +
                                      "boulder BIT NOT NULL DEFAULT 0," +
                                      "climber_id INT NULL FOREIGN KEY REFERENCES ONLclimbers(iid) ON DELETE NO ACTION ON UPDATE NO ACTION, " +
                                      "op_id BIGINT NOT NULL FOREIGN KEY REFERENCES ONLoperations(iid) ON DELETE CASCADE ON UPDATE CASCADE," +
                                      "is_del BIT NOT NULL DEFAULT 0)";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "CREATE TABLE dbo.ONLrankings (" +
                        "climber VARCHAR(MAX) COLLATE Cyrillic_General_CI_AS NOT NULL," +
                        "age INT NOT NULL," +
                        "style VARCHAR(MAX) COLLATE Cyrillic_General_CI_AS NOT NULL," +
                        "group_id INT NOT NULL FOREIGN KEY REFERENCES ONLgroups(iid) ON DELETE CASCADE ON UPDATE CASCADE," +
                        "team_id INT NOT NULL FOREIGN KEY REFERENCES ONLteams(iid) ON DELETE CASCADE ON UPDATE CASCADE," +
                        "pos INT NOT NULL" +
                        ")";
                    cmd.ExecuteNonQuery();

                    AddAdmin(cmd.Connection, cmd.Transaction, newAdminPassword);

                    cmd.CommandText = @"
CREATE PROC dbo.InsertTeamGroup
  @iid INT,
  @name VARCHAR(255),
  @IsTeam BIT,
  @yO INT = 0,
  @yY INT = " + DateTime.Now.Year.ToString() + @",
  @gF BIT = 0,
  @minQf INT = NULL
AS BEGIN
  DECLARE @cn INT;
  IF @IsTeam = 0 BEGIN
    IF (@minQf IS NULL) BEGIN
      SELECT @minQf = ISNULL(MAX(iid),0) FROM ONLqfList(NOLOCK)
    END
    SELECT @cn = COUNT(*) FROM ONLgroups(NOLOCK) WHERE iid=@iid
    IF (@cn > 0) BEGIN
      UPDATE ONLgroups SET name = @name, oldYear = @yO, youngYear = @yY, genderFemale=@gF, minQf=@minQf WHERE iid = @iid
    END
    ELSE BEGIN
      INSERT INTO ONLgroups(iid, name, oldYear, youngYear,genderFemale,minQf) VALUES (@iid, @name, @yO, @yY,@gF,@minQf);
    END
  END
  ELSE BEGIN 
    IF (@yO IS NULL OR @yO < 1)
       SET @yO = " + int.MaxValue.ToString() + @"
    SELECT @cn = COUNT(*) FROM ONLteams(NOLOCK) WHERE iid=@iid
    IF (@cn > 0) BEGIN
      UPDATE ONLteams SET name = @name, pos=@yO WHERE iid = @iid
    END
    ELSE BEGIN
      INSERT INTO ONLteams(iid, name,pos) VALUES (@iid, @name, @yO);
    END
  END
END;";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"
CREATE PROC dbo.InsertPos
  @iid INT,
  @name VARCHAR(255)   
AS
  DECLARE @cn INT;
  SELECT @cn = COUNT(*) FROM ONLpositions(NOLOCK) WHERE iid=@iid
  IF (@cn > 0) BEGIN
    UPDATE ONLpositions SET name = @name WHERE iid = @iid
  END
  ELSE BEGIN
    INSERT INTO ONLpositions(iid, name) VALUES (@iid, @name);
  END;";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"
CREATE PROC dbo.InsertJudge
  @iid INT,
  @surname VARCHAR(255),
  @name VARCHAR(255),
  @patronimic VARCHAR(255),
  @city VARCHAR(255),
  @category VARCHAR(50),
  @photo IMAGE = NULL
AS
  DECLARE @cn INT;
  SELECT @cn = COUNT(*) FROM ONLjudges(NOLOCK) WHERE iid=@iid
  IF (@cn > 0) BEGIN
    UPDATE ONLjudges SET surname=@surname, name = @name, patronimic=@patronimic,
                         city=@city, category=@category, photo=@photo, photoLoaded=0 WHERE iid = @iid;
  END
  ELSE BEGIN
    INSERT INTO ONLjudges(iid, surname, name, patronimic, category, city, photo) VALUES 
                         (@iid, @surname, @name, @patronimic, @category, @city, @photo);
  END;";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"
CREATE PROC dbo.InsertJudgePos
  @iid INT,
  @judge_id INT,
  @pos_id INT
AS
  DECLARE @cn INT;
  SELECT @cn = COUNT(*) FROM ONLjudgePos(NOLOCK) WHERE iid=@iid
  IF (@cn > 0) BEGIN
    UPDATE ONLjudgePos SET judge_id=@judge_id, pos_id=@pos_id WHERE iid = @iid;
  END
  ELSE BEGIN
    INSERT INTO ONLjudgePos(iid, judge_id, pos_id) VALUES (@iid, @judge_id, @pos_id);
  END;";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"
CREATE PROC dbo.InsertClimber
  @iid INT,
  @surname VARCHAR(255),
  @name VARCHAR(255),
  @age INT,
  @qf VARCHAR(50),
  @team_id INT,
  @group_id INT,
  @lead BIT,
  @speed BIT,
  @boulder BIT,
  @rankingLead INT,
  @rankingSpeed INT,
  @rankingBoulder INT,
  @vk BIT,
  @nopoints BIT,
  @genderFemale BIT
AS
  DECLARE @cn INT;
  SELECT @cn = COUNT(*) FROM ONLclimbers(NOLOCK) WHERE iid=@iid
  IF (@cn > 0) BEGIN
    UPDATE ONLclimbers SET surname=@surname, name=@name, age=@age, qf=@qf, team_id=@team_id,
                        group_id=@group_id, rankingLead=@rankingLead, rankingSpeed=@rankingSpeed,
                        rankingBoulder=@rankingBoulder, vk=@vk, nopoints=@nopoints,genderFemale = @genderFemale,
                        lead=@lead, speed=@speed, boulder=@boulder
                  WHERE iid = @iid;
  END
  ELSE BEGIN
    INSERT INTO ONLclimbers(iid, surname, name, age, qf, team_id, group_id, lead, speed, boulder, rankingLead, rankingSpeed, rankingBoulder, vk, nopoints, genderFemale)
      VALUES(@iid, @surname, @name, @age, @qf, @team_id, @group_id, @lead, @speed, @boulder, @rankingLead, @rankingSpeed, @rankingBoulder, @vk, @nopoints, @genderFemale)
  END;";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"
CREATE PROC dbo.InsertList
  @list_id INT,
  @style VARCHAR(50),
  @group_id INT,
  @round VARCHAR(255),
  @routeNumber INT,
  @live BIT,
  @quote INT,
  @prevRound INT,
  @iid_parent INT,
  @start_time VARCHAR(255) = ''
AS
  IF @start_time IS NULL
  BEGIN
    SET @start_time = ''
  END
  DECLARE @cn INT;
  SELECT @cn = COUNT(*) FROM ONLlists(NOLOCK) WHERE iid=@list_id
  IF (@cn > 0) BEGIN
    UPDATE ONLlists SET style=@style, group_id=@group_id, round=@round, routeNumber=@routeNumber,
                        live=@live,quote=(CASE WHEN @quote IS NULL THEN 0 ELSE @quote END),
                        prevRound=@prevRound,iid_parent=@iid_parent,start_time=@start_time
                  WHERE iid = @list_id;
  END
  ELSE BEGIN
    INSERT INTO ONLlists(iid, style, group_id, round, routeNumber, live, quote, prevRound, iid_parent, start_time)
      VALUES(@list_id, @style, @group_id, @round, @routeNumber, @live, 
             (CASE WHEN @quote IS NULL THEN 0 ELSE @quote END), @prevRound, @iid_parent,@start_time);
  END;";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"
CREATE PROC dbo.InsertInitialData
  @list_id INT,
  @climber_id INT,
  @start INT,
  @resI BIGINT,
  @preQf BIT,
  @iid_line BIGINT OUTPUT,
  @dsc BIT = 1
AS
  BEGIN TRANSACTION
  DECLARE @cn INT;
  SELECT @cn = COUNT(*) FROM ONLlistdata(NOLOCK) WHERE iid=@list_id AND climber_id = @climber_id;
  IF (@cn > 0) BEGIN
    SELECT @iid_line = iid_line FROM ONLlistdata(NOLOCK) WHERE iid=@list_id AND climber_id=@climber_id;
  END
  ELSE BEGIN
    IF EXISTS(SELECT * FROM ONLlistdata(NOLOCK)) BEGIN
      SELECT @iid_line = ISNULL(MAX(iid_line) + 1, 1) FROM ONLlistdata(NOLOCK)
    END
    ELSE BEGIN
      SET @iid_line = 1
    END
    INSERT INTO ONLlistdata(iid, iid_line, climber_id) VALUES (@list_id, @iid_line, @climber_id);
  END;
  UPDATE ONLlistdata SET start = @start, res = @resI, preQf = @preQf WHERE iid_line = @iid_line;

  DECLARE @curRes BIGINT, @fetchRes BIGINT, @clmCnt INT, @curPos INT, @c INT

    SET @clmCnt = 1
    SET @curPos = 1
    IF (@dsc > 0) BEGIN
      DECLARE crs CURSOR FOR SELECT ISNULL(res," + long.MinValue.ToString() + @") resM, climber_id FROM ONLlistdata WHERE iid=@list_id ORDER BY resM DESC
    END
    ELSE BEGIN
      DECLARE crs CURSOR FOR SELECT ISNULL(res," + long.MaxValue.ToString() + @") resM, climber_id FROM ONLlistdata WHERE iid=@list_id ORDER BY resM
    END
    OPEN crs;
      FETCH NEXT FROM crs INTO @fetchRes, @climber_id;
      IF @@FETCH_STATUS = 0 BEGIN
         UPDATE ONLlistdata SET pos = @curPos WHERE iid=@list_id AND climber_id = @climber_id
         SET @curRes = @fetchRes
         FETCH NEXT FROM crs INTO @fetchRes, @climber_id;
         WHILE @@FETCH_STATUS = 0 BEGIN
           IF @fetchRes = @curRes BEGIN
             SET @clmCnt = @clmCnt + 1
           END
           ELSE BEGIN
             SET @curPos = @curPos + @clmCnt
             SET @clmCnt = 1
             SET @curRes = @fetchRes
           END
           UPDATE ONLlistdata SET pos = @curPos WHERE iid=@list_id AND climber_id = @climber_id
           FETCH NEXT FROM crs INTO @fetchRes, @climber_id;
         END
      END
    CLOSE crs;
    DEALLOCATE crs;
COMMIT TRANSACTION
";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"
CREATE PROC dbo.InsertLead
  @list_id INT,
  @climber_id INT,
  @start INT,
  @res VARCHAR(50),
  @resI BIGINT,
  @preQf BIT = 0
AS
  DECLARE @iid_line BIGINT;
  DECLARE @cn INT;

  EXECUTE InsertInitialData @list_id, @climber_id, @start, @resI, @preQf, @iid_line OUTPUT;
  
  SELECT @cn = COUNT(*) FROM ONLlead(NOLOCK) WHERE idPK = @iid_line;
  IF (@cn > 0) BEGIN
    UPDATE ONLlead SET res = @res WHERE idPK = @iid_line
  END
  ELSE BEGIN
    INSERT INTO ONLlead (idPK,iid_line, res) VALUES (@iid_line, @iid_line, @res)
  END";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"
CREATE PROC dbo.InsertSpeed
  @list_id INT,
  @climber_id INT,
  @start INT,
  @r1 VARCHAR(50),
  @r2 VARCHAR(50),
  @res VARCHAR(50),
  @resI BIGINT,
  @preQf BIT = 0,
  @qf VARCHAR(50) = ''
AS
  DECLARE @iid_line BIGINT;
  DECLARE @cn INT;

  EXECUTE InsertInitialData @list_id, @climber_id, @start, @resI, @preQf, @iid_line OUTPUT, 0;
  
  SELECT @cn = COUNT(*) FROM ONLspeed(NOLOCK) WHERE idPK = @iid_line;
  IF (@cn > 0) BEGIN
    UPDATE ONLspeed SET r1 = @r1, r2 = @r2, res = @res, qf=(ISNULL(@qf,'')) WHERE idPK = @iid_line
  END
  ELSE BEGIN
    INSERT INTO ONLspeed (idPK,iid_line, r1, r2, res, qf) VALUES (@iid_line, @iid_line, @r1, @r2, @res, ISNULL(@qf,''))
  END
  DECLARE @sF BIT
  SELECT @sF = CASE WHEN CHARINDEX('фин',round) > 0 AND style='Скорость' THEN 1 ELSE 0 END FROM ONLlists WHERE iid=@list_id;
  IF @sF > 0 BEGIN
    DECLARE @curRes BIGINT, @fetchRes BIGINT, @clmCnt INT, @curPos INT, @c INT, @lp INT, @curIt INT
    SET @lp = 0
    SET @curIt = 0
    WHILE @curIt < 2 BEGIN
      SET @curIt = @curIt + 1
      SET @clmCnt = 1
      SET @curPos = @lp + 1
      IF (@curIt > 1) BEGIN
        DECLARE crs CURSOR FOR SELECT ISNULL(l.res," + long.MaxValue.ToString() + @") resM, climber_id 
                                 FROM ONLlistdata l(NOLOCK)
                                 JOIN ONLspeed ll(NOLOCK) ON ll.iid_line = l.iid_line
                                WHERE l.iid=@list_id 
                                  AND ISNULL(ll.qf,'') = ''
                             ORDER BY resM
      END
      ELSE BEGIN
        DECLARE crs CURSOR FOR SELECT ISNULL(l.res," + long.MaxValue.ToString() + @") resM, climber_id 
                                 FROM ONLlistdata l(NOLOCK)
                                 JOIN ONLspeed ll(NOLOCK) ON ll.iid_line = l.iid_line
                                WHERE l.iid=@list_id 
                                  AND ISNULL(ll.qf,'') <> ''
                             ORDER BY resM
      END
      OPEN crs;
        FETCH NEXT FROM crs INTO @fetchRes, @climber_id;
        IF @@FETCH_STATUS = 0 BEGIN
           UPDATE ONLlistdata SET pos = @curPos WHERE iid=@list_id AND climber_id = @climber_id
           SET @curRes = @fetchRes
           FETCH NEXT FROM crs INTO @fetchRes, @climber_id;
           WHILE @@FETCH_STATUS = 0 BEGIN
             IF @fetchRes = @curRes BEGIN
               SET @clmCnt = @clmCnt + 1
             END
             ELSE BEGIN
               SET @curPos = @curPos + @clmCnt
               SET @clmCnt = 1
               SET @curRes = @fetchRes
             END
             UPDATE ONLlistdata SET pos = @curPos WHERE iid=@list_id AND climber_id = @climber_id
             FETCH NEXT FROM crs INTO @fetchRes, @climber_id;
           END
        END
      CLOSE crs;
      DEALLOCATE crs;
      SET @lp = @curPos
    END
  END
";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"
CREATE PROC dbo.InsertBoulder
  @list_id INT,
  @climber_id INT,
  @start INT,
  @T  INT,
  @Ta INT,
  @B  INT,
  @Ba INT,
  @nya BIT,
  @disq BIT,
  @resI BIGINT,
  @preQf BIT = 0
AS
  DECLARE @iid_line BIGINT;
  DECLARE @cn INT;

  EXECUTE InsertInitialData @list_id, @climber_id, @start, @resI, @preQf, @iid_line OUTPUT;
  
  SELECT @cn = COUNT(*) FROM ONLboulder(NOLOCK) WHERE idPK = @iid_line;
  IF (@cn > 0) BEGIN
    UPDATE ONLboulder SET T =@T , Ta=@Ta, B =@B , Ba=@Ba, nya=@nya, disq=@disq
                    WHERE idPK = @iid_line;
  END
  ELSE BEGIN
    INSERT INTO ONLboulder(idPK, iid_line, T, Ta, B, Ba, nya, disq) 
                  VALUES (@iid_line, @iid_line, @T , @Ta, @B , @Ba, @nya, @disq);
  END";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = @"
 CREATE PROC dbo.InsertBoulderRoute
   @list_id INT,
   @climber_id INT,
   @routeN INT,
   @topA INT,
   @bonusA INT
 AS BEGIN
   DECLARE @iid_parent BIGINT

   SELECT @iid_parent = b.idPK
     FROM ONLBoulder b(NOLOCK)
     JOIN ONLlistData ld(NOLOCK) ON ld.iid_line = b.iid_line
    WHERE ld.iid = @list_id
      AND ld.climber_id = @climber_id;
   BEGIN TRAN
     IF @iid_parent IS NOT NULL BEGIN
       IF ((@topA IS NULL) AND (@bonusA IS NULL)) BEGIN
         DELETE FROM ONLboulderRoutes WHERE iid_parent = @iid_parent AND routeN = @routeN;
       END
       ELSE BEGIN
         DECLARE @idN BIGINT
         SELECT @idN = iid_line FROM ONLboulderRoutes WHERE iid_parent = @iid_parent AND routeN = @routeN;
         IF @idN IS NULL BEGIN
           SELECT @idN = (ISNULL(MAX(iid_line),0)+1) FROM ONLboulderRoutes;
           INSERT INTO ONLboulderRoutes(iid_line, iid_parent, routeN, topA, bonusA)
                                 VALUES(@idN, @iid_parent, @routeN, @topA, @bonusA);
         END
         ELSE BEGIN
           UPDATE ONLboulderRoutes SET topA = @topA, bonusA = @bonusA
                                 WHERE iid_line = @idN;
         END
       END
     END;
   COMMIT TRAN
 END";
                    cmd.ExecuteNonQuery();
                    cmd.Transaction.Commit();
                }
                catch (Exception ex)
                {
                    if (cmd.Transaction != null)
                        try { cmd.Transaction.Rollback(); }
                        catch { }
                    throw ex;
                }
                remote.Close();
                MessageBox.Show("База данных успешно создана.");
            }
            catch (SqlException ex)
            { MessageBox.Show("Ошибка создания базы данных.\r\n\r\nДанные ошибки:\r\n" + ex.Message); }
        }

        private void rbCString_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Enabled = rbCString.Checked || rbMVC.Checked;
            groupBox1.Enabled = rbServer.Checked;
            gbUseWebServices.Enabled = rbUseService.Checked || rbMVC.Checked;
            gbRemoteDBServices.Enabled = !rbUseService.Checked;
            if (rbMVC.Checked)
            {
                label5.Text = "Год проведения соревнований:";
                tbAdminPasswordService.PasswordChar = default(char);
            }
            else if (rbUseService.Checked)
            {
                label5.Text = "Пароль администратора:";
                tbAdminPasswordService.PasswordChar = '*';
            }
        }

        private void rbUseWin_CheckedChanged(object sender, EventArgs e)
        {
            tbUser.Enabled = tbPassword.Enabled = !rbUseWin.Checked;
        }

        private void FillDb()
        {
            if (AllowServerData())
                return;
            SqlConnection cnL;
            if (rbUseSQL.Checked)
                cnL = AccountForm.CreateConnection(tbServer.Text, tbUser.Text, tbPassword.Text, "master");
            else
                cnL = AccountForm.CreateConnection(tbServer.Text, "master");
            try
            {
                cnL.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cnL;
                cmd.CommandText = "SELECT name FROM sysdatabases(NOLOCK) ORDER BY name";
                SqlDataReader rdr = cmd.ExecuteReader();
                try
                {
                    List<string> ls = new List<string>();
                    while (rdr.Read())
                        ls.Add(rdr["name"].ToString());
                    cbDataBase.Invoke(new EventHandler(delegate
                    {
                        cbDataBase.Items.Clear();
                        foreach (string str in ls)
                            cbDataBase.Items.Add(str);
                    }));
                }
                finally { rdr.Close(); }
            }
            catch (Exception ex)
            {
                this.Invoke(new EventHandler(delegate
                {
                    MessageBox.Show("Ошибка загрузки списка баз данных\r\n" + ex.Message);
                }));
            }
            finally
            {
                try { cnL.Close(); }
                catch { }
            }
        }

        private bool AllowServerData()
        {
            return (tbServer.Text.Length < 1 || rbUseSQL.Checked && (tbPassword.Text.Length * tbUser.Text.Length == 0));
        }

        private void cbDataBase_DropDown(object sender, EventArgs e)
        {
            StaticClass.DoSimpleWork(FillDb);
        }

        private bool SetCString()
        {
            bool bRes = false;
            if (rbServer.Checked)
            {
                if (AllowServerData() || cbDataBase.Text.Length < 1)
                    bRes = false;
                else
                {
                    if (rbUseSQL.Checked)
                        textBox1.Text = AccountForm.CreateConnection(tbServer.Text, tbUser.Text, tbPassword.Text, cbDataBase.Text).ConnectionString;
                    else
                        textBox1.Text = AccountForm.CreateConnection(tbServer.Text, cbDataBase.Text).ConnectionString;
                    bRes = true;
                }
            }
            else if (rbCString.Checked)
            {
                if (textBox1.Text.Length < 1)
                    bRes = (MessageBox.Show(this, "Вы уверены, что хотите удалить данные для онлайн-трансляции?",
                        "Удалить строку соединения?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                         DialogResult.Yes);
                else
                    bRes = true;
            }
            else if (rbMVC.Checked)
            {
                if (String.IsNullOrEmpty(textBox1.Text))
                {
                    MessageBox.Show(this, "Не указан адрес службы");
                    bRes = false;
                }
                else
                {
                    XmlClientWrapper cWrapper = GetClientWrapper(textBox1.Text);
                    if (cWrapper == null)
                        cWrapper = new XmlClientWrapper { CompetitionId = 0, CompYear = 0, Uri = textBox1.Text.Trim() };
                    CompetitionApiModel model = cbCompSelect.SelectedItem as CompetitionApiModel;
                    if (model != null)
                    {
                        cWrapper.CompetitionId = model.Iid;
                        cWrapper.CompYear = model.DateStart.Year;
                    }
                    if (cWrapper.CompetitionId < 1)
                    {
                        MessageBox.Show(this, "Не выбраны соревнования");
                        bRes = false;
                    }
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        using (XmlWriter wr = XmlWriter.Create(sb))
                        {
                            StaticClass.ClientSerializer.Serialize(wr, cWrapper);
                        }
                        textBox1.Text = sb.ToString();
                        bRes = true;
                    }
                }
            }
            else
            {
                StaticClass.WebPasswordEntered = false;
                if (String.IsNullOrEmpty(tbAdminPasswordService.Text))
                {
                    MessageBox.Show(this, "Не введён пароль администратора");
                    return false;
                }
                if (cbCompSelect.SelectedIndex >= 0 && cbCompSelect.SelectedItem != null)
                {
                    string[] resP;
                    try
                    {
                        resP = (cbCompSelect.SelectedItem as string).Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
                        long lInd = long.Parse(resP[0]);

                        byte[] pwd = PasswordWorkingClass.EncryptPassword(tbAdminPasswordService.Text);
                        ClimbingService service = new ClimbingService();
                        if (!service.ValidateAdminPassword(pwd, lInd))
                        {
                            MessageBox.Show(this, "Пароль администратора введён неверно");
                            return false;
                        }

                        textBox1.Text = "SERV=" + lInd.ToString();
                        StaticClass.WebPasswordEntered = true;
                        bRes = true;
                    }
                    catch { }
                }
            }
            if (!bRes)
                MessageBox.Show("Введены не все данные о соединении");
            return bRes;
        }

        private static XmlClientWrapper GetClientWrapper(String str)
        {
            XmlClientWrapper cWrapper = null;

            try
            {
                using (var rdr = XmlReader.Create(new StringReader(str)))
                {
                    if (StaticClass.ClientSerializer.CanDeserialize(rdr))
                        cWrapper = StaticClass.ClientSerializer.Deserialize(rdr) as XmlClientWrapper;
                }
            }
            catch (XmlException) { }
            catch (NullReferenceException) { }
            return cWrapper;
        }

        private void btnChangeAdmPassword_Click(object sender, EventArgs e)
        {
            bool needToClose = false;
            try
            {
                if (remote.State != ConnectionState.Open)
                {
                    remote.Open();
                    needToClose = true;
                }
                string newPassword;
                if (!SetAdminPassword.CreateNewPassword(this, out newPassword))
                    return;
                
                AddAdmin(remote, null, newPassword);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Ошибка изменения пароля:\r\n" +
                    ex.ToString());
            }
            finally
            {
                try
                {
                    if (needToClose)
                        remote.Close();
                }
                catch { }
            }
        }

        private bool inEventLoadCompList = false;
        private void cbCompSelect_DropDown(object sender, EventArgs e)
        {
            if (inEventLoadCompList)
                return;
            if (rbMVC.Checked)
            {
                if (String.IsNullOrEmpty(textBox1.Text))
                {
                    MessageBox.Show(this, "Введите адрес сервиса");
                    return;
                }
                int compYear;
                if (!int.TryParse(tbAdminPasswordService.Text, out compYear))
                {
                    MessageBox.Show(this, "Введите год");
                    return;
                }
                cbCompSelect.Items.Clear();
                XmlClientWrapper cWrapper = null;
                try
                {
                    using (var rdr = XmlReader.Create(new StringReader(textBox1.Text)))
                    {
                        if (StaticClass.ClientSerializer.CanDeserialize(rdr))
                            cWrapper = StaticClass.ClientSerializer.Deserialize(rdr) as XmlClientWrapper;
                    }
                }
                catch (XmlException) { }
                catch (NullReferenceException) { }
                if (cWrapper == null)
                    cWrapper = new XmlClientWrapper { Uri = textBox1.Text, CompYear = compYear }; ;
                try
                {
                    XmlClient cl = new XmlClient(cWrapper.Uri, 0, null, String.Empty) { Proxy = null, Timeout = 60000 };
                    var compCollection = cl.GetCompetitions(cWrapper.CompYear);
                    foreach (var comp in compCollection.Data)
                        cbCompSelect.Items.Add(comp);
                    textBox1.Text = cl.BaseUrl.ToString();
                }
                catch (UriFormatException)
                {
                    MessageBox.Show(this, "Адресная строка имеет неверный формат");
                    return;
                }
                catch (WebException ex)
                {
                    MessageBox.Show(String.Format("Ошибка доступа:{0}{1}", Environment.NewLine, ex.Message));
                }
            }
            else
                StaticClass.DoSimpleWork(LoadCompList);
        }
        private void LoadCompList()
        {
            try
            {
                inEventLoadCompList = true;
                if (cbCompSelect.InvokeRequired)
                    cbCompSelect.Invoke(new EventHandler(delegate { cbCompSelect.Items.Clear(); }));
                else
                    cbCompSelect.Items.Clear();
                ClimbingService serv = new ClimbingService();
                var vList = serv.GetAllCompetitions();
                foreach (var v in vList)
                {
                    string sToEnter = v.Competition.iid + " - " + v.Competition.short_name;
                    if (cbCompSelect.InvokeRequired)
                        cbCompSelect.Invoke(new EventHandler(delegate
                        {
                            if (!cbCompSelect.DroppedDown)
                                cbCompSelect.DroppedDown = true;
                            cbCompSelect.Items.Add(sToEnter);
                        }));
                    else
                    {
                        if (!cbCompSelect.DroppedDown)
                            cbCompSelect.DroppedDown = true;
                        cbCompSelect.Items.Add(sToEnter);
                    }
                }
            }
            catch (Exception ex)
            {
                if (this.InvokeRequired)
                    this.Invoke(new EventHandler(delegate
                    {
                        MessageBox.Show(this, "Ошибка загрузки спика соревнований:\r\n" + ex.ToString(), "Ошибка", MessageBoxButtons.OK,
                             MessageBoxIcon.Error);
                    }));
                else
                    MessageBox.Show(this, "Ошибка загрузки спика соревнований:\r\n" + ex.ToString(), "Ошибка", MessageBoxButtons.OK,
                             MessageBoxIcon.Error);
            }
            finally { inEventLoadCompList = false; }
        }

        private void btnSetAuth_Click(object sender, EventArgs e)
        {
            SecurityOperations.SetSecurityData(cn, this);
        }
    }
}