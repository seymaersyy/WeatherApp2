using System.Xml.Serialization;

namespace WeatherApp.Models
{
    [XmlRoot("Merkezler")]
    public class WeatherCenter
    {
        [XmlElement("ili")]
        public required string CityName { get; set; } // Şehir ismi

        [XmlElement("Tarih")]
        public required string Date { get; set; } // Tarih formatı: ddMMyyHHmm

        [XmlElement("tmp")]
        public required string Temperature { get; set; } // Sıcaklık

        [XmlElement("nem")]
        public required string Humidity { get; set; } // Nem oranı

        [XmlElement("Durum")]
        public required string WeatherCondition { get; set; } // Hava durumu
    }
}
