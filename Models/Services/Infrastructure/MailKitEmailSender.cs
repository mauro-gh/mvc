using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;

namespace mvc.Models.Services.Infrastructure
{
    public class MailKitEmailSender : IEmailClient
    {
        public readonly ILogger<MailKitEmailSender> _logger;

        public MailKitEmailSender(ILogger<MailKitEmailSender> logger)
        {
            _logger = logger;
        }
       

        // versione con email del mittente
        public async Task SendEmailAsync(string emailTo, string replyTo, string oggetto, string testo)
        {
            using var client = new SmtpClient(); // Mailkit!
            await client.ConnectAsync("sandbox.smtp.mailtrap.io", 2525, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync("d108ae3834189e", "9b7e99d18ed83a");

            var message = new MimeMessage();
            // TODO: leggere da appsettings
            message.From.Add(MailboxAddress.Parse("MyCourse <noreply@mycourse.com>"));
            message.To.Add(MailboxAddress.Parse(emailTo));
            if (replyTo is not (null or ""))
            {
                message.ReplyTo.Add(MailboxAddress.Parse(replyTo));
            }
            
            message.Subject = oggetto;
            message.Body = new TextPart("html") { Text = testo }; 
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            
        }

        // versione senza email del mittente
        public Task SendEmailAsync(string emailTo, string oggetto, string testo)
        {
            return SendEmailAsync(emailTo, string.Empty, oggetto, testo);
        }
    }
}