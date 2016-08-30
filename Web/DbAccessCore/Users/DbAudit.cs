// <copyright file="DbAudit.cs">
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
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DbAccessCore.Users
{
    [Table("usr_db_audit")]
    public class DbAudit : IIIDObject
    {
        public enum AuditDataType : byte { Login = 1, Logout = 0, AutoLoginWeb = 2, DbInit = 255 }

        protected DbAudit() { }
        public DbAudit(AuditDataType auditType, BaseContext context)
        {
            String iid;
            DateTime eventDate;
            context.GetObjectCreationValues(out iid, out eventDate);
            this.Iid = iid;
            this.EventDate = eventDate;
            this.SPID = context.SPID;
            this.SpidLoginTime = context.LoginTime;
            this.EventType = auditType;
            this.WebLogin = context.IsWebContext;
            this.ClrHostName = context.CLRHostNameForSQL;
            this.ClrIP = context.CLRIPAddressForSQL;
            this.ClientHostName = context.ClientHostNameForSQL;
            this.ClientIP = context.ClientIPForSQL;
        }

        [Key, Column("iid"), MaxLength(BaseObject.IID_SIZE), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public String Iid { get; protected set; }

        [Column("event_date")]
        public DateTime EventDate { get; protected set; }

        [Column("result_success")]
        public Boolean ResultSuccess { get; set; }

        [Column("event_type"), Required, MaxLength(20)]
        public String EventTypeStore { get; protected set; }

        [NotMapped]
        public AuditDataType EventType
        {
            get { return (AuditDataType)Enum.Parse(typeof(AuditDataType), EventTypeStore); }
            protected set { EventTypeStore = value.ToString("G"); }
        }

        [Column("user_id"), MaxLength(BaseObject.IID_SIZE)]
        public String UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual DbUser User { get; set; }

        [Column("spid")]
        public int SPID { get; protected set; }

        [Column("spid_login_time")]
        public DateTime SpidLoginTime { get; protected set; }

        [Column("web_login")]
        public Boolean WebLogin { get; protected set; }

        String message = String.Empty;
        [Column("message"), MaxLength(4096)]
        public String Message
        {
            get { return message; }
            set
            {
                if (value == null)
                    message = String.Empty;
                else if (value.Length > 4096)
                    message = value.Substring(0, 4096);
                else
                    message = value;
            }
        }

        [Column("client_host_name"), MaxLength(255)]
        public String ClientHostName { get; protected set; }

        [Column("clr_host_name"), MaxLength(255)]
        public String ClrHostName { get; protected set; }

        [Column("client_ip"), MaxLength(15)]
        public String ClientIP { get; protected set; }

        [Column("clr_ip"), MaxLength(15)]
        public String ClrIP { get; protected set; }

        public void RemoveObject(BaseContext context, Log.LogicTransaction ltr = null)
        {
            if (context != null)
                context.Audit.Remove(this);
        }
    }
}
