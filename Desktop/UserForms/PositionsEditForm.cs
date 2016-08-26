using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ClimbingCompetition
{
    /// <summary>
    /// Форма для редактирования справочника судейских должностей
    /// </summary>
    public partial class PositionsEditForm : BaseForm
    {
        public PositionsEditForm(SqlConnection baseCn, string competitionTitle):
            base(baseCn, competitionTitle)
        {
            InitializeComponent();
            this.positionsTableAdapter.Connection = cn;
        }

        private void PositionsEditForm_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'dsClimbing.positions' table. You can move, or remove it, as needed.
            this.positionsTableAdapter.Fill(this.dsClimbing.positions);
            // TODO: This line of code loads data into the 'dsClimbing.positions' table. You can move, or remove it, as needed.
            
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                positionsBindingSource.EndEdit();
                positionsTableAdapter.Update(dsClimbing);
                this.Close();
            }
            catch (Exception ex) { MessageBox.Show(this, "Ошибка обновления списка должностей:\r\n" + ex.Message); }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (dgPosView.SelectedRows.Count < 1)
                return;
            if (MessageBox.Show(this, "Вы уверены, что хотите удалить выбранную должность?",
                "Удалить должность", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                return;
            try
            {
                positionsBindingSource.RemoveCurrent();
                positionsBindingSource.EndEdit();
                positionsTableAdapter.Update(dsClimbing);
                //positionsBindingSource.ResetBindings(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Ошибка удаления должностей:\r\n" + ex.Message);
            }
        }
    }
}
