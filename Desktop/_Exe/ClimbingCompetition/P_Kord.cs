// <copyright file="P_Kord.cs">
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

﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Globalization;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace ClimbingCompetition
{
    public partial class P_Kord : BaseForm
    {
        private static class NumByWords
        {
            //public static string RurPhrase(decimal money)
            //{
            //    return CurPhrase(money, "рубль", "рубля", "рублей", "копейка", "копейки", "копеек");
            //}

            //public static string UsdPhrase(decimal money)
            //{
            //    return CurPhrase(money, "доллар США", "доллара США", "долларов США", "цент", "цента", "центов");
            //}

            public static string NumPhrase(ulong Value, bool IsMale)
            {
                if (Value == 0UL) return "Ноль";
                string[] Dek1 = { "", " од", " дв", " три", " четыре", " пять", " шесть", " семь", " восемь", " девять", " десять", " одиннадцать", " двенадцать", " тринадцать", " четырнадцать", " пятнадцать", " шестнадцать", " семнадцать", " восемнадцать", " девятнадцать" };
                string[] Dek2 = { "", "", " двадцать", " тридцать", " сорок", " пятьдесят", " шестьдесят", " семьдесят", " восемьдесят", " девяносто" };
                string[] Dek3 = { "", " сто", " двести", " триста", " четыреста", " пятьсот", " шестьсот", " семьсот", " восемьсот", " девятьсот" };
                string[] Th = { "", "", " тысяч", " миллион", " миллиард", " триллион", " квадрилион", " квинтилион" };
                string str = "";
                for (byte th = 1; Value > 0; th++)
                {
                    ushort gr = (ushort)(Value % 1000);
                    Value = (Value - gr) / 1000;
                    if (gr > 0)
                    {
                        byte d3 = (byte)((gr - gr % 100) / 100);
                        byte d1 = (byte)(gr % 10);
                        byte d2 = (byte)((gr - d3 * 100 - d1) / 10);
                        if (d2 == 1) d1 += (byte)10;
                        bool ismale = (th > 2) || ((th == 1) && IsMale);
                        str = Dek3[d3] + Dek2[d2] + Dek1[d1] + EndDek1(d1, ismale) + Th[th] + EndTh(th, d1) + str;
                    };
                };
                str = str.Substring(1, 1).ToUpper() + str.Substring(2);
                return str;
            }

            #region Private members
            private static string CurPhrase(decimal money,
                string word1, string word234, string wordmore,
                string sword1, string sword234, string swordmore)
            {
                money = decimal.Round(money, 2);
                decimal decintpart = decimal.Truncate(money);
                ulong intpart = decimal.ToUInt64(decintpart);
                string str = NumPhrase(intpart, true) + " ";
                byte endpart = (byte)(intpart % 100UL);
                if (endpart > 19) endpart = (byte)(endpart % 10);
                switch (endpart)
                {
                    case 1: str += word1; break;
                    case 2:
                    case 3:
                    case 4: str += word234; break;
                    default: str += wordmore; break;
                }
                byte fracpart = decimal.ToByte((money - decintpart) * 100M);
                str += " " + ((fracpart < 10) ? "0" : "") + fracpart.ToString() + " ";
                if (fracpart > 19) fracpart = (byte)(fracpart % 10);
                switch (fracpart)
                {
                    case 1: str += sword1; break;
                    case 2:
                    case 3:
                    case 4: str += sword234; break;
                    default: str += swordmore; break;
                };
                return str;
            }
            private static string EndTh(byte ThNum, byte Dek)
            {
                bool In234 = ((Dek >= 2) && (Dek <= 4));
                bool More4 = ((Dek > 4) || (Dek == 0));
                if (((ThNum > 2) && In234) || ((ThNum == 2) && (Dek == 1))) return "а";
                else if ((ThNum > 2) && More4) return "ов";
                else if ((ThNum == 2) && In234) return "и";
                else return "";
            }
            private static string EndDek1(byte Dek, bool IsMale)
            {
                if ((Dek > 2) || (Dek == 0)) return "";
                else if (Dek == 1)
                {
                    if (IsMale) return "ин";
                    else return "на";
                }
                else
                {
                    if (IsMale) return "а";
                    else return "е";
                }
            }
            #endregion
        }

        #region static_methods
        public static T GetOrderParam<T>(string paramId, SqlConnection cn, SqlTransaction tran = null)
            where T : class
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.Transaction = tran;

            cmd.CommandText = "SELECT param_value" +
                              "  FROM dbo.order_params(nolock)" +
                              " WHERE param_id = @pid";
            cmd.Parameters.Add("@pid", SqlDbType.VarChar, 255);
            cmd.Parameters[0].Value = paramId;

            object o = cmd.ExecuteScalar();
            if (o == null || o == DBNull.Value)
                return null;
            try { return (T)Convert.ChangeType(o, typeof(T)); }
            catch (InvalidCastException) { return null; }
            catch (FormatException) { return null; }
        }

        public static Nullable<T> GetOrderNullableParam<T>(string paramId, SqlConnection cn, SqlTransaction tran = null)
            where T : struct
        {
            try
            {
                string s = GetOrderParam<string>(paramId, cn, tran);
                if (String.IsNullOrEmpty(s))
                    return null;
                if (typeof(T).Equals(typeof(DateTime)))
                {
                    DateTime dt = DateTime.Parse(s, DEFAULT_PARSE);
                    return new Nullable<T>((T)Convert.ChangeType(dt, typeof(T)));
                }
                else
                    try
                    {
                        return new Nullable<T>((T)Convert.ChangeType(s, typeof(T)));
                    }
                    catch (FormatException)
                    {
                        s = s.Replace(',', '.').Trim();
                        try { return new Nullable<T>((T)Convert.ChangeType(s, typeof(T))); }
                        catch (FormatException)
                        {
                            s = s.Replace('.', ',');
                            return new Nullable<T>((T)Convert.ChangeType(s, typeof(T)));
                        }
                    }
            }
            catch { return null; }
        }

        private static SqlCommand CheckParamExists(string paramId, SqlConnection cn, SqlTransaction tran = null)
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.Transaction = tran;

            cmd.CommandText = @"
IF NOT EXISTS(select 1
            from dbo.order_params(nolock)
           where param_id = @pid) begin
  INSERT INTO dbo.order_params(param_id, param_value) VALUES (@pid, '')
end";
            cmd.Parameters.Add("@pid", SqlDbType.VarChar, 255);
            cmd.Parameters[0].Value = paramId;
            cmd.ExecuteNonQuery();
            cmd.CommandText = String.Empty;
            return cmd;
        }

        public static void SaveStringParam(string param_Id, string param_value, SqlConnection cn, SqlTransaction tran = null)
        {
            var cmd = CheckParamExists(param_Id, cn, tran);
            cmd.CommandText = "UPDATE dbo.order_params SET param_value = @v WHERE param_id = @pid";
            cmd.Parameters.Add("@v", SqlDbType.VarChar, 4096);
            if (param_value == null)
                cmd.Parameters[1].Value = DBNull.Value;
            else
                cmd.Parameters[1].Value = param_value;
            cmd.ExecuteNonQuery();
        }

        private static readonly CultureInfo DEFAULT_PARSE = new CultureInfo("en-US");

        public static void SaveNumericParam<T>(string paramId, Nullable<T> value, SqlConnection cn, SqlTransaction tran = null)
            where T : struct
        {
            string val;
            if (value == null || !value.HasValue)
                val = null;
            else
            {
                if (typeof(T).Equals(typeof(DateTime)))
                {
                    DateTime dt = (value as DateTime?).Value;
                    val = dt.ToString("dd-MMM-yyyy", DEFAULT_PARSE);
                }
                else
                    val = value.Value.ToString().Replace(',', '.');
            }
            SaveStringParam(paramId, val, cn, tran);
        }

        public static string DoubleToVerbal(double d, out int kop)
        {
            if (d < 0)
            {
                kop = 0;
                return String.Empty;
            }
            ulong n = (ulong)Math.Floor(d);
            kop = (int)((d - (double)n) * 100.0);
            return NumByWords.NumPhrase(n, true);
        }

        #endregion

        #region Defines
        public const string PARAM_ORG = "ORG";
        public const string PARAM_AMOUNT1 = "AMOUNT_1";
        public const string PARAM_AMOUNT2 = "AMOUNT_2";
        public const string PARAM_AMOUNT3 = "AMOUNT_3";
        public const string PARAM_AMOUNT_DNS = "AMOUNT_DNS";
        public const string PARAM_NAIM = "NAIM";
        public const string PARAM_FROM = "ORD_FROM";
        public const string PARAM_DATE = "ORD_DATE";
        public const string PARAM_FOLDER = "ORD_FOLDER";
        public const string PARAM_CONTR = "GL_BUH";
        public const string PARAM_CASHIER = "CASHIER";
        public const string PARAM_USECLIMBER = "USE_CLIMBER";

        public const string PART_RP = "&УчР";

        private const string Where1 = @" AND(
 p.lead > 0 AND p.speed = 0 and p.boulder = 0
OR p.lead = 0 AND p.speed > 0 and p.boulder = 0
OR p.lead = 0 AND p.speed = 0 and p.boulder > 0
) ";
        private const string Where2 = @" AND(
p.lead > 0 AND p.speed > 0 and p.boulder = 0
OR p.lead > 0 AND p.speed = 0 and p.boulder > 0
OR p.lead = 0 AND p.speed > 0 and p.boulder > 0
) ";

        private const string Where3 = @" AND(
p.lead > 0 AND p.speed > 0 and p.boulder > 0
) ";
        #endregion

        readonly int teamID;
        readonly string teamName;
        readonly string teamChief;
        readonly string teamChiefRP;

        public P_Kord(SqlConnection baseCn, string competitionName, int selectedTeam)
            : base(baseCn, competitionName, null)
        {
            InitializeComponent();
            teamID = selectedTeam;
            if (cn.State != ConnectionState.Open)
                cn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT name, chief, chief_ord" +
                              "  FROM Teams(nolock)" +
                              " WHERE iid = " + teamID.ToString();
            using (var rdr = cmd.ExecuteReader())
            {
                if (rdr.Read())
                {
                    teamName = rdr["name"].ToString().Trim();
                    teamChief = rdr["chief"].ToString().Trim();
                    teamChiefRP = rdr["chief_ord"].ToString().Trim();
                }
                else
                    teamName = teamChief = teamChiefRP = String.Empty;
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            string sExcelMessage;
            var excels = Process.GetProcessesByName("EXCEL");
            if (excels != null && excels.Length > 0)
            {
                sExcelMessage = "\r\nВНИМАНИЕ!!! У вас запущено " + excels.Length.ToString() + " экземпляр(-а,-ов) Microsoft Excel.\r\n" +
                    "Все они будут принудительно закрыты.\r\n" +
                    "Пожалуйста, сохраните ваши данные.\r\n" +
                    "Продолжить?";
                foreach (var p in excels)
                    p.Kill();
            }
            else
                sExcelMessage = String.Empty;
            if (MessageBox.Show(this, "Сформировать ордера?" + sExcelMessage,String.Empty, MessageBoxButtons.YesNo) ==
                System.Windows.Forms.DialogResult.No)
                return;
            try
            {
                if (!SaveOrderParams())
                    return;
            }
            catch (Exception ex1)
            {
                if (MessageBox.Show(this, "Ошибка сохранения параметров:\r\n" +
                    ex1.Message + "\r\n\r\nПродолжить?", String.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Error)
                    == System.Windows.Forms.DialogResult.No)
                    return;
            }
#if !DEBUG
            try
            {
#endif
                if (CreateOrder())
                    this.Close();
#if !DEBUG
            }
            catch (Exception ex) { MessageBox.Show(this, "Ошибка печати:\r\n" + ex.Message); }
#endif
        }

        bool closngCancelled = false;
        private void btnCancel_Click(object sender, EventArgs e)
        {
            var dg = MessageBox.Show(this, "Отменить печать ордеров?", String.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dg == System.Windows.Forms.DialogResult.No || dg == System.Windows.Forms.DialogResult.Cancel)
                closngCancelled = true;
            else
            {
                closngCancelled = false;
                this.Close();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (closngCancelled)
            {
                e.Cancel = true;
                closngCancelled = false;
            }
            base.OnClosing(e);
        }

        private void SetDoubleParam(string paramId, TextBox tb)
        {
            double? val = GetOrderNullableParam<double>(paramId, cn);
            tb.Text = (val == null || !val.HasValue) ? String.Empty : val.ToString();
        }

        private void SetStringParam(string paramId, TextBox tb)
        {
            string val = GetOrderParam<string>(paramId, cn);
            tb.Text = String.IsNullOrEmpty(val) ? String.Empty : val;
        }

        private bool GetDoubleValue(string tbLabel, TextBox tb, out double? val)
        {
            tb.Text = tb.Text.Trim();
            if (String.IsNullOrEmpty(tb.Text))
                val = null;
            else
            {
                double d;
                if (!double.TryParse(tb.Text, out d))
                {
                    tb.Text = tb.Text.Replace('.', ',');
                    if (!double.TryParse(tb.Text, out d))
                    {
                        tb.Text = tb.Text.Replace(',', '.');
                        if (!double.TryParse(tb.Text, out d))
                        {
                            MessageBox.Show("Параметр \"" + tbLabel + "\" введен не верно");
                            val = null;
                            return false;
                        }
                    }
                }
                val = new double?(d);
            }
            return true;
        }

        private bool SaveDoubleParam(string paramId, string tbLabel, TextBox tb, SqlTransaction tran)
        {
            double? val;
            if (!GetDoubleValue(tbLabel, tb, out val))
                return false;
            SaveNumericParam<double>(paramId, val, cn, tran);
            return true;
        }

        private void SaveStringParam(string paramId, TextBox tb, SqlTransaction tran)
        {
            SaveStringParam(paramId, tb.Text, cn, tran);
        }

        private void P_Kord_Load(object sender, EventArgs e)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            try
            {
                if (cn.State != ConnectionState.Open)
                    cn.Open();
                cmd.CommandText = "SELECT name FROM teams(NOLOCK) WHERE iid = " + teamID.ToString();
                var obj = cmd.ExecuteScalar();
                string teamName = (obj == null || obj == DBNull.Value) ? String.Empty : (string)obj;
                this.Text = "Ордера для " + teamName;
                AccountForm.CreateOrderTables(cn, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка создания таблицы ордеров:\r\n" + ex.Message);
                this.Close();
            }

            try
            {
                SetStringParam(PARAM_ORG, tbOrg);
                SetDoubleParam(PARAM_AMOUNT1, tbAmount1);
                SetDoubleParam(PARAM_AMOUNT2, tbAmount2);
                SetDoubleParam(PARAM_AMOUNT3, tbAmount3);
                SetStringParam(PARAM_NAIM, tbNaim);
                SetStringParam(PARAM_FROM, tbFrom);
                SetStringParam(PARAM_CASHIER, tbCashier);
                SetStringParam(PARAM_CONTR, tbContr);
                SetStringParam(PARAM_AMOUNT_DNS, tbAmountDNS);

                DateTime? dt = GetOrderNullableParam<DateTime>(PARAM_DATE, cn);
                dateToPrint.Value = (dt != null && dt.HasValue) ? dt.Value.Date : DateTime.Now.Date;

                int? useClimber = GetOrderNullableParam<int>(PARAM_USECLIMBER, cn);

                rbClimber.Checked = useClimber != null && useClimber.HasValue && useClimber.Value > 0;
                rbTeam.Checked = !rbClimber.Checked;


                ClimbingCompetition.Properties.Settings aSet = ClimbingCompetition.Properties.Settings.Default;
                lblDir.Text = String.IsNullOrEmpty(aSet.OrderFolder) ? String.Empty : aSet.OrderFolder.Trim();
                if(!String.IsNullOrEmpty(lblDir.Text))
                    try { folderDialog.SelectedPath = lblDir.Text; }
                    catch { }
            }
            catch (Exception ex) { MessageBox.Show("Ошибка загрузки данных:\r\n" + ex.Message); }
        }

        private bool SaveOrderParams()
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            SqlTransaction tran = cn.BeginTransaction();
            bool tranSucess = false;
            try
            {
                SaveStringParam(PARAM_ORG, tbOrg, tran);
                if (!SaveDoubleParam(PARAM_AMOUNT1, "Ст.Взнос за 1 вид", tbAmount1, tran))
                    return false;
                if (!SaveDoubleParam(PARAM_AMOUNT2, "Ст.Взнос за 2 вида", tbAmount2, tran))
                    return false;
                if (!SaveDoubleParam(PARAM_AMOUNT3, "Ст.Взнос за 3 вида", tbAmount3, tran))
                    return false;
                if (!SaveDoubleParam(PARAM_AMOUNT_DNS, "Ст.Взнос за неявки", tbAmountDNS, tran))
                    return false;
                SaveStringParam(PARAM_FROM, tbFrom, tran);
                SaveStringParam(PARAM_NAIM, tbNaim, tran);
                SaveStringParam(PARAM_CASHIER, tbCashier, tran);
                SaveStringParam(PARAM_CONTR, tbContr, tran);
                ClimbingCompetition.Properties.Settings aSet = ClimbingCompetition.Properties.Settings.Default;
                aSet.OrderFolder = lblDir.Text;
                aSet.Save();
                SaveNumericParam<DateTime>(PARAM_DATE, dateToPrint.Value.Date, cn, tran);
                SaveNumericParam<int>(PARAM_USECLIMBER, new int?(rbClimber.Checked ? 1 : 0), cn, tran);
                tranSucess = true;
                return true;
            }
            finally
            {
                if (tranSucess)
                    tran.Commit();
                else
                    tran.Rollback();
            }
        }

        private int GetCountWhere(string sWhere)
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT COUNT(*) cnt" +
                              "  FROM Participants P(nolock)" +
                              " WHERE p.team_id = " + teamID.ToString() + sWhere + SClimberWhere;
            object o = cmd.ExecuteScalar();
            if (o == null || o == DBNull.Value)
                return 0;
            try { return Convert.ToInt32(o); }
            catch { return 0; }
        }

        private void btnCalc_Click(object sender, EventArgs e)
        {
            double dPrice = 0.0, dTmp;
            if (rbParticipantsReal.Checked)
            {
                int nTmp = GetCountWhere(Where1);
                if (nTmp > 0)
                {
                    if (GetBoolPrice("Ст.Взнос за 1 вид", tbAmount1, out dTmp))
                        dPrice += dTmp * (double)nTmp;
                    else
                        return;
                }

                nTmp = GetCountWhere(Where2);
                if (nTmp > 0)
                {
                    if (GetBoolPrice("Ст.Взнос за 2 вида", tbAmount2, out dTmp))
                        dPrice += dTmp * (double)nTmp;
                    else
                        return;
                }

                nTmp = GetCountWhere(Where3);
                if (nTmp > 0)
                {
                    if (GetBoolPrice("Ст.Взнос за 3 вида", tbAmount3, out dTmp))
                        dPrice += dTmp * (double)nTmp;
                    else
                        return;
                }
            }
            else
            {
                int nNya = GetCountWhere(String.Empty);
                if (nNya > 0)
                {
                    if (GetBoolPrice("Ст.Взнос за неявки", tbAmountDNS, out dTmp))
                        dPrice += dTmp * (double)nNya;
                    else
                        return;
                }
            }
            tbAmountTeam.Text = dPrice.ToString("0.00");
        }

        private bool GetBoolPrice(string sMessage, TextBox tb, out double d2)
        {
            d2 = default(double);
            double? dTmp;
            if (GetDoubleValue(sMessage, tb, out dTmp))
            {
                if (dTmp == null || !dTmp.HasValue)
                {
                    MessageBox.Show(this, "\"" + sMessage + "\" не введен");
                    return false;
                }
            }
            else
                return false;
            d2 = dTmp.Value;
            return true;
        }

        private string CalculateNaim(string _naim, int? climberId, SqlTransaction tran)
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.Transaction = tran;
            string naim = (string)_naim.Clone(), sRepl;
            if (naim.IndexOf(PART_RP) >= 0)
            {
                cmd.CommandText = "SELECT LTRIM(RTRIM(CASE p.name_ord WHEN '' THEN p.surname + ' ' + p.name ELSE p.name_ord END)) n_ord" +
                                      "  FROM Participants P(nolock) WHERE ";
                if (climberId == null || !climberId.HasValue)
                {
                    cmd.CommandText += " p.team_id = " + teamID.ToString() + " ORDER BY n_ord";
                    sRepl = String.Empty;
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            if (!String.IsNullOrEmpty(sRepl))
                                sRepl += ", ";
                            sRepl += dr["n_ord"].ToString();
                        }
                    }
                }
                else
                {
                    cmd.CommandText += " p.iid = " + climberId.Value.ToString();
                    sRepl = cmd.ExecuteScalar() as string;
                    if (sRepl == null)
                        sRepl = String.Empty;
                    else
                        sRepl = sRepl.Trim();
                }
                naim = naim.Replace(PART_RP, sRepl);
            }

            if (naim.IndexOf("&Учк") >= 0)
            {
                cmd.CommandText = "SELECT LTRIM(RTRIM(p.surname + ' ' + p.name)) n_ord" +
                                      "  FROM Participants P(nolock) WHERE ";
                if (climberId == null || !climberId.HasValue)
                {
                    sRepl = string.Empty;
                    cmd.CommandText += " p.team_id = " + teamID.ToString() + " ORDER BY n_ord";
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            if (!String.IsNullOrEmpty(sRepl))
                                sRepl += ", ";
                            sRepl += dr["n_ord"].ToString();
                        }
                    }
                }
                else
                {
                    cmd.CommandText += " p.iid = " + climberId.Value.ToString();
                    sRepl = cmd.ExecuteScalar() as string;
                    if (sRepl == null)
                        sRepl = String.Empty;
                    else
                        sRepl = sRepl.Trim();
                }
                naim = naim.Replace("&Учк", sRepl);
            }

            if (naim.IndexOf("&Сор") > -1 || naim.IndexOf("&КрСор") > -1)
            {
                string sCompName, sCompShortName;
                cmd.CommandText = "SELECT NAME, SHORT_NAME FROM CompetitionData(NOLOCK)";
                using (var rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        sCompName = rdr["NAME"].ToString().Trim();
                        sCompShortName = rdr["SHORT_NAME"].ToString().Trim();
                    }
                    else
                        sCompName = sCompShortName = String.Empty;
                }
                naim = naim.Replace("&Сор", sCompName).Replace("&КрСор", sCompShortName);
            }

            if (naim.IndexOf("&Ком") >= 0)
                naim = naim.Replace("&Ком", teamName);

            if (naim.IndexOf("&ПредК") >= 0)
                naim = naim.Replace("&ПредК", teamChief);

            if (naim.IndexOf("&ПредР") >= 0)
                naim = naim.Replace("&ПредР", String.IsNullOrEmpty(teamChiefRP) ? teamChief : teamChiefRP);

            return naim;
        }

        private const int MAXLEN = 240;
        private static void ReplaceLabel(string label, string toReplace, Excel.Worksheet ws)
        {
            string sToRepl, sSrc = toReplace, sLbl = "$[" + label + "]";
            int curLen = MAXLEN - sLbl.Length;
            while (!String.IsNullOrEmpty(sSrc))
            {
                if (sSrc.Length <= curLen)
                {
                    sToRepl = sSrc;
                    sSrc = String.Empty;
                }
                else
                {
                    sToRepl = sSrc.Substring(0, curLen) + sLbl;
                    sSrc = sSrc.Substring(curLen);
                }
                ws.Range["A1"].Select();
                ws.Cells.Replace(
                    What: sLbl,
                    Replacement: sToRepl,
                    LookAt: Excel.XlLookAt.xlPart,
                    SearchOrder: Excel.XlSearchOrder.xlByRows,
                    MatchCase: false, SearchFormat: false,
                    ReplaceFormat: false);
            }
        }

        private int FillOrder(double amount, Excel.Worksheet ws, SqlTransaction _tran = null, int? climber_id = null)
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            bool scs = false;
            int number = 1;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.Transaction = _tran == null ? cn.BeginTransaction() : _tran;
            try
            {

                while (true)
                {
                    cmd.CommandText = "SELECT COUNT(*) cnt" +
                                      "  FROM orders" +
                                      " WHERE order_number = " + number.ToString();
                    object o = cmd.ExecuteScalar();
                    if (o == null || o == DBNull.Value)
                        break;
                    if (Convert.ToInt32(o) == 0)
                        break;
                    number++;
                }

                int iid;
                cmd.CommandText = "SELECT ISNULL(MAX(iid),0) + 1 iid FROM orders";
                object oR = cmd.ExecuteScalar();
                if (oR == null || oR == DBNull.Value)
                    iid = 1;
                else
                    iid = Convert.ToInt32(oR);

                CultureInfo ci = new CultureInfo("ru-RU");
                DateTime dt = dateToPrint.Value.Date;
                ReplaceLabel("ORG", CalculateNaim(tbOrg.Text, climber_id, cmd.Transaction), ws);
                ReplaceLabel("NUM", number.ToString(), ws);
                ReplaceLabel("DATE", dt.ToString("dd.MM.yyyy", ci), ws);
                ReplaceLabel("AMT", amount.ToString(), ws);
                ReplaceLabel("FROM", CalculateNaim(tbFrom.Text, climber_id, cmd.Transaction), ws);
                ReplaceLabel("NAIM", CalculateNaim(tbNaim.Text, climber_id, cmd.Transaction), ws);
                ReplaceLabel("CASHIER", tbCashier.Text, ws);
                ReplaceLabel("CONTR", tbContr.Text, ws);
                int kop;
                ReplaceLabel("AMT_TEXT", DoubleToVerbal(amount, out kop), ws);
                ReplaceLabel("KOP", kop.ToString(), ws);
                ReplaceLabel("DAY", dt.ToString("dd"), ws);
                ReplaceLabel("YEAR", dt.ToString("yy"), ws);
                ReplaceLabel("MONTH", dt.ToString("dd-MMMM-yyyy", ci).Replace(dt.Day.ToString("00") + "-", "").Replace("-" + dt.Year.ToString("0000"), ""), ws);

                cmd.CommandText = @"
INSERT INTO orders(iid, team_id, climber_id, order_number, order_date, order_amount, order_file, order_data)
VALUES(@iid," + teamID.ToString() + ",@climber," + number.ToString() + ",@order_date,@amt, @file, @data)";
                cmd.Parameters.Add("@climber", SqlDbType.Int);
                if (climber_id != null && climber_id.HasValue)
                    cmd.Parameters[0].Value = climber_id.Value;

                cmd.Parameters.Add("@order_date", SqlDbType.DateTime);
                cmd.Parameters[1].Value = dt;

                cmd.Parameters.Add("@amt", SqlDbType.Decimal, 18);
                cmd.Parameters[2].Value = amount;

                cmd.Parameters.Add("@file", SqlDbType.VarChar, 500);
                cmd.Parameters[3].Value = "-";

                cmd.Parameters.Add("@data", SqlDbType.Image);
                cmd.Parameters[4].Value = new byte[] { (byte)0 };

                cmd.Parameters.Add("@iid", SqlDbType.Int);
                cmd.Parameters[5].Value = iid;

                if (climber_id == null || !climber_id.HasValue)
                {
                    List<int> climbersOrd = new List<int>();
                    if (climbersToForm != null)
                        foreach (var n in climbersToForm)
                            climbersOrd.Add(n);
                    else
                    {
                        SqlCommand cmd2 = new SqlCommand();
                        cmd2.Connection = cmd.Connection;
                        cmd2.Transaction = cmd.Transaction;
                        cmd2.CommandText = "SELECT iid FROM Participants P(nolock) WHERE P.team_id = " + teamID.ToString();
                        using (SqlDataReader rdr = cmd2.ExecuteReader())
                        {
                            while (rdr.Read())
                                climbersOrd.Add(Convert.ToInt32(rdr["iid"]));
                        }
                    }
                    for (int n = 0; n < climbersOrd.Count; n++)
                    {
                        cmd.Parameters[0].Value = climbersOrd[n];
                        if (n > 0)
                            cmd.Parameters[5].Value = (++iid);
                        cmd.ExecuteNonQuery();
                    }
                    scs = (climbersOrd.Count > 0);
                }
                else
                {
                    cmd.ExecuteNonQuery();
                    scs = true;
                }
            }
            finally
            {
                if (_tran == null)
                {
                    if (scs)
                        cmd.Transaction.Commit();
                    else
                        cmd.Transaction.Rollback();
                }
            }
            return (scs ? number : -1);
        }

        private struct OrderStruct
        {
            public int climberId;
            public int cntStyle;
            public double amount;
            public string name;
        }

        private string GetExistOrder(SqlTransaction tran, out Process[] prc, int[] climbers = null)
        {
            List<Process> prcList = new List<Process>();
            prc = null;
            string res = String.Empty;
            if (climbers != null && climbers.Length < 1)
                return res;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.Transaction = tran;
            string sWhereClm = String.Empty;
            if (climbers != null)
            {
                foreach (var n in climbers)
                {
                    if (String.IsNullOrEmpty(sWhereClm))
                        sWhereClm = " AND climber_id IN(";
                    else
                        sWhereClm += ",";
                    sWhereClm += n.ToString();
                }
                sWhereClm += ") ";
            }

            cmd.CommandText = "SELECT DISTINCT order_file" +
                              "  FROM orders(nolock)" +
                              " WHERE team_id = " + teamID.ToString() + sWhereClm;
            List<string> files = new List<string>();
            using (MemoryStream ms = new MemoryStream())
            {
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                        files.Add(rdr[0].ToString());
                }
                cmd.CommandText = "SELECT TOP 1 order_data" +
                                      "  FROM orders(NOLOCK)" +
                                      " WHERE team_id = " + teamID.ToString() +
                                      "   AND order_file = @f";
                cmd.Parameters.Add("@f", SqlDbType.VarChar, 255);
                foreach (var s in files)
                {
                    cmd.Parameters[0].Value = s;
                    byte[] fData = cmd.ExecuteScalar() as byte[];
                    if (fData == null || fData.Length < 10)
                        continue;
                    ms.Write(fData, 0, fData.Length);
                    string f = Path.Combine(lblDir.Text, s);
                    if (!String.IsNullOrEmpty(res))
                        res += "\r\n";
                    res += f;
                    using (var fS = File.Create(f))
                    {
                        fS.Write(fData, 0, fData.Length);
                    }
                    prcList.Add(Process.Start(f));
                }
                prc = prcList.ToArray();
                return res;
            }
        }

        private static string createReplNameStr(string source)
        {
            return source.Replace(',', '_').Replace('.', '_').Replace(' ', '_').Replace('\"', '_').Replace('\'', '_')
                .Replace('\r', '_').Replace('\n', '_').Replace('\t', '_').Replace('&', '_');
        }

        private bool CreateOrder(SqlTransaction _tran = null)
        {
            if (String.IsNullOrEmpty(lblDir.Text))
            {
                MessageBox.Show(this, "Папка для сохранения ордеров не выбрана");
                return false;
            }
            Excel.Application xlApp;
            Excel.Workbook wb;
            Excel.Worksheet ws;


            if (cn.State != ConnectionState.Open)
                cn.Open();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = cn;

            string sFNToSave = string.Empty;

            cmd.Transaction = _tran == null ? cn.BeginTransaction() : _tran;
            Process pr = null;

            bool scs = false;
            try
            {
                cmd.CommandText = "SELECT COUNT(*)" +
                                  "  FROM orders O" +
                                  "  JOIN Participants P(nolock) ON P.iid = O.climber_id " +
                                  " WHERE O.team_id=" + teamID.ToString() + SClimberWhere;
                if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                {
                    var dr = MessageBox.Show(this, "Для выбранной команды найдены сформированные ордера.\r\n" +
                        "Переформировать?\r\n" +
                        "(ответ НЕТ означает распечатать старые)", String.Empty, MessageBoxButtons.YesNoCancel);
                    if (dr == System.Windows.Forms.DialogResult.Cancel)
                        return false;
                    else if (dr == System.Windows.Forms.DialogResult.Yes)
                    {
                        cmd.CommandText = "DELETE orders" +
                                          "  FROM orders O(nolock)" +
                                          "  JOIN Participants P(nolock) ON P.iid = O.climber_id" +
                                          " WHERE O.team_id=" + teamID.ToString() + SClimberWhere;
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        Process[] prcL;
                        sFNToSave = GetExistOrder(cmd.Transaction, out prcL);
                        if (String.IsNullOrEmpty(sFNToSave))
                        {
                            MessageBox.Show(this, "Не удалось извлечь ордера из БД. Требуется их переформировать");
                            return false;
                        }
                        if (prcL != null && prcL.Length > 0)
                            pr = prcL[0];
                        scs = true;
                        return true;
                    }
                }

                List<OrderStruct> climbers = new List<OrderStruct>();
                double amtTeam = default(double);
                if (rbClimber.Checked)
                {
                    if ((tbOrg.Text + tbFrom.Text + tbNaim.Text).IndexOf(PART_RP) > -1)
                    {
                        if (!TableDataChange.FillClimbersRP(teamID, cn, cmd.Transaction, this, SClimberWhere))
                            return false;
                    }
                    cmd.CommandText = "SELECT iid, lead, speed, boulder, surname+' '+name clm_name" +
                                      "  FROM Participants P(nolock)" +
                                      " WHERE team_id = " + teamID.ToString() +
                                      "   AND (lead > 0" +
                                      "    OR speed > 0" +
                                      "    OR boulder > 0)" + SClimberWhere +
                                   " ORDER BY surname, name";
                    using (var rdr = cmd.ExecuteReader())
                    {
                        int climber_id;
                        bool lead, speed, boulder;
                        while (rdr.Read())
                        {
                            climber_id = Convert.ToInt32(rdr["iid"]);
                            lead = Convert.ToInt32(rdr["lead"]) > 0;
                            speed = Convert.ToInt32(rdr["speed"]) > 0;
                            boulder = Convert.ToInt32(rdr["boulder"]) > 0;
                            OrderStruct clm;
                            clm.climberId = climber_id;
                            if (lead && !speed && !boulder || !lead && speed && !boulder || !lead && !speed && boulder)
                                clm.cntStyle = 1;
                            else if (lead && speed && !boulder || lead && !speed && boulder || !lead && speed && boulder)
                                clm.cntStyle = 2;
                            else
                                clm.cntStyle = 3;
                            clm.name = rdr["clm_name"].ToString();

                            double? amount;
                            bool useDns = rbParticipantsDNS.Checked;
                            if (useDns)
                            {
                                if (!GetDoubleValue("Ст.Взнос за неявку", tbAmountDNS, out amount))
                                    return false;
                            }
                            else
                            {
                                switch (clm.cntStyle)
                                {
                                    case 1:
                                        if (!GetDoubleValue("Ст.Взнос за 1 вид", tbAmount1, out amount))
                                            return false;
                                        break;
                                    case 2:
                                        if (!GetDoubleValue("Ст.Взнос за 2 вида", tbAmount2, out amount))
                                            return false;
                                        break;
                                    case 3:
                                        if (!GetDoubleValue("Ст.Взнос за 3 вида", tbAmount3, out amount))
                                            return false;
                                        break;
                                    default:
                                        return false;
                                }
                            }
                            if (amount == null || !amount.HasValue)
                            {
                                MessageBox.Show(this, "Неверная сумма");
                                return false;
                            }
                            clm.amount = amount.Value;
                            climbers.Add(clm);
                        }
                    }
                    if (climbers.Count < 1)
                    {
                        MessageBox.Show(this, "Нет ордеров для формирования");
                        return false;
                    }
                }
                else
                {
                    double? amt;
                    if (!GetDoubleValue("Сумма на команду", tbAmountTeam, out amt))
                        return false;
                    if (amt == null || !amt.HasValue)
                    {
                        MessageBox.Show(this, "Сумма на команду не введена.\r\nДля рассчета нажмите на \"Рассчитать\"");
                        return false;
                    }
                    amtTeam = amt.Value;
                }
                if (!StaticClass.LaunchExcel(out xlApp, out wb, out ws, false, false, cmd.Connection, false, false, "P_Kord.xlt", cmd.Transaction))
                    return false;
                StaticClass.SetExcelVisible(xlApp);
                xlApp.PromptForSummaryInfo = false;
                List<int> CreatedOrders = new List<int>();

                if (rbClimber.Checked)
                {
                    #region FillByClimber
                    for (int i = 0; i < climbers.Count; i++)
                    {
                        var clm = climbers[i];

                        if (i < climbers.Count - 1)
                        {
                            ((Excel.Worksheet)wb.Sheets[1]).Copy(After: wb.Sheets[wb.Sheets.Count]);
                            ws = (Excel.Worksheet)wb.Sheets[wb.Sheets.Count];
                        }
                        else
                            ws = (Excel.Worksheet)wb.Sheets[1];
                        ((Excel._Worksheet)ws).Activate();
                        ws.Name = createReplNameStr(clm.name) + '_' + clm.climberId.ToString();
                        CreatedOrders.Add(FillOrder(clm.amount, ws, cmd.Transaction, new int?(clm.climberId)));
                        if (climbers.Count > 1 && i == climbers.Count - 1)
                        {
                            ws.Move(After: wb.Sheets[wb.Sheets.Count]);
                            ((Excel._Worksheet)wb.Sheets[1]).Activate();
                        }
                    }
                    #endregion
                }
                else
                    CreatedOrders.Add(FillOrder(amtTeam, ws, cmd.Transaction));
                string fileName = "P_KORD_" + createReplNameStr(teamName) + '_' + teamID.ToString() + ".xls";
                sFNToSave = Path.Combine(lblDir.Text, fileName);
                int nn = 0;

                cmd.Parameters.Clear();
                cmd.CommandText = "SELECT COUNT(*) FROM orders WHERE order_file = @fname";
                cmd.Parameters.Add("@fname", SqlDbType.VarChar, 4096);
                while (true)
                {
                    if (!File.Exists(sFNToSave))
                    {
                        cmd.Parameters[0].Value = fileName;
                        if (Convert.ToInt32(cmd.ExecuteScalar()) < 1)
                            break;
                    }
                    string toRepl = (nn == 0) ? ".xls" : "__" + nn.ToString() + ".xls";
                    fileName = fileName.Replace(toRepl, "__" + (++nn).ToString() + ".xls");
                    sFNToSave = Path.Combine(lblDir.Text, fileName);
                }
                string sErr = String.Empty;

                try { wb.SaveAs(sFNToSave, FileFormat: Excel.XlFileFormat.xlExcel8); }
                catch (Exception ex) { sErr = "Ошибка сохранения ордера: " + ex.Message; }

                if (sErr.Equals(String.Empty))
                {
                    try { xlApp.Quit(); }
                    catch (Exception ex) { sErr = "Ошибка завершения процесса EXCEL: " + ex.Message; }
                }

                if (sErr.Equals(String.Empty))
                {
                    Thread.Sleep(1000);

                    var prc = Process.GetProcessesByName("EXCEL");
                    for (int i = 0; i < prc.Length; i++)
                        prc[i].Kill();

                    Thread.Sleep(1000);
                }
                else
                {
                    try { xlApp.Visible = true; }
                    catch { }
                }

                byte[] res;
                try
                {
                    using (var m = new MemoryStream())
                    {
                        byte[] buffer = new byte[4096];
                        using (var s = File.Open(sFNToSave, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            int n;
                            while ((n = s.Read(buffer, 0, buffer.Length)) > 0)
                                m.Write(buffer, 0, n);
                        }
                        m.Position = 0;
                        res = m.ToArray();
                    }
                }
                catch (Exception ex)
                {
                    res = new byte[] { 0 };
                    MessageBox.Show(this, "Ордера созданы, но не сохранены в БД. Ошибка:\r\n" + ex.Message);
                }



                cmd.CommandText = "UPDATE orders SET order_file = @fname, order_data = @odata WHERE order_number = @number";
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@number", SqlDbType.Int);
                cmd.Parameters.Add("@fname", SqlDbType.VarChar, 4096);
                cmd.Parameters[1].Value = fileName;
                cmd.Parameters.Add("@odata", SqlDbType.Image);
                cmd.Parameters[2].Value = res;
                foreach (var n in CreatedOrders)
                {
                    cmd.Parameters[0].Value = n;
                    cmd.ExecuteNonQuery();
                }
                if (sErr.Equals(String.Empty))
                {
                    try { pr = Process.Start(sFNToSave); }
                    catch (Exception ex) { sErr = "Ошибка открытия файла: " + ex.Message; }
                }
                if (!sErr.Equals(String.Empty))
                    MessageBox.Show(sErr);
                scs = true;
                return true;
            }
            finally
            {
                if (_tran == null)
                {
                    if (scs)
                        cmd.Transaction.Commit();
                    else
                        cmd.Transaction.Rollback();
                }
                //if (pr != null)
                //    pr.WaitForExit();
                if (scs)
                    MessageBox.Show(this, "Ордера сохранены в \"" + sFNToSave + "\"");
            }
        }

        private void btnSelDir_Click(object sender, EventArgs e)
        {
            var s = folderDialog.ShowDialog(this);
            if (s == System.Windows.Forms.DialogResult.No || s == System.Windows.Forms.DialogResult.Cancel)
                return;
            lblDir.Text = folderDialog.SelectedPath;
        }

        private void btnSetTemplate_Click(object sender, EventArgs e)
        {
            try
            {
                string sPath = Path.Combine(Thread.GetDomain().BaseDirectory, "Templates\\P_Kord.xlt");
                var dtSource = File.GetLastWriteTime(sPath);
                var dr = MessageBox.Show(this, "Изменение шаблона затронет только данный компьютер.\r\n" +
                    "Пожалуйста, сохраните шаблон в формате \"Шаблон Excel 97-2003\"  в файл:\r\n" +
                    sPath + "\r\n\r\nПродолжить?", "Изменение шаблона", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dr == System.Windows.Forms.DialogResult.No)
                    return;
                var p = Process.Start(sPath);
                if (p != null)
                    p.WaitForExit();
                var dtlast = File.GetLastWriteTime(sPath);
                if (dtlast <= dtSource)
                    MessageBox.Show(this, "Возможно, шаблон не изменился. Проверьте правильность сохранения файла");
            }
            catch (Exception ex) { MessageBox.Show(this, "Ошибка правки шаблона:\r\n" + ex.Message); }
        }

        private int[] climbersToForm = null;

        private string SClimberWhere
        {
            get
            {
                if (climbersToForm == null || climbersToForm.Length < 1)
                    return String.Empty;
                else
                {
                    string sRes = String.Empty;
                    foreach (var n in climbersToForm)
                    {
                        if (String.IsNullOrEmpty(sRes))
                            sRes = " AND P.iid IN(";
                        else
                            sRes += ",";
                        sRes += n.ToString();
                    }
                    sRes += ") ";
                    return sRes;
                }
            }
        }

        private void btnFormTeamPArt_Click(object sender, EventArgs e)
        {
            var res = TableDataChange.CreateClimbersListForOrders(teamID, cn, null, this, climbersToForm);
            if (res == null)
                return;
            if (res.Length < 1)
                climbersToForm = null;
            else
                climbersToForm = res;
            tbAmountTeam.Text = String.Empty;
        }

        private void rbParticipantsDNS_CheckedChanged(object sender, EventArgs e)
        {
            gbStyles.Enabled = !(gbNYA.Enabled = rbParticipantsDNS.Checked);
        }
    }
}
