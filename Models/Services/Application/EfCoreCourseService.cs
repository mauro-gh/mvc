using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using mvc.Models.Entities;
using mvc.Models.Exceptions;
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
        private IHttpContextAccessor httpContextAccessor;

        public EfCoreCourseService(MyCourseDbContext dbContext, 
                IOptionsMonitor<CoursesOptions> coursesOptions,
                IHttpContextAccessor httpContextAccessor )
        {
            this.dbContext = dbContext;
            this.coursesOptions = coursesOptions;
            this.httpContextAccessor = httpContextAccessor;
        }

        public string Version => "1.0";



        public async Task<CourseDetailViewModel> GetCourseAsync(int id)
        {
                CourseDetailViewModel corso = await dbContext.Courses
                    .AsNoTracking()
                    .Where(course => course.Id == id)
                    .Select(course => CourseDetailViewModel.FromEntity(course))
                    // .Select(course => new CourseDetailViewModel{
                    //     Id = course.Id,
                    //     Title = course.Title,
                    //     Description = course.Description,
                    //     LogoPath = course.LogoPath,
                    //     Author = course.Author,
                    //     Rating = course.Rating,
                    //     CurrentPriceAmount = Convert.ToDecimal(course.CurrentPriceAmount),
                    //     FullPriceAmount = Convert.ToDecimal(course.FullPriceAmount),
                    //     Lessons = course.Lessons.Select(lesson => new LessonViewModel{
                    //         Id = lesson.Id,
                    //         Title = lesson.Title,
                    //         Description = lesson.Description,
                    //         Duration = lesson.Duration
                    //     }).ToList() // serve una lista!
                    //})
                    .SingleAsync(); // va bene solo se restituisce 1 record, altrimenti eccezione
                    //.First --> restiuisce il primo, eccezione solo elenco vuoto
                    //.FirstOrDefaultAsync // restituisce il primo, null se vuoto, mai eccezioni

            //.FirstOrDefaultAsync(); //Restituisce null se l'elenco è vuoto e non solleva mai un'eccezione
            //.SingleOrDefaultAsync(); //Tollera il fatto che l'elenco sia vuoto e in quel caso restituisce null, oppure se l'elenco contiene più di 1 elemento, solleva un'eccezione
            //.FirstAsync(); //Restituisce il primo elemento, ma se l'elenco è vuoto solleva un'eccezione
            //.SingleAsync(); //Restituisce il primo elemento, ma se l'elenco è vuoto o contiene più di un elemento, solleva un'eccezione


                return corso;
        }

        public async Task<ListViewModel<CourseViewModel>> GetCoursesAsync(CourseListInputModel model)
        {

            var user = httpContextAccessor.HttpContext;
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
                        baseQuery = baseQuery.OrderBy(course => course.CurrentPrice.Amount);
                    }
                    else
                    {
                        baseQuery = baseQuery.OrderByDescending(course => course.CurrentPrice.Amount);
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
            .Where(course => course.Title.ToUpper().Contains(model.Search.ToUpper()))  // clausola WHERE
            //.AsNoTracking()     // non traccia le modifiche
            .Select(course => CourseViewModel.FromEntity(course));
            // .Select(course =>
            //     new CourseViewModel     // sostituibile con CourseViewModel.FromEntity(course)
            //     {
            //         Id = course.Id,
            //         Title = course.Title,
            //         LogoPath = course.LogoPath,
            //         Author = course.Author,
            //         Rating = course.Rating,
            //         CurrentPriceAmount = Convert.ToDecimal(course.CurrentPriceAmount),
            //         FullPriceAmount = Convert.ToDecimal(course.FullPriceAmount)
            //     });
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

        public async Task<CourseDetailViewModel> CreateCourseAsync(CourseCreateInputModel inputModel)
        {
            // qui fara' tutto il dbcontext (add, update, remove...)
            // i dati non vengono salvati subito, ma parcheggiata nel change tracker(added),
            // e solo nel SaveChanges viene salvato (lo stato diventa unchanged)
            // Per conoscere lo stato:
            // dbContext.Entry(course).State = EntityState.Added

            string title = inputModel.Title;

            string author = httpContextAccessor.HttpContext.User.FindFirst("NomeCompleto").Value;

            string authorId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            // nuova istanza
            var course = new Course(title, author,authorId);

            // var lesson01 = new Lesson()
            // {Title="Lezione EF 01", Description="Descrizione lezione 01"};
            // var lesson02 = new Lesson()
            // {Title="Lezione EF 02", Description="Descrizione lezione 02"};
            // course.Lessons.Add(lesson01);
            // course.Lessons.Add(lesson02);


            // la affidiamo al dbcontext
            dbContext.Add(course);

            try
            {
                await dbContext.SaveChangesAsync();
                // detached -> added -> unchanged
                
            }
            catch (DbUpdateException exc)
            {
                
                throw new CourseTitleDuplicateException(title, exc);
            }


            return CourseDetailViewModel.FromEntity(course);

        }

        public async Task<bool> IsTitleAvailableAsync(string title, int id)
        {
            
            
            bool titoloesistente = await dbContext.Courses.AnyAsync(course => EF.Functions.Like(course.Title, title) && course.Id != id);
            return !titoloesistente;


        }

        public async Task<CourseEditInputModel> GetCourseForEditingAsync(int id)
        {
            IQueryable<CourseEditInputModel> queryLinq = dbContext.Courses
                .AsNoTracking()
                .Where(course => course.Id == id)
                .Select(course => CourseEditInputModel.FromEntity(course)); //Usando metodi statici come FromEntity, la query potrebbe essere inefficiente. Mantenere il mapping nella lambda oppure usare un extension method personalizzato

            CourseEditInputModel viewModel = await queryLinq.FirstOrDefaultAsync();

            if (viewModel == null)
            {
                //logger.LogWarning("Course {id} not found", id);
                throw new CourseNotFoundException(id);
            }

            return viewModel;
        }

        public async Task<CourseDetailViewModel> SaveCourseAsync(CourseEditInputModel inputModel)
        {
            Course c = await dbContext.Courses.FindAsync(inputModel.Id);

            if (c != null)
            {
                c.ChangeTitle(inputModel.Title);
                c.ChangePrices(inputModel.FullPrice, inputModel.CurrentPrice);
                c.ChangeDescription(inputModel.Description);
                c.ChangeEmail(inputModel.Email);

                dbContext.Entry(c).Property(c => c.RowVersion).OriginalValue = inputModel.RowVersion;


                //TODO: aggiungere imagepersister e changeimagepath per salvare immagine


                try
                {
                    await dbContext.SaveChangesAsync();
                }
                // catch (DbUpdateException exc)
                // {
                    
                //     throw new CourseTitleDuplicateException(inputModel.Title, exc);
                // }
                catch (DbUpdateConcurrencyException)
                {
                    
                    throw new OptimisticConcurrencyException();
                }
                catch (Exception)
                {
                    throw;
                }

                return CourseDetailViewModel.FromEntity(c);

            }
            else
            {
                throw new Exception("Corso non trovato");
            }



        }

        public async Task DeleteCourseAsync(CourseDeleteInputModel i)
        {
            // ottengo l'entita'
            Course c = dbContext.Courses.Find(i.Id);

            if (c == null)
            {
                throw new CourseNotFoundException(i.Id);
            }

            // elimino il corso logicamente
            c.ChangeStatus(Enums.CourseStatus.Deleted);
            await dbContext.SaveChangesAsync();


        }
    }
}


