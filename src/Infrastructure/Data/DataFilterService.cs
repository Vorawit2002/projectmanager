using Microsoft.EntityFrameworkCore;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Constants;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Infrastructure.Data;

/// <summary>
/// Service for applying role-based data filtering
/// </summary>
public class DataFilterService : IDataFilterService
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;

    public DataFilterService(
        IApplicationDbContext context,
        IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    /// <summary>
    /// Applies role-based filtering to queries based on user role
    /// Admin: All data
    /// Manager: Department data only
    /// User/Viewer: Own data only
    /// </summary>
    public async Task<IQueryable<T>> ApplyRoleBasedFilterAsync<T>(
        IQueryable<T> query,
        string userId,
        string[] roles) where T : class
    {
        // Admin and Administrator can see everything
        if (roles.Contains(Roles.Admin) || roles.Contains(Roles.Administrator))
        {
            return query;
        }

        // Get user's employee record to determine department
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.UserId == userId);

        if (employee == null)
        {
            // If no employee record, return empty query
            return query.Where(_ => false);
        }

        var employeeId = employee.Id;
        var departmentId = employee.DepartmentId;

        // Apply filtering based on entity type
        var entityType = typeof(T);

        // ActivityPlan filtering
        if (entityType == typeof(ActivityPlan))
        {
            var activityPlanQuery = query as IQueryable<ActivityPlan>;
            if (activityPlanQuery != null)
            {
                var filtered = ApplyActivityPlanFilter(activityPlanQuery, roles, employeeId, departmentId, userId);
                return (filtered as IQueryable<T>)!;
            }
        }

        // Project filtering
        if (entityType == typeof(Project))
        {
            var projectQuery = query as IQueryable<Project>;
            if (projectQuery != null)
            {
                var filtered = ApplyProjectFilter(projectQuery, roles, employeeId, departmentId, userId);
                return (filtered as IQueryable<T>)!;
            }
        }

        // Employee filtering
        if (entityType == typeof(Employee))
        {
            var employeeQuery = query as IQueryable<Employee>;
            if (employeeQuery != null)
            {
                var filtered = ApplyEmployeeFilter(employeeQuery, roles, employeeId, departmentId);
                return (filtered as IQueryable<T>)!;
            }
        }

        // CheckInCheckOut filtering
        if (entityType == typeof(CheckInCheckOut))
        {
            var checkInQuery = query as IQueryable<CheckInCheckOut>;
            if (checkInQuery != null)
            {
                var filtered = ApplyCheckInCheckOutFilter(checkInQuery, roles, employeeId, departmentId);
                return (filtered as IQueryable<T>)!;
            }
        }

        // For other entities, apply default filtering
        return ApplyDefaultFilter(query, roles, employeeId, departmentId, userId);
    }

    /// <summary>
    /// Checks if user can access a specific resource
    /// </summary>
    public async Task<bool> CanAccessResourceAsync(
        string userId,
        string[] roles,
        string? resourceOwnerId = null,
        Guid? resourceDepartmentId = null)
    {
        // Admin and Administrator can access everything
        if (roles.Contains(Roles.Admin) || roles.Contains(Roles.Administrator))
        {
            return true;
        }

        // Get user's employee record
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.UserId == userId);

        if (employee == null)
        {
            return false;
        }

        // Manager can access resources in their department
        if (roles.Contains(Roles.Manager))
        {
            if (resourceDepartmentId.HasValue && employee.DepartmentId.HasValue)
            {
                return resourceDepartmentId.Value == employee.DepartmentId.Value;
            }

            // If checking by owner ID, check if owner is in same department
            if (!string.IsNullOrEmpty(resourceOwnerId))
            {
                var resourceOwnerEmployee = await _context.Employees
                    .FirstOrDefaultAsync(e => e.UserId == resourceOwnerId);

                if (resourceOwnerEmployee != null && 
                    resourceOwnerEmployee.DepartmentId.HasValue && 
                    employee.DepartmentId.HasValue)
                {
                    return resourceOwnerEmployee.DepartmentId.Value == employee.DepartmentId.Value;
                }
            }

            return false;
        }

        // User and Viewer can only access their own resources
        if (roles.Contains(Roles.User) || roles.Contains(Roles.Viewer))
        {
            if (!string.IsNullOrEmpty(resourceOwnerId))
            {
                return resourceOwnerId == userId;
            }

            return false;
        }

        return false;
    }

    #region Private Filter Methods

    private IQueryable<ActivityPlan> ApplyActivityPlanFilter(
        IQueryable<ActivityPlan> query,
        string[] roles,
        Guid employeeId,
        Guid? departmentId,
        string userId)
    {
        // Manager: See all activity plans in their department
        if (roles.Contains(Roles.Manager) && departmentId.HasValue)
        {
            return query.Where(ap =>
                ap.Employees != null &&
                ap.Employees.DepartmentId == departmentId.Value);
        }

        // User/Viewer: See only their own activity plans
        if (roles.Contains(Roles.User) || roles.Contains(Roles.Viewer))
        {
            return query.Where(ap =>
                ap.EmployeeId == employeeId ||
                ap.CreatedBy == userId);
        }

        return query;
    }

    private IQueryable<Project> ApplyProjectFilter(
        IQueryable<Project> query,
        string[] roles,
        Guid employeeId,
        Guid? departmentId,
        string userId)
    {
        // Manager: See all projects in their department (based on creator's department)
        if (roles.Contains(Roles.Manager) && departmentId.HasValue)
        {
            return query.Where(p =>
                _context.Employees.Any(e =>
                    e.UserId == p.CreatedBy &&
                    e.DepartmentId == departmentId.Value));
        }

        // User/Viewer: See only projects they created
        if (roles.Contains(Roles.User) || roles.Contains(Roles.Viewer))
        {
            return query.Where(p => p.CreatedBy == userId);
        }

        return query;
    }

    private IQueryable<Employee> ApplyEmployeeFilter(
        IQueryable<Employee> query,
        string[] roles,
        Guid employeeId,
        Guid? departmentId)
    {
        // Manager: See all employees in their department
        if (roles.Contains(Roles.Manager) && departmentId.HasValue)
        {
            return query.Where(e => e.DepartmentId == departmentId.Value);
        }

        // User/Viewer: See only themselves
        if (roles.Contains(Roles.User) || roles.Contains(Roles.Viewer))
        {
            return query.Where(e => e.Id == employeeId);
        }

        return query;
    }

    private IQueryable<CheckInCheckOut> ApplyCheckInCheckOutFilter(
        IQueryable<CheckInCheckOut> query,
        string[] roles,
        Guid employeeId,
        Guid? departmentId)
    {
        // Manager: See all check-ins in their department
        if (roles.Contains(Roles.Manager) && departmentId.HasValue)
        {
            return query.Where(c =>
                c.Employees != null &&
                c.Employees.DepartmentId == departmentId.Value);
        }

        // User/Viewer: See only their own check-ins
        if (roles.Contains(Roles.User) || roles.Contains(Roles.Viewer))
        {
            return query.Where(c => c.EmployeeId == employeeId);
        }

        return query;
    }

    private IQueryable<T> ApplyDefaultFilter<T>(
        IQueryable<T> query,
        string[] roles,
        Guid employeeId,
        Guid? departmentId,
        string userId) where T : class
    {
        // For entities with CreatedBy field, filter by creator
        var entityType = typeof(T);
        var hasCreatedBy = entityType.GetProperty("CreatedBy") != null;

        if (hasCreatedBy)
        {
            // Manager: See items created by anyone in their department
            if (roles.Contains(Roles.Manager) && departmentId.HasValue)
            {
                return query.Where(item =>
                    _context.Employees.Any(e =>
                        e.UserId == EF.Property<string>(item, "CreatedBy") &&
                        e.DepartmentId == departmentId.Value));
            }

            // User/Viewer: See only items they created
            if (roles.Contains(Roles.User) || roles.Contains(Roles.Viewer))
            {
                return query.Where(item =>
                    EF.Property<string>(item, "CreatedBy") == userId);
            }
        }

        return query;
    }

    #endregion
}
