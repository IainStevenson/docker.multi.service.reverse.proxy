namespace Data.Model.Response
{
    public class BadRequestModel
    {
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Reason { get; set; }
    }
}
