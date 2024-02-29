using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using mvc.Models.Services.Infastructure;
using mvc.Models.ViewModels;
using SQLitePCL;

namespace mvc.Models.Services.Application
{
    public class AdoNetCourseService : ICourseService
    {
        private readonly IDatabaseAccessor db;
     
        public AdoNetCourseService(IDatabaseAccessor db)
        {
            this.db = db;
            
        }

        public async Task<CourseDetailViewModel> GetCourseAsync(int id)
        {
            FormattableString query = $@"SELECT * FROM Courses WHERE Id = {id};
                SELECT * FROM Lessons where CourseId = {id}" ;

            


            DataSet ds = await db.QueryAsync(query);

            // Corsi
            DataTable dtCorsi = ds.Tables[0];
            if (dtCorsi.Rows.Count != 1){
                throw new InvalidOperationException($"non ha restituito 1 record per il corso {id}");
            }

            DataRow drCorso = dtCorsi.Rows[0];
            CourseDetailViewModel courseDetailViewModel = CourseDetailViewModel.FromDataRow(drCorso);

            // Lezioni del corso
            DataTable dtLezioni = ds.Tables[1];

            foreach (DataRow drLesson in dtLezioni.Rows)
            {
                LessonViewModel lessonViewModel = LessonViewModel.FromDataRow(drLesson);
                courseDetailViewModel.Lessons.Add(lessonViewModel);
                
            }

            return courseDetailViewModel;
            
        }

        public async Task<List<CourseViewModel>> GetCoursesAsync()
        {
            FormattableString query = $@"SELECT Id, Title, LogoPath, Author, Rating, FullPrice_Amount, FullPrice_Currency, CurrentPrice_Amount, CurrentPrice_Currency  FROM Courses";
            DataSet ds = await db.QueryAsync(query);
            
            var dt = ds.Tables[0];

            var courseList = new List<CourseViewModel>();

            foreach (DataRow dr in dt.Rows)
            {
                CourseViewModel course = CourseViewModel.FromDataRow(dr);
                courseList.Add(course);

            }
        


            return courseList;
        }

        public string Version => throw new NotImplementedException();


    }
}