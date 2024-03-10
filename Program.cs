using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using mvc;
using mvc.Models.Options;
using mvc.Models.Services.Application;
using mvc.Models.Services.Infrastructure;

internal class Program
{
    private static void Main(string[] args)
    {



        var builder = WebApplication.CreateBuilder(args);

        Startup startup = new Startup(builder.Configuration);
        startup.test();


        // Add services to the container.
        //builder.Services.AddControllersWithViews();

        builder.Services.AddMvc(options => options.EnableEndpointRouting = false);
        // deve preparare alla gestione di oggetti di tipo CourseService,
        // net core deve costruirlo e passarlo
        //builder.Services.AddTransient<ICourseService, CourseService>();  // versione con valori auto generati da codice
        builder.Services.AddTransient<ICourseService, AdoNetCourseService>(); // versione con valori letti da DB con adonet
        //builder.Services.AddTransient<ICourseService, EfCoreCourseService>();  // versione con valori letti da entity framework

        //builder.Services.AddDbContext<MyCourseDbContext>(); 
        // lettura di una chiave
        //string connectionString = startup.Config.GetSection("ConnectionStrings").GetValue<string>("Default");
        string connectionString = startup.Config.GetConnectionString("Default");
        // sostituire AddDbContext con AddDbContextPool
        
        Action<DbContextOptionsBuilder> actionSqLite = (action) => action.UseSqlite(connectionString);
        builder.Services.AddDbContextPool<MyCourseDbContext>(actionSqLite);


        // Ogni volta che un componente ha una dipendenza da questa interfaccia,
        // net core initierra' un'istanza di SqliteDatabaseAccessor
        builder.Services.AddTransient<IDatabaseAccessor, SqliteDatabaseAccessor>();
        // per la cache
        builder.Services.AddTransient<ICachedCourseService, MemoryCacheCourseService>();

        // Options
        builder.Services.Configure<ConnectionStringsOptions>(startup.Config.GetSection("ConnectionStrings"));
        // da ora in poi, basta implementare interfaccia con IOptionsMonitor<CoursesOptions> e otteniamo tutta la sezione "Courses"
        builder.Services.Configure<CoursesOptions>(startup.Config.GetSection("Courses")); 




        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            //app.UseExceptionHandler("/Home/Error");
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();

            app.Lifetime.ApplicationStarted.Register(() =>
            {
                // Scrive file di testo ad ogni run
                string filePath = Path.Combine(app.Environment.ContentRootPath, "bin/reload.txt");
                File.WriteAllText(filePath, DateTime.Now.ToString());

            });

        } else{
             app.UseExceptionHandler("/Error");
            
        }

        app.UseStaticFiles();


        /*
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        */

        app.UseMvc(routeBuilder =>
        {
            // /courses/detail/5
            routeBuilder.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
        });


        /*
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        */

        // Func<int, int, bool> numeriuguali =
        //     (num1, num2) =>  {
        //         return num1 == num2;
        //         };

        // bool uguali  = numeriuguali(10, 15);
        // if (uguali)
        //     Console.WriteLine("uguali");
        // else
        //     Console.WriteLine("diversi");



        app.Run();
    }

    

}
