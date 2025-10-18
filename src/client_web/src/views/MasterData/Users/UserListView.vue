<template>
  <VRow class="match-height px-0 mb-1">
    <v-col
      cols="12"
      class="d-flex justify-space-between"
    >
      <span class="text-sub-title">
        <v-icon
          icon="ri-user-settings-line"
          class="mr-1"
        />
        จัดการผู้ใช้งาน</span
      >
    </v-col>
  </VRow>

  <VRow class="mb-2">
    <VCol
      cols="12"
      md="4"
    >
      <VTextField
        v-model="search"
        @input="initialize"
        label="ค้นหาผู้ใช้งาน (ชื่อ, อีเมล, แผนก)"
        color="primary"
        class="form-field"
        append-inner-icon="ri-search-line"
        clearable
        dense
      ></VTextField>
    </VCol>
    <VCol
      cols="12"
      md="3"
    >
      <VSelect
        v-model="roleFilter"
        @update:model-value="initialize"
        :items="roleOptions"
        label="กรองตาม Role"
        color="primary"
        class="form-field"
        clearable
        dense
      ></VSelect>
    </VCol>
    <VCol
      cols="12"
      md="3"
    >
      <VSelect
        v-model="isActiveFilter"
        @update:model-value="initialize"
        :items="statusOptions"
        label="กรองตามสถานะ"
        color="primary"
        class="form-field"
        clearable
        dense
      ></VSelect>
    </VCol>
  </VRow>

  <VCard class="card-table custom-scrollbar">
    <v-data-table-server
      v-model:page="pageNumber"
      v-model:items-per-page="pageSize"
      :items-per-page-options="pageSizeOptions"
      :loading="isLoading"
      :headers="headers"
      :items="users"
      :items-length="totalItems"
      @update:page="handlePageChange"
      @update:items-per-page="handlePageSizeChange"
      class="text-no-wrap"
    >
      <template v-slot:item.imageProfile="{ item }">
        <v-avatar
          size="40"
          class="my-2"
          variant="tonal"
        >
          <v-img
            v-if="item.imageProfile"
            :src="getImageUrl(item.imageProfile)"
            :alt="item.firstName + ' ' + item.lastName"
          />
          <span v-else class="text-subtitle-1 font-weight-medium">
            {{ getUserInitials(item) }}
          </span>
        </v-avatar>
      </template>

      <template v-slot:item.fullName="{ item }">
        <a
          href="javascript:void(0)"
          @click="openUserDetail(item.userId)"
          class="text-primary"
        >
          {{ item.firstName }} {{ item.lastName }}
        </a>
      </template>

      <template v-slot:item.roles="{ item }">
        <v-chip
          v-for="role in item.roles"
          :key="role"
          :color="getRoleColor(role)"
          size="small"
          class="mr-1"
        >
          {{ role }}
        </v-chip>
      </template>

      <template v-slot:item.isActive="{ item }">
        <v-chip
          :color="item.isActive ? 'success' : 'error'"
          size="small"
        >
          {{ item.isActive ? 'Active' : 'Inactive' }}
        </v-chip>
      </template>

      <template v-slot:item.actions="{ item }">
        <v-btn
          class="text-white mr-2"
          density="compact"
          icon
          color="primary"
          rounded="lg"
          v-tooltip="{ text: 'กำหนด Role', contentClass: 'bg-primary text-white', location: 'top' }"
          @click="openAssignRoleDialog(item)"
        >
          <v-icon size="18">ri-shield-user-line</v-icon>
        </v-btn>
        <v-btn
          class="text-white"
          density="compact"
          icon
          color="info"
          rounded="lg"
          v-tooltip="{ text: 'ดูรายละเอียด', contentClass: 'bg-info text-white', location: 'top' }"
          @click="openUserDetail(item.userId)"
        >
          <v-icon size="18">ri-article-line</v-icon>
        </v-btn>
      </template>
    </v-data-table-server>
  </VCard>

  <!-- Assign Role Drawer -->
  <v-navigation-drawer
    v-model="assignRoleDrawer"
    :width="$vuetify.display.xs ? '100vw' : '550'"
    class="z-indexDialog create-activity-drawer"
    close-on-back
    temporary
    transition="dialog-right-transition"
    location="right"
    scrollable
  >
    <AssignRoleDrawer
      v-if="assignRoleDrawer"
      :user="selectedUser"
      @close="assignRoleDrawer = false"
      @role-assigned="handleRoleAssigned"
    />
  </v-navigation-drawer>

  <!-- User Detail Drawer -->
  <v-navigation-drawer
    v-model="userDetailDrawer"
    :width="$vuetify.display.xs ? '100vw' : '550'"
    class="z-indexDialog create-activity-drawer"
    close-on-back
    temporary
    transition="dialog-right-transition"
    location="right"
    scrollable
  >
    <UserDetailView
      v-if="userDetailDrawer"
      :user-id="selectedUserId"
      @close="userDetailDrawer = false"
    />
  </v-navigation-drawer>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import { Client } from '@/client'
import { BACKEND_API_URL } from '@/constants'
import { useSweetAlertStore } from '@/stores'
import AssignRoleDrawer from './AssignRoleDrawer.vue'
import UserDetailView from './UserDetailView.vue'

const client = new Client(BACKEND_API_URL)

export default defineComponent({
  name: 'UserListView',
  components: {
    AssignRoleDrawer,
    UserDetailView,
  },
  data() {
    return {
      sweetAlert: useSweetAlertStore(),
      headers: [
        { title: 'รูปโปรไฟล์', value: 'imageProfile', align: 'center', sortable: false },
        { title: 'ชื่อ-นามสกุล', value: 'fullName' },
        { title: 'อีเมล', value: 'email' },
        { title: 'ตำแหน่ง', value: 'position' },
        { title: 'แผนก', value: 'department' },
        { title: 'Role', value: 'roles', sortable: false },
        { title: 'สถานะ', value: 'isActive', align: 'center' },
        { title: 'จัดการ', value: 'actions', align: 'center', sortable: false },
      ] as any,
      users: [] as any[],
      search: '',
      roleFilter: null as string | null,
      isActiveFilter: null as boolean | null,
      pageNumber: 1,
      pageSize: 10,
      totalItems: 0,
      pageSizeOptions: [
        { value: 5, title: '5' },
        { value: 10, title: '10' },
        { value: 25, title: '25' },
        { value: 50, title: '50' },
        { value: 100, title: '100' },
      ],
      roleOptions: [
        { title: 'Admin', value: 'Admin' },
        { title: 'Manager', value: 'Manager' },
        { title: 'User', value: 'User' },
        { title: 'Viewer', value: 'Viewer' },
      ],
      statusOptions: [
        { title: 'Active', value: true },
        { title: 'Inactive', value: false },
      ],
      isLoading: false,
      assignRoleDrawer: false,
      userDetailDrawer: false,
      selectedUser: null as any,
      selectedUserId: '',
    }
  },
  async mounted() {
    await this.initialize()
  },
  methods: {
    getImageUrl(imageProfile: string): string {
      if (!imageProfile) return ''
      
      // If it's already a full URL (http/https) or data URL (data:), use as is
      if (imageProfile.startsWith('http') || imageProfile.startsWith('data:')) {
        return imageProfile
      }
      
      // If it's a relative path, prepend BACKEND_API_URL
      return `${BACKEND_API_URL}${imageProfile.startsWith('/') ? '' : '/'}${imageProfile}`
    },
    getUserInitials(item: any): string {
      const firstName = item.firstName || ''
      const lastName = item.lastName || ''
      const userName = item.userName || ''
      
      if (firstName && lastName) {
        return `${firstName.charAt(0)}${lastName.charAt(0)}`.toUpperCase()
      } else if (firstName) {
        return firstName.charAt(0).toUpperCase()
      } else if (userName) {
        return userName.charAt(0).toUpperCase()
      }
      return 'U'
    },
    handlePageChange(page: number) {
      this.pageNumber = page
      this.initialize()
    },
    handlePageSizeChange(size: number) {
      this.pageSize = size
      this.pageNumber = 1
      this.initialize()
    },
    async initialize() {
      try {
        this.isLoading = true
        const response = await client.getAllUsers(
          this.search || undefined,
          this.roleFilter || undefined,
          this.isActiveFilter !== null ? this.isActiveFilter : undefined
        )
        this.users = response
        this.totalItems = response.length // For now, use array length. Later can add pagination from backend
      } catch (error: any) {
        console.error('Error loading users:', error)
        this.sweetAlert.error('ไม่สามารถโหลดข้อมูลผู้ใช้งานได้')
      } finally {
        this.isLoading = false
      }
    },
    getRoleColor(role: string): string {
      const colors: Record<string, string> = {
        Admin: 'error',
        Manager: 'warning',
        User: 'primary',
        Viewer: 'secondary',
      }
      return colors[role] || 'default'
    },
    openAssignRoleDialog(user: any) {
      this.selectedUser = user
      this.assignRoleDrawer = true
    },
    openUserDetail(userId: string) {
      this.selectedUserId = userId
      this.userDetailDrawer = true
    },
    async handleRoleAssigned() {
      this.assignRoleDrawer = false
      await this.initialize()
    },
  },
})
</script>

<style scoped>
.text-sub-title {
  font-size: 1.25rem;
  font-weight: 600;
}

.text-primary {
  text-decoration: none;
  color: rgb(var(--v-theme-primary));
}

.text-primary:hover {
  text-decoration: underline;
}
</style>
