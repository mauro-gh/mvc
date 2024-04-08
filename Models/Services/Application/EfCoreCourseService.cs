using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using mvc.Models.Entities;
using mvc.Models.InputModels;
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

        public async Task<ListViewModel<CourseViewModel>> GetCoursesAsync(CourseListInputModel model)
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

            // logica di sanitizzazione
            //search = search ?? "";

            //page = Math.Max(1, page); // sanitizzare il valore, potrebbe arrivare un -40
            //int limit = coursesOptions.CurrentValue.PerPage;
            //int offset = (page -1) * 10;
              // sanitizzazione del orderby (l'utente puo' scrivere qualsiasi cosa)
            //if (orderby == "CurrentPrice")
            //{
            //    orderby = "CurrentPrice_Amount";
            //}

            //var orderOptions = coursesOptions.CurrentValue.Order;
            //if (!orderOptions.Allow.Contains(orderby))
            //{
                // valori di default da setting
            //    orderby = orderOptions.By;
            //    ascending = orderOptions.Ascending;
            //}    


            // creazione query base
            IQueryable<Course> baseQuery = dbContext.Courses;

            // in base a ordinamento aggiungiamo pezzi di query

            switch (model.OrderBy)
            {
                case "Title":
                    if (model.Ascending)
                    {
                        baseQuery = baseQuery.OrderBy(course => course.Title);
                    }
                    else
                    {
                        baseQuery = baseQuery.OrderByDescending(course => course.Title);
                    }
                    break;

                case "Rating":
                    if (model.Ascending)
                    {
                        baseQuery = baseQuery.OrderBy(course => course.Rating);
                    }
                    else
                    {
                        baseQuery = baseQuery.OrderByDescending(course => course.Rating);
                    }
                    break;
                case "CurrentPrice":
                    if (model.Ascending)
                    {
                        baseQuery = baseQuery.OrderBy(course => course.CurrentPriceAmount);
                    }
                    else
                    {
                        baseQuery = baseQuery.OrderByDescending(course => course.CurrentPriceAmount);
                    }
                    break;
                case "Id":
                    if (model.Ascending)
                    {
                        baseQuery = baseQuery.OrderBy(course => course.Id);
                    }
                    else
                    {
                        baseQuery = baseQuery.OrderByDescending(course => course.Id);
                    }
                    break;                    

                
                default:
                    break;
            }




            // separare query ed esecuzione
            IQueryable<CourseViewModel> queryLinq = baseQuery
            .Where(course => course.Title.Contains(model.Search, StringComparison.InvariantCultureIgnoreCase))  // clausola WHERE
            .AsNoTracking()     // non traccia le modifiche
            .Select(course =>
                new CourseViewModel     // sostituibile con CourseViewModel.FromEntity(course)
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
            
            // la query viene inviata SOLO in questo punto, prima era solo preparata
            List<CourseViewModel> courses = await queryLinq
                    .Skip(model.Offset)       // salta i primi N record --> offset
                    .Take(model.Limit)        // prende i successivi N record --> limit
                    .ToListAsync();
            // esegue la query preparata prima, senza skip e take
            int totalCount = await queryLinq.CountAsync();

            ListViewModel<CourseViewModel> result = new ListViewModel<CourseViewModel>();
            result.Results = courses;
            result.TotalCount = totalCount;


            // eventualmente scorrere le liste (IEnumerable<>)
            // foreach (CourseViewModel c in courses)
            // {
            //          string corso = c.Title;
            // }

            // e mai scorrere le query link perchè verrebbero rieseguite una seconda volta (IQueryable<>)
            // foreach (CourseViewModel c in queryLinq)
            // {
            //          string corso = c.Title;
            // }

            return result;

        }

        public async Task<List<CourseViewModel>> GetMostRecentCoursesAsync()
        {
            CourseListInputModel inputModel = new CourseListInputModel(
                search: "",
                page: 1,
                orderby: "Id",
                ascending: false,
                limit: coursesOptions.CurrentValue.InHome,
                orderOptions: coursesOptions.CurrentValue.Order);

                ListViewModel<CourseViewModel> result = await GetCoursesAsync(inputModel);
                return result.Results;
        }

        public async Task<List<CourseViewModel>> GetBestRatingCoursesAsync()
        {
            CourseListInputModel inputModel = new CourseListInputModel(
                search: "",
                page: 1,
                orderby: "Rating",
                ascending: false,
                limit: coursesOptions.CurrentValue.InHome,
                orderOptions: coursesOptions.CurrentValue.Order);

                ListViewModel<CourseViewModel> result = await GetCoursesAsync(inputModel);
                return result.Results;
        }

    }
}


