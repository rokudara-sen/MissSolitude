using MissSolitude.Application.Commands;
using MissSolitude.Application.Results;

namespace MissSolitude.Application.Services;

public interface IUserService
{
    Task<CreateUserResult> CreateAsync(CreateUserCommand request, CancellationToken cancellationToken);
    Task<RemoveUserResult> RemoveAsync(RemoveUserCommand request, CancellationToken cancellationToken);
}