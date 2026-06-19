using OrbitalSOS.Tests.Helpers;
namespace OrbitalSOS.Tests;
public class RiskAreasControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    public RiskAreasControllerTests(TestWebApplicationFactory factory) => _client = factory.CreateClient();
    [Fact]
    public async Task Get_ReturnsHttpStatusCode200()
    {
        var request = "/api/riskareas";
        var response = await _client.GetAsync(request);
        response.EnsureSuccessStatusCode();
    }
}
