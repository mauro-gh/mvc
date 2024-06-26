using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;



namespace mvc.Models.Options
{

    public partial class CoursesOptions
    {
        public int PerPage { get; set; }

        public int InHome {get;set;}

        public CoursesOrderOptions Order { get; set; }
    }

    public partial class CoursesOrderOptions
    {

        public string By { get; set; }


        public bool Ascending { get; set; }


        public string[] Allow { get; set; }
    }
}

