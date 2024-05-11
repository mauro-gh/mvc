using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using mvc.Models.Entities;
using mvc.Models.Enums;
using mvc.Models.ValueObjects;

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
        //public decimal FullPriceAmount {get;set;}
        //public decimal CurrentPriceAmount {get;set;}
        public Money FullPrice { get; set; }
        public Money CurrentPrice { get; set; }      

        public static CourseViewModel FromDataRow(DataRow dr)
        {
            var courseViewModel =new CourseViewModel{
                Title = (string) dr["Title"],
                Author = (string) dr["Author"],
                LogoPath = (string) dr["LogoPath"],
                Rating = Convert.ToDouble( dr["Rating"]),
                //FullPriceAmount = Convert.ToDecimal( dr["FullPrice_Amount"]),
                //CurrentPriceAmount = Convert.ToDecimal(dr["CurrentPrice_Amount"]),
                FullPrice = new Money(
                    Enum.Parse<Currency>(Convert.ToString(dr["FullPrice_Currency"])),
                    Convert.ToDecimal(dr["FullPrice_Amount"])
                ),
                CurrentPrice = new Money(
                    Enum.Parse<Currency>(Convert.ToString(dr["CurrentPrice_Currency"])),
                    Convert.ToDecimal(dr["CurrentPrice_Amount"])
                ),                
                Id = Convert.ToInt32(dr["id"])

            };

            return courseViewModel;

        }

        public static CourseViewModel FromEntity(Course course)
        {
            return new CourseViewModel {
                Id = course.Id,
                Title = course.Title,
                LogoPath = course.LogoPath,
                Author = course.Author,
                Rating = course.Rating,
                CurrentPrice = course.CurrentPrice,
                FullPrice = course.FullPrice
            };
        }        
    }
}