using System;
using System.Collections.Generic;
using MyCourse.Models.Enums;
using MyCourse.Models.ValueObjects;

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

        CurrentPrice = new Money(Currency.EUR, 0);
        FullPrice = new Money(Currency.EUR, 0);
        LogoPath = "/Courses/default.png";
        
    }

    public int Id { get; private set; }

    public string Title { get; private set; } = null!;

    public string Description { get; private set; } = string.Empty;

    public string LogoPath { get; private set; } = string.Empty;

    public string Author { get; private set; } = null!;

    public string? Email { get; private set; }

    public double Rating { get; private set; }

     public Money FullPrice { get; private set; }

    public double FullPriceAmount { get; private set; }

    public string FullPriceCurrency { get; private set; } = null!;

    public Money CurrentPrice { get; private set; }

    public double CurrentPriceAmount { get; private set; }

    public string CurrentPriceCurrency { get; private set; } = null!;

    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();

    public void ChangeTitle(string newTitle)
    {
        if (string.IsNullOrEmpty(newTitle))
        {
            throw new ArgumentException("Nuovo titolo vuoto");
        }     

        Title = newTitle;

    }

    public void ChangePrices(Money newFullPrice, Money newCurrentPrice)
    {
        if (newFullPrice == null || newCurrentPrice == null)
        {
            throw new ArgumentException("Prices can't be null");
        }
        if (newFullPrice.Currency != newCurrentPrice.Currency)
        {
            throw new ArgumentException("Currencies don't match");
        }
        if (newFullPrice.Amount < newCurrentPrice.Amount)
        {
            throw new ArgumentException("Full price can't be less than the current price");
        }
        FullPrice = newFullPrice;
        CurrentPrice = newCurrentPrice;
    }

    public void ChangeDescription(string description)
    {
        if (string.IsNullOrEmpty(description))
        {
            throw new ArgumentException("Descrizione vuota");
        }

        Description = description;

    }

    internal void ChangeEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            throw new ArgumentException("Email vuota");
        }

        Email = email;
    }
}
