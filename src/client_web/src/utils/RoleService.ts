import { NATMember, NATEmployee_Admin,NATEmployee_ServiceGroup_Admin,NATEmployee_DocumentManagement_Admin,NATEmployee_DocumentPreservation_Admin,NATEmployee_EventKeeper_Admin,NATEmployee_ArchiveAccount_Admin,NATEmployee_Finance_Admin}  from "@/utils/ApplicationRoles";

export class RoleService {
  // New role constants for role management feature
  static readonly ADMIN = 'Admin'
  static readonly MANAGER = 'Manager'
  static readonly USER = 'User'
  static readonly VIEWER = 'Viewer'

  // Existing methods
  isNATEmployee(roles: string[]): boolean {
    let targetRoles = [NATEmployee_Admin,NATEmployee_ServiceGroup_Admin,NATEmployee_DocumentManagement_Admin,NATEmployee_DocumentPreservation_Admin,NATEmployee_EventKeeper_Admin,NATEmployee_ArchiveAccount_Admin,NATEmployee_Finance_Admin]
    return roles.some(role => targetRoles.includes(role))
  }

  isMember(roles: string[]): boolean {
    return roles.includes(NATMember)
  }

  // New role checking methods
  isAdmin(roles: string[]): boolean {
    return roles.includes(RoleService.ADMIN)
  }

  isManager(roles: string[]): boolean {
    return roles.includes(RoleService.MANAGER)
  }

  isUser(roles: string[]): boolean {
    return roles.includes(RoleService.USER)
  }

  isViewer(roles: string[]): boolean {
    return roles.includes(RoleService.VIEWER)
  }

  // Permission checking methods
  canAccessMasterData(roles: string[]): boolean {
    return this.isAdmin(roles) || this.isManager(roles)
  }

  canModifyData(roles: string[]): boolean {
    return !this.isViewer(roles)
  }

  canViewDepartmentData(roles: string[]): boolean {
    return this.isAdmin(roles) || this.isManager(roles)
  }

  // Action permission methods
  canCreate(roles: string[]): boolean {
    return !this.isViewer(roles)
  }

  canEdit(roles: string[], resourceOwnerId?: string, currentUserId?: string, resourceDepartmentId?: string, currentUserDepartmentId?: string): boolean {
    // Viewer cannot edit
    if (this.isViewer(roles)) {
      return false
    }

    // Admin can edit everything
    if (this.isAdmin(roles)) {
      return true
    }

    // Manager can edit within their department
    if (this.isManager(roles)) {
      if (resourceDepartmentId && currentUserDepartmentId) {
        return resourceDepartmentId === currentUserDepartmentId
      }
      return true
    }

    // User can only edit their own data
    if (this.isUser(roles)) {
      if (resourceOwnerId && currentUserId) {
        return resourceOwnerId === currentUserId
      }
      return false
    }

    return false
  }

  canDelete(roles: string[], resourceOwnerId?: string, currentUserId?: string, resourceDepartmentId?: string, currentUserDepartmentId?: string): boolean {
    // Same logic as edit
    return this.canEdit(roles, resourceOwnerId, currentUserId, resourceDepartmentId, currentUserDepartmentId)
  }

  getPermissionMessage(roles: string[]): string {
    if (this.isViewer(roles)) {
      return 'คุณไม่มีสิทธิ์ดำเนินการนี้ (บัญชีของคุณเป็นแบบอ่านอย่างเดียว)'
    }
    if (this.isUser(roles)) {
      return 'คุณไม่มีสิทธิ์แก้ไขข้อมูลของผู้อื่น'
    }
    if (this.isManager(roles)) {
      return 'คุณไม่มีสิทธิ์แก้ไขข้อมูลของแผนกอื่น'
    }
    return 'คุณไม่มีสิทธิ์ดำเนินการนี้'
  }
}



