using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ListShow
{
    public partial class SpeedPhotoControl : ListShow.BasePhotoControl
    {
        public SpeedPhotoControl()
        {
            AllowEvent = false;
            InitializeComponent();
        }

        protected override void DoLayout()
        {
            this.spcMain.SuspendLayout();
            this.spcLeft.SuspendLayout();
            this.spcLeftBottom.SuspendLayout();

            this.Controls.Remove(namesPanel);
            this.spcLeft.Panel1.Controls.Add(namesPanel);
            this.namesPanel.Dock = DockStyle.Fill;

            this.Controls.Remove(pbPhoto);
            this.spcLeftBottom.Panel1.Controls.Add(pbPhoto);
            this.pbPhoto.Dock = DockStyle.Fill;

            this.Controls.Remove(listBox1);
            this.spcLeftBottom.Panel2.Controls.Add(listBox1);
            this.listBox1.Dock = DockStyle.Fill;

            this.spcMain.Dock = DockStyle.Fill;

            this.spcLeftBottom.ResumeLayout(false);
            this.spcLeftBottom.PerformLayout();
            this.spcLeft.ResumeLayout(false);
            this.spcLeft.PerformLayout();
            this.spcMain.ResumeLayout(false);
            this.spcMain.PerformLayout();
            this.ResumeLayout(false);
        }

        private ClimberData clRight = ClimberData.Empty;

        protected override bool SetCurrentList()
        {
            if (currentList < 0 || currentList >= ShowingLists.Count)
                return false;
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            cmd.CommandText = "SELECT nowClimbing l, nowClimbingTmp r FROM lists(NOLOCK) WHERE iid=" + ShowingLists[currentList].iid.ToString();

            int iRight, iLeft;
            SqlDataReader rdr = cmd.ExecuteReader();
            try
            {
                if (rdr.Read())
                {
                    if (rdr["l"] != DBNull.Value)
                        iLeft = Convert.ToInt32(rdr["l"]);
                    else
                        iLeft = int.MinValue;
                    if (rdr["r"] != DBNull.Value)
                        iRight = Convert.ToInt32(rdr["r"]);
                    else
                        iRight = int.MinValue;
                }
                else
                    return false;
            }
            finally { rdr.Close(); }
            if (iLeft <= 0 && iRight <= 0)
                return false;
            ClimberData newLeft, newRight;
            bool leftSet = true, rightSet = true;
            if (iLeft <= 0)
                newLeft = ClimberData.Empty;
            else if (iLeft == currentClimber.Iid)
            {
                leftSet = false;
                newLeft = currentClimber;
            }
            else if (iLeft == clRight.Iid)
                newLeft = clRight;
            else
            {
                newLeft = new ClimberData(iLeft);
                if (!newLeft.LoadData(cn))
                    return false;
            }

            if (iRight <= 0)
                newRight = ClimberData.Empty;
            else if (iRight == clRight.Iid)
            {
                rightSet = false;
                newRight = clRight;
            }
            else if (iRight == currentClimber.Iid)
                newRight = currentClimber;
            else
            {
                newRight = new ClimberData(iRight);
                if (!newRight.LoadData(cn))
                    return false;
            }
            if (this.InvokeRequired)
                this.Invoke(new EventHandler(delegate
                {
                    if (leftSet)
                        SetClimberData(newLeft);
                    if (rightSet)
                        SetClimberToRight(newRight);
                }));
            else
            {
                if (leftSet)
                    SetClimberData(newLeft);
                if (rightSet)
                    SetClimberToRight(newRight);
            }
            currentClimber = newLeft;
            clRight = newRight;
            return true;
        }

        private void SetClimberToRight(ClimberData clm)
        {
            ClimberData c;
            if (clm == null)
                c = ClimberData.Empty;
            else
                c = clm;
            tbRightAge.Text = c.AgeStr;
            tbRightGroup.Text = c.Group;
            tbRightNum.Text = c.IidStr;
            tbRightQf.Text = c.Qf;
            tbRightSurname.Text = c.Name;
            tbRightTeam.Text = c.Team;
            pbRightPhoto.Image = c.Photo;
            c.SetToListBox(listBoxRight);
        }

        private void spcLeftBottom_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (!AllowEvent)
                return;
            if (sender == null || !(sender is SplitContainer))
                return;
            if (EnterEvent())
                try
                {
                    SplitContainer sp = (SplitContainer)sender;
                    sp.SplitterDistance = sp.Width / 2;
                }
                catch { }
                finally { ExitEvent(); }
        }

        private void spcLeft_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (!AllowEvent)
                return;
            if (EnterEvent())
                try
                {
                    if (sender == null || !(sender is SplitContainer))
                        return;
                    SplitContainer spc = (SplitContainer)sender;
                    try
                    {
                        if (spc.SplitterDistance > 230)
                            spc.SplitterDistance = 230;
                    }
                    catch { }
                    if (sender == spcLeft)
                        spcRight.SplitterDistance = spcLeft.SplitterDistance;
                    else
                        spcLeft.SplitterDistance = spcRight.SplitterDistance;
                }
                catch { }
                finally { ExitEvent(); }
        }

        private void spcMain_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (!AllowEvent)
                return;
            if (EnterEvent())
                try { spcMain.SplitterDistance = spcMain.Width / 2; }
                catch { }
                finally { ExitEvent(); }
        }
    }
}
