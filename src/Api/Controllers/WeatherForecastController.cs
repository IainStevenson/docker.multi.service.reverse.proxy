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

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> Get(Guid id)
        {
            return Ok(_forecasts.FirstOrDefault(x=>x.Id == id));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] WeatherForecast model)
        {
            if (ModelState.IsValid)
            {
                _forecasts.Add(model);
                return CreatedAtRoute(nameof(Get), new { id = model.Id }, null);
            }
            else
            { 
                return BadRequest(model);
            }
        }
    }
}
