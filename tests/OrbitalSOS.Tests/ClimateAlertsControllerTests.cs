using OrbitalSOS.Tests.Helpers;
namespace OrbitalSOS.Tests;
public class ClimateAlertsControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    public ClimateAlertsControllerTests(TestWebApplicationFactory factory) => _client = factory.CreateClient();
    [Fact]
    public async Task Get_ReturnsHttpStatusCode200()
    {
        var request = "/api/climatealerts";
        var response = await _client.GetAsync(request);
        response.EnsureSuccessStatusCode();
    }
}
