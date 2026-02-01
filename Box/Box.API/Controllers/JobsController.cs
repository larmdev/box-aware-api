using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Box.Application.Interfaces;
using Hangfire;

[ApiController]
[Route("api/jobs")]
public class JobsController : ControllerBase
{
    private readonly IEmailJobService _service;

    public JobsController(
        IEmailJobService service
        )
    {
        _service = service;
    }

    [Authorize]
    [HttpGet("send-email/{msg}")]
    public async Task<IActionResult> Logout(string msg)
    {
        BackgroundJob.Enqueue(() => _service.SendWelcomeEmailAsync(msg));
        return Ok();
    }
}

