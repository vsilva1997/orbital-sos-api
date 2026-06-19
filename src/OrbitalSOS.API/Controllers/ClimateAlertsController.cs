using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrbitalSOS.Application.DTOs.ClimateAlerts;
using OrbitalSOS.Application.Interfaces;
namespace OrbitalSOS.API.Controllers;
[ApiController]
[Route("api/climatealerts")]
public class ClimateAlertsController : ControllerBase
{
    private readonly IClimateAlertService _svc;
    public ClimateAlertsController(IClimateAlertService svc) => _svc = svc;
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10,
        [FromQuery] string? region = null, [FromQuery] bool? isActive = null)
    {
        var result = await _svc.GetAllAsync(page, pageSize, region, isActive);
        return Ok(result);
    }
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _svc.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }
    [HttpPost]
    [Authorize(Roles = "Admin,Operador")]
    public async Task<IActionResult> Create([FromBody] CreateClimateAlertDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _svc.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
}
