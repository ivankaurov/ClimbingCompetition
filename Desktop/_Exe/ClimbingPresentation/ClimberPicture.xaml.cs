// <copyright file="ClimberPicture.xaml.cs">
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
using System.Data.SqlClient;
using System.Data;
using ListShow;
using System.IO;
using System.Threading;
using System.Windows.Threading;

namespace ClimbingPresentation
{
    /// <summary>
    /// Interaction logic for ClimberPicture.xaml
    /// </summary>
    public partial class ClimberPicture : UserControl
    {
        public ClimberPicture()
        {
            InitializeComponent();
        }

        private SqlConnection cn;
        public SqlConnection Connection { get { return cn; } set { cn = value; } }

        private ClimberData current = null;

        public ClimberData CurrentClimber
        {
            get { return current; }
            set
            {
                current = value;
                SafeLoad();
            }
        }
        private bool empty = true;
        public bool IsEmpty { get { return empty; } }

        public int CurrentClimberIid
        {
            get { return (current == null ? -1 : current.Iid); }
            set
            {
                if (value < 1)
                    CurrentClimber = null;
                else
                    CurrentClimber = new ClimberData(value);
            }
        }

        private void SafeLoad()
        {
            if (img.Dispatcher.CheckAccess())
                LoadToShow();
            else
                img.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate { LoadToShow(); });
        }

        private void LoadToShow()
        {
            MemoryStream imageSource;
            if (current == null || current.Iid < 1 || current.NoPhoto)
            {
                empty = true;
                imageSource = ListShow.ClimberData.GetEmptyImage();
            }
            else
            {
                if (!current.Loaded)
                {
                    if (cn == null)
                        return;
                    if (cn.State != ConnectionState.Open)
                        cn.Open();
                    current.LoadData(cn);
                }
                imageSource = current.GetImageStream();
                empty = false;
            }
            this.BeginInit();
            BitmapImage bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.StreamSource = imageSource;
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.EndInit();
            imageSource.Close();
            img.Source = bmp;

            this.Visibility = (empty ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible);

            this.EndInit();
        }
    }
}
