using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Response.Formater;
using Storage;

namespace Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public partial class ResourcesController : ControllerBase
    {

        private static Dictionary<string, string> EmptyEntityList = new Dictionary<string, string>() { };
        private readonly ILogger<ResourcesController> _logger;
        private readonly IResponseLinksProvider<Data.Model.Response.Resource> _responseLinksProvider;
        private readonly IRepository<Data.Model.Storage.Resource> _storage;
        private readonly Guid _ownerId = new Guid("00000000-0000-0000-0000-000000000000"); // TO BE Replaced by AUTH token OwnerId claim
        private readonly Guid _requestId = Guid.NewGuid();
        private readonly IResourceContentModifier<Data.Model.Response.Resource> _resourceModifier;
        private readonly IMapper _mapper;
        private readonly IResponseHeadersProvider _responseHeadersProvider;
        private readonly IRequestHeadersProvider _requestHeadersProvider;

        public ResourcesController(
            ILogger<ResourcesController> logger,
            IMapper mapper,
            IResponseLinksProvider<Data.Model.Response.Resource> responseLinksProvider,
            IResourceContentModifier<Data.Model.Response.Resource> resourceModifier,
            IRepository<Data.Model.Storage.Resource> storage,
            IResponseHeadersProvider responseHeadersProvider,
            IRequestHeadersProvider requestHeadersProvider)
        {
            _logger = logger;
            _mapper = mapper;
            _responseLinksProvider = responseLinksProvider;
            _storage = storage;
            _resourceModifier = resourceModifier;
            _responseHeadersProvider = responseHeadersProvider;
            _requestHeadersProvider = requestHeadersProvider;
        }

    }
};