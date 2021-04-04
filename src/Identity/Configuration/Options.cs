namespace Configuration
{
    public class Options
    {
        public MongoOptions Mongo { get; set; } = new MongoOptions();
        public RequestResponseOptions RequestResponse { get; set; } = new RequestResponseOptions();
        public ServiceOptions Service { get; set; } = new ServiceOptions();       
        public GoogleOptions Google {  get;set; } = new GoogleOptions();
    }
}
