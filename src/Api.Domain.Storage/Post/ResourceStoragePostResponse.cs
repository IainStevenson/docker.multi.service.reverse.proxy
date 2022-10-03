using System.Diagnostics.CodeAnalysis;

namespace Api.Domain.Storage.Post
{
    [ExcludeFromCodeCoverage]
    public class ResourceStoragePostResponse
    {
        /// <summary>
        /// Determines the resulting action response from the calling controller
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// The desired payload model
        /// </summary>
        public Data.Model.Storage.Resource Model { get; set; }

        /// <summary>
        /// A collection of resource validation error messages
        /// </summary>
        public List<string> RequestValidationErrors { get; set; } = new List<string>();
    }

}
