# syntax=docker/dockerfile:1

# ---------- Estágio 1: build (SDK completo, descartado no final) ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Restore em camada própria: só roda de novo quando um .csproj ou lock file muda
COPY Directory.Build.props ./
COPY src/OrbitalSOS.Domain/OrbitalSOS.Domain.csproj src/OrbitalSOS.Domain/packages.lock.json src/OrbitalSOS.Domain/
COPY src/OrbitalSOS.Infrastructure/OrbitalSOS.Infrastructure.csproj src/OrbitalSOS.Infrastructure/packages.lock.json src/OrbitalSOS.Infrastructure/
COPY src/OrbitalSOS.Application/OrbitalSOS.Application.csproj src/OrbitalSOS.Application/packages.lock.json src/OrbitalSOS.Application/
COPY src/OrbitalSOS.API/OrbitalSOS.API.csproj src/OrbitalSOS.API/packages.lock.json src/OrbitalSOS.API/
RUN dotnet restore src/OrbitalSOS.API/OrbitalSOS.API.csproj --locked-mode

# Código-fonte e publish em Release
COPY src/ src/
RUN dotnet publish src/OrbitalSOS.API/OrbitalSOS.API.csproj -c Release -o /app/publish --no-restore -p:UseAppHost=false

# ---------- Estágio 2: runtime (só o ASP.NET Core runtime) ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# curl apenas para o HEALTHCHECK (a imagem aspnet não traz curl)
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /app/publish .

# Porta 8080 (padrão do .NET 8 para usuário não root); ambiente padrão Production
ENV ASPNETCORE_HTTP_PORTS=8080 \
    ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

# Versão (SHA do commit) injetada pelo pipeline e exibida em /health
ARG APP_VERSION=dev
ENV APP_VERSION=$APP_VERSION

# Usuário não root que já existe nas imagens .NET 8
USER app

HEALTHCHECK --interval=30s --timeout=5s --start-period=60s --retries=3 \
    CMD curl -fsS http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "OrbitalSOS.API.dll"]
