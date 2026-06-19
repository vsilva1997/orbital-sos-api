using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrbitalSOS.Application.DTOs.RiskAreas;
using OrbitalSOS.Application.Interfaces;
namespace OrbitalSOS.API.Controllers;
[ApiController]
[Route("api/riskareas")]
public class RiskAreasController : ControllerBase
{
    private readonly IRiskAreaService _svc;
    public RiskAreasController(IRiskAreaService svc) => _svc = svc;
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] Guid? alertId = null)
    {
        var result = await _svc.GetAllAsync(page, pageSize, alertId);
        return Ok(result);
    }
    [HttpPost]
    [Authorize(Roles = "Admin,Operador")]
    public async Task<IActionResult> Create([FromBody] CreateRiskAreaDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _svc.CreateAsync(dto);
        return Created($"api/riskareas/{result.Id}", result);
    }
}
