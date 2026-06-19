using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrbitalSOS.Infrastructure.Data;
namespace OrbitalSOS.Tests.Helpers;
public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var desc = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (desc != null) services.Remove(desc);
            services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("TestDb_" + Guid.NewGuid()));
        });
    }
}
