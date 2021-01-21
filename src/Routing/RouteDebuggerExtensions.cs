using Microsoft.AspNetCore.Builder;

namespace Routing
{
    public static class RouteDebuggerExtensions
    {
        public static IApplicationBuilder UseRouteDebugger(this IApplicationBuilder app, string path = "/route-debugger")
        {
            return app.UseMiddleware<RouteDebuggerMiddleware>(path);
        }
    }
}
