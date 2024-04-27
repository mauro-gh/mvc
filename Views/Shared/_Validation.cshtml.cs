using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace mvc.Views.Shared
{
    public class _Validation : PageModel
    {
        private readonly ILogger<_Validation> _logger;

        public _Validation(ILogger<_Validation> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}