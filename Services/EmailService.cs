using BaltaWeb.Models;
using System.Net;
using System.Net.Mail;


namespace BaltaWeb.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;       

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;          
                    
        }
        
        public bool Send(string toName, string toEmail, string subject, string body, string fromName = "hscarvalho@outlook.com.br", string fromEmail = "herbertzin@gmail.com")
        {
            var smtpConfigurationAccess = new SmtpConfiguration();
            _configuration.GetSection("SmtpConfiguration").Bind(smtpConfigurationAccess);

            var smtpClient = new SmtpClient(smtpConfigurationAccess.Host, int.Parse(smtpConfigurationAccess.Port));
            smtpClient.Credentials = new NetworkCredential(smtpConfigurationAccess.Username, smtpConfigurationAccess.Password);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = true;

            var mail = new MailMessage();

            mail.From = new MailAddress(fromName);
            mail.To.Add(new MailAddress(toEmail, toName));
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;

            try
            {
                smtpClient.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
