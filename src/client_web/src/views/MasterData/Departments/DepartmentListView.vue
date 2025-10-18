<template>
  <VRow class="match-height px-0 mb-1">
    <v-col
      cols="12"
      class="d-flex justify-space-between"
    >
      <span class="text-sub-title">
        <v-icon icon="ri-building-line" />
        แผนก</span
      >

      <v-btn 
        v-if="canModifyMasterData()"
        @click="OpenDialogCreate"
        ><v-icon
          class="mr-2"
          icon="ri-add-circle-line"
        />
        เพิ่มแผนก</v-btn
      >
    </v-col>
  </VRow>

  <VRow
    class="mb-2"
    justify="end"
  >
    <VCol
      cols="12"
      md="4"
    >
      <VTextField
        v-model="search"
        @input="initialize"
        label="ค้นหาแผนก"
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
      :items="data"
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
          <td class="text-center align-center">
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
          </td>
          <td>{{ item.name }}</td>
          <td class="text-center">
            <v-chip
              :color="item.isActive ? 'success' : 'error'"
              size="small"
            >
              {{ item.isActive ? 'ใช้งาน' : 'ไม่ใช้งาน' }}
            </v-chip>
          </td>
          <td>{{ formatDate(item.created) }}</td>
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
    <CreateDepartment
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
    <UpdateDepartment
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
    <DepartmentDetail
      v-if="DialogDetail"
      :id="id"
      :CloseDialogDetail="CloseDialogDetail"
    />
  </v-navigation-drawer>
</template>

<script lang="ts">
import { Client, GetDepartmentWithPaginationQuery } from '@/client'
import { BACKEND_API_URL } from '@/constants'
import { useAuthStore, useSweetAlertStore } from '@/stores'
import { RoleService } from '@/utils/RoleService'
import { defineComponent } from 'vue'
import CreateDepartment from './CreateDepartment.vue'
import DepartmentDetail from './DepartmentDetail.vue'
import UpdateDepartment from './UpdateDepartment.vue'

const client = new Client(BACKEND_API_URL)
const roleService = new RoleService()

export default defineComponent({
  name: 'DepartmentListView',
  components: {
    CreateDepartment,
    UpdateDepartment,
    DepartmentDetail,
  },
  data() {
    return {
      auth: useAuthStore(),
      sweetAlert: useSweetAlertStore(),
      header: [
        { title: 'จัดการ', value: 'actions', align: 'center' },
        { title: 'ชื่อแผนก', value: 'name' },
        { title: 'สถานะ', value: 'isActive', align: 'center' },
        { title: 'วันที่สร้าง', value: 'created' },
      ] as any,
      search: '',
      data: [] as any,
      request: new GetDepartmentWithPaginationQuery(),
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
    
    if (!this.canAccessMasterData()) {
      this.sweetAlert.error('คุณไม่มีสิทธิ์เข้าถึงข้อมูลหลัก (Master Data)')
      this.$router.push('/Homepage')
      return
    }
    
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
    async initialize() {
      try {
        this.isLoading = true
        this.request.search = this.search
        this.request.pageNumber = this.pageNumber
        this.request.pageSize = this.pageSize === -1 ? this.totalItem : this.pageSize
        const response = await client.getDepartmentWithPagination(this.request)
        this.data = response.items
        this.totalItem = response.totalCount
      } catch (error) {
        console.error(error)
        this.sweetAlert.error('เกิดข้อผิดพลาดในการโหลดข้อมูล')
      } finally {
        this.isLoading = false
      }
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
                  item.name === search.name &&
                  item.isActive === search.isActive,
              )

              if (matchedItem) {
                this.id = matchedItem.id
                setTimeout(() => {
                  this.scrollToHighlightedRow()
                }, 500)
              } else {
                if (this.data.length > 0) {
                  this.id = this.data[0].id
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
        text: `คุณต้องการลบแผนกนี้หรือไม่ ?`,
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
          const del = await client.deleteDepartment(id)
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
    canAccessMasterData(): boolean {
      return roleService.canAccessMasterData(this.auth.roles)
    },
    canModifyMasterData(): boolean {
      return roleService.canModifyData(this.auth.roles) && 
             roleService.canAccessMasterData(this.auth.roles)
    },
    formatDate(date: any): string {
      if (!date) return '-'
      const d = new Date(date)
      return d.toLocaleDateString('th-TH', {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
      })
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
