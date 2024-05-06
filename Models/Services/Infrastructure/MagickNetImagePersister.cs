using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageMagick;

namespace mvc.Models.Services.Infrastructure
{
    public class MagickNetImagePersister : IImagePersister
    {

        private IWebHostEnvironment env;
        private string webRootPath;
        private string contentPath;

        public MagickNetImagePersister(IWebHostEnvironment env)
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

            using Stream inputStream = formFile.OpenReadStream();
            using MagickImage image = new MagickImage(inputStream);

            // Manipolare immagine, ridimensionare mantenendo le proporizione e tagliano l'eccesso partendo dal centro
            int width = 300;
            int height = 300;

            MagickGeometry resize = new MagickGeometry(width, height)
            {
                FillArea  = true
            };
            image.Resize(resize);
            image.Crop(width, height, Gravity.Center);
            image.Quality = 90;

            // salva su file system
            image.Write(fullPath, MagickFormat.Jpg);


            
            // Restituisce il percorso al file
            return path;
        }
    }
}