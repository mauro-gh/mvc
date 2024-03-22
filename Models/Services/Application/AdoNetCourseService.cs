using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.Options;
using mvc.Models.Exceptions;
using mvc.Models.Options;
using mvc.Models.Services.Infrastructure;
using mvc.Models.ViewModels;
using SQLitePCL;

namespace mvc.Models.Services.Application
{
    public class AdoNetCourseService : ICourseService
    {
        private readonly ILogger<AdoNetCourseService> logger;
        private readonly IDatabaseAccessor db;
        private readonly IOptionsMonitor<CoursesOptions> coursesOptions;

        public AdoNetCourseService(
            ILogger<AdoNetCourseService> logger,    // log con sua categoria
            IDatabaseAccessor db, // nostra interfaccia per implementare metodo QueryAsync
            IOptionsMonitor<CoursesOptions> coursesOptions // sezione di configurazione nel appsetting
            )
        {
            this.coursesOptions = coursesOptions;
            string by = coursesOptions.CurrentValue.Order.By;

            long perpage = coursesOptions.CurrentValue.PerPage;
            this.logger = logger;
            this.db = db;
        }

        public async Task<CourseDetailViewModel> GetCourseAsync(int id)
        {

            // log strutturato
            logger.LogInformation("Couses {id} requested", id);


            FormattableString query = $@"SELECT * FROM Courses WHERE Id = {id};
                SELECT * FROM Lessons where CourseId = {id}" ;



            DataSet ds = await db.QueryAsync(query);

            // Corsi
            DataTable dtCorsi = ds.Tables[0];
            if (dtCorsi.Rows.Count != 1){
                logger.LogWarning("Course {id} not found", id);
                //throw new InvalidOperationException($"non ha restituito 1 record per il corso {id}");
                throw new CourseNotFoundException(id);
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

        public async Task<List<CourseViewModel>> GetCoursesAsync(string search, int page)
        {
            page = Math.Max(1, page); // sanitizzare il valore, potrebbe arrivare un -40
            int limit = coursesOptions.CurrentValue.PerPage;
            int offset = (page -1) * 10;
            
            FormattableString query = $@"SELECT Id, Title, LogoPath, Author, Rating, FullPrice_Amount, FullPrice_Currency, CurrentPrice_Amount, CurrentPrice_Currency  FROM Courses WHERE Title LIKE {"%" +search +"%"} LIMIT {limit} OFFSET {offset}";
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