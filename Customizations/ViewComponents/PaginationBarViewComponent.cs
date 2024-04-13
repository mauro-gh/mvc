using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using mvc.Models.ViewModels;

namespace mvc.Customizations.ViewComponents
{
    // con questo attributo si puo' cambiare nome, ma anche la cartella deve essere coerente
    [ViewComponent(Name = "PaginationBar")]
    public class PaginationBarViewComponent : ViewComponent
    {
        // va sempre definito il metodo Invoke, con tutti i parametri necessari
        // lato html verra' richiamato cosi':
        // <vc:pagination-bar model="@Model"></vc:pagination-bar>
        //public IViewComponentResult Invoke(CourseListViewModel model)
        public IViewComponentResult Invoke(IPaginationInfo model)
        {
            // Numero di pag corrente, risultati totali, risultati per pagina
            // Search, orderby, ascending

            return View(model);
        }
  
    }
}