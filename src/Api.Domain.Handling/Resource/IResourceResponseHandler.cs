using Data.Model;
using Microsoft.AspNetCore.Mvc;

namespace Api.Domain.Handling.Resource
{
    /// <summary>
    /// Transalates a formatted response to final output ready for returning to the Mvc framework for client consumption
    /// </summary>
    public interface IResourceResponseHandler
    {

        IActionResult Handle<T>(ControllerBase controller, ResourceResponse<T> resourceOutput) where T : IEntity;
        IActionResult Handle<T>(ControllerBase controller, ResourceResponse resourceOutput) where T : IEntity;
    }
}