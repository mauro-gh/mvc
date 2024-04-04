using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using mvc.Models.InputModels;
using mvc.Models.Options;

namespace mvc.Customizations.ModelBinders
{
    

    // deve implementare IModelBinder
    // nel BindModelAsync occorre costruire istanza di CourseListInputModel

    public class CourseListInputModelBinder : IModelBinder
    {
        
        private readonly IOptionsMonitor<CoursesOptions> courseOptions;

        // Tramite dependency injection inserisco un costruttore che accetta l'interfaccia IOptionMonitor,
        // in modo tale da trovarmi l'istanza gia' valorizzata
        public CourseListInputModelBinder(IOptionsMonitor<CoursesOptions> courseOptions)
        {
            this.courseOptions = courseOptions;
        }



        // Estrapolo tutti i valori e li verifico
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            //Recuperiamo i valori grazie ai value provider
            string search = bindingContext.ValueProvider.GetValue("Search").FirstValue;
            string orderBy = bindingContext.ValueProvider.GetValue("OrderBy").FirstValue;
            int.TryParse(bindingContext.ValueProvider.GetValue("Page").FirstValue, out int page);
            bool.TryParse(bindingContext.ValueProvider.GetValue("Ascending").FirstValue, out bool ascending);

            //Creiamo l'istanza del CourseListInputModel
            var inputModel = new CourseListInputModel(search, page, orderBy, ascending, courseOptions.CurrentValue);

            //Impostiamo il risultato per notificare che la creazione è avvenuta con successo
            bindingContext.Result =  ModelBindingResult.Success(inputModel);

            //Restituiamo un task completato
            return Task.CompletedTask;


        }
    }
}