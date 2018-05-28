using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace StudyTogether_backend.Code
{
    public static class EmailManager
    {
        private static string mail = ConfigurationManager.AppSettings["Mail"];
        private static string mailPassword = ConfigurationManager.AppSettings["MailPassword"];
        private static string mailServer = ConfigurationManager.AppSettings["MailServer"];
        private static int mailServerPort = Convert.ToInt32(ConfigurationManager.AppSettings["MailServerPort"]);

        public static void SendMail(string reciverEmail, string mailBody, string mailSubject)
        {
            SmtpClient smtpClient = new SmtpClient(mailServer, mailServerPort)
            {
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(mail, mailPassword),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = true
            };

            MailMessage newMail = new MailMessage
            {
                //Setting From , To and CC
                From = new MailAddress(mail),
            };

            newMail.To.Add(new MailAddress(reciverEmail));
            newMail.Subject = mailSubject;
            newMail.Body = mailBody;

            smtpClient.Send(newMail);
        }
    }
}