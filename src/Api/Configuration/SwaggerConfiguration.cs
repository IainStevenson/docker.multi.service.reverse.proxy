namespace Configuration
{
    public class SwaggerConfiguration
    {
        public string Version { get; set; } = "v1";
        public OpenAPIInfoConfiguration OpenAPIInfo { get;  set; } = new OpenAPIInfoConfiguration();
        public string RoutePrefix { get; set; } = "api";
        public string Endpoint { get; set; } = "/swagger/v1/swagger.json";
        public string EndpointName { get; set; } = "myStore.Api V1";
    }
}