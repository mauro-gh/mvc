using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvc.Models.Exceptions
{
    public class OptimisticConcurrencyException : Exception
    {
        public OptimisticConcurrencyException() : base("Corso modificato da altri"){}         
        
    }
}