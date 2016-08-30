// <copyright file="MailService.cs">
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
using System.Net.Mail;
using System.Configuration;
using System.Net.Configuration;
using System.Web.Configuration;
using System.Net;
using System.IO;
using System.Globalization;
using System.Net.Mime;

namespace WebClimbing.src
{
    public sealed class MailService
    {
        private MailAddress adFrom;
        private SmtpClient client;
        private Entities entities;
        private long compID;
        private ONLCompetition currentComp = null;
        private string compName
        {
            get
            {
                if (currentComp == null)
                    return null;
                else
                    return currentComp.short_name;
            }
        }


        private void LoadCompName()
        {
            if (entities.Connection.State != System.Data.ConnectionState.Open)
                entities.Connection.Open();
            var cList = from c in entities.ONLCompetitions
                        where c.iid == compID
                        select c;
            if (cList.Count() < 1)
                currentComp = null;
            else
                currentComp = cList.First();
        }

        public MailService(Entities e, long compID)
        {
            this.entities = e;
            this.compID = compID;
            LoadCompName();

            client = new SmtpClient();
            SmtpSection sect = (SmtpSection)WebConfigurationManager.GetSection("system.net/mailSettings/smtp");

            adFrom = new MailAddress(WebConfigurationManager.AppSettings["email_from"], String.IsNullOrEmpty(compName) ? sect.From : compName);
            client.Host = sect.Network.Host;
            client.UseDefaultCredentials = sect.Network.DefaultCredentials;
            if (!client.UseDefaultCredentials)
                client.Credentials = new NetworkCredential(sect.Network.UserName,
                    sect.Network.Password);
        }
        private void SendMessage(string to, string subj, string body, MailPriority priority, string attachmentFileName, Stream attachment)
        {
            MailAddress adTo = new MailAddress(to);
            MailMessage msg = new MailMessage(adFrom, adTo);
            msg.Body = body;
            msg.Subject = subj;
            msg.Priority = priority;
            msg.Body += "\r\n\r\nДанное сообщение создано автоматически. " +
                "Пожалуйста, не отвечайте на это сообщение.";
            if (attachment != null)
            {
                string efName;
                if (String.IsNullOrEmpty(attachmentFileName))
                    efName = DateTime.UtcNow.ToString("yyMMdd_hhmm", CultureInfo.CurrentCulture);
                else
                    efName = attachmentFileName;
                try
                {
                    if (attachment.Position != 0)
                        attachment.Position = 0;
                }
                catch { }
                ContentType ct = new ContentType();
                ct.MediaType = MediaTypeNames.Application.Octet;
                ct.Name = efName;
                Attachment atc = new Attachment(attachment, ct);
                msg.Attachments.Add(atc);
            }
            client.Send(msg);
        }

        public bool SendMail(string to, string subj, string message,
            MailPriority priority, out string errMessage)
        {
            return SendMail(to, subj, message, priority, null, null,
                out errMessage);
        }

        public bool SendMail(string to, string subj, string message,
            MailPriority priority, string attachmentFileName, Stream attachmentStream, out string errMessage)
        {
            return SendMail(new string[] { to }, subj, message, priority, attachmentFileName, attachmentStream,
                out errMessage);
        }

        private bool SendMail(string[] to, string subj, string message,
            MailPriority priority, string attachmentFileName, Stream attachmentStream, out string errMessage)
        {
            try
            {
                foreach (string sTo in to)
                    this.SendMessage(sTo, subj, message, priority, attachmentFileName, attachmentStream);

                errMessage = "";
                return true;
            }
            catch (Exception ex)
            {
                errMessage = ex.Message;
                return false;
            }
        }
    }
}
