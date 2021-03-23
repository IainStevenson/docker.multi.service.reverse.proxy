using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Handlers
{
    public interface IResponseAction
    {
        IActionResult Handle(ControllerBase source);
    }
}
