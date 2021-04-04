using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Support.Controllers
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
            using (var apiClient = _httpClientFactory.CreateClient())
            {
                string accessToken = await HttpContext.GetTokenAsync("access_token");
                //string refreshToken = await HttpContext.GetTokenAsync("refresh_token");
                apiClient.SetBearerToken(accessToken);
                var apiUri = new Uri($"{_options.BaseUri}/weatherforecast");
                var response = await apiClient.GetAsync(apiUri);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"API Call incorrect response {response.StatusCode}");
                }
                else
                {
                    ViewBag.JsonData = await response.Content.ReadAsStringAsync();
                }
            }

            return View();
        }
    }
}
