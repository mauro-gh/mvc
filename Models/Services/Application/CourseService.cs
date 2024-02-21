using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mvc.Models.ViewModels;

namespace mvc.Models.Services.Application
{
    public class CourseService
    {
        public List<CourseViewModel> GetServices()
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
                    Author = "Nome cognome",
                    Rating = rand.NextDouble() * 5.0,
                    ImagePath = "~/logo.png"
                };
                courseList.Add(course);

            }
            return courseList;

        }

    }
}