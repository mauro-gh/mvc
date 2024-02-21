using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvc.Models.ViewModels
{
    public class CourseViewModel
    {
        public int Id {get;set;}
        public string Title {get;set;} = string.Empty;
        /* aaa */
        public string ImagePath {get;set;} = string.Empty;
        public string Author {get;set;} = string.Empty;
        public double Rating {get;set;}
        public decimal FullPrice {get;set;}
        public decimal CurrentPrice {get;set;}
        
    }
}