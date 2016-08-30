// <copyright file="ActionDescriptor.cs">
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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace DbAccessCore.Windows
{
    [Table("wn_actions")]
    public class ActionDescriptor : BaseObject
    {
        protected override void RemoveEntity(BaseContext context)
        {
            context.Actions.Remove(this);
        }

        protected ActionDescriptor() { }

        public ActionDescriptor(BaseContext context, String actionKey, WindowDescriptor parent)
            : base(context)
        {
            if (String.IsNullOrEmpty(actionKey))
                throw new ArgumentNullException(actionKey);
            if (parent == null)
                throw new ArgumentNullException("parent");
            this.ParentWindow = parent;
            parent.ChildActions.Add(this);
        }

        [Column("iid_parent"), MaxLength(IID_SIZE),
        Index("wn_actions_UX1", IsUnique = true, Order = 2),
        Index("wn_actions_UX2", IsUnique = true, Order = 2)]
        public String ParentWindowId { get; protected set; }
        [ForeignKey("ParentWindowId")]
        public virtual WindowDescriptor ParentWindow { get; protected set; }

        private String actionKey;
        [Column("action_key"), MaxLength(IID_SIZE), Index("wn_actions_UX1", IsUnique = true, Order = 1)]
        public String ActionKey
        {
            get { return actionKey; }
            protected set
            {
                if (value == null)
                    actionKey = value;
                else
                    actionKey = value.Length > IID_SIZE ? value.Substring(0, IID_SIZE) : value;
            }
        }

        private String actionName;
        [Column("action_name"), MaxLength(2 * IID_SIZE), Index("wn_actions_UX2", IsUnique = true, Order = 1), Required]
        public String ActionName
        {
            get { return actionName; }
            set
            {
                if (value == null)
                    actionName = null;
                else
                    actionName = value.Length > 2 * IID_SIZE ? value.Substring(0, 2 * IID_SIZE) : value;
            }
        }

        public override RightsEnum? GetRights(string securityEntityID, RightsActionEnum action, BaseContext context, out bool isInherited)
        {
            var result = base.GetRights(securityEntityID, action, context, out isInherited);
            if (result != null)
                return result;
            var window = this.ParentWindow == null && !String.IsNullOrEmpty(this.ParentWindowId) ? context.Windows.FirstOrDefault(wnd => wnd.Iid.Equals(ParentWindowId, StringComparison.OrdinalIgnoreCase)) : ParentWindow;
            if (window != null)
            {
                result = window.GetRights(securityEntityID, action, context);
                isInherited = true;
            }
            return result;
        }
    }
}