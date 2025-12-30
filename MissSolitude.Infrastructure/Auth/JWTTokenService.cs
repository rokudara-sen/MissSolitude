using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MissSolitude.Application.Commands.User;
using MissSolitude.Application.Interfaces.Functions;
using MissSolitude.Application.Results.User;

namespace MissSolitude.Infrastructure.Auth;

public sealed class JWTTokenService : ITokenService
{
    private readonly TokenOptions _tokenOptions;
    private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = new();

    public JWTTokenService(IOptions<TokenOptions> tokenOptions)
    {
        _tokenOptions = tokenOptions.Value;

        if (string.IsNullOrWhiteSpace(_tokenOptions.SigningKey) || _tokenOptions.SigningKey.Length < 32)
            throw new ArgumentException("Invalid signing key.");
    }

    public TokenPair IssueTokens(TokenUser user, IEnumerable<string>? roles = null)
    {
        var timeNow = DateTimeOffset.UtcNow;

        var accessExpires = timeNow.AddMinutes(_tokenOptions.AccessTokenMinutes);
        var refresh = CreateRefreshToken();

        var accessToken = CreateAccessToken(user, roles, timeNow.UtcDateTime, accessExpires.UtcDateTime);

        return new TokenPair(
            AccessToken: accessToken,
            AccessTokenExpiration: accessExpires,
            RefreshToken: refresh.Token,
            RefreshTokenExpiration: refresh.Expiration,
            RefreshTokenHash: refresh.TokenHash
        );
    }

    public RefreshTokenDescriptor CreateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);

        var token = Base64UrlEncoder.Encode(bytes);
        var hash = Sha256Base64Url(token);

        var expiresAt = DateTimeOffset.UtcNow.AddDays(_tokenOptions.RefreshTokenDays);

        return new RefreshTokenDescriptor(token, hash, expiresAt);
    }

    private string CreateAccessToken(TokenUser user, IEnumerable<string>? roles, DateTime issuedAtUtc, DateTime expiresAtUtc)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenOptions.SigningKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.Username),
            new(JwtRegisteredClaimNames.Email, user.Email.Value),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
        };

        if (roles != null)
        {
            foreach (var role in roles)
                claims.Add(new(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            issuer: _tokenOptions.Issuer,
            audience: _tokenOptions.Audience,
            claims: claims,
            notBefore: issuedAtUtc,
            expires: expiresAtUtc,
            signingCredentials: creds
        );

        return _jwtSecurityTokenHandler.WriteToken(token);
    }

    private static string Sha256Base64Url(string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        var hash = SHA256.HashData(bytes);
        return Base64UrlEncoder.Encode(hash);
    }
}