namespace Configuration
{
    public class Options
    {
        public MongoOptions Mongo { get; set; } = new MongoOptions();
        public RequestResponseOptions RequestResponse { get; set; } = new RequestResponseOptions();
        public ServiceOptions Service { get; set; } = new ServiceOptions();
        public AuthenticationOptions Authentication { get; set; } = new AuthenticationOptions();
        public ApiOptions Api { get; set; } = new ApiOptions();
    }
}
