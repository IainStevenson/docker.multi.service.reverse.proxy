using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
       
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly List<WeatherForecast> _forecasts;
        public WeatherForecastController(ILogger<WeatherForecastController> logger, List<WeatherForecast> forecasts)
        {
            _logger = logger;
            _forecasts = forecasts;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(_forecasts);
        }
    }
}
