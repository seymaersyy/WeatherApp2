using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using WeatherApp.Models;  // Modelleri dahil et
using WeatherApp.Data;    // DbContext dahil et
using Microsoft.EntityFrameworkCore;  // DbContext işlemleri için gerekli

namespace WeatherApp.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly WeatherAppDbContext _dbContext;

        // Constructor: HttpClient ve DbContext'i enjekte ediyoruz
        public WeatherService(HttpClient httpClient, WeatherAppDbContext dbContext)
        {
            _httpClient = httpClient;
            _dbContext = dbContext;
        }

        // API'den veri çekme metodu
        public async Task<List<HourlyWeather>> GetWeatherDataAsync()
        {
            // MGM API'den veri çekme isteği
            var response = await _httpClient.GetAsync("https://www.mgm.gov.tr/FTPDATA/analiz/SonDurumlarTumu.xml");

            response.EnsureSuccessStatusCode(); // Başarı durumunu kontrol et

            // XML formatındaki veriyi string olarak al
            var xmlData = await response.Content.ReadAsStringAsync();

            // XML verisini C# nesnesine dönüştür
            var serializer = new XmlSerializer(typeof(List<HourlyWeather>));
            using var stringReader = new StringReader(xmlData);
            var weatherData = (List<HourlyWeather>)serializer.Deserialize(stringReader);

            return weatherData; // Veriyi döndür
        }

        // Veritabanına veri kaydetme metodu
        public async Task SaveWeatherDataToDatabaseAsync(List<HourlyWeather> weatherData)
        {
            foreach (var hourlyWeather in weatherData)
            {
                // Veritabanında aynı şehre ve saate ait veri var mı kontrol et
                var existingWeather = await _dbContext.HourlyWeathers
                    .FirstOrDefaultAsync(hw => hw.CityID == hourlyWeather.CityID && hw.Date == hourlyWeather.Date);

                // Eğer veritabanında yoksa ekle
                if (existingWeather == null)
                {
                    _dbContext.HourlyWeathers.Add(hourlyWeather);
                }
            }

            await _dbContext.SaveChangesAsync(); // Veritabanına kaydet
        }
    }
}

