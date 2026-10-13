using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OrbitalSOS.API.HealthChecks;
using OrbitalSOS.API.Middlewares;
using OrbitalSOS.Application.Interfaces;
using OrbitalSOS.Application.Services;
using OrbitalSOS.Infrastructure.Data;
using OrbitalSOS.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// DB: a connection string vem da variável de ambiente ConnectionStrings__DefaultConnection
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrWhiteSpace(connectionString))
        throw new InvalidOperationException("Defina a variável de ambiente ConnectionStrings__DefaultConnection.");
    // Retry para falhas transitórias (ex.: Azure SQL serverless retomando após pausa)
    opt.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure());
});

// Repos
builder.Services.AddScoped<ShelterRepository>();
builder.Services.AddScoped<ClimateAlertRepository>();
builder.Services.AddScoped<RiskAreaRepository>();
builder.Services.AddScoped<SosRequestRepository>();
builder.Services.AddScoped<DonationRepository>();
builder.Services.AddScoped<UserRepository>();

// Services
builder.Services.AddScoped<IShelterService, ShelterService>();
builder.Services.AddScoped<IClimateAlertService, ClimateAlertService>();
builder.Services.AddScoped<IRiskAreaService, RiskAreaService>();
builder.Services.AddScoped<ISosRequestService, SosRequestService>();
builder.Services.AddScoped<IDonationService, DonationService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// JWT: a chave vem da variável de ambiente Jwt__Key (HMAC-SHA256 exige no mínimo 32 bytes)
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey) || Encoding.UTF8.GetByteCount(jwtKey) < 32)
    throw new InvalidOperationException("Defina a variável de ambiente Jwt__Key com pelo menos 32 caracteres.");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, ValidateAudience = true,
            ValidateLifetime = true, ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
builder.Services.AddAuthorization();

// Health check: verifica se a API consegue conectar no banco
builder.Services.AddHealthChecks().AddDbContextCheck<AppDbContext>("database");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Orbital SOS API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization", Type = SecuritySchemeType.Http,
        Scheme = "Bearer", BearerFormat = "JWT", In = ParameterLocation.Header,
        Description = "Informe o token JWT. Exemplo: Bearer {seu_token}"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }},
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Apply migrations on startup (somente em banco relacional; os testes usam EF InMemory)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (db.Database.IsRelational()) db.Database.Migrate();
    if (await AdminSeeder.SeedAsync(db, app.Configuration["Admin:Email"], app.Configuration["Admin:Password"], app.Configuration["Admin:Name"]))
        app.Logger.LogInformation("Administrador inicial criado: {Email}", app.Configuration["Admin:Email"]);
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapHealthChecks("/health", new HealthCheckOptions { ResponseWriter = HealthResponseWriter.WriteAsync })
    .AllowAnonymous();
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
app.MapControllers();
app.Run();

public partial class Program { }
