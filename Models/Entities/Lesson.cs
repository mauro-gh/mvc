using System;
using System.Collections.Generic;

namespace mvc.Models.Entities;

public partial class Lesson
{
    public int Id { get; private set; }

    public int CourseId { get; private set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public TimeSpan Duration { get; private set; }

    public virtual Course Course { get; private set; } = null!;
}
