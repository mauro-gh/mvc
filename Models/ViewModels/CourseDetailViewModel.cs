using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

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
                Description = (string) dr["Description"],
                Author = (string) dr["Author"],
                LogoPath = (string) dr["LogoPath"],
                Rating = Convert.ToDouble( dr["Rating"]),
                FullPrice = Convert.ToDecimal( dr["FullPrice_Amount"]),
                CurrentPrice = Convert.ToDecimal(dr["CurrentPrice_Amount"]),
                Id = Convert.ToInt32(dr["id"]),
                Lessons = new List<LessonViewModel>()

            };

            return courseDetailViewModel;

        }
                

    }
}