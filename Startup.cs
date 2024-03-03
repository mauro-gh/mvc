using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvc
{
    public class Startup
    {
        
        public IConfiguration Config { get; }
        
        public Startup(IConfiguration config)
        {
            Config = config;
        }

        

        public void test(){}

    }
}