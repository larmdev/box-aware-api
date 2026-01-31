using Box.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/students")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _service;

    public StudentsController(IStudentService service)
    {
        _service = service;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetStudents(
        int offset = 0,
        int limit = 10)
    {
        var result = await _service.GetStudentsAsync(offset, limit);
        return Ok(result);
    }

    [Authorize]
    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetStudentById(int id)
    {
        var result = await _service.GetStudentByIdAsync(id);
        return Ok(result);
    }

}
