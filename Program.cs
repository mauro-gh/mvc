internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        //builder.Services.AddControllersWithViews();

        builder.Services.AddMvc(options => options.EnableEndpointRouting = false);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();

            app.Lifetime.ApplicationStarted.Register(() =>
            {
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