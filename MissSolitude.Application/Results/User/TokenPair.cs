namespace MissSolitude.Application.Results.User;

public sealed record TokenPair(string AccessToken, DateTimeOffset AccessTokenExpiration, string RefreshToken, DateTimeOffset RefreshTokenExpiration, string RefreshTokenHash);