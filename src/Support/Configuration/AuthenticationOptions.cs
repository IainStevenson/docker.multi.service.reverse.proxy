namespace Configuration
{
    public class AuthenticationOptions
    {
        public string Scheme { get; set; }
        public string ChallengeScheme { get; set; }
        public string Authority { get; set; }
        public bool RequireHttpsMetadata { get; set; }
        public string RequiredScopes { get; set; }
        public bool SaveTokens { get; set; }
        public string ResponseType { get; set; }
        public string ClientSecret { get; set; }
        public string ClientId { get; set; }
        public bool GetClaimsFromUserInfoEndpoint { get; set; }
    }
}