import { computed } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { RoleService } from '@/utils/RoleService'

export interface PermissionOptions {
  resourceOwnerId?: string
  resourceDepartmentId?: string
  currentUserId?: string
  currentUserDepartmentId?: string
}

export function usePermissions() {
  const authStore = useAuthStore()
  const roleService = new RoleService()

  // Basic role checks
  const isAdmin = computed(() => roleService.isAdmin(authStore.roles))
  const isManager = computed(() => roleService.isManager(authStore.roles))
  const isUser = computed(() => roleService.isUser(authStore.roles))
  const isViewer = computed(() => roleService.isViewer(authStore.roles))

  // Permission checks
  const canModifyData = computed(() => roleService.canModifyData(authStore.roles))
  const canAccessMasterData = computed(() => roleService.canAccessMasterData(authStore.roles))
  const canViewDepartmentData = computed(() => roleService.canViewDepartmentData(authStore.roles))

  /**
   * Check if user can create new records
   */
  const canCreate = computed(() => {
    return !isViewer.value
  })

  /**
   * Check if user can edit a specific resource
   * @param options - Resource ownership and department information
   */
  const canEdit = (options: PermissionOptions = {}) => {
    // Viewer cannot edit anything
    if (isViewer.value) {
      return false
    }

    // Admin can edit everything
    if (isAdmin.value) {
      return true
    }

    // Manager can edit within their department
    if (isManager.value) {
      if (options.resourceDepartmentId && options.currentUserDepartmentId) {
        return options.resourceDepartmentId === options.currentUserDepartmentId
      }
      return true // If no department info, allow (will be filtered by backend)
    }

    // User can only edit their own data
    if (isUser.value) {
      if (options.resourceOwnerId && options.currentUserId) {
        return options.resourceOwnerId === options.currentUserId
      }
      return false
    }

    return false
  }

  /**
   * Check if user can delete a specific resource
   * @param options - Resource ownership and department information
   */
  const canDelete = (options: PermissionOptions = {}) => {
    // Same logic as edit for now
    return canEdit(options)
  }

  /**
   * Check if user can view a specific resource
   * @param options - Resource ownership and department information
   */
  const canView = (options: PermissionOptions = {}) => {
    // Admin can view everything
    if (isAdmin.value) {
      return true
    }

    // Manager can view within their department
    if (isManager.value) {
      if (options.resourceDepartmentId && options.currentUserDepartmentId) {
        return options.resourceDepartmentId === options.currentUserDepartmentId
      }
      return true
    }

    // User and Viewer can only view their own data
    if (isUser.value || isViewer.value) {
      if (options.resourceOwnerId && options.currentUserId) {
        return options.resourceOwnerId === options.currentUserId
      }
      return false
    }

    return false
  }

  /**
   * Get permission error message based on role
   */
  const getPermissionErrorMessage = () => {
    if (isViewer.value) {
      return 'คุณไม่มีสิทธิ์ดำเนินการนี้ (บัญชีของคุณเป็นแบบอ่านอย่างเดียว)'
    }
    if (isUser.value) {
      return 'คุณไม่มีสิทธิ์แก้ไขข้อมูลของผู้อื่น'
    }
    if (isManager.value) {
      return 'คุณไม่มีสิทธิ์แก้ไขข้อมูลของแผนกอื่น'
    }
    return 'คุณไม่มีสิทธิ์ดำเนินการนี้'
  }

  /**
   * Show permission denied message
   */
  const showPermissionDenied = () => {
    // You can use your SweetAlert store here
    const message = getPermissionErrorMessage()
    console.warn('Permission denied:', message)
    return message
  }

  return {
    // Role checks
    isAdmin,
    isManager,
    isUser,
    isViewer,
    
    // Permission checks
    canModifyData,
    canAccessMasterData,
    canViewDepartmentData,
    canCreate,
    canEdit,
    canDelete,
    canView,
    
    // Utilities
    getPermissionErrorMessage,
    showPermissionDenied,
  }
}
