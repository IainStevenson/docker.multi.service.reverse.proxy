using Storage;
using System;

namespace Api
{
    public class WeatherForecast : IStorageItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? Modified { get; set; }
        public string ETag { get; set; } = Guid.NewGuid().ToString();


        public DateTime Date { get; set; }
        public int TemperatureC { get; set; }
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
        public string Summary { get; set; }

    }
}
