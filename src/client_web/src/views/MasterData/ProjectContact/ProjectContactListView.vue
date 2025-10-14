<template>
  <VRow class="match-height px-0 mb-1">
    <v-col
      cols="12"
      class="d-flex justify-space-between"
    >
      <span class="text-sub-title">
        <v-icon icon="ri-group-line" />
        ผู้ติดต่อโครงการ</span
      >

      <v-btn @click="OpenDialogCreate"
        ><v-icon
          class="mr-2"
          icon="ri-add-circle-line"
        />
        เพิ่มผู้ติดต่อโครงการ</v-btn
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
        label="ค้นหาโครงการ"
        color="primary"
        class="mr-4 form-field"
        append-inner-icon="ri-search-line"
        clearable
        dense
      ></VTextField>
    </VCol>
  </VRow>

  <VCard class="card-table">
    <v-data-table-server
      v-model:page="pageNumber"
      v-model:items-per-page="pageSize"
      :loading="isLoading"
      :headers="header"
      :items="tableItems"
      :itemsLength="totalItem"
      @update:page="handlePageChange"
      @update:items-per-page="handlePageSizeChange"
      class="text-no-wrap"
    >
      <template v-slot:item.fullName="{ item }">
        {{ getFullName(item) }}
      </template>

      <template v-slot:item.organizationName="{ item }">
        {{ (item as any).organizations?.name || '-' }}
      </template>

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
              class="text-white mr-2"
              density="compact"
              color="warning"
              icon
              rounded="lg"
              v-tooltip="{ text: 'แก้ไขข้อมูล', contentClass: 'bg-warning text-white', location: 'top' }"
              @click.stop="OpenDialogEdit(item.id)"
            >
              <v-icon size="18">ri-edit-line</v-icon>
            </v-btn>
            <v-btn
              class="text-white mr-2"
              density="compact"
              color="error"
              icon
              rounded="lg"
              v-tooltip="{ text: 'ลบข้อมูล', contentClass: 'bg-error text-white', location: 'top' }"
              @click.stop="OpenDelete(item.id)"
            >
              <v-icon size="18">ri-delete-bin-line</v-icon>
            </v-btn>
          </td>
          <td>{{ item.projectName }}</td>
          <td>{{ item.contactFullName }}</td>
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
    <CreateProjectContact
      v-if="DialogCreate"
      :CloseDialogCreate="CloseDialogCreate"
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
    <UpdateProjectContact
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
    <DetailProjectContact
      v-if="DialogDetail"
      :id="id"
      :CloseDialogDetail="CloseDialogDetail"
    />
  </v-navigation-drawer>
</template>

<script lang="ts">
import { Client, GetProjectContactWithPaginationQuery } from '@/client'
import { BACKEND_API_URL } from '@/constants'
import { useSweetAlertStore } from '@/stores'
import moment from 'moment'
import { defineComponent } from 'vue'
import CreateProjectContact from './CreateProjectContact.vue'
import DetailProjectContact from './DetailProjectContact.vue'
import UpdateProjectContact from './UpdateProjectContact.vue'
const client = new Client(BACKEND_API_URL)
export default defineComponent({
  name: 'ProjectContactListView',
  components: {
    CreateProjectContact,
    UpdateProjectContact,
    DetailProjectContact,
  },
  data() {
    return {
      sweetAlert: useSweetAlertStore(),
      header: [
        { title: 'จัดการ', value: 'actions', align: 'center' },
        { title: 'ชื่อโครงการ', value: 'projectName' },
        { title: 'ผู้ติดต่อโครงการ', value: 'contactFullName' },
      ] as any,
      data: [] as any,
      search: '',
      request: new GetProjectContactWithPaginationQuery(),
      pageNumber: 1,
      pageSize: 10,
      totalItem: 0 as any,
      id: '',
      DialogCreate: false,
      DialogEdit: false,
      DialogDetail: false,
      tableItems: [],
      isLoading: false,
    }
  },
  async mounted() {
    this.forceCloseAllDrawers() // บังคับปิด Drawer ทั้งหมดเมื่อ Component ถูก Mount
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
    async initialize() {
      try {
        this.request.search = this.search
        this.request.pageNumber = this.pageNumber
        this.request.pageSize = this.pageSize === -1 ? this.totalItem : this.pageSize
        console.log(this.request)
        const response = await client.getProjectContactWithPagination(this.request)
        this.data = response.items
        console.log(this.data)
        this.totalItem = response.totalCount

        // แปลงข้อมูลให้ง่ายสำหรับ v-data-table
        this.tableItems = this.data.map((item: any) => {
          const project = item.projects || {}
          const contact = item.organizationContacts || {}

          return {
            id: item.id,
            projectName: project.projectName || '-',
            contactFullName: `${contact.firstName || ''} ${contact.lastName || ''}`.trim() || '-',
          }
        })
      } catch (error) {
        console.error(error)
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
      console.log('CloseDialogCreate called with:', { value, reload, searchData }) // Debug log
      this.DialogCreate = value
      if (reload === true) {
        this.initialize().then(() => {
          console.log('Initialize completed, tableItems:', this.tableItems) // Debug log

          // หาก searchData ถูกส่งมา ให้หา ID ที่ตรงกัน
          if (searchData) {
            try {
              const search = JSON.parse(searchData)
              console.log('Searching for item with:', search) // Debug log

              // หาข้อมูลในฐานข้อมูลที่ตรงกับที่เพิ่งสร้าง
              const matchedItem = this.data.find(
                (item: any) =>
                  item.projectId === search.projectId && item.organizationContactId === search.organizationContactId,
              )

              console.log('Found matched item:', matchedItem) // Debug log

              if (matchedItem) {
                this.id = matchedItem.id
                console.log('Setting selected ID to:', this.id) // Debug log

                // รอให้ DOM อัพเดท
                setTimeout(() => {
                  console.log('Trying to scroll to highlighted row for ID:', this.id) // Debug log
                  this.scrollToHighlightedRow()
                }, 500)
              } else {
                // หากไม่เจอ ให้เลือกแถวแรก (ข้อมูลใหม่มักจะอยู่ด้านบน)
                console.log('No matched item found, selecting first item') // Debug log
                if (this.tableItems.length > 0) {
                  this.id = (this.tableItems[0] as any).id
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
          const del = await client.deleteProjectContact(id)
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
      console.log('Item ID:', item.id, 'Selected ID:', this.id)
      console.log('IDs match:', item.id === this.id)
      console.log('ID types:', typeof item.id, typeof this.id)
    },
    scrollToHighlightedRow() {
      console.log('scrollToHighlightedRow called, current selected ID:', this.id) // Debug log
      // หาแถวที่ถูก highlight
      const highlightedRow = document.querySelector('.selected-row')
      console.log('Found highlighted row:', highlightedRow) // Debug log

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

        console.log('Scrolled to highlighted row and added animation') // Debug log
      } else {
        console.log(
          'No highlighted row found, available table items:',
          this.tableItems.map((item: any) => item.id),
        ) // Debug log
      }
    },
    formatThaiDate(date: string | Date): string {
      if (!date || date === '0001-01-01T00:00:00' || new Date(date).getFullYear() === 1) {
        return ''
      }

      const monthShortThai = [
        '',
        'ม.ค.',
        'ก.พ.',
        'มี.ค.',
        'เม.ย.',
        'พ.ค.',
        'มิ.ย.',
        'ก.ค.',
        'ส.ค.',
        'ก.ย.',
        'ต.ค.',
        'พ.ย.',
        'ธ.ค.',
      ]

      const m = moment(date)
      const day = m.date()
      const month = monthShortThai[m.month() + 1] // index 0-11
      const year = m.year() + 543

      return `${day} ${month} ${year}`
    },
    getFullName(item: any): string {
      // Adjust this logic based on your actual data structure
      const contact = item.organizationContacts || item
      const firstName = contact.firstName || ''
      const lastName = contact.lastName || ''
      return `${firstName} ${lastName}`.trim() || '-'
    },
  },
})
</script>

<style scoped>
/* Additional safety for drawer state */
.z-indexDialog {
  z-index: 9999;
}

/* Mobile drawer fix - ensure drawer is closed on page load */
.create-activity-drawer {
  z-index: 9999;
  background: #fff !important;
  overflow: hidden;
}

.text-sub-title {
  font-size: 25px;
  font-weight: bold;
  color: #2b3086;
  /* background-image: linear-gradient( 135deg, #7e4ee6b6 10%, #8C57FF 100%); */
  background-clip: text;
  -webkit-background-clip: text; /* สำหรับเว็บเบราว์เซอร์ที่รองรับ */
  text-shadow: 5px 5px 8px rgba(65, 69, 92, 0.447);
}
:deep(.selected-row) {
  background-color: rgba(var(--v-theme-info), 0.1) !important;
}

/* Enhanced form fields */
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

.textarea-field:deep(.v-field) {
  border-radius: 16px;
}

.checkbox-field:deep(.v-selection-control) {
  min-height: 32px;
}

.add-btn {
  opacity: 0.7;
  transition: all 0.2s ease;
}

.add-btn:hover {
  opacity: 1;
  transform: scale(1.1);
}

.upload-container {
  background-color: #fff; /* ต้องมีพื้นหลังสีขาว */
  border: 1px solid rgba(0, 0, 0, 0.12); /* สร้างเส้นขอบบางๆ สีเทาอ่อน */
  border-radius: 12px; /* ทำให้ขอบมนเหมือน form-field */
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.04); /* เพิ่มเงาเหมือน form-field */
  transition: all 0.2s ease;
}

/* สร้างเอฟเฟกต์ตอน focus */
.upload-container:focus-within {
  border-color: rgba(43, 48, 134, 0.6); /* เปลี่ยนสีขอบเมื่อ focus */
  box-shadow: 0 4px 16px rgba(43, 48, 134, 0.12); /* เพิ่มเงาตอน focus */
}

@media (max-width: 767.98px) {
  .create-activity-drawer:not(.v-navigation-drawer--active) {
    transform: translateX(100%) !important;
    visibility: hidden !important;
  }
}
</style>
