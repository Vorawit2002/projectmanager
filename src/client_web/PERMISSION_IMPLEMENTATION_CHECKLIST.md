# Permission Controls Implementation Checklist

## ✅ Task 20: Completed

### Infrastructure (100% Complete)
- [x] Created `usePermissions` composable
- [x] Created `PermissionButton` component
- [x] Updated `RoleService` with permission methods
- [x] Updated `auth` store with employeeId and departmentId
- [x] Created Activity Plan permission mixin
- [x] Created Project permission mixin
- [x] Created comprehensive documentation
- [x] Created example component
- [x] Created quick reference guides

---

## 🎯 Next Steps: Component Integration

### Priority 1: Activity Plans
- [ ] Update `CustomerAppointmentPlanListView.vue`
  - [ ] Import `useActivityPlanPermissions`
  - [ ] Hide Create button for Viewer
  - [ ] Add conditional Edit buttons
  - [ ] Add conditional Delete buttons
  - [ ] Add permission checks in handlers
  
- [ ] Update `CreateCustomerAppointmentPlan.vue`
  - [ ] Check `canCreate` before allowing form submission
  
- [ ] Update `UpdateCustomerAppointmentPlan.vue`
  - [ ] Check `canEdit` before allowing form submission
  - [ ] Show read-only mode for Viewer

### Priority 2: Projects
- [ ] Update `ProjectListView.vue`
  - [ ] Import `useProjectPermissions`
  - [ ] Hide Create button for Viewer
  - [ ] Add conditional Edit buttons
  - [ ] Add conditional Delete buttons
  
- [ ] Update `CreateProject.vue`
  - [ ] Check `canCreate` before allowing form submission
  
- [ ] Update `UpdateProject.vue`
  - [ ] Check `canEdit` before allowing form submission
  - [ ] Show read-only mode for Viewer

### Priority 3: Master Data
- [ ] Update `EventTypeListView.vue`
  - [ ] Already has basic checks ✓
  - [ ] Enhance with new composable
  
- [ ] Update `OrganizationListView.vue`
  - [ ] Add Master Data access check
  - [ ] Hide Create button for User/Viewer
  - [ ] Hide Edit/Delete buttons for User/Viewer
  
- [ ] Update `DepartmentListView.vue`
  - [ ] Add Master Data access check
  - [ ] Hide Create button for User/Viewer
  - [ ] Hide Edit/Delete buttons for User/Viewer

### Priority 4: Employees
- [ ] Update Employee list views
  - [ ] Add department-level permission checks
  - [ ] Hide for User/Viewer roles
  - [ ] Manager can only edit department employees

### Priority 5: Check-in/Check-out
- [ ] Update Check-in/Check-out views
  - [ ] Add ownership checks
  - [ ] Hide Edit/Delete for other users' records

---

## 🧪 Testing Checklist

### Test with Admin Role
- [ ] Can see all Create buttons
- [ ] Can see all Edit buttons
- [ ] Can see all Delete buttons
- [ ] Can access Master Data
- [ ] Can edit all records
- [ ] Can delete all records

### Test with Manager Role
- [ ] Can see all Create buttons
- [ ] Can see Edit buttons for department records
- [ ] Cannot see Edit buttons for other departments
- [ ] Can access Master Data
- [ ] Can edit department records
- [ ] Cannot edit other department records
- [ ] Sees appropriate error message when trying to edit other departments

### Test with User Role
- [ ] Can see Create button
- [ ] Can see Edit buttons for own records
- [ ] Cannot see Edit buttons for other users' records
- [ ] Cannot access Master Data
- [ ] Can edit own records
- [ ] Cannot edit other users' records
- [ ] Sees appropriate error message when trying to edit others' data

### Test with Viewer Role
- [ ] Cannot see Create buttons
- [ ] Cannot see Edit buttons (or they are disabled)
- [ ] Cannot see Delete buttons (or they are disabled)
- [ ] Cannot access Master Data
- [ ] Can view own records
- [ ] Forms are in read-only mode
- [ ] Sees "read-only account" message when trying to modify

---

## 📋 Integration Template

Use this template when updating a component:

```vue
<template>
  <!-- Create Button -->
  <v-btn v-if="permissions.canCreate.value" @click="handleCreate">
    <v-icon class="mr-2">ri-add-circle-line</v-icon>
    สร้างใหม่
  </v-btn>

  <!-- Table with conditional actions -->
  <v-data-table :items="items">
    <template v-slot:item.actions="{ item }">
      <!-- View (always visible) -->
      <v-btn icon @click="handleView(item)">
        <v-icon>ri-article-line</v-icon>
      </v-btn>

      <!-- Edit (conditional) -->
      <v-btn
        v-if="canEditItem(item)"
        icon
        @click="handleEdit(item)"
      >
        <v-icon>ri-edit-2-line</v-icon>
      </v-btn>

      <!-- Delete (conditional) -->
      <v-btn
        v-if="canDeleteItem(item)"
        icon
        @click="handleDelete(item)"
      >
        <v-icon>ri-delete-bin-6-line</v-icon>
      </v-btn>
    </template>
  </v-data-table>
</template>

<script setup lang="ts">
import { usePermissions } from '@/composables/usePermissions'
import { useAuthStore, useSweetAlertStore } from '@/stores'

const permissions = usePermissions()
const auth = useAuthStore()
const sweetAlert = useSweetAlertStore()

// Permission checks
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

// Handlers
const handleCreate = () => {
  if (!permissions.canCreate.value) {
    sweetAlert.warning(permissions.getPermissionErrorMessage())
    return
  }
  // Open create dialog
}

const handleEdit = (item: any) => {
  if (!canEditItem(item)) {
    sweetAlert.warning(permissions.getPermissionErrorMessage())
    return
  }
  // Open edit dialog
}

const handleDelete = async (item: any) => {
  if (!canDeleteItem(item)) {
    sweetAlert.warning(permissions.getPermissionErrorMessage())
    return
  }
  // Confirm and delete
}
</script>
```

---

## 📚 Documentation References

| Document | Purpose |
|----------|---------|
| `src/docs/PERMISSION_CONTROLS_GUIDE.md` | Complete implementation guide |
| `src/docs/PERMISSION_EXAMPLE_COMPONENT.vue` | Full working example |
| `src/docs/PERMISSION_QUICK_REFERENCE.md` | Quick reference card |
| `src/docs/PERMISSION_CONTROLS_README.md` | Quick start guide |
| `.kiro/specs/role-management-and-profile/task-20-verification.md` | Verification document |

---

## 🔧 Backend Requirements

For the permission system to work correctly, the backend must:

- [ ] Include `employeeId` in user session/token response
- [ ] Include `departmentId` in user session/token response
- [ ] Validate all permissions on the backend (frontend is UX only)
- [ ] Return appropriate error codes (401, 403) for unauthorized access
- [ ] Filter data based on user role before sending to frontend

---

## 📞 Support

If you need help implementing permission controls:

1. Check the Quick Reference: `src/docs/PERMISSION_QUICK_REFERENCE.md`
2. Review the Example Component: `src/docs/PERMISSION_EXAMPLE_COMPONENT.vue`
3. Read the Complete Guide: `src/docs/PERMISSION_CONTROLS_GUIDE.md`
4. Check the Verification Document for testing guidance

---

**Status**: Infrastructure Complete ✅  
**Next**: Component Integration 🚀  
**Last Updated**: Task 20 Implementation
