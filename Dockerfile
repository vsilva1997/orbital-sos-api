FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/OrbitalSOS.API/OrbitalSOS.API.csproj", "src/OrbitalSOS.API/"]
COPY ["src/OrbitalSOS.Application/OrbitalSOS.Application.csproj", "src/OrbitalSOS.Application/"]
COPY ["src/OrbitalSOS.Domain/OrbitalSOS.Domain.csproj", "src/OrbitalSOS.Domain/"]
COPY ["src/OrbitalSOS.Infrastructure/OrbitalSOS.Infrastructure.csproj", "src/OrbitalSOS.Infrastructure/"]
RUN dotnet restore "src/OrbitalSOS.API/OrbitalSOS.API.csproj"
COPY . .
RUN dotnet publish "src/OrbitalSOS.API/OrbitalSOS.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_ENVIRONMENT=Docker
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "OrbitalSOS.API.dll"]
