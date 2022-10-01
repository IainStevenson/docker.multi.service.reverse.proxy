using MediatR;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Response.Formatting
{
    public class PostResourceOutputHandler : IRequestHandler<PostResourceOutputRequest, ResourceOutputResponse<Data.Model.Response.Resource>>
    {
        private readonly Dictionary<string, string> EmptyEntityList = new() { };
        private readonly IResponseLinksProvider _responseLinksProvider;
        private readonly IResourceContentModifier<Data.Model.Response.Resource> _resourceModifier;
        public PostResourceOutputHandler( IResponseLinksProvider responseLinksProvider, IResourceContentModifier<Data.Model.Response.Resource> resourceModifier)
        {
            _responseLinksProvider = responseLinksProvider;
            _resourceModifier = resourceModifier;
        }
    
        public async Task<ResourceOutputResponse<Data.Model.Response.Resource>> Handle(PostResourceOutputRequest request, CancellationToken cancellationToken)
        {
            var response = new ResourceOutputResponse<Data.Model.Response.Resource>() { StatusCode = (HttpStatusCode)request.StatusCode };

            if (response.StatusCode != HttpStatusCode.Created) return response;

            response.Model = string.IsNullOrWhiteSpace(request.Keys) ?
                            request.Model :
                            await _resourceModifier.CollapseContent(request.Model, request.Keys.Split(','));

            await BuildResponseLinks(request, response);

            return response;

        }

        private async Task BuildResponseLinks(PostResourceOutputRequest request, ResourceOutputResponse<Data.Model.Response.Resource> response)
        {
            var systemKeys = new Dictionary<string, string>() { { "{id}", $"{request.Model.Id}" } };
            var relatedEntities = EmptyEntityList;

            response.Links = await _responseLinksProvider.BuildLinks(
                                                            request.Scheme,
                                                            request.Host,
                                                            request.PathBase.TrimEnd('/'),
                                                            request.Path.TrimEnd('/'),
                                                            systemKeys,
                                                            relatedEntities);
        }
    }

}