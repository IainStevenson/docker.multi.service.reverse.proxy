namespace Api.Configuration
{
    public class TokenValidation
    {
        public bool ValidateIssuer { get; set; } = true;
        public bool ValidateAudience { get; set; } = true;
    }
}