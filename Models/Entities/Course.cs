﻿using System;
using System.Collections.Generic;
using mvc.Models.Enums;
using mvc.Models.ValueObjects;

namespace mvc.Models.Entities;

public partial class Course
{
    public Course(string title, string author, string authorId)
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
        AuthorId = authorId;

        Lessons = new HashSet<Lesson>();

        CurrentPrice = new 
        (Currency.EUR, 0);
        FullPrice = new Money(Currency.EUR, 0);
        LogoPath = "/Courses/default.png";

        Status = CourseStatus.Draft;
        
    }

    public int Id { get; private set; }

    public string Title { get; private set; } = null!;

    public string Description { get; private set; } = string.Empty;

    public string LogoPath { get; private set; } = string.Empty;

    public string Author { get; private set; } = null!;

    public string? Email { get; private set; } = string.Empty;

    public double Rating { get; private set; }

     public Money FullPrice { get; private set; }

    public Money CurrentPrice { get; private set; }

    public string? RowVersion { get; set; } = string.Empty;
    
    public CourseStatus Status {get;private set;}

    public string AuthorId { get; set; }

    public virtual ApplicationUser AuthorUser {get;set;}

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

    public void ChangeStatus (CourseStatus newStatus)
    {
        Status = newStatus;
    }
}
