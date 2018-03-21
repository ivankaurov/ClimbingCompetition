using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ClimbingCompetition.WPF;
using System.Data.SqlClient;

namespace ClimbingCompetition.UserForms.WpfIntegration
{
    public partial class WpfContainerForm : BaseForm
    {
        private readonly BaseWpfControl wpfControl;

        [Obsolete("Designer only", true)]
        public WpfContainerForm()
        {
            InitializeComponent();
        }

        public WpfContainerForm(SqlConnection cn, string competitionTitle, IWpfControlFactory controlFactory)
            : base(cn, competitionTitle)
        {
            if (controlFactory == null)
            {
                throw new ArgumentNullException("controlFactory");
            }

            this.InitializeComponent();

            this.wpfControl = controlFactory.CreateControl(this.cn, this.competitionTitle);
            this.elementHost1.Child = this.wpfControl;
            this.wpfControl.DialogClosed += (s, e) =>
            {
                this.DialogResult = e.Data;
                this.Close();
            };
        }

        public BaseWpfControl WpfControl
        {
            get
            {
                return this.wpfControl;
            }
        }
    }
}
