using Microsoft.AspNetCore.Mvc;

namespace Handlers
{
    public interface IResponseAction
    {
        IActionResult Handle(ControllerBase source);
    }
}
