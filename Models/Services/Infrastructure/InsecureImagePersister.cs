using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvc.Models.Services.Infrastructure
{
    public class InsecureImagePersister : IImagePersister
    {
        private IWebHostEnvironment env;
        private string webRootPath;
        private string contentPath;

        public InsecureImagePersister(IWebHostEnvironment env)
        {
            this.env = env;
            webRootPath = env.WebRootPath;
            contentPath = env.ContentRootPath;
            
        }

        public async Task<string> SaveCourseImageAsync(int courseId, IFormFile formFile)
        {
            // Salva il file in wwwrout (C:\_Svil\netcore\mvc\wwwroot\Courses)   
            string path = $"/Courses/{courseId}.jpg";         
            string fullPath = Path.Combine(webRootPath, "Courses", $"{courseId}.jpg" );

            using FileStream stream = File.OpenWrite(fullPath);

            await formFile.CopyToAsync(stream);


            // Restituisce il percorso al file
            return path;
        }
    }
}