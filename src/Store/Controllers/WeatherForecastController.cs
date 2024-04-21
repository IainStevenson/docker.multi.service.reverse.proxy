using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Store.Controllers
{
    public class WeatherForecastController : Controller
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly Configuration.ApiOptions _options;
        private readonly IHttpClientFactory _httpClientFactory;

        public WeatherForecastController(ILogger<WeatherForecastController> logger,
             IHttpClientFactory httpClientFactory,
             IOptions<Configuration.ApiOptions> options)
        {
            _logger = logger;
            _options = options.Value;
            _httpClientFactory = httpClientFactory;
        }


        public async Task<IActionResult> Index()
        {

            var response = await GetApiResources("weatherforecast");
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"API Call incorrect response {response.StatusCode}");
                ViewBag.JsonData = new { StatusCode = response.StatusCode };
            }
            else
            {
                ViewBag.JsonData = await response.Content.ReadAsStringAsync();
            }
            return View();
        }

        private async Task<HttpResponseMessage> GetApiResources(string uri)
        {
            using (var apiClient = _httpClientFactory.CreateClient())
            {
                string accessToken = await HttpContext.GetTokenAsync("access_token");
                apiClient.SetBearerToken(accessToken);
                var apiUri = new Uri($"{_options.BaseUri}/{uri}");
                return await apiClient.GetAsync(apiUri);
            }
        }
    }
}
