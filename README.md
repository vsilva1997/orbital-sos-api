# Orbital SOS API

API RESTful em .NET Core 8 para a plataforma Orbital SOS — conecta vítimas de desastres climáticos a abrigos, alertas e suporte.

## Tecnologias
- .NET 8 / ASP.NET Core Web API
- Entity Framework Core 8 + SQL Server
- JWT Bearer Authentication
- xUnit + WebApplicationFactory
- Docker + Docker Compose

## Rodar localmente (sem Docker)
1. Instale o [.NET 8 SDK](https://dotnet.microsoft.com/download) e SQL Server / LocalDB
2. `cd src/OrbitalSOS.API`
3. `dotnet run`
4. Acesse: http://localhost:5000/swagger

## Rodar via Docker Compose
```bash
docker compose up --build
```
API: http://localhost:8080/swagger

## Rodar os testes
```bash
dotnet test
```

## Estrutura do projeto
```
OrbitalSOS/
├── src/
│   ├── OrbitalSOS.API/          Controllers, Program.cs, Middlewares
│   ├── OrbitalSOS.Application/  DTOs, Services, Interfaces
│   ├── OrbitalSOS.Domain/       Entities, Enums
│   └── OrbitalSOS.Infrastructure/ DbContext, Repositories, Migrations
├── tests/
│   └── OrbitalSOS.Tests/        xUnit integration tests
├── Dockerfile
├── docker-compose.yml
└── README.md
```

## Endpoints principais
| Método | Rota | Auth |
|--------|------|------|
| POST | /api/auth/register | — |
| POST | /api/auth/login | — |
| GET | /api/shelters | — |
| POST | /api/shelters | Admin/Operador |
| GET | /api/climatealerts | — |
| POST | /api/climatealerts | Admin/Operador |
| GET | /api/riskareas | — |
| GET | /api/sosrequests | Autenticado |
| POST | /api/sosrequests | Autenticado |
| PATCH | /api/sosrequests/{id}/status | Admin/Operador |
| GET | /api/donations | — |
| POST | /api/donations | Autenticado |
| GET | /api/donations/shelter/{id}/balance | — |

## Fluxo de autenticação
1. POST /api/auth/register → recebe token
2. Copie o `token` da resposta
3. No Swagger: clique em **Authorize** → `Bearer {token}`
