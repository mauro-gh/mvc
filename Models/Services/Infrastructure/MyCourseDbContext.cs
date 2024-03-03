using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using mvc.Models.Entities;

namespace mvc.Models.Services.Infrastructure;

public partial class MyCourseDbContext : DbContext
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
        
        modelBuilder.Entity<Course>(entity =>
        {
            entity.ToTable("Courses");  // nome tabella
            entity.HasKey(course => course.Id); // pk

            // mapping per owned types (Money)
            // TODO

            // mapping per le relazioni, il corso vede N lezioni
            entity.HasMany(course => course.Lessons)
                .WithOne(lesson => lesson.Course)
                .HasForeignKey(lesson => lesson.CourseId);

            entity.Property(e => e.CurrentPriceAmount)
                .HasColumnType("NUMERIC")
                .HasColumnName("CurrentPrice_Amount");

            entity.Property(e => e.FullPriceAmount)
                .HasColumnType("NUMERIC")
                .HasColumnName("FullPrice_Amount");



            #region generato automaticamente
            /*
            entity.Property(e => e.Author).HasColumnType("Text (100)");
            entity.Property(e => e.CurrentPriceAmount)
                .HasColumnType("NUMERIC")
                .HasColumnName("CurrentPrice_Amount");
            entity.Property(e => e.CurrentPriceCurrency)
                .HasDefaultValue("EUR")
                .HasColumnType("TEXT(3)")
                .HasColumnName("CurrentPrice_Currency");
            entity.Property(e => e.Description).HasColumnType("Text (10000)");
            entity.Property(e => e.Email).HasColumnType("Text(100)");
            entity.Property(e => e.FullPriceAmount)
                .HasColumnType("NUMERIC")
                .HasColumnName("FullPrice_Amount");
            entity.Property(e => e.FullPriceCurrency)
                .HasDefaultValue("EUR")
                .HasColumnType("TEXT(3)")
                .HasColumnName("FullPrice_Currency");
            entity.Property(e => e.LogoPath).HasColumnType("Text (100)");
            entity.Property(e => e.Title).HasColumnType("Text (100)");
            */
            #endregion

        });


        modelBuilder.Entity<Lesson>(entity =>
        {

            // mapping per le relazioni, la lezione vede un solo corso
            //entity.HasOne(lesson => lesson.Course)
            //.WithMany(course => course.Lessons);


            #region generato automaticamente
            /*
            entity.Property(e => e.Description).HasColumnType("TEXT (10000)");
            entity.Property(e => e.Duration)
                .HasDefaultValueSql("'00:00:00'")
                .HasColumnType("TEXT (8)");
            entity.Property(e => e.Title).HasColumnType("TEXT (100)");

            entity.HasOne(d => d.Course).WithMany(p => p.Lessons).HasForeignKey(d => d.CourseId);
            #endregion
            */
            #endregion
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
