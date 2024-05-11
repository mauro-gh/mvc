using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace mvc.Models.InputModels
{
    public class CourseDeleteInputModel
    {
        [Required]
        public int Id { get; set; }

    }
}