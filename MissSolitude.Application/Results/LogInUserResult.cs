namespace MissSolitude.Application.Results;

public sealed record LogInUserResult(string AccessToken, string RefreshToken, ReadUserResult User);