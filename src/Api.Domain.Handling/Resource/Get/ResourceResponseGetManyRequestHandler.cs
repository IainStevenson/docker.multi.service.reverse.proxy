using Api.Domain.Handling.Framework;
using AutoMapper;
using MediatR;

namespace Api.Domain.Handling.Resource.Get
{
    public class ResourceResponseGetManyRequestHandler : IRequestHandler<ResourceResponseGetManyRequest, ResourceResponse<IEnumerable<Data.Model.Response.Resource>>>
    {
        private readonly IMapper _mapper;
        private readonly Dictionary<string, string> EmptyEntityList = new Dictionary<string, string>() { };
        private readonly IResponseLinksProvider _responseLinksProvider;

        public ResourceResponseGetManyRequestHandler(
          IMapper mapper,
          IResponseLinksProvider responseLinksProvider
          )
        {
            _responseLinksProvider = responseLinksProvider;
            _mapper = mapper;
        }

        public async Task<ResourceResponse<IEnumerable<Data.Model.Response.Resource>>> Handle(ResourceResponseGetManyRequest request, CancellationToken cancellationToken)
        {
            var response = new ResourceResponse<IEnumerable<Data.Model.Response.Resource>>();

            IEnumerable<Data.Model.Storage.Resource> resources = new List<Data.Model.Storage.Resource>();

            if (request.StatusCode == System.Net.HttpStatusCode.OK)
            {

                response.Model = _mapper.Map<IEnumerable<Data.Model.Response.Resource>>(request.Model);


                foreach (var item in response.Model)
                {
                    var systemKeys = new Dictionary<string, string>() { { "{id}", $"{item.Id}" } };
                    response.Links = await _responseLinksProvider.BuildLinks(
                                                                    request.Scheme,
                                                                    request.Host,
                                                                    request.PathBase.TrimEnd('/'),
                                                                    request.Path.TrimEnd('/'),
                                                                    systemKeys,
                                                                    EmptyEntityList);
                }
            }

            return response;
        }
    }
}
