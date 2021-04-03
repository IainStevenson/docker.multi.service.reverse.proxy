using System.Collections.Generic;

namespace Api.Configuration
{
    public class AuthoriszationPolicy
    {
        public string Name { get;  set; } = "ApiScope";
        public string ClaimName { get; set; } = "scope";
        public string ClaimValues { get;  set; } = "myStore.Api";
    }
}