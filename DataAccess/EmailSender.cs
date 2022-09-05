
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
namespace DataAccess
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;
        public EmailSender(IConfiguration config)
        {
            _config = config;
        }

        public void SendEmail(string email, string subject, string htmlMessage)
        {
            string fromMail = _config["CommonSettings:SendNotificationMail"];
            string fromPassword = _config["CommonSettings:SendNotificationPassword"];

            MailMessage message = new()
            {
                From = new MailAddress(fromMail),
                Subject = subject
            };
            message.To.Add(new MailAddress(email));
            message.Body = "<html><body><div> <h1>Hello There!</h1> </div> <div> Welcome To Our Site. Please follow this link to activate your account : <a href=\" " + htmlMessage + " \"> Validate your account</a> </div>   </body></html>";
            message.IsBodyHtml = true;

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Host = "smtp.gmail.com",
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false

            };
            smtpClient.Send(message);
        }
        public void SendFileEmail(string email, string subject, string htmlMessage, string Code, string userName)
        {
            string fromMail = _config["CommonSettings:SendNotificationMail"];
            string fromPassword = _config["CommonSettings:SendNotificationPassword"];

            MailMessage message = new()
            {
                From = new MailAddress(fromMail),
                Subject = subject
            };
            message.To.Add(new MailAddress(email));
            message.Body = "<html><body><div> <h1>Hello There!</h1> </div> <div> You have just received a file from " + userName + " with the code " + Code + ".Please click this link to download it : <a href=\" " + htmlMessage + " \"> Download</a> </div>   </body></html>";
            message.IsBodyHtml = true;

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Host = "smtp.gmail.com",
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false

            };
            smtpClient.Send(message);
        }
        public void SendEmailPassword(string email, string subject, string htmlMessage)
        {
            string fromMail = _config["CommonSettings:SendNotificationMail"];
            string fromPassword = _config["CommonSettings:SendNotificationPassword"];
            MailMessage message = new()
            {
                From = new MailAddress(fromMail),
                Subject = subject
            };
            message.To.Add(new MailAddress(email));
            message.Body = "<html><body><div> <h1>Hello There!</h1> </div> <div> We are sorry that you Lost your Password. Please follow this link to use a new one : <a href=\" " + htmlMessage + " \">Reset Password </a> </div>   </body></html>";
            message.IsBodyHtml = true;

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = true,
            };
            smtpClient.Send(message);

        }
    }

}
