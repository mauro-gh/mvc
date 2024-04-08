using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using mvc.Customizations.ModelBinders;
using mvc.Models.Options;

namespace mvc.Models.InputModels
{

    // model binder personalizzato, ricordarsi attributo ModelBinder



    [ModelBinder(BinderType = typeof(CourseListInputModelBinder))]
    public class CourseListInputModel
    {
        public string Search { get;  } = null!;
        public int Page { get;  }
        public string OrderBy { get;  } = null!;
        public bool Ascending { get;  }
        public int Limit { get; }
        public int Offset { get; }

        // TODO lezione 91

        public CourseListInputModel(string search, int page, string orderby, bool ascending, int limit, CoursesOrderOptions orderOptions)
        {

            // logica di sanitizzazione
            if (!orderOptions.Allow.Contains(orderby))
            {
                // valori di default da setting
                orderby = orderOptions.By;
                ascending = orderOptions.Ascending;
            }  


            Search = search is null ? "" : search;
            Page = Math.Max(1, page); // sanitizzare il valore, potrebbe arrivare un -40
            OrderBy = orderby;
            Ascending = ascending;
            
            Limit = Math.Max(1, limit);
            Offset = (page -1) * Limit;
            
            
        }
    }
}