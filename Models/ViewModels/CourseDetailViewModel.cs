using System;
using System.Collections.Generic;
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

    }
}