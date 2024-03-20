using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using mvc.Models.Entities;
using mvc.Models.Options;
using mvc.Models.Services.Infrastructure;
using mvc.Models.ViewModels;

namespace mvc.Models.Services.Application
{
    public class EfCoreCourseService : ICourseService
    {
        private readonly MyCourseDbContext dbContext;
        private readonly IOptionsMonitor<CoursesOptions> coursesOptions;

        public EfCoreCourseService(MyCourseDbContext dbContext, IOptionsMonitor<CoursesOptions> coursesOptions )
        {
            this.dbContext = dbContext;
            this.coursesOptions = coursesOptions;
        }

        public string Version => "1.0";

        public async Task<CourseDetailViewModel> GetCourseAsync(int id)
        {
                CourseDetailViewModel corso = await dbContext.Courses
                    .AsNoTracking()
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

        public async Task<List<CourseViewModel>> GetCoursesAsync(string search)
        {
            // mappatura tra viewmodel e classe ef
            // List<CourseViewModel> courses = await dbContext.Courses.Select(course =>
            // new CourseViewModel
            // {
            //     Id = course.Id,
            //     Title = course.Title,
            //     LogoPath = course.LogoPath,
            //     Author = course.Author,
            //     Rating = course.Rating,
            //     CurrentPriceAmount = Convert.ToDecimal(course.CurrentPriceAmount),
            //     FullPriceAmount = Convert.ToDecimal(course.FullPriceAmount)
            // })
            // .ToListAsync();

            // il FromEntity estrarrebbe tutti i campi, anche quelli che non servono
            // non tirerebbe fuori le tabelle esterne: serve Include(course => course.Lessons) Eager Loading

            // usare AsNoTracking() se non serve il change tracker (tracciamento delle modifiche)


            search = search ?? "";


            // separare query ed esecuzione
            IQueryable<CourseViewModel> queryLinq = dbContext.Courses
            .Where(course => course.Title.Contains(search, StringComparison.InvariantCultureIgnoreCase))  // clausola WHERE
            .AsNoTracking()
            .Select(course =>
                new CourseViewModel
                {
                    Id = course.Id,
                    Title = course.Title,
                    LogoPath = course.LogoPath,
                    Author = course.Author,
                    Rating = course.Rating,
                    CurrentPriceAmount = Convert.ToDecimal(course.CurrentPriceAmount),
                    FullPriceAmount = Convert.ToDecimal(course.FullPriceAmount)
                });
            //.OrderBy(course => course.Title);
            
            
            List<CourseViewModel> courses = await queryLinq.ToListAsync();

            // eventualmente scorrere le liste (IEnumerable<>)
            foreach (CourseViewModel c in courses)
            {
                     string corso = c.Title;
            }

            // e mai scorrere le query link perchè verrebbero rieseguite una seconda volta (IQueryable<>)
            // foreach (CourseViewModel c in queryLinq)
            // {
            //          string corso = c.Title;
            // }

            return courses;

        }
    }
}


