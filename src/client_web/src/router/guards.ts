import type { RouteLocationNormalized, NavigationGuardNext } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

/**
 * Role-based authorization guard
 * Checks if the user has the required roles to access a route
 * 
 * @param to - Target route
 * @param from - Current route
 * @param next - Navigation callback
 */
export function roleGuard(
  to: RouteLocationNormalized,
  from: RouteLocationNormalized,
  next: NavigationGuardNext
) {
  const authStore = useAuthStore()
  
  // Get required roles from route meta
  const requiredRoles = to.meta.roles as string[] | undefined
  
  // If no roles are required, allow access
  if (!requiredRoles || requiredRoles.length === 0) {
    next()
    return
  }
  
  // Check if user has at least one of the required roles
  const hasRequiredRole = requiredRoles.some(role => 
    authStore.roles.includes(role)
  )
  
  if (hasRequiredRole) {
    // User has required role, allow access
    next()
  } else {
    // User doesn't have required role, redirect to not-authorized
    console.warn(`Access denied to ${to.path}. Required roles: ${requiredRoles.join(', ')}. User roles: ${authStore.roles.join(', ')}`)
    next('/not-authorized')
  }
}

/**
 * Helper function to check if user has any of the specified roles
 * Can be used in components or other parts of the application
 * 
 * @param roles - Array of role names to check
 * @returns true if user has at least one of the specified roles
 */
export function hasRole(...roles: string[]): boolean {
  const authStore = useAuthStore()
  return roles.some(role => authStore.roles.includes(role))
}

/**
 * Helper function to check if user has all of the specified roles
 * 
 * @param roles - Array of role names to check
 * @returns true if user has all of the specified roles
 */
export function hasAllRoles(...roles: string[]): boolean {
  const authStore = useAuthStore()
  return roles.every(role => authStore.roles.includes(role))
}
