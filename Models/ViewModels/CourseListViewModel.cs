using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mvc.Models.InputModels;

namespace mvc.Models.ViewModels
{
    // questa classe verra' passata nelle viste per memorizzare
    // le scelte dell'utente (es. filtro, ordinamento, pagina corrente)
    public class CourseListViewModel
    {
        
        public ListViewModel<CourseViewModel> Courses  {get; set;}

        public CourseListInputModel Input { get; set; }
        



    }
}