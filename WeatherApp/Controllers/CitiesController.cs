using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeatherApp.Data;
using WeatherApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeatherApp.Wrapper;

namespace WeatherApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private readonly WeatherAppDbContext _context;
        private readonly ILogger<CitiesController> _logger;
        //burda logger ekliyorum ve bir tanesine örnek olarak koyacam geri kalanlarınada buna bakarak eklersin yaada ekli olanları ayarlarsın
        public CitiesController(WeatherAppDbContext context, ILogger<CitiesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Cities
        [HttpGet]
        public async Task<ActionResult<IEnumerable<City>>> GetCities()
        {
            return await _context.Cities.ToListAsync();
        }

        // GET: api/Cities/5
        [HttpGet("{id}")]
        public async Task<ActionResult<City>> GetCity(int id)
        {
            try
            {
                var city = await _context.Cities.FindAsync(id);

                if (city == null)
                {
                    return NotFound();
                }

                return city;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CitiesController->GetCity"); //loglardken strşngi bu şekilde yazma nedenim hatayı aldığın metodu loglarda bulup daha rahat inceleyebilmen. Bunun gibi diğerkinleride yapabilirsin
                return NotFound();
            }
            
        }

        // POST: api/Cities
        [HttpPost]
        public async Task<ActionResult<ResultModel>> PostCity(City city) //Bu şekilde bir result model yapısı entegre edebilirsin bütün dönüşlere
        {
            try
            {
                _context.Cities.Add(city);
                await _context.SaveChangesAsync();

                return new ResultModel() { IsSuccess = false, Message = "istek başarılı" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CitiesController->PostCity");
                return new ResultModel() { IsSuccess = false , Message = ex.Message };
            }
            
        }

        // PUT: api/Cities/5
        [HttpPut("{id}")]

        public async Task<IActionResult> PutCity(int id, City city)
        {
            // Şehir ID'si modelden alındığı için parametredeki id ile eşleşmelidir
            if (id != city.CityID)
            {
                return BadRequest("ID uyuşmazlığı");
            }

            // Veritabanındaki şehir kaydını getiriyoruz
            var existingCity = await _context.Cities.Include(c => c.HourlyWeathers)
                                                    .FirstOrDefaultAsync(c => c.CityID == id);

            if (existingCity == null)
            {
                return NotFound("Şehir bulunamadı.");
            }

            // Şehir adını güncelliyoruz
            existingCity.CityName = city.CityName;

            // Saatlik hava durumu verilerini güncelliyoruz
            if (city.HourlyWeathers != null && city.HourlyWeathers.Any())
            {
                // Mevcut saatlik hava durumu verilerini temizliyoruz
                existingCity.HourlyWeathers.Clear();

                // Yeni saatlik hava durumu verilerini ekliyoruz
                foreach (var hourlyWeather in city.HourlyWeathers)
                {
                    existingCity.HourlyWeathers.Add(hourlyWeather);
                }
            }

            // Değişiklikleri veritabanına kaydediyoruz
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CitiesController->PutCity");
                return StatusCode(500, "Bir hata oluştu: " + ex.Message);
            }

            return NoContent(); // Başarılı olursa 204 NoContent döndürür
        }


        // DELETE: api/Cities/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCity(int id)
        {
            var city = await _context.Cities.FindAsync(id);
            if (city == null)
            {
                return NotFound();
            }

            _context.Cities.Remove(city);//TODO: şehirleri komle silmek isdeleted kolonu açarak onu true yap bilgileri çekerken isdeleted olmayanları çek.
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Helper Method: Check if City Exists
        private bool CityExists(int id)
        {
            return _context.Cities.Any(e => e.CityID == id);
        }
    }
}