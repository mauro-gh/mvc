using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using mvc.Models.Options;

namespace mvc
{
    public class Startup
    {
        
        public IConfiguration Config { get; }
        
        public Startup(IConfiguration config)
        {
            Config = config;
        }
        

        public void Test2(IOptionsMonitor<CoursesOptions> coursesOptions)
        {
            
            string by = coursesOptions.CurrentValue.Order.By;

            long perpage = coursesOptions.CurrentValue.PerPage;
            

        }

        public void test(){}

    }
}