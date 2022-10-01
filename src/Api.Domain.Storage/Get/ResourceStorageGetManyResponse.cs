namespace Api.Domain.Storage.Get
{
    public class ResourceStorageGetManyResponse
    {
        /// <summary>
        /// Determines the handler action status
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// The desired payload model
        /// </summary>
        public IEnumerable<Data.Model.Storage.Resource> Model { get; set; }

        /// <summary>
        /// A collection of resource validation error messages
        /// </summary>
        public List<string> RequestValidationErrors { get; set; } = new List<string>();
    }

}
