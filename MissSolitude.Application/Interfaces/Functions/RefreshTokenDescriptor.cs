namespace MissSolitude.Application.Interfaces.Functions;

public sealed record RefreshTokenDescriptor(string Token, string TokenHash, DateTimeOffset Expiration);