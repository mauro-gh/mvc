using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mvc.Models.ViewModels;

namespace mvc.Models.Services.Application
{
    public interface ICourseService
    {
        // dichiaro tutti i metodi di questo contratto

        Task<ListViewModel<CourseViewModel>> GetCoursesAsync(string search, int page, string orderby, bool ascending);

        Task<CourseDetailViewModel> GetCourseAsync(int id);

        public string Version { get; }


    }
}