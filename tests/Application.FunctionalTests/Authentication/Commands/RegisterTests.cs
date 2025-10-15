using ProjectManagement.Application.Authentication.Commands.Register;
using ProjectManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectManagement.Application.FunctionalTests.Authentication.Commands;

using static Testing;

public class RegisterTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRegisterNewUser()
    {
        // Arrange
        var command = new RegisterCommand
        {
            Email = "newuser@example.com",
            Username = "newuser",
            Password = "NewUser1234!",
            ConfirmPassword = "NewUser1234!",
            FirstName = "New",
            LastName = "User"
        };

        // Act
        var result = await SendAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();
        
        // Verify user was created in database
        using var scope = _scopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var user = await userManager.FindByEmailAsync(command.Email);
        user.Should().NotBeNull();
        user!.Email.Should().Be(command.Email);
        user.UserName.Should().Be(command.Username);
    }

    [Test]
    public async Task ShouldFailWithDuplicateEmail()
    {
        // Arrange
        var email = "duplicate@example.com";
        await RunAsUserAsync(email, "Test1234!", Array.Empty<string>());

        var command = new RegisterCommand
        {
            Email = email,
            Username = "differentusername",
            Password = "Test1234!",
            ConfirmPassword = "Test1234!",
            FirstName = "Test",
            LastName = "User"
        };

        // Act
        var result = await SendAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Test]
    public async Task ShouldFailWithPasswordMismatch()
    {
        // Arrange
        var command = new RegisterCommand
        {
            Email = "mismatch@example.com",
            Username = "mismatchuser",
            Password = "Test1234!",
            ConfirmPassword = "DifferentPassword1234!",
            FirstName = "Test",
            LastName = "User"
        };

        // Act
        var result = await SendAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("รหัสผ่าน") || e.Contains("password"));
    }

    [Test]
    public async Task ShouldFailWithWeakPassword()
    {
        // Arrange
        var command = new RegisterCommand
        {
            Email = "weak@example.com",
            Username = "weakuser",
            Password = "123",
            ConfirmPassword = "123",
            FirstName = "Test",
            LastName = "User"
        };

        // Act
        var result = await SendAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Test]
    public async Task ShouldFailWithInvalidEmail()
    {
        // Arrange
        var command = new RegisterCommand
        {
            Email = "invalid-email",
            Username = "testuser",
            Password = "Test1234!",
            ConfirmPassword = "Test1234!",
            FirstName = "Test",
            LastName = "User"
        };

        // Act
        var result = await SendAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    private static IServiceScopeFactory _scopeFactory = null!;

    [OneTimeSetUp]
    public void Setup()
    {
        var factory = new CustomWebApplicationFactory(
            TestDatabaseFactory.CreateAsync().Result.GetConnection(),
            TestDatabaseFactory.CreateAsync().Result.GetConnectionString()
        );
        _scopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();
    }
}
