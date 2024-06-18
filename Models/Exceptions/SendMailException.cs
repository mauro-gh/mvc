using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvc.Models.Exceptions
{
    public class SendMailException : Exception
    {           
    
        public SendMailException(string email) : base($"Errore durante invio email a {email}")
        {
        }
    
    }
}