using Microsoft.Extensions.Diagnostics.HealthChecks;
namespace OrbitalSOS.API.HealthChecks;
// Resposta JSON do /health: usada pelo smoke test do pipeline para confirmar ambiente e versão publicada
public static class HealthResponseWriter
{
    public static Task WriteAsync(HttpContext ctx, HealthReport report)
    {
        var env = ctx.RequestServices.GetRequiredService<IWebHostEnvironment>();
        var config = ctx.RequestServices.GetRequiredService<IConfiguration>();
        var body = new
        {
            status = report.Status.ToString(),
            environment = env.EnvironmentName,
            version = config["APP_VERSION"] ?? "dev",
            totalDurationMs = Math.Round(report.TotalDuration.TotalMilliseconds),
            checks = report.Entries.ToDictionary(e => e.Key, e => e.Value.Status.ToString())
        };
        return ctx.Response.WriteAsJsonAsync(body);
    }
}
