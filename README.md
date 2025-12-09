# MissSolitude (Work in Progress)

> ⚠️ This repository is in an early, non-production-ready state. Interfaces, behavior, and database schema are expected to change frequently.

MissSolitude is an exploratory .NET 8 Web API that experiments with a layered architecture for simple user management. It currently exposes basic CRUD-style endpoints for working with users backed by PostgreSQL via Entity Framework Core. Passwords are hashed using ASP.NET Core Identity primitives.

## Project layout

- `MissSolitude.Domain` — Domain entities such as `User` and value objects like `EmailAddress`.
- `MissSolitude.Application` — Commands, results, and service contracts (e.g., `IUserService`, `IPasswordHasher`).
- `MissSolitude.Infrastructure` — EF Core data access (`DatabaseContext`), migrations, and concrete service implementations (password hashing and user persistence).
- `MissSolitude.API` — ASP.NET Core Web API host with controllers and Swagger setup.
- `compose.yaml` — Docker Compose definition for the API and PostgreSQL database.

## Prerequisites

- .NET SDK 8.0+
- PostgreSQL 16+ (local instance or Docker)

## Configuration

Set the connection string named `ConnectionStrings:Default` for the API host:

- Development default (from `appsettings.Development.json`):
  ```json
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=MissSolitude;Username=postgres;Password=postgres"
  }
  ```
- Or override via environment variables (useful for Docker):
  ```bash
  export ConnectionStrings__Default="Host=<host>;Port=5432;Database=<db>;Username=<user>;Password=<password>"
  ```

## Local development

1. Restore dependencies:
   ```bash
   dotnet restore
   ```
2. Apply EF Core migrations to provision the database (requires the connection string above):
   ```bash
   dotnet ef database update --project MissSolitude.Infrastructure --startup-project MissSolitude.API
   ```
3. Run the API:
   ```bash
   dotnet run --project MissSolitude.API
   ```
4. Visit Swagger UI at `https://localhost:7076/swagger` (port may vary based on your ASP.NET profile) to explore endpoints.

## Docker Compose

The provided `compose.yaml` spins up the API alongside PostgreSQL:

```bash
docker compose up --build
```

Configure `POSTGRES_USER`, `POSTGRES_PASSWORD`, and `POSTGRES_DB` environment variables (e.g., via a `.env` file) before running. The API will be available on port `8080` by default.

## HTTP endpoints (current state)

- `GET /api/user` — List all users.
- `GET /api/user/{id}` — Retrieve a single user by ID.
- `POST /api/user` — Create a user.
  ```json
  {
    "username": "alice",
    "password": "Sup3rS3cret!",
    "email": "alice@example.com"
  }
  ```
- `DELETE /api/user/{id}` — Remove a user by ID.

> Note: There is no authentication, authorization, or input validation yet. Use only in controlled, non-production environments.

## Contributing

Contributions and feedback are welcome, but expect rapid iteration and breaking changes while the project stabilizes.