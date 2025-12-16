namespace MissSolitude.Application.Results.User;

public sealed record LogInUserResult(string AccessToken, string RefreshToken, ReadUserResult User);