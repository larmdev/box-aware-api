using Box.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Box.API.Controllers;

[ApiController]
[Route("api/todo")]
public class TodoController : ControllerBase
{
    private readonly ITodoService _service;

    public TodoController(ITodoService service)
    {
        _service = service;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok(await _service.GetTodoAsync());
    }

    [Authorize]
    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        return Ok(await _service.GetTodoByIdAsync(id));
    }
}
