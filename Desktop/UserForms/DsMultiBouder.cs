// <copyright file="DsMultiBouder.cs">
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

﻿namespace ClimbingCompetition
{


    public partial class DsMultiBouder
    {

        partial class boulderResultsDataTable
        {
            public boulderResultsRow GetRowByIid(long iid)
            {
                for (int i = 0; i < Rows.Count; i++)
                    if (this[i].iid == iid)
                        return this[i];
                return null;
            }
        }

        partial class boulderResultsRow
        {
            public BoulderRoutesRow GetByRouteNumber(int routeNumber)
            {
                BoulderRoutesRow[] rows = this.GetBoulderRoutesRows();
                if (rows == null)
                    return null;
                for (int i = 0; i < rows.Length; i++)
                    if (rows[i].routeN == routeNumber)
                        return rows[i];
                return null;
            }
        }
    }
}
