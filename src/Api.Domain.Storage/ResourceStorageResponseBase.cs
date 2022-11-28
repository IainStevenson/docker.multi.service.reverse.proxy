namespace Api.Domain.Storage
{
    public class ResourceStorageResponseBase
    {
        /// <summary>
        /// Determines the handler action outcome
        /// </summary>
        public HttpStatusCodes StatusCode { get; set; }


        /// <summary>
        /// A collection of resource validation error messages
        /// </summary>
        public List<string> RequestValidationErrors { get; set; } = new List<string>();
    }
}
