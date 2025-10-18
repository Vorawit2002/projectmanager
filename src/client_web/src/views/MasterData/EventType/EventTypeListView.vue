<template>
  <VRow class="match-height px-0 mb-1">
    <v-col
      cols="12"
      class="d-flex justify-space-between"
    >
      <span class="text-sub-title">
        <v-icon
          icon="ri-home-office-line"
          class="mr-1"
        />
        ข้อมูลประเภทกิจกรรม</span
      >

      <v-btn 
        v-if="canModifyMasterData()"
        @click="OpenDialogCreate"
        ><v-icon
          class="mr-2"
          icon="ri-add-circle-line"
        />
        เพิ่มข้อมูลผู้ติดต่อ</v-btn
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
        label="ค้นหาผู้ติดต่อ"
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
      :loading="isLoading"
      :headers="header"
      :items="data"
      :items-length="totalItem"
      @update:page="handlePageChange"
      @update:items-per-page="handlePageSizeChange"
      class="text-no-wrap"
    >
      <template v-slot:item="{ item, index }: any">
        <tr
          :class="{ 'selected-row': item.id === id }"
          @click="debugRowClick(item)"
        >
          <td class="text-center">
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
              v-tooltip="{ text: 'แก้ไขข้อมูล', contentClass: 'bg-warning text-white', location: 'top' }"
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
              v-tooltip="{ text: 'ลบข้อมูล', contentClass: 'bg-error-darken-1 text-white', location: 'top' }"
              @click.stop="OpenDelete(item.id)"
            >
              <v-icon size="18">ri-delete-bin-6-line</v-icon>
            </v-btn>
          </td>
          <td>{{ item.name || '-' }}</td>
          <td>{{ formattdate(item.created) }}</td>
        </tr>
      </template>
    </v-data-table-server>
  </VCard>

  <v-navigation-drawer
    v-model="DialogCreate"
    :width="$vuetify.display.xs ? '100vw' : '550'"
    class="z-indexDialog create-activity-drawer"
    close-on-back
    temporary
    transition="dialog-right-transition"
    location="right"
    scrollable
  >
    <CreateEventType
      v-if="DialogCreate"
      @close="DialogCreate = false"
      @created="handleCreated"
    />
  </v-navigation-drawer>

  <v-navigation-drawer
    v-model="DialogEdit"
    :width="$vuetify.display.xs ? '100vw' : '550'"
    class="z-indexDialog create-activity-drawer"
    close-on-back
    temporary
    transition="dialog-right-transition"
    location="right"
    scrollable
  >
    <UpdateEventType
      v-if="DialogEdit"
      :id="id"
      :CloseDialogEdit="CloseDialogEdit"
    />
  </v-navigation-drawer>

  <v-navigation-drawer
    v-model="DialogDetail"
    :width="$vuetify.display.xs ? '100vw' : '550'"
    class="z-indexDialog create-activity-drawer"
    close-on-back
    temporary
    transition="dialog-right-transition"
    location="right"
    scrollable
  >
    <EventTypeDetailView
      v-if="DialogDetail"
      :id="id"
      :CloseDialogEdit="CloseDialogDetail"
    />
  </v-navigation-drawer>
</template>

<script lang="ts">
import { TypeOrganizationEnum } from '@/@layouts/enums'
import { Client, GetEventTypeWithPaginationQuery } from '@/client'
import { BACKEND_API_URL, formattdate } from '@/constants'
import { useAuthStore, useSweetAlertStore } from '@/stores'
import { RoleService } from '@/utils/RoleService'
import { defineComponent } from 'vue'
import CreateEventType from './CreateEventType.vue'
import EventTypeDetailView from './EventTypeDetailView.vue'
import UpdateEventType from './UpdateEventType.vue'
const client = new Client(BACKEND_API_URL)
const roleService = new RoleService()
export default defineComponent({
  name: 'EventTypeListView',
  components: {
    CreateEventType,
    UpdateEventType,
    EventTypeDetailView,
  },
  data() {
    return {
      auth: useAuthStore(),
      sweetAlert: useSweetAlertStore(),
      header: [
        { title: 'จัดการ', value: 'actions', align: 'center' },
        { title: 'ประเภทกิจกรรม', value: 'name' },
        { title: 'วันที่สร้าง', value: 'created' },
      ] as any,
      search: '',
      data: [] as any,
      request: new GetEventTypeWithPaginationQuery(),
      pageNumber: 1,
      pageSize: 10,
      totalItem: 0 as any,
      TypeOrganizationEnum,
      id: '',
      DialogCreate: false,
      DialogEdit: false,
      DialogDetail: false,
      isLoading: false,
      formattdate,
    }
  },
  async mounted() {
    this.forceCloseAllDrawers() // บังคับปิด Drawer ทั้งหมดเมื่อ Component ถูก Mount
    
    // Check if user has access to Master Data
    if (!this.canAccessMasterData()) {
      this.sweetAlert.error('คุณไม่มีสิทธิ์เข้าถึงข้อมูลหลัก (Master Data)')
      this.$router.push('/Homepage')
      return
    }
    
    await this.initialize()
    window.addEventListener('keydown', this.handleEscCloseDrawer)
    window.addEventListener('popstate', this.handleBackButton) // เพิ่ม listener สำหรับ back button บนมือถือ
  },
  beforeUnmount() {
    window.removeEventListener('keydown', this.handleEscCloseDrawer)
    window.removeEventListener('popstate', this.handleBackButton) // ลบ listener
  },
  methods: {
    forceCloseAllDrawers() {
      this.DialogCreate = false
      this.DialogEdit = false
      this.DialogDetail = false
    },
    handleBackButton() {
      // ตรวจสอบว่ามี drawer ตัวไหนเปิดอยู่หรือไม่
      if (this.DialogCreate || this.DialogEdit || this.DialogDetail) {
        this.forceCloseAllDrawers()
        // ไม่ต้อง history.pushState(null, '', location.href); เพราะ close-on-back จะจัดการให้
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
    onDrawerClose(drawer: 'DialogCreate' | 'DialogEdit' | 'DialogDetail', value: boolean) {
      if (!value) {
        ;(this as any)[drawer] = false
        // Clear id and any other data if needed
        if (drawer === 'DialogEdit' || drawer === 'DialogDetail') {
          this.id = ''
        }
      }
    },
    async initialize() {
      try {
        this.request.pageNumber = this.pageNumber
        this.request.search = this.search
        this.request.pageSize = this.pageSize === -1 ? this.totalItem : this.pageSize
        const response = await client.getEventTypeWithPagination(this.request)
        this.data = response.items
        console.log('Data', this.data)
        this.totalItem = response.totalCount
      } catch (error) {
        console.error(error)
      }
    },
    handleCreated() {
      this.initialize()
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
      console.log('CloseDialogCreate called with:', { value, reload, searchData }) // Debug log
      this.DialogCreate = value
      if (reload === true) {
        this.initialize().then(() => {
          console.log('Initialize completed, organization contacts:', this.data) // Debug log

          // หาก searchData ถูกส่งมา ให้หา ID ที่ตรงกัน
          if (searchData) {
            try {
              const search = JSON.parse(searchData)
              console.log('Searching for organization contact with:', search) // Debug log

              // หาข้อมูลในฐานข้อมูลที่ตรงกับที่เพิ่งสร้าง
              let matchedItem = this.data.find(
                (item: any) => item.firstName === search.firstName && item.lastName === search.lastName,
              )

              // หากไม่เจอแบบตรงทุกฟิลด์ ให้ลองหาแบบ name matching
              if (!matchedItem && search.firstName && search.lastName) {
                console.log('Exact match not found, trying name-based search') // Debug log

                matchedItem = this.data.find((item: any) => {
                  // ตรวจสอบการ match แบบต่างๆ
                  const firstNameMatch = item.firstName?.toLowerCase() === search.firstName?.toLowerCase()
                  const lastNameMatch = item.lastName?.toLowerCase() === search.lastName?.toLowerCase()
                  const emailMatch = item.email?.toLowerCase() === search.email?.toLowerCase()

                  // ต้อง match ชื่อ + นามสกุล และ email หรือ organizationId
                  return (
                    firstNameMatch && lastNameMatch && (emailMatch || item.organizationId === search.organizationId)
                  )
                })
              }

              console.log('Found matched organization contact:', matchedItem) // Debug log

              if (matchedItem) {
                this.id = matchedItem.id
                console.log('Setting selected organization contact ID to:', this.id) // Debug log

                // รอให้ DOM อัพเดท
                setTimeout(() => {
                  console.log('Trying to scroll to highlighted row for organization contact ID:', this.id) // Debug log
                  this.scrollToHighlightedRow()
                }, 500)
              } else {
                // หากไม่เจอ ให้เลือกแถวแรก (ข้อมูลใหม่มักจะอยู่ด้านบน)
                console.log('No matched organization contact found, selecting first contact') // Debug log
                if (this.data.length > 0) {
                  this.id = this.data[0].id
                  setTimeout(() => {
                    this.scrollToHighlightedRow()
                  }, 500)
                }
              }
            } catch (error) {
              console.error('Error parsing organization contact search data:', error)
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
      if (reload === true) {
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
    async OpenDelete(id: any) {
      this.id = id
      const confirmDelete = await this.sweetAlert.showAlert({
        title: 'ยืนยันการลบข้อมูล',
        text: `คุณต้องการลบข้อมูลนี้หรือไม่ ?`,
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
          const del = await client.deleteEventType(id)
          if (del) {
            this.sweetAlert.successDeleted('ลบข้อมูลสำเร็จ')
            this.initialize()
          }
        } catch (error) {
          this.sweetAlert.error('เกิดข้อผิดพลาดในการลบข้อมูล !')
        }
      }
    },
    debugRowClick(item: any) {
      console.log('Organization contact row clicked:', item)
      console.log('Contact ID:', item.id, 'Selected ID:', this.id)
      console.log('IDs match:', item.id === this.id)
      console.log('ID types:', typeof item.id, typeof this.id)
    },
    scrollToHighlightedRow() {
      console.log('scrollToHighlightedRow called, current selected organization contact ID:', this.id) // Debug log
      // หาแถวที่ถูก highlight
      const highlightedRow = document.querySelector('.selected-row')
      console.log('Found highlighted organization contact row:', highlightedRow) // Debug log

      if (highlightedRow) {
        // เลื่อนไปยังแถวนั้น
        highlightedRow.scrollIntoView({
          behavior: 'smooth',
          block: 'center',
        })

        // เพิ่ม pulse animation เพื่อให้เห็นชัดขึ้น
        highlightedRow.classList.add('pulse-animation')
        setTimeout(() => {
          highlightedRow.classList.remove('pulse-animation')
        }, 2000)

        console.log('Scrolled to highlighted organization contact row and added animation') // Debug log
      } else {
        console.log(
          'No highlighted organization contact row found, available contacts:',
          this.data.map((item: any) => item.id),
        ) // Debug log
      }
    },
    canAccessMasterData(): boolean {
      // Only Admin and Manager can access Master Data
      return roleService.canAccessMasterData(this.auth.roles)
    },
    canModifyMasterData(): boolean {
      // Only Admin and Manager can modify Master Data
      // Viewer cannot modify
      return roleService.canModifyData(this.auth.roles) && 
             roleService.canAccessMasterData(this.auth.roles)
    },
  },
})
</script>

<style scoped>
@media (max-width: 767.98px) {
  .create-activity-drawer:not(.v-navigation-drawer--active) {
    transform: translateX(100%) !important;
    visibility: hidden !important;
  }
}
</style>
