namespace MissSolitude.Domain;

public sealed record EmailAddress(string Value)
{
    public override String ToString() => Value;
}