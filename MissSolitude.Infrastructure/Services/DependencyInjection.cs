using Microsoft.Extensions.DependencyInjection;
using MissSolitude.Application.Interfaces.Functions;
using MissSolitude.Application.Interfaces.Repositories;
using MissSolitude.Infrastructure.Auth;
using MissSolitude.Infrastructure.Repositories;

namespace MissSolitude.Infrastructure.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IContactRepository, ContactRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<ITokenService, JWTTokenService>();
        return services;
    }
}