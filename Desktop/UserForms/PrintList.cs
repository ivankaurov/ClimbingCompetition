// <copyright file="PrintList.cs">
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
using System.Text;
using System.Drawing;
using System.Drawing.Printing;
using System.Data;
using System.Windows.Forms;

namespace ClimbingCompetition
{
    /// <summary>
    /// ����� ��� ������ ��������� �������� �� ������� (��� Excel)
    /// ���� ������������ ������ ��� ����������� ��������� DEBUG (�������� �����������)
    /// </summary>
    public class PrintList
    {
        DataTable dt;
        PrintDocument docToPrint = new PrintDocument();
        PrintDialog printDialog = new PrintDialog();
        PageSetupDialog pageSetupDialog = new PageSetupDialog();
        PrintPreviewDialog pd = new PrintPreviewDialog();
        Font font = new Font("Courier New", 10f);
        protected int[] columns;

        public PrintList(DataTable dTable)
        {
            docToPrint.PrintPage += new PrintPageEventHandler(docToPrint_PrintPage);
            dt = dTable;

            printDialog.ShowHelp = true;
            printDialog.Document = docToPrint;

            pageSetupDialog.AllowOrientation = pageSetupDialog.AllowMargins = true;
            pageSetupDialog.Document = docToPrint;

            pd.Document = docToPrint;

            columns = new int[dt.Columns.Count];
            for (int columnIndx = 0; columnIndx < dt.Columns.Count; columnIndx++)
            {
                columns[columnIndx] = dt.Columns[columnIndx].ColumnName.Length;
                for (int rowIndx = 0; rowIndx < dt.Rows.Count; rowIndx++)
                    if (dt.Rows[rowIndx][columnIndx].ToString().Length > columns[columnIndx])
                        columns[columnIndx] = dt.Rows[rowIndx][columnIndx].ToString().Length;
                //if (dt.Columns[columnIndx].ColumnName.IndexOf("����") > 0 ||
                //    dt.Columns[columnIndx].ColumnName.IndexOf("�����") > 0)
                //    columns[columnIndx] = (int)(0.75 * (double)columns[columnIndx]);
            }
        }

        public void PrintPreview()
        {
            if (pageSetupDialog.ShowDialog() != DialogResult.OK)
                return;

            rowToPrint = 0;
            HeaderNotPrinted = true;

            try { pd.ShowDialog(); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        public void Print()
        {
            if (pageSetupDialog.ShowDialog() != DialogResult.OK)
                return;

            rowToPrint = 0;
            HeaderNotPrinted = true;

            try
            { docToPrint.Print(); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        int rowToPrint;
        bool HeaderNotPrinted;

        protected virtual float PrintHeader(Font f, ref PrintPageEventArgs ev)
        {
            return PrintColumnsHeaders(ev.MarginBounds.Top, f, ref ev);
        }

        protected virtual float PrintColumnsHeaders(float yStart, Font f, ref PrintPageEventArgs ev)
        {
            float fontH = f.GetHeight(ev.Graphics);
            float yPos = yStart;
            float yNext = yPos + fontH;
            float xPos = ev.MarginBounds.Left;
            ev.Graphics.DrawLine(Pens.Black, xPos, yPos, xPos, yNext);
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                ev.Graphics.DrawString(dt.Columns[i].ColumnName, font, Brushes.Black, xPos, yPos);
                xPos += (float)columns[i] * f.SizeInPoints;
                ev.Graphics.DrawLine(Pens.Black, xPos, yPos, xPos, yNext);
            }
            ev.Graphics.DrawLine(Pens.Black, ev.MarginBounds.Left, yPos, xPos, yPos);
            ev.Graphics.DrawLine(Pens.Black, ev.MarginBounds.Left, yNext, xPos, yNext);
            return yNext;
        }

        void docToPrint_PrintPage(object sender, PrintPageEventArgs ev)
        {
            //float linesPerPage = 0;
            
            //int count = 0;
            float leftMargin = ev.MarginBounds.Left;
            float topMargin = ev.MarginBounds.Top;
            float yPos = topMargin;
            DataRow line;

            float fontH = font.GetHeight(ev.Graphics);

            // Calculate the number of lines per page.
            //linesPerPage = ev.MarginBounds.Height / fontH;

            // Iterate over the file, printing each line.
            float yNext = -1, xPos = 0;

            if (HeaderNotPrinted)
            {
                yPos = PrintHeader(font, ref ev);
                HeaderNotPrinted = false;
            }
            xPos = leftMargin;
            foreach (int f in columns)
                xPos += font.SizeInPoints * (float)f;
            ev.Graphics.DrawLine(Pens.Black, leftMargin, yPos, xPos, yPos);
            while ((yPos+fontH) < ev.MarginBounds.Bottom && rowToPrint < dt.Rows.Count)
            {
                //yPos += font.GetHeight(ev.Graphics);
                yNext = yPos + fontH;
                line = dt.Rows[rowToPrint];
                xPos = leftMargin;
                ev.Graphics.DrawLine(Pens.Black,leftMargin,yPos,leftMargin,yNext);
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    ev.Graphics.DrawString(line[i].ToString(), font, Brushes.Black, xPos, yPos);
                    xPos += (float)columns[i] * font.SizeInPoints;
                    ev.Graphics.DrawLine(Pens.Black, xPos, yPos, xPos, yNext);
                }
                ev.Graphics.DrawLine(Pens.Black, leftMargin, yNext, xPos, yNext);
                rowToPrint++;
                yPos = yNext;
            }
            //if (yNext > 0)
            //    ev.Graphics.DrawLine(Pens.Black, leftMargin, yPos, xPos, yPos);

            // If more lines exist, print another page.
            if (rowToPrint < dt.Rows.Count)
                ev.HasMorePages = true;
            else
            {
                ev.HasMorePages = false;
                rowToPrint = 0;
                HeaderNotPrinted = true;
            }
        }
    }
}
