using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrbitalSOS.Application.DTOs.SosRequests;
using OrbitalSOS.Application.Interfaces;
namespace OrbitalSOS.API.Controllers;
[ApiController]
[Route("api/sosrequests")]
[Authorize]
public class SosRequestsController : ControllerBase
{
    private readonly ISosRequestService _svc;
    public SosRequestsController(ISosRequestService svc) => _svc = svc;
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _svc.GetAllAsync(page, pageSize);
        return Ok(result);
    }
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _svc.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSosRequestDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _svc.CreateAsync(dto, userId);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = "Admin,Operador")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateSosStatusDto dto)
    {
        var result = await _svc.UpdateStatusAsync(id, dto);
        return Ok(result);
    }
}
