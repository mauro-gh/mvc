using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
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

        public Task<List<CourseViewModel>> GetCoursesAsync()
        {
            TimeSpan time60sec = TimeSpan.FromSeconds(60);

            // prima lo cerca in cache, se non lo trova lo cerca dal DB
            Task<List<CourseViewModel>?> task = memoryCache.GetOrCreateAsync("Courses", cacheEntry =>
            {
                cacheEntry.SetSize(100);
                cacheEntry.SetAbsoluteExpiration(time60sec);
                return courseService.GetCoursesAsync();
            });

            return task;

        }

        public string Version => "1.0";
        
    }
}