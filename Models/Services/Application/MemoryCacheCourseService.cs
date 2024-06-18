using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using mvc.Models.InputModels;
using mvc.Models.ViewModels;

namespace mvc.Models.Services.Application
{
    public class MemoryCacheCourseService : ICachedCourseService
    {
        private readonly ICourseService courseService;
        private readonly IMemoryCache memoryCache;

        public MemoryCacheCourseService(ICourseService courseService, IMemoryCache memoryCache)
    {
            this.courseService = courseService;
            this.memoryCache = memoryCache;
        }

        // Utilizzare memoryCache.Remove($"Course{id}") quando si aggiorna corso su DB

        public Task<CourseDetailViewModel> GetCourseAsync(int id)
        {

            TimeSpan time60sec = TimeSpan.FromSeconds(60);

            // prima lo cerca in cache, se non lo trova lo cerca dal DB
            Task<CourseDetailViewModel?> task = memoryCache.GetOrCreateAsync($"Course{id}", cacheEntry =>
            {
                cacheEntry.SetSize(1); // occupazione in ram di ogni oggetto (in byte o unita')
                cacheEntry.SetAbsoluteExpiration(time60sec); // durata della cache (meglio se preso ad options)
                return courseService.GetCourseAsync(id);
            });

            return task;

        }

        public Task<ListViewModel<CourseViewModel>> GetCoursesAsync(CourseListInputModel model)
        {
            TimeSpan time60sec = TimeSpan.FromSeconds(60);

            // prima lo cerca in cache, se non lo trova lo cerca dal DB
            // e lo associa SEMPRE alla stessa chiave Courses, quindi in caso di
            // search valorizzato non restituisce i valori aggiornati
            Task<ListViewModel<CourseViewModel>?> task = memoryCache.GetOrCreateAsync($"Courses{model.Page}-{model.OrderBy}-{model.Ascending}-{model.Search}", cacheEntry =>
            {
                cacheEntry.SetSize(100);
                cacheEntry.SetAbsoluteExpiration(time60sec);
                return courseService.GetCoursesAsync(model);
            });

            return task;

        }

        public Task<List<CourseViewModel>> GetBestRatingCoursesAsync()
        {

            TimeSpan time60sec = TimeSpan.FromSeconds(60);

            Task<List<CourseViewModel>?> task = memoryCache.GetOrCreateAsync($"BestRatingCourses", cacheEntry  =>
            {
                cacheEntry.SetSize(100);
                cacheEntry.SetAbsoluteExpiration(time60sec);
                return courseService.GetBestRatingCoursesAsync();

            });

            return task;


        }

        public Task<List<CourseViewModel>> GetMostRecentCoursesAsync()
        {
            TimeSpan time60sec = TimeSpan.FromSeconds(60);

            Task<List<CourseViewModel>?> task = memoryCache.GetOrCreateAsync($"MostRecentCourses", cacheEntry  =>
            {
                cacheEntry.SetSize(100);
                cacheEntry.SetAbsoluteExpiration(time60sec);
                return courseService.GetMostRecentCoursesAsync();

            });

            return task;
        }

        public Task<CourseDetailViewModel> CreateCourseAsync(CourseCreateInputModel inputModel)
        {
            // non si usa la cache negli inserimenti!
            return courseService.CreateCourseAsync(inputModel);
        }

        public Task<bool> IsTitleAvailableAsync(string title, int id)
        {
            return courseService.IsTitleAvailableAsync(title, id);
        }

        public Task<CourseEditInputModel> GetCourseForEditingAsync(int id)
        {
            return courseService.GetCourseForEditingAsync(id);
        }

        public async Task<CourseDetailViewModel> SaveCourseAsync(CourseEditInputModel inputModel)
        {

            // Utilizzare memoryCache.Remove($"Course{id}") quando si aggiorna corso su DB
            
            //return courseService.SaveCourseAsync(inputModel);

            // aspettiamo il salvataggio
            // invalidiamo la cache
            // lo restituiamo            

            CourseDetailViewModel viewModel = await courseService.SaveCourseAsync(inputModel);
            memoryCache.Remove($"Course{inputModel.Id}") ;
            return viewModel;



        }

        public async Task DeleteCourseAsync(CourseDeleteInputModel inputModel)
        {
            await courseService.DeleteCourseAsync(inputModel);
            memoryCache.Remove($"Course{inputModel.Id}");

        }

        public Task SendQuestionToCourseAuthorAsync(int id, string contactQuestion)
        {
            return courseService.SendQuestionToCourseAuthorAsync(id, contactQuestion);
        }

        public string Version => "1.0";
        
    }
}