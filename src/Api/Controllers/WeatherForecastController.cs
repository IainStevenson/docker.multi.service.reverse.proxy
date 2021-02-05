using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Storage;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IRepository<WeatherForecast> _forecasts;
        public WeatherForecastController(ILogger<WeatherForecastController> logger, IRepository<WeatherForecast> forecasts)
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
            _logger.LogInformation("Retrieving all forecasts");
            return Ok(await _forecasts.Retrieve(x=> true));
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
            _logger.LogInformation($"Retrieving a forecast by id {id}");
            return Ok(await _forecasts.Retrieve( id));
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
            _logger.LogInformation($"Adding a forecast by id {model.Id}");
            if (ModelState.IsValid)
            {
                var item = await _forecasts.Store(model);
                _logger.LogInformation($"Added a forecast by id {item.Id}");
                return CreatedAtRoute("GetById", new { id = item.Id }, item);
            }
            else
            {
                _logger.LogInformation($"Failed to add a forecast by id {model.Id}");
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
            _logger.LogInformation($"Updating a forecast by id {id}");
            if (ModelState.IsValid)
            {
                var item = await _forecasts.Retrieve(id);
                if (item != null)
                {
                    item.Date = model.Date;
                    item.TemperatureC = model.TemperatureC;
                    item.Summary = model.Summary;
                    await _forecasts.Store(item);
                    _logger.LogInformation($"Updated a forecast by id {id}");
                    return Ok(item);
                }
                _logger.LogError($"Failed to update a forecast by id {id}");
                return BadRequest(ModelState);
            }
            else
            {
                _logger.LogError($"Failed to update a forecast by id {id}");
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
            _logger.LogInformation($"Deleting a forecast by id {id}");
            if (Guid.Empty != id)
            {
                var item = await _forecasts.Retrieve(id);
                if (item != null)
                {
                    await _forecasts.Discard(item);
                    _logger.LogInformation($"Deleted a forecast by id {id}");
                    return NoContent();
                }
                _logger.LogError($"Failed to delete a forecast by id {id}");
                return BadRequest(ModelState);
            }
            else
            {
                _logger.LogError($"Failed to delete a forecast by id {id}");
                return BadRequest();
            }
        }
    }
};