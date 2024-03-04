using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;



namespace mvc.Models.Options
{

    public partial class CoursesOptions
    {
        public long PerPage { get; set; }

        public CousesOrderOptions Order { get; set; }
    }

    public partial class CousesOrderOptions
    {

        public string By { get; set; }


        public bool Ascending { get; set; }


        public string[] Allow { get; set; }
    }
}

