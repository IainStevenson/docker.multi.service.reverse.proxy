using System.Net;

namespace Data.Model.Response
{
    public class RequestErrorModel
    {
        public string Controller { get; set; }
        public string Action { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string Reason { get; set; }        
    }
}
