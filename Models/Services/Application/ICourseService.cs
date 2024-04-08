using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mvc.Models.InputModels;
using mvc.Models.ViewModels;

namespace mvc.Models.Services.Application
{
    public interface ICourseService
    {
        // dichiaro tutti i metodi di questo contratto

        Task<ListViewModel<CourseViewModel>> GetCoursesAsync(CourseListInputModel model);

        Task<CourseDetailViewModel> GetCourseAsync(int id);

        Task<List<CourseViewModel>> GetBestRatingCoursesAsync();

        Task<List<CourseViewModel>> GetMostRecentCoursesAsync();

        public string Version { get; }


    }
}