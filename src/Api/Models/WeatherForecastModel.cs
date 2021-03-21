using System;

namespace Api.Models
{
    public class WeatherForecastModel
    {
        public DateTime Date { get; set; }
        public int TemperatureC { get; set; }
        public string Summary { get; set; }
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}
