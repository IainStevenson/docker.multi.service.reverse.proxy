using MediatR;

namespace Api.Domain.Storage.Get
{
    public class ResourceStorageGetOneRequest : IRequest<ResourceStorageGetOneResponse>
    {
        public Guid Id { get; set; }      
        public string Namespace { get; set; }
        public Guid OwnerId { get; set; }
        public Guid RequestId { get; set; }        
        public List<string> IfNotETags { get; set; }  = new List<string>();
        /// <summary>
        /// Note: the default value of <see cref="DateTimeOffset.MinValue"/> resutls in getting everything ever that matches other constraints.
        /// To exclude a get based on near past changes set this value to an appropriate point in time.
        /// </summary>
        public DateTimeOffset IfModifiedSince { get;  set; }
    }
}
