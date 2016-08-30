// <copyright file="ReportCreationBar.cs">
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
using Word = Microsoft.Office.Interop.Word;
using Excel = Microsoft.Office.Interop.Excel;
using System.Reflection;

namespace ClimbingCompetition
{
    /// <summary>
    /// ����� � ProgressBar, ���������� �������� ��� ������ �������� ������� � ���������
    /// </summary>
    public partial class ReportCreationBar<T> : Form where T : class
    {
        public delegate T CreateRep(BackgroundWorker bw);

        CreateRep createRep;
        BackgroundWorker bw;
        bool allowClose = false;
        int pCount = 0;

        bool showAfterCompletion = true;
        bool animateHeader = true;
        public bool ShowAfterComplete { get { return showAfterCompletion; } set { showAfterCompletion = value; } }
        public bool AnimatedHeader { get { return animateHeader; } set { animateHeader = value; } }

        public ReportCreationBar(CreateRep createRep, string caption)
            : this(createRep)
        { this.Text = caption; }

        public ReportCreationBar(CreateRep createRep)
        {
            InitializeComponent();
            this.createRep = createRep;
            InitializeBackgroundWorker();
        }

        void InitializeBackgroundWorker()
        {
            bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            bw.WorkerReportsProgress = true;
        }

        void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            timer.Stop();
            allowClose = true;
            if (!e.Cancelled)
            {
                if (e.Error != null)
                    MessageBox.Show(e.Error.Message);
                else
                {
                    if (e.Result != null)
                    {
                        try
                        {
                            //Word.Application app = (Word.Application)e.Result;
                            //if (showAfterCompletion)
                            //{
                            //    app.Visible = true;
                            //    app.Activate();
                            //}
                            
                            if (showAfterCompletion)
                            {
                                object app = e.Result;
                                if (app == null)
                                    return;
                                Type t = app.GetType();
                                PropertyInfo visibleProperty;
                                try { visibleProperty = t.GetProperty("Visible"); }
                                catch { visibleProperty = null; }
                                MethodInfo activateMethod;
                                try { activateMethod = t.GetMethod("Activate", new Type[0]); }
                                catch { activateMethod = null; }
                                if (visibleProperty != null)
                                    visibleProperty.SetValue(app, true, null);
                                if (activateMethod != null)
                                    activateMethod.Invoke(app, null);

                                try { StaticClass.SetExcelVisible(app as Excel.Application); }
                                catch { }
                            }

                        }
                        catch { }
                    }
                }
            }
            Close();
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bwTh = (BackgroundWorker)sender;
            e.Result = createRep(bwTh);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            bw.RunWorkerAsync();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = !allowClose;
            base.OnClosing(e);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (!animateHeader)
                return;
            pCount++;
            if (pCount > 3)
                pCount = 0;
            try
            {
                this.Invoke(new EventHandler(delegate
                {
                    this.Text = "������������ ������";
                    for (int i = 0; i < pCount; i++)
                        this.Text += ".";
                }));
            }
            catch { }
        }
    }

    public sealed class WordReportCreationBar : ReportCreationBar<Word.Application>
    {
        public static void ReplaceLabel(Word.Document doc, Word.Application app, string label, string replaceText, bool replaceAll)
        {
            string repl = (replaceText == null ? String.Empty : replaceText);
            doc.Select();
            object findText = "$[" + label + "]";
            object matchCase = false;
            object matchWholeWord = false;
            object matchWildCards = false;
            object matchSoundsLike = false;
            object matchAllForms = false;
            object forward = true;
            object wrap = Word.WdFindWrap.wdFindContinue;
            object format = false;
            object replaceWith = repl;
            object replace;
            if (replaceAll)
                replace = Word.WdReplace.wdReplaceAll;
            else
                replace = Word.WdReplace.wdReplaceOne;
            app.Selection.Find.ExecuteOld(ref findText, ref matchCase, ref matchWholeWord, ref matchWildCards,
                ref matchSoundsLike, ref matchAllForms, ref forward, ref wrap, ref format, ref replaceWith,
                ref replace);
            /*object line = Word.WdUnit.wdLine;
            object oFalse = false;
            app.Selection.EndKey(ref line, ref oFalse);*/
        }

        public static Word.Selection MoveToLabel(Word.Selection s, string label)
        {
            object oUnit = Word.WdUnits.wdStory;
            object oXtend = false;
            s.HomeKey(ref oUnit, ref oXtend);
            s.Find.ClearFormatting();
            object findText = "$[" + label + "]";
            object matchCase = false;
            object matchWholeWord = false;
            object matchWildCards = false;
            object matchSoundsLike = false;
            object matchAllForms = false;
            object forward = true;
            object wrap = Word.WdFindWrap.wdFindContinue;
            object format = false;
            object replaceWith = "";
            object replace = Word.WdReplace.wdReplaceNone;
            object matchKashida = Type.Missing;
            object matchDiactrics = Type.Missing;
            object matchAliH = Type.Missing;
            object matchCOntrol = Type.Missing;

            s.Find.Execute(ref findText, ref matchCase, ref matchWholeWord, ref matchWildCards,
                ref matchSoundsLike, ref matchAllForms, ref forward, ref wrap, ref format,
                ref replaceWith, ref replace, ref matchKashida, ref matchDiactrics,
                ref matchAliH, ref matchCOntrol);
            return s;
        }

        public WordReportCreationBar(ReportCreationBar<Word.Application>.CreateRep createRep, string caption)
            : base(createRep, caption) { }

        public WordReportCreationBar(ReportCreationBar<Word.Application>.CreateRep createRep) :
            base(createRep) { }
    }

    public sealed class ExcelReportCreationBar : ReportCreationBar<Excel.Application>
    {
        public ExcelReportCreationBar(ReportCreationBar<Excel.Application>.CreateRep createRep, string caption)
            : base(createRep, caption) { }
        public ExcelReportCreationBar(ReportCreationBar<Excel.Application>.CreateRep createRep)
            : base(createRep) { }
    }
}
