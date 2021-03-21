namespace Data.Model.Response
{
    public interface IApiLink
    {
        string Rel { get; set; }
        string Href { get; set; }
        string Action { get; set; }
    }
}
