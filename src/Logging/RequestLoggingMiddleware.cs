using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IO;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Logging
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;
        private const int ReadChunkBufferLength = 4096;
        private readonly RequestLoggingMiddlewareOptions _options;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger, IOptions<RequestLoggingMiddlewareOptions> options)
        {
            _next = next;
            _logger = logger;
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
            _options = options.Value;

        }

        public async Task Invoke(HttpContext context, IActionDescriptorCollectionProvider provider = null)
        {
            string sessionId = null;
            try
            {
                sessionId = context?.Session?.Id ?? null;
            }
            catch { } // sessions may or may not be enabled , if not it blows up
          


            var requestProfilerModel = new RequestProfilerModel
            {
                Source = _options.LogSource,
                SessionId = sessionId,
                TraceIdentifier = context?.TraceIdentifier ?? null,
                TimeOfRequest = DateTimeOffset.UtcNow,
                Request = await FormatRequestAsync(context)
               

            };

            var originalBody = context.Response.Body;

            using var newResponseBody = _recyclableMemoryStreamManager.GetStream();
            try
            {
                context.Response.Body = newResponseBody;
                await _next(context);



            }
            catch (Exception ex)
            {
                requestProfilerModel.Exception = ex;

            }
            finally
            {
                if (requestProfilerModel.Exception == null)
                {
                    newResponseBody.Seek(0, SeekOrigin.Begin);
                    await newResponseBody.CopyToAsync(originalBody);
                    newResponseBody.Seek(0, SeekOrigin.Begin);
                    requestProfilerModel.Response = await FormatResponseAsync(context, newResponseBody);

                }
                else
                {
                    // there was an excption so populate the more detailed information
                    object routes = null;
                    if (provider != null)
                    {
                        routes = provider.ActionDescriptors.Items.Select(x => new
                        {
                            Action = x.RouteValues.ContainsKey("Action") ? x.RouteValues["Action"] : null,
                            Controller = x.RouteValues.ContainsKey("Controller") ? x.RouteValues["Controller"] : null,
                            Page = x.RouteValues.ContainsKey("Page") ? x.RouteValues["Page"] : null,
                            x.AttributeRouteInfo?.Name,
                            x.AttributeRouteInfo?.Template,
                            Contraint = JsonConvert.SerializeObject(x.ActionConstraints)
                        }).ToArray();

                        //routesJson = JsonConvert.SerializeObject(routes, new JsonSerializerSettings() { Formatting = Formatting.Indented });
                    }
                    requestProfilerModel.Routes = routes ?? new object[] { };

                }
                requestProfilerModel.TimeOfResponse = DateTimeOffset.UtcNow;

                _logger.LogInformation(JsonConvert.SerializeObject(requestProfilerModel, new JsonSerializerSettings()
                {
                    Formatting = Formatting.Indented,
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                }));
            }
        }

        private async Task<RequestProfilerResponseModel> FormatResponseAsync(HttpContext context, Stream newResponseBody)
        {
            var result = new RequestProfilerResponseModel
            {
                StatusCode = context.Response.StatusCode,
                Headers = context.Request.Headers,
                Body = await ReadStreamInChunksAsync(newResponseBody)
            };

            return result;
        }

        private async Task<RequestProfilerRequestModel> FormatRequestAsync(HttpContext context)
        {
            var result = new RequestProfilerRequestModel
            {
                Method = context.Request.Method,
                Scheme = context.Request.Scheme,
                Host = context.Request.Host,
                PathBase = context.Request.PathBase,
                Path = context.Request.Path,
                QueryString = context.Request.QueryString,
                Headers = context.Request.Headers,
                Body = await GetRequestBodyAsync(context.Request)
            };
            return result;
        }



        public async Task<string> GetRequestBodyAsync(HttpRequest request)
        {
            request.EnableBuffering();
            using var stream = _recyclableMemoryStreamManager.GetStream();
            await request.Body.CopyToAsync(stream);
            request.Body.Seek(0, SeekOrigin.Begin);
            return await ReadStreamInChunksAsync(stream);
        }

        private static async Task<string> ReadStreamInChunksAsync(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            string result;

            using (var stringWriter = new StringWriter())
            using (var streamReader = new StreamReader(stream))
            {
                var readChunk = new char[ReadChunkBufferLength];
                int readChunkLength;
                //do while: is useful for the last iteration in case readChunkLength < chunkLength
                do
                {
                    readChunkLength = await streamReader.ReadBlockAsync(readChunk, 0, ReadChunkBufferLength);
                    await stringWriter.WriteAsync(readChunk, 0, readChunkLength);
                } while (readChunkLength > 0);

                result = stringWriter.ToString();
            }

            return result;
        }
    }
}
