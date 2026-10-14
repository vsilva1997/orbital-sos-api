using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using OrbitalSOS.Tests.Helpers;
namespace OrbitalSOS.Tests;
public class HealthCheckTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _client;
    public HealthCheckTests(TestWebApplicationFactory factory) { _factory = factory; _client = factory.CreateClient(); }
    [Fact]
    public async Task Health_WithoutAuth_ReturnsHealthy()
    {
        var response = await _client.GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Healthy", body.GetProperty("status").GetString());
        Assert.Equal("Healthy", body.GetProperty("checks").GetProperty("database").GetString());
        Assert.False(string.IsNullOrEmpty(body.GetProperty("environment").GetString()));
        Assert.False(string.IsNullOrEmpty(body.GetProperty("version").GetString()));
    }
    [Fact]
    public async Task SwaggerJson_ReturnsHttpStatusCode200()
    {
        var response = await _client.GetAsync("/swagger/v1/swagger.json");
        response.EnsureSuccessStatusCode();
    }
    [Fact]
    public async Task Root_RedirectsToSwagger()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        var response = await client.GetAsync("/");
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal("/swagger", response.Headers.Location?.OriginalString);
    }
}
