using Api.Domain.Handling.Framework;
using AutoMapper;
using MediatR;

namespace Api.Domain.Handling.Resource.Get
{
    public class ResourceResponseGetOneRequestHandler : IRequestHandler<ResourceResponseGetOneRequest, ResourceResponse<Data.Model.Response.Resource>>
    {
        private readonly IResponseHeadersProvider _responseHeadersProvider;
        private readonly IMapper _mapper;
      

        public ResourceResponseGetOneRequestHandler(
            IResponseHeadersProvider responseHeadersProvider,
            IMapper mapper           
            )
        {
            _responseHeadersProvider = responseHeadersProvider;
            _mapper = mapper;            
        }

        public async Task<ResourceResponse<Data.Model.Response.Resource>> Handle(ResourceResponseGetOneRequest request, CancellationToken cancellationToken)
        {
            ResourceResponse<Data.Model.Response.Resource> response = new ResourceResponse<Data.Model.Response.Resource>();
            response.StatusCode = request.StatusCode;

            if (request.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseModel = _mapper.Map<Data.Model.Response.Resource>(request.Model);
                response.Model = responseModel;
                response.Headers = _responseHeadersProvider.AddHeadersFromItem(responseModel);
            }

            return response;
        }
    }
}
