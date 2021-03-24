namespace Data.Model.Response
{
    public class ApiLink : IApiLink
    {
        public string Rel { get; set; }
        public string Href { get; set; }
        public string Action { get; set; }
    }
}