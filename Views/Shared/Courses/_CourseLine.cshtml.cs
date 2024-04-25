using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace mvc.Views.Shared
{
    public class _CourseLine : PageModel
    {
        private readonly ILogger<_CourseLine> _logger;

        public _CourseLine(ILogger<_CourseLine> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}