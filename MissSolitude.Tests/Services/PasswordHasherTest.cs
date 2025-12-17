using JetBrains.Annotations;
using MissSolitude.Infrastructure.Services;

namespace MissSolitude.Tests.Services;

[TestSubject(typeof(PasswordHasher))]
public class PasswordHasherTest
{
    private readonly PasswordHasher _systemUnderTest = new();

    [Fact]
    public void Verify_shouldReturnTrue_WhenPasswordMatchesHash()
    {
        // Arrange
        var password = "MySecurePassword123!";

        // Act
        var hash = _systemUnderTest.Hash(password);
        var result = _systemUnderTest.Verify(password, hash);

        // Assert
        Assert.True(result, "Verify should return true for the correct password.");
    }

    [Fact]
    public void Verify_shouldReturnFalse_WhenPasswordDoesNotMatch()
    {
        // Arrange
        var password = "MySecurePassword123!";
        var wrongPassword = "WrongPassword!";

        // Act
        var hash = _systemUnderTest.Hash(password);
        var result = _systemUnderTest.Verify(wrongPassword, hash);

        // Assert
        Assert.False(result, "Verify should return false for an incorrect password.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Hash_shouldReturnString_WhenPasswordIsValid(string? validPassword)
    {
        if (validPassword == null)
        {
            Assert.Throws<ArgumentNullException>(() => _systemUnderTest.Hash(validPassword));
        }
        else
        {
            var hash = _systemUnderTest.Hash(validPassword);
            Assert.False(string.IsNullOrWhiteSpace(hash));
        }
    }
}