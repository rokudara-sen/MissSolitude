namespace MissSolitude.Application;

public interface IUserService
{
    Task<CreateUserResult> CreateAsync(CreateUserCommand request, CancellationToken cancellationToken);
    Task<RemoveUserResult> RemoveAsync(RemoveUserCommand request, CancellationToken cancellationToken);
}