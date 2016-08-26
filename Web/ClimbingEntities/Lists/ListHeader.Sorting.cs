using ClimbingCompetition.Common;
using ClimbingEntities.Competitions;
using Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace ClimbingEntities.Lists
{
    partial class ListHeader
    {
        private IEnumerable<IListLine> preloadedResults;

        private IEnumerable<IListLine> preloadedResultSorted;

        public IEnumerable<IListLine> LoadSortedResults(ClimbingContext2 context)
        {
            if(preloadedResultSorted == null)
            {
                var pLpreloadedResultSorted = this.LoadResults(context)
                                                  .ToList();
                pLpreloadedResultSorted.Sort((a, b) =>
                {
                    int n = a.Pos.CompareTo(b.Pos);
                    if (n != 0)
                        return n;
                    return a.CompareTo(b, this.PreviousRound != null);
                });
                this.preloadedResultSorted = pLpreloadedResultSorted;
            }
            return this.preloadedResultSorted;
        }

        public IEnumerable<IListLine> PreloadedSortedResults { get { return this.preloadedResultSorted; } }
        
        private IEnumerable<IListLine> LoadResults(ClimbingContext2 context)
        {
            if (preloadedResults == null)
            {
                if (context == null)
                    throw new ArgumentNullException("context");
                if (this.ListType == ListType.LeadFlash)
                    preloadedResults = this.CreateFlashResults(context);
                else
                    switch (this.Style)
                    {
                        case ClimbingStyles.Lead:
                            preloadedResults = context.ResultsLead.Where(r => r.ListId == this.Iid).ToList();
                            break;
                        case ClimbingStyles.Speed:
                            preloadedResults = context.ResultsSpeed.Where(r => r.ListId == this.Iid).ToList();
                            break;
                        case ClimbingStyles.Bouldering:
                            preloadedResults = context.ResultsBoulder.Where(r => r.ListId == this.Iid).ToList();
                            break;
                    }
            }
            return preloadedResults;
        }

        public void Sort(ClimbingContext2 context)
        {
            switch (this.ListType)
            {
                case ListType.SpeedFinal:
                    this.SortSpeedFinals(this.LoadResults(context).OfType<ListLineSpeed>(), context);
                    break;
                case ListType.LeadFlash:
                    break;
                default:
                    this.Sort(this.LoadResults(context), true, context);
                    break;
            }
        }

        /// <summary>
        /// Сортировка протоколов
        /// </summary>
        /// <param name="resultList"></param>
        private void Sort(IEnumerable<IListLine> resultList, bool setPoints, ClimbingContext2 context)
        {
            if (resultList == null)
                return;

            // Только с результатами
            var results = resultList.ToList().Where(r => r.HasResult() && !r.PreQf).ToList();
            
            foreach(var r in resultList.Where(r => !r.HasResult()))
            {
                r.PosText = r.PtsText = string.Empty;
                r.Pos = int.MaxValue;
            }

            foreach (var r in resultList.Where(r => r.PreQf))
            {
                r.Points = r.Pos = 0;
                r.PtsText = r.PosText = string.Empty;
            }

            results.Sort((a, b) => a.CompareTo(b, this.PreviousRound != null));
            
            // ищем одинаковые места
            IListLine current = null;
            List<IListLine> currentBlock = this.PreviousRound == null ? null : new List<IListLine>();
            for(int i = 0; i < results.Count; i++)
            {
                if(current != null && results[i].EqualResults(current))
                {
                    results[i].Pos = current.Pos;
                    if(currentBlock!= null)
                    {
                        if (currentBlock.Count < 1)
                            currentBlock.Add(current);
                        currentBlock.Add(results[i]);
                    }
                }
                else
                {
                    if(currentBlock!= null && currentBlock.Count > 1)
                    {
                        this.PreviousRound.SortBlock(currentBlock, context);
                        currentBlock.Clear();
                    }
                    current = results[i];
                    current.Pos = i + 1;
                }
            }

            if (currentBlock != null && currentBlock.Count > 1)
                this.PreviousRound.SortBlock(currentBlock,context);

            if (!setPoints)
                return;
            results.Sort((a, b) =>
            {
                int n = a.Pos.CompareTo(b.Pos);
                if (n != 0)
                    return n;
                return a.Climber.VK.CompareTo(b.Climber.VK);
            });
            // тут проставляем баллы и попадание в следующий тур
            int groupStart = -1;
            int vkCount = 0;
            int groupPos = 0;
            int groupCount = 0;
            int vkGrpCount = 0;
            IListLine groupResult = null;
            for(int i = 0; i < results.Count; i++)
            {
                // одинаковые места
                if (groupResult != null && results[i].Pos == groupResult.Pos)
                {
                    if(results[i].Climber.VK)
                    {
                        results[i].PosText = results[i].PtsText = "в/к";
                        vkCount++;
                    }
                    else
                    {
                        groupCount++;
                        results[i].PosText = groupPos.ToString();
                    }
                    if (vkGrpCount <= 0)
                        results[i].Qf = groupPos <= this.Quota ? NextRoundQf.Qualified : NextRoundQf.NotQf;
                }
                else
                {
                    // если места разные
                    // считаем баллы для участников в конкурсе
                    if (vkGrpCount <= 0 && groupStart >= 0)
                    {
                        var pts = groupPos + (groupCount - 1) / 2.0;
                        for (int k = groupStart; k < i; k++)
                        {
                            results[k].Points = pts;
                            if (!results[k].Climber.VK)
                                results[k].PtsText = pts.ToString("0.#");
                        }
                    }

                    // если чувак вне конкурса, ситаем общее число таких результатов
                    if (results[i].Climber.VK)
                    {
                        if (vkGrpCount <= 0)
                            groupStart = i;
                        vkGrpCount++;
                        vkCount++;
                    }
                    else
                    {
                        // считаем квоту попадания
                        var qf = (i + 1 - vkCount) <= this.Quota ? NextRoundQf.Qualified : NextRoundQf.NotQf;

                        // если предыдущий чувак был в/к, раком поставим ему баллы
                        if (vkGrpCount > 0)
                        {
                            var ptsStep = 1.0 / (vkGrpCount + 1);
                            var pts = (double)i - vkCount;
                            IListLine currentVK = null;
                            for (int k = groupStart; k < i; k++)
                            {
                                if (currentVK != null && currentVK.Pos == results[k].Pos)
                                    results[k].Points = pts;
                                else
                                {
                                    pts += ptsStep;
                                    currentVK = results[k];
                                    currentVK.Points = pts;
                                }
                                results[k].Qf = qf;
                            }
                        }

                        groupStart = i;
                        groupCount = 1;
                        groupPos = i + 1 - vkCount;
                        vkGrpCount = 0;
                        results[i].Qf = qf;
                        results[i].PosText = groupPos.ToString();
                    }
                    groupResult = results[i];
                }
            }

            // для последнего места
            if(groupStart >= 0)
            {
                if (vkGrpCount > 0)
                {
                    var qf = (results.Count + 1 - vkCount) <= this.Quota ? NextRoundQf.Qualified : NextRoundQf.NotQf;

                    var ptsStep = 1.0 / (vkGrpCount + 1);
                    var pts = (double)results.Count - vkCount;
                    IListLine currentVK = null;
                    for (int k = groupStart; k < results.Count; k++)
                    {
                        if (currentVK != null && currentVK.Pos == results[k].Pos)
                            results[k].Points = pts;
                        else
                        {
                            pts += ptsStep;
                            currentVK = results[k];
                            currentVK.Points = pts;
                        }
                        results[k].Qf = qf;
                    }
                }
                else
                {
                    var pts = groupPos + (groupCount - 1) / 2.0;
                    for (int k = groupStart; k < results.Count; k++)
                    {
                        results[k].Points = pts;
                        if (!results[k].Climber.VK)
                            results[k].PtsText = pts.ToString("0.#");
                    }
                }
            }
            
            if(this.PreviousRound== null)
                foreach (var r in results.Where(r => r.Dns || r.Dsq))
                    r.PosText = string.Empty;

            foreach (var r in results.Where(r => !r.Climber.VK && r.NilResult()))
            {
                r.PosText = (results.Count - vkCount).ToString();
            }

        }

        private void SortSpeedFinals(IEnumerable<ListLineSpeed> results, ClimbingContext2 context)
        {
            if (this.PreviousRound.ListType == ListType.SpeedFinal && results.Count() <= 4 && this.PreviousRound.LoadResults(context).Count() <= 4)
                this.SortSpeedFinalRound(results, context);
            else if ((this.Competition.GetRules() & Rules.International) == Rules.International)
                this.SortSpeedFinalsInternational(results, context);
            else
                this.SortSpeedFinalsRussian(results, context);
        }

        private void SortSpeedFinalRound(IEnumerable<ListLineSpeed> results, ClimbingContext2 context)
        {
            var climbed = results.Where(r => r.HasResult()).ToList();
            while (climbed.Count > 0)
            {
                var c1 = climbed.First();
                var c2 = climbed.FirstOrDefault(c => c.Start == (c1.Start % 2 == 1 ? c1.Start + 1 : (c1.Start - 1)));

                int n;
                var c1Prev = this.PreviousRound.GetResult(c1.Climber.Iid, out n, context);
                if (c2 == null)
                {
                    c1.Points = c1.Pos = c1Prev.Pos;
                    c1.PosText = c1.PtsText = c1.Points.ToString();
                    climbed.RemoveAt(0);
                    continue;
                }

                var c2Prev = this.PreviousRound.GetResult(c2.Climber.Iid, out n, context);
                if (c1.CompareTo(c2, true) > 0)
                    c2 = System.Threading.Interlocked.Exchange(ref c1, c2);
                c1.Pos = Math.Min(c1Prev.Pos, c2Prev.Pos);
                c2.Pos = Math.Max(c1Prev.Pos, c2Prev.Pos);
                c1.Points = c1.Pos;
                c2.Points = c2.Pos;
                c1.PosText = c1.PtsText = c1.Pos.ToString();
                c2.PosText = c2.PtsText = c2.Pos.ToString();
                climbed.Remove(c1);
                climbed.Remove(c2);
            }
        }

        private void SortSpeedFinalsRussian(IEnumerable<ListLineSpeed> results, ClimbingContext2 context)
        {
            var climbed = results.Where(r => r.HasResult()).ToList();
            List<ListLineSpeed> winners = new List<ListLineSpeed>(), loosers = new List<ListLineSpeed>();
            int llCount = 0;
            while (climbed.Count > 0)
            {
                var c1 = climbed.First();
                var c2 = climbed.FirstOrDefault(c => c.Start == (c1.Start % 2 == 0 ? c1.Start - 1 : (c1.Start + 1)));
                if (c2 == null)
                {
                    if (c1.Failed)
                    {
                        llCount++;
                        loosers.Add(c1);
                    }
                    else
                        winners.Add(c1);
                    climbed.RemoveAt(0);
                    continue;
                }

                if (c1.Failed && c2.Failed)
                {
                    loosers.Add(c1);
                    loosers.Add(c2);
                    llCount++;
                }
                else
                {
                    int n = c1.CompareTo(c2, true);
                    if (n < 0)
                    {
                        winners.Add(c1);
                        loosers.Add(c2);
                    }
                    else
                    {
                        winners.Add(c2);
                        loosers.Add(c1);
                    }
                }

                climbed.Remove(c1);
                climbed.Remove(c2);
            }

            this.Sort(loosers, false, context);
            loosers.Sort((a, b) => a.Pos.CompareTo(b.Pos));

            loosers.ForEach(l => l.Qf = NextRoundQf.NotQf);
            winners.ForEach(l => l.Qf = NextRoundQf.Qualified);
            if(llCount > 0)
            {
                var luckyLoosers = loosers.Where(l => !l.Failed).Take(llCount).ToList();
                luckyLoosers.ForEach(a => a.Qf = NextRoundQf.LuckyLooser);
                winners.AddRange(luckyLoosers);
                loosers.RemoveRange(0, luckyLoosers.Count);
            }

            this.Sort(winners, false, context);
            winners.Sort((a, b) => a.Pos.CompareTo(b.Pos));

            ListLineSpeed currentRes = null;
            int currentPos = -1;
            for (int i = 0; i < winners.Count; i++)
            {
                if (currentRes != null && currentPos == winners[i].Pos)
                    winners[i].Pos = currentRes.Pos;
                else
                {
                    currentRes = winners[i];
                    currentPos = winners[i].Pos;
                    currentRes.Pos = i + 1;
                }
            }

            currentRes = null;
            for(int i = 0; i < loosers.Count; i++)
            {
                if (currentRes != null && currentPos == loosers[i].Pos)
                    loosers[i].Pos = currentRes.Pos;
                else
                {
                    currentRes = loosers[i];
                    currentPos = loosers[i].Pos;
                    currentRes.Pos = winners.Count + 1 + i;
                }
            }

            winners.ForEach(p =>
            {
                p.Points = p.Pos;
                p.PosText = p.PtsText = p.Pos.ToString();
            });
            loosers.ForEach(p =>
            {
                p.Points = p.Pos;
                p.PosText = p.PtsText = p.Pos.ToString();
            });
        }

        private void SortSpeedFinalsInternational(IEnumerable<ListLineSpeed> results, ClimbingContext2 context)
        {
            var climbed = results.Where(r => r.HasResult()).ToList();
            int climbersCount = 1;
            while (climbersCount < results.Count())
                climbersCount *= 2;

            List<ListLineSpeed> winners = new List<ListLineSpeed>(), loosers = new List<ListLineSpeed>();
            while(climbed.Count > 0)
            {
                int n;
                var c1 = climbed.First();
                var c2 = climbed.FirstOrDefault(c => c.Start == (c1.Start % 2 == 1 ? c1.Start + 1 : (c1.Start - 1)));
                if(c2 == null)
                {
                    if (c1.Failed)
                        loosers.Add(c1);
                    else
                    {
                        winners.Add(c1);
                        c1.Pos = this.PreviousRound.GetResult(c1.ClimberId, out n, context).Pos;
                    }
                    climbed.RemoveAt(0);
                    continue;
                }
                if(c1.Failed && c2.Failed)
                {
                    loosers.Add(c1);
                    loosers.Add(c2);
                    continue;
                }

                n = c1.CompareTo(c2, true);
                if (n > 0)
                    c2 = System.Threading.Interlocked.Exchange(ref c1, c2);

                var c1PR = this.PreviousRound.GetResult(c1.ClimberId, out n, context);
                var c2PR = this.PreviousRound.GetResult(c2.ClimberId, out n, context);
                c1.Pos = Math.Min(c1PR.Pos, c2PR.Pos);
                winners.Add(c1);
                loosers.Add(c2);

                climbed.Remove(c1);
                climbed.Remove(c2);
            }

            winners.ForEach(r =>
            {
                r.Points = r.Pos;
                r.PosText = r.PtsText = r.Pos.ToString();
                r.Qf = NextRoundQf.Qualified;
            });

            this.Sort(loosers, false, context);
            loosers.Sort((a, b) => a.Pos.CompareTo(b.Pos));
            ListLineSpeed currentResult = null;
            int currentPos = -1;
            for(int i = 0; i < loosers.Count; i++)
            {
                if (currentResult != null && currentPos == loosers[i].Pos)
                    loosers[i].Pos = currentResult.Pos;
                else
                {
                    currentResult = loosers[i];
                    currentPos = loosers[i].Pos;
                    currentResult.Pos = winners.Count + i + 1;
                }
            }

            loosers.ForEach(r =>
            {
                r.Points = r.Pos;
                r.PosText = r.PtsText = r.Pos.ToString();
                r.Qf = NextRoundQf.NotQf;
            });
        }

        private void SortBlock(IEnumerable<IListLine> block, ClimbingContext2 context)
        {
            var roundResults = block.Select(ln =>
            {
                int routeNumber;
                var n = this.GetResult(ln.Climber.Iid, out routeNumber, context);
                return new { Result = n, Route = routeNumber };
            })
            .ToList();

            if (roundResults.Count(r => r.Result == null) > 0 || roundResults.Select(r => r.Route).Distinct().Count() > 1)
                return;

            roundResults.Sort((a, b) => a.Result.CompareTo(b.Result, this.PreviousRound != null));

            var resultDict = block.ToDictionary(b => b.Climber.Iid);
            
            IListLine current = null;
            List<IListLine> currentBlock = this.PreviousRound == null ? null : new List<IListLine>();

            int startPos = block.First().Pos;
            for (int i = 0; i < roundResults.Count; i++)
            {
                if (current != null && current.EqualResults(roundResults[i].Result))
                {
                    resultDict[roundResults[i].Result.Climber.Iid].Pos = resultDict[current.Climber.Iid].Pos;
                    if (currentBlock != null)
                    {
                        if (currentBlock.Count < 1)
                            currentBlock.Add(resultDict[current.Climber.Iid]);
                        currentBlock.Add(resultDict[roundResults[i].Result.Climber.Iid]);
                    }
                }
                else
                {
                    if (currentBlock != null && currentBlock.Count > 1)
                    {
                        this.PreviousRound.SortBlock(currentBlock, context);
                        currentBlock.Clear();
                    }

                    current = roundResults[i].Result;
                    resultDict[current.Climber.Iid].Pos = startPos + i;
                }
            }

            if (currentBlock != null && currentBlock.Count > 1)
                this.PreviousRound.SortBlock(currentBlock, context);
        }
        
        private IListLine GetResult(String climberId, out int routeNumber, ClimbingContext2 context)
        {
            routeNumber = 0;
            switch (this.ListType)
            {
                case ClimbingCompetition.Common.ListType.LeadGroups:
                case ClimbingCompetition.Common.ListType.BoulderGroups:
                    int n;
                    foreach(var childList in this.Children)
                    {
                        routeNumber++;
                        var res = childList.GetResult(climberId, out n, context);
                        if (res != null)
                        {
                            if (n < 0)
                                routeNumber = n;
                            return res;
                        }
                    }
                    return null;
                default:
                    var result = this.LoadResults(context).FirstOrDefault(r => r.Climber.Iid == climberId);
                    if (result == null || result.PreQf)
                        routeNumber = -1;
                    return result;
            }
        }

        private List<FlashListLine> CreateFlashResults(ClimbingContext2 context)
        {
            if (this.ListType != ClimbingCompetition.Common.ListType.LeadFlash || this.Children == null || this.Children.Count < 1)
                return new List<FlashListLine>();
            var routes = this.Children.OrderBy(a => a.RouteNumber).ToArray();

            routes[0].Sort(context);

            var resultDict = routes[0].LoadResults(context).OfType<ListLine>().Select(r =>
            {
                var fl = new FlashListLine { Climber = r.Climber, PreQf=r.PreQf };
                fl.ChildResults.Add(r);
                return fl;
            })
            .ToDictionary(r => r.Climber.Iid);
            
            for(int i = 1; i < routes.Length; i++)
            {
                routes[i].Sort(context);
                foreach(var res in routes[i].LoadResults(context).OfType<ListLine>())
                {
                    if (!resultDict.ContainsKey(res.ClimberId))
                        resultDict.Add(res.ClimberId, new FlashListLine { Climber = res.Climber });
                    resultDict[res.ClimberId].ChildResults.Add(res);
                    if (res.PreQf)
                        resultDict[res.ClimberId].PreQf = true;
                }
            }

            var result = resultDict.Values.ToList();
            result.ForEach(v => v.CalculatePts(this.PreviousRound != null));
            this.Sort(result, true, context);
            return result;
        }

        public sealed class FlashListLine : IListLine
        {
            private const int DSQ = int.MaxValue - 100;
            private const int DNS = int.MaxValue - 50;

            public bool Dns
            {
                get { return DNS == this.Res; }
            }

            public bool Dsq
            {
                get { return this.Res == DSQ; }
            }

            public ClimberOnCompetition Climber { get; set; }

            public int Pos
            {
                get; set;
            }

            public int Res { get; set; }

            public string PosText
            {
                get; set;
            }

            public double? Points { get; set; }

            public string PtsText { get; set; }

            public int CompareTo(IListLine other, bool hasPrevRound)
            {
                var e = other as FlashListLine;
                if (e == null)
                    throw new ArgumentException("other");
                var n= this.Res.CompareTo(e.Res);
                if (n != 0)
                    return n;
                n = this.Climber.VK.CompareTo(other.Climber.VK);
                if (n != 0)
                    return n;
                n = this.Climber.Team.CompareTo(other.Climber.Team);
                if (n != 0)
                    return n;
                return this.Climber.Person.FullName.CompareTo(other.Climber.Person.FullName);
            }

            public bool EqualResults(IListLine other)
            {
                return this.Res == ((FlashListLine)other).Res;
            }

            public bool NilResult()
            {
                return false;
            }

            readonly List<ListLine> childResults = new List<ListLine>();

            public List<ListLine> ChildResults { get { return this.childResults; } }

            public void CalculatePts(bool hasPrevRound)
            {
                if (!this.HasResult())
                    return;
                bool allDns = true, allDsq = true;
                foreach(var r in childResults.Where(r => r.HasResult()))
                {
                    if (!r.Dns)
                        allDns = false;
                    if (!r.Dsq)
                        allDsq = false;
                    if (!allDns && !allDsq)
                        break;
                }
                if (allDns)
                {
                    this.Res = DNS;
                    return;
                }
                if (allDsq)
                {
                    this.Res = hasPrevRound ? DNS : DSQ;
                    return;
                }
                int routeCount = childResults.Count(p => p.Points.HasValue);
                if (routeCount < 1)
                {
                    this.Res = DNS;
                    return;
                }

                var result = 1.0;
                foreach (var r in childResults.Where(p => p.Points.HasValue))
                    result *= r.Points.Value;
                result = Math.Round(Math.Pow(result, 1.0 / routeCount), 2);
                this.ResText = this.Climber.VK ? "в/к" : result.ToString("0.00");
                this.Res = (int)(result * 100.0);
            }

            public bool HasResult()
            {
                return this.childResults.Count(r => r.HasResult()) > 0;
            }

            public NextRoundQf Qf { get; set; }

            public bool PreQf { get; set; }

            public string ResText { get; set; }
        }
    }
}
