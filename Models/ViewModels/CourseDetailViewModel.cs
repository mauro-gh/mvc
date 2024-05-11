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
    public class CourseDetailViewModel : CourseViewModel
    {
        public string Description { get; set; } = string.Empty;
        public List<LessonViewModel> Lessons { get; set; } = new List<LessonViewModel>();
        public TimeSpan TotalCourseDuration
        {
            get => TimeSpan.FromSeconds(Lessons?.Sum(l => l.Duration.TotalSeconds) ?? 0);
        }

        public static CourseDetailViewModel FromDataRow(DataRow dr)
        {
            var courseDetailViewModel =new CourseDetailViewModel{
                Title = (string) dr["Title"],
                Description = (string) (dr["Description"] == DBNull.Value ? string.Empty : dr["Description"]),
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
                Id = Convert.ToInt32(dr["id"]),
                Lessons = new List<LessonViewModel>()

            };

            return courseDetailViewModel;

        }

    public static CourseDetailViewModel FromEntity(Course course)
    {
        return new CourseDetailViewModel
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            Author = course.Author,
            LogoPath = course.LogoPath,
            Rating = Convert.ToDouble(course.Rating),
            //CurrentPriceAmount = Convert.ToDecimal(course.CurrentPriceAmount),
            //FullPriceAmount = Convert.ToDecimal(course.FullPriceAmount),
                CurrentPrice = course.CurrentPrice,
                FullPrice = course.FullPrice,            
            Lessons = new List<LessonViewModel>()
        };
    }        
                

    }
}