using Data.Model;
using Microsoft.AspNetCore.Mvc;

namespace Api.Domain.Handling
{
    /// <summary>
    /// Transalates a formatted response to final output ready for returning to the Mvc framework for client consumption
    /// </summary>
    public interface IResponseOutputHandler
    {

        IActionResult Handle<T>(ControllerBase controller, ResourceOutputResponse<T> resourceOutput) where T : IEntity;
        IActionResult Handle<T>(ControllerBase controller, ResourceOutputResponse resourceOutput) where T : IEntity;
    }
}