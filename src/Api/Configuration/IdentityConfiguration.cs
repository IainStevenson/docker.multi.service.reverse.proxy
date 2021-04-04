namespace Configuration
{
    public class IdentityConfiguration
    {
        public string Authority { get; set; } = "https://mystore.local/identity";
        public string Audience { get; internal set; } = "https://mystore.local/identity/resources";
        public bool RequireHttpsMetadata { get; set; } = true;

        public TokenValidation TokenValidation { get; set; } = new TokenValidation();
    }
}