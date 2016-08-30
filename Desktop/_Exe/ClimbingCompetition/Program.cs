// <copyright file="Program.cs">
// Copyright � 2016 All Rights Reserved
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
// (���� ���� � ����� ClimbingCompetition.
// 
// ClimbingCompetition - ��������� ���������: �� ������ ������������������ �� �/���
// �������� �� �� �������� ����������� ������������ �������� GNU � ��� ����,
// � ����� ��� ���� ������������ ������ ���������� ������������ �����������;
// ���� ������ 3 ��������, ���� (�� ������ ������) ����� ����� �������
// ������.
// 
// ClimbingCompetition ���������������� � �������, ��� ��� ����� ��������,
// �� ���� ������ ��������; ���� ��� ������� �������� ��������� ����
// ��� ����������� ��� ������������ �����. ��������� ��. � �����������
// ������������ �������� GNU.
// 
// �� ������ ���� �������� ����� ����������� ������������ �������� GNU
// ������ � ���� ����������. ���� ��� �� ���, ��. <http://www.gnu.org/licenses/>.)
// </copyright>
// <author>Ivan Kaurov</author>
// <date>30.08.2016 23:51</date>

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;

namespace ClimbingCompetition
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try { Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException); }
            catch { }
            try { Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException); }
            catch { }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            string exMessage;
            if (e != null && e.Exception != null)
                exMessage = e.Exception.ToString();
            else
                exMessage = "";
            if (MessageBox.Show("�������������� ������:\r\n" + exMessage +
                "\r\n���� ����� ������ ����� ����������� � ����������, ���������� � ������������.\r\n" +
                "���������� ������?", "�������������� ����������", MessageBoxButtons.YesNo, MessageBoxIcon.Error)
                == DialogResult.No)
                try { Application.Exit(); }
                catch
                {
                    try { Process.GetCurrentProcess().Kill(); }
                    catch (Exception ex2) { MessageBox.Show("���������� ��������� �������:\r\n" + ex2.Message); }
                }
        }
    }
}