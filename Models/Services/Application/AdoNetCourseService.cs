using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using mvc.Models.Services.Infastructure;
using mvc.Models.ViewModels;

namespace mvc.Models.Services.Application
{
    public class AdoNetCourseService : ICourseService
    {
        private readonly IDatabaseAccessor db;
     
        public AdoNetCourseService(IDatabaseAccessor db)
        {
            this.db = db;
            
        }

        public CourseDetailViewModel GetCourse(int id)
        {
            throw new NotImplementedException();
        }

        public List<CourseViewModel> GetCourses()
        {
            string query = "SELECT * FROM Courses";
            DataSet ds = db.Query(query);
            throw new NotImplementedException();
        }

        public string Version => throw new NotImplementedException();


    }
}