using MissSolitude.Application.Commands;
using MissSolitude.Application.Results;

namespace MissSolitude.Application.Services.Interfaces;

public interface IUserService
{
    Task<CreateUserResult> CreateAsync(CreateUserCommand request, CancellationToken cancellationToken);
    Task<DeleteUserResult> DeleteAsync(DeleteUserCommand request, CancellationToken cancellationToken);
}