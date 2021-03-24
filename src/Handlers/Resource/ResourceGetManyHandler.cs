using AutoMapper;
using MediatR;
using Response.Formater;
using Storage;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Handlers.Resource
{
    public class ResourceGetManyHandler : IRequestHandler<ResourceGetManyRequest, ResourceGetManyResponse>
    {
        private readonly IRepository<Data.Model.Storage.Resource> _storage;
        private readonly IRequestHeadersProvider _requestHeadersProvider;
        private readonly IMapper _mapper;

        public ResourceGetManyHandler(IRepository<Data.Model.Storage.Resource> storage,
          IRequestHeadersProvider requestHeadersProvider,
          IMapper mapper)
        {
            _storage = storage;
            _requestHeadersProvider = requestHeadersProvider;
            _mapper = mapper;
        }

        public async Task<ResourceGetManyResponse> Handle(ResourceGetManyRequest request, CancellationToken cancellationToken)
        {
            var response = new ResourceGetManyResponse();

            IEnumerable<Data.Model.Storage.Resource> resources = new List<Data.Model.Storage.Resource>();

            var ifModifiedSince = await _requestHeadersProvider.IfModifiedSince(request.Headers);

            if (ifModifiedSince.HasValue)
            {
                resources = await _storage.GetAsync(
                    r => r.OwnerId == request.OwnerId &&
                    r.Namespace == request.Namespace &&
                    r.Modified.HasValue ? r.Modified > ifModifiedSince.Value : r.Created > ifModifiedSince.Value);

                if (!resources.Any())
                {
                    response.StatusCode = System.Net.HttpStatusCode.NotModified;
                    return response;
                }
            }
            else
            {
                resources = await _storage.GetAsync(
                        r => r.OwnerId ==request.OwnerId && 
                        r.Namespace == request.Namespace);
            }
            if (!resources.Any())
            {
                response.StatusCode = System.Net.HttpStatusCode.NotFound;
                return response;
            }
            response.Model = _mapper.Map<IEnumerable<Data.Model.Response.Resource>>(resources);
            response.StatusCode = System.Net.HttpStatusCode.OK;
            return response;
        }
    }
}
