using Microsoft.EntityFrameworkCore;
using WeatherApp.Data;  // Model ve DbContext dosyasýnýn namespace'i
using Serilog;
using WeatherApp.Services;



var builder = WebApplication.CreateBuilder(args);

// Serilog yapýlandýrmasý
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()    // Loglarý konsola yaz
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)  // Loglarý dosyaya yaz
    .CreateLogger();

// Serilog'u kullanmak için logging yapýlandýrmasýný güncelle
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddHttpClient(); // Servisi ekledim.


// Swagger ve OpenAPI ayarlarý
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Veritabaný baðlantýsý ve DbContext ayarlarý
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
    Log.CloseAndFlush();  // Loglama iþlemini tamamla
}
