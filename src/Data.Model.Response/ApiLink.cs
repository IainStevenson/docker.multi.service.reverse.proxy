namespace Data.Model.Response
{
    public class ApiLink : IApiLink
    {
        public string Action { get; set; }
        public string Rel { get; set; }
        public string Href { get; set; }
    }
}