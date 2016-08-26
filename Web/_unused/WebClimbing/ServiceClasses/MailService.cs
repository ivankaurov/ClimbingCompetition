using System;
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