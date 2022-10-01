using Data.Model;
using Microsoft.AspNetCore.Mvc;

namespace Response.Formatting
{
    /// <summary>
    /// Transalates a formatted response to final output ready for returning to the Mvc framework for client consumption
    /// </summary>
    public interface IResponseOutputHandler
    {
        IActionResult Handle<T>(ControllerBase resourcesController, ResourceOutputResponse<T> resourceOutput) where T : IEntity;
    }
}