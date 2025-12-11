# MissSolitude (Work in Progress)

> ⚠️ This repository is in an early, non-production-ready state. Interfaces, behavior, and database schema are expected to change frequently.

MissSolitude is an exploratory .NET 8 Web API that experiments with a layered architecture for simple user management. It exposes CRUD-style endpoints backed by PostgreSQL via Entity Framework Core. Passwords are hashed using ASP.NET Core Identity primitives, and login returns JWT access/refresh tokens issued by a configurable token service.

## What you'll find

- Domain-driven structure with separate `Domain`, `Application`, `Infrastructure`, and `API` layers.
- PostgreSQL persistence through EF Core with migrations stored in `MissSolitude.Infrastructure/Migrations`.
- Token issuance via `JWTTokenService` with adjustable issuer, audience, and signing key settings.
- Docker Compose definition for running the API and PostgreSQL together.
- 
## Project layout

- `MissSolitude.Domain` — Domain entities such as `User` and value objects like `EmailAddress`.
- `MissSolitude.Application` — Commands, results, and service contracts (e.g., `IUserService`, `IPasswordHasher`).
- `MissSolitude.Infrastructure` — EF Core data access (`DatabaseContext`), migrations, and concrete service implementations (password hashing, token service, user persistence).
- `MissSolitude.API` — ASP.NET Core Web API host with controllers and Swagger setup.
- `compose.yaml` — Docker Compose definition for the API and PostgreSQL database.

## Prerequisites

- .NET SDK 8.0+
- PostgreSQL 16+ (local instance or Docker)
- EF Core CLI (`dotnet tool install --global dotnet-ef`) for running migrations locally

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

JWT issuance uses the `Tokens` configuration section. Override the defaults in production and keep the signing key private:

```bash
export Tokens__Issuer="MissSolitude"
export Tokens__Audience="MissSolitude.Api"
export Tokens__SigningKey="<at-least-32-characters-random-string>"
export Tokens__AccessTokenMinutes=15
export Tokens__RefreshTokenDays=30
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
4. Visit Swagger UI at `https://localhost:7083/swagger` (or the HTTP profile at `http://localhost:5016/swagger`) to explore endpoints.

## Docker Compose

The provided `compose.yaml` spins up the API alongside PostgreSQL:

```bash
docker compose up --build
```

Create a `.env` file (copy from `.env.example`) or export variables before running:

```env
POSTGRES_USER=misssolitude
POSTGRES_PASSWORD=misssolitude
POSTGRES_DB=misssolitude
ASPNETCORE_Kestrel__Certificates__Default__Password=changeit
```

The API will be available on port `8080` by default.

### Enabling HTTPS locally

The API can also listen on `https://localhost:8443` when provided a certificate. Docker Compose is preconfigured to load a **PFX** from `./certificates/misssolitude.pfx` and pass the password via `ASPNETCORE_Kestrel__Certificates__Default__Password`. A standalone `.key` file is not sufficient for Kestrel; you need a PKCS#12 bundle (PFX) that includes the certificate and private key.

1. Generate a development certificate (uses OpenSSL) and export it to `./certificates/misssolitude.pfx`:
   ```bash
   ./scripts/generate-dev-certificate.sh
   ```
    - Set `ASPNETCORE_Kestrel__Certificates__Default__Password` (or `PFX_PASSWORD`) beforehand to choose a password.
    - If you already have a `misssolitude.key` and `misssolitude.crt`, the script will reuse them to create the PFX. If only one of those files exists, delete the leftover and re-run the script so it can produce both.
2. Add the same password to your environment (or `.env`) before running Compose:
   ```bash
   export ASPNETCORE_Kestrel__Certificates__Default__Password="changeit"
   ```
3. Start the stack and reach the API at `https://localhost:8443`.

## Database migrations

Create new migrations from the solution root:

```bash
dotnet ef migrations add <MigrationName> --project MissSolitude.Infrastructure --startup-project MissSolitude.API
```

Apply migrations to the configured database:

```bash
dotnet ef database update --project MissSolitude.Infrastructure --startup-project MissSolitude.API
```

## HTTP endpoints (current state)

- `POST /api/user` — Create a user.
  ```json
  {
    "username": "alice",
    "password": "Sup3rS3cret!",
    "email": "alice@example.com"
  }
  ```
- `GET /api/user/{id}` — Retrieve a single user by ID.
- `PUT /api/user` — Update a user's username and email.
  ```json
  {
    "id": "<user-id-guid>",
    "username": "alice-updated",
    "email": "new-email@example.com"
  }
  ```
- `DELETE /api/user/{id}` — Remove a user by ID.
- `POST /api/user/login` — Authenticate a user and receive JWT tokens.
  ```json
  {
    "identifier": "alice", // username or email
    "password": "Sup3rS3cret!"
  }
  ```
  **Response:**
  ```json
  {
    "accessToken": "<jwt>",
    "refreshToken": "<refresh-token>",
    "user": {
      "id": "<user-id-guid>",
      "username": "alice",
      "email": "alice@example.com"
    }
  }
  ```

> Note: There is no authentication, authorization, or comprehensive input validation yet. Use only in controlled, non-production environments.

## Contributing

Contributions and feedback are welcome, but expect rapid iteration and breaking changes while the project stabilizes.