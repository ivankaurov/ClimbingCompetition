// <copyright file="Competition.Parameters.cs">
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

﻿using ClimbingCompetition.Common;
using DbAccessCore.Log;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace ClimbingEntities.Competitions
{
    partial class Competition
    {
        public virtual ICollection<CompetitionParameter> Parameters { get; set; }

        public CompetitionParameter this[CompetitionParamId paramId]
        {
            get
            {
                if (Parameters == null)
                    return null;
                return this.Parameters.FirstOrDefault(p => p.ParamIdString.Equals(paramId.ToString("G"), StringComparison.Ordinal));
            }
        }

        private T GetParameterValue<T>(CompetitionParamId paramId, T defaultValue, Func<CompetitionParameter, T> valueFactory)
        {
            var param = this[paramId];
            return param == null ? defaultValue : valueFactory(param);
        }

        private CompetitionParameter SetParameterValue<T>(CompetitionParamId paramId, T value, Action<CompetitionParameter, T> setValueAction, ClimbingContext2 context, LogicTransaction ltr)
        {
            var param = this[paramId];
            if (param == null)
            {
                if (this.Parameters == null)
                    this.Parameters = new List<CompetitionParameter>();
                this.Parameters.Add(param = new CompetitionParameter(context)
                {
                    ParamId = paramId,
                    CompId = this.Iid
                });
                setValueAction(param, value);
                if (ltr != null)
                    ltr.AddCreatedObject(param, context);
            }
            else
            {
                ltr.AddUpdatedObjectBefore(param, context);
                setValueAction(param, value);
                ltr.AddUpatedObjectAfter(param, context);
            }
            return param;
        }

        public string GetStringParameterValue(CompetitionParamId paramId, String defaultValue = null)
        {
            return GetParameterValue<string>(paramId, defaultValue, p => p.StringValue);
        }

        public CompetitionParameter SetStringParameterValue(CompetitionParamId paramId, String value, ClimbingContext2 context, LogicTransaction ltr = null)
        {
            return SetParameterValue(paramId, value, (p, v) => p.StringValue = v, context, ltr);
        }

        public DateTime? GetDatetimeNullableValue(CompetitionParamId paramId)
        {
            return GetParameterValue<DateTime?>(paramId, null, p => p.DateTimeValue);
        }

        public CompetitionParameter SetDateTimeValue(CompetitionParamId paramId, DateTime? value, ClimbingContext2 context, LogicTransaction ltr = null)
        {
            return SetParameterValue(paramId, value, (p, v) => p.DateTimeValue = v, context, ltr);
        }

        public DateTime GetDatetimeValue(CompetitionParamId paramId, DateTime defaultValue)
        {
            return GetDatetimeNullableValue(paramId) ?? defaultValue;
        }

        public Boolean GetBooleanValue(CompetitionParamId paramId)
        {
            return String.Equals(GetStringParameterValue(paramId), "true", StringComparison.Ordinal);
        }

        public CompetitionParameter SetBooleanValue(CompetitionParamId paramId, Boolean value, ClimbingContext2 context, LogicTransaction ltr = null)
        {
            return SetStringParameterValue(paramId, value ? "true" : "false", context, ltr);
        }

        public ClimbingStyles GetStyles()
        {
            var sValue = GetStringParameterValue(CompetitionParamId.CompetitionStyles);
            if (String.IsNullOrEmpty(sValue))
                return 0;
            else
                return ClimbingStyleExtensions.ParseSerializedValue(sValue);
        }
        public CompetitionParameter SetStyles(ClimbingStyles styles, ClimbingContext2 context, LogicTransaction ltr = null)
        {
            return SetStringParameterValue(CompetitionParamId.CompetitionStyles, styles.GetSerializedValue(), context, ltr);
        }

        public Rules GetRules()
        {
            var sValue = this.GetStringParameterValue(CompetitionParamId.Rules);
            if (string.IsNullOrEmpty(sValue))
                return Rules.Russian;

            long res;
            if (!long.TryParse(sValue, out res))
                return Rules.Russian;
            return (Rules)res;
        }

        public CompetitionParameter SetRules(Rules rules, ClimbingContext2 context, LogicTransaction ltr = null)
        {
            return this.SetStringParameterValue(CompetitionParamId.Rules, ((long)rules).ToString(), context, ltr);
        }
    }
}
