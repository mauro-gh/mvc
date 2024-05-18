using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using mvc;
using mvc.Models.Options;
using mvc.Models.Services.Application;
using mvc.Models.Services.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using mvc.Customizations.Identity;
internal class Program
{


    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        Startup startup = new Startup(builder.Configuration);
        startup.test();

        // Add services to the container.
        //builder.Services.AddControllersWithViews();

        //builder.Services.AddMvc(options => options.EnableEndpointRouting = false);

        // Razor Pages, per mostrare le UI di authentication
        builder.Services.AddRazorPages();


        builder.Services.AddMvc(options =>
        {
            options.EnableEndpointRouting = false;
            // leggo opzioni cache da appsetting
            var homeProfile = new CacheProfile();
            homeProfile.Duration = startup.Config.GetValue<int>("ResponseCache:Home:Duration");
            homeProfile.Location = startup.Config.GetValue<ResponseCacheLocation>("ResponseCache:Home:Location");
            // aggiungo profilo
            options.CacheProfiles.Add("Home", homeProfile);
        });

        // deve preparare alla gestione di oggetti di tipo CourseService,
        // net core deve costruirlo e passarlo
        //builder.Services.AddTransient<ICourseService, CourseService>();  // versione con valori auto generati da codice
        //builder.Services.AddTransient<ICourseService, AdoNetCourseService>(); // versione con valori letti da DB con adonet
        builder.Services.AddTransient<ICourseService, EfCoreCourseService>();  // versione con valori letti da entity framework

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
        //builder.Services.AddTransient<IDatabaseAccessor, SqlServerDatabaseAccessor>();
        //builder.Services.AddTransient<IDatabaseAccessor, OracleDatabaseAccessor>();
        
        // per la cache
        builder.Services.AddTransient<ICachedCourseService, MemoryCacheCourseService>();
        // per persistre immagini
        builder.Services.AddSingleton<IImagePersister, MagickNetImagePersister>();
        

        // Options (verra' iniettato dalla DI tramite IOptionsMonitor<ConnectionStringsOptions> connectionStringsOptions))
        builder.Services.Configure<ConnectionStringsOptions>(startup.Config.GetSection("ConnectionStrings"));
        // da ora in poi, basta implementare interfaccia con IOptionsMonitor<CoursesOptions> e otteniamo tutta la sezione "Courses"
        builder.Services.Configure<CoursesOptions>(startup.Config.GetSection("Courses")); 
        // Cache:
        builder.Services.Configure<MemoryCacheOptions>(startup.Config.GetSection("MemoryCache"));

        // Registrazione Identity, con criteri di complessita' della password
        builder.Services.AddDefaultIdentity<IdentityUser>(options =>{
                  options.Password.RequireDigit = true;
                  options.Password.RequiredLength = 6;
                  options.Password.RequireNonAlphanumeric = false;
                  options.Password.RequireLowercase = false;
                  options.Password.RequireUppercase = false;
                  //options.Password.RequiredUniqueChars = 2;
                  })
            .AddPasswordValidator<CommonPasswordValidator<IdentityUser>>()
            .AddEntityFrameworkStores<MyCourseDbContext>();
        

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            //app.UseExceptionHandler("/Home/Error");
            //app.UseExceptionHandler("/Error");
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

        // 1. contenuti statici middleware per jpg, css, ecc...
        app.UseStaticFiles();

        // 2. endpoint routing middleware (\courses, selezionera' l'action corretto per la richiesta)
        app.UseRouting();

        // Middleware Identity qui in mezzo
        app.UseAuthentication();
        // da ora in poi la richiesta e' autenticata
        app.UseAuthorization();
        // se l'utente non ha il privilegio, torna indietro


        // 3. endpoint middleware (esegue il controller corretto)
        app.UseEndpoints(routeBuilder =>{
            routeBuilder.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            routeBuilder.MapRazorPages();
        });


        /*
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        */

        /*
        app.UseMvc(routeBuilder =>
        {
            // /courses/detail/5
            routeBuilder.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
        });
        */


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
