using Codenames.Data;
using Codenames.Hubs;
using Codenames.Model;
using Codenames.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<CodenamesDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<CodenamesDbContext>();

builder.Services.AddIdentityServer(options => options.IssuerUri = "https://localhost:44421/")
    .AddApiAuthorization<User, CodenamesDbContext>();


builder.Services.AddAuthentication()
    .AddIdentityServerJwt();






builder.Services.TryAddEnumerable(
    ServiceDescriptor.Singleton<IPostConfigureOptions<JwtBearerOptions>,
        ConfigureJwtBearerOptions>());




builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
//builder.Services.AddSignalR();
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});
builder.Services.AddSingleton<IUserIdProvider, IdBasedUserIdProvider>();


builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "MyAllowSpecificOrigins",
                      policy =>
                      {
                          policy.WithOrigins("https://localhost:44421").AllowAnyMethod().AllowAnyHeader().AllowCredentials(); ; // add the allowed origins
                      });
});


var app = builder.Build();
app.UseCors("MyAllowSpecificOrigins");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseIdentityServer();
app.UseAuthorization();

app.MapHub<ChatHub>("/chathub", options =>
{
    options.Transports =
        Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets;
});
app.MapHub<GameHub>("/gamehub", options =>
{
    options.Transports =
        Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets;
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");
app.MapRazorPages();

//app.MapHub<ChatHub>("/chathub");

app.MapFallbackToFile("index.html"); ;

app.Run();


