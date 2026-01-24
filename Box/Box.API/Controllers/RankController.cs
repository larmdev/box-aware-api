using Microsoft.AspNetCore.Mvc;
using Box.Application.Interfaces;
using Box.Application.Dtos;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/rank")]
public class RankController : ControllerBase
{
    private readonly IRankService _service;

    public RankController(IRankService service)
    {
        _service = service;
    }
    
    [Authorize]
    [HttpPost]
    [Route("")]
    public IActionResult Post([FromBody] RankRequestDto request)
    {
        var result = _service.Process(request);
        return Ok(result);
    }
}
