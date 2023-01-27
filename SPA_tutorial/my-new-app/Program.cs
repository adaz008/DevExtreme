using Microsoft.EntityFrameworkCore;
using my_new_app.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<WeatherDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllersWithViews().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");;


app.UseSpa(spa =>
{
  // To learn more about options for serving an Angular SPA from ASP.NET Core,
  // see https://go.microsoft.com/fwlink/?linkid=864501

  spa.Options.SourcePath = "ClientApp";

  if (app.Environment.IsDevelopment())
  {
    //spa.Options.StartupTimeout = new TimeSpan(0, 0, 120);
    //spa.UseAngularCliServer(npmScript: "start");
    spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
  }
});

app.Run();
