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
    /// форма с ProgressBar, отображает прогресс при печати вордовых отчётов и бейджиков
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
                    this.Text = "Формирование отчёта";
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
