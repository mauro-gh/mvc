using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvc.Models.Services.Infrastructure
{
    public interface IImagePersister
    {
        /// <summary>
        /// SaveCourseImage persiste immagine
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="formFile"></param>
        /// <returns>Image url, es. /Course/1.jpg</returns>
        Task <string> SaveCourseImageAsync(int courseId, IFormFile formFile);
    }
}