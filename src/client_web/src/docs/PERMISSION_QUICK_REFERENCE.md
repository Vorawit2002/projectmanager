# Permission Controls - Quick Reference Card

## 🚀 Quick Start (3 Steps)

### Step 1: Import
```typescript
import { usePermissions } from '@/composables/usePermissions'
```

### Step 2: Setup
```typescript
const permissions = usePermissions()
const auth = useAuthStore()
```

### Step 3: Use
```vue
<v-btn v-if="permissions.canCreate.value">สร้าง</v-btn>
```

---

## 📋 Permission Matrix

| Role | Create | Edit | Delete | Master Data |
|------|:------:|:----:|:------:|:-----------:|
| Admin | ✅ All | ✅ All | ✅ All | ✅ Yes |
| Manager | ✅ All | ✅ Dept | ✅ Dept | ✅ Yes |
| User | ✅ Own | ✅ Own | ✅ Own | ❌ No |
| Viewer | ❌ No | ❌ No | ❌ No | ❌ No |

---

## 🔑 Common Patterns

### Pattern 1: Hide Create Button
```vue
<v-btn v-if="permissions.canCreate.value" @click="create">
  สร้าง
</v-btn>
```

### Pattern 2: Conditional Edit Button
```vue
<v-btn v-if="canEdit(item)" @click="edit(item)">
  แก้ไข
</v-btn>

<script setup>
const canEdit = (item) => permissions.canEdit({
  resourceOwnerId: item.employeeId,
  currentUserId: auth.employeeId,
  resourceDepartmentId: item.departmentId,
  currentUserDepartmentId: auth.departmentId
})
</script>
```

### Pattern 3: Show Error Message
```typescript
if (!canEdit(item)) {
  sweetAlert.warning(permissions.getPermissionErrorMessage())
  return
}
```

### Pattern 4: Master Data Check
```typescript
onMounted(() => {
  if (!permissions.canAccessMasterData.value) {
    sweetAlert.error('ไม่มีสิทธิ์เข้าถึง Master Data')
    router.push('/Homepage')
  }
})
```

---

## 🛠️ Available Methods

### Role Checks (Reactive)
```typescript
permissions.isAdmin.value      // true/false
permissions.isManager.value    // true/false
permissions.isUser.value       // true/false
permissions.isViewer.value     // true/false
```

### Permission Checks
```typescript
permissions.canCreate.value    // Reactive boolean

permissions.canEdit({
  resourceOwnerId: string,
  currentUserId: string,
  resourceDepartmentId: string,
  currentUserDepartmentId: string
})

permissions.canDelete({ ... })  // Same as canEdit

permissions.canView({ ... })    // Same as canEdit
```

### Utilities
```typescript
permissions.getPermissionErrorMessage()  // Returns Thai message
permissions.showPermissionDenied()       // Logs warning
```

---

## 💬 Error Messages

| Role | Message |
|------|---------|
| Viewer | คุณไม่มีสิทธิ์ดำเนินการนี้ (บัญชีของคุณเป็นแบบอ่านอย่างเดียว) |
| User | คุณไม่มีสิทธิ์แก้ไขข้อมูลของผู้อื่น |
| Manager | คุณไม่มีสิทธิ์แก้ไขข้อมูลของแผนกอื่น |

---

## 🎯 Use Cases

### Activity Plans
```typescript
// Check if user can edit activity plan
const canEdit = (plan) => permissions.canEdit({
  resourceOwnerId: plan.employeeId,
  currentUserId: auth.employeeId,
  resourceDepartmentId: plan.employees?.departmentId,
  currentUserDepartmentId: auth.departmentId
})
```

### Master Data (Organizations, Departments, etc.)
```typescript
// Check access on mount
onMounted(() => {
  if (!permissions.canAccessMasterData.value) {
    router.push('/Homepage')
  }
})

// Check if can modify
const canModify = computed(() => 
  permissions.canModifyData.value && 
  permissions.canAccessMasterData.value
)
```

### Employees
```typescript
// Check if can edit employee
const canEdit = (employee) => permissions.canEdit({
  resourceDepartmentId: employee.departmentId,
  currentUserDepartmentId: auth.departmentId
})
```

---

## 🧩 PermissionButton Component

```vue
<PermissionButton
  action="edit"
  :permission-options="{
    resourceOwnerId: item.employeeId,
    currentUserId: auth.employeeId
  }"
  color="warning"
  icon
  @click="handleEdit(item)"
>
  <v-icon>ri-edit-2-line</v-icon>
</PermissionButton>
```

**Props:**
- `action`: 'create' | 'edit' | 'delete' | 'view'
- `permissionOptions`: Object with resource IDs
- `color`, `variant`, `size`, `icon`: Standard Vuetify props
- `showAlert`: Show alert on click when no permission (default: true)

---

## 📁 File Locations

| File | Purpose |
|------|---------|
| `src/composables/usePermissions.ts` | Main composable |
| `src/components/PermissionButton.vue` | Reusable button |
| `src/utils/RoleService.ts` | Role utility class |
| `src/stores/auth.ts` | Auth store (has employeeId, departmentId) |

---

## ✅ Integration Checklist

- [ ] Import `usePermissions`
- [ ] Setup composable
- [ ] Add `v-if` to Create button
- [ ] Add `v-if` to Edit buttons
- [ ] Add `v-if` to Delete buttons
- [ ] Add permission checks in handlers
- [ ] Add Master Data access check (if applicable)
- [ ] Test with all 4 roles

---

## 📚 Full Documentation

- **Complete Guide**: `src/docs/PERMISSION_CONTROLS_GUIDE.md`
- **Example Component**: `src/docs/PERMISSION_EXAMPLE_COMPONENT.vue`
- **Verification**: `.kiro/specs/role-management-and-profile/task-20-verification.md`

---

## 🐛 Troubleshooting

**Q: Buttons not hiding?**
A: Check that `permissions.canCreate.value` is used (with `.value`)

**Q: Permission checks not working?**
A: Verify `auth.employeeId` and `auth.departmentId` are populated

**Q: Error messages in Thai not showing?**
A: Use `permissions.getPermissionErrorMessage()` method

**Q: Need custom permission logic?**
A: Create a mixin like `ActivityPlanPermissionMixin.ts`

---

**Last Updated**: Task 20 Implementation
**Version**: 1.0.0
