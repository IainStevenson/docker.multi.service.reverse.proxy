namespace Configuration
{
    public class Options
    {
        public SwaggerOptions Swagger { get; set; } = new SwaggerOptions();
        public MongoOptions Mongo { get; set; } = new MongoOptions();
        public IdentityOptions Identity { get; set; } = new IdentityOptions();
        public HeadersOptions Headers { get; set; } = new HeadersOptions();
        public RequestResponseOptions RequestResponse { get; set; } = new RequestResponseOptions();
        public AuthorizationOptions Authorization { get; set; } = new AuthorizationOptions();
        public ServiceOptions Service { get; set; } = new ServiceOptions();
    }
}
