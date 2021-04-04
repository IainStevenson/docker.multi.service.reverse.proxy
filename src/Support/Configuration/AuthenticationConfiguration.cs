namespace Configuration
{
    public class AuthenticationConfiguration
    {
        public string Scheme { get;  set; } = "Cookies";
        public string ChallengeScheme { get; set; } = "oidc";
        public string Authority { get;  set; } = "https://mystore.local/identity";
        public bool RequireHttpsMetadata { get;  set; } = true;
        public string  RequiredScopes { get;  set; } = "openid, profile, email, myStore.Api";
        public bool SaveTokens { get;  set; } = true;
        public string ResponseType { get;  set; }= "code";
        public string ClientSecret { get;  set; } = "secret";
        public string ClientId { get;  set; } = "myStore.Mvc";
        public bool GetClaimsFromUserInfoEndpoint { get; set; } = true;
    }
}