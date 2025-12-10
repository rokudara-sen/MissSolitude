using MissSolitude.Application.Commands;
using MissSolitude.Application.Results;

namespace MissSolitude.Application.Interfaces.Functions;

public interface ITokenService
{
    TokenPair IssueTokens(TokenUser user, IEnumerable<string>? roles = null);
    RefreshTokenDescriptor CreateRefreshToken();
}