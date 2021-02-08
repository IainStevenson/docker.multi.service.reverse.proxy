using System;

namespace Api
{
    /// <summary>
    /// Weather Forecast POCO model
    /// </summary>
    public class WeatherForecastModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTimeOffset Date { get; set; } = DateTimeOffset.UtcNow;
        public int TemperatureC { get; set; }
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
        public string Summary { get; set; }
    }
}
