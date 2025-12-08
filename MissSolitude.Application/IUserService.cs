namespace MissSolitude.Application;

public interface IUserService
{
    Task<CreateUserResult> CreateAsync(CreateUserCommand request);
}