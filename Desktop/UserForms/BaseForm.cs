// <copyright file="BaseForm.cs">
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
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Threading;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;

namespace ClimbingCompetition
{
    /// <summary>
    /// Форма-родитель для всех используемых форм. В конструкторе открывается новое соединение с БД,
    /// в деструткоре оно закрывается
    /// </summary>
    public class BaseForm : Form, IRefreshableAndLocalizable
    {
        protected const int FORM_CLASS_FIELD_LENGTH = 500;
        public const int HOST_NAME_FIELD_LENGTH = 100;
        protected const int PROPERTY_NAME_FIELD_LENGTH = 290;

        private bool ncl = false;
        protected bool needToClose { get { return ncl; } set { ncl = value; } }
        private SqlConnection con;
        protected string cTitle;
        private SpeedRules cr;
        protected SpeedRules CR { get { return cr; } }
        public int TimeRemaining { get; private set; }
        protected bool trial { get { return ValidationClass.IsTrial; } }
        public string competitionTitle
        {
            get { return cTitle; }
            set { cTitle = value; }
        }
        public SqlConnection cn { get { return con; } }

        public BaseForm()
            : this(null)
        {
        }
        private BaseForm(String baseResourceFile/* = "ClimbingCompetition.ListHeaderStrings"*/)
        {
            this.baseResourceFile = baseResourceFile;
            this.ShowIcon = false;
            int n;
            alEx = ValidationClass.AllowExecute(out n, false);
            TimeRemaining = n;
            this.SizeGripStyle = SizeGripStyle.Show;
            this.LoadLocalizedStrings(Thread.CurrentThread.CurrentUICulture);
        }

        private bool alEx;
        public BaseForm(SqlConnection cn, string competitionTitle, String baseResourceFile = "ClimbingCompetition.ListHeaderStrings")
            : this(baseResourceFile)
        {
            this.cTitle = competitionTitle;
            if (cn == null || cn.ConnectionString == "")
            {
                cr = SpeedRules.DefaultAll;
                return;
            }
            con = new SqlConnection(cn.ConnectionString);
            try { con.Open(); }
            catch (Exception ex) { MessageBox.Show("Невозможно открыть соединение:\r\n" + ex.Message); }
            try
            {
                cr = SortingClass.GetCompRules(this.cn, false);
                bool leaveTrains = SettingsForm.GetLeaveTrains(this.cn);
                if (leaveTrains)
                    cr = cr | SpeedRules.InternationalRules;
                else
                    cr = cr & (~SpeedRules.InternationalRules);
            }
            catch (Exception ex)
            {
                cr = SpeedRules.DefaultAll;
                MessageBox.Show("Невозможно определить правила проведения соревнований:\r\n"
                    + ex.Message + "\r\nБудут использоваться российские правила");
            }
        }

        public static void FormGenerateSaveTables(SqlConnection cn, SqlTransaction tran = null)
        {
            SqlCommand cmd = new SqlCommand { Connection = cn, Transaction = tran };
            cmd.CommandText = "SELECT COUNT(*) cnt" +
                              "  FROM sysobjects(nolock)" +
                              " WHERE name = 'form_object_data'" +
                              "   AND type = 'U'";
            if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                return;
            cmd.CommandText = @"
  create table form_object_data(
        form_class varchar(" + FORM_CLASS_FIELD_LENGTH.ToString() + @"),
        host_name  varchar(" + HOST_NAME_FIELD_LENGTH.ToString() + @"),
        property   varchar(" + PROPERTY_NAME_FIELD_LENGTH + @"),
        value      image)

  create unique index form_object_data_XU1 on form_object_data(form_class, host_name, property)
  create index form_object_data_XK1 on form_object_data(form_class, host_name)
";
            cmd.ExecuteNonQuery();
        }

        protected virtual void FormLoadProperties(SqlTransaction tran = null)
        {
            SqlCommand cmd = new SqlCommand { Connection = cn, Transaction = tran };
            cmd.CommandText = "SELECT property, value" +
                              "  FROM form_object_data" +
                              " WHERE form_class = @formClass" +
                              "   AND host_name = @hostName";
            String typeName;
            String hostName;
            GetFormDataKeys(out typeName, out hostName);

            cmd.Parameters.Add("@formClass", SqlDbType.VarChar, FORM_CLASS_FIELD_LENGTH)
                .Value = typeName;
            cmd.Parameters.Add("@hostName", SqlDbType.VarChar, HOST_NAME_FIELD_LENGTH)
                .Value = hostName;
            Dictionary<String,object> properties = new Dictionary<string, object>();
            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                    properties.Add((String)rdr["property"], rdr["value"]);
            }
            if (properties.Count < 1)
                return;
            BinaryFormatter fmt = new BinaryFormatter();

            foreach (var p in GetPropertiesToSave())
            {
                if (!properties.ContainsKey(p.Name))
                    continue;
                p.SetSerializedValue(properties[p.Name], fmt);
            }
        }

        protected virtual void FormSaveProperties(SqlTransaction tran = null)
        {
            SqlCommand cmd = new SqlCommand { Connection = cn, Transaction = (tran ?? cn.BeginTransaction()) };
            try
            {
                cmd.CommandText = @"
IF EXISTS(SELECT 1
            FROM form_object_data
           WHERE form_class = @formClass
             AND host_name = @hostName
             AND property = @property
         ) begin
  update form_object_data
     set value = @value
   where form_class = @formClass
     and host_name = @hostName
     and property = @property
end
else begin
  insert into form_object_data(form_class, host_name, property, value)
  values (@formClass, @hostName, @property, @value)
end";
                String formClass, hostName;
                GetFormDataKeys(out formClass, out hostName);
                cmd.Parameters.Add("@formClass", SqlDbType.VarChar, PROPERTY_NAME_FIELD_LENGTH).Value = formClass;
                cmd.Parameters.Add("@hostName", SqlDbType.VarChar, HOST_NAME_FIELD_LENGTH).Value = hostName;

                var propetyParam = cmd.Parameters.Add("@property", SqlDbType.VarChar, PROPERTY_NAME_FIELD_LENGTH);
                var valueParam = cmd.Parameters.Add("@value", SqlDbType.Image);

                BinaryFormatter fmt = new BinaryFormatter();
                foreach (var p in GetPropertiesToSave())
                {
                    var pValue = p.GetSerializedValue(fmt);
                    if (pValue == null)
                        continue;
                    valueParam.Value = pValue;
                    propetyParam.Value = p.Name;
                    cmd.ExecuteNonQuery();
                }
                                  

                if (tran == null)
                    cmd.Transaction.Commit();
            }
            catch
            {
                if (tran == null)
                    cmd.Transaction.Rollback();
                throw;
            }
        }

        protected virtual void GetFormDataKeys(out String typeName, out String hostName)
        {
            typeName = this.GetType().AssemblyQualifiedName;
            if (typeName.Length > FORM_CLASS_FIELD_LENGTH)
                typeName = typeName.Substring(0, FORM_CLASS_FIELD_LENGTH);

            hostName = System.Net.Dns.GetHostName();
            if (hostName.Length > HOST_NAME_FIELD_LENGTH)
                hostName = hostName.Substring(0, HOST_NAME_FIELD_LENGTH);
        }

        private static readonly List<String> allowedFormProperties = new List<string>() { "LOCATION", "SIZE", "MAXIMIZED", "MINIMIZED" };
        private static readonly List<String> allowedControlProperties = new List<string>() { "SPLITTERDISTANCE" };
        private static readonly List<Type> controlTypes = new List<Type>() { typeof(SplitContainer) };
        protected virtual IEnumerable<FormPropetyData> GetPropertiesToSave()
        {
            List<String> keys = new List<string>();
            foreach (var p in typeof(Form).GetProperties())
            {
                if (p.CanRead && p.CanWrite && allowedFormProperties.Contains(p.Name.Trim().ToUpperInvariant()))
                {
                    if (!keys.Contains(p.Name.ToUpperInvariant()))
                    {
                        keys.Add(p.Name.ToUpperInvariant());
                        
                        yield return (new FormPropetyData { Property = p, Target = this });
                    }
                }
            }
            foreach (var control in this.Controls)
            {
                if (controlTypes.Contains(control.GetType()))
                    foreach (var p in GetControlProperties((Control)control))
                        if (!keys.Contains(p.Name.ToUpperInvariant()))
                        {
                            keys.Add(p.Name.ToUpperInvariant());
                            yield return p;
                        }
            }
        }

        private IEnumerable<FormPropetyData> GetControlProperties(Control control)
        {
            foreach (var p in control.GetType().GetProperties())
            {
                if (p.CanRead && p.CanWrite && allowedControlProperties.Contains(p.Name.Trim().ToUpperInvariant()))
                    yield return (new FormPropetyData
                    {
                        Property = p,
                        Target = control,
                        Name = String.Format("{0}.{1}", control.Name, p.Name)
                    });
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!alEx || ncl)
            {
                try { this.Close(); }
                catch { }
            }
            else
            {
                try
                {
                    FormGenerateSaveTables(cn);
                    FormLoadProperties();
                }
                catch { }
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if ((!e.Cancel) && (con != null) && (con.State != ConnectionState.Closed))
            {
                try { FormSaveProperties(); }
                catch { }
                //try
                //{
                //    if (!this.useParentConnection)
                //        con.Close();
                //}
                //catch { }
            }
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            if (con != null && con.State != ConnectionState.Closed)
            {
                try { con.Close(); }
                catch { }
            }
            base.OnClosed(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (cn != null)
                    cn.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // BaseForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Name = "BaseForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.ResumeLayout(false);

        }


        private static StringSortStruct GetSortValue(String src)
        {
            if (sortablePair.ContainsKey(src))
                return sortablePair[src];
            StringSortStruct s = new StringSortStruct(src);
            sortablePair.Add(src, s);
            if (!reverseSortingPair.ContainsKey(s.SortableValue))
                reverseSortingPair.Add(s.SortableValue, src);
            return s;
        }
        public static bool IsSortable(String src)
        {
            return GetSortValue(src).IsSortable;
        }

        private sealed class StringSortStruct
        {
            public String Source { get; private set; }
            public String SortableValue { get; private set; }
            public Boolean IsSortable { get; private set; }
            public StringSortStruct(String src)
            {
                this.Source = src;
                this.IsSortable = true;
                foreach (var c in this.Source)
                    if (!(Char.IsLetterOrDigit(c) || c.Equals('_')))
                    {
                        this.IsSortable = false;
                        break;
                    }
                if (this.IsSortable)
                    this.SortableValue = this.Source;
                else
                {
                    StringBuilder res = new StringBuilder(src.Length);
                    foreach (var c in src)
                    {
                        if (Char.IsLetterOrDigit(c) || c.Equals('_'))
                            res.Append(c);
                        else
                            res.Append('_');
                    }
                    this.SortableValue = res.ToString();
                }
            }
        }

        private static Dictionary<String, StringSortStruct> sortablePair = new Dictionary<String, StringSortStruct>();
        private static Dictionary<String, String> reverseSortingPair = new Dictionary<string, string>();
        public static String GetSortable(String src)
        {
            return GetSortValue(src).SortableValue;
        }

        public static String GetInitialValue(String sortableValue)
        {
            if (reverseSortingPair.ContainsKey(sortableValue))
                return reverseSortingPair[sortableValue];
            return sortableValue;
        }

        public virtual void LoadLocalizedStrings(CultureInfo ci = null)
        {
            if (!String.IsNullOrEmpty(baseResourceFile))
                StaticClass.LoadInitialStrings(baseResourceFile, this, ci);
        }

        private readonly String baseResourceFile;
        public String BaseResourceFile { get { return baseResourceFile; } }


        public virtual void RefreshAndReload() { }

        public sealed class FormPropetyData
        {
            public PropertyInfo Property { get; set; }
            public Object Target { get; set; }
            private String name;
            public String Name
            {
                get
                {
                    String retVal = String.IsNullOrEmpty(name) ? Property.Name : name;
                    return (retVal.Length > PROPERTY_NAME_FIELD_LENGTH) ? retVal.Substring(0, PROPERTY_NAME_FIELD_LENGTH) : retVal;
                }
                set { this.name = value; }
            }

            private object Value
            {
                get
                {
                    return this.Property.GetValue(this.Target, null);
                }
                set
                {
                    this.Property.SetValue(this.Target, value, null);
                }
            }

            public object GetSerializedValue(BinaryFormatter fmt = null)
            {
                object value = this.Value;
                if (value == null)
                    return DBNull.Value;
                if (fmt == null)
                    fmt = new BinaryFormatter();
                using (var mstr = new MemoryStream())
                {
                    try { fmt.Serialize(mstr, value); }
                    catch (SerializationException) { return null; }
                    mstr.Position = 0;
                    return mstr.ToArray();
                }

            }

            public void SetSerializedValue(object value, BinaryFormatter fmt = null)
            {
                byte[] data = value as byte[];
                if (data == null)
                {
                    this.Value = null;
                    return;
                }
                if (fmt == null)
                    fmt = new BinaryFormatter();
                using (var mstr = new MemoryStream(data))
                {
                    try { this.Value = fmt.Deserialize(mstr); }
                    catch (SerializationException) { }
                }
            }
        }
    }

    

    public interface IRefreshableAndLocalizable
    {
        void LoadLocalizedStrings(CultureInfo ci);
        void RefreshAndReload();
    }
}
