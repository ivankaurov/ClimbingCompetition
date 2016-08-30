// <copyright file="SelectCardType.cs">
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

namespace ClimbingCompetition
{
    /// <summary>
    /// Форма для настройки печати карточек участников
    /// </summary>
    public partial class SelectCardType : Form
    {
        
        public SelectCardType()
        {
            ClComp cSet = ClComp.Default;
            InitializeComponent();
#if !FULL
            cbLead.Checked = cbBoulder.Checked = cbLead.Enabled = cbBoulder.Enabled =
                gbLead.Enabled = gbBoulder.Enabled = false;
            cbSpeed.Checked = gbSpeed.Enabled = true;
            cbSpeed.Enabled = false;
#endif
            tbQroute.Text = cSet.tbQroute;
            tbSroute.Text = cSet.tbSroute;
            tbFroute.Text = cSet.tbFroute;
            rbLFlash_CheckedChanged(null, null);
        }

        private void cbBoulder_CheckedChanged(object sender, EventArgs e)
        {
            gbBoulder.Enabled = cbBoulder.Checked;
        }

        private void cbSpeed_CheckedChanged(object sender, EventArgs e)
        {
            gbSpeed.Enabled = cbSpeed.Checked;
        }

        private void cbLead_CheckedChanged(object sender, EventArgs e)
        {
            gbLead.Enabled = cbLead.Checked;
            rbLFlash_CheckedChanged(null, null);
        }

        private void cbBQuali_CheckedChanged(object sender, EventArgs e)
        {
            gbBoulderQuali.Enabled = cbBQuali.Checked;
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            lblSRoute.Enabled = tbSroute.Enabled = cbBSemi.Checked;
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            lblFRoute.Enabled = tbFroute.Enabled = cbBFinal.Checked;
        }

        bool Cancel;

        CardPrint WhatToPrint;

        public CardPrint GetWhatToPrint
        {
            get
            {
                return WhatToPrint;
            }
        }

        private CardPrint GetCardPrint()
        {
            CardPrint retVal = new CardPrint();
            retVal.CANCEL = Cancel;
            retVal.OnlyParticipants = cbOnlyReal.Checked;
            if (Cancel)
                return retVal;
            if (!(cbLead.Checked || cbSpeed.Checked || cbBoulder.Checked))
            {
                MessageBox.Show(this, "Для печати в карточку ничего не выбрано.");
                retVal.CANCEL = true;
                return retVal;
            }
            if (cbLead.Checked)
            {
                Lead l = new Lead();
                l.quali = cbLQuali.Checked;
                l.q1g = rbL1gr.Checked;
                l.q2g = rbL2gr.Checked;
                if (cbLead.Enabled && cbLead.Checked && rbLFlash.Enabled && rbLFlash.Checked)
                {
                    int nRn;
                    if (!int.TryParse(tbFlashRN.Text, out nRn))
                    {
                        MessageBox.Show(this, "Число трасс для квалификации flash введено неверно");
                        retVal.CANCEL = true;
                        return retVal;
                    }
                    l.qFlashRN = nRn;
                }

                l.final = cbLFinal.Checked;
                l.semi = cbLSemi.Checked;
                l.super = cbLSuper.Checked;
                retVal.lead = l;
            }

            if (cbSpeed.Checked)
            {
                Speed s = new Speed();
                if (rbS1q.Checked)
                    s.q1 = !(s.q2 = false);
                else
                    s.q1 = s.q2 = true;
                if (rbSF16.Checked)
                    s.f16 = s.f8 = s.f4 = true;
                else
                {
                    if (rbSF08.Checked)
                    {
                        s.f16 = false;
                        s.f4 = s.f8 = true;
                    }
                    else
                    {
                        s.f16 = s.f8 = false;
                        s.f4 = true;
                    }
                }
                retVal.speed = s;
            }

            if (cbBoulder.Checked)
            {
                int nTmp;
                Boulder b = new Boulder();
                b.quali = cbBQuali.Checked;
                ClComp cSet = ClComp.Default;
                if (b.quali)
                {
                    b.q2g = rbB2g.Checked;
                    if (!int.TryParse(tbQroute.Text, out nTmp))
                    {
                        MessageBox.Show("Число трасс в квалификации боулдеринга введено неверно.");
                        retVal.CANCEL = true;
                        return retVal;
                    }
                    cSet.tbQroute = nTmp.ToString();
                    b.rQuali = nTmp;
                }

                b.semi = cbBSemi.Checked;
                if (b.semi)
                {
                    if (!int.TryParse(tbSroute.Text, out nTmp))
                    {
                        MessageBox.Show("Число трасс в полуфинале боулдеринга введено неверно.");
                        retVal.CANCEL = true;
                        return retVal;
                    }
                    cSet.tbSroute = nTmp.ToString();
                    b.rSemi = nTmp;
                }
                b.final = cbBFinal.Checked;
                if (b.final)
                {
                    if (!int.TryParse(tbFroute.Text, out nTmp))
                    {
                        MessageBox.Show("Число трасс в финале боулдеринга введено неверно.");
                        retVal.CANCEL = true;
                        return retVal;
                    }
                    cSet.tbFroute = nTmp.ToString();
                    b.rFinal = nTmp;
                }
                b.super = cbBSuper.Checked;
                retVal.boulder = b;
            }
            return retVal;
        }

        public class CardPrint
        {
            public Lead lead = null;
            public Speed speed = null;
            public Boulder boulder = null;
            public bool CANCEL = true;
            public bool OnlyParticipants = false;
            public List<RoundToCheck> roundsList = null;
            public List<RoundToCheck> RoundsToCheck
            {
                get
                {
                    if(roundsList != null)
                        return roundsList;
                    roundsList = new List<RoundToCheck>();
                    List<string> roundsToCheck = new List<string>();
                    if (lead != null)
                    {
                        if (lead.quali)
                        {
                            if (lead.q1g)
                               roundsToCheck.Add("1/4 финала");
                            if (lead.q2g)
                            {
                                roundsToCheck.Add("1/4 финала Трасса 1");
                                roundsToCheck.Add("1/4 финала Трасса 2");
                            }
                            if (lead.qFlash)
                                roundsToCheck.Add("Квалификация 1");
                        }
                        if (lead.semi)
                            roundsToCheck.Add("1/2 финала");
                        if (lead.final)
                            roundsToCheck.Add("Финал");
                        if (lead.super)
                            roundsToCheck.Add("Суперфинал");
                        foreach(var s in roundsToCheck)
                            roundsList.Add(new RoundToCheck("routeResults", "Трудность", s));
                    }
                    if (speed != null)
                    {
                        roundsToCheck.Clear();
                        if (speed.q1)
                            roundsToCheck.Add("Квалификация");
                        if (speed.q2)
                            roundsToCheck.Add("Квалификация 2");

                        if (speed.Finals)
                        {
                            roundsToCheck.Add("Финал");
                            roundsToCheck.Add("1/2 финала");
                            if (speed.f8 || speed.f16)
                            {
                                roundsToCheck.Add("1/4 финала");
                                if (speed.f16)
                                    roundsToCheck.Add("1/8 финала");
                            }
                        }
                        foreach (string rnd_r in roundsToCheck)
                            roundsList.Add(new RoundToCheck("speedResults", "Скорость",rnd_r));
                    }
                    if (boulder != null)
                    {
                        roundsToCheck.Clear();
                        if (boulder.quali)
                        {
                            if (boulder.q2g)
                            {
                                roundsToCheck.Add("Квалификация Группа А");
                                roundsToCheck.Add("Квалификация Группа Б");
                            }
                            else
                               roundsToCheck.Add("1/4 финала");
                        }
                        if(boulder.semi)
                            roundsToCheck.Add("1/2 финала");
                        if(boulder.final)
                            roundsToCheck.Add("Финал");
                        foreach(var s in roundsToCheck)
                            roundsList.Add(new RoundToCheck("boulderResults","Боулдеринг",s));
                        if(boulder.super)
                            roundsList.Add(new RoundToCheck("routeResults", "Боулдеринг", "Суперфинал"));
                    }
                    return roundsList;
                }
            }
        }

        public class RoundToCheck
        {
            public string Style { get; private set; }
            public string Table { get; private set; }
            public string Round { get; private set; }
            public RoundToCheck(string table, string style, string round)
            {
                this.Style = style;
                this.Table = table;
                this.Round = round;
            }
        }

        /*
         *  if (!data.OnlyParticipants)
                return true;
           
            return false;
        */
        public class Lead
        {
            public bool quali = false;
            public bool qFlash { get { return qFlashRN > 0; } }
            public int qFlashRN = 0;
            public bool q2g = false;
            public bool q1g = false;
            public bool semi = false;
            public bool final = false;
            public bool super = false;
        }

        public class Boulder
        {
            public bool quali = false;
            public bool q2g = false;
            public bool semi = false;
            public bool final = false;
            public bool super = false;
            public int rQuali = 0;
            public int rSemi = 0;
            public int rFinal = 0;
        }

        public class Speed
        {
            public bool q1 = false;
            public bool q2 = false;
            public bool f16 = false;
            public bool f8 = false;
            public bool f4 = false;
            public bool Finals { get { return f16 || f8 || f4; } }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Cancel = false;
            WhatToPrint = GetCardPrint();
            if (!WhatToPrint.CANCEL)
            {
                if (WhatToPrint.lead != null && WhatToPrint.lead.qFlash)
                {
                    ClComp cSet = ClComp.Default;
                    cSet.CardFlashRN = WhatToPrint.lead.qFlashRN;
                    cSet.Save();
                }
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Cancel = true;
            WhatToPrint = GetCardPrint();
            this.Close();
        }

        private void cbLQuali_CheckedChanged(object sender, EventArgs e)
        {
            rbL1gr.Enabled = rbL2gr.Enabled = rbLFlash.Enabled = cbLQuali.Checked;
        }

        private void rbLFlash_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                tbFlashRN.Enabled = rbLFlash.Checked;
                if (tbFlashRN.Enabled && tbFlashRN.Text.Length < 1)
                    tbFlashRN.Text = ((ClComp.Default.CardFlashRN > 0) ? ClComp.Default.CardFlashRN.ToString() : "");
            }
            catch { }
        }
    }
}