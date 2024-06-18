using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace mvc.Models.Services.Infrastructure
{
    public interface IEmailClient : IEmailSender
    {
        Task SendEmailAsync(string emailTo, string replyTo, string oggetto, string testo);
        
    }
}