namespace MissSolitude.Application.Interfaces.Functions;

public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken);
}