using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceLib.Notify
{
    public class GmailNotifier
    {
        private MailAddress _fromAddress;
        private string _fromPassword;

        private SmtpClient _smtpClient;

        public GmailNotifier() : this(new MailAddress("infoecommrudy@gmail.com", "Rudycommerce"), "infoecomm") { }

        public GmailNotifier(MailAddress fromAddress, string fromPassword)
        {
            _fromAddress = fromAddress;
            _fromPassword = fromPassword;

            _smtpClient = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_fromAddress.Address, _fromPassword)
            };
        }

        public void Notify(MailAddress toAddress, string title, string content)
        {
            using (var newMail =
                   new MailMessage(_fromAddress, toAddress)
                   {
                        Subject = title,
                        Body = content
                   })
            {
                _smtpClient.Send(newMail);
            }
        }
    }
}
