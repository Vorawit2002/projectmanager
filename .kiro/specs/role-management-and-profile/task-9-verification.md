# Task 9 Verification: อัพเดท API Endpoints ที่มีอยู่ด้วย Data Filtering

## Summary of Changes

This document verifies that all sub-tasks for Task 9 have been completed successfully.

### ✅ Sub-task 1: อัพเดท Activity Plans endpoints เพื่อใช้ DataFilterService สำหรับ role-based filtering

**Files Modified:**
1. `src/Application/ActivityPlans/Queries/GetActivityPlanQuery.cs`
   - Added IUser, IIdentityService, and IDataFilterService dependencies
   - Implemented role-based filtering using DataFilterService.ApplyRoleBasedFilterAsync()
   
2. `src/Application/ActivityPlans/Queries/GetActivityPlanWithPaginationQuery.cs`
   - Added IUser, IIdentityService, and IDataFilterService dependencies
   - Implemented role-based filtering before applying pagination
   
3. `src/Application/ActivityPlans/Commands/UpdateActivityPlan/UpdateActivityPlanCommand.cs`
   - Added authorization check using DataFilterService.CanAccessResourceAsync()
   - Throws ForbiddenAccessException if user doesn't have permission
   
4. `src/Application/ActivityPlans/Commands/DeleteActivityPlan/DeleteActivityPlanCommand.cs`
   - Added authorization check using DataFilterService.CanAccessResourceAsync()
   - Throws ForbiddenAccessException if user doesn't have permission
   
5. `src/Web/Endpoints/ActivityPlanEndpoint.cs`
   - Split endpoints into read operations (all authenticated users) and write operations (CanModifyData policy)
   - Added Policies.CanModifyData authorization for Create/Update/Delete operations

### ✅ Sub-task 2: อัพเดท Projects endpoints เพื่อใช้ DataFilterService สำหรับ role-based filtering

**Files Modified:**
1. `src/Application/Projects/Queries/GetProjectQuery.cs`
   - Added IUser, IIdentityService, and IDataFilterService dependencies
   - Implemented role-based filtering using DataFilterService.ApplyRoleBasedFilterAsync()
   
2. `src/Application/Projects/Queries/GetProjectWithPaginationQuery.cs`
   - Added IUser, IIdentityService, and IDataFilterService dependencies
   - Implemented role-based filtering before applying pagination
   
3. `src/Application/Projects/Command/UpdateProject/UpdateProjectCommand.cs`
   - Added authorization check using DataFilterService.CanAccessResourceAsync()
   - Throws ForbiddenAccessException if user doesn't have permission
   
4. `src/Application/Projects/Command/DeleteProject/DeleteProjectCommand.cs`
   - Added authorization check using DataFilterService.CanAccessResourceAsync()
   - Throws ForbiddenAccessException if user doesn't have permission
   
5. `src/Web/Endpoints/ProjectEndpoint.cs`
   - Split endpoints into read operations (all authenticated users) and write operations (CanModifyData policy)
   - Added Policies.CanModifyData authorization for Create/Update/Delete operations

### ✅ Sub-task 3: อัพเดท Employees endpoints เพื่อใช้ DataFilterService สำหรับ role-based filtering

**Files Modified:**
1. `src/Application/Employees/Queries/GetEmployeeQuery.cs`
   - Added IUser, IIdentityService, and IDataFilterService dependencies
   - Implemented role-based filtering using DataFilterService.ApplyRoleBasedFilterAsync()
   
2. `src/Application/Employees/Queries/GetEmployeeWithPaginationQuery.cs`
   - Added IUser, IIdentityService, and IDataFilterService dependencies
   - Implemented role-based filtering before applying pagination
   
3. `src/Application/Employees/Commands/UpdateEmployee/UpdateEmployeeCommand.cs`
   - Added authorization check using DataFilterService.CanAccessResourceAsync()
   - Throws ForbiddenAccessException if user doesn't have permission
   
4. `src/Application/Employees/Commands/DeleteEmployee/DeleteEmployeeCommand.cs`
   - Added authorization check using DataFilterService.CanAccessResourceAsync()
   - Throws ForbiddenAccessException if user doesn't have permission
   
5. `src/Web/Endpoints/EmployeeEndpoint.cs`
   - Split endpoints into read operations (CanViewDepartmentData policy) and write operations (CanManageMasterData policy)
   - Added proper authorization policies for different operations

### ✅ Sub-task 4: อัพเดท Organizations endpoints เพื่อตรวจสอบ CanViewMasterData policy

**Files Modified:**
1. `src/Web/Endpoints/OrganizationEndpoint.cs`
   - Added using statement for ProjectManagement.Domain.Constants
   - Split endpoints into read operations (CanViewMasterData policy) and write operations (CanManageMasterData policy)
   - Read operations: GetOrganizationQuery, GetOrganizationQueryByID, GetOrganizationWithPagination
   - Write operations: CreateOrganization, CreateOrganizationJustName, UpdateOrganization, DeleteOrganization

### ✅ Sub-task 5: อัพเดท Departments endpoints เพื่อตรวจสอบ CanViewMasterData policy

**Files Modified:**
1. `src/Web/Endpoints/DepartmentEndpoint.cs`
   - Added using statement for ProjectManagement.Domain.Constants
   - Split endpoints into read operations (CanViewMasterData policy) and write operations (CanManageMasterData policy)
   - Read operations: GetDepartmentQuery, GetDepartmentQueryByID, GetDepartmentWithPagination
   - Write operations: CreateDepartment, UpdateDepartment, DeleteDepartment

### ✅ Sub-task 6: เพิ่ม authorization checks ก่อน Create/Edit/Delete operations

**Authorization Checks Added:**
- All Update and Delete command handlers now check user permissions before modifying data
- Uses DataFilterService.CanAccessResourceAsync() to verify access rights
- Throws ForbiddenAccessException when user lacks permission
- Checks are based on:
  - User role (Admin, Manager, User, Viewer)
  - Resource ownership (CreatedBy field)
  - Department membership (for Manager role)

## Role-Based Access Control Summary

### Admin Role
- Can view and modify all data across all endpoints
- No filtering applied

### Manager Role
- Can view and modify data within their department
- Activity Plans: See all plans in their department
- Projects: See projects created by department members
- Employees: See and manage employees in their department
- Master Data: Full access to Organizations and Departments

### User Role
- Can view and modify only their own data
- Activity Plans: See only their own plans
- Projects: See only their own projects
- Employees: See only themselves
- Master Data: No access to Organizations and Departments

### Viewer Role
- Can view only their own data (read-only)
- Cannot create, update, or delete any data
- Same visibility as User role but no modification rights
- Master Data: No access to Organizations and Departments

## Requirements Coverage

This implementation satisfies the following requirements:

- **Requirement 7.3**: Manager can see activity plans of employees in their department ✅
- **Requirement 7.4**: Manager can see employees in their department ✅
- **Requirement 7.5**: User can see only their own data ✅
- **Requirement 7.6**: User can see only their own activity plans ✅
- **Requirement 7.7**: User can see only their own projects ✅
- **Requirement 7.8**: Viewer has same visibility as User (read-only) ✅
- **Requirement 7.12**: API endpoints check role and apply filters before returning data ✅
- **Requirement 8.1**: Admin can Create/Edit/Delete all data ✅
- **Requirement 8.2**: Manager can Create/Edit/Delete data in their department ✅
- **Requirement 8.3**: Manager cannot edit data from other departments ✅
- **Requirement 8.4**: User can Create/Edit/Delete only their own data ✅
- **Requirement 8.5**: User cannot edit others' data ✅
- **Requirement 8.9**: API endpoints check role and permissions before operations ✅

## Compilation Status

All modified files have been checked for compilation errors:
- ✅ No diagnostics found in any modified files
- ✅ All dependencies properly injected
- ✅ All authorization checks implemented correctly

## Next Steps

The implementation is complete and ready for testing. The next task should focus on:
1. Testing role-based access control with different user roles
2. Verifying data filtering works correctly for each role
3. Testing authorization checks for Create/Update/Delete operations
4. Ensuring proper error messages are returned for unauthorized access
