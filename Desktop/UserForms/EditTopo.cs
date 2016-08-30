// <copyright file="EditTopo.cs">
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
using System.IO;
using System.Threading;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace ClimbingCompetition
{
    public partial class EditTopo : BaseForm
    {
        private const int RADIUS = 5;
        public enum WorkMode { EditTopo, ShowResults }
        private readonly WorkMode workMode;
        private SqlTransaction tran = null;

        private delegate T Func<out T>();
        private delegate void Action<in T>(T param);

        private Image emptyImage = null;

        private readonly int listId, climberId;
        private readonly String Header;

        private List<HoldData> holds = new List<HoldData>();

        private Color holdColor
        {
            get { return btnColorHold.BackColor; }
            set { btnColorHold.BackColor = value; }
        }
        private Color selectedHoldColor
        {
            get { return btnColorSelectedHold.BackColor; }
            set { btnColorSelectedHold.BackColor = value; }
        }

        private string tempTable = null;
        private string tempTableQuery = null;
        private void backupTable()
        {
            var cmd = new SqlCommand { Connection = cn, Transaction = cn.BeginTransaction() };
            try
            {
                cmd.CommandText = "SELECT COUNT(*) FROM tempdb..sysobjects WHERE name=@name and type = 'U'";
                cmd.Parameters.Add("@name", SqlDbType.VarChar, 255);
                Random rnd = new Random();
                String sTable;
                do
                {
                    sTable = String.Format("##TOPO{0}", rnd.Next());
                    cmd.Parameters[0].Value = sTable;
                } while (Convert.ToInt32(cmd.ExecuteScalar()) > 0);
                cmd.CommandText = String.Format("SELECT * INTO {0} FROM topo_holds(nolock) WHERE list_id={1}",
                    sTable, listId);
                cmd.ExecuteNonQuery();
                tempTable = sTable;

                tempTableQuery = SortingClass.CreateTableComparisonQuery("topo_holds", sTable, "iid", cmd.Connection, cmd.Transaction);

                cmd.Transaction.Commit();
            }
            catch
            {
                tempTable = tempTableQuery = null;
                cmd.Transaction.Rollback();
                throw;
            }
        }

        private const string ROLBACK = "Откатить";
        private Stopwatch sw;

        protected EditTopo(SqlConnection _cn, String _competitionTitle, int listId, int climberId, Stopwatch sw)
            : base(_cn, _competitionTitle)
        {
            InitializeComponent();

            this.sw = sw;
            this.label1.Visible = this.tbTime.Visible = (sw != null && sw.IsRunning);
            Log = new TopoLogDataStack(this);

            holdColor = SettingsForm.GetHoldColor(false, cn);
            selectedHoldColor = SettingsForm.GetHoldColor(true, cn);

            this.listId = listId;
            this.climberId = climberId;
            SqlCommand cmd = new SqlCommand { Connection = this.cn };
            cmd.CommandText = "SELECT g.name, l.round" +
                              "  FROM lists l(nolock)" +
                              "  JOIN Groups g(nolock) on G.iid = l.group_id" +
                              " WHERE l.iid = @iid";
            cmd.Parameters.Add("@iid", SqlDbType.Int).Value = listId;
            StringBuilder sbHeader = new StringBuilder();
            using (var rdr = cmd.ExecuteReader())
            {
                if (rdr.Read())
                    sbHeader.AppendFormat("{0} {1}", rdr["name"], rdr["round"]);
                else
                    throw new ArgumentException("No list found", "listId");
            }
            if (climberId > 0)
            {
                cmd.CommandText = "SELECT r.start, p.surname + ' ' + p.name climber" +
                                "  FROM routeResults r(nolock)" +
                                "  JOIN Participants p(nolock) on p.iid = r.climber_id" +
                                " WHERE r.list_id = @iid" +
                                "   AND r.climber_id = @climber";
                cmd.Parameters.Add("@climber", SqlDbType.Int).Value = climberId;
                using (var rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                        sbHeader.AppendFormat("{0}{1} (Ст.№ {2})", Environment.NewLine, rdr["climber"], rdr["start"]);
                    else
                        throw new ArgumentException("No climber found", "climberId");
                }
                this.workMode = WorkMode.ShowResults;
            }
            else
            {
                this.workMode = WorkMode.EditTopo;
                backupTable();
            }
            Header = sbHeader.ToString();
            if (this.workMode == WorkMode.EditTopo)
            {
                this.gbEditTopo.Enabled = true;
                this.gbEditClimber.Enabled = false;
            }
            else
            {
                this.gbEditTopo.Enabled = false;
                this.gbEditClimber.Enabled = true;
                this.tbHeader.Text = Header;
            }
            this.Text = Header.Replace(Environment.NewLine, " ");
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            
        }


        void topoBox_Paint(object sender, PaintEventArgs e)
        {
            DrawHolds();
        }

        public event EventHandler<TopoClickEventArgs> TopoClick;
        private void OnTopoClick(TopoClickEventArgs e)
        {
            var temp = Interlocked.CompareExchange(ref TopoClick, null, null);
            if (temp != null)
                temp(this, e);
        }

        #region BASIC
        private void EditTopo_Load(object sender, EventArgs e)
        {
            this.emptyImage = LoadTopoFromDb();
            this.topoBox.Image = emptyImage;
            if (this.workMode == WorkMode.ShowResults && emptyImage == null)
            {
                MessageBox.Show("Схема не загружена.");
                this.Close();
                return;
            }
            LoadHolds();
            DrawHolds();
            this.btnDelTopo.Enabled = this.btnSaveToFile.Enabled = gbEditTopo.Enabled && (emptyImage != null);
        }

        private void LoadHolds()
        {
            holds.Clear();
            SqlCommand cmd = new SqlCommand { Connection = cn, Transaction = tran };
            cmd.CommandText = String.Format("SELECT iid, hold_id, x, y FROM topo_holds(nolock) WHERE list_id={0} ORDER BY hold_id", this.listId);
            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    holds.Add(new HoldData
                    {
                        Iid = Convert.ToInt32(rdr["iid"]),
                        Selected = false,
                        Tag = rdr["hold_id"].ToString(),
                        TopoCorrdinates = new Point(Convert.ToInt32(rdr["x"]), Convert.ToInt32(rdr["y"]))
                    });
                }
            }
            SortHoldsOrder();
        }

        private void SortHoldsOrder()
        {
            holds.Sort((a, b) =>
            {
                long tagA, tagB;
                try { ResultListLead.GetUnifiedHoldTag(a.Tag, false, false, out tagA); }
                catch (ArgumentException) { tagA = long.MaxValue; }
                try { ResultListLead.GetUnifiedHoldTag(b.Tag, false, false, out tagB); }
                catch (ArgumentException) { tagB = long.MaxValue; }
                return tagA.CompareTo(tagB);
            });
            int order = 0;
            holds.ForEach(h => h.Order = (++order));
        }

        private static bool Intersects(RectangleF r1, RectangleF r2)
        {
            return (RectangleF.Intersect(r1, r2) != RectangleF.Empty);
        }

        //private void DrawTopo(Graphics g, Size size)
        //{
        //    using (var bitmap = new Bitmap(emptyImage))
        //    {
        //        if (holds.Count > 0)
        //            using (var g = Graphics.FromImage(bitmap))
        //            {
        //            }
        //    }
        //    return;
        //    //g.DrawImage(emptyImage, 
        //    var scaleX =  (double)size.Width / emptyImage.Width;
        //    var scaleY =  (double)size.Height / emptyImage.Height;
        //    holds.ForEach(h => h.ScreenCoordinates = new Point(
        //        x: (int)(scaleX * h.TopoCorrdinates.X),
        //        y: (int)(scaleY * h.TopoCorrdinates.Y)));
        //    Pen penOrdinary = null, penSelected = null;

        //    float fRadius = RADIUS;
        //    foreach (var h in holds)
        //    {
        //        if (h.Selected && penSelected == null)
        //            penSelected = new Pen(selectedHoldColor);
        //        else if (!h.Selected && penOrdinary == null)
        //            penOrdinary = new Pen(holdColor);
        //        RectangleF rect = new RectangleF(h.TopoCorrdinates.X - fRadius, h.TopoCorrdinates.Y - fRadius, 2 * fRadius, 2 * fRadius);
        //        g.DrawEllipse(h.Selected ? penSelected : penOrdinary, rect);
        //    }
            

        //}
        private void DrawHolds()
        {
            if (this.emptyImage == null)
                return;
            if (this.topoBox.Image != emptyImage)
                try
                {
                    this.topoBox.Image.Dispose();
                    this.topoBox.Image = null;
                    GC.Collect(2, GCCollectionMode.Forced);
                }
                catch { }

            //var scale = Math.Min((float)topoBox.ClientSize.Height / (float)emptyImage.Size.Height, (float)topoBox.ClientSize.Width / (float)emptyImage.Size.Width);
            //scale *= 0.5f;
            //var img = DrawHoldsOnEmptyImage();
            //var scaledBitmap = new Bitmap(topoBox.Width, topoBox.Height);
            //using (var mx = new Matrix())
            //{
            //    mx.Scale(scale, scale);
            //    using (var g = Graphics.FromImage(scaledBitmap))
            //    {
            //        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //        g.Transform = mx;
            //        g.DrawImage(img, new Rectangle(0, 0, topoBox.Width, topoBox.Height));
            //    }
            //}
            this.topoBox.Image = /*scaledBitmap;*/DrawHoldsOnEmptyImage();
        }

        private Image DrawHoldsOnEmptyImage()
        {
            var bmp = new Bitmap(emptyImage.Width, emptyImage.Height);
            

            using (var g = Graphics.FromImage(bmp))
            {
                g.DrawImage(emptyImage, new Rectangle(0, 0, emptyImage.Width, emptyImage.Height));

                if (holds.Count > 0)
                {
                    double curScale;
                    int xOffset, yOffset;
                    GetOffsetData(out curScale, out yOffset, out xOffset);
                    holds.ForEach(a =>
                        a.ScreenCoordinates = new Point(x: (int)(xOffset + a.TopoCorrdinates.X * curScale),
                            y: (int)(yOffset + a.TopoCorrdinates.Y * curScale)));
                    float fRadius = (float)(RADIUS / curScale);

                    List<RectangleF> occupiedRect = new List<RectangleF>();
                    Pen penNormal = null, penSelected = null;
                    foreach (var h in holds)
                    {
                        if (h.Selected && penSelected == null)
                            penSelected = new Pen(selectedHoldColor, (float)(2.0 / curScale));
                        else if (!h.Selected && penNormal == null)
                            penNormal = new Pen(holdColor, (float)(2.0 / curScale));
                        //Pen penRed = new Pen(h.Selected ? selectedHoldColor : holdColor, (float)(2.0 / curScale));
                        RectangleF rect = new RectangleF(h.TopoCorrdinates.X - fRadius, h.TopoCorrdinates.Y - fRadius, 2 * fRadius, 2 * fRadius);
                        occupiedRect.Add(rect);
                        g.DrawEllipse(h.Selected ? penSelected : penNormal, rect);
                    }

                    Font f = new Font("Times New Roman", (float)(10.0 / curScale));
                    var fmt = new StringFormat
                    {
                        Alignment = StringAlignment.Near,
                        LineAlignment = StringAlignment.Near
                    };
                    foreach (var h in holds)
                    {
                        PointF point = new PointF(h.TopoCorrdinates.X + fRadius, h.TopoCorrdinates.Y - fRadius);
                        var res = g.MeasureString(h.Tag, f);
                        RectangleF rectString;
                        int i = 0;
                        do
                        {
                            float pointX = (i == 0) ? (h.TopoCorrdinates.X + fRadius) : (h.TopoCorrdinates.X - fRadius);
                            float pointY;
                            switch (i)
                            {
                                case 0:
                                    pointX = h.TopoCorrdinates.X + fRadius + 1.0f;
                                    pointY = h.TopoCorrdinates.Y - res.Height / 2.0f;
                                    fmt.Alignment = StringAlignment.Center;
                                    fmt.LineAlignment = StringAlignment.Near;
                                    break;
                                case 1:
                                    pointX = h.TopoCorrdinates.X - res.Width / 2.0f;
                                    pointY = h.TopoCorrdinates.Y + fRadius + 1.0f;
                                    fmt.Alignment = StringAlignment.Near;
                                    fmt.LineAlignment = StringAlignment.Center;
                                    break;
                                case 2:
                                    pointX = h.TopoCorrdinates.X - fRadius - res.Width - 1.0f;
                                    pointY = h.TopoCorrdinates.Y - res.Height / 2.0f;
                                    fmt.Alignment = StringAlignment.Center;
                                    fmt.LineAlignment = StringAlignment.Far;
                                    break;
                                case 3:
                                    pointX = h.TopoCorrdinates.X - res.Width / 2.0f;
                                    pointY = h.TopoCorrdinates.Y - fRadius - res.Height - 1.0f;
                                    fmt.Alignment = StringAlignment.Far;
                                    fmt.LineAlignment = StringAlignment.Center;
                                    break;
                                default:
                                    goto case 0;
                            }
                            rectString = new RectangleF(new PointF(pointX, pointY), res);

                            bool intersects = false;
                            foreach (var r in occupiedRect)
                                if (Intersects(rectString, r))
                                {
                                    intersects = true;
                                    break;
                                }
                            if (!intersects)
                                break;
                            i++;
                        } while (i < 5);
                        g.DrawString(h.Tag, f, new SolidBrush(h.Selected ? selectedHoldColor : holdColor), rectString.Location);
                        occupiedRect.Add(rectString);
                    }

                }
            }

            return bmp;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (tran != null)
            {
                try
                {
                    tran.Commit();
                    tran = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, String.Format("Ошибка сохранения транзакции: {0}", ex.Message));
                    return;
                }
            }
            if (workMode == WorkMode.ShowResults)
            {
                String sItem = cbModifier.SelectedItem as string;
                if (sItem == null)
                    sItem = String.Empty;
                OnTopoClick(new TopoClickEventArgs(tbResult.Text + sItem, tbTime.Text, autoSave: true));
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.checkChanges = false;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (HasChanges())
            {
                if (MessageBox.Show(this, "Все несохраненные изменения будут потеряны. Продолжить?", String.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                    == System.Windows.Forms.DialogResult.No)
                    return;
            }
            RollbackFullData();
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.checkChanges = false;
            this.Close();
        }

        private void RollbackFullData()
        {
            if (tran != null)
            {
                try { tran.Rollback(); }
                catch { }
                tran = null;
            }
            if (tempTable != null)
            {
                var cmd = new SqlCommand { Connection = cn, Transaction = cn.BeginTransaction() };
                try
                {
                    cmd.CommandText = String.Format("DELETE FROM topo_holds WHERE list_id={0}", listId);
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = String.Format("INSERT INTO topo_holds SELECT * FROM {0}", tempTable);
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = String.Format("DROP TABLE {0}", tempTable);
                    cmd.ExecuteNonQuery();
                    cmd.Transaction.Commit();
                }
                catch
                {
                    try { cmd.Transaction.Rollback(); }
                    catch { }
                }
            }
        }

        private void EditTopo_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (tran != null)
            {
                try { tran.Rollback(); }
                catch { }
                finally { tran = null; }
            }
        }
        #endregion

        #region FACTORY
        public static EditTopo CreateTopo(int listId, SqlConnection cn, String competitionTitle)
        {
            var et = new EditTopo(cn, competitionTitle, listId, -1, null);
            return et;
        }

        public static EditTopo EditClimber(int listId, int climberId, SqlConnection cn, String competitionTitle, ResultListLead parent, Stopwatch sw)
        {
            var et = new EditTopo(cn, competitionTitle, listId, climberId, sw);
            et.parent = parent;
            if (et.parent != null)
                et.parent.TimerTopoTick += new EventHandler<TopoClickEventArgs>(et.parent_TimerTopoTick);
            return et;
        }

        public static bool CheckTopoHold(String holdId, int listId, SqlConnection cn, SqlTransaction tran)
        {
            var cmd = new SqlCommand { Connection = cn, Transaction = tran };
            cmd.CommandText = "SELECT COUNT(*)" +
                              "  FROM topo_holds H(nolock)" +
                              " WHERE H.list_id = @listId" +
                              "   AND H.hold_id = @holdId";
            cmd.Parameters.Add("@listId", SqlDbType.Int).Value = listId;
            cmd.Parameters.Add("@holdId", SqlDbType.VarChar, 15).Value =
                holdId.Trim().Replace("+", String.Empty).Replace("-", String.Empty);
            return (Convert.ToInt32(cmd.ExecuteScalar()) > 0);
        }

        private ResultListLead parent = null;

        void parent_TimerTopoTick(object sender, TopoClickEventArgs e)
        {
            if (!this.IsDisposed)
                lblTimer.Invoke(new EventHandler(delegate { lblTimer.Text = e.Result; }));
        }
        #endregion

        #region EditTopo
        private enum EditTopoState { None, Clicking, Edit }

        private EditTopoState editTopoState = EditTopoState.None;
        private Image LoadTopoFromDb()
        {
            try
            {
                SqlCommand cmd = new SqlCommand { Connection = cn, Transaction = tran };
                cmd.CommandText = String.Format("SELECT topo FROM lists WHERE iid={0}", listId);
                byte[] res = cmd.ExecuteScalar() as byte[];
                if (res == null)
                    return null;
                else
                    return ImageWorker.GetFromBytes(res);
            }
            finally { GC.Collect(2, GCCollectionMode.Forced); }
        }

        private Image LoadImageFromFile()
        {
            var dRes = ofd.ShowDialog(this);
            if (dRes == System.Windows.Forms.DialogResult.No || dRes == System.Windows.Forms.DialogResult.Cancel)
                return null;
            try
            {
                return Image.FromFile(ofd.FileName);
            }
            catch (OutOfMemoryException)
            {
                MessageBox.Show(this, "Недостаточно памяти для загрузки файла");
                return null;
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show(this, String.Format("Файл {0} не найден", ofd.FileName));
                return null;
            }
        }

        private void btnUploadTopo_Click(object sender, EventArgs e)
        {
            if (this.workMode != WorkMode.EditTopo)
                return;
            if (this.emptyImage != null)
                if (MessageBox.Show(this, "Вы уверены, что хотите загрузить новую схему?", String.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    == System.Windows.Forms.DialogResult.No)
                    return;
            this.emptyImage = LoadImageFromFile();
            this.topoBox.Image = emptyImage;
            //this.topoBox.SizeMode = PictureBoxSizeMode.StretchImage;
            var cmd = ClearHolds();
            cmd.CommandText = String.Format("UPDATE lists SET topo = @topo WHERE iid={0}", this.listId);
            cmd.Parameters.Add("@topo", SqlDbType.Image).Value = ImageWorker.GetBytesFromImage(this.emptyImage);
            cmd.ExecuteNonQuery();
            DrawHolds();
        }

        private SqlCommand ClearHolds(SqlCommand cmd = null)
        {
            if (cmd == null)
                cmd = CreateCommand();
            cmd.CommandText = String.Format("DELETE FROM topo_holds WHERE list_id={0}", this.listId);
            cmd.ExecuteNonQuery();
            return cmd;
        }

        private void ProcessClickEditTopo(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && editTopoState == EditTopoState.Edit)
            {
                SaveCurrentHold();
                return;
            }
            if (e.Button != System.Windows.Forms.MouseButtons.Left)
                return;
            if (editTopoState == EditTopoState.None)
            {
                SelectHoldToEdit(e.Location);
                return;
            }
            else if (editTopoState == EditTopoState.Edit)
            {
                if (SelectHoldToEdit(e.Location, true))
                    return;
            }
            Point pRes = GetPictureXY(e.Location);
            if (pRes.X < 0 || pRes.Y < 0)
                return;
            foreach (var hold in holds)
            {
                if (editTopoState == EditTopoState.Edit && currentHold != null && Object.ReferenceEquals(hold, currentHold))
                    continue;
                if (Distance(e.Location, hold.ScreenCoordinates) < 2 * RADIUS)
                {
                    MessageBox.Show(this, "Зацепы находятся слишком близко друг к другу");
                    return;
                }
            }
            if (editTopoState == EditTopoState.Edit)
            {
                var oldCoordinates = currentHold.TopoCorrdinates;
                currentHold.TopoCorrdinates = pRes;
                SaveCurrentHold(false);
                Log.Push(new TopoLogData
                {
                    Action = TopoLogData.StackAction.Update,
                    Hold = currentHold,
                    PositionNew = pRes,
                    PositionOld = oldCoordinates
                });
            }
            else
            {
                int order, tag;
                
                if (currentHold != null && !currentHold.Tag.Equals(tbHeight.Text, StringComparison.Ordinal))
                {
                    long r;
                    try { currentHold.Tag = ResultListLead.GetUnifiedHoldTag(tbHeight.Text, false, false, out r); }
                    catch (ArgumentException)
                    {
                        StaticClass.ShowExceptionMessageBox("Неверный ввод высоты", owner: this);
                        return;
                    }
                    SaveTopoHold(currentHold);
                }

                if (this.holds.Count < 1)
                    order = tag = 1;
                else
                {
                    double maxHold = 0.0;
                    foreach (var h in holds)
                        try
                        {
                            double dVal = ResultListLead.ParseUnversalDouble(h.Tag);
                            if (dVal > maxHold)
                                maxHold = dVal;
                        }
                        catch (ArgumentException) { }


                    this.holds.Sort((a, b) => a.Order.CompareTo(b.Order));
                    order = this.holds[this.holds.Count - 1].Order + 1;
                    tag = (int)maxHold + 1;
                }

                var newHold = new HoldData { Order = order, Tag = tag.ToString(), TopoCorrdinates = pRes, Selected = true };
                currentHold = newHold;
                this.holds.ForEach(hd => hd.Selected = false);
                this.holds.Add(newHold);
                SortHoldsOrder();
                Log.Push(new TopoLogData { Action = TopoLogData.StackAction.Add, Hold = newHold });
                tbHeight.Text = newHold.Tag;
                tbHeight.Enabled = true;
                SaveTopoHold(currentHold);
            }
            DrawHolds();
        }

        private static double Distance(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }

        private Point GetPictureXY(Point p)
        {
            double curScale;
            int yOffset;
            int xOffset;
            GetOffsetData(out curScale, out yOffset, out xOffset);

            if (p.X < xOffset || p.X > topoBox.Width - xOffset || p.Y < yOffset || p.Y > topoBox.Height - yOffset)
                return new Point(-1, -1);
            return new Point((int)((p.X - xOffset) / curScale), (int)((p.Y - yOffset) / curScale));
        }

        private void GetOffsetData(out double curScale, out int yOffset, out int xOffset)
        {
            var BackgroundTopoImage = this.emptyImage;
            var heightScale = (double)topoBox.Height / (double)BackgroundTopoImage.Height;
            var widthScale = (double)topoBox.Width / (double)BackgroundTopoImage.Width;

            curScale = Math.Min(heightScale, widthScale);

            yOffset = (heightScale <= widthScale) ? 0 : (int)((topoBox.Height - BackgroundTopoImage.Height * curScale) / 2.0);
            xOffset = (heightScale >= widthScale) ? 0 : (int)((topoBox.Width - BackgroundTopoImage.Width * curScale) / 2.0);
        }

        private SqlTransaction GetTransaction()
        {
            if (tran == null)
            {
                tran = cn.BeginTransaction();
            }
            return tran;
        }

        private SqlCommand CreateCommand()
        {
            if (cn.State != ConnectionState.Open)
                cn.Open();
            return new SqlCommand { Connection = cn, Transaction = GetTransaction() };
        }

        private void btnNumberHolds_Click(object sender, EventArgs e)
        {
            if (emptyImage == null)
            {
                MessageBox.Show(this, "Схема не загружена");
                return;
            }
            switch (editTopoState)
            {
                case EditTopoState.Clicking:
                    if (currentHold != null && !currentHold.Tag.Equals(tbHeight.Text, StringComparison.Ordinal))
                        SaveCurrentHold(false);
                    if (tran != null)
                    {
                        try
                        {
                            tran.Commit();
                            tran = null;
                        }
                        catch (Exception ex)
                        {
                            StaticClass.ShowExceptionMessageBox("Ошибка сохранения", ex, this);
                        }
                    }
                    StartClicking(false);
                    LoadHolds();
                    DrawHolds();
                    Log.Clear();
                    return;
                case EditTopoState.Edit:
                    SaveCurrentHold();
                    if (tran != null)
                    {
                        try
                        {
                            tran.Commit();
                            tran = null;
                        }
                        catch (Exception ex) { StaticClass.ShowExceptionMessageBox("Ошибка сохранения", ex, this); }
                    }
                    return;
                default:
                    if (holds.Count > 0)
                    {
                        switch (MessageBox.Show(this, String.Format("Продолжить нумерацию?{0}Ответ \"НЕТ\" означает очистить схему", Environment.NewLine),
                            String.Empty, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                        {
                            case System.Windows.Forms.DialogResult.No:
                                holds.Clear();
                                ClearHolds();
                                break;
                            case System.Windows.Forms.DialogResult.Cancel:
                                return;
                        }
                    }
                    DrawHolds();
                    StartClicking(true);
                    currentHold = null;
                    return;
            }

        }

        private bool SaveCurrentHold(bool reDraw = true)
        {
            if (String.IsNullOrEmpty(tbHeight.Text))
            {
                MessageBox.Show(this, "Высота не указана");
                return false;
            }
            String newTag;
            try
            {
                long lTemp;
                newTag = ResultListLead.GetUnifiedHoldTag(tbHeight.Text, false, false, out lTemp);
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Неверный ввод номера зацепки");
                return false;
            }
            tbHeight.Text = newTag;
            foreach (var h in holds)
            {
                if (Object.ReferenceEquals(h, currentHold))
                    continue;
                if (String.Equals(h.Tag, tbHeight.Text, StringComparison.OrdinalIgnoreCase))
                {
                    if (MessageBox.Show(this, String.Format("Зацепка с такой высотой ({0}) уже есть на схеме. Продолжить?", h.Tag), String.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        == System.Windows.Forms.DialogResult.No)
                        return false;
                }
            }
            currentHold.Tag = newTag;
            SaveTopoHold(currentHold);
            if (reDraw)
            {
                StartClicking(false);
                LoadHolds();
                DrawHolds();
            }
            return true;
        }

        private void StartClicking(bool start)
        {
            if ((editTopoState == EditTopoState.Edit || editTopoState == EditTopoState.Clicking) && !start)
            {
                tbHeight.Enabled = lblHeight.Enabled = false;
                tbHeight.Text = String.Empty;
                currentHold = null;
            }
            this.btnLoadOtherTopo.Enabled= this.btnSaveToFile.Enabled = this.btnDelTopo.Enabled = this.btnUploadTopo.Enabled = gbSaveCancel.Enabled = !start;
            btnCancelEdit.Enabled = start;
            btnNumberHolds.Text = start ? "Сохранить данные" : "Пронумеровать зацепки";
            this.editTopoState = start ? EditTopoState.Clicking : EditTopoState.None;
            if (!start)
                btnDelHold.Enabled = false;
        }

        private void btnCancelEdit_Click(object sender, EventArgs e)
        {
            CancelEditTopo();
        }

        private void CancelEditTopo()
        {
            if (tran != null)
            {
                try
                {
                    tran.Rollback();
                    tran = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Ошибка отката транзакции: " + ex.Message);
                    return;
                }
            }
            LoadHolds();
            DrawHolds();
            StartClicking(false);
        }

        private void btnDelTopo_Click(object sender, EventArgs e)
        {
            if (this.emptyImage == null)
                return;
            if (MessageBox.Show(this, "вы уверены, что хотите удалить схему трассы?", String.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                == System.Windows.Forms.DialogResult.No)
                return;
            var cmd = CreateCommand();
            cmd.CommandText = String.Format("DELETE FROM topo_holds WHERE list_id = {0}", this.listId);
            cmd.ExecuteNonQuery();
            cmd.CommandText = String.Format("UPDATE lists SET topo = NULL WHERE iid={0}", this.listId);
            cmd.ExecuteNonQuery();
            this.topoBox.Image = this.emptyImage = null;
        }

        private bool SelectHoldToEdit(Point p, bool replaceCurrent = false)
        {
            var selected = FindHold(p);
            if (selected == null)
                return false;
            if (replaceCurrent)
            {
                if (currentHold != null)
                    currentHold.Selected = false;
            }
            SetEditOneHold(selected);
            return true;
        }

        HoldData currentHold = null;
        private void SetEditOneHold(HoldData holdToEdit)
        {
            btnDelHold.Enabled = btnCancelEdit.Enabled = lblHeight.Enabled = tbHeight.Enabled = true;
            btnLoadOtherTopo.Enabled= btnUploadTopo.Enabled = btnDelTopo.Enabled = btnSaveToFile.Enabled = gbSaveCancel.Enabled = false;

            tbHeight.Text = holdToEdit.Tag;
            currentHold = holdToEdit;
            currentHold.Selected = true;
            btnNumberHolds.Text = "Сохранить данные";

            editTopoState = EditTopoState.Edit;
            DrawHolds();
        }

        private void btnDelHold_Click(object sender, EventArgs e)
        {
            if (currentHold == null || editTopoState != EditTopoState.Edit)
                return;
            try
            {
                var cmd = CreateCommand();
                cmd.CommandText = "DELETE FROM topo_holds WHERE iid=@iid";
                cmd.Parameters.Add("@iid", SqlDbType.Int).Value = currentHold.Iid;
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка удаления: " + ex.Message);
                return;
            }
            holds.Remove(currentHold);
            currentHold.Selected = false;
            Log.Push(new TopoLogData { Hold = currentHold, Action = TopoLogData.StackAction.Delete });
            currentHold = null;
            StartClicking(false);
            DrawHolds();

        }

        private sealed class TopoLogDataStack : Stack<TopoLogData>
        {
            public EditTopo Parent { get; private set; }
            public TopoLogDataStack(EditTopo parent)
            {
                this.Parent = parent;
            }

            public new void Push(TopoLogData item)
            {
                base.Push(item);
                Parent.btnRollback.Invoke(new ThreadStart(() =>
                {
                    if (!Parent.btnRollback.Enabled)
                    {
                        Parent.btnRollback.Enabled = true;
                        Parent.toolTip1.SetToolTip(Parent.btnRollback, ROLBACK);
                    }
                }));
            }

            public new void Clear()
            {
                base.Clear();
                Parent.btnRollback.Invoke(new ThreadStart(() =>
                {
                    Parent.btnRollback.Enabled = false;
                    Parent.toolTip1.RemoveAll();
                }));
            }

            public new TopoLogData Pop()
            {
                try { return base.Pop(); }
                catch (InvalidOperationException) { return null; }
                finally
                {
                    Parent.btnRollback.Invoke(new ThreadStart(() =>
                        {
                            Parent.btnRollback.Enabled = (this.Count > 0);
                            if (!Parent.btnRollback.Enabled)
                                Parent.toolTip1.RemoveAll();
                        }));
                }
            }
        }

        private TopoLogDataStack Log;

        private sealed class TopoLogData
        {
            public enum StackAction { Add, Update, Delete }
            public StackAction Action { get; set; }

            public HoldData Hold { get; set; }
            public Point PositionOld { get; set; }
            public Point PositionNew { get; set; }
        }

        private void SaveTopoHold(HoldData hold)
        {
            var cmd = CreateCommand();
            cmd.Parameters.Add("@iid", SqlDbType.Int);
            cmd.Parameters.Add("@listId", SqlDbType.Int).Value = listId;
            if (hold.Iid > 0)
            {
                cmd.CommandText = "SELECT COUNT(*) FROM topo_holds WHERE iid=@iid and list_id=@listId";
                cmd.Parameters[0].Value = hold.Iid;
                if (Convert.ToInt32(cmd.ExecuteScalar()) < 1)
                    hold.Iid = -1;
                else
                    cmd.CommandText = "UPDATE topo_holds SET hold_id=@holdId, x=@x, y=@y WHERE iid=@iid";
            }
            if (hold.Iid <= 0)
            {
                hold.Iid = (int)SortingClass.GetNextIID("topo_holds", "iid", cmd.Connection, cmd.Transaction);
                cmd.CommandText = "INSERT INTO topo_holds(iid, list_id, hold_id, x, y)" +
                    " VALUES(@iid, @listId, @holdId, @x, @y)";
                cmd.Parameters[0].Value = hold.Iid;
            }
            cmd.Parameters.Add("@holdId", SqlDbType.VarChar, 20).Value = hold.Tag;
            cmd.Parameters.Add("@x", SqlDbType.Int).Value = hold.TopoCorrdinates.X;
            cmd.Parameters.Add("@y", SqlDbType.Int).Value = hold.TopoCorrdinates.Y;

            cmd.ExecuteNonQuery();
        }

        private void RollBackAction()
        {
            var action = Log.Pop();
            if (action == null)
                return;
            StringBuilder sMessage = new StringBuilder("Откатить ");
            switch (action.Action)
            {
                case TopoLogData.StackAction.Add:
                    sMessage.Append("добавление");
                    break;
                case TopoLogData.StackAction.Delete:
                    sMessage.Append("удаление");
                    break;
                case TopoLogData.StackAction.Update:
                    sMessage.Append("перемещение");
                    break;
                default:
                    return;
            }
            sMessage.Append(" зацепки ").Append(action.Hold.Tag).Append("?");
            if (MessageBox.Show(this, sMessage.ToString(), String.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                == System.Windows.Forms.DialogResult.No)
            {
                Log.Push(action);
                return;
            }
            try
            {
                switch (action.Action)
                {
                    case TopoLogData.StackAction.Add:
                        var cmd = CreateCommand();
                        cmd.CommandText = "DELETE FROM topo_holds WHERE iid=@iid";
                        cmd.Parameters.Add("@iid", SqlDbType.Int).Value = action.Hold.Iid;
                        cmd.ExecuteNonQuery();
                        holds.RemoveAll(a => Object.ReferenceEquals(action.Hold, a));
                        currentHold = null;
                        action.Hold.Selected = false;
                        this.StartClicking(true);
                        break;
                    case TopoLogData.StackAction.Delete:
                        SaveTopoHold(action.Hold);
                        holds.RemoveAll(h => Object.ReferenceEquals(h, action.Hold));
                        holds.Add(action.Hold);
                        currentHold = action.Hold;
                        SetEditOneHold(currentHold);
                        editTopoState = EditTopoState.Edit;
                        break;
                    case TopoLogData.StackAction.Update:
                        action.Hold.TopoCorrdinates = action.PositionOld;
                        SaveTopoHold(action.Hold);
                        holds.RemoveAll(h => Object.ReferenceEquals(h, action.Hold));
                        holds.Add(action.Hold);
                        if (currentHold != null)
                            currentHold.Selected = false;
                        currentHold = action.Hold;
                        currentHold.Selected = true;
                        SetEditOneHold(currentHold);
                        editTopoState = EditTopoState.Edit;
                        break;
                    default:
                        return;
                }
                DrawHolds();
            }
            catch (Exception ex)
            {
                StaticClass.ShowExceptionMessageBox("Ошибка отката", ex, this);
            }
        }

        private void btnRollback_Click(object sender, EventArgs e)
        {
            RollBackAction();
        }

        #endregion

        private HoldData FindHold(Point p)
        {
            HoldData selected = null;
            foreach (var h in holds)
                if (Distance(p, h.ScreenCoordinates) < RADIUS)
                {
                    selected = h;
                    break;
                }
            return selected;
        }

        private void topoBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (emptyImage == null)
                return;
            switch (workMode)
            {
                case WorkMode.EditTopo:
                    ProcessClickEditTopo(sender, e);
                    break;
                case WorkMode.ShowResults:
                    ProcessClickEditClimber(sender, e);
                    break;
            }

        }

        private void ProcessClickEditClimber(object sender, MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Left)
                return;
            HoldData hold = FindHold(e.Location);
            if (hold == null)
                return;
            if (hold.Selected)
                hold.Selected = false;
            else
            {
                hold.Selected = true;
                this.tbResult.Text = hold.Tag;
                if (sw != null && sw.IsRunning)
                {
                    var elapsed = sw.Elapsed;
                    tbTime.Text = String.Format("{0:00}:{1:00}", elapsed.Minutes, elapsed.Seconds);
                }
                else
                    tbTime.Text = String.Empty;
                OnTopoClick(new TopoClickEventArgs(hold.Tag, tbTime.Text));
            }
            DrawHolds();
        }

        private sealed class HoldData
        {
            public bool Selected { get; set; }
            public int Iid { get; set; }
            public Point TopoCorrdinates { get; set; }
            public Point ScreenCoordinates { get; set; }
            public String Tag { get; set; }
            public int Order { get; set; }
            public HoldData()
            {
                this.Iid = -1;
                this.Selected = false;
            }
        }

        private void EditTopo_FormClosing(object sender, FormClosingEventArgs e)
        {
            if ((e.CloseReason == CloseReason.UserClosing || e.CloseReason == CloseReason.None) && HasChanges())
            {
                switch (MessageBox.Show(this, "Есть несохраненные изменения. Сохранить?", String.Empty, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                {
                    case System.Windows.Forms.DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                    case System.Windows.Forms.DialogResult.No:
                        try { RollbackFullData(); }
                        catch { }
                        break;
                    default:
                        try
                        {
                            if (tran != null)
                            {
                                tran.Commit();
                                tran = null;
                            }
                        }
                        catch { }
                        break;
                }
            }
            if (!e.Cancel && this.parent != null)
                parent.TimerTopoTick -= this.parent_TimerTopoTick;
        }

        private void topoBox_Resize(object sender, EventArgs e)
        {
            DrawHolds();
        }

        private void btnSaveToFile_Click(object sender, EventArgs e)
        {
            if (emptyImage == null)
            {
                MessageBox.Show(this, "Схема не создана");
                return;
            }
            if (!(saveTopo.ShowDialog(this) == System.Windows.Forms.DialogResult.OK))
                return;
            if (String.IsNullOrEmpty(saveTopo.FileName))
                return;
            ImageFormat fmt = null;
            if (saveTopo.FileName.Length >= 3)
                switch (saveTopo.FileName.Substring(saveTopo.FileName.Length - 3, 3).ToLowerInvariant())
                {
                    case "jpg":
                        fmt = ImageFormat.Jpeg;
                        break;
                    case "png":
                        fmt = ImageFormat.Png;
                        break;
                    case "bmp":
                        fmt = ImageFormat.Bmp;
                        break;
                }
            if (fmt == null)
                fmt = ImageFormat.Jpeg;
            using (var img = DrawHoldsOnEmptyImage())
            {
                using (var f = File.Open(saveTopo.FileName, FileMode.Create, FileAccess.Write))
                {
                    img.Save(f, fmt);
                }
            }
        }

        private void btnColorHold_Click(object sender, EventArgs e)
        {
            bool selected;
            if (sender == btnColorHold)
                selected = false;
            else if (sender == btnColorSelectedHold)
                selected = true;
            else
                return;
            ColorDialog selectColor = new ColorDialog
            {
                AllowFullOpen = true,
                Color = ((Control)sender).BackColor,
                ShowHelp = false
            };
            if (selectColor.ShowDialog(this) != System.Windows.Forms.DialogResult.OK)
                return;
            ((Control)sender).BackColor = selectColor.Color;
            SettingsForm.SetHoldColor(selectColor.Color, selected, cn, tran);
            foreach (var h in holds)
            {
                if (h.Selected != selected)
                    continue;
            }
            DrawHolds();
        }

        private bool checkChanges = true;
        private bool HasChanges()
        {
            if (!checkChanges || tempTable == null || tempTableQuery == null)
                return false;

            if (tran != null)
                return true;
            try
            {
                if (cn.State != ConnectionState.Open)
                    return false;
                return SortingClass.TablesDiffer("topo_holds", tempTable, "iid", cn, tran, tempTableQuery);
            }
            catch (ArgumentException) { return true; }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (workMode == WorkMode.ShowResults)
            {
                switch (e.KeyCode)
                {
                    case Keys.Space:
                        this.Invoke(new ThreadStart(() =>
                        {
                            if (holds == null || holds.Count < 1)
                                return;
                            int lastHoldIndex = -1;
                            int temp = holds.Count;
                            for (int i = 0; i < temp; i++)
                                if (holds[i].Selected)
                                    lastHoldIndex = i;
                            if (lastHoldIndex < 0)
                                lastHoldIndex = 0;
                            else if (lastHoldIndex < (temp - 1))
                                lastHoldIndex++;
                            try
                            {
                                holds[lastHoldIndex].Selected = true;
                                tbResult.Text = holds[lastHoldIndex].Tag;
                                if (sw != null && sw.IsRunning)
                                {
                                    var elapsed = sw.Elapsed;
                                    tbTime.Text = String.Format("{0:00}:{1:00}", elapsed.Minutes, elapsed.Seconds);
                                }
                                else
                                    tbTime.Text = String.Empty;
                                DrawHolds();
                                OnTopoClick(new TopoClickEventArgs(holds[lastHoldIndex].Tag, time: tbTime.Text));
                            }
                            catch { }
                        }));
                        e.Handled = true;
                        break;
                    case Keys.Enter:
                        this.Invoke(new EventHandler(delegate { btnSave_Click(btnSave, new EventArgs()); }));
                        e.Handled = true;
                        break;
                }
            }
            base.OnKeyDown(e);
        }

        private void btnLoadOtherTopo_Click(object sender, EventArgs e)
        {
            var otherGroup = FindOtherLists.SelectList(this, listId, cn, tran);
            if (otherGroup == null)
                return;
            var cmd = CreateCommand();
            cmd.CommandText = "UPDATE lists SET topo=(SELECT topo FROM lists L2(nolock) WHERE L2.iid=@iid2) WHERE iid=@iid";
            cmd.Parameters.Add("@iid", SqlDbType.Int).Value = listId;
            cmd.Parameters.Add("@iid2", SqlDbType.Int).Value = otherGroup.Iid;
            cmd.ExecuteNonQuery();

            cmd.CommandText = "DELETE FROM topo_holds WHERE list_id = @iid";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "SELECT hold_id, x, y" +
                            "  FROM topo_holds(nolock)" +
                            " WHERE list_id=@iid2";
            List<HoldData> buffer = new List<HoldData>();
            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    buffer.Add(new HoldData
                    {
                        Tag = (string)rdr["hold_id"],
                        TopoCorrdinates = new Point(Convert.ToInt32(rdr["x"]), Convert.ToInt32(rdr["y"]))
                    });
                }
            }
            cmd.Parameters.Clear();
            cmd.CommandText = "INSERT INTO topo_holds (iid, list_id, hold_id, x, y)" +
                " VALUES(@iid, @listId, @holdId, @x, @y)";
            cmd.Parameters.Add("@iid", SqlDbType.Int);
            cmd.Parameters.Add("@listId", SqlDbType.Int).Value = listId;
            cmd.Parameters.Add("@holdId", SqlDbType.VarChar, 20);
            cmd.Parameters.Add("@x", SqlDbType.Int);
            cmd.Parameters.Add("@y", SqlDbType.Int);

            foreach (var h in buffer)
            {
                cmd.Parameters["@iid"].Value = SortingClass.GetNextIID("topo_holds", "iid", cmd.Connection, cmd.Transaction);
                cmd.Parameters["@holdId"].Value = h.Tag;
                cmd.Parameters["@x"].Value = h.TopoCorrdinates.X;
                cmd.Parameters["@y"].Value = h.TopoCorrdinates.Y;
                cmd.ExecuteNonQuery();
            }
            this.topoBox.Image = this.emptyImage = LoadTopoFromDb();
            LoadHolds();
            DrawHolds();
        }
    }



    public sealed class TopoClickEventArgs : EventArgs
    {
        public String Result { get; private set; }
        public bool AutoSave { get; private set; }
        public String Time { get; private set; }
        public TopoClickEventArgs(String result, String time = null, bool autoSave = false)
        {
            this.Result = String.IsNullOrEmpty(result) ? String.Empty : result.Trim().ToUpperInvariant();
            this.AutoSave = autoSave;
            this.Time = time;
        }
    }
}
