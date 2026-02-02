using Microsoft.AspNetCore.Mvc;
using Box.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IAuthService _service;

    public UsersController(
        IAuthService service
        )
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult Get() => Ok("Hello from Box.API");

    [HttpPost]
    public IActionResult Post() => Ok("Hello from Box.API");
}

