using System.Net.Http.Json;
using OrbitalSOS.Application.DTOs.Auth;
using OrbitalSOS.Tests.Helpers;
namespace OrbitalSOS.Tests;
public class AuthControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    public AuthControllerTests(TestWebApplicationFactory factory) => _client = factory.CreateClient();
    [Fact]
    public async Task Register_ReturnsHttpStatusCode200()
    {
        var dto = new RegisterDto { Name = "Test User", Email = $"test{Guid.NewGuid()}@test.com", Password = "pass123" };
        var response = await _client.PostAsJsonAsync("/api/auth/register", dto);
        response.EnsureSuccessStatusCode();
    }
    [Fact]
    public async Task Login_InvalidCredentials_Returns401()
    {
        var dto = new LoginDto { Email = "naoexiste@test.com", Password = "errada" };
        var response = await _client.PostAsJsonAsync("/api/auth/login", dto);
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
