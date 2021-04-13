namespace Configuration
{
    public class Options
    {
        public MongoOptions Mongo { get; set; } = new MongoOptions();
        public RequestResponseOptions RequestResponse { get; set; } = new RequestResponseOptions();
        public ServiceOptions Service { get; set; } = new ServiceOptions();       
        public ExternalProviderOptions Google {  get;set; } = new ExternalProviderOptions();
    }
}
