namespace Configuration
{
    public class Configuration
    {
        public MongoConfiguration Mongo { get; set; } = new MongoConfiguration();
        public LoggingConfiguration Logging { get; set; } = new LoggingConfiguration();
        public GoogleConfiguration Google { get; set; } = new GoogleConfiguration();
        public ServiceConfiguration Service { get; set; } = new ServiceConfiguration();
        public AuthenticationConfiguration Authentication { get; set; } = new AuthenticationConfiguration();
    }
}
