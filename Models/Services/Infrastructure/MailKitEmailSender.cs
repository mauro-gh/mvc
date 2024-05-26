using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;

namespace mvc.Models.Services.Infrastructure
{
    public class MailKitEmailSender : IEmailSender
    {
        public readonly ILogger<MailKitEmailSender> _logger;

        public MailKitEmailSender(ILogger<MailKitEmailSender> logger)
        {
            _logger = logger;
        }
       


        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            using var client = new SmtpClient(); // Mailkit!
            await client.ConnectAsync("sandbox.smtp.mailtrap.io", 2525, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync("d108ae3834189e", "9b7e99d18ed83a");

            var message = new MimeMessage();

            message.From.Add(MailboxAddress.Parse("MyCourse <noreply@mycourse.com>"));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = htmlMessage }; 
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            
        }


    }
}