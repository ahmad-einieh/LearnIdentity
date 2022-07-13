using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace WebApplication1.Services
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var myEmail = ""; // enter your email here
            var myPassword = ""; // enter your password here

            var message = new MailMessage();
            message.From =  new MailAddress(myEmail);
            message.Subject = subject;
            message.To.Add(email);
            message.Body = $"<html><body>{htmlMessage}</body></html>";
            message.IsBodyHtml = true;

            var stmpClint = new SmtpClient("smtp-mail.outlook.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(myEmail,myPassword),
                EnableSsl = true
            };

            stmpClint.Send(message);
        }
    }
}
