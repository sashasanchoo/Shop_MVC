using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace ASP.NET_Blog_MVC_Identity.Services
{
    public class MailSender:IEmailSender
    {
        private string _host;
        private int _port;
        private bool _enableSSL;
        private string _userName;
        private string _password;
        public MailSender(string host, int port, bool enableSSL, string username, string password)
        {
            _host = host;
            _port = port;
            _enableSSL = enableSSL;
            _userName = username;
            _password = password;
        }
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient(_host, _port)
            {
                Credentials = new NetworkCredential(_userName, _password),
                EnableSsl = _enableSSL
            };
            return client.SendMailAsync(new MailMessage(_userName, email, subject, htmlMessage) { IsBodyHtml = true });
        }
    }
}
