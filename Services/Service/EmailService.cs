using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace MarketPlace.API.Services.Service
{
    public interface IEmailService
    {
        Task<bool> Send(string to, string subject, string html);
    }

    public class EmailService : IEmailService
    {
        public IConfiguration Configuration { get; }

        public EmailService(IConfiguration Configuration)
        {
            this.Configuration = Configuration;
        }

        public async Task<bool> Send(string to, string subject, string html)
        {
            try
            {
                // create message
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(Configuration["Email:username"]));
                email.To.Add(MailboxAddress.Parse(to));
                email.Subject = subject;
                email.Body = new TextPart(TextFormat.Html) { Text = html };
                // email.HtmlBody = html;
                // send email
                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(Configuration["Email:smtpHost"], Int32.Parse(Configuration["Email:port"]), SecureSocketOptions.None);
                await smtp.AuthenticateAsync(Configuration["Email:username"], Configuration["Email:password"]);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
    }
}