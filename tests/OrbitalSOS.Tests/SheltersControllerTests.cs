using System.Net.Http.Json;
using OrbitalSOS.Application.DTOs.Auth;
using OrbitalSOS.Tests.Helpers;
namespace OrbitalSOS.Tests;
public class SheltersControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    public SheltersControllerTests(TestWebApplicationFactory factory) => _client = factory.CreateClient();
    [Fact]
    public async Task Get_ReturnsHttpStatusCode200()
    {
        var request = "/api/shelters";
        var response = await _client.GetAsync(request);
        response.EnsureSuccessStatusCode();
    }
    [Fact]
    public async Task GetById_NotFound_Returns404()
    {
        var response = await _client.GetAsync($"/api/shelters/{Guid.NewGuid()}");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
}
