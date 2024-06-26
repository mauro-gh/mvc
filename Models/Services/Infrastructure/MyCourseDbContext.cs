﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using mvc.Models.Entities;

namespace mvc.Models.Services.Infrastructure;

public partial class MyCourseDbContext : IdentityDbContext<ApplicationUser>
{


    public MyCourseDbContext(DbContextOptions<MyCourseDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Lesson> Lessons { get; set; }

//     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
// #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//         => optionsBuilder.UseSqlite("Data Source=Data/MyCourse.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Eredito il mapping del modello base, ovvero IdentityDbContext
        base.OnModelCreating(modelBuilder);


        modelBuilder.Entity<Course>(entity =>
        {
            entity.ToTable("Courses");  // nome tabella
            entity.HasKey(course => course.Id); // pk

            entity.HasIndex(course => course.Title).IsUnique();
            

            // mapping per owned types (Money)
            entity.OwnsOne(course => course.CurrentPrice, builder => {
                builder.Property(money => money.Currency)
                .HasConversion<string>()
                .HasColumnName("CurrentPrice_Currency"); //Superfluo perché le nostre colonne seguono già la convenzione di nomi
                builder.Property(money => money.Amount)
                .HasColumnName("CurrentPrice_Amount")
                .HasConversion<decimal>(); //Superfluo perché le nostre colonne seguono già la convenzione di nomi
            });

            entity.OwnsOne(course => course.FullPrice, builder => {
                builder.Property(money => money.Currency)
                .HasConversion<string>()
                .HasColumnName("FullPrice_Currency");
                builder.Property(money => money.Amount)
                .HasColumnName("FullPrice_Amount")
                .HasConversion<decimal>();
            });            

            // mapping per le relazioni, il corso vede N lezioni
            // relazione corso (identita principale) - lezioni (non puo' esistere senza corso)
            entity.HasMany(course => course.Lessons)
                .WithOne(lesson => lesson.Course)
                .HasForeignKey(lesson => lesson.CourseId);

            // ogni corso ha un solo AUTORE, tramite:
            // corso AuthorId = utenti id
            // relazione corso (non puo' esistere senza corso) - utente (identita' principale)
            entity.HasOne(course => course.AuthorUser)
                .WithMany(user => user.AuthoredCourses)
                .HasForeignKey(course => course.AuthorId);



            // entity.Property(e => e.CurrentPriceAmount)
            //     .HasColumnType("NUMERIC")
            //     .HasColumnName("CurrentPrice_Amount");

            // entity.Property(e => e.FullPriceAmount)
            //     .HasColumnType("NUMERIC")
            //     .HasColumnName("FullPrice_Amount");

                



            #region generato automaticamente

            // entity.Property(e => e.Author).HasColumnType("Text (100)");
            // entity.Property(e => e.CurrentPriceAmount)
            //     .HasColumnType("NUMERIC")
            //     .HasColumnName("CurrentPrice_Amount");
            // entity.Property(e => e.CurrentPriceCurrency)
            //     .HasDefaultValue("EUR")
            //     .HasColumnType("TEXT(3)")
            //     .HasColumnName("CurrentPrice_Currency");
            // entity.Property(e => e.Description).HasColumnType("Text (10000)");
            // entity.Property(e => e.Email).HasColumnType("Text(100)");
            // entity.Property(e => e.FullPriceAmount)
            //     .HasColumnType("NUMERIC")
            //     .HasColumnName("FullPrice_Amount");
            // entity.Property(e => e.FullPriceCurrency)
            //     .HasDefaultValue("EUR")
            //     .HasColumnType("TEXT(3)")
            //     .HasColumnName("FullPrice_Currency");
            // entity.Property(e => e.LogoPath).HasColumnType("Text (100)");
            // entity.Property(e => e.Title).HasColumnType("Text (100)");

            // per concorrenza ottimistica
            entity.Property(course => course.RowVersion).IsRowVersion();

            entity.Property(course => course.Status).HasConversion<string>();

            // Global Query Filter
            entity.HasQueryFilter(c => c.Status != Enums.CourseStatus.Deleted);
            

            #endregion

        });


        modelBuilder.Entity<Lesson>(entity =>
        {

            // mapping per le relazioni, la lezione vede un solo corso
            // entity.HasOne(lesson => lesson.Course)
            // .WithMany(course => course.Lessons);


            // #region generato automaticamente
            
            // entity.Property(e => e.Description).HasColumnType("TEXT (10000)");
            // entity.Property(e => e.Duration)
            //     .HasDefaultValueSql("'00:00:00'")
            //     .HasColumnType("TEXT (8)");
            // entity.Property(e => e.Title).HasColumnType("TEXT (100)");

            // entity.HasOne(d => d.Course).WithMany(p => p.Lessons).HasForeignKey(d => d.CourseId);
            // #endregion
            
            
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
