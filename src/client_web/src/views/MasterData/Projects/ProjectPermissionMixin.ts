/**
 * Project Permission Mixin
 * 
 * This mixin provides permission checking methods for Project components
 * to determine if the current user can create, edit, or delete projects.
 */

import { useAuthStore } from '@/stores/auth'
import { RoleService } from '@/utils/RoleService'
import { useSweetAlertStore } from '@/stores'

const roleService = new RoleService()

export function useProjectPermissions() {
  const auth = useAuthStore()
  const sweetAlert = useSweetAlertStore()

  /**
   * Check if user can create new projects
   * Viewer role cannot create
   */
  const canCreateProject = (): boolean => {
    return roleService.canCreate(auth.roles)
  }

  /**
   * Check if user can edit a specific project
   * @param project - The project object with owner/department info
   */
  const canEditProject = (project: any): boolean => {
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
      // Check if project belongs to manager's department
      // This might need to check project contacts or other relationships
      return true // For now, allow managers to edit (backend will filter)
    }

    // User can only edit projects they're involved in
    if (roleService.isUser(auth.roles)) {
      // Check if user is a contact on the project
      // This would require checking project contacts
      return false // For now, restrict (backend will filter)
    }

    return false
  }

  /**
   * Check if user can delete a specific project
   * Same logic as edit
   */
  const canDeleteProject = (project: any): boolean => {
    return canEditProject(project)
  }

  /**
   * Show permission denied message based on user role
   */
  const showPermissionDenied = (): void => {
    const message = roleService.getPermissionMessage(auth.roles)
    sweetAlert.warning(message)
  }

  /**
   * Check if user is in read-only mode (Viewer role)
   */
  const isReadOnly = (): boolean => {
    return roleService.isViewer(auth.roles)
  }

  return {
    canCreateProject,
    canEditProject,
    canDeleteProject,
    showPermissionDenied,
    isReadOnly,
  }
}
