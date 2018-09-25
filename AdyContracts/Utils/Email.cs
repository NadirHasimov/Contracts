using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Helpers;

namespace AdyContracts.Utils
{
    public static class Email
    {
        public static void SendEmail(string mail, string subject, string message)
        {
            ServicePointManager.ServerCertificateValidationCallback =
                  delegate (object s, X509Certificate certificate,
                           X509Chain chain, SslPolicyErrors sslPolicyErrors)
                  { return true; };
            SmtpClient smtp = new SmtpClient("192.168.11.11", 587);
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Credentials = new NetworkCredential("huquq@ady.az", "hTE8saHYHR7kXD7M");
            smtp.UseDefaultCredentials = false;
            MailMessage mailMessage = new MailMessage();
            MailAddress ma = new MailAddress("huquq@ady.az", "huquq@ady.az");
            mailMessage.From = ma;
            mailMessage.To.Add(new MailAddress(mail, "Test tester"));
            mailMessage.Subject = subject;
            mailMessage.Body = message;
            smtp.Send(mailMessage);
        }
    }
}