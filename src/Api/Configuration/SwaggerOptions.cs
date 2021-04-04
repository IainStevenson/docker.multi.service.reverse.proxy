namespace Configuration
{
    public class SwaggerOptions
    {
        public string Version { get; set; }
        public OpenAPIInfoOptions OpenAPIInfo { get; set; } = new OpenAPIInfoOptions();
        public string RoutePrefix { get; set; }
        public string Endpoint { get; set; }
        public string EndpointName { get; set; } 
    }
}