using my_new_app.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace my_new_app.Data
{
    public partial class WeatherDbContext : DbContext
    {
        public DbSet<WeatherForecast> weatherForecasts { get; set; }
        public DbSet<Notification> notifications { get; set; }

        public WeatherDbContext(DbContextOptions<WeatherDbContext> options): base(options){ }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog = WeatherForecast; Integrated Security= True;");
        }

    }
}
