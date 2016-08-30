// <copyright file="ClimberResults.xaml.cs">
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
using System.Data;
using System.Data.SqlClient;
using ListShow;
using ClimbingCompetition;

namespace ClimbingPresentation
{
    /// <summary>
    /// Interaction logic for ClimberResults.xaml
    /// </summary>
    public partial class ClimberResults : UserControl
    {
        private SqlConnection cn;
        public SqlConnection Connection
        {
            get { return cn; }
            set
            {
                cn = value;
                if (cn != null && cn.State != ConnectionState.Open)
                    cn.Open();
            }
        }
        public ClimberResults()
        {
            InitializeComponent();
        }

        private ClimberData climber = null;

        public ClimberData CurrentClimber
        {
            get { return climber; }
            set
            {
                climber = value;
                if (cn != null && value != null && !value.Loaded)
                    value.LoadData(cn);

                ShowResults();
            }
        }
        private void ShowResults()
        {
            lstResView.BeginInit();
            try
            {
                if (climber == null)
                {
                    lstResView.ItemsSource = null;
                    return;
                }
                DataTable dt = climber.GetResultsTable(String.Empty);
                lstResView.ItemsSource = dt.DefaultView;
            }
            finally { lstResView.EndInit(); }
        }
    }

    public sealed class FontStyleConverter : IValueConverter
    {
        public FontWeight HeaderW { get; set; }
        public FontWeight DefaultW { get; set; }


        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DataRowView v = value as DataRowView;
            if (v == null)
                return DefaultW;
            string s = v[0] as string;
            if (s == null)
                return DefaultW;
            if (s.LastIndexOf(':') == s.Length - 1)
                return HeaderW;
            else
                return DefaultW;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
