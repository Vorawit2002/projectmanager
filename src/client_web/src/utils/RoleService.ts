import { NATMember, NATEmployee_Admin,NATEmployee_ServiceGroup_Admin,NATEmployee_DocumentManagement_Admin,NATEmployee_DocumentPreservation_Admin,NATEmployee_EventKeeper_Admin,NATEmployee_ArchiveAccount_Admin,NATEmployee_Finance_Admin}  from "@/utils/ApplicationRoles";

export class RoleService {

  isNATEmployee(roles: string[]): boolean {
    let targetRoles = [NATEmployee_Admin,NATEmployee_ServiceGroup_Admin,NATEmployee_DocumentManagement_Admin,NATEmployee_DocumentPreservation_Admin,NATEmployee_EventKeeper_Admin,NATEmployee_ArchiveAccount_Admin,NATEmployee_Finance_Admin]
    return roles.some(role => targetRoles.includes(role))
  }

  isMember(roles: string[]): boolean {
    return roles.includes(NATMember)
  }
}



