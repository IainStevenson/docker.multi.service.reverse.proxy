using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Store.Controllers
{
    public class WeatherForecastController : Controller
    {
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var apiClient = new HttpClient(); //TODO: Inject HttpClientFactory and create here
            
            string accessToken = await HttpContext.GetTokenAsync("access_token");
            //string refreshToken = await HttpContext.GetTokenAsync("refresh_token");

            apiClient.SetBearerToken(accessToken);
            
            var apiUri = new Uri("https://api.mystore.local/api/resources/weatherforecast"); //TODO: CONFIG

            var response = await apiClient.GetAsync(apiUri);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"API Call incorrect response {response.StatusCode}");
            }
            else
            {
                ViewBag.JsonData = await response.Content.ReadAsStringAsync();
            }

            return View();
        }
    }
}
