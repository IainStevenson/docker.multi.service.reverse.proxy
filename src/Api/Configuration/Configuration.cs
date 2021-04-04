namespace Configuration
{
    public class Configuration
    {
        public SwaggerConfiguration Swagger { get; set; } = new SwaggerConfiguration();
        public MongoConfiguration Mongo { get; set; } = new MongoConfiguration();
        public IdentityConfiguration Identity { get; set; } = new IdentityConfiguration();
        public HeadersConfiguration Headers { get; set; } = new HeadersConfiguration();
        public LoggingConfiguration Logging { get; set; } = new LoggingConfiguration();
        public AuthorizationConfiguration Authorization { get; set; } = new AuthorizationConfiguration();
        public ServiceConfiguration Service { get; set; } = new ServiceConfiguration();
    }
}
