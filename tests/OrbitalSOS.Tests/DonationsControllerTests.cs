using OrbitalSOS.Tests.Helpers;
namespace OrbitalSOS.Tests;
public class DonationsControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    public DonationsControllerTests(TestWebApplicationFactory factory) => _client = factory.CreateClient();
    [Fact]
    public async Task Get_ReturnsHttpStatusCode200()
    {
        var request = "/api/donations";
        var response = await _client.GetAsync(request);
        response.EnsureSuccessStatusCode();
    }
}
