// <copyright file="ListLineModel.cs">
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
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using WebClimbing.ServiceClasses;
using XmlApiData;

namespace WebClimbing.Models
{
    [EnumCustomDisplay]
    public enum ResultType : short
    {
        [Display(Name="-")]
        HasResult = 0,
        [Display(Name="н/я")]
        DidNotStarted = 1,
        [Display(Name="дискв.")]
        Disqualified = 2
    }

    public interface IListLineModel
    {
        [Display(Name = "Фамилия, Имя")]
        Comp_CompetitiorRegistrationModel Climber { get; set; }
        
        [Column("pos_text"), Display(Name = "Место")]
        String PosText { get; set; }

        [Column("res_text"), Display(Name = "Рез-т")]
        String ResText { get; set; }

        [Column("ptsText"), Display(Name = "Балл")]
        String PointsText { get; set; }

        [Column("qf"), Display(Name = "Кв.")]
        String Qf { get; set; }

        [Column("pre_qf"), Display(Name = "Пре Кв.")]
        bool PreQf { get; set; }
    }

    [Table("MVCResultLine")]
    public abstract class ListLineModel : IListLineModel
    {
        [Key, Column("iid"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Iid { get; set; }

        [Column("iid_parent"), ForeignKey("ListHeader")]
        public long IidParent { get; set; }
        public virtual ListHeaderModel ListHeader { get; set; }

        [Column("climber_id"), ForeignKey("Climber")]
        public long ClimberId { get; set; }
        [Display(Name="Фамилия, Имя")]
        public virtual Comp_CompetitiorRegistrationModel Climber { get; set; }

        [Column("start"), Display(Name = "Ст.№")]
        public int StartNumber { get; set; }

        [Column("pos")]
        public int? Position { get; set; }

        [Column("pos_text"), Display(Name = "Место")]
        public String PosText { get; set; }

        [Column("result")]
        public long? Result { get; set; }

        [Column("res_text"), Display(Name = "Рез-т")]
        public String ResText { get; set; }

        [Column("points")]
        public double? Points { get; set; }

        [Column("ptsText"), Display(Name = "Балл")]
        public String PointsText { get; set; }

        [Column("qf"), Display(Name = "Кв.")]
        public String Qf { get; set; }

        [Column("pre_qf"), Display(Name = "Пре Кв.")]
        public bool PreQf { get; set; }

        [NotMapped, Display(Name="Пред.раунд")]
        public virtual String PreviousRound
        {
            get
            {
                if (this.ListHeader.PreviousRoundId == null)
                    return String.Empty;
                var prevRound = this.ListHeader.PreviousRound;
                if (prevRound == null)
                    return String.Empty;
                var clmRes = this.ListHeader.PreviousRound.Results.FirstOrDefault(r => r.ClimberId == this.ClimberId);
                if (clmRes == null)
                    return String.Empty;
                else
                    return clmRes.ResText ?? String.Empty;
            }
        }

        [NotMapped]
        public virtual String TextResult { get { return this.ResText; } }
    }

    [Table("MVCResultLineLead")]
    public class LeadResultLine : ListLineModel
    {
        [Column("time_value")]
        public int? Time { get; set; }

        [Column("time_text")]
        public String TimeText { get; set; }
    }

    [Table("MVCResultLineSpeed")]
    public class SpeedResultLine : ListLineModel
    {
        [Column("route1")]
        public long? Route1Data { get; set; }

        [Column("route1Text"), Display(Name = "Трасса 1")]
        public String Route1Text { get; set; }

        [Column("route2")]
        public long? Route2Data { get; set; }
        [Column("route2Text"), Display(Name="Трасса 2")]
        public String Route2Text { get; set; }

        [NotMapped]
        public override string TextResult
        {
            get
            {
                if (this.ListHeader.SecondQfWithBestFirst)
                {
                    List<Tuple<long, String>> lstRes = new List<Tuple<long, string>>();
                    lstRes.Add(new Tuple<long, String>(this.Result ?? long.MaxValue, this.ResText));
                    var listModel = this.ListHeader;
                    while (listModel.PreviousRoundId != null)
                    {
                        listModel = listModel.PreviousRound;
                        if (listModel == null)
                            break;
                        var clmRes = listModel.ResultsSpeed.FirstOrDefault(r => r.ClimberId == this.ClimberId);
                        if (clmRes == null || clmRes.PreQf)
                            continue;
                        lstRes.Add(new Tuple<long, string>(clmRes.Result ?? long.MaxValue, clmRes.ResText));
                    }
                    lstRes.Sort((a, b) => a.Item1.CompareTo(b.Item1));
                    return lstRes.First().Item2;
                }
                else
                    return base.TextResult;
            }
        }
    }

    [Table("MVCResultLineBoulder")]
    public class BoulderResultLine : ListLineModel
    {
        [Column("result_code")]
        public short ResultTypeCode { get; set; }
        [NotMapped]
        public ResultType ResultTypeValue
        {
            get { return (ResultType)ResultTypeCode; }
            set { ResultTypeCode = (short)value; }
        }

        private int? GetPredicateCount(Func<BoulderResultRoute, bool> predicate)
        {
            if (this.Routes == null || this.Routes.Count() < 1)
                return null;
            return this.Routes.Count(predicate);
        }
        private int? GetFieldSum(Func<BoulderResultRoute,int> selector)
        {
            if (this.Routes == null || this.Routes.Count() < 1)
                return null;
            return this.Routes.Sum(selector);
        }
        [NotMapped, Display(Name="Тр.")]
        public int? Top { get { return GetPredicateCount(r => (r.Top ?? 0) > 0); } }
        [NotMapped, Display(Name="Поп.")]
        public int? TopAttempts { get { return GetFieldSum(r => (r.Top ?? 0)); } }

        [NotMapped, Display(Name="Бон.")]
        public int? Bonus { get { return GetPredicateCount(r => (r.Bonus ?? 0) > 0); } }
        [NotMapped, Display(Name="Поп.")]
        public int? BonusAttempts { get { return GetFieldSum(r => (r.Bonus ?? 0)); } }

        [NotMapped]
        public override string TextResult
        {
            get
            {
                if (this.ResultTypeValue == ResultType.HasResult)
                {
                    if (this.Top != null)
                        return String.Format("{0}/{1} {2}/{3}", this.Top, this.TopAttempts, this.Bonus, this.BonusAttempts);
                    else
                        return String.Empty;
                }
                else
                    return this.ResultTypeValue.GetFriendlyValue();
            }
        }

        public virtual ICollection<BoulderResultRoute> Routes { get; set; }
    }

    [Table("MVCResultLineBoulderRoute")]
    public class BoulderResultRoute
    {
        [Key, Column("iid"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Iid { get; set; }

        [Column("iid_parent"), ForeignKey("ResultLine")]
        public long IidParent { get; set; }
        public virtual BoulderResultLine ResultLine { get; set; }

        [Column("route")]
        public int Route { get; set; }

        [Column("tops")]
        public int? Top { get; set; }

        [Column("bonuses")]
        public int? Bonus { get; set; }
    }

    public class FlashResult : IListLineModel
    {
        private List<LeadResultLine> routes = new List<LeadResultLine>();
        public List<LeadResultLine> Routes { get { return this.routes; } }

        public Comp_CompetitiorRegistrationModel Climber { get; set; }
        public string PosText { get; set; }
        public string ResText { get; set; }

        public string PointsText { get; set; }
        public string Qf { get; set; }

        public bool PreQf { get; set; }
    }
}