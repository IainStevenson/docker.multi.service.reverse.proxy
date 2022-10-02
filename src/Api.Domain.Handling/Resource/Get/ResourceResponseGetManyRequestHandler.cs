using Api.Domain.Handling.Framework;
using AutoMapper;
using MediatR;

namespace Api.Domain.Handling.Resource.Get
{
    public class ResourceResponseGetManyRequestHandler : IRequestHandler<ResourceResponseGetManyRequest, ResourceResponse<IEnumerable<Data.Model.Response.Resource>>>
    {
        private readonly IRequestHeadersProvider _requestHeadersProvider;
        private readonly IMapper _mapper;
        private readonly Dictionary<string, string> EmptyEntityList = new Dictionary<string, string>() { };
        private readonly IResponseLinksProvider _responseLinksProvider;

        public ResourceResponseGetManyRequestHandler(
          IRequestHeadersProvider requestHeadersProvider,
          IMapper mapper,
          IResponseLinksProvider responseLinksProvider
          )
        {
            _requestHeadersProvider = requestHeadersProvider;
            _mapper = mapper;
            _responseLinksProvider = responseLinksProvider;
        }



        public async Task<ResourceResponse<IEnumerable<Data.Model.Response.Resource>>> Handle(ResourceResponseGetManyRequest request, CancellationToken cancellationToken)
        {
            var response = new ResourceResponse<IEnumerable<Data.Model.Response.Resource>>();

            IEnumerable<Data.Model.Storage.Resource> resources = new List<Data.Model.Storage.Resource>();

            if (resources.Any())
            {
                // if all of them are unmodified since then return none
                var unmodifiedItems = resources.Where(r =>
                                            r.Modified.HasValue ? r.Modified < request.IfModifiedSince :
                                            r.Created < request.IfModifiedSince);
                if (unmodifiedItems.Count() == resources.Count())
                {
                    response.StatusCode = request.StatusCode;
                    return response;
                }

                // else return modified since items
                var modifiedItems = resources.Where(r =>
                            r.Modified.HasValue ? r.Modified >= request.IfModifiedSince :
                            r.Created > request.IfModifiedSince);

                response.Model = _mapper.Map<IEnumerable<Data.Model.Response.Resource>>(request.Model);

            }
            else
            {
                response.Model = _mapper.Map<IEnumerable<Data.Model.Response.Resource>>(request.Model);
            }

            var relatedEntities = EmptyEntityList;
            foreach (var item in response.Model)
            {
                var systemKeys = new Dictionary<string, string>() {
                    { "{id}", $"{item.Id}" }
                 };
                response.Links = await _responseLinksProvider.BuildLinks(
                                                                request.Scheme,
                                                                request.Host,
                                                                request.PathBase.TrimEnd('/'),
                                                                request.Path.TrimEnd('/'),
                                                                systemKeys,
                                                                relatedEntities);
            }

            return response;
        }
    }


}
