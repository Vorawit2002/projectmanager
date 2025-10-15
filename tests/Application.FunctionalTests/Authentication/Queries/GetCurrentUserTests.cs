using ProjectManagement.Application.ApplicationUserProfile.Queries;

namespace ProjectManagement.Application.FunctionalTests.Authentication.Queries;

using static Testing;

public class GetCurrentUserTests : BaseTestFixture
{
    [Test]
    public async Task ShouldReturnCurrentUserProfile()
    {
        // Arrange
        var email = "currentuser@example.com";
        var password = "Test1234!";
        var userId = await RunAsUserAsync(email, password, Array.Empty<string>());

        var query = new GetApplicationUserProfileCommand();

        // Act
        var result = await SendAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(Guid.Parse(userId));
        result.UserName.Should().Be(email);
    }

    [Test]
    public async Task ShouldReturnUserWithRoles()
    {
        // Arrange
        var email = "adminuser@example.com";
        var password = "Admin1234!";
        await RunAsUserAsync(email, password, new[] { "Administrator" });

        var query = new GetApplicationUserProfileCommand();

        // Act
        var result = await SendAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.RoleNames.Should().Contain("Administrator");
    }
}
