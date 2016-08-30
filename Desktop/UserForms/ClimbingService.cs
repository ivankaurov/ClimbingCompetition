// <copyright file="ClimbingService.cs">
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
using System.Text;
using ClimbingCompetition.WebServices;

namespace ClimbingCompetition
{
    public sealed class ClimbingService : ClimbingWebService
    {
        private const int DEFAULT_TIMEOUT 
#if DEBUG
            = 600000;
#else
            = 300000;
#endif
        public new int Timeout
        {
            get { return base.Timeout; }
            private set { base.Timeout = value; }
        }

        public new string Url
        {
            get { return base.Url; }
            private set { base.Url = value; }
        }

        public ClimbingService()
#if DEBUG
            : this("http://localhost:1191/ClimbingWebService.asmx")
#else
            : this("http://climbing-competition.org/ClimbingWebService.asmx")
#endif
        { }

        public ClimbingService(string url)
            : base()
        {
            this.Url = url;
            this.Timeout = DEFAULT_TIMEOUT;
        }
    }
}
