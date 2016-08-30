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
using WebClimbing.Models;
using System.Net.Mail;
using System.Net.Configuration;
using System.Web.Configuration;
using System.Net;

namespace WebClimbing.ServiceClasses
{
    public sealed class MailService
    {
        ClimbingContext db;
        SmtpClient client;
        MailAddress adFrom;
        private MailService()
        {
            db = new ClimbingContext();
            client = new SmtpClient();
            SmtpSection sect = (SmtpSection)WebConfigurationManager.GetSection("system.net/mailSettings/smtp");
            string emailFrom = WebConfigurationManager.AppSettings["emailFrom"];
            adFrom = new MailAddress(emailFrom, sect.From);
            client.Host = sect.Network.Host;
            client.UseDefaultCredentials = sect.Network.DefaultCredentials;
            if (!client.UseDefaultCredentials)
                client.Credentials = new NetworkCredential(sect.Network.UserName, sect.Network.Password);
            client.EnableSsl = sect.Network.EnableSsl;
        }

        private void localSendMessage(string to, string subj, string body, bool async = false)
        {
            MailAddress adTo = new MailAddress(to);
            MailMessage msg = new MailMessage(adFrom, adTo);
            msg.Body = body + Environment.NewLine + Environment.NewLine + "Данное письмо создано автоматически." +
                Environment.NewLine + "Пожалуйста, не отвечайте на это письмо";
            msg.Subject = subj;
            if (async)
                client.SendAsync(msg, null);
            else
                client.Send(msg);
        }

        public static bool SendMessage(string to, string subj, string body, out string errorMessage, bool async = false)
        {
            try
            {
                MailService mService = new MailService();
                mService.localSendMessage(to, subj, body, async);
                errorMessage = String.Empty;
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.ToString();
                return false;
            }
        }

        public static bool SendMessage(string to, string subj, string body, bool async = false)
        {
            string erMsg;
            return SendMessage(to, subj, body, out erMsg, async);
        }
    }
}