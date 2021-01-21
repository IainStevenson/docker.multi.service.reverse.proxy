using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Routing
{
    public class RouteDebuggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _path;
        private readonly ILogger<RouteDebuggerMiddleware> _logger;

        public RouteDebuggerMiddleware(RequestDelegate next, ILogger<RouteDebuggerMiddleware> logger, string path)
        {
            _logger = logger;
            _next = next;
            _path = path;
        }

        public async Task Invoke(HttpContext context, IActionDescriptorCollectionProvider provider = null)
        {
            if (context.Request.Path == _path)
            {
                if (provider != null)
                {
                    var routes = provider.ActionDescriptors.Items.Select(x => new
                    {
                        Action = x.RouteValues.ContainsKey("Action") ? x.RouteValues["Action"] : null,
                        Controller = x.RouteValues.ContainsKey("Controller") ? x.RouteValues["Controller"] : null,
                        Page = x.RouteValues.ContainsKey("Page") ? x.RouteValues["Page"] : null,
                        x.AttributeRouteInfo?.Name,
                        x.AttributeRouteInfo?.Template,
                        Contraint = JsonConvert.SerializeObject(x.ActionConstraints)
                    }).ToArray();

                    var routesJson = JsonConvert.SerializeObject(routes, new JsonSerializerSettings() { Formatting = Formatting.Indented });

                    _logger.LogDebug(routesJson);
                }

            }

            await _next(context);
        }
    }
}
