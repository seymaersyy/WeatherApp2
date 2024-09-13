using Microsoft.EntityFrameworkCore;
using WeatherApp.Models;

namespace WeatherApp.Data
{
    public class WeatherAppDbContext : DbContext
    {
        public WeatherAppDbContext(DbContextOptions<WeatherAppDbContext> options)
            : base(options)
        {
        }

        // DbSet tanımları, veri tabanındaki tablolar ile eşleşir
        public DbSet<City> Cities { get; set; }
        public DbSet<HourlyWeather> HourlyWeathers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Tablo isimlerinin veri tabanında belirtilen adlara göre ayarlanması
            modelBuilder.Entity<City>()
                .ToTable("Cities");

            modelBuilder.Entity<HourlyWeather>()
                .ToTable("HourlyWeather");

            // İlişkilerin tanımlanması
            modelBuilder.Entity<City>()
                .HasMany(c => c.HourlyWeathers)
                .WithOne(hw => hw.City)
                .HasForeignKey(hw => hw.CityID);
                

            base.OnModelCreating(modelBuilder);
        }
    }
}
