// <copyright file="WindowDescriptor.cs">
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
    [Table("wn_windows")]
    public class WindowDescriptor : BaseObject
    {
        protected override void RemoveEntity(BaseContext context)
        {
            context.Windows.Remove(this);
        }

        protected override void RemoveLinkedCollections(BaseContext context, Log.LogicTransaction ltr)
        {
            RemoveChildCollection(context, this.ChildWindows, ltr);
            RemoveChildCollection(context, this.ChildActions, ltr);
        }

        protected WindowDescriptor() { }

        public WindowDescriptor(BaseContext context, String iidToSet, String windowName)
        {
            if (String.IsNullOrEmpty(iidToSet))
                throw new ArgumentNullException("iidToSet");

            if (String.IsNullOrWhiteSpace(windowName))
                throw new ArgumentNullException("windowName");

            this.UserCreated = this.UserUpdated = context.CurrentUserID;
            this.UpdateDate = this.CreateDate = context.Now;

            this.Iid = iidToSet;
            this.Name = windowName;
        }

        private String name;

        [Column("name"), MaxLength(2 * IID_SIZE), Index("wn_windows_UX1", IsUnique = true), Required]
        public String Name
        {
            get { return name; }
            set
            {
                if (value == null)
                    name = String.Empty;
                else
                    name = value.Length > 2 * IID_SIZE ? value.Substring(0, 2 * IID_SIZE) : value;
            }
        }

        [Column("iid_parent"), MaxLength(IID_SIZE)]
        public String ParentWindowId { get; set; }
        [ForeignKey("ParentWindowId")]
        public virtual WindowDescriptor ParentWindow { get; set; }

        public virtual ICollection<WindowDescriptor> ChildWindows { get; set; }
        public virtual ICollection<ActionDescriptor> ChildActions { get; set; }

        public override RightsEnum? GetRights(string securityEntityID, RightsActionEnum action, BaseContext context, out bool isInherited)
        {
            var result = base.GetRights(securityEntityID, action, context, out isInherited);
            if (result != null)
                return result;
            var parentWindow = this.ParentWindow == null && !String.IsNullOrEmpty(ParentWindowId) ? context.Windows.FirstOrDefault(wnd => wnd.Iid.Equals(this.ParentWindowId, StringComparison.OrdinalIgnoreCase)) : this.ParentWindow;
            if (parentWindow != null)
            {
                result = parentWindow.GetRights(securityEntityID, action, context);
                isInherited = true;
            }
            return result;
        }
    }
}