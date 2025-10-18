# Route Guards Documentation

## Overview

This directory contains route guard implementations for role-based access control in the application.

## Files

### guards.ts

Contains the main route guard functions for role-based authorization.

#### Functions

##### `roleGuard(to, from, next)`

Main role-based authorization guard that checks if the user has the required roles to access a route.

**Usage in router:**
```typescript
import { roleGuard } from '@/router/guards'

router.beforeEach(async (to, from, next) => {
  // ... authentication checks ...
  
  // Check role-based access after authentication
  roleGuard(to, from, next)
})
```

**Usage in route meta:**
```typescript
{
  path: 'MasterData/UserListView',
  name: 'UserListView',
  component: UserListView,
  meta: {
    requiresAuth: true,
    roles: ['Admin'] // Only Admin can access
  }
}
```

##### `hasRole(...roles: string[]): boolean`

Helper function to check if the current user has any of the specified roles.

**Usage:**
```typescript
import { hasRole } from '@/router/guards'

if (hasRole('Admin', 'Manager')) {
  // User is either Admin or Manager
}
```

##### `hasAllRoles(...roles: string[]): boolean`

Helper function to check if the current user has all of the specified roles.

**Usage:**
```typescript
import { hasAllRoles } from '@/router/guards'

if (hasAllRoles('Admin', 'Manager')) {
  // User has both Admin AND Manager roles
}
```

## Role Definitions

The application uses the following roles:

- **Admin**: Full system access, can manage users and all master data
- **Manager**: Can access master data and view department-level data
- **User**: Can access activity plans, projects, and own data
- **Viewer**: Read-only access to activity plans, projects, and own data

## Route Configuration Examples

### Admin-Only Route
```typescript
{
  path: 'MasterData/UserListView',
  name: 'UserListView',
  component: UserListView,
  meta: {
    requiresAuth: true,
    roles: ['Admin']
  }
}
```

### Admin and Manager Route
```typescript
{
  path: 'MasterData/OrganizationListView',
  name: 'OrganizationListView',
  component: OrganizationListView,
  meta: {
    requiresAuth: true,
    roles: ['Admin', 'Manager']
  }
}
```

### All Authenticated Users
```typescript
{
  path: 'activity-plans',
  name: 'ActivityPlans',
  component: ActivityPlans,
  meta: {
    requiresAuth: true
    // No roles specified = all authenticated users can access
  }
}
```

## Not Authorized Page

When a user attempts to access a route without the required role, they are redirected to `/not-authorized`.

The not-authorized page displays:
- An error icon
- A message explaining they don't have access
- A button to return to the dashboard

## Integration with Auth Store

The route guards integrate with the auth store to:
1. Check if the user is authenticated
2. Retrieve the user's current roles
3. Validate role requirements

## Testing Role-Based Access

To test role-based access:

1. **Admin Access**
   - Login as admin
   - Navigate to `/MasterData/UserListView` - Should succeed
   - Navigate to `/MasterData/OrganizationListView` - Should succeed

2. **Manager Access**
   - Login as manager
   - Navigate to `/MasterData/OrganizationListView` - Should succeed
   - Navigate to `/MasterData/UserListView` - Should redirect to `/not-authorized`

3. **User/Viewer Access**
   - Login as user or viewer
   - Navigate to `/MasterData/OrganizationListView` - Should redirect to `/not-authorized`
   - Navigate to `/MasterData/UserListView` - Should redirect to `/not-authorized`

## Security Considerations

1. **Client-Side Only**: These guards provide UI-level protection only. Backend API endpoints must also validate roles.

2. **Token Validation**: The router checks token expiration before applying role guards.

3. **Session Restoration**: If the user is not logged in but has a valid token, the session is restored before checking roles.

4. **Public Routes**: The following routes bypass role checks:
   - `/login`
   - `/register`
   - `/report/:id` (shared reports)
   - `/not-authorized`

## Future Enhancements

- Add permission-based guards (more granular than roles)
- Add resource-based authorization (e.g., can edit own resources)
- Add audit logging for unauthorized access attempts
- Add dynamic role loading from backend
