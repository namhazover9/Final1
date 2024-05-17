using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalWeb1.Utility
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = new MimeMessage();
            message.Sender = new MailboxAddress(_configuration["EmailSettings:UserName"], _configuration["EmailSettings:UserName"]);
            message.From.Add(new MailboxAddress(_configuration["EmailSettings:UserName"], _configuration["EmailSettings:UserName"]));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = subject;
            message.Body = new TextPart("html")
            {
                Text = htmlMessage
            };
            var builder = new BodyBuilder();
            builder.HtmlBody = htmlMessage;
            message.Body = builder.ToMessageBody();

            using var client = new MailKit.Net.Smtp.SmtpClient();

            try
            {
                client.Connect(_configuration["EmailSettings:Host"], Convert.ToInt32(_configuration["EmailSettings:Port"]), SecureSocketOptions.StartTls);
                client.Authenticate(_configuration["EmailSettings:UserName"], _configuration["EmailSettings:Password"]);
                client.Send(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            client.Disconnect(true);
        }
    }
}
