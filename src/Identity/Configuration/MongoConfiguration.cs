namespace Configuration
{
    public class MongoConfiguration
    {
        public string ConnectionString { get; set; } = $"mongodb://storage:storagepass@mongo.mystore.local:27017";
        public string DatabaseName { get; set; } = "myStoreIdentity";
    }
}