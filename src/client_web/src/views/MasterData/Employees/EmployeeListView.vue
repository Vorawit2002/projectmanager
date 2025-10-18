<template>
  <VRow class="match-height px-0 mb-1">
    <v-col
      cols="12"
      class="d-flex justify-space-between"
    >
      <span class="text-sub-title">
        <v-icon icon="ri-user-line" />
        พนักงาน</span
      >

      <!-- <v-btn 
        v-if="canModifyMasterData()"
        @click="OpenDialogCreate"
        ><v-icon
          class="mr-2"
          icon="ri-add-circle-line"
        />
        เพิ่มพนักงาน</v-btn
      > -->
    </v-col>
  </VRow>

  <VRow
    class="mb-2"
    justify="end"
  >
    <VCol
      cols="12"
      md="3"
    >
      <VSelect
        v-model="selectedDepartment"
        @update:model-value="applyFilters"
        label="กรองตามแผนก"
        :items="departments"
        item-title="name"
        item-value="id"
        color="primary"
        class="mr-2 form-field"
        clearable
        dense
      ></VSelect>
    </VCol>
    <VCol
      cols="12"
      md="3"
    >
      <VSelect
        v-model="selectedStatus"
        @update:model-value="applyFilters"
        label="กรองตามสถานะ"
        :items="statusOptions"
        item-title="text"
        item-value="value"
        color="primary"
        class="mr-2 form-field"
        clearable
        dense
      ></VSelect>
    </VCol>
    <VCol
      cols="12"
      md="6"
    >
      <VTextField
        v-model="search"
        @input="applyFilters"
        label="ค้นหาพนักงาน (ชื่อ, อีเมล, ตำแหน่ง)"
        color="primary"
        class="mr-4 form-field"
        append-inner-icon="ri-search-line"
        clearable
        dense
      ></VTextField>
    </VCol>
  </VRow>

  <VCard class="card-table custom-scrollbar">
    <v-data-table-server
      v-model:page="pageNumber"
      v-model:items-per-page="pageSize"
      :headers="header"
      :items="filteredData"
      :loading="isLoading"
      :items-length="totalItem"
      class="text-no-wrap"
      @update:page="handlePageChange"
      @update:items-per-page="handlePageSizeChange"
    >
      <template v-slot:item="{ item, index }: any">
        <tr
          :class="{ 'selected-row': item.id === id }"
          @click="debugRowClick(item)"
        >
          <!-- จัดการ - ซ่อนไว้ชั่วคราว -->
          <!-- <td class="text-center align-center">
            <v-btn
              class="text-white mr-2"
              density="compact"
              icon
              color="info"
              rounded="lg"
              v-tooltip="{ text: 'ดูรายละเอียด', contentClass: 'bg-info text-white', location: 'top' }"
              @click.stop="OpenDialogDetail(item.id)"
            >
              <v-icon size="18">ri-article-line</v-icon>
            </v-btn>
            <v-btn
              v-if="canModifyMasterData()"
              class="text-white mr-2"
              density="compact"
              color="warning"
              icon
              rounded="lg"
              v-tooltip="{ text: 'แก้ไขข้อมูล', contentClass: 'bg-warning text-white ', location: 'top' }"
              @click.stop="OpenDialogEdit(item.id)"
            >
              <v-icon size="18">ri-edit-2-line</v-icon>
            </v-btn>
            <v-btn
              v-if="canModifyMasterData()"
              class="text-white mr-2"
              density="compact"
              color="error-darken-1"
              icon
              rounded="lg"
              v-tooltip="{ text: 'ลบข้อมูล', contentClass: 'bg-error-darken-1 text-white ', location: 'top' }"
              @click.stop="OpenDelete(item.id)"
            >
              <v-icon size="18">ri-delete-bin-6-line</v-icon>
            </v-btn>
          </td> -->
          <td class="text-center">
            <v-avatar
              v-if="item.imageProfile"
              size="40"
              class="profile-avatar"
            >
              <v-img
                :src="getImageUrl(item.imageProfile)"
                :alt="getFullName(item)"
              ></v-img>
            </v-avatar>
            <v-avatar
              v-else
              size="40"
              color="primary"
              class="profile-avatar"
            >
              <span class="text-white">{{ getInitials(item) }}</span>
            </v-avatar>
          </td>
          <td>{{ getFullName(item) }}</td>
          <td>{{ item.email || '-' }}</td>
          <td>{{ item.position || '-' }}</td>
          <td>{{ getDepartmentName(item.departmentId) }}</td>
          <td class="text-center">
            <v-chip
              :color="item.isActive ? 'success' : 'error'"
              size="small"
            >
              {{ item.isActive ? 'ใช้งาน' : 'ไม่ใช้งาน' }}
            </v-chip>
          </td>
        </tr>
      </template>
    </v-data-table-server>
  </VCard>

  <v-navigation-drawer
    v-model="DialogCreate"
    scrollable
    temporary
    class="z-indexDialog create-activity-drawer"
    :width="$vuetify.display.xs ? '100vw' : '550'"
    close-on-back
    transition="dialog-right-transition"
    location="right"
  >
    <CreateEmployee
      v-if="DialogCreate"
      :CloseDialogCreate="CloseDialogCreate"
    />
  </v-navigation-drawer>

  <v-navigation-drawer
    v-model="DialogEdit"
    scrollable
    temporary
    transition="dialog-right-transition"
    class="z-indexDialog create-activity-drawer"
    location="right"
    :width="$vuetify.display.xs ? '100vw' : '550'"
    close-on-back
  >
    <UpdateEmployee
      v-if="DialogEdit"
      :id="id"
      :CloseDialogEdit="CloseDialogEdit"
    />
  </v-navigation-drawer>

  <v-navigation-drawer
    v-model="DialogDetail"
    scrollable
    temporary
    class="z-indexDialog create-activity-drawer"
    transition="dialog-right-transition"
    location="right"
    :width="$vuetify.display.xs ? '100vw' : '550'"
    close-on-back
  >
    <EmployeeDetail
      v-if="DialogDetail"
      :id="id"
      :CloseDialogDetail="CloseDialogDetail"
    />
  </v-navigation-drawer>
</template>

<script lang="ts">
import { Client, GetEmployeeWithPaginationQuery, EmployeeDto, DepartmentDto } from '@/client'
import { BACKEND_API_URL } from '@/constants'
import { useAuthStore, useSweetAlertStore } from '@/stores'
import { RoleService } from '@/utils/RoleService'
import { defineComponent } from 'vue'
import CreateEmployee from './CreateEmployee.vue'
import EmployeeDetail from './EmployeeDetail.vue'
import UpdateEmployee from './UpdateEmployee.vue'

const client = new Client(BACKEND_API_URL)
const roleService = new RoleService()

export default defineComponent({
  name: 'EmployeeListView',
  components: {
    CreateEmployee,
    UpdateEmployee,
    EmployeeDetail,
  },
  data() {
    return {
      auth: useAuthStore(),
      sweetAlert: useSweetAlertStore(),
      header: [
        // { title: 'จัดการ', value: 'actions', align: 'center' }, // ซ่อนไว้ชั่วคราว
        { title: 'รูปโปรไฟล์', value: 'imageProfile', align: 'center' },
        { title: 'ชื่อ-นามสกุล', value: 'fullName' },
        { title: 'อีเมล', value: 'email' },
        { title: 'ตำแหน่ง', value: 'position' },
        { title: 'แผนก', value: 'departmentId' },
        { title: 'สถานะ', value: 'isActive', align: 'center' },
      ] as any,
      search: '',
      data: [] as EmployeeDto[],
      filteredData: [] as EmployeeDto[],
      departments: [] as DepartmentDto[],
      selectedDepartment: null as string | null,
      selectedStatus: null as boolean | null,
      statusOptions: [
        { text: 'ใช้งาน', value: true },
        { text: 'ไม่ใช้งาน', value: false },
      ],
      request: new GetEmployeeWithPaginationQuery(),
      pageNumber: 1,
      pageSize: 10,
      totalItem: 0 as any,
      id: '',
      DialogCreate: false,
      DialogEdit: false,
      DialogDetail: false,
      isLoading: false,
    }
  },
  async mounted() {
    this.forceCloseAllDrawers()
    
    if (!this.canAccessDepartmentData()) {
      this.sweetAlert.error('คุณไม่มีสิทธิ์เข้าถึงข้อมูลพนักงาน')
      this.$router.push('/Homepage')
      return
    }
    
    await this.loadDepartments()
    await this.initialize()
    window.addEventListener('keydown', this.handleEscCloseDrawer)
    window.addEventListener('popstate', this.handleBackButton)
  },

  beforeUnmount() {
    window.removeEventListener('keydown', this.handleEscCloseDrawer)
    window.removeEventListener('popstate', this.handleBackButton)
  },
  methods: {
    forceCloseAllDrawers() {
      this.DialogCreate = false
      this.DialogEdit = false
      this.DialogDetail = false
    },
    handleBackButton() {
      if (this.DialogCreate || this.DialogEdit || this.DialogDetail) {
        this.forceCloseAllDrawers()
      }
    },
    handleEscCloseDrawer(e: KeyboardEvent) {
      if (e.key === 'Escape') {
        if (this.DialogCreate) {
          this.CloseDialogCreate(false, false)
        } else if (this.DialogEdit) {
          this.CloseDialogEdit(false, false)
        } else if (this.DialogDetail) {
          this.CloseDialogDetail(false, false)
        }
      }
    },
    async loadDepartments() {
      try {
        const response = await client.getDepartmentQuery()
        this.departments = response.filter((dept: DepartmentDto) => dept.isActive)
      } catch (error) {
        console.error('Error loading departments:', error)
      }
    },
    async initialize() {
      try {
        this.isLoading = true
        this.request.pageNumber = this.pageNumber
        this.request.pageSize = this.pageSize === -1 ? this.totalItem : this.pageSize
        const response = await client.getEmployeeWithPagination(this.request)
        this.data = response.items || []
        this.totalItem = response.totalCount || 0
        
        // Debug: Check if email and imageProfile are coming from backend
        console.log('Employee data from backend:', this.data)
        if (this.data.length > 0) {
          console.log('First employee:', this.data[0])
          console.log('Email:', this.data[0].email)
          console.log('ImageProfile:', this.data[0].imageProfile)
        }
        
        this.applyFilters()
      } catch (error) {
        console.error(error)
        this.sweetAlert.error('เกิดข้อผิดพลาดในการโหลดข้อมูล')
      } finally {
        this.isLoading = false
      }
    },
    applyFilters() {
      let filtered = [...this.data]

      // Apply search filter
      if (this.search && this.search.trim() !== '') {
        const searchLower = this.search.toLowerCase().trim()
        filtered = filtered.filter((item: EmployeeDto) => {
          const fullName = this.getFullName(item).toLowerCase()
          const email = (item.email || '').toLowerCase()
          const position = (item.position || '').toLowerCase()
          return fullName.includes(searchLower) || 
                 email.includes(searchLower) || 
                 position.includes(searchLower)
        })
      }

      // Apply department filter
      if (this.selectedDepartment) {
        filtered = filtered.filter((item: EmployeeDto) => 
          item.departmentId === this.selectedDepartment
        )
      }

      // Apply status filter
      if (this.selectedStatus !== null) {
        filtered = filtered.filter((item: EmployeeDto) => 
          item.isActive === this.selectedStatus
        )
      }

      this.filteredData = filtered
    },
    clearFilters() {
      this.search = ''
      this.selectedDepartment = null
      this.selectedStatus = null
      this.applyFilters()
    },
    async handlePageChange(page: number) {
      this.pageNumber = page
      await this.initialize()
    },
    async handlePageSizeChange(size: number) {
      this.pageSize = size
      await this.initialize()
    },
    OpenDialogCreate() {
      this.DialogCreate = true
    },
    CloseDialogCreate(value: boolean, reload: boolean, searchData?: string) {
      this.DialogCreate = value
      if (reload === true) {
        this.initialize().then(() => {
          if (searchData) {
            try {
              const search = JSON.parse(searchData)
              const matchedItem = this.data.find(
                (item: any) =>
                  item.email === search.email &&
                  item.firstName === search.firstName &&
                  item.lastName === search.lastName,
              )

              if (matchedItem) {
                this.id = matchedItem.id!
                setTimeout(() => {
                  this.scrollToHighlightedRow()
                }, 500)
              } else {
                if (this.data.length > 0) {
                  this.id = this.data[0].id!
                  setTimeout(() => {
                    this.scrollToHighlightedRow()
                  }, 500)
                }
              }
            } catch (error) {
              console.error('Error parsing search data:', error)
            }
          }
        })
      }
    },
    OpenDialogEdit(id: any) {
      if (id) {
        this.DialogEdit = true
        this.id = id
      }
    },
    CloseDialogEdit(value: boolean, reload: boolean) {
      this.DialogEdit = value
      if (reload) {
        this.initialize()
      }
    },
    OpenDialogDetail(id: any) {
      if (id) {
        this.DialogDetail = true
        this.id = id
      }
    },
    CloseDialogDetail(value: boolean, reload: boolean) {
      this.DialogDetail = value
      if (reload === true) {
        this.initialize()
      }
    },
    async OpenDelete(id: string) {
      this.id = id
      const confirmDelete = await this.sweetAlert.showAlert({
        title: 'ยืนยันการลบข้อมูล',
        text: `คุณต้องการลบพนักงานนี้หรือไม่ ?`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'ลบข้อมูล',
        cancelButtonText: 'ยกเลิก',
        customClass: {
          popup: 'bg-white z-index-5000 rounded-popup',
        },
      })

      if (confirmDelete.isConfirmed) {
        try {
          const del = await client.deleteEmployee(id)
          if (del) {
            this.sweetAlert.successDeleted('ลบข้อมูลสำเร็จ')
            this.initialize()
          }
        } catch (error) {
          this.sweetAlert.error('เกิดข้อผิดพลาดในการลบข้อมูล !')
          console.error(error)
        }
      }
    },
    debugRowClick(item: any) {
      console.log('Row clicked:', item)
    },
    scrollToHighlightedRow() {
      const highlightedRow = document.querySelector('.selected-row')

      if (highlightedRow) {
        highlightedRow.scrollIntoView({
          behavior: 'smooth',
          block: 'center',
        })

        highlightedRow.classList.add('pulse-animation')
        setTimeout(() => {
          highlightedRow.classList.remove('pulse-animation')
        }, 2000)
      }
    },
    getDepartmentName(departmentId?: string): string {
      if (!departmentId) return '-'
      const department = this.departments.find((dept: DepartmentDto) => dept.id === departmentId)
      return department?.name || '-'
    },
    getFullName(employee: EmployeeDto): string {
      const parts = []
      if (employee.titleName) parts.push(employee.titleName)
      if (employee.firstName) parts.push(employee.firstName)
      if (employee.lastName) parts.push(employee.lastName)
      return parts.length > 0 ? parts.join(' ') : '-'
    },
    getInitials(employee: EmployeeDto): string {
      const firstName = employee.firstName || ''
      const lastName = employee.lastName || ''
      return (firstName.charAt(0) + lastName.charAt(0)).toUpperCase() || '?'
    },
    getImageUrl(imageProfile: string): string {
      if (!imageProfile) return ''
      
      // If it's already a full URL (http/https) or data URL (data:), use as is
      if (imageProfile.startsWith('http') || imageProfile.startsWith('data:')) {
        return imageProfile
      }
      
      // If it's a relative path, prepend BACKEND_API_URL
      return `${BACKEND_API_URL}${imageProfile.startsWith('/') ? '' : '/'}${imageProfile}`
    },
    canAccessDepartmentData(): boolean {
      return roleService.canViewDepartmentData(this.auth.roles)
    },
    canModifyMasterData(): boolean {
      return roleService.canModifyData(this.auth.roles) && 
             roleService.canAccessMasterData(this.auth.roles)
    },
  },
})
</script>

<style scoped>
.z-indexDialog {
  z-index: 9999;
}

.create-activity-drawer {
  z-index: 9999;
  background: #fff !important;
  overflow: hidden;
}

.card-form {
  background: #ffffff;
  border-radius: 15px;
}

.text-sub-title {
  font-size: 25px;
  font-weight: bold;
  color: #2b3086;
  background-clip: text;
  -webkit-background-clip: text;
  text-shadow: 5px 5px 8px rgba(65, 69, 92, 0.447);
}

:deep(.selected-row) {
  background-color: rgba(var(--v-theme-info), 0.1) !important;
}

.form-field {
  transition: all 0.2s ease;
}

.form-field:deep(.v-field) {
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.04);
  transition: all 0.2s ease;
}

.form-field:deep(.v-field--focused) {
  box-shadow: 0 4px 16px rgba(43, 48, 134, 0.12);
}

.profile-avatar {
  border: 2px solid rgba(var(--v-theme-primary), 0.2);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

@media (max-width: 767.98px) {
  .create-activity-drawer:not(.v-navigation-drawer--active) {
    transform: translateX(100%) !important;
    visibility: hidden !important;
  }
}

@keyframes pulse {
  0%, 100% {
    opacity: 1;
  }
  50% {
    opacity: 0.5;
  }
}

.pulse-animation {
  animation: pulse 0.5s ease-in-out 4;
}
</style>
