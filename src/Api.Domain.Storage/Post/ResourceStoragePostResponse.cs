using System.Diagnostics.CodeAnalysis;

namespace Api.Domain.Storage.Post
{
    [ExcludeFromCodeCoverage]
    public class ResourceStoragePostResponse : ResourceStorageResponseBase
    {      

        /// <summary>
        /// The desired payload model
        /// </summary>
        public Data.Model.Storage.Resource? Model { get; set; }

    }

}
