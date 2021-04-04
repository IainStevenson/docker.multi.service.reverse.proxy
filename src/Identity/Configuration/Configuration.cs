﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Configuration
{
    public class Configuration
    {
        //public SwaggerConfiguration Swagger { get; set; } = new SwaggerConfiguration();    
        public MongoConfiguration Mongo { get; set; } = new MongoConfiguration();
        //public IdentityConfiguration Identity { get; set; } = new IdentityConfiguration();
        //public HeadersConfiguration Headers { get;  set; } = new HeadersConfiguration();
        public LoggingConfiguration Logging { get;  set; } = new LoggingConfiguration();
        public GoogleConfiguration Google { get;  set; } = new GoogleConfiguration();
        public ServiceConfiguration Service { get;  set; } = new ServiceConfiguration();
    }
}
