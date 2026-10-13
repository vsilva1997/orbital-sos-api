using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OrbitalSOS.Application.DTOs.Auth;
using OrbitalSOS.Application.Interfaces;
using OrbitalSOS.Domain.Entities;
using OrbitalSOS.Domain.Enums;
using OrbitalSOS.Infrastructure.Repositories;
namespace OrbitalSOS.Application.Services;
public class AuthService : IAuthService
{
    private readonly UserRepository _repo;
    private readonly IConfiguration _config;
    public AuthService(UserRepository repo, IConfiguration config) { _repo = repo; _config = config; }
    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        if (await _repo.EmailExistsAsync(dto.Email))
            throw new InvalidOperationException("Email already in use.");
        var user = new User
        {
            Name = dto.Name, Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = UserRole.Usuario
        };
        await _repo.AddAsync(user);
        return GenerateToken(user);
    }
    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _repo.GetByEmailAsync(dto.Email) ?? throw new UnauthorizedAccessException("Invalid credentials.");
        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials.");
        return GenerateToken(user);
    }
    private AuthResponseDto GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddHours(8);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
        };
        var token = new JwtSecurityToken(_config["Jwt:Issuer"], _config["Jwt:Audience"], claims, expires: expires, signingCredentials: creds);
        return new AuthResponseDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Name = user.Name, Email = user.Email,
            Role = user.Role.ToString(), ExpiresAt = expires
        };
    }
}
