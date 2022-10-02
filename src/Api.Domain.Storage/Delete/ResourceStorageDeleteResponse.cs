namespace Api.Domain.Storage.Delete
{
    public class ResourceStorageDeleteResponse
    {
        public int StatusCode { get;  set; } = 400;
        public List<string> RequestValidationErrors { get;  set; } = new List<string>();
    }
}
