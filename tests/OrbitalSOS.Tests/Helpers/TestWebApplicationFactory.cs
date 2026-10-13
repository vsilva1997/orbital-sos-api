using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrbitalSOS.Infrastructure.Data;
namespace OrbitalSOS.Tests.Helpers;
public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    // Um banco InMemory por factory, compartilhado entre requisições (permite register -> login)
    private readonly string _dbName = "TestDb_" + Guid.NewGuid();
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var desc = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (desc != null) services.Remove(desc);
            services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase(_dbName));
        });
    }
}
