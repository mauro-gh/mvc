using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.Options;
using mvc.Models.Exceptions;
using mvc.Models.InputModels;
using mvc.Models.Options;
using mvc.Models.Services.Infrastructure;
using mvc.Models.ValueObjects;
using mvc.Models.ViewModels;
using SQLitePCL;

namespace mvc.Models.Services.Application
{
    public class AdoNetCourseService : ICourseService
    {
        private readonly ILogger<AdoNetCourseService> logger;
        private readonly IDatabaseAccessor db;
        private readonly IOptionsMonitor<CoursesOptions> coursesOptions;

        public string Version => throw new NotImplementedException();

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

        public async Task<ListViewModel<CourseViewModel>> GetCoursesAsync(CourseListInputModel model)
        {

            string orderby = model.OrderBy;
            // sanitizzazione del orderby (l'utente puo' scrivere qualsiasi cosa)
            if (orderby == "CurrentPrice")
            {
                orderby = "CurrentPrice_Amount";
            }
            
            // operatore terniario
            string direction = model.Ascending ? "ASC" : "DESC";




            // logica di sanitizzazione
            //search = search is null ? "" : search;
            //page = Math.Max(1, page); // sanitizzare il valore, potrebbe arrivare un -40
            //int limit = coursesOptions.CurrentValue.PerPage;
            //int offset = (page -1) * limit;
            //var orderOptions = coursesOptions.CurrentValue.Order;
            //if (!orderOptions.Allow.Contains(orderby))
            //{
                // valori di default da setting
            //    orderby = orderOptions.By;
            //    ascending = orderOptions.Ascending;
            //}    








            // prima query: tutti i corsi in base alla ricerca impostata
            // seconda query: per ottenere il totalCount
            FormattableString query = $@"
                SELECT 
                    Id, 
                    Title, 
                    LogoPath, 
                    Author, 
                    Rating, 
                    FullPrice_Amount, 
                    FullPrice_Currency, 
                    CurrentPrice_Amount, 
                    CurrentPrice_Currency  
                FROM 
                    Courses 
                WHERE 
                    Title LIKE {"%" +model.Search +"%"}
                    ORDER BY {(Sql) orderby} {(Sql) direction}
                LIMIT {model.Limit} 
                OFFSET {model.Offset};
                SELECT
                    COUNT(*)
                FROM 
                    Courses 
                WHERE 
                    Title LIKE {"%" +model.Search +"%"}
                "; 

            DataSet ds = await db.QueryAsync(query);
            
            var dtCorsi = ds.Tables[0];

            var courseList = new List<CourseViewModel>();

            foreach (DataRow dr in dtCorsi.Rows)
            {
                CourseViewModel course = CourseViewModel.FromDataRow(dr);
                courseList.Add(course);

            }

            ListViewModel<CourseViewModel> result = new ListViewModel<CourseViewModel>();
            // lista corsi (solo 10)
            result.Results = courseList;
            // totale corsi (tutti quelli presenti in tabella)
            result.TotalCount = Convert.ToInt32(ds.Tables[1].Rows[0][0]);
        


            return result;
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

        public async Task<CourseDetailViewModel> CreateCourseAsync(CourseCreateInputModel inputModel)
        {
            string title = inputModel.Title;
            string author = "Pippo Pelo";

            try
            {
                DataSet ds = await db.QueryAsync(@$"
                        INSERT INTO Courses (
                            Title,
                            Description,
                            Author, 
                            LogoPath, 
                            CurrentPrice_Currency,
                            CurrentPrice_Amount, 
                            FullPrice_Currency,
                            FullPrice_Amount)
                        VALUES(
                            {title},
                            {title},
                            {author},
                            '/Courses/default.png',
                            'EUR',
                            0,
                            'EUR',
                            0);

                        SELECT last_insert_rowid() as Id;
                ");

                int courseId = Convert.ToInt32(ds.Tables[0].Rows[0]["Id"]);
                CourseDetailViewModel course = await GetCourseAsync(courseId);

                return course;
                
            }
            catch (Exception exc)
            {
                throw new CourseTitleDuplicateException(title, exc);

            } 
            
        }

        public async Task<bool> IsTitleAvailableAsync(string title)
        {
            DataSet result = await db.QueryAsync(@$"SELECT COUNT(*) as Conteggio FROM Courses WHERE Title LIKE {title}");
            
            bool disponibile = ((Int64) result.Tables[0].Rows[0]["Conteggio"]) == 0 ? true : false;
            return disponibile;


        }

    public async Task<CourseEditInputModel> GetCourseForEditingAsync(int id)
    {
        FormattableString query = $@"SELECT Id, Title, Description, ImagePath, Email, FullPrice_Amount, FullPrice_Currency, CurrentPrice_Amount, CurrentPrice_Currency, RowVersion FROM Courses WHERE Id={id}";

        DataSet dataSet = await db.QueryAsync(query);

        DataTable courseTable = dataSet.Tables[0];
        if (courseTable.Rows.Count != 1)
        {
            logger.LogWarning("Course {id} not found", id);
            throw new CourseNotFoundException(id);
        }
        DataRow courseRow = courseTable.Rows[0];
        CourseEditInputModel courseEditInputModel = CourseEditInputModel.FromDataRow(courseRow);
        return courseEditInputModel;
    }

        public async Task<CourseDetailViewModel> SaveCourseAsync(CourseEditInputModel i)
        {
           try
            {
                // Salvataggio corso
                DataSet ds = await db.QueryAsync(@$"
                        UPDATE Courses SET 
                           Title = {i.Title},
                           Description = {i.Description},
                           Email = {i.Email},
                           CurrentPrice_Currency = {i.CurrentPrice.Currency},
                           CurrentPrice_Amount = {i.CurrentPrice.Amount},
                           FullPrice_Currency = {i.FullPrice.Currency},
                           FullPrice_Amount = {i.FullPrice.Amount}
                        WHERE
                           Id={i.Id}");

                // Rilettura corso da DB
                CourseDetailViewModel course = await GetCourseAsync(i.Id);
                return course;                
            }
            catch (Exception exc)
            {
                throw new CourseTitleDuplicateException(i.Title, exc);

            }             
        }
    }
}