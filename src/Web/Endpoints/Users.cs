using Microsoft.AspNetCore.Http.HttpResults;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Application.Users.Commands.AssignRole;
using ProjectManagement.Application.Users.Commands.RemoveRole;
using ProjectManagement.Application.Users.Commands.UploadProfileImage;
using ProjectManagement.Application.Users.Commands.UpdateAccountSettings;
using ProjectManagement.Application.Users.Queries.GetAccountSettings;
using ProjectManagement.Application.Users.Queries.GetAllUsers;
using ProjectManagement.Application.Users.Queries.GetUserById;
using ProjectManagement.Domain.Constants;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Infrastructure.Identity;

namespace ProjectManagement.Web.Endpoints;

public class Users : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapIdentityApi<ApplicationUser>();

        // User management endpoints - Admin only
        app.MapGroup(this)
            .RequireAuthorization(Policies.CanManageUsers)
            .MapGet(GetAllUsers, "/")
            .MapGet(GetUserById, "/{id}")
            .MapPost(AssignRole, "/assign-role")
            .MapDelete(RemoveRole, "/{id}/roles/{roleName}");

        // Profile image upload - Authenticated users (with antiforgery disabled for file upload)
        app.MapPost("/api/users/profile-image", UploadProfileImage)
            .RequireAuthorization()
            .DisableAntiforgery();

        // Account settings - Authenticated users
        app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetAccountSettings, "/account-settings")
            .MapPut(UpdateAccountSettings, "/account-settings");
    }

    /// <summary>
    /// Get all users with optional filtering
    /// </summary>
    /// <param name="sender">MediatR sender</param>
    /// <param name="searchTerm">Search term for filtering users</param>
    /// <param name="roleFilter">Filter by role</param>
    /// <param name="isActiveFilter">Filter by active status</param>
    /// <returns>List of users</returns>
    public async Task<Results<Ok<List<UserDto>>, BadRequest<string>, UnauthorizedHttpResult, ForbidHttpResult>> GetAllUsers(
        ISender sender,
        string? searchTerm = null,
        string? roleFilter = null,
        bool? isActiveFilter = null)
    {
        try
        {
            var query = new GetAllUsersQuery
            {
                SearchTerm = searchTerm,
                RoleFilter = roleFilter,
                IsActiveFilter = isActiveFilter
            };

            var result = await sender.Send(query);

            if (result.Succeeded)
            {
                return TypedResults.Ok(result.Data);
            }

            return TypedResults.BadRequest(string.Join(", ", result.Errors));
        }
        catch (UnauthorizedAccessException)
        {
            return TypedResults.Unauthorized();
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest($"Error retrieving users: {ex.Message}");
        }
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    /// <param name="sender">MediatR sender</param>
    /// <param name="id">User ID</param>
    /// <returns>User details</returns>
    public async Task<Results<Ok<UserDto>, NotFound<string>, BadRequest<string>, UnauthorizedHttpResult>> GetUserById(
        ISender sender,
        string id)
    {
        try
        {
            var query = new GetUserByIdQuery { UserId = id };
            var result = await sender.Send(query);

            if (result.Succeeded)
            {
                return TypedResults.Ok(result.Data);
            }

            if (result.Errors.Any(e => e.Contains("not found")))
            {
                return TypedResults.NotFound(string.Join(", ", result.Errors));
            }

            return TypedResults.BadRequest(string.Join(", ", result.Errors));
        }
        catch (UnauthorizedAccessException)
        {
            return TypedResults.Unauthorized();
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest($"Error retrieving user: {ex.Message}");
        }
    }

    /// <summary>
    /// Assign role to user (Admin only)
    /// </summary>
    /// <param name="sender">MediatR sender</param>
    /// <param name="command">Assign role command</param>
    /// <returns>Success or error result</returns>
    public async Task<Results<Ok<string>, BadRequest<string>, UnauthorizedHttpResult, ForbidHttpResult>> AssignRole(
        ISender sender,
        AssignRoleCommand command)
    {
        try
        {
            var result = await sender.Send(command);

            if (result.Succeeded)
            {
                return TypedResults.Ok("กำหนด Role สำเร็จ");
            }

            return TypedResults.BadRequest(string.Join(", ", result.Errors));
        }
        catch (UnauthorizedAccessException)
        {
            return TypedResults.Unauthorized();
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest($"ไม่สามารถกำหนด Role ได้: {ex.Message}");
        }
    }

    /// <summary>
    /// Remove role from user (Admin only)
    /// </summary>
    /// <param name="sender">MediatR sender</param>
    /// <param name="id">User ID</param>
    /// <param name="roleName">Role name to remove</param>
    /// <returns>Success or error result</returns>
    public async Task<Results<Ok<string>, BadRequest<string>, UnauthorizedHttpResult, ForbidHttpResult>> RemoveRole(
        ISender sender,
        string id,
        string roleName)
    {
        try
        {
            var command = new RemoveRoleCommand
            {
                UserId = id,
                RoleName = roleName
            };

            var result = await sender.Send(command);

            if (result.Succeeded)
            {
                return TypedResults.Ok("ลบ Role สำเร็จ");
            }

            return TypedResults.BadRequest(string.Join(", ", result.Errors));
        }
        catch (UnauthorizedAccessException)
        {
            return TypedResults.Unauthorized();
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest($"ไม่สามารถลบ Role ได้: {ex.Message}");
        }
    }

    /// <summary>
    /// Upload profile image for current user
    /// </summary>
    /// <param name="sender">MediatR sender</param>
    /// <param name="user">Current user</param>
    /// <param name="imageFile">Image file to upload</param>
    /// <returns>Image URL or error</returns>
    public async Task<Results<Ok<ProfileImageUploadResponse>, BadRequest<string>, UnauthorizedHttpResult>> UploadProfileImage(
        ISender sender,
        IUser user,
        IFormFile imageFile)
    {
        try
        {
            if (string.IsNullOrEmpty(user.Id))
            {
                return TypedResults.Unauthorized();
            }

            var command = new UploadProfileImageCommand
            {
                ImageFile = imageFile,
                UserId = user.Id
            };

            var result = await sender.Send(command);

            if (result.Succeeded)
            {
                return TypedResults.Ok(new ProfileImageUploadResponse
                {
                    ImageUrl = result.Data ?? string.Empty,
                    Message = "อัพโหลดรูปโปรไฟล์สำเร็จ"
                });
            }

            return TypedResults.BadRequest(string.Join(", ", result.Errors));
        }
        catch (UnauthorizedAccessException)
        {
            return TypedResults.Unauthorized();
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest($"ไม่สามารถอัพโหลดรู��โปรไฟล์ได้: {ex.Message}");
        }
    }

    /// <summary>
    /// Get account settings for current user
    /// </summary>
    /// <param name="sender">MediatR sender</param>
    /// <param name="user">Current user</param>
    /// <returns>Account settings data</returns>
    public async Task<Results<Ok<AccountSettingsDto>, BadRequest<string>, UnauthorizedHttpResult>> GetAccountSettings(
        ISender sender,
        IUser user)
    {
        try
        {
            if (string.IsNullOrEmpty(user.Id))
            {
                return TypedResults.Unauthorized();
            }

            var query = new GetAccountSettingsQuery
            {
                UserId = user.Id
            };

            var result = await sender.Send(query);

            if (result.Succeeded)
            {
                return TypedResults.Ok(result.Data);
            }

            return TypedResults.BadRequest(string.Join(", ", result.Errors));
        }
        catch (UnauthorizedAccessException)
        {
            return TypedResults.Unauthorized();
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest($"ไม่สามารถดึงข้อมูลบัญชีได้: {ex.Message}");
        }
    }

    /// <summary>
    /// Update account settings for current user
    /// </summary>
    /// <param name="sender">MediatR sender</param>
    /// <param name="user">Current user</param>
    /// <param name="command">Update account settings command</param>
    /// <returns>Success or error result</returns>
    public async Task<Results<Ok<string>, BadRequest<string>, UnauthorizedHttpResult>> UpdateAccountSettings(
        ISender sender,
        IUser user,
        UpdateAccountSettingsCommand command)
    {
        try
        {
            if (string.IsNullOrEmpty(user.Id))
            {
                return TypedResults.Unauthorized();
            }

            var result = await sender.Send(command);

            if (result.Succeeded)
            {
                return TypedResults.Ok("บันทึกการเปลี่ยนแปลงเสร็จสิ้น");
            }

            return TypedResults.BadRequest(string.Join(", ", result.Errors));
        }
        catch (UnauthorizedAccessException)
        {
            return TypedResults.Unauthorized();
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest($"ไม่สามารถบันทึกการเปลี่ยนแปลงได้: {ex.Message}");
        }
    }
}

public class ProfileImageUploadResponse
{
    public string ImageUrl { get; set; } = default!;
    public string Message { get; set; } = default!;
}
