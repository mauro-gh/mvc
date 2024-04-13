using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvc.Models.ViewModels
{
    public interface IPaginationInfo
    {
        public int CurrentPage { get; }
        public int TotalResults { get;  }
        public int ResultsPerPage { get;  }
        public string Search { get;  }
        public string OrderBy { get;  }
        public bool Ascending { get;  }


    }
}