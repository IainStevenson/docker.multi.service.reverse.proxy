using System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Resource.Handling;
using Response.Formatting;

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


        private readonly IResourceRequestFactory _resourceRequestFactory;

        private readonly IResponseOutputHandler _responseOutputHandler;

        /// <summary>
        /// Controller constructor
        /// </summary>
        /// <param name="logger">The logger instance provided by dependency injection</param>
        /// <param name="mediator">the mediator instance provided by dependency injection.</param>
        public ResourcesController(ILogger<ResourcesController> logger, IMediator mediator, IResourceRequestFactory resourceRequestFactory, IResponseOutputHandler responseOutputHandler)
        {
            _logger = logger;
            _mediator = mediator;
            _resourceRequestFactory = resourceRequestFactory;
            _responseOutputHandler = responseOutputHandler;
        }
    }
};