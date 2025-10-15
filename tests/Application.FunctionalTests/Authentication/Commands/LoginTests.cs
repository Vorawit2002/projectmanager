using ProjectManagement.Application.Authentication.Commands.Login;
using ProjectManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectManagement.Application.FunctionalTests.Authentication.Commands;

using static Testing;

public class LoginTests : BaseTestFixture
{
    [Test]
    public async Task ShouldLoginWithEmail()
    {
        // Arrange
        var email = "testuser@example.com";
        var password = "Test1234!";
        await RunAsUserAsync(email, password, Array.Empty<string>());

        var command = new LoginCommand
        {
            EmailOrUsername = email,
            Password = password
        };

        // Act
        var result = await SendAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Token.Should().NotBeNullOrEmpty();
        result.Data.Email.Should().Be(email);
    }

    [Test]
    public async Task ShouldLoginWithUsername()
    {
        // Arrange
        var username = "testuser";
        var email = "testuser2@example.com";
        var password = "Test1234!";
        
        using var scope = _scopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var user = new ApplicationUser { UserName = username, Email = email };
        await userManager.CreateAsync(user, password);

        var command = new LoginCommand
        {
            EmailOrUsername = username,
            Password = password
        };

        // Act
        var result = await SendAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Token.Should().NotBeNullOrEmpty();
        result.Data.Username.Should().Be(username);
    }

    [Test]
    public async Task ShouldFailWithInvalidPassword()
    {
        // Arrange
        var email = "testuser3@example.com";
        var password = "Test1234!";
        await RunAsUserAsync(email, password, Array.Empty<string>());

        var command = new LoginCommand
        {
            EmailOrUsername = email,
            Password = "WrongPassword123!"
        };

        // Act
        var result = await SendAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Test]
    public async Task ShouldFailWithNonExistentUser()
    {
        // Arrange
        var command = new LoginCommand
        {
            EmailOrUsername = "nonexistent@example.com",
            Password = "Test1234!"
        };

        // Act
        var result = await SendAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Test]
    public async Task ShouldIncludeRolesInToken()
    {
        // Arrange
        var email = "admin@example.com";
        var password = "Admin1234!";
        await RunAsUserAsync(email, password, new[] { "Administrator" });

        var command = new LoginCommand
        {
            EmailOrUsername = email,
            Password = password
        };

        // Act
        var result = await SendAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Roles.Should().Contain("Administrator");
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
