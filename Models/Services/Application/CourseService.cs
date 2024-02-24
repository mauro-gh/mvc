using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mvc.Models.ViewModels;

namespace mvc.Models.Services.Application
{
    public class CourseService
    {
        public List<CourseViewModel> GetCourses()
        {
            
            var courseList = new List<CourseViewModel>();
            var rand = new Random();

            for (int i =1; i <= 20; i++)
            {
                var price = Convert.ToDecimal(rand.NextDouble() * 10 + 10);
                var course = new CourseViewModel
                {
                    Id = i,
                    Title = $"Corso {i}",
                    CurrentPrice = price,
                    FullPrice = rand.NextDouble() > 0.5 ? price : price -1,
                    Author = "Nome Cognome",
                    Rating = rand.NextDouble() * 5.0,
                    LogoPath = "/logo.png"
                };
                courseList.Add(course);

            }
            return courseList;

        }

        public CourseDetailViewModel GetCourse(int id)
        {
            var rand = new Random();
            var price = Convert.ToDecimal(rand.NextDouble() * 10 + 10);

            CourseDetailViewModel corso = new CourseDetailViewModel
            {
                Id = id,
                Title = $"Corso {id}",
                CurrentPrice = price,
                FullPrice = rand.NextDouble() > 0.5 ? price : price -1,
                Author = "Nome Cognome",
                Rating = rand.NextDouble() * 5.0,
                LogoPath = "/logo.png",
                Description = $"Descrizione {id}",
                Lessons = new List<LessonViewModel>()
            };

            for (int i = 1;i <= 5; i++)
            {
                LessonViewModel lesson = new LessonViewModel
                {
                    Title = $"Lezione {i}",
                    Duration = TimeSpan.FromSeconds(rand.Next(40, 90))                
                };
                corso.Lessons.Add(lesson);

            }

            return corso;



            
        }
    }
}