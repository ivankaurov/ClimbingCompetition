// <copyright file="NewRemoteConnectionControl.xaml.cs">
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

﻿using ClimbingCompetition.Client;
using ClimbingCompetition.Common.API;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WF = System.Windows.Forms;

namespace ClimbingCompetition.WPF
{
    /// <summary>
    /// Interaction logic for NewRemoteConnectionControl.xaml
    /// </summary>
    public partial class NewRemoteConnectionControl : BaseWpfControl
    {
        private readonly SqlConnection cn;
        private readonly ServiceConnectionModel connectionModel;

        [Obsolete("Designer only", true)]
        public NewRemoteConnectionControl()
        {
            this.connectionModel = new ServiceConnectionModel();
            this.DataContext = this.connectionModel;
            this.InitializeComponent();
        }

        public NewRemoteConnectionControl(SqlConnection cn)
        {
            if (cn == null)
                throw new ArgumentNullException("cn");
            this.cn = cn;
            this.connectionModel = ServiceConnectionModel.FromSql(cn);
            if (this.connectionModel == null)
                this.connectionModel = new ServiceConnectionModel();
            this.DataContext = this.connectionModel;

            InitializeComponent();

            if (this.connectionModel.Competition != null)
                this.cbSelectCompetitions.Items.Add(this.connectionModel.Competition);
            this.pbxPassword.Password = this.connectionModel.Password ?? string.Empty;
        }


        public SqlConnection Cn { get { return this.cn; } }

        private void cbSelectCompetitions_DropDownOpened(object sender, EventArgs e)
        {
            try
            {
                var client = ServiceClient.CreateTemporary(connectionModel.Address, pbxPassword.Password);
                client.BeginGetCompetitions(a => this.Dispatcher.Invoke(new Action(() => this.CompetitionsLoaded(a))), client);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void CompetitionsLoaded(IAsyncResult asyncResult)
        {
            ApiCompetition[] comps;
            try { comps = ((ServiceClient)asyncResult.AsyncState).EndGetCompetitions(asyncResult); }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return;
            }

            if(comps == null || comps.Length < 1)
            {
                MessageBox.Show("Возможно вы ввели неверный пароль. Соревнований с данным паролем не найдено.");
                return;
            }

            this.cbSelectCompetitions.Items.Clear();
            foreach (var e in comps)
                this.cbSelectCompetitions.Items.Add(e);
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(pbxPassword.Password))
            {
                System.Windows.MessageBox.Show("Пароль не введен");
                return;
            }

            if (string.IsNullOrEmpty(this.connectionModel.Address))
            {
                System.Windows.MessageBox.Show("Адрес службы не введен");
                return;
            }

            if (this.connectionModel.Competition == null)
            {
                System.Windows.MessageBox.Show("Соревнования не выбраны");
                return; ;
            }

            try
            {
                var client = ServiceClient.CreateTemporary(connectionModel.Address, pbxPassword.Password);
                client.BeginCheckCompetitionPassword(connectionModel.Competition, pbxPassword.Password,
                    a => Dispatcher.Invoke(new Action(() => this.PasswordCheckCompleted(a))), client);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void PasswordCheckCompleted(IAsyncResult asyncResult)
        {
            try
            {
                var passwordOK = ((ServiceClient)asyncResult.AsyncState).EndCheckPassword(asyncResult);

                if (passwordOK)
                {
                    this.connectionModel.Password = pbxPassword.Password;
                    this.connectionModel.Persist(this.cn);
                    ServiceClient.GetInstance(this.cn).Load(this.cn);
                    this.RaiseDialogClosedEvent(System.Windows.Forms.DialogResult.OK);
                }
                else
                {
                    MessageBox.Show("Неверный пароль");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.RaiseDialogClosedEvent(System.Windows.Forms.DialogResult.Cancel);
        }

        private void pbxPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            this.cbSelectCompetitions.Items.Clear();
            this.cbSelectCompetitions.SelectedItem = null;
        }
    }
}
