using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebApp2.Controllers
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
            var apiClient = new HttpClient();
            var tokenSB = new StringBuilder(HttpContext.Request.Headers["access-token"]);
            tokenSB.Remove(0, "bearer ".Length);
            apiClient.SetBearerToken(tokenSB.ToString());
            
            var apiUri = new Uri("https://api.mystore.local/weatherforecast"); // TODO: Add to configuration
            
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
