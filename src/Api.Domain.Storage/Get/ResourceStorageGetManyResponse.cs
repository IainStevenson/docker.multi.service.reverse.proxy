namespace Api.Domain.Storage.Get
{
    public class ResourceStorageGetManyResponse : ResourceStorageResponseBase
    {
        /// <summary>
        /// The desired payload model
        /// </summary>
        public IEnumerable<Data.Model.Storage.Resource> Model { get; set; }

       
    }

}
