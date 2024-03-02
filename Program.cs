using mvc.Models.Services.Application;
using mvc.Models.Services.Infrastructure;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        //builder.Services.AddControllersWithViews();

        builder.Services.AddMvc(options => options.EnableEndpointRouting = false);
        // deve preparare alla gestione di oggetti di tipo CourseService,
        // net core deve costruirlo e passarlo
        //builder.Services.AddTransient<ICourseService, CourseService>();  // versione con valori auto generati da codice
        //builder.Services.AddTransient<ICourseService, AdoNetCourseService>(); // versione con valori letti da DB con adonet
        builder.Services.AddTransient<ICourseService, EfCoreCourseService>();  // versione con valori letti da entity framework

        builder.Services.AddDbContext<MyCourseDbContext>();   

        // Ogni volta che un componente ha una dipendenza da questa interfaccia,
        // net core initierra' un'istanza di SqliteDatabaseAccessor
        builder.Services.AddTransient<IDatabaseAccessor, SqliteDatabaseAccessor>();

        string testo = null;


        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();

            app.Lifetime.ApplicationStarted.Register(() =>
            {
                // Scrive file di testo ad ogni run
                string filePath = Path.Combine(app.Environment.ContentRootPath, "bin/reload.txt");
                File.WriteAllText(filePath, DateTime.Now.ToString());

            });

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

        app.Run();
    }
}
