using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace mvc.Models.ViewModels
{
    public class CourseViewModel
    {
        public int Id {get;set;}
        public string Title {get;set;} = string.Empty;
        /* aaa */
        public string LogoPath {get;set;} = string.Empty;
        public string Author {get;set;} = string.Empty;
        public double Rating {get;set;}
        public decimal FullPriceAmount {get;set;}
        public decimal CurrentPriceAmount {get;set;}

        public static CourseViewModel FromDataRow(DataRow dr)
        {
            var courseViewModel =new CourseViewModel{
                Title = (string) dr["Title"],
                Author = (string) dr["Author"],
                LogoPath = (string) dr["LogoPath"],
                Rating = Convert.ToDouble( dr["Rating"]),
                FullPriceAmount = Convert.ToDecimal( dr["FullPrice_Amount"]),
                CurrentPriceAmount = Convert.ToDecimal(dr["CurrentPrice_Amount"]),
                Id = Convert.ToInt32(dr["id"])

            };

            return courseViewModel;

        }
    }
}