using Microsoft.EntityFrameworkCore;
using WeatherApp.Data;  // Model ve DbContext dosyas�n�n namespace'i
using Serilog;
using WeatherApp.Services;



var builder = WebApplication.CreateBuilder(args);

// Serilog yap�land�rmas�
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()    // Loglar� konsola yaz
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)  // Loglar� dosyaya yaz
    .CreateLogger();

// Serilog'u kullanmak i�in logging yap�land�rmas�n� g�ncelle
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddHttpClient(); // Servisi ekledim.


// Swagger ve OpenAPI ayarlar�
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Veritaban� ba�lant�s� ve DbContext ayarlar�
builder.Services.AddDbContext<WeatherAppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

try
{
    Log.Information("Starting up the application");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "The application failed to start correctly");
    throw;
}
finally
{
    Log.CloseAndFlush();  // Loglama i�lemini tamamla
}
