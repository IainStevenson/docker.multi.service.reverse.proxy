namespace Configuration
{
    public class IdentityOptions
    {
        public string Authority { get; set; } 
        public string Audience { get; set; }
        public bool RequireHttpsMetadata { get; set; } 
        public TokenValidation TokenValidation { get; set; } = new TokenValidation();
    }
}