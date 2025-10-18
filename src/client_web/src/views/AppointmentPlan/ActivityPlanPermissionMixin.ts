/**
 * Activity Plan Permission Mixin
 * 
 * This mixin provides permission checking methods for Activity Plan components
 * to determine if the current user can create, edit, or delete activity plans.
 */

import { useAuthStore } from '@/stores/auth'
import { RoleService } from '@/utils/RoleService'
import { useSweetAlertStore } from '@/stores'

const roleService = new RoleService()

export function useActivityPlanPermissions() {
  const auth = useAuthStore()
  const sweetAlert = useSweetAlertStore()

  /**
   * Check if user can create new activity plans
   * Viewer role cannot create
   */
  const canCreateActivityPlan = (): boolean => {
    return roleService.canCreate(auth.roles)
  }

  /**
   * Check if user can edit a specific activity plan
   * @param activityPlan - The activity plan object with employeeId and department info
   */
  const canEditActivityPlan = (activityPlan: any): boolean => {
    // Viewer cannot edit
    if (roleService.isViewer(auth.roles)) {
      return false
    }

    // Admin can edit everything
    if (roleService.isAdmin(auth.roles)) {
      return true
    }

    // Manager can edit within their department
    if (roleService.isManager(auth.roles)) {
      const resourceDepartmentId = activityPlan.employees?.departmentId || activityPlan.departmentId
      if (resourceDepartmentId && auth.departmentId) {
        return resourceDepartmentId === auth.departmentId
      }
      return true // If no department info, allow (backend will filter)
    }

    // User can only edit their own activity plans
    if (roleService.isUser(auth.roles)) {
      const resourceOwnerId = activityPlan.employeeId
      if (resourceOwnerId && auth.employeeId) {
        return resourceOwnerId === auth.employeeId
      }
      return false
    }

    return false
  }

  /**
   * Check if user can delete a specific activity plan
   * Same logic as edit
   */
  const canDeleteActivityPlan = (activityPlan: any): boolean => {
    return canEditActivityPlan(activityPlan)
  }

  /**
   * Show permission denied message based on user role
   */
  const showPermissionDenied = (): void => {
    const message = roleService.getPermissionMessage(auth.roles)
    sweetAlert.warning(message)
  }

  /**
   * Check if user can view activity plan details
   * All roles can view their accessible data
   */
  const canViewActivityPlan = (activityPlan: any): boolean => {
    // Admin can view everything
    if (roleService.isAdmin(auth.roles)) {
      return true
    }

    // Manager can view within their department
    if (roleService.isManager(auth.roles)) {
      const resourceDepartmentId = activityPlan.employees?.departmentId || activityPlan.departmentId
      if (resourceDepartmentId && auth.departmentId) {
        return resourceDepartmentId === auth.departmentId
      }
      return true
    }

    // User and Viewer can only view their own activity plans
    const resourceOwnerId = activityPlan.employeeId
    if (resourceOwnerId && auth.employeeId) {
      return resourceOwnerId === auth.employeeId
    }

    return false
  }

  /**
   * Check if user is in read-only mode (Viewer role)
   */
  const isReadOnly = (): boolean => {
    return roleService.isViewer(auth.roles)
  }

  return {
    canCreateActivityPlan,
    canEditActivityPlan,
    canDeleteActivityPlan,
    canViewActivityPlan,
    showPermissionDenied,
    isReadOnly,
  }
}
