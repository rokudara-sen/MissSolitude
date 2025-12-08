namespace MissSolitude.Domain;

public sealed class User
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Password { get; private set; }
    public EmailAddress Email { get; private set; }

    public User()
    {
        
    }
    
    public User(Guid id, string username, string password, EmailAddress email)
    {
        Id = id;
        Username = username;
        Password = password;
        Email = email;
    }

    public void ChangeEmail(EmailAddress email)
    {
        Email = email;
    }
}