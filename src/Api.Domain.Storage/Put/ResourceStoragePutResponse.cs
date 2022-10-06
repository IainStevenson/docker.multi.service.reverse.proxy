namespace Api.Domain.Storage.Put
{
    public class ResourceStoragePutResponse
    {
        public int StatusCode { get; set; }
        public Data.Model.Storage.Resource Model { get; set; }
        public List<string> RequestValidationErrors { get; set; } = new List<string>();
    }
}
