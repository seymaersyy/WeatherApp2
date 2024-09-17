using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeatherApp.Data;
using WeatherApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeatherApp.Services;


namespace WeatherApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HourlyWeatherController : ControllerBase
    {
        private readonly WeatherAppDbContext _context;

        public HourlyWeatherController(WeatherAppDbContext context)
        {
            _context = context;
        }

        private readonly WeatherService _weatherService;

        public HourlyWeatherController(WeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        // API'den hava durumu verilerini çekip veritabanına kaydet
        [HttpGet("fetch-weather-data")]
        public async Task<IActionResult> FetchWeatherData()
        {
            try
            {
                var weatherData = await _weatherService.GetWeatherDataAsync(); // API'den veri çek
                await _weatherService.SaveWeatherDataToDatabaseAsync(weatherData); // Veritabanına kaydet

                return Ok("Weather data fetched and saved successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        // GET: api/HourlyWeather
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HourlyWeather>>> GetHourlyWeathers()
        {
            return await _context.HourlyWeathers
                                 .Include(hw => hw.City) // Şehir ilişkisini dahil ediyoruz
                                 .ToListAsync();
        }

        // GET: api/HourlyWeather/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HourlyWeather>> GetHourlyWeather(int id)
        {
            var hourlyWeather = await _context.HourlyWeathers
                                              .Include(hw => hw.City) // Şehir ilişkisini dahil ediyoruz
                                              .FirstOrDefaultAsync(hw => hw.Id == id);

            if (hourlyWeather == null)
            {
                return NotFound();
            }

            return hourlyWeather;
        }

        // GET: api/HourlyWeather/Filter?cityName=Istanbul&startDate=2024-01-01&endDate=2024-01-10
        [HttpGet("Filter")]
        public async Task<ActionResult<IEnumerable<HourlyWeather>>> GetFilteredHourlyWeather(
            string cityName, DateTime startDate, DateTime endDate)
        {
            // Şehri ve tarih aralığını kontrol edin
            if (string.IsNullOrEmpty(cityName) || startDate == default || endDate == default)
            {
                return BadRequest("City name, start date, and end date are required.");
            }

            // Veritabanında sorgu oluştur
            var hourlyWeathers = await _context.HourlyWeathers
                .Include(hw => hw.City)  // Şehri dahil et
                .Where(hw => hw.City.CityName == cityName &&
                             hw.Date >= startDate && hw.Date <= endDate)
                .ToListAsync();

            if (hourlyWeathers == null || hourlyWeathers.Count == 0)
            {
                return NotFound("No hourly weather data found for the given city and date range.");
            }

            return hourlyWeathers;
        }


        // POST: api/HourlyWeather
        [HttpPost]
        public async Task<ActionResult<HourlyWeather>> PostHourlyWeather(HourlyWeather hourlyWeather)
        {
            // Şehir ID'si geçerli mi kontrol ediyoruz
            var city = await _context.Cities.FindAsync(hourlyWeather.CityID);
            if (city == null)
            {
                return BadRequest("CityID is invalid.");
            }

            _context.HourlyWeathers.Add(hourlyWeather);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetHourlyWeather), new { id = hourlyWeather.Id }, hourlyWeather);
        }

        // PUT: api/HourlyWeather/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHourlyWeather(int id, HourlyWeather hourlyWeather)
        {
            if (id != hourlyWeather.Id)
            {
                return BadRequest();
            }

            // Şehir ID'si geçerli mi kontrol ediyoruz
            var city = await _context.Cities.FindAsync(hourlyWeather.CityID);
            if (city == null)
            {
                return BadRequest("CityID is invalid.");
            }

            _context.Entry(hourlyWeather).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HourlyWeatherExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/HourlyWeather/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHourlyWeather(int id)
        {
            var hourlyWeather = await _context.HourlyWeathers.FindAsync(id);
            if (hourlyWeather == null)
            {
                return NotFound();
            }

            _context.HourlyWeathers.Remove(hourlyWeather);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Helper Method: Check if HourlyWeather Exists
        private bool HourlyWeatherExists(int id)
        {
            return _context.HourlyWeathers.Any(e => e.Id == id);
        }
    }
}

