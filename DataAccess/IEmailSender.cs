using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public interface IEmailSender
    {
        void SendEmail(string email, string subject, string htmlMessage);
        void SendEmailPassword(string email, string subject, string htmlMessage);
        public void SendFileEmail(string email, string subject, string htmlMessage, string Code, string userName);
    }
}
