using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using mvc.Models.Entities;
using mvc.Models.Services.Infrastructure;
using mvc.Models.ViewModels;

namespace mvc.Models.Services.Application
{
    public class EfCoreCourseService : ICourseService
    {
        private readonly MyCourseDbContext dbContext;

        public EfCoreCourseService(MyCourseDbContext dbContext)
        {
            this.dbContext = dbContext;
            

        }

        public string Version => "1.0";

        public async Task<CourseDetailViewModel> GetCourseAsync(int id)
        {
                CourseDetailViewModel corso = await dbContext.Courses
                    .Where(course => course.Id == id)
                    .Select(course => new CourseDetailViewModel{
                        Id = course.Id,
                        Title = course.Title,
                        Description = course.Description,
                        LogoPath = course.LogoPath,
                        Author = course.Author,
                        Rating = course.Rating,
                        CurrentPriceAmount = Convert.ToDecimal(course.CurrentPriceAmount),
                        FullPriceAmount = Convert.ToDecimal(course.FullPriceAmount),
                        Lessons = course.Lessons.Select(lesson => new LessonViewModel{
                            Id = lesson.Id,
                            Title = lesson.Title,
                            Description = lesson.Description,
                            Duration = lesson.Duration
                        }).ToList() // serve una lista!
                    })
                    .SingleAsync(); // va bene solo se restituisce 1 record, altrimenti eccezione
                    //.First --> restiuisce il primo, eccezione solo elenco vuoto
                    //.FirstOrDefaultAsync // restituisce il primo, null se vuoto, mai eccezioni

                return corso;
        }

        public async Task<List<CourseViewModel>> GetCoursesAsync()
        {
            // mappatura tra viewmodel e classe ef
            List<CourseViewModel> courses = await dbContext.Courses.Select(course =>
            new CourseViewModel
            {
                Id = course.Id,
                Title = course.Title,
                LogoPath = course.LogoPath,
                Author = course.Author,
                Rating = course.Rating,
                CurrentPriceAmount = Convert.ToDecimal(course.CurrentPriceAmount),
                FullPriceAmount = Convert.ToDecimal(course.FullPriceAmount)
            })
            .ToListAsync();
            return courses;

        }
    }
}


