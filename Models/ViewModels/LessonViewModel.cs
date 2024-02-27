using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace mvc.Models.ViewModels
{
    public class LessonViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TimeSpan Duration { get; set; }   

         public static LessonViewModel FromDataRow(DataRow dataRow)
        {
            var lessonViewModel = new LessonViewModel {
                Id = Convert.ToInt32(dataRow["Id"]),
                Title = Convert.ToString(dataRow["Title"]),
                Description = Convert.ToString(dataRow["Description"]),
                Duration = TimeSpan.Parse(Convert.ToString(dataRow["Duration"])),
            };
            return lessonViewModel;
        }
    }
}