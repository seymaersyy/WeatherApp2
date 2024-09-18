using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using WeatherApp.Models;  // Modelleri dahil et
using WeatherApp.Data;    // DbContext dahil et
using Microsoft.EntityFrameworkCore;  // DbContext işlemleri için gerekli
using System.Globalization;

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

        // MGM API'den veri çekme metodu
        public async Task<List<HourlyWeather>> GetWeatherDataAsync()
        {
            var response = await _httpClient.GetAsync("https://www.mgm.gov.tr/FTPDATA/analiz/SonDurumlarTumu.xml");
            response.EnsureSuccessStatusCode(); // İsteğin başarılı olup olmadığını kontrol et

            var xmlData = await response.Content.ReadAsStringAsync(); // XML verisini string olarak al

            // XML'i deserializasyon yap (WeatherCenter modelini kullanacağız)
            var serializer = new XmlSerializer(typeof(List<WeatherCenter>));
            using var stringReader = new StringReader(xmlData);
            var weatherData = (List<WeatherCenter>)serializer.Deserialize(stringReader);

            return ConvertToHourlyWeather(weatherData); // WeatherCenter'ı HourlyWeather'a dönüştür
        }

        // XML verisinden HourlyWeather modeline dönüşüm
        private List<HourlyWeather> ConvertToHourlyWeather(List<WeatherCenter> weatherCenters)
        {
            List<HourlyWeather> hourlyWeathers = new List<HourlyWeather>();

            foreach (var w in weatherCenters)
            {
                int cityId = GetCityIdFromName(w.CityName).Result; // Şehir adını veritabanında bul

                hourlyWeathers.Add(new HourlyWeather
                {
                    CityID = cityId,
                    Date = DateTime.ParseExact(w.Date, "ddMMyyHHmm", CultureInfo.InvariantCulture),
                    Temperature = float.Parse(w.Temperature),
                    Humidity = int.Parse(w.Humidity),
                    WeatherCondition = w.WeatherCondition ?? "Unknown"
                });
            }

            return hourlyWeathers;
        }

        // Şehir adından CityID'yi bulma ve ekleme
        public async Task<int> GetCityIdFromName(string cityName)
        {
            var city = await _dbContext.Cities.FirstOrDefaultAsync(c => c.CityName == cityName);

            if (city == null) // Eğer şehir veritabanında yoksa ekleyelim
            {
                city = new City { CityName = cityName };
                _dbContext.Cities.Add(city);
                await _dbContext.SaveChangesAsync();
            }

            return city.CityID;
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

