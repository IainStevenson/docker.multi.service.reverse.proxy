using System;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public partial class ResourcesController : ControllerBase
    {
        private readonly Guid _ownerId = new Guid("00000000-0000-0000-0000-000000000000"); // TODO: Replaced from AUTH token
        private readonly Guid _requestId = Guid.NewGuid();
        private IMediator _mediator;
        private readonly ILogger<ResourcesController> _logger;     

        public ResourcesController(
            ILogger<ResourcesController> logger,
            IMediator mediator
            )
        {
            _logger = logger;
            _mediator = mediator;
        }
    }
};