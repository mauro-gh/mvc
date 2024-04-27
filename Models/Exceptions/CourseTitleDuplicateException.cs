using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace mvc.Models.Exceptions
{
    public class CourseTitleDuplicateException : Exception
    {
        public CourseTitleDuplicateException(string title, Exception innerException) : base($"Il titolo '{title}' esiste già. {innerException.Message}", innerException)
        {
        }
    }
}
