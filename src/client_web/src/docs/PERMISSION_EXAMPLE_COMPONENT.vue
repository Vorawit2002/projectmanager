<template>
  <VRow class="match-height px-0 mb-1">
    <v-col
      cols="12"
      class="d-flex justify-space-between"
    >
      <span class="text-sub-title">
        <v-icon
          icon="ri-id-card-line"
          class="mr-1"
        />
        รายการข้อมูล
      </span>

      <!-- Create button - hidden for Viewer role -->
      <v-btn 
        v-if="canCreate"
        @click="handleCreate"
        color="success"
      >
        <v-icon class="mr-2" icon="ri-add-circle-line" />
        เพิ่มข้อมูล
      </v-btn>
    </v-col>
  </VRow>

  <VCard class="card-table">
    <v-data-table-server
      v-model:page="pageNumber"
      v-model:items-per-page="pageSize"
      :headers="headers"
      :items="items"
      :items-length="totalItems"
      :loading="isLoading"
      @update:page="handlePageChange"
      @update:items-per-page="handlePageSizeChange"
    >
      <template v-slot:item.actions="{ item }">
        <!-- View button - always visible -->
        <v-btn
          density="compact"
          icon
          color="info"
          @click="handleView(item)"
          v-tooltip="{ text: 'ดูรายละเอียด', contentClass: 'bg-info text-white', location: 'top' }"
        >
          <v-icon size="18">ri-article-line</v-icon>
        </v-btn>

        <!-- Edit button - conditional based on permissions -->
        <v-btn
          v-if="canEditItem(item)"
          density="compact"
          icon
          color="warning"
          @click="handleEdit(item)"
          v-tooltip="{ text: 'แก้ไขข้อมูล', contentClass: 'bg-warning text-white', location: 'top' }"
        >
          <v-icon size="18">ri-edit-2-line</v-icon>
        </v-btn>

        <!-- Edit button disabled with tooltip for no permission -->
        <v-tooltip
          v-else
          location="top"
        >
          <template v-slot:activator="{ props }">
            <v-btn
              v-bind="props"
              density="compact"
              icon
              color="warning"
              disabled
            >
              <v-icon size="18">ri-edit-2-line</v-icon>
            </v-btn>
          </template>
          <span>{{ permissionMessage }}</span>
        </v-tooltip>

        <!-- Delete button - conditional based on permissions -->
        <v-btn
          v-if="canDeleteItem(item)"
          density="compact"
          icon
          color="error"
          @click="handleDelete(item)"
          v-tooltip="{ text: 'ลบข้อมูล', contentClass: 'bg-error text-white', location: 'top' }"
        >
          <v-icon size="18">ri-delete-bin-6-line</v-icon>
        </v-btn>

        <!-- Delete button disabled with tooltip for no permission -->
        <v-tooltip
          v-else
          location="top"
        >
          <template v-slot:activator="{ props }">
            <v-btn
              v-bind="props"
              density="compact"
              icon
              color="error"
              disabled
            >
              <v-icon size="18">ri-delete-bin-6-line</v-icon>
            </v-btn>
          </template>
          <span>{{ permissionMessage }}</span>
        </v-tooltip>
      </template>

      <!-- Other columns -->
      <template v-slot:item.name="{ item }">
        {{ item.name }}
      </template>

      <template v-slot:item.department="{ item }">
        {{ item.department?.name || '-' }}
      </template>

      <template v-slot:item.owner="{ item }">
        {{ item.employee?.firstName }} {{ item.employee?.lastName }}
      </template>
    </v-data-table-server>
  </VCard>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore, useSweetAlertStore } from '@/stores'
import { usePermissions } from '@/composables/usePermissions'

// Stores
const auth = useAuthStore()
const sweetAlert = useSweetAlertStore()
const router = useRouter()

// Permissions
const permissions = usePermissions()

// Data
const items = ref<any[]>([])
const pageNumber = ref(1)
const pageSize = ref(10)
const totalItems = ref(0)
const isLoading = ref(false)

const headers = [
  { title: 'จัดการ', value: 'actions', align: 'center', sortable: false },
  { title: 'ชื่อ', value: 'name' },
  { title: 'แผนก', value: 'department' },
  { title: 'ผู้รับผิดชอบ', value: 'owner' },
]

// Permission checks
const canCreate = computed(() => permissions.canCreate.value)

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

const permissionMessage = computed(() => permissions.getPermissionErrorMessage())

// Handlers
const handleCreate = () => {
  if (!canCreate.value) {
    sweetAlert.warning(permissionMessage.value)
    return
  }
  // Open create dialog
  console.log('Create new item')
}

const handleView = (item: any) => {
  // View is always allowed
  console.log('View item:', item)
}

const handleEdit = (item: any) => {
  if (!canEditItem(item)) {
    sweetAlert.warning(permissionMessage.value)
    return
  }
  // Open edit dialog
  console.log('Edit item:', item)
}

const handleDelete = async (item: any) => {
  if (!canDeleteItem(item)) {
    sweetAlert.warning(permissionMessage.value)
    return
  }

  const confirmDelete = await sweetAlert.showAlert({
    title: 'ยืนยันการลบข้อมูล',
    text: 'คุณต้องการลบข้อมูลนี้หรือไม่?',
    icon: 'warning',
    showCancelButton: true,
    confirmButtonText: 'ลบข้อมูล',
    cancelButtonText: 'ยกเลิก',
  })

  if (confirmDelete.isConfirmed) {
    try {
      // Delete item
      console.log('Delete item:', item)
      sweetAlert.successDeleted('ลบข้อมูลสำเร็จ')
    } catch (error) {
      sweetAlert.error('เกิดข้อผิดพลาดในการลบข้อมูล')
    }
  }
}

const handlePageChange = (page: number) => {
  pageNumber.value = page
  loadItems()
}

const handlePageSizeChange = (size: number) => {
  pageSize.value = size
  loadItems()
}

const loadItems = async () => {
  isLoading.value = true
  try {
    // Load items from API
    // The backend should already filter based on user role
    console.log('Loading items...')
  } catch (error) {
    console.error('Error loading items:', error)
  } finally {
    isLoading.value = false
  }
}

// Lifecycle
onMounted(() => {
  loadItems()
})
</script>

<style scoped>
/* Add any component-specific styles here */
</style>
