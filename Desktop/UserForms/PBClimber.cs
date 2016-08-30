// <copyright file="PBClimber.cs">
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
    public class PBClimber
    {
#if FULL
        public enum ResType { NOT_CLIMBED, RES, DSQ, NYA }
        public const int EMPTY_RES = -1;
        private ResType res;
        private string name, team;
        private int start, iid;
        private int top, bonus;
        private bool empty;

        public bool Empty { get { return empty; } }

        public string Team { get { return team; } }

        public string Name { get { return name; } }

        public ResType Res
        {
            get { return res; }
            set { res = value; }
        }

        public int Bonus
        {
            get { return bonus; }
            set { bonus = value; }
        }

        public int Top
        {
            get { return top; }
            set { top = value; }
        }

        public int Start { get { return start; } }

        public int Iid { get { return iid; } }

        public PBClimber(int iid, string name, string team, int start)
        {
            this.iid = iid;
            this.name = name;
            this.team = team;
            this.start = start;
            top = bonus = EMPTY_RES;
            res = ResType.NOT_CLIMBED;
            empty = false;
        }

        public static PBClimber GetEmpty()
        {
            PBClimber c = new PBClimber(0, "", "", 0);
            c.empty = true;
            return c;
        }

        public const string ClmPrefix = "CLIMBER:";
        public const string ClmTail = ":ENDCLM;";

        public override string ToString()
        {
            string str = ClmPrefix;
            if (empty)
                str += "EMPTY=1;";
            else
            {
                str += "RESTYPE=" + res.ToString() + ";";
                if (res == ResType.RES)
                    str += "TOP=" + top.ToString() + ";BONUS=" + bonus.ToString() + ";";
                str += "NAME=" + name + ";TEAM=" + team + ";IID=" + iid.ToString() + ";START=" + start.ToString() + ";";
            }
            if (this.res == ResType.NYA)
                str += "NYA=1;";
            else if (this.res == ResType.DSQ)
                str += "DISQ=1;";
            str += ClmTail;
            return str;
        }

        public static PBClimber GetFromString(string source)
        {
            int k = source.IndexOf(ClmPrefix);
            if (k < 0)
                return PBClimber.GetEmpty();
            int n = source.IndexOf(ClmTail, k);
            if (n <= k)
                return PBClimber.GetEmpty();
            string str = source.Substring(k, n - k);
            if (getParam(str, "EMPTY") != "")
                return PBClimber.GetEmpty();
            int iid;
            string sTmp = getParam(str, "IID");
            try { iid = int.Parse(sTmp); }
            catch { return PBClimber.GetEmpty(); }
            int start;
            sTmp = getParam(str, "START");
            try { start = int.Parse(sTmp); }
            catch { return PBClimber.GetEmpty(); }
            string name = getParam(str, "NAME");
            string team = getParam(str, "TEAM");
            PBClimber clm = new PBClimber(iid, name, team, start);
            sTmp = getParam(str, "RESTYPE");
            ResType rt;
            if (sTmp == ResType.DSQ.ToString())
                rt = ResType.DSQ;
            else if (sTmp == ResType.NOT_CLIMBED.ToString())
                rt = ResType.NOT_CLIMBED;
            else if (sTmp == ResType.NYA.ToString())
                rt = ResType.NYA;
            else if (sTmp == ResType.RES.ToString())
                rt = ResType.RES;
            else
                rt = ResType.NOT_CLIMBED;
            clm.res = rt;
            if (rt == ResType.RES)
            {
                sTmp = getParam(str, "TOP");
                int top;
                try { top = int.Parse(sTmp); }
                catch { top = EMPTY_RES; }
                int bonus;
                sTmp = getParam(str, "BONUS");
                try { bonus = int.Parse(sTmp); }
                catch { bonus = EMPTY_RES; }
                clm.top = top;
                clm.bonus = bonus;
            }
            return clm;
        }

        public static string getParam(string source, string param)
        {
            int k = source.IndexOf(param + "=");
            if (k < 0)
                return "";
            int end = source.IndexOf(';', k);
            if (end <= k)
                return "";
            return source.Substring(k + param.Length + 1, end - (k + param.Length + 1));
        }
#endif
    }
}