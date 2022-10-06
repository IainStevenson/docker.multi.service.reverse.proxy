using System;
using Api.Domain.Handling.Framework;
using Api.Domain.Handling.Resource;
using Api.Domain.Storage;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Api.Controllers
{
    /// <summary>
    /// Manages the storage of client side, client defined JSON formatted content. 
    /// The content is stored keyed to a client specific dataset.
    /// Providing a means of organising the content in virtual folders by means of a namespace concept.
    /// Client specific datasets are keyed to a unique Owner Id providied by registration via the OATH authority for the API.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public partial class ResourcesController : ControllerBase
    {
        /// <summary>
        /// TODO: Replaced from AUTH token
        /// </summary>
        private readonly Guid _ownerId = new Guid("00000000-0000-0000-0000-000000000001"); 
        /// <summary>
        /// Generated here per request
        /// </summary>
        private readonly Guid _requestId = Guid.NewGuid();
        /// <summary>
        /// Mediator service to abstract controller processing
        /// </summary>
        private readonly IMediator _mediator;
        /// <summary>
        /// Logger for this controller
        /// </summary>
        private readonly ILogger<ResourcesController> _logger;
        /// <summary>
        /// Factory for request creation.
        /// </summary>
        private readonly IResourceRequestFactory _resourceRequestFactory;
        /// <summary>
        /// Factory for response output creation
        /// </summary>
        private readonly IResourceResponseFactory _resourceResponseFactory;
        /// <summary>
        /// performs final handling of the request/response process.
        /// </summary>
        private readonly IResourceResponseHandler _resourceResponseHandler;
        /// <summary>
        /// Provides a service to process the incoming <see cref="HttpRequst.Headers"/>
        /// </summary>
        private readonly IRequestHeadersProvider _requestHeadersProvider;
        /// <summary>
        /// Provides a sservice to proces the outgoing <see cref="HttpResponse.Headers"/>
        /// </summary>
        private readonly IResponseHeadersProvider _responseHeadersProvider;
        /// <summary>
        /// Controller constructor
        /// </summary>
        /// <param name="logger">The logger instance provided by dependency injection</param>
        /// <param name="mediator">The mediator instance provided by dependency injection.</param>
        public ResourcesController(
                ILogger<ResourcesController> logger, 
                IMediator mediator, 
                IResourceRequestFactory resourceRequestFactory, 
                IRequestHeadersProvider requestHeadersProvider,
                IResponseHeadersProvider responseHeadersProvider,
                IResourceResponseFactory resourceResponseFactory,
                IResourceResponseHandler responseResponseHandler
            )
        {
            _logger = logger;
            _mediator = mediator;
            _resourceRequestFactory = resourceRequestFactory;
            _requestHeadersProvider= requestHeadersProvider;
            _responseHeadersProvider= responseHeadersProvider;
            _resourceResponseFactory = resourceResponseFactory;
            _resourceResponseHandler = responseResponseHandler;
        }
    }
};