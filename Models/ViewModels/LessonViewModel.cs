using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvc.Models.ViewModels
{
    public class LessonViewModel
    {
        public string Title { get; set; } = string.Empty;
        public TimeSpan Duration { get; set; }

    }
}