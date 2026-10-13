using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using OrbitalSOS.Application.DTOs.Auth;
using OrbitalSOS.Tests.Helpers;
namespace OrbitalSOS.Tests;
public class AdminSeedWebApplicationFactory : TestWebApplicationFactory
{
    public const string Email = "admin@teste.local";
    public const string Password = "Senha-Admin-Teste-123";
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.UseSetting("Admin:Email", Email);
        builder.UseSetting("Admin:Password", Password);
    }
}
public class AdminSeedTests : IClassFixture<AdminSeedWebApplicationFactory>
{
    private readonly HttpClient _client;
    public AdminSeedTests(AdminSeedWebApplicationFactory factory) => _client = factory.CreateClient();
    [Fact]
    public async Task SeededAdmin_CanLoginAndCreateShelter()
    {
        var login = await _client.PostAsJsonAsync("/api/auth/login",
            new LoginDto { Email = AdminSeedWebApplicationFactory.Email, Password = AdminSeedWebApplicationFactory.Password });
        login.EnsureSuccessStatusCode();
        var auth = await login.Content.ReadFromJsonAsync<AuthResponseDto>();
        Assert.Equal("Admin", auth!.Role);

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/shelters")
        {
            Content = JsonContent.Create(new { name = "Abrigo Teste", address = "Rua A, 1", latitude = -23.55, longitude = -46.63, capacity = 50, type = 0 })
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", auth.Token);
        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}
