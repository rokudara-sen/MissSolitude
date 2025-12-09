namespace MissSolitude.Domain.Entities;

public sealed class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public EmailAddress Email { get; private set; }

    public User()
    {
        
    }
    
    public User(Guid id, string username, string passwordHash, EmailAddress email)
    {
        Id = id;
        Username = username;
        PasswordHash = passwordHash;
        Email = email;
    }

    public void ChangeEmail(EmailAddress email)
    {
        Email = email;
    }
}