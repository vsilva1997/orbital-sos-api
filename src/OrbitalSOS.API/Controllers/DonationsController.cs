using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrbitalSOS.Application.DTOs.Donations;
using OrbitalSOS.Application.Interfaces;
namespace OrbitalSOS.API.Controllers;
[ApiController]
[Route("api/donations")]
public class DonationsController : ControllerBase
{
    private readonly IDonationService _svc;
    public DonationsController(IDonationService svc) => _svc = svc;
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _svc.GetAllAsync(page, pageSize);
        return Ok(result);
    }
    [HttpGet("shelter/{shelterId:guid}/balance")]
    public async Task<IActionResult> GetBalance(Guid shelterId)
    {
        var result = await _svc.GetShelterBalanceAsync(shelterId);
        return Ok(result);
    }
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateDonationDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _svc.CreateAsync(dto);
        return Created($"api/donations/{result.Id}", result);
    }
    [HttpPost("needs")]
    [Authorize(Roles = "Admin,Operador")]
    public async Task<IActionResult> CreateNeed([FromBody] CreateShelterNeedDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _svc.CreateShelterNeedAsync(dto);
        return Created($"api/donations/needs/{result.Id}", result);
    }
}
