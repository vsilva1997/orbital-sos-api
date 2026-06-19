using OrbitalSOS.Tests.Helpers;
namespace OrbitalSOS.Tests;
public class SosRequestsControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    public SosRequestsControllerTests(TestWebApplicationFactory factory) => _client = factory.CreateClient();
    [Fact]
    public async Task Get_WithoutAuth_Returns401()
    {
        var request = "/api/sosrequests";
        var response = await _client.GetAsync(request);
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
