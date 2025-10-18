# Permission Controls - Quick Reference

## Overview
This system implements role-based action permission controls for the frontend application.

## Roles
- **Admin**: Full access to everything
- **Manager**: Access to Master Data and department-level data
- **User**: Access to own data only
- **Viewer**: Read-only access to own data

## Quick Start

### 1. Import the Composable
```typescript
import { usePermissions } from '@/composables/usePermissions'

const permissions = usePermissions()
```

### 2. Check Permissions
```typescript
// Check if user can create
if (permissions.canCreate.value) {
  // Show create button
}

// Check if user can edit specific item
if (permissions.canEdit({
  resourceOwnerId: item.employeeId,
  currentUserId: auth.employeeId,
  resourceDepartmentId: item.departmentId,
  currentUserDepartmentId: auth.departmentId
})) {
  // Show edit button
}
```

### 3. Use in Template
```vue
<template>
  <!-- Create button - hidden for Viewer -->
  <v-btn v-if="permissions.canCreate.value" @click="handleCreate">
    สร้างใหม่
  </v-btn>

  <!-- Edit button - conditional -->
  <v-btn v-if="canEditItem(item)" @click="handleEdit(item)">
    แก้ไข
  </v-btn>
</template>
```

## Available Tools

### 1. usePermissions Composable
Location: `src/composables/usePermissions.ts`

**Role Checks:**
- `isAdmin.value`
- `isManager.value`
- `isUser.value`
- `isViewer.value`

**Permission Checks:**
- `canCreate.value`
- `canEdit(options)`
- `canDelete(options)`
- `canView(options)`

**Utilities:**
- `getPermissionErrorMessage()`
- `showPermissionDenied()`

### 2. PermissionButton Component
Location: `src/components/PermissionButton.vue`

```vue
<PermissionButton
  action="edit"
  :permission-options="{ resourceOwnerId, currentUserId }"
  @click="handleEdit"
>
  แก้ไข
</PermissionButton>
```

### 3. RoleService Utility
Location: `src/utils/RoleService.ts`

```typescript
import { RoleService } from '@/utils/RoleService'

const roleService = new RoleService()
roleService.canCreate(roles)
roleService.canEdit(roles, resourceOwnerId, currentUserId)
```

## Permission Matrix

| Action | Admin | Manager | User | Viewer |
|--------|-------|---------|------|--------|
| Create | ✅ All | ✅ All | ✅ Own | ❌ None |
| Edit | ✅ All | ✅ Dept | ✅ Own | ❌ None |
| Delete | ✅ All | ✅ Dept | ✅ Own | ❌ None |
| View | ✅ All | ✅ Dept | ✅ Own | ✅ Own |
| Master Data | ✅ Yes | ✅ Yes | ❌ No | ❌ No |

## Common Patterns

### Pattern 1: Hide Create Button for Viewer
```vue
<v-btn v-if="permissions.canCreate.value" @click="handleCreate">
  สร้างใหม่
</v-btn>
```

### Pattern 2: Conditional Edit Button
```vue
<v-btn
  v-if="canEditItem(item)"
  @click="handleEdit(item)"
>
  แก้ไข
</v-btn>

<script setup>
const canEditItem = (item) => {
  return permissions.canEdit({
    resourceOwnerId: item.employeeId,
    currentUserId: auth.employeeId,
    resourceDepartmentId: item.departmentId,
    currentUserDepartmentId: auth.departmentId
  })
}
</script>
```

### Pattern 3: Show Permission Denied Message
```typescript
const handleEdit = (item) => {
  if (!canEditItem(item)) {
    sweetAlert.warning(permissions.getPermissionErrorMessage())
    return
  }
  // Proceed with edit
}
```

### Pattern 4: Master Data Access Check
```typescript
onMounted(() => {
  if (!permissions.canAccessMasterData.value) {
    sweetAlert.error('คุณไม่มีสิทธิ์เข้าถึงข้อมูลหลัก (Master Data)')
    router.push('/Homepage')
  }
})
```

## Error Messages

- **Viewer**: "คุณไม่มีสิทธิ์ดำเนินการนี้ (บัญชีของคุณเป็นแบบอ่านอย่างเดียว)"
- **User**: "คุณไม่มีสิทธิ์แก้ไขข้อมูลของผู้อื่น"
- **Manager**: "คุณไม่มีสิทธิ์แก้ไขข้อมูลของแผนกอื่น"

## Files Created

1. `src/composables/usePermissions.ts` - Main composable
2. `src/components/PermissionButton.vue` - Reusable button component
3. `src/views/AppointmentPlan/ActivityPlanPermissionMixin.ts` - Activity plan permissions
4. `src/views/MasterData/Projects/ProjectPermissionMixin.ts` - Project permissions
5. `src/docs/PERMISSION_CONTROLS_GUIDE.md` - Comprehensive guide
6. `src/docs/PERMISSION_EXAMPLE_COMPONENT.vue` - Example component

## Files Updated

1. `src/utils/RoleService.ts` - Added permission methods
2. `src/stores/auth.ts` - Added employeeId and departmentId fields

## Testing

See the testing checklist in the verification document:
`.kiro/specs/role-management-and-profile/task-20-verification.md`

## Documentation

For detailed documentation, see:
- `src/docs/PERMISSION_CONTROLS_GUIDE.md` - Full implementation guide
- `src/docs/PERMISSION_EXAMPLE_COMPONENT.vue` - Complete example
- `.kiro/specs/role-management-and-profile/task-20-verification.md` - Verification document

## Support

For questions or issues, refer to the comprehensive guide or the example component.
