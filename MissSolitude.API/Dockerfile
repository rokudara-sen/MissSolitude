FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# --- 1. COPY ALL PROJECT FILES ---
# You must copy every .csproj that is part of the solution dependency graph
COPY ["MissSolitude.API/MissSolitude.API.csproj", "MissSolitude.API/"]
COPY ["MissSolitude.Tests/MissSolitude.Tests.csproj", "MissSolitude.Tests/"]
COPY ["MissSolitude.Contracts/MissSolitude.Contracts.csproj", "MissSolitude.Contracts/"]
COPY ["MissSolitude.Infrastructure/MissSolitude.Infrastructure.csproj", "MissSolitude.Infrastructure/"]
# I am assuming you also have these, based on your architecture:
COPY ["MissSolitude.Domain/MissSolitude.Domain.csproj", "MissSolitude.Domain/"]
COPY ["MissSolitude.Application/MissSolitude.Application.csproj", "MissSolitude.Application/"]

# --- 2. RESTORE ---
# Restoring the API and Test projects will now successfully recursively 
# restore the dependencies because the csproj files exist in the image.
RUN dotnet restore "MissSolitude.API/MissSolitude.API.csproj"
RUN dotnet restore "MissSolitude.Tests/MissSolitude.Tests.csproj"

# --- 3. COPY SOURCE CODE & TEST ---
COPY . .

RUN dotnet test "MissSolitude.Tests/MissSolitude.Tests.csproj" -c $BUILD_CONFIGURATION --no-restore

# Continue with the standard build...
WORKDIR "/src/MissSolitude.API"
RUN dotnet build "./MissSolitude.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MissSolitude.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MissSolitude.API.dll"]