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
        [Route("{id:guid}", Name = "GetById", Order = 1)]
        public async Task<IActionResult> Get(Guid id)
        {
            return Ok(_forecasts.FirstOrDefault(x => x.Id == id));
        }


        /// <summary>
        /// Add one. 
        /// </summary>
        /// <remarks>The client does not need to specify an Id as one will be assigned and reported via the Location header</remarks>
        /// <example>
        /// Example body:
        /// {
        ///    "temperatureC": 37,
        ///    "summary": "Balmy"
        /// }
        /// </example>
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


        /// <summary>
        /// Change one
        /// </summary>
        /// <param name="id">The id of the item to update</param>
        /// <param name="model">The data as Json body content</param>
        /// <returns>204 NoContent, otherwise 400 BadRequest</returns>
        [HttpPut]
        [Route("{id:guid}", Name = "PutById", Order = 2)]
        public async Task<IActionResult> Put(Guid id, [FromBody] WeatherForecast model)
        {
            if (ModelState.IsValid)
            {
                var item = _forecasts.FirstOrDefault(x => x.Id == id);
                if (item != null)
                {
                    item.Id = id;
                    item.Date = model.Date;
                    item.TemperatureC = model.TemperatureC;
                    item.Summary = model.Summary;
                    return NoContent();
                }

                return BadRequest(ModelState);
            }
            else
            {
                return BadRequest(model);
            }
        }
        /// <summary>
        /// Delete one
        /// </summary>
        /// <param name="id">The id of the item to delete</param>
        /// <returns>204 NoContent</returns>
        [HttpDelete]
        [Route("{id:guid}", Name = "DeleteById", Order = 3)]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (Guid.Empty != id)
            {
                var item = _forecasts.FirstOrDefault(x => x.Id == id);
                if (item != null)
                {
                    _forecasts.Remove(item);
                    return NoContent();
                }
                return BadRequest(ModelState);
            }
            else
            {
                return BadRequest();
            }
        }
    }
};