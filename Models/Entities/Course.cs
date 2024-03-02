using System;
using System.Collections.Generic;

namespace mvc.Models.Entities;

public partial class Course
{
    public Course(string title, string author)
    {
        if (string.IsNullOrEmpty(title))
        {
            throw new ArgumentException("Corso privo di titolo");
        }
        if (string.IsNullOrEmpty(author))
        {
            throw new ArgumentException("Corso privo di autore");
        }

        Title = title;
        Author = author;

        Lessons = new HashSet<Lesson>();
    }

    public int Id { get; private set; }

    public string Title { get; private set; } = null!;

    public string Description { get; private set; } = string.Empty;

    public string LogoPath { get; private set; } = string.Empty;

    public string Author { get; private set; } = null!;

    public string? Email { get; private set; }

    public double Rating { get; private set; }

    public double FullPriceAmount { get; private set; }

    public string FullPriceCurrency { get; private set; } = null!;

    public double CurrentPriceAmount { get; private set; }

    public string CurrentPriceCurrency { get; private set; } = null!;

    public virtual ICollection<Lesson> Lessons { get; private set; } = new List<Lesson>();

    public void ChangeTitle(string newTitle)
    {
        if (string.IsNullOrEmpty(newTitle))
        {
            throw new ArgumentException("Nuovo titolo vuoto");
        }     

        Title = newTitle;

    }

    public void ChangePrices()
    {
        
    }



}
