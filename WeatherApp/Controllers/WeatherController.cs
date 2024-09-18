using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeatherApp.Services;

namespace WeatherApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly WeatherService _weatherService;

        public WeatherController(WeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpGet("FetchWeather")]
        public async Task<IActionResult> FetchWeatherData()
        {
            var weatherData = await _weatherService.GetWeatherDataAsync();
            await _weatherService.SaveWeatherDataToDatabaseAsync(weatherData);
            return Ok("Weather data fetched and saved successfully.");
        }
    }
}
