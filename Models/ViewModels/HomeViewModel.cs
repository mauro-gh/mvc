using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvc.Models.ViewModels
{
    public class HomeViewModel : CourseViewModel
    {
        public List<CourseViewModel> MostRecentCourses { get; set; } = new List<CourseViewModel>();
        public List<CourseViewModel> BestRatingCourses { get; set; } = new List<CourseViewModel>();
    }
}