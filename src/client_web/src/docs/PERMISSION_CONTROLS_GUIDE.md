# Permission Controls Implementation Guide

This guide explains how to implement role-based permission controls in frontend components.

## Overview

The permission system supports 4 roles:
- **Admin**: Full access to all features and data
- **Manager**: Access to Master Data and department-level data
- **User**: Access to own data only, can create/edit/delete own records
- **Viewer**: Read-only access to own data

## Available Tools

### 1. usePermissions Composable

Located at: `src/composables/usePermissions.ts`

```typescript
import { usePermissions } from '@/composables/usePermissions'

const permissions = usePermissions()

// Role checks
permissions.isAdmin.value
permissions.isManager.value
permissions.isUser.value
permissions.isViewer.value

// Permission checks
permissions.canCreate.value
permissions.canEdit({ resourceOwnerId, currentUserId, resourceDepartmentId, currentUserDepartmentId })
permissions.canDelete({ resourceOwnerId, currentUserId, resourceDepartmentId, currentUserDepartmentId })
permissions.canView({ resourceOwnerId, currentUserId, resourceDepartmentId, currentUserDepartmentId })

// Utilities
permissions.getPermissionErrorMessage()
permissions.showPermissionDenied()
```

### 2. PermissionButton Component

Located at: `src/components/PermissionButton.vue`

```vue
<PermissionButton
  action="create"
  color="success"
  @click="handleCreate"
>
  <v-icon class="mr-2">ri-add-circle-line</v-icon>
  สร้างใหม่
</PermissionButton>

<PermissionButton
  action="edit"
  :permission-options="{
    resourceOwnerId: item.employeeId,
    currentUserId: auth.employeeId,
    resourceDepartmentId: item.departmentId,
    currentUserDepartmentId: auth.departmentId
  }"
  color="warning"
  icon
  @click="handleEdit(item)"
>
  <v-icon>ri-edit-2-line</v-icon>
</PermissionButton>
```

### 3. RoleService Utility

Located at: `src/utils/RoleService.ts`

```typescript
import { RoleService } from '@/utils/RoleService'

const roleService = new RoleService()

// Role checks
roleService.isAdmin(roles)
roleService.isManager(roles)
roleService.isUser(roles)
roleService.isViewer(roles)

// Permission checks
roleService.canCreate(roles)
roleService.canEdit(roles, resourceOwnerId, currentUserId, resourceDepartmentId, currentUserDepartmentId)
roleService.canDelete(roles, resourceOwnerId, currentUserId, resourceDepartmentId, currentUserDepartmentId)
roleService.canAccessMasterData(roles)
roleService.canModifyData(roles)
roleService.canViewDepartmentData(roles)

// Get error message
roleService.getPermissionMessage(roles)
```

## Implementation Patterns

### Pattern 1: Hide/Show Create Button

```vue
<template>
  <v-btn
    v-if="canCreate"
    @click="handleCreate"
  >
    <v-icon class="mr-2">ri-add-circle-line</v-icon>
    สร้างใหม่
  </v-btn>
</template>

<script setup lang="ts">
import { usePermissions } from '@/composables/usePermissions'

const permissions = usePermissions()
const canCreate = permissions.canCreate
</script>
```

### Pattern 2: Conditional Edit/Delete Buttons in Table

```vue
<template>
  <v-btn
    v-if="canEditItem(item)"
    @click="handleEdit(item)"
    color="warning"
    icon
  >
    <v-icon>ri-edit-2-line</v-icon>
  </v-btn>
  
  <v-btn
    v-if="canDeleteItem(item)"
    @click="handleDelete(item)"
    color="error"
    icon
  >
    <v-icon>ri-delete-bin-6-line</v-icon>
  </v-btn>
</template>

<script setup lang="ts">
import { usePermissions } from '@/composables/usePermissions'
import { useAuthStore } from '@/stores/auth'

const permissions = usePermissions()
const auth = useAuthStore()

const canEditItem = (item: any) => {
  return permissions.canEdit({
    resourceOwnerId: item.employeeId,
    currentUserId: auth.employeeId,
    resourceDepartmentId: item.departmentId,
    currentUserDepartmentId: auth.departmentId
  })
}

const canDeleteItem = (item: any) => {
  return permissions.canDelete({
    resourceOwnerId: item.employeeId,
    currentUserId: auth.employeeId,
    resourceDepartmentId: item.departmentId,
    currentUserDepartmentId: auth.departmentId
  })
}
</script>
```

### Pattern 3: Show Permission Denied Message

```vue
<script setup lang="ts">
import { usePermissions } from '@/composables/usePermissions'
import { useSweetAlertStore } from '@/stores'

const permissions = usePermissions()
const sweetAlert = useSweetAlertStore()

const handleEdit = (item: any) => {
  const canEdit = permissions.canEdit({
    resourceOwnerId: item.employeeId,
    currentUserId: auth.employeeId,
    resourceDepartmentId: item.departmentId,
    currentUserDepartmentId: auth.departmentId
  })
  
  if (!canEdit) {
    sweetAlert.warning(permissions.getPermissionErrorMessage())
    return
  }
  
  // Proceed with edit
  openEditDialog(item)
}
</script>
```

### Pattern 4: Read-Only Mode for Viewer

```vue
<template>
  <v-text-field
    v-model="formData.name"
    :readonly="isReadOnly"
    label="ชื่อ"
  />
  
  <v-btn
    v-if="!isReadOnly"
    @click="handleSave"
  >
    บันทึก
  </v-btn>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { usePermissions } from '@/composables/usePermissions'

const permissions = usePermissions()
const isReadOnly = computed(() => permissions.isViewer.value)
</script>
```

### Pattern 5: Master Data Access Check

```vue
<script setup lang="ts">
import { onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { usePermissions } from '@/composables/usePermissions'
import { useSweetAlertStore } from '@/stores'

const router = useRouter()
const permissions = usePermissions()
const sweetAlert = useSweetAlertStore()

onMounted(() => {
  // Check if user has access to Master Data
  if (!permissions.canAccessMasterData.value) {
    sweetAlert.error('คุณไม่มีสิทธิ์เข้าถึงข้อมูลหลัก (Master Data)')
    router.push('/Homepage')
  }
})
</script>
```

## Common Scenarios

### Scenario 1: Activity Plans List

**Requirements:**
- Admin: See all activity plans, can create/edit/delete all
- Manager: See department activity plans, can create/edit/delete department plans
- User: See own activity plans, can create/edit/delete own plans
- Viewer: See own activity plans, read-only

**Implementation:**
```vue
<template>
  <!-- Create button - hidden for Viewer -->
  <v-btn
    v-if="canCreate"
    @click="handleCreate"
  >
    สร้างนัดหมายใหม่
  </v-btn>
  
  <!-- Table with conditional actions -->
  <v-data-table :items="activityPlans">
    <template v-slot:item.actions="{ item }">
      <v-btn
        v-if="canEditItem(item)"
        @click="handleEdit(item)"
        icon
      >
        <v-icon>ri-edit-2-line</v-icon>
      </v-btn>
      
      <v-btn
        v-if="canDeleteItem(item)"
        @click="handleDelete(item)"
        icon
      >
        <v-icon>ri-delete-bin-6-line</v-icon>
      </v-btn>
    </template>
  </v-data-table>
</template>

<script setup lang="ts">
import { usePermissions } from '@/composables/usePermissions'
import { useAuthStore } from '@/stores/auth'

const permissions = usePermissions()
const auth = useAuthStore()

const canCreate = permissions.canCreate

const canEditItem = (item: any) => {
  return permissions.canEdit({
    resourceOwnerId: item.employeeId,
    currentUserId: auth.employeeId,
    resourceDepartmentId: item.employees?.departmentId,
    currentUserDepartmentId: auth.departmentId
  })
}

const canDeleteItem = (item: any) => {
  return permissions.canDelete({
    resourceOwnerId: item.employeeId,
    currentUserId: auth.employeeId,
    resourceDepartmentId: item.employees?.departmentId,
    currentUserDepartmentId: auth.departmentId
  })
}
</script>
```

### Scenario 2: Master Data (Organizations, Departments, Event Types)

**Requirements:**
- Admin: Full access
- Manager: Full access
- User: No access
- Viewer: No access

**Implementation:**
```vue
<script setup lang="ts">
import { onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { usePermissions } from '@/composables/usePermissions'
import { useSweetAlertStore } from '@/stores'

const router = useRouter()
const permissions = usePermissions()
const sweetAlert = useSweetAlertStore()

onMounted(() => {
  // Check Master Data access
  if (!permissions.canAccessMasterData.value) {
    sweetAlert.error('คุณไม่มีสิทธิ์เข้าถึงข้อมูลหลัก (Master Data)')
    router.push('/Homepage')
    return
  }
})

// Create button - hidden for Viewer
const canCreate = computed(() => {
  return permissions.canModifyData.value && permissions.canAccessMasterData.value
})

// Edit/Delete buttons - hidden for Viewer
const canModify = computed(() => {
  return permissions.canModifyData.value && permissions.canAccessMasterData.value
})
</script>
```

### Scenario 3: Employee List

**Requirements:**
- Admin: See all employees, can edit all
- Manager: See department employees, can edit department employees
- User: No access
- Viewer: No access

**Implementation:**
```vue
<script setup lang="ts">
import { onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { usePermissions } from '@/composables/usePermissions'
import { useAuthStore } from '@/stores'
import { useSweetAlertStore } from '@/stores'

const router = useRouter()
const permissions = usePermissions()
const auth = useAuthStore()
const sweetAlert = useSweetAlertStore()

onMounted(() => {
  // Only Admin and Manager can access employee list
  if (!permissions.canViewDepartmentData.value) {
    sweetAlert.error('คุณไม่มีสิทธิ์เข้าถึงข้อมูลพนักงาน')
    router.push('/Homepage')
    return
  }
})

const canEditEmployee = (employee: any) => {
  return permissions.canEdit({
    resourceDepartmentId: employee.departmentId,
    currentUserDepartmentId: auth.departmentId
  })
}
</script>
```

## Testing Checklist

- [ ] Admin can access all pages and perform all actions
- [ ] Manager can access Master Data and department data
- [ ] Manager cannot edit data from other departments
- [ ] User cannot access Master Data
- [ ] User can only edit own data
- [ ] User cannot edit other users' data
- [ ] Viewer cannot access Master Data
- [ ] Viewer cannot see Create/Edit/Delete buttons
- [ ] Viewer sees read-only forms
- [ ] Permission denied messages are shown correctly
- [ ] Buttons are properly hidden/disabled based on permissions

## Migration Guide

To add permission controls to an existing component:

1. Import the composable:
```typescript
import { usePermissions } from '@/composables/usePermissions'
```

2. Use the composable:
```typescript
const permissions = usePermissions()
```

3. Add permission checks to buttons:
```vue
<v-btn v-if="permissions.canCreate.value" @click="handleCreate">
  สร้างใหม่
</v-btn>
```

4. Add permission checks to actions:
```typescript
const handleEdit = (item: any) => {
  if (!permissions.canEdit({ resourceOwnerId: item.employeeId, currentUserId: auth.employeeId })) {
    sweetAlert.warning(permissions.getPermissionErrorMessage())
    return
  }
  // Proceed with edit
}
```

5. Add Master Data access check (if applicable):
```typescript
onMounted(() => {
  if (!permissions.canAccessMasterData.value) {
    sweetAlert.error('คุณไม่มีสิทธิ์เข้าถึงข้อมูลหลัก (Master Data)')
    router.push('/Homepage')
  }
})
```
