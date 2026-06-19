using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrbitalSOS.Application.DTOs.Shelters;
using OrbitalSOS.Application.Interfaces;
namespace OrbitalSOS.API.Controllers;
[ApiController]
[Route("api/shelters")]
public class SheltersController : ControllerBase
{
    private readonly IShelterService _svc;
    public SheltersController(IShelterService svc) => _svc = svc;
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10,
        [FromQuery] double? lat = null, [FromQuery] double? lon = null, [FromQuery] double? radiusKm = null)
    {
        var result = await _svc.GetAllAsync(page, pageSize, lat, lon, radiusKm);
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
    public async Task<IActionResult> Create([FromBody] CreateShelterDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _svc.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
}
