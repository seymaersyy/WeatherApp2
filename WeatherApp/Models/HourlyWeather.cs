using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WeatherApp.Models
{
    [Table("HourlyWeather")]
    public class HourlyWeather
    {
        public int Id { get; set; } // Primary key (ID)
        public int CityID { get; set; } // Foreign key referencing City
        public DateTime Date { get; set; } // Date and time of the weather data
        public float Temperature { get; set; } // Temperature in degrees
        public int Humidity { get; set; } // Humidity percentage
        public  required string WeatherCondition { get; set; } // Weather description (e.g., Sunny, Rainy)

        // Navigation property to represent the relationship with City
        [JsonIgnore]
        public City? City { get; set; }
    }
}
