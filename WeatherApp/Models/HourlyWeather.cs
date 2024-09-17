using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace WeatherApp.Models
{
    [Table("HourlyWeather")]
    public class HourlyWeather
    {
        public int Id { get; set; } // Primary key (ID)
        public int CityID { get; set; } // Foreign key referencing City

        [XmlElement("Tarih")]
        public DateTime Date { get; set; } // XML'deki Tarih elemanıyla eşleşir

        [XmlElement("tmp")]
        public float Temperature { get; set; } // XML'deki tmp elemanıyla eşleşir

        [XmlElement("nem")]
        public int Humidity { get; set; } // XML'deki nem elemanıyla eşleşir


        [XmlElement("Durum")]
        public required string WeatherCondition { get; set; } // XML'deki Durum elemanıyla eşleşir


        [JsonIgnore]
        public City? City { get; set; } // Relationship with City
    }
}

