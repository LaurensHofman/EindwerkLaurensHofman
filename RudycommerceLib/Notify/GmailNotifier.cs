using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceLib.Notify
{
    /// <summary>
    /// Allows to send e-mails by using Gmail
    /// </summary>
    public class GmailNotifier
    {
        /// <summary>
        /// The sender's address
        /// </summary>
        private MailAddress _fromAddress;
        /// <summary>
        /// The password of the mail account
        /// </summary>
        private string _fromPassword;

        /// <summary>
        /// Allows to send e-mails by using SMTP
        /// </summary>
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

        /// <summary>
        /// Sends an e-mail
        /// </summary>
        /// <param name="toAddress">Recipient's email address</param>
        /// <param name="title">Title of the email</param>
        /// <param name="content">Content of the email</param>
        public void Notify(MailAddress toAddress, string title, string content)
        {
            try
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
            catch (Exception)
            {
                throw new CustomExceptions.EmailSentFailed();
            }            
        }
    }
}
