namespace ProjectManagement.Application.Common.Interfaces;

/// <summary>
/// Service for applying role-based data filtering to queries
/// </summary>
public interface IDataFilterService
{
    /// <summary>
    /// Applies role-based filtering to a query based on user's role and department
    /// </summary>
    /// <typeparam name="T">The entity type being queried</typeparam>
    /// <param name="query">The base query to filter</param>
    /// <param name="userId">The ID of the current user</param>
    /// <param name="roles">The roles assigned to the current user</param>
    /// <returns>Filtered query based on role permissions</returns>
    Task<IQueryable<T>> ApplyRoleBasedFilterAsync<T>(
        IQueryable<T> query,
        string userId,
        string[] roles) where T : class;

    /// <summary>
    /// Checks if a user can access a specific resource based on their role and department
    /// </summary>
    /// <param name="userId">The ID of the current user</param>
    /// <param name="roles">The roles assigned to the current user</param>
    /// <param name="resourceOwnerId">The ID of the resource owner (CreatedBy or EmployeeId)</param>
    /// <param name="resourceDepartmentId">The department ID of the resource (optional)</param>
    /// <returns>True if user can access the resource, false otherwise</returns>
    Task<bool> CanAccessResourceAsync(
        string userId,
        string[] roles,
        string? resourceOwnerId = null,
        Guid? resourceDepartmentId = null);
}
