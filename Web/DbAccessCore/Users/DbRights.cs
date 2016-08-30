// <copyright file="DbRights.cs">
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

namespace DbAccessCore.Users
{
    [Table("usr_object_rights")]
    class DbRights : BaseObject
    {
        protected override void RemoveEntity(BaseContext context)
        {
            context.AllRights.Remove(this);
        }

        protected DbRights() { }

        public DbRights(BaseContext context) : base(context) { }

        [Column("subject_id"), MaxLength(IID_SIZE), Index("usr_obj_rights_UX1", IsUnique = true, Order = 2)]
        public String SubjectId { get; set; }
        [ForeignKey("SubjectId")]
        public virtual BaseObject Subject { get; set; }

        [Column("object_id"), MaxLength(IID_SIZE), Index("usr_obj_rights_UX1", IsUnique = true, Order = 1)]
        public String ObjectId { get; set; }
        [ForeignKey("ObjectId")]
        public virtual DbSecurityEntity Object { get; set; }

        [Column("right_data"), MaxLength(1)]
        protected String RightData { get; set; }

        [Column("action_data"), MaxLength(1)]
        protected string ActionData { get; set; }

        [NotMapped]
        public RightsEnum Rights
        {
            get { return RightData.GetByFirstLetter<RightsEnum>(); }
            set { RightData = value.GetFirstLetter(); }
        }

        [NotMapped]
        public RightsActionEnum Action
        {
            get { return ActionData.GetByFirstLetter<RightsActionEnum>(); }
            set { ActionData = value.GetFirstLetter(); }
        }

        [Column("is_inherited")]
        public Boolean IsInherited { get; protected internal set; }
    }
}