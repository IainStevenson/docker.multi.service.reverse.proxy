using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Logging
{
    public static class RequestLoggingMiddlewareExtension
    {
        public static IServiceCollection AddRequestResponseLoggingMiddlewareWithOptions(this IServiceCollection service, Action<RequestLoggingMiddlewareOptions> options = default)
        {
            options ??= (opts => { });

            service.Configure(options);
            return service;
        }


        public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestResponseLoggingMiddleware>();
        }
    }
}
