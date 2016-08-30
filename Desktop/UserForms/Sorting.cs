// <copyright file="Sorting.cs">
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace ClimbingCompetition
{
    /// <summary>
    /// Форма для настройки жеребьёвки
    /// </summary>
    /// 

    public partial class Sorting : Form
    {
        int routeNumber = -1;

        List<Starter> data;

        List<List<Starter>> starterMatrix;
        private bool onlySecondRoute = false;

        public List<Starter> Starters
        {
            get { return this[0]; }
            set { this[0] = value; }
        }

        public List<Starter> Starters2
        {
            get { return this[1]; }
            set { this[1] = value; }
        }

        public List<Starter> this[int n]
        {
            get
            {
                if (starterMatrix == null)
                    return new List<Starter>();
                if (starterMatrix.Count < (n + 1))
                    return new List<Starter>();
                return starterMatrix[n];
            }
            set
            {
                if (starterMatrix == null)
                    starterMatrix = new List<List<Starter>>();
                if (n < starterMatrix.Count)
                    starterMatrix[n] = value;
                else
                {
                    for (int i = starterMatrix.Count; i < n; i++)
                        starterMatrix.Add(new List<Starter>());
                    starterMatrix.Add(value);
                }
            }
        }
        public int Count { get { return starterMatrix.Count; } }

        StartListMode sm;
        bool cncl = false;

        public bool Cancel
        {
            get { return cncl; }
            set { cncl = value; }
        }

        public Sorting(List<Starter> data, StartListMode sm) : this(data, sm, false) { }

        public Sorting(List<Starter> data, StartListMode sm, bool showFlash)
        {
            InitializeComponent();
            this.data = new List<Starter>(data);
            this.sm = sm;
            switch (sm)
            {
                case StartListMode.NotFirstRound:
                    secondRoute.Enabled = false;
                    cbLateAppl.Checked = lateAppl.Enabled = false;
                    prevRound.Enabled = rbPrev.Checked = true;
                    random.Enabled = rbRandom.Checked = false;
                    rb113End.Checked = rb123Reverse.Checked = rb131All.Checked = true;
                    rb111middle.Enabled = rb112Number.Enabled = rb121Random.Enabled = rb122Direct.Enabled =
                        rb132Some.Enabled = false;
                    if (showFlash)
                    {
                        gbRoundFlash.Visible = true;
                        rbPrev_CheckedChanged(null, null);
                    }
                    break;
                case StartListMode.OneRoute:
                    secondRoute.Enabled = false;
                    rbPrev.Enabled = prevRound.Enabled = false;
                    rbRandom.Checked = true;
                    break;
                case StartListMode.QualiFlash:
                    secondRoute.Enabled = true;
                    rbPrev.Enabled = prevRound.Enabled = false;
                    rbRandom.Checked = true;
                    break;
                case StartListMode.TwoRoutes:
                    secondRoute.Enabled = false;
                    rbPrev.Enabled = prevRound.Enabled = false;
                    rbRandom.Checked = true;
                    //rb111middle.Enabled = false;
                    break;
            }
        }

        public Sorting(List<Starter> data, StartListMode sm, int routeNumber)
            : this(data, sm)
        {
            if (sm == StartListMode.QualiFlash)
            {
                SetRouteNumber(routeNumber);
            }
        }

        private void SetRouteNumber(int routeNumber)
        {
            if (this.InvokeRequired)
                this.Invoke(new EventHandler(delegate { SetRouteNumberD(routeNumber); }));
            else
                SetRouteNumberD(routeNumber);
        }

        private void SetRouteNumberD(int routeNumber)
        {
            this.routeNumber = routeNumber;
            if (routeNumber > 2)
            {
                rb31Reverse.Text = "По группам";
                rb33Intl.Enabled = rb34City.Enabled = false;
            }
            else
            {
                rb31Reverse.Text = "В обратном порядке";
                rb33Intl.Enabled = rb34City.Enabled = true;
            }
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void lateAppl_Enter(object sender, EventArgs e)
        {

        }

        private static int RankingDirectCompare(Starter s1, Starter s2)
        {
            if (s1.iid == s2.iid)
                return 0;
            int r1 = s1.HasRanking ? s1.ranking : int.MaxValue,
                r2 = s2.HasRanking ? s2.ranking : int.MaxValue;
            if (r1 != r2)
                return r1.CompareTo(r2);
            return s1.random.CompareTo(s2.random);
        }

        private static int SimpleRandomComparison(Starter s1, Starter s2)
        {
            if (s1.iid == s2.iid)
                return 0;
            return s1.random.CompareTo(s2.random);
        }

        private enum Direction { UP, DOWN, STAND }
        private enum Inserting { FORWARD, BACKWARD }
        private List<Starter>[] NewRandomSort(out bool resF)
        {
            resF = true;
            List<Starter> toSort = new List<Starter>();
            foreach (var s in data)
                toSort.Add(s);
            List<Starter> rankings = new List<Starter>();
            List<Starter> ordinary = new List<Starter>();
            List<Starter> lateAppl = new List<Starter>();

            int ind = 0;
            if (cbLateAppl.Checked)
            {
                while (ind < toSort.Count)
                    if (toSort[ind].lateAppl)
                    {
                        lateAppl.Add(toSort[ind]);
                        toSort.RemoveAt(ind);
                    }
                    else
                        ind++;
                lateAppl.Sort(SimpleRandomComparison);
            }
            if (cbRanking.Checked)
            {
                ind = 0;
                while (ind < toSort.Count)
                    if (toSort[ind].HasRanking)
                    {
                        rankings.Add(toSort[ind]);
                        toSort.RemoveAt(ind);
                    }
                    else
                        ind++;
                if (rb112Number.Checked)
                {
                    int nM = int.Parse(tb12quan.Text);
                    rankings.Sort(RankingDirectCompare);
                    while (rankings.Count > nM && rankings.Count > 0)
                    {
                        ordinary.Add(rankings[rankings.Count - 1]);
                        rankings.RemoveAt(rankings.Count - 1);
                    }
                }
                if (rb121Random.Checked)
                    rankings.Sort(SimpleRandomComparison);
                else if (rb122Direct.Checked)
                    rankings.Sort(RankingDirectCompare);
                else
                {
                    rankings.Sort(RankingDirectCompare);
                    rankings.Reverse();
                }
            }
            while (toSort.Count > 0)
            {
                ordinary.Add(toSort[0]);
                toSort.RemoveAt(0);
            }
            ordinary.Sort(SimpleRandomComparison);





            List<Starter>[] routData = new List<Starter>[(routeNumber < 1) ? 1 : routeNumber];
            for (int i = 0; i < routData.Length; i++)
                routData[i] = new List<Starter>();
            Direction d = Direction.STAND;
            Inserting inst = Inserting.BACKWARD;
            int cuR = 0;

            if (rankings.Count > 0)
            {
                if (rb111middle.Checked)
                {
                    inst = Inserting.FORWARD;
                    List<Starter> toInsert = rankings;
                    InsertToMiddle(routData, ref d, ref inst, ref cuR, rankings);
                    InsertToMiddle(routData, ref d, ref inst, ref cuR, ordinary);
                    InsertToMiddle(routData, ref d, ref inst, ref cuR, lateAppl);
                }
                else if (rb114Beg.Checked)
                {
                    InsertToBack(routData, ref d, ref cuR, rankings, Inserting.BACKWARD);
                    lateAppl.Sort(RankingDirectCompare);
                    if (rb22Beginning.Checked)
                        InsertToBack(routData, ref d, ref cuR, lateAppl, Inserting.BACKWARD);
                    InsertToBack(routData, ref d, ref cuR, ordinary, Inserting.BACKWARD);
                    if (rb21End.Checked)
                        InsertToBack(routData, ref d, ref cuR, rankings, Inserting.BACKWARD);
                }
                else if (rb113End.Checked)
                {
                    rankings.Reverse();
                    InsertToBack(routData, ref d, ref cuR, rankings, Inserting.FORWARD);
                    lateAppl.Sort(RankingDirectCompare);
                    if (rb21End.Checked)
                        InsertToBack(routData, ref d, ref cuR, lateAppl, Inserting.FORWARD);
                    InsertToBack(routData, ref d, ref cuR, ordinary, Inserting.FORWARD);
                    if (rb22Beginning.Checked)
                        InsertToBack(routData, ref d, ref cuR, lateAppl, Inserting.FORWARD);
                }

                else
                {
                    int[] inserted = new int[routData.Length];
                    for (int ijk = 0; ijk < inserted.Length; ijk++)
                        inserted[ijk] = 0;
                    int PSTART;
                    if (!int.TryParse(tb11stNum.Text, out PSTART))
                    {
                        MessageBox.Show("Ст.Номер введён неверно.");
                        resF = false;
                        return null;
                    }
                    PSTART = (PSTART - 1) * routData.Length;
                    List<Starter> InsertFirst = new List<Starter>();
                    if (rb22Beginning.Checked)
                        while (InsertFirst.Count <= PSTART && lateAppl.Count > 0)
                        {
                            InsertFirst.Add(lateAppl[0]);
                            lateAppl.RemoveAt(0);
                        }
                    while (ordinary.Count > 0 && InsertFirst.Count <= PSTART)
                    {
                        InsertFirst.Add(ordinary[0]);
                        ordinary.RemoveAt(0);
                    }
                    InsertToBack(routData, ref d, ref cuR, rankings, Inserting.BACKWARD);
                    InsertFirst.Reverse();
                    InsertToBack(routData, ref d, ref cuR, InsertFirst, Inserting.FORWARD);
                    if (rb22Beginning.Checked)
                        InsertToBack(routData, ref d, ref cuR, lateAppl, Inserting.BACKWARD);
                    InsertToBack(routData, ref d, ref cuR, ordinary, Inserting.BACKWARD);
                    if (rb21End.Checked)
                        InsertToBack(routData, ref d, ref cuR, lateAppl, Inserting.BACKWARD);
                }
            }
            else
            {
                if (rb22Beginning.Checked)
                    InsertToBack(routData, ref d, ref cuR, lateAppl, Inserting.BACKWARD);
                InsertToBack(routData, ref d, ref cuR, ordinary, Inserting.BACKWARD);
                if (rb21End.Checked)
                    InsertToBack(routData, ref d, ref cuR, lateAppl, Inserting.BACKWARD);
            }

            if (sm == StartListMode.QualiFlash && routData != null && routData.Length == 2 && routData[0].Count > routData[1].Count && rb33Intl.Checked)
            {
                var tmp = routData[0];
                routData[0] = routData[1];
                routData[1] = tmp;
            }
            return routData;
        }

        private static void InsertToBack(List<Starter>[] routData, ref Direction d, ref int cuR, List<Starter> toInsert, Inserting inst)
        {
            while (toInsert.Count > 0)
            {
                Starter cur = toInsert[0];
                toInsert.RemoveAt(0);
                if (inst == Inserting.BACKWARD)
                    routData[cuR].Add(cur);
                else
                    routData[cuR].Insert(0, cur);
                ModifyCurrentRouteToInsert(routData, ref d, ref cuR);
            }
        }

        private static void InsertToMiddle(List<Starter>[] routData, ref Direction d, ref Inserting inst, ref int cuR, List<Starter> toInsert)
        {
            while (toInsert.Count > 0)
            {
                Starter cur = toInsert[0];
                toInsert.RemoveAt(0);
                if (inst == Inserting.FORWARD)
                {
                    routData[cuR].Insert(0, cur);
                    inst = Inserting.BACKWARD;
                }
                else
                {
                    routData[cuR].Add(cur);
                    inst = Inserting.FORWARD;
                }
                ModifyCurrentRouteToInsert(routData, ref d, ref cuR);
            }
        }

        private static void ModifyCurrentRouteToInsert(List<Starter>[] routData, ref Direction d, ref int cuR)
        {
            if (cuR == 0)
            {
                if (d == Direction.DOWN)
                    d = Direction.STAND;
                else
                    d = Direction.UP;
            }
            else if (cuR == routData.Length - 1)
            {
                if (d == Direction.UP)
                    d = Direction.STAND;
                else
                    d = Direction.DOWN;
            }
            if (cuR < (routData.Length - 1) && d == Direction.UP)
                cuR++;
            else if (cuR > 0 && d == Direction.DOWN)
                cuR--;
        }

        [Flags]
        private enum RandomSortingType : long
        {
            Ranking = 0x1,
            RankingPosStart = 0x2,
            RankingPosMiddle = 0x4,
            RankingPosEnd = 0x8,
            RankingPosCertainNumber = 0x10,
            RankingOrderDisrect = 0x20,
            RankingOrderReverse = 0x40,
            RankingOrderRandom = 0x80,
            RankingUseAll = 0x100,
            RankingUseCertain = 0x200,
            LateAppl = 0x400,
            LateApplPosStart = 0x800,
            LateApplPosEnd = 0x1000,
            PrevRound = 0x2000,
            PrevRoundFieldStart = 0x4000,
            PrevRoundFieldPos = 0x8000,
            PrevRoundDirectionSame = 0x10000,
            PrevRoundDirectionReverse = 0x20000
        }

        private static void ClearList(List<Starter> toClear, List<Starter> whatToClear)
        {
            List<Starter> toRemove = new List<Starter>();
            foreach (var v in toClear)
            {
                foreach (var r in whatToClear)
                    if (r.iid == v.iid)
                    {
                        toRemove.Add(v);
                        break;
                    }
            }
            foreach (var v in toRemove)
                toClear.Remove(v);
        }

        private static List<Starter> SetRecursiveSorting(List<Starter> group, FieldInfo fieldToUse, RandomSortingType currentS, RandomSortingType filterS, int? rankingCount, int? rankingPosStart)
        {
            if (group.Count < 2)
                return group;
            List<Starter> toSortList = new List<Starter>();
            List<Starter> resList = new List<Starter>();
            toSortList.Add(group[0]);
            long fieldVal = Convert.ToInt64(fieldToUse.GetValue(group[0]));
            long cFVal;
            for (int i = 1; i < group.Count; i++)
            {
                cFVal = Convert.ToInt64(fieldToUse.GetValue(group[i]));
                if (cFVal != fieldVal)
                {
                    if (toSortList.Count > 1)
                        toSortList = sortGroup(toSortList, currentS & (~filterS), rankingCount, rankingPosStart);
                    foreach (var v in toSortList)
                        resList.Add(v);
                    toSortList.Clear();
                    fieldVal = cFVal;
                }
                toSortList.Add(group[i]);
            }

            if (toSortList.Count > 1)
                toSortList = sortGroup(toSortList, currentS & (~filterS), rankingCount, rankingPosStart);
            foreach (var v in toSortList)
                resList.Add(v);
            return resList;
        }

        private static List<Starter> sortGroup(List<Starter> group, RandomSortingType sortingType, int? rankingCount, int? rankingPosStart)
        {
            FieldInfo fieldToUse;
            List<Starter> prevRound = new List<Starter>();
            int totalCount = group.Count;

            if ((sortingType & RandomSortingType.PrevRound) == RandomSortingType.PrevRound)
            {
                foreach (var s in group)
                    if (s.prevPos > 0 && s.prevPos < int.MaxValue)
                        prevRound.Add(s);
                ClearList(group, prevRound);


                if ((sortingType & RandomSortingType.PrevRoundFieldStart) == RandomSortingType.PrevRoundFieldStart)
                {
                    fieldToUse = typeof(Starter).GetField("prevStart");
                    prevRound.Sort((a, b) => a.prevStart.CompareTo(b.prevStart));
                }
                else
                {
                    fieldToUse = typeof(Starter).GetField("prevPos");
                    prevRound.Sort((a, b) => a.prevPos.CompareTo(b.prevPos));
                }
                if ((sortingType & RandomSortingType.PrevRoundDirectionReverse) == RandomSortingType.PrevRoundDirectionReverse)
                    prevRound.Reverse();
                prevRound = SetRecursiveSorting(prevRound, fieldToUse, sortingType, RandomSortingType.PrevRound, rankingCount, rankingPosStart);
            }

            List<Starter> lateAppl = new List<Starter>();
            if ((sortingType & RandomSortingType.LateAppl) == RandomSortingType.LateAppl)
            {
                foreach (var s in group)
                    if (s.lateAppl)
                        lateAppl.Add(s);
                ClearList(group, lateAppl);

                lateAppl = sortGroup(lateAppl, sortingType & (~RandomSortingType.LateAppl), rankingCount, rankingPosStart);
            }

            List<Starter> rankings = new List<Starter>();
            if ((sortingType & RandomSortingType.Ranking) == RandomSortingType.Ranking)
            {
                group.Sort(new Comparison<Starter>(delegate(Starter a, Starter b)
                {
                    if (a.HasRanking && !b.HasRanking)
                        return -1;
                    if (!a.HasRanking && b.HasRanking)
                        return 1;
                    if (!a.HasRanking && !b.HasRanking)
                        return 0;
                    return a.ranking.CompareTo(b.ranking);
                }));


                if ((sortingType & RandomSortingType.RankingUseCertain) == RandomSortingType.RankingUseCertain)
                {
                    if (rankingCount == null || !rankingCount.HasValue)
                        throw new ArgumentNullException("rankingCount");
                    int? lastRank = null;
                    foreach (var s in group)
                        if (s.HasRanking &&
                            (rankings.Count < rankingCount.Value || lastRank == null || s.ranking == lastRank.Value))
                        {
                            rankings.Add(s);
                            lastRank = s.ranking;
                        }
                }
                else
                    foreach (var s in group)
                        if (s.HasRanking)
                            rankings.Add(s);
                if ((sortingType & RandomSortingType.RankingOrderRandom) == RandomSortingType.RankingOrderRandom)
                    rankings.Sort((a, b) => a.random.CompareTo(b.random));
                else if ((sortingType & RandomSortingType.RankingOrderReverse) == RandomSortingType.RankingOrderReverse)
                    rankings.Reverse();
                fieldToUse = typeof(Starter).GetField("ranking");

                rankings = SetRecursiveSorting(rankings, fieldToUse, sortingType, RandomSortingType.Ranking, rankingCount, rankingPosStart);

                ClearList(group, rankings);
            }

            group.Sort((a, b) => a.random.CompareTo(b.random));

            List<Starter> resList = new List<Starter>();

            if (prevRound.Count > 0)
            {
                foreach (var v in prevRound)
                    resList.Add(v);
                if ((sortingType & RandomSortingType.PrevRoundDirectionSame) == RandomSortingType.PrevRoundDirectionSame)
                {
                    foreach (var v in rankings)
                        resList.Add(v);
                    foreach (var v in group)
                        resList.Add(v);
                }
                else
                {
                    foreach (var v in rankings)
                        resList.Insert(0, v);
                    foreach (var v in group)
                        resList.Insert(0, v);
                }
                if (lateAppl.Count > 0)
                {
                    if ((sortingType & RandomSortingType.LateApplPosStart) == RandomSortingType.LateApplPosStart)
                        for (int i = 0; i < lateAppl.Count; i++)
                            resList.Insert(i, lateAppl[i]);
                    else
                        foreach (var v in lateAppl)
                            resList.Add(v);
                }
                return resList;
            }

            if (rankings.Count > 0)
            {
                if ((sortingType & RandomSortingType.RankingPosCertainNumber) == RandomSortingType.RankingPosCertainNumber)
                {
                    if (rankingPosStart == null || !rankingPosStart.HasValue)
                        throw new ArgumentNullException("rankingPosStart");

                    while (resList.Count < rankingPosStart.Value)
                    {
                        if (lateAppl.Count > 0 && (sortingType & RandomSortingType.LateApplPosStart) == RandomSortingType.LateApplPosStart)
                        {
                            resList.Add(lateAppl[0]);
                            lateAppl.RemoveAt(0);
                        }
                        else if (group.Count > 0)
                        {
                            resList.Add(group[0]);
                            group.RemoveAt(0);
                        }
                        else if (lateAppl.Count > 0 && (sortingType & RandomSortingType.LateApplPosStart) != RandomSortingType.LateApplPosStart)
                        {
                            resList.Add(lateAppl[0]);
                            lateAppl.RemoveAt(0);
                        }
                    }

                    foreach (var v in rankings)
                        resList.Add(v);

                    if ((sortingType & RandomSortingType.LateApplPosStart) == RandomSortingType.LateApplPosStart)
                        foreach (var v in lateAppl)
                            resList.Add(v);
                    foreach (var v in group)
                        resList.Add(v);
                    foreach (var v in lateAppl)
                        resList.Add(v);
                    return resList;
                }
            }

            return null;
        }

        private static List<Starter>[] SplitToGroups(List<Starter> srcList, int groupCount)
        {

            List<Starter>[] resList = new List<Starter>[groupCount];
            for (int i = 0; i < groupCount; i++)
                resList[i] = new List<Starter>();

            srcList.Sort(new Comparison<Starter>(delegate(Starter a, Starter b)
            {
                if (a.iid == b.iid)
                    return 0;
                int n = b.HasRanking.CompareTo(a.HasRanking);
                if (n != 0)
                    return n;
                if (a.HasRanking)
                {
                    n = a.ranking.CompareTo(b.ranking);
                    return (n == 0) ? a.random.CompareTo(b.random) : n;
                }
                else
                    return a.random.CompareTo(b.random);
            }));

            int step = 1;
            int route = 0;
            for (int i = 0; i < srcList.Count; i++)
            {
                resList[route].Add(srcList[i]);
                route += step;
                if (route >= srcList.Count || route < 0)
                {
                    route -= step;
                    step = -step;
                }
            }
            return resList;
        }

        private void sortRandom()
        {
            bool resF;
            if (sm == StartListMode.QualiFlash && (routeNumber == 2 && rb33Intl.Checked ||
                routeNumber > 2 && !rb32SameOrder.Checked))
            {
                List<Starter>[] lists_gen = NewRandomSort(out resF);
                if (resF)
                {
                    for (int firstGroup = 0; firstGroup < lists_gen.Length; firstGroup++)
                    {
                        this[firstGroup] = new List<Starter>();
                        int cG = firstGroup;
                        do
                        {
                            foreach (var s in lists_gen[cG])
                                this[firstGroup].Add(s);
                            cG--;
                            if (cG < 0)
                                cG = lists_gen.Length - 1;
                        } while (cG != firstGroup && cG >= 0);
                    }
                }
                return;
            }
            List<Starter> lateApplList = new List<Starter>();
            List<Starter> ranking = new List<Starter>();
            List<Starter> other = new List<Starter>();
            Starters = new List<Starter>();
            Starters2 = new List<Starter>();
            randomComparer rComp = new randomComparer();
            rankingSorter rankSorn = new rankingSorter();
            lateApplComparer lateComp = new lateApplComparer();
            foreach (Starter str in data)
            {
                if (str.lateAppl && cbLateAppl.Checked)
                    lateApplList.Add(str);
                else
                {
                    if (str.ranking > 0)
                        ranking.Add(str);
                    else
                        other.Add(str);
                }
            }

            // Выделение рейтинговой группы
            #region RankingSelction

            List<Starter> ranking2 = new List<Starter>();

            if (cbRanking.Checked)
            {
                if (rb132Some.Checked || rb131All.Checked)
                {
                    try
                    {

                        rankingSorter rst = new rankingSorter();
                        ranking.Sort(rst);
                        int nTmp2;
                        if (rb132Some.Checked)
                        {
                            int nTmp = int.Parse(tb12quan.Text);
                            if (nTmp < ranking.Count)
                            {
                                nTmp2 = nTmp;
                                for (int i = nTmp; i < ranking.Count; i++)
                                    if (((Starter)data[i]).ranking != ((Starter)data[nTmp - 1]).ranking)
                                    {
                                        nTmp2 = i;
                                        break;
                                    }
                            }
                            else
                                nTmp2 = ranking.Count;
                        }
                        else
                            nTmp2 = ranking.Count;
                        while (ranking.Count > nTmp2)
                        {
                            Starter stm = (Starter)ranking[ranking.Count - 1];
                            other.Add(stm);
                            ranking.RemoveAt(ranking.Count - 1);
                        }
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message); return; }

                    if (sm == StartListMode.TwoRoutes && !rb111middle.Checked)
                        for (int i = 1; i < ranking.Count; i += 2)
                            for (int j = 0; j < 2; j++)
                            {
                                if ((i == ranking.Count - 1 && ranking.Count > ranking2.Count) || i < ranking.Count - 1)
                                {
                                    ranking2.Add((Starter)ranking[i]);
                                    ranking.RemoveAt(i);
                                }
                            }

                    if (rb123Reverse.Checked)
                    {
                        ranking.Reverse();
                        ranking2.Reverse();
                    }

                    if (rb121Random.Checked)
                    {
                        ranking.Sort(rComp);
                        ranking2.Sort(rComp);
                    }

                }
            }
            else
            {
                //Если не надо выделять рейтингвоых
                foreach (Starter stm in ranking)
                    other.Add(stm);
                ranking = new List<Starter>();
            }
            #endregion

            // Обработка поздних заявок
            #region LateAppl
            if (cbLateAppl.Checked)
            {
                if (cbRanking.Checked)
                {
                    lateApplList.Sort(lateComp);
                    if (rb22Beginning.Checked)
                        lateApplList.Reverse();
                }
                else
                    lateApplList.Sort(rComp);
            }
            #endregion

            other.Sort(rComp);

            //Создание протокола
            #region ListCreation
            bool scndRoute = false;
            //Если рейтиновые идут с номера N
            if (rb112Number.Checked)
            {
                Starters.Clear();
                #region rnakingN
                int rankingStart;
                try { rankingStart = int.Parse(tb11stNum.Text) - 1; }
                catch (Exception Exception)
                {
                    MessageBox.Show(Exception.Message);
                    return;
                }

                //Если поздние заявки в начало
                if (rb22Beginning.Checked)
                {
                    while (((Starters.Count < rankingStart) ||
                        (Starters2.Count < rankingStart && sm == StartListMode.TwoRoutes)) &&
                        lateApplList.Count > 0)
                    {
                        if (sm == StartListMode.TwoRoutes && scndRoute)
                            Starters2.Add(lateApplList[0]);
                        else
                            Starters.Add(lateApplList[0]);
                        scndRoute = !scndRoute;
                        lateApplList.RemoveAt(0);
                    }
                }
                while (((Starters.Count < rankingStart) ||
                        (Starters2.Count < rankingStart && sm == StartListMode.TwoRoutes))
                        && other.Count > 0)
                {
                    if (sm == StartListMode.TwoRoutes && scndRoute)
                        Starters2.Add(other[0]);
                    else
                        Starters.Add(other[0]);
                    scndRoute = !scndRoute;
                    other.RemoveAt(0);
                }
                //Рейтинговая группа
                if (sm == StartListMode.TwoRoutes)
                {
                    if ((ranking.Count < ranking2.Count) && (Starters.Count < Starters2.Count) ||
                        (ranking.Count > ranking2.Count) && (Starters.Count > Starters2.Count))
                    {
                        while (ranking.Count > 0)
                        {
                            Starters2.Add(ranking[0]);
                            ranking.RemoveAt(0);
                        }
                        while (ranking2.Count > 0)
                        {
                            Starters.Add(ranking2[0]);
                            ranking2.RemoveAt(0);
                        }
                    }
                    else
                    {
                        while (ranking.Count > 0)
                        {
                            Starters.Add(ranking[0]);
                            ranking.RemoveAt(0);
                        }
                        while (ranking2.Count > 0)
                        {
                            Starters2.Add(ranking2[0]);
                            ranking2.RemoveAt(0);
                        }
                    }

                }
                else
                    while (ranking.Count > 0)
                    {
                        Starters.Add(ranking[0]);
                        ranking.RemoveAt(0);
                    }

                scndRoute = (Starters.Count > Starters2.Count);
                //Оставшиеся поздние заявки
                if (rb22Beginning.Checked)
                {
                    while (lateApplList.Count > 0)
                    {
                        if (sm == StartListMode.TwoRoutes && scndRoute)
                            Starters2.Add(lateApplList[0]);
                        else
                            Starters.Add(lateApplList[0]);
                        scndRoute = !scndRoute;
                        lateApplList.RemoveAt(0);
                    }
                }
                //нерейинговая группа
                while (other.Count > 0)
                {
                    if (sm == StartListMode.TwoRoutes && scndRoute)
                        Starters2.Add(other[0]);
                    else
                        Starters.Add(other[0]);
                    scndRoute = !scndRoute;
                    other.RemoveAt(0);
                }

                //Если поздние заявки в конец
                if (rb21End.Checked)
                {
                    while (lateApplList.Count > 0)
                    {
                        if (sm == StartListMode.TwoRoutes && scndRoute)
                            Starters2.Add(lateApplList[0]);
                        else
                            Starters.Add(lateApplList[0]);
                        scndRoute = !scndRoute;
                        lateApplList.RemoveAt(0);
                    }
                }
                #endregion
            }

            //Еслм рейтинговые идут в конце
            if (rb113End.Checked)
            {
                Starters.Clear();
                #region rankingEnd
                //Если поздние заявки в начало
                if (rb22Beginning.Checked)
                    while (lateApplList.Count > 0)
                    {
                        if (sm == StartListMode.TwoRoutes && scndRoute)
                            Starters2.Add(lateApplList[0]);
                        else
                            Starters.Add(lateApplList[0]);
                        scndRoute = !scndRoute;
                        lateApplList.RemoveAt(0);
                    }
                while (other.Count > 0)
                {
                    if (sm == StartListMode.TwoRoutes && scndRoute)
                        Starters2.Add(other[0]);
                    else
                        Starters.Add(other[0]);
                    scndRoute = !scndRoute;
                    other.RemoveAt(0);
                }

                //Если поздние заявки в конец
                if (rb21End.Checked)
                    while (lateApplList.Count > 0)
                    {
                        if (sm == StartListMode.TwoRoutes && scndRoute)
                            Starters2.Add(lateApplList[0]);
                        else
                            Starters.Add(lateApplList[0]);
                        scndRoute = !scndRoute;
                        lateApplList.RemoveAt(0);
                    }

                if (sm == StartListMode.TwoRoutes)
                {
                    if (Starters.Count > Starters2.Count &&
                        ranking2.Count > ranking.Count)
                    {
                        while (ranking.Count > 0)
                        {
                            Starters2.Add(ranking[0]);
                            ranking.RemoveAt(0);
                        }
                        while (ranking2.Count > 0)
                        {
                            Starters.Add(ranking2[0]);
                            ranking2.RemoveAt(0);
                        }
                    }
                    else
                    {
                        while (ranking.Count > 0)
                        {
                            Starters.Add(ranking[0]);
                            ranking.RemoveAt(0);
                        }
                        while (ranking2.Count > 0)
                        {
                            Starters2.Add(ranking2[0]);
                            ranking2.RemoveAt(0);
                        }
                    }
                }
                else
                    while (ranking.Count > 0)
                    {
                        Starters.Add(ranking[0]);
                        ranking.RemoveAt(0);
                    }
                #endregion
            }

            //Еслм рейтинговые идут в начале
            if (rb114Beg.Checked)
            {
                lateApplList.Reverse();
                Starters.Clear();
                #region rankingEnd

                if (sm == StartListMode.TwoRoutes)
                {
                    if (Starters.Count > Starters2.Count &&
                        ranking2.Count > ranking.Count)
                    {
                        while (ranking.Count > 0)
                        {
                            Starters2.Add(ranking[0]);
                            ranking.RemoveAt(0);
                        }
                        while (ranking2.Count > 0)
                        {
                            Starters.Add(ranking2[0]);
                            ranking2.RemoveAt(0);
                        }
                    }
                    else
                    {
                        while (ranking.Count > 0)
                        {
                            Starters.Add(ranking[0]);
                            ranking.RemoveAt(0);
                        }
                        while (ranking2.Count > 0)
                        {
                            Starters2.Add(ranking2[0]);
                            ranking2.RemoveAt(0);
                        }
                    }
                }
                else
                    while (ranking.Count > 0)
                    {
                        Starters.Add(ranking[0]);
                        ranking.RemoveAt(0);
                    }

                //Если поздние заявки в начало
                if (rb22Beginning.Checked)
                    while (lateApplList.Count > 0)
                    {
                        if (sm == StartListMode.TwoRoutes && scndRoute)
                            Starters2.Add(lateApplList[0]);
                        else
                            Starters.Add(lateApplList[0]);
                        scndRoute = !scndRoute;
                        lateApplList.RemoveAt(0);
                    }
                while (other.Count > 0)
                {
                    if (sm == StartListMode.TwoRoutes && scndRoute)
                        Starters2.Add(other[0]);
                    else
                        Starters.Add(other[0]);
                    scndRoute = !scndRoute;
                    other.RemoveAt(0);
                }

                //Если поздние заявки в конец
                if (rb21End.Checked)
                    while (lateApplList.Count > 0)
                    {
                        if (sm == StartListMode.TwoRoutes && scndRoute)
                            Starters2.Add(lateApplList[0]);
                        else
                            Starters.Add(lateApplList[0]);
                        scndRoute = !scndRoute;
                        lateApplList.RemoveAt(0);
                    }


                #endregion
            }

            //Если рейтинговые идут из середины
            if (rb111middle.Checked)
            {
                Starters.Clear();
                #region rankingMiddle
                Starter[] lst = new Starter[other.Count + ranking.Count + lateApplList.Count];
                int mid = Convert.ToInt32((double)(lst.Length) / 2.0 - 0.01);
                int dir = 0;
                while (ranking.Count > 0)
                {
                    lst[mid + dir] = (Starter)ranking[0];
                    if (dir == 0)
                        dir = -1;
                    else
                    {
                        if (dir < 0)
                            dir = -dir;
                        else
                            dir = -(dir + 1);
                    }
                    ranking.RemoveAt(0);
                }
                int indxx;
                if (rb21End.Checked)
                {
                    indxx = lst.Length;
                    while (lateApplList.Count > 0)
                    {
                        while (lst[--indxx] != null) ;
                        lst[indxx] = (Starter)lateApplList[0];
                        lateApplList.RemoveAt(0);
                    }
                }
                if (rb22Beginning.Checked)
                {
                    indxx = -1;
                    while (lateApplList.Count > 0)
                    {
                        while (lst[++indxx] != null) ;
                        lst[indxx] = (Starter)lateApplList[0];
                        lateApplList.RemoveAt(0);
                    }
                }

                indxx = -1;
                while (other.Count > 0)
                {
                    while (lst[++indxx] != null) ;
                    lst[indxx] = (Starter)other[0];
                    other.RemoveAt(0);
                }

                foreach (Starter strte in lst)
                    Starters.Add(strte);

                if (sm == StartListMode.TwoRoutes)
                {
                    Starters2 = new List<Starter>();
                    for (int i = 1; i < Starters.Count; i++)
                    {
                        Starters2.Add((Starter)Starters[i]);
                        Starters.RemoveAt(i);
                    }
                }
                #endregion
            }

            //Если надо создать протокол на 2ую трассу
            if (sm == StartListMode.QualiFlash)
            {
                #region QualiFlash
                CreateFlashRoutes();

                #endregion
            }

            #endregion
        }

        private void CreateFlashRoutes()
        {
            if (routeNumber > 2)
            {
                for (int i = 1; i < routeNumber; i++)
                    this[i] = new List<Starter>();
                if (rb32SameOrder.Checked)
                    foreach (Starter starter in Starters)
                        for (int i = 1; i < routeNumber; i++)
                            this[i].Add(starter);
                else //if (rb31Reverse.Checked)
                {
                    int grLen = Starters.Count / routeNumber;
                    int grMod = Starters.Count % routeNumber;
                    List<Starter>[] groups = new List<Starter>[routeNumber];
                    int cI = 0;
                    for (int k = 0; k < grMod; k++)
                    {
                        groups[k] = new List<Starter>();
                        for (int i = 0; i <= grLen; i++)
                        {
                            groups[k].Add(Starters[cI++]);
                        }
                    }
                    for (int k = grMod; k < routeNumber; k++)
                    {
                        groups[k] = new List<Starter>();
                        for (int i = 0; i < grLen; i++)
                        {
                            groups[k].Add(Starters[cI++]);
                        }
                    }
                    int curFG = groups.Length;
                    int n = 0;
                    while (curFG > 0)
                    {
                        if (curFG == groups.Length)
                            curFG = 0;
                        int curG = curFG;
                        this[n] = new List<Starter>();
                        while (curG < groups.Length)
                        {
                            foreach (Starter st in groups[curG])
                                this[n].Add(st);
                            curG++;
                        }
                        curG = 0;
                        while (curG < curFG)
                        {
                            foreach (Starter st in groups[curG])
                                this[n].Add(st);
                            curG++;
                        }
                        if (curFG == 0)
                            curFG = (groups.Length - 1);
                        else
                            curFG--;
                        n++;
                    }
                }
            }
            else
            {
                if (rb31Reverse.Checked || rb32SameOrder.Checked)
                {
                    Starters2.Clear();
                    foreach (Starter starter in Starters)
                        Starters2.Add(starter);
                    if (rb31Reverse.Checked)
                        Starters2.Reverse();
                }
                if (rb33Intl.Checked)
                {
                    int med = Starters.Count / 2;
                    if (Starters.Count % 2 == 1)
                        med++;
                    Starter[] frst = new Starter[med];
                    Starter[] lstt = new Starter[Starters.Count - med];

                    Starters.CopyTo(0, lstt, 0, Starters.Count - med);
                    Starters.CopyTo(Starters.Count - med, frst, 0, med);

                    foreach (Starter stl in frst)
                        Starters2.Add(stl);
                    foreach (Starter stl in lstt)
                        Starters2.Add(stl);
                }
                if (rb34City.Checked)
                {
                    Starters.Reverse();
                    int md = Starters.Count / 2;
                    int md2 = md / 2;
                    int md3 = (Starters.Count - md) / 2;
                    Starter[] fst = new Starter[md2];
                    Starter[] scnd = new Starter[md - md2];
                    Starter[] thrd = new Starter[md3];
                    Starter[] last = new Starter[Starters.Count - md3 - md];

                    Starters.CopyTo(0, fst, 0, md2);
                    Starters.CopyTo(md2, scnd, 0, md - md2);
                    Starters.CopyTo(md, thrd, 0, md3);
                    Starters.CopyTo(md + md3, last, 0, Starters.Count - md3 - md);

                    md = 0;
                    foreach (Starter stl in last)
                        Starters2.Add(stl);
                    foreach (Starter stl in thrd)
                        Starters2.Add(stl);
                    foreach (Starter stl in scnd)
                        Starters2.Add(stl);
                    foreach (Starter stl in fst)
                        Starters2.Add(stl);
                }
            }
        }

        private void sortPrevR()
        {
            Starters = new List<Starter>(data);
            if (rb_21reverse.Checked || rb_22SameOrder.Checked)
            {
                resComparer cmp = new resComparer();
                Starters.Sort(cmp);
                if (rb_22SameOrder.Checked)
                    Starters.Reverse();
            }
            else
                if (rb_23reverseStart.Checked || rb_24sameStart.Checked)
                {
                    stPosComparer stc = new stPosComparer();
                    Starters.Sort(stc);
                    if (rb_23reverseStart.Checked)
                        Starters.Reverse();
                }
            Starters2 = new List<Starter>();
        }

        public static List<int>[] CreateOtherRoutes(List<int> curRoute, IWin32Window owner, int routeCount)
        {
            if (routeCount == 2)
            {
                List<int>[] rv = new List<int>[2];
                rv[0] = curRoute;
                rv[1] = CreateSecondRoute(curRoute, owner);
                return rv;
            }
            Sorting s = new Sorting(new List<Starter>(), StartListMode.QualiFlash, routeCount);
            s.prevRound.Enabled = false;
            s.random.Enabled = true;
            s.rankings.Enabled = false;
            s.secondRoute.Enabled = true;
            s.lateAppl.Enabled = false;
            s.cbRanking.Enabled = false;
            s.rbRandom.Enabled = false;
            s.rbPrev.Enabled = false;
            s.onlySecondRoute = true;
            s.cbLateAppl.Enabled = false;
            s.ShowDialog(owner);
            if (s.cncl)
                return null;
            if (s.rb32SameOrder.Checked)
            {
                List<int>[] rs = new List<int>[routeCount];
                for (int i = 0; i < routeCount; i++)
                    rs[i] = curRoute;
                return rs;
            }
            List<int>[] groups = new List<int>[routeCount];
            int grLen, grMod;
            grLen = curRoute.Count / routeCount;
            grMod = curRoute.Count % routeCount;
            int curI = 0;
            for (int i = 0; i < grMod; i++)
            {
                groups[i] = new List<int>();
                for (int j = 0; j <= grLen; j++)
                    groups[i].Add(curRoute[curI++]);
            }
            for (int i = grMod; i < groups.Length; i++)
            {
                groups[i] = new List<int>();
                for (int j = 0; j < grLen; j++)
                    groups[i].Add(curRoute[curI++]);
            }

            List<int>[] res = new List<int>[routeCount];
            int curFG = groups.Length;
            int n = 0;
            while (curFG > 0)
            {
                if (curFG == groups.Length)
                    curFG = 0;
                int curG = curFG;
                res[n] = new List<int>();
                while (curG < groups.Length)
                {
                    foreach (int st in groups[curG])
                        res[n].Add(st);
                    curG++;
                }
                curG = 0;
                while (curG < curFG)
                {
                    foreach (int st in groups[curG])
                        res[n].Add(st);
                    curG++;
                }
                if (curFG == 0)
                    curFG = (groups.Length - 1);
                else
                    curFG--;
                n++;
            }
            return res;
        }

        private static List<int> CreateSecondRoute(List<int> frstRoute, IWin32Window owner)
        {
            Sorting s = new Sorting(new List<Starter>(), StartListMode.QualiFlash);
            s.prevRound.Enabled = false;
            s.random.Enabled = true;
            s.rankings.Enabled = false;
            s.secondRoute.Enabled = true;
            s.lateAppl.Enabled = false;
            s.cbRanking.Enabled = false;
            s.rbRandom.Enabled = false;
            s.rbPrev.Enabled = false;
            s.onlySecondRoute = true;
            s.cbLateAppl.Enabled = false;
            s.ShowDialog(owner);
            if (s.cncl)
                return null;
            List<int> starters = new List<int>();
            if (s.rb31Reverse.Checked || s.rb32SameOrder.Checked)
            {
                foreach (int i in frstRoute)
                    starters.Add(i);
                if (s.rb31Reverse.Checked)
                    starters.Reverse();
                return starters;
            }
            if (s.rb33Intl.Checked)
            {
                int med = frstRoute.Count / 2;
                if (frstRoute.Count % 2 == 1)
                    med++;
                int[] frst = new int[med];
                int[] lstt = new int[frstRoute.Count - med];
                

                frstRoute.CopyTo(0, lstt, 0, lstt.Length);
                frstRoute.CopyTo(lstt.Length, frst, 0, frst.Length);

                foreach (int stl in frst)
                    starters.Add(stl);
                foreach (int stl in lstt)
                    starters.Add(stl);
                return starters;
            }
            if (s.rb34City.Checked)
            {
                frstRoute.Reverse();
                int md = frstRoute.Count / 2;
                int md2 = md / 2;
                int md3 = (frstRoute.Count - md) / 2;
                int[] fst = new int[md2];
                int[] scnd = new int[md - md2];
                int[] thrd = new int[md3];
                int[] last = new int[starters.Count - md3 - md];

                frstRoute.CopyTo(0, fst, 0, md2);
                frstRoute.CopyTo(md2, scnd, 0, md - md2);
                frstRoute.CopyTo(md, thrd, 0, md3);
                frstRoute.CopyTo(md + md3, last, 0, starters.Count - md3 - md);

                md = 0;
                foreach (int stl in last)
                    starters.Add(stl);
                foreach (int stl in thrd)
                    starters.Add(stl);
                foreach (int stl in scnd)
                    starters.Add(stl);
                foreach (int stl in fst)
                    starters.Add(stl);
                return starters;
            }
            MessageBox.Show(owner, "Тип жеребьёвки не выбран");
            return null;
        }

        private void rbPrev_CheckedChanged(object sender, EventArgs e)
        {
            prevRound.Enabled = rbPrev.Checked;

            if (gbRoundFlash.Visible && cbRoundFlash.Checked)
            {
                random.Enabled = true;
                rankings.Enabled = rankings.Visible && cbRanking.Checked;
                cbRanking.Enabled = rbRandom.Checked;

                secondRoute.Enabled = true;

                cbLateAppl.Enabled = cbLateAppl.Checked && cbLateAppl.Visible;
                cbLateAppl.Enabled = rbRandom.Checked;
            }
            else
                random.Enabled = rbRandom.Checked;
        }

        public int SettedRouteNumber { get { return routeNumber; } }
        public bool RoundFlash { get; private set; }



        private void btnOK_Click(object sender, EventArgs e)
        {
            RoundFlash = false;
            if (gbRoundFlash.Visible)
            {
                RoundFlash = cbRoundFlash.Checked;
                if (RoundFlash)
                {
                    int selRoute;
                    if (!int.TryParse(tbFlashRoutes.Text, out selRoute))
                        selRoute = -1;
                    if (selRoute < 2)
                    {
                        MessageBox.Show("Число трасс для раунда Flash должно быть >= 2");
                        return;
                    }
                    routeNumber = selRoute;
                }
            }
            if (!onlySecondRoute)
            {
                if (rbRandom.Checked)
                    sortRandom();
                else
                    if (rbPrev.Checked)
                        sortPrevR();

                if (RoundFlash)
                    CreateFlashRoutes();
            }
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            cncl = true;
            this.Close();
        }

        private void cbLateAppl_CheckedChanged(object sender, EventArgs e)
        {
            lateAppl.Enabled = cbLateAppl.Checked;
        }

        private void cbRanking_CheckedChanged(object sender, EventArgs e)
        {
            rankings.Enabled = cbRanking.Checked;
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void prevRound_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void cbRoundFlash_CheckedChanged(object sender, EventArgs e)
        {
            tbFlashRoutes.Enabled = cbRoundFlash.Checked;
            rbPrev_CheckedChanged(sender, e);
        }

        private void tbFlashRoutes_TextChanged(object sender, EventArgs e)
        {
            if (gbRoundFlash.Visible && tbFlashRoutes.Enabled)
            {
                int enteredText;
                if (int.TryParse(tbFlashRoutes.Text, out enteredText))
                    SetRouteNumber(enteredText);
            }
        }
    }

    public enum StartListMode { OneRoute, TwoRoutes, QualiFlash, NotFirstRound }

    class rankingSorter : IComparer<Starter>
    {
        #region IComparer Members
        public int Compare(Starter x, Starter y)
        {
            return x.ranking.CompareTo(y.ranking);
        }
        #endregion
    }

    class randomComparer : IComparer<Starter>
    {
        #region IComparer Members

        Random rnd = new Random();

        public int Compare(Starter X, Starter Y)
        {
            if (X.iid == Y.iid)
                return X.iid.CompareTo(Y.iid);
            if (X.random != Y.random)
                return X.random.CompareTo(Y.random);
            double dTmp = rnd.NextDouble();
            double d2 = rnd.NextDouble();
            while (d2 == dTmp)
            {
                d2 = rnd.NextDouble();
                dTmp = rnd.NextDouble();
            }
            //((Starter)x).random = dTmp;
            //((Starter)y).random = d2;
            return d2.CompareTo(dTmp);
        }
        #endregion
    }

    class lateApplComparer : IComparer<Starter>
    {
        #region IComparer Members

        Random rnd = new Random();

        public int Compare(Starter X, Starter Y)
        {
            if (X.iid == Y.iid)
                return X.iid.CompareTo(Y.iid);
            if (X.ranking != Y.ranking)
            {
                if (X.ranking == 0)
                    return int.MaxValue.CompareTo(Y.ranking);
                if (Y.ranking == 0)
                    return X.ranking.CompareTo(int.MaxValue);
                return X.ranking.CompareTo(Y.ranking);
            }
            if (X.random != Y.random)
                return X.random.CompareTo(Y.random);
            double d1, d2;
            do
            {
                d1 = rnd.NextDouble();
                d2 = rnd.NextDouble();
            } while (d1 == d2);
            return d1.CompareTo(d2);
        }

        #endregion
    }

    class resComparer : IComparer<Starter>
    {
        #region IComparer Members

        Random rnd;
        public resComparer() { rnd = new Random(); }
        public int Compare(Starter X, Starter Y)
        {
            if (X.iid == Y.iid)
                return X.iid.CompareTo(Y.iid);
            if (X.prevPos != Y.prevPos)
                return Y.prevPos.CompareTo(X.prevPos);
            if (X.ranking != Y.ranking)
                return Y.ranking.CompareTo(X.ranking);
            if (X.random != Y.random)
                return X.random.CompareTo(Y.random);
            double d1, d2;
            do
            {
                d1 = rnd.NextDouble();
                d2 = rnd.NextDouble();
            } while (d1 == d2);
            return d1.CompareTo(d2);
        }

        #endregion
    }

    class stPosComparer : IComparer<Starter>
    {
        #region IComparer Members

        Random rnd;
        public stPosComparer() { rnd = new Random(); }
        public int Compare(Starter X, Starter Y)
        {
            if (X.iid == Y.iid)
                return X.iid.CompareTo(Y.iid);
            if (X.prevStart != Y.prevStart)
                return X.prevStart.CompareTo(Y.prevStart);
            if (X.ranking != Y.ranking)
                return Y.ranking.CompareTo(X.ranking);
            if (X.random != Y.random)
                return X.random.CompareTo(Y.random);
            double d1, d2;
            do
            {
                d1 = rnd.NextDouble();
                d2 = rnd.NextDouble();
            } while (d1 == d2);
            return d1.CompareTo(d2);
        }
        #endregion
    }
    public class Starter
    {
        public Starter() { }
        public int iid = 0;
        public int ranking = int.MaxValue;
        public bool HasRanking { get { return !((ranking < 1) || ranking == int.MaxValue); } }
        public bool lateAppl = false;
        public int prevPos = 0;
        public int prevStart = 0;
        public double random = 0.0;
    }
}
