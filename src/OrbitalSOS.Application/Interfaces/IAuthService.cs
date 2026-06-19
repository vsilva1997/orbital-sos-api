using OrbitalSOS.Application.DTOs.Auth;
namespace OrbitalSOS.Application.Interfaces;
public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
}
