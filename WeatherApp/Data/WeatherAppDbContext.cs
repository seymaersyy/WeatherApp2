using Microsoft.EntityFrameworkCore;
using WeatherApp.Models;

namespace WeatherApp.Data
{
    public class WeatherAppDbContext : DbContext
    {
        public WeatherAppDbContext(DbContextOptions<WeatherAppDbContext> options) : base(options) { }

        public DbSet<City> Cities { get; set; }
        public DbSet<HourlyWeather> HourlyWeathers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>()
                .HasMany(c => c.HourlyWeathers)
                .WithOne(hw => hw.City)
                .HasForeignKey(hw => hw.CityID);
        }
    }
}