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
        /// <summary>
        ///  Get them all
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(_forecasts);
        }

        /// <summary>
        /// Get just one, if that!
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id:guid}", Name = "GetById", Order =1)]
        public async Task<IActionResult> Get(Guid id)
        {
            return Ok(_forecasts.FirstOrDefault(x=>x.Id == id));
        }


        /// <summary>
        /// Add one
        /// </summary>
        /// <param name="model">The data as Json body content</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] WeatherForecast model)
        {
            if (ModelState.IsValid)
            {
                _forecasts.Add(model);
                return CreatedAtRoute("GetById", new { id = model.Id }, null);
            }
            else
            { 
                return BadRequest(model);
            }
        }
    }
}
