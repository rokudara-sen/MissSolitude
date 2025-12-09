namespace MissSolitude.Domain.ValueObjects;

public readonly record struct EmailAddress(string Value)
{
    public override String ToString() => Value;
}