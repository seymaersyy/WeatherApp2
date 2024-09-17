using System.Text.Json.Serialization;

namespace WeatherApp.Models
{
    public class City
    {
        public int CityID { get; set; } // Primary key (ID)
        public required string CityName { get; set; } // City name

        // City can have multiple HourlyWeather entries (1-to-many relationship)
        public  ICollection<HourlyWeather>? HourlyWeathers { get; set; } = new List<HourlyWeather>();
    }
}
