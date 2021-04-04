namespace Configuration
{
    public class GoogleConfiguration
    {
        public string SignInScheme {  get;set; } = "idsrv.external";
        public string ClientId {  get;set;} = "<your google client id>";
        public string ClientSecret {  get;set;} = "<your google client secret>";
    }
}