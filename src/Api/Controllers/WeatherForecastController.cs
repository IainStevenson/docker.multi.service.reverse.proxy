using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Response.Formater;
using Storage.MongoDb;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IResponseLinksProvider<ItemResponseModel<WeatherForecastModel>> _responseLinksProvider;
        private static Dictionary<string, string> EmptyEntityList = new Dictionary<string, string>() { };
        private readonly IMapper _mapper;
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IRepository<ItemStorageModel<WeatherForecastModel>> _forecasts;
        public WeatherForecastController(ILogger<WeatherForecastController> logger,
            IMapper mapper,
            IResponseLinksProvider<ItemResponseModel<WeatherForecastModel>> responseLinksProvider,
        IRepository<ItemStorageModel<WeatherForecastModel>> forecasts)
        {
            _logger = logger;
            _mapper = mapper;
            _responseLinksProvider = responseLinksProvider;
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
            var documents = await _forecasts.GetAsync(x => true);
            return Ok(documents);
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
            var document = await _forecasts.GetAsync(id);
            return Ok(document);
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
        public async Task<IActionResult> Post([FromBody] ItemResponseModel<WeatherForecastModel> model)
        {
            _logger.LogInformation($"Adding a forecast ");
            if (ModelState.IsValid)
            {

                var storageItem = _mapper.Map<ItemResponseModel<WeatherForecastModel>, ItemStorageModel<WeatherForecastModel>>(model);

                var itemStored = await _forecasts.CreateAsync(storageItem);

                var systemKeys = new Dictionary<string, string>() { { "{id}", $"{itemStored.Id}" } };

                var relatedEntities = EmptyEntityList;

                var itemResponse = _mapper.Map<ItemStorageModel<WeatherForecastModel>, ItemResponseModel<WeatherForecastModel>>(itemStored);

                itemResponse = await _responseLinksProvider.AddLinks(itemResponse,
                                                            Request.Scheme,
                                                            Request.Host.Value,
                                                            Request.Path.Value.TrimEnd('/'),
                                                            systemKeys,
                                                            relatedEntities);

                _logger.LogInformation($"Added a forecast with id {itemResponse.Id} and Etag {itemResponse.Etag }");
                return CreatedAtRoute("GetById", new { id = itemResponse.Id }, itemResponse);
            }
            else
            {
                _logger.LogInformation($"Failed to add a forecast");
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
        public async Task<IActionResult> Put(Guid id, [FromBody] ItemResponseModel<WeatherForecastModel> model)
        {
            _logger.LogInformation($"Updating a forecast by id {id}");
            if (ModelState.IsValid)
            {
                var item = (await _forecasts.GetAsync(x => x.Id == id && x.Etag == model.Etag)).FirstOrDefault();
                if (item != null)
                {
                    item.Item.Date = model.Item.Date;
                    item.Item.TemperatureC = model.Item.TemperatureC;
                    item.Item.Summary = model.Item.Summary;
                    await _forecasts.UpdateAsync(item);
                    _logger.LogInformation($"Updated a forecast by id {id}");
                    //TODO: Return ETAG as header also add to test
                    return NoContent(); 
                }
                item = (await _forecasts.GetAsync(x => x.Id == id)).FirstOrDefault();
                if (item != null)
                {
                    _logger.LogError($"Failed to update a forecast by id {id} as item has changed.");
                    ModelState.AddModelError("Item Changed", "The item has changed. Latest item provided.");
                    return BadRequest(item);
                }
                else
                {
                    _logger.LogError($"Failed to update a forecast by id {id} as item not found.");
                    ModelState.AddModelError("Not Found", "The item was not found. It may have been deleted. please redresh your view.");
                    return BadRequest(ModelState);
                }
            }
            else
            {
                _logger.LogError($"Failed to update a forecast by id {id}");
                return BadRequest(ModelState);
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

                await _forecasts.DeleteAsync(id);
                _logger.LogInformation($"Deleted a forecast by id {id}");
                return NoContent();

            }
            else
            {
                _logger.LogError($"Failed to delete a forecast by id {id}");
                return BadRequest();
            }
        }
    }
};