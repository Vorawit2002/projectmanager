<template>
  <VRow class="match-height px-0 mb-1">
    <v-col
      cols="12"
      md="10"
      class="d-flex align-center"
    >
      <span class="text-sub-title">
        <v-icon icon="ri-file-chart-line" />
        สรุปผลการนัดหมาย</span
      >
    </v-col>
    <VCol
      cols="12"
      md="2"
      class="d-flex justify-end "
    >
      <v-btn
        color="#307750"
        @click="ExportExcel"
        :block="$vuetify.display.smAndDown"
        class="export-btn"
        ><v-icon
          class="mr-2"
          icon="ri-file-excel-2-line"
          size="20"
        />
        <span class="d-none d-sm-inline">ดาวน์โหลด Excel</span>
        <span class="d-inline d-sm-none">Excel</span>
      </v-btn>
    </VCol>
  </VRow>

  <VRow class="match-height px-2 mb-2 d-flex justify-end">
    <VCol
      cols="12"
      sm="8"
      md="6"
      lg="4"
    >
      <VTextField
        v-model="search"
        @input="initialize"
        label="ค้นหา รหัสโครงการ / ชื่อหน่วยงาน / วัตถุประสงค์"
        color="primary"
        class="text-black form-field"
        append-inner-icon="ri-search-line"
        clearable
        dense
        hide-details
        density="comfortable"
        variant="outlined"
      ></VTextField>
    </VCol>
  </VRow>

  <VRow class="match-height px-2 mb-1">
    <VCol
      cols="12"
      sm="6"
      md="3"
    >
      <v-autocomplete
        label="ปี"
        :items="YearList"
        item-title="title"
        item-id="value"
        v-model="request.years"
        @update:model-value="initialize"
        hide-details
        density="comfortable"
        variant="outlined"
        class="form-field"
      ></v-autocomplete>
    </VCol>

    <VCol
      cols="12"
      sm="6"
      md="3"
    >
      <v-autocomplete
        label="เดือน"
        :items="month"
        item-title="title"
        item-id="value"
        v-model="request.month"
        @update:model-value="initialize"
        clearable
        hide-details
        density="comfortable"
        variant="outlined"
        class="form-field"
      ></v-autocomplete>
    </VCol>

    <VCol
      cols="12"
      sm="6"
      md="3"
    >
      <v-autocomplete
        label="แผนก"
        :items="DepartmentsList"
        item-title="name"
        item-value="id"
        v-model="request.departmentId"
        @update:model-value="getEmployeeByDepartment"
        clearable
        hide-details
        density="comfortable"
        variant="outlined"
        class="form-field"
      />
    </VCol>

    <VCol
      cols="12"
      sm="6"
      md="3"
    >
      <v-autocomplete
        :disabled="!request.departmentId"
        label="พนักงาน"
        :items="EmployeeList"
        item-title="name"
        item-id="value"
        v-model="request.employeeId"
        @update:model-value="initialize"
        clearable
        hide-details
        density="comfortable"
        variant="outlined"
        class="form-field"
      ></v-autocomplete>
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
      class="text-no-wrap responsive-table single-line-table"
      @update:page="handlePageChange"
      @update:items-per-page="handlePageSizeChange"
      :item-class="getRowClass"
      :mobile-breakpoint="0"
    >
      <template v-slot:item="{ item }: any">
        <tr :class="{ 'selected-row': item.id && highlightedId && item.id.toString() === highlightedId.toString() }">
          <td class="action-cell text-center">
            <div class="action-buttons">
              <v-btn
                class="text-white"
                density="compact"
                icon
                color="info"
                rounded="lg"
                size="small"
                v-tooltip="{
                  text: 'ดูรายละเอียด',
                  contentClass: 'bg-info text-white ',
                  location: 'top',
                }"
                @click="OpenDialogDetail(item.id)"
              >
                <v-icon size="16">ri-article-line</v-icon>
              </v-btn>
            </div>
          </td>
          <td class="employee-cell">
            <div class="employee-info">
              <span
                class="employee-name"
                :title="item.employees?.titleName + ' ' + item.employees?.firstName + ' ' + item.employees?.lastName"
              >
                {{ item.employees?.titleName + ' ' + item.employees?.firstName + ' ' + item.employees?.lastName }}
              </span>
            </div>
          </td>
          <td class="organization-cell">
            <div
              class="organization-info"
              :title="item.organizations?.name"
            >
              {{ item.organizations?.name }}
            </div>
          </td>
          <td class="objective-cell">
            <div
              class="objective-info"
              :title="item.objective"
            >
              {{ item.objective }}
            </div>
          </td>
          <td class="location-cell">
            <div
              class="location-info"
              :title="item.location"
            >
              {{ item.location }}
            </div>
          </td>
          <td class="date-cell">
            <div
              class="date-info"
              :title="formatDateforshow(item.startDate, item.endDate, item.allDay)"
            >
              {{ formatDateforshow(item.startDate, item.endDate, item.allDay) }}
            </div>
          </td>
        </tr>
      </template>
    </v-data-table-server>
  </VCard>

  <!-- Detail Activity Dialog -->
  <v-navigation-drawer
    v-model="detailActivityDialog"
    :width="$vuetify.display.xs ? '100vw' : '550'"
    class="z-indexDialog create-activity-drawer"
    close-on-back
    temporary
    transition="dialog-righ-transition"
    location="right"
    scrollable
    :permanent="false"
  >
    <CustomerAppointmentByIdDetailView
      v-if="selectedActivityId"
      :id="selectedActivityId"
      @close="closeDetailActivityDialog"
    />
  </v-navigation-drawer>
</template>

<script lang="ts">
import { TypeOrganizationEnum } from '@/@layouts/enums'
import {
  Client,
  GetActivityPlanContactByActivityPlanIdQuery,
  GetActivityPlanWithPlanNoteWithPaginationQuery,
  GetEmployeeByDepartmentIdQuery,
} from '@/client'
import { BACKEND_API_URL } from '@/constants'
import { useSweetAlertStore } from '@/stores'
import moment from 'moment'
import 'moment/locale/th'
import { defineComponent } from 'vue'
import * as XLSX from 'xlsx'
import CustomerAppointmentByIdDetailView from './CustomerAppointmentByIdDetailView.vue'
moment.locale('th')
const client = new Client(BACKEND_API_URL)
export default defineComponent({
  name: 'ReportCRMListView',
  components: {
    CustomerAppointmentByIdDetailView,
  },
  data() {
    const currentYear = new Date().getFullYear()
    return {
      sweetAlert: useSweetAlertStore(),
      header: [
        { title: 'จัดการ', value: 'actions', align: 'center' },
        { title: 'ผู้รับผิดชอบ', value: 'fullName' },
        { title: 'หน่วยงาน/ลูกค้า', value: 'organizations.name' },
        { title: 'วัตถุประสงค์', value: 'objective' },
        // { title: 'รายละเอียด', value: 'detail' },
        { title: 'สถานที่', value: 'location' },
        { title: 'เวลา', value: 'time' },
      ] as any,
      data: [] as any,
      search: '' as any,
      request: new GetActivityPlanWithPlanNoteWithPaginationQuery(),
      month: [
        { title: 'มกราคม', value: 1 },
        { title: 'กุมภาพันธ์', value: 2 },
        { title: 'มีนาคม', value: 3 },
        { title: 'เมษายน', value: 4 },
        { title: 'พฤษภาคม', value: 5 },
        { title: 'มิถุนายน', value: 6 },
        { title: 'กรกฏาคม', value: 7 },
        { title: 'สิงหาคม', value: 8 },
        { title: 'กันยายน', value: 9 },
        { title: 'ตุลาคม', value: 10 },
        { title: 'พฤศจิกายน', value: 11 },
        { title: 'ธันวาคม', value: 12 },
      ],
      YearList: Array.from({ length: 11 }, (_, i) => {
        const yearCE = currentYear - i
        const yearBE = yearCE + 543
        return {
          title: yearBE.toString(),
          value: yearCE.toString(),
        }
      }),
      pageNumber: 1,
      pageSize: 10,
      totalItem: 0 as any,
      TypeOrganizationEnum,
      id: '',
      DialogCreate: false,
      DialogEdit: false,
      DialogDetail: false,
      isLoading: false,
      requestActivityContact: new GetActivityPlanContactByActivityPlanIdQuery(),
      highlightedId: null as string | null, // สำหรับเก็บ id ที่จะไฮไลต์
      DepartmentsList: [] as any,
      requestEmployee: new GetEmployeeByDepartmentIdQuery(),
      EmployeeList: [] as any,
      detailActivityDialog: false as boolean,
      selectedActivityId: null as string | number | null,
    }
  },
  async mounted() {
    // ตรวจสอบว่าเป็นการ refresh หน้าหรือไม่
    const isPageRefresh = this.isActualRefresh()
    console.log('isPageRefresh:', isPageRefresh)

    await this.initialize()
    await this.getDepartments()
    await this.getEmployeeByDepartment()

    // หากเป็น page refresh ให้เคลียร์ highlight และ localStorage ทันที
    if (isPageRefresh) {
      this.highlightedId = null
      localStorage.removeItem('selectedAppointmentId')
      console.log('Page refresh detected - cleared highlight and localStorage')
    } else {
      // โหลด id ที่เคยเลือกไว้จาก localStorage เฉพาะกรณีไม่ใช่ refresh
      const storedId = localStorage.getItem('selectedAppointmentId')

      if (storedId) {
        this.highlightedId = String(storedId)
        // ลบ id ออกจาก localStorage หลังจากใช้งานแล้ว
        localStorage.removeItem('selectedAppointmentId')
        console.log('Restored highlight from localStorage:', storedId)
      }
    }
    // เพิ่ม event listener สำหรับ ESC
    window.addEventListener('keydown', this.handleEscDrawer)
  },
  beforeUnmount() {
    window.removeEventListener('keydown', this.handleEscDrawer)
  },
  handleEscDrawer(e: KeyboardEvent) {
    if (e.key === 'Escape' && this.detailActivityDialog) {
      this.detailActivityDialog = false
      this.selectedActivityId = null
    }
  },
  methods: {
    handleEscDrawer(e: KeyboardEvent) {
      if (e.key === 'Escape' && this.detailActivityDialog) {
        this.detailActivityDialog = false
        this.selectedActivityId = null
      }
    },
    // วิธีใหม่ในการตรวจจับ refresh ที่แม่นยำกว่า
    isActualRefresh(): boolean {
      // ตรวจสอบ Navigation API ก่อน
      const navigation = window.performance.getEntriesByType('navigation')[0] as any
      console.log('Navigation type:', navigation?.type)

      // ถ้าเป็น reload จริงๆ ให้เคลียร์ localStorage ทันที
      if (navigation && navigation.type === 'reload') {
        const hasStoredId = localStorage.getItem('selectedAppointmentId')
        if (hasStoredId) {
          console.log('Reload detected with stored ID - clearing localStorage immediately')
          localStorage.removeItem('selectedAppointmentId')
        }
        console.log('Navigation type is reload - treating as actual refresh')
        return true
      }

      // ตรวจสอบ performance.navigation สำหรับ browser เก่า
      if (window.performance.navigation && window.performance.navigation.type === 1) {
        const hasStoredId = localStorage.getItem('selectedAppointmentId')
        if (hasStoredId) {
          console.log('Performance navigation type 1 with stored ID - clearing localStorage immediately')
          localStorage.removeItem('selectedAppointmentId')
        }
        return true
      }

      console.log('Not a refresh - normal navigation')
      return false
    },

    async initialize() {
      try {
        this.isLoading = true
        // this.request.departmentId =
        // this.request.employeeId =
        this.request.search = this.search
        this.request.pageNumber = this.pageNumber
        this.request.pageSize = this.pageSize === -1 ? this.totalItem : this.pageSize
        const response = await client.getActivityPlanWithPlanNoteWithPagination(this.request)
        this.data = response.items
        this.totalItem = response.totalCount
        console.log('Data loaded:', this.data)
      } catch (error) {
        console.error(error)
      } finally {
        this.isLoading = false
      }
    },
    async ExportExcelAll() {
      // สร้าง Workbook และ Worksheet
      const wb = XLSX.utils.book_new()

      // สร้าง Header
      const header = [
        'ผู้รับผิดชอบ',
        'รหัสโครงการ',
        'หน่วยงาน/ลูกค้า',
        'ลูกค้าที่นัดพบ',
        'วัตถุประสงค์',
        'รายละเอียด',
        'สถานที่',
        'เวลา',
        'ค่าใช้จ่าย',
        'รายละเอียดค่าใช้จ่าย',
        'สรุปผลการนัดพบ',
        'สิ่งที่ต้องดำเนินการ',
        'หมายเหตุ',
      ]
      const response = await client.getActivityPlanWithPlanNoteForExcel()
      // สร้างข้อมูลให้เป็น Array พร้อมกับลำดับ
      const data = await Promise.all(
        response.map(async (item: any) => {
          this.requestActivityContact.activityPlanId = item.id
          const contact = await client.getActivityPlanContactQueryByActivityPlanId(this.requestActivityContact)
          let ActivityContact = contact
            .map(c =>
              `${c.organizationContacts?.titleName ?? ''}${c.organizationContacts?.firstName ?? ''} ${
                c.organizationContacts?.lastName ?? ''
              }`.trim(),
            )
            .join(', ')
          if (item.cost === null || item.cost === undefined) {
            item.cost = 0
          }
          return [
            item.employees?.titleName + ' ' + item.employees?.firstName + ' ' + item.employees?.lastName || '',
            item.projects?.projectCode || '',
            item.organizations.name || '',
            ActivityContact || '',
            (item.objective !== 'อื่น ๆ' ? item.objective : item.objectiveDetail) || '',
            item.detail || '',
            item.location || '',
            this.formatDateforshow(item.startDate, item.endDate, item.allDay) || '',
            (item.cost === 0 ? 0 : item.cost.toLocaleString(undefined, { maximumFractionDigits: 3 })) + ' บาท' || '',
            item.costDetail || '',
            item.summary || '',
            item.toDoNext || '',
            item.remarks || '',
          ]
        }),
      )
      // เพิ่มข้อมูล Header ที่สร้างขึ้น
      const wsData = [header, ...data]

      // สร้าง Worksheet
      const ws = XLSX.utils.aoa_to_sheet(wsData)

      // เพิ่ม Worksheet ลงใน Workbook
      XLSX.utils.book_append_sheet(wb, ws, 'สรุปผลการนัดหมาย')

      // สร้างไฟล์ Excel และดาวน์โหลด
      XLSX.writeFile(wb, 'สรุปผลการนัดหมาย.xlsx')
    },
    async ExportExcel() {
      // สร้าง Workbook และ Worksheet
      const wb = XLSX.utils.book_new()

      // สร้าง Header
      const header = [
        'ผู้รับผิดชอบ',
        'รหัสโครงการ',
        'หน่วยงาน/ลูกค้า',
        'ลูกค้าที่นัดพบ',
        'วัตถุประสงค์',
        'รายละเอียด',
        'สถานที่',
        'เวลา',
        'ค่าใช้จ่าย',
        'รายละเอียดค่าใช้จ่าย',
        'สรุปผลการนัดพบ',
        'สิ่งที่ต้องดำเนินการ',
        'หมายเหตุ',
      ]
      // สร้างข้อมูลให้เป็น Array พร้อมกับลำดับ
      const data = await Promise.all(
        this.data.map(async (item: any) => {
          this.requestActivityContact.activityPlanId = item.id
          const contact = await client.getActivityPlanContactQueryByActivityPlanId(this.requestActivityContact)
          let ActivityContact = contact
            .map(c =>
              `${c.organizationContacts?.titleName ?? ''}${c.organizationContacts?.firstName ?? ''} ${
                c.organizationContacts?.lastName ?? ''
              }`.trim(),
            )
            .join(', ')
          if (item.cost === null || item.cost === undefined) {
            item.cost = 0
          }
          return [
            item.employees?.titleName + ' ' + item.employees?.firstName + ' ' + item.employees?.lastName || '',
            item.projects?.projectCode || '',
            item.organizations.name || '',
            ActivityContact || '',
            (item.objective !== 'อื่น ๆ' ? item.objective : item.objectiveDetail) || '',
            item.detail || '',
            item.location || '',
            this.formatDateforshow(item.startDate, item.endDate, item.allDay) || '',
            (item.cost === 0 ? 0 : item.cost.toLocaleString(undefined, { maximumFractionDigits: 3 })) + ' บาท' || '',
            item.costDetail || '',
            item.summary || '',
            item.toDoNext || '',
            item.remarks || '',
          ]
        }),
      )
      // เพิ่มข้อมูล Header ที่สร้างขึ้น
      const wsData = [header, ...data]

      // สร้าง Worksheet
      const ws = XLSX.utils.aoa_to_sheet(wsData)

      // เพิ่ม Worksheet ลงใน Workbook
      XLSX.utils.book_append_sheet(wb, ws, 'สรุปผลการนัดหมาย')

      // สร้างไฟล์ Excel และดาวน์โหลด
      XLSX.writeFile(wb, 'สรุปผลการนัดหมาย(ตามฟิลเตอร์).xlsx')
    },
    async handlePageChange(page: number) {
      this.pageNumber = page
      await this.initialize()
    },
    async handlePageSizeChange(size: number) {
      this.pageSize = size
      await this.initialize()
    },
    formatDateforshow(startdate: any, enddate: any, allday: any) {
      if (
        !startdate ||
        !enddate ||
        startdate === '0001-01-01T00:00:00' ||
        enddate === '0001-01-01T00:00:00' ||
        new Date(startdate).getFullYear() === 1 ||
        new Date(enddate).getFullYear() === 1
      ) {
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

      const start = new Date(startdate)
      const end = new Date(enddate)

      const startText = `${start.getDate()} ${monthShortThai[start.getMonth() + 1]} ${start.getFullYear() + 543}${
        allday !== true
          ? ` เวลา ${start.getHours().toString().padStart(2, '0')}:${start.getMinutes().toString().padStart(2, '0')} น.`
          : ''
      }`
      const endText = `${end.getDate()} ${monthShortThai[end.getMonth() + 1]} ${end.getFullYear() + 543}${
        allday !== true
          ? ` เวลา ${end.getHours().toString().padStart(2, '0')}:${end.getMinutes().toString().padStart(2, '0')} น.`
          : ' (ทั้งวัน)'
      }`

      return `${startText} ถึง ${endText}`
    },
    OpenDialogCreate() {
      this.DialogCreate = true
    },
    CloseDialogCreate(value: boolean, reload: boolean) {
      this.DialogCreate = value
      if (reload === true) {
        this.initialize()
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
        // Set highlight และเปิด dialog แทนการ navigate
        this.highlightedId = String(id)
        this.selectedActivityId = id
        this.detailActivityDialog = true

        // บันทึกใน localStorage สำหรับการกลับมา
        localStorage.setItem('selectedAppointmentId', String(id))
      }
    },
    getRowClass(item: any) {
      return item.id && this.highlightedId && item.id.toString() === this.highlightedId.toString() ? 'selected-row' : ''
    },
    closeDetailActivityDialog(): void {
      this.detailActivityDialog = false
      this.selectedActivityId = null

      // เก็บค่า highlightedId ก่อน refresh
      const currentHighlightedId = this.highlightedId

      // เรียก initialize() เพื่อ refresh data
      this.initialize().then(() => {
        // กู้คืน highlightedId หลัง refresh
        if (currentHighlightedId) {
          this.highlightedId = currentHighlightedId
          console.log('Restored highlightedId after refresh:', currentHighlightedId)
        }
      })
    },
    CloseDialogDetail(value: boolean, reload: boolean) {
      this.DialogDetail = value
      if (reload === true) {
        this.initialize()
      }
    },
    async OpenDelete(id: any) {
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
          const del = await client.deleteOrganization(id)
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
    async getDepartments() {
      try {
        const response = await client.getDepartmentQuery()
        if (response) {
          this.DepartmentsList = response
          console.log('DepartmentsList ', response)
        }
      } catch (error) {
        console.error('Error fetching departments:', error)
      }
    },
    async getEmployeeByDepartment() {
      try {
        this.request.employeeId = undefined

        if (!this.request.departmentId) return

        this.requestEmployee.departmentId = this.request.departmentId
        console.log('requestEmployee ', this.requestEmployee)

        this.EmployeeList = await client.getEmployeeQueryByDepartmentId(this.requestEmployee)
        this.EmployeeList.forEach((x: any) => {
          x.name = x.firstName + ' ' + x.lastName
        })
        console.log('EmployeeList', this.EmployeeList)
      } catch (error) {
        console.error(error)
      }
    },
  },
})
</script>

<style scoped>
.card-form {
  background: #ffffff;
  border-radius: 15px;
  /* box-shadow: 2px 2px 6px 6px rgba(43, 48, 134, 0.15); */
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

/* Responsive font sizes */
@media (max-width: 959px) {
  .text-sub-title {
    font-size: 20px;
  }
}

@media (max-width: 599px) {
  .text-sub-title {
    font-size: 18px;
    text-align: center;
    margin-bottom: 10px;
  }
}

.export-btn {
  min-width: auto !important;
}

@media (max-width: 959px) {
  .export-btn {
    width: 100%;
  }
}

/* Table responsive styles - แถวเดียวไม่เว้นบรรทัด */
.responsive-table {
  overflow-x: auto !important;
  width: 100% !important;
}

.single-line-table :deep(.v-data-table__wrapper) {
  overflow-x: auto !important;
  -webkit-overflow-scrolling: touch;
  width: 100% !important;
}

:deep(.selected-row) {
  background-color: rgba(var(--v-theme-info), 0.1) !important;
}

/* Cell responsive styles - บังคับให้แสดงแถวเดียว */
.employee-info,
.organization-info,
.objective-info,
.location-info {
  max-width: 200px;
  white-space: nowrap !important;
  overflow: hidden !important;
  text-overflow: ellipsis !important;
  line-height: 1.2 !important;
}

.date-info {
  min-width: 280px !important;
  max-width: none !important; /* ไม่จำกัดความกว้าง */
  white-space: nowrap !important;
  overflow: visible !important; /* ให้แสดงเต็ม */
  text-overflow: unset !important;
  line-height: 1.2 !important;
}

.employee-name {
  font-weight: 500;
  color: #2c3e50;
}

.action-buttons {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 4px;
}

/* กำหนดความกว้างคอลัมน์ */
:deep(.v-data-table) {
  width: 100% !important;
  overflow-x: auto !important;

  .employee-cell {
    min-width: 180px;
    max-width: 200px;
  }

  .organization-cell {
    min-width: 150px;
    max-width: 180px;
  }

  .objective-cell {
    min-width: 200px;
    max-width: 250px;
  }

  .location-cell {
    min-width: 120px;
    max-width: 150px;
  }

  .date-cell {
    min-width: 280px !important; /* เพิ่มความกว้างสำหรับเวลา */
    max-width: none !important; /* ไม่จำกัดความกว้าง */
    width: auto !important;
  }

  .action-cell {
    min-width: 60px;
    width: 60px;
  }
}

/* ปรับ table ให้เลื่อนได้เต็ม */
:deep(.v-data-table__wrapper) {
  overflow-x: auto !important;
  width: 100% !important;
}

:deep(.v-table) {
  width: 100% !important;
  min-width: 1100px !important; /* เพิ่มความกว้างขั้นต่ำ */
}

/* Mobile adjustments */
@media (max-width: 1199px) {
  .employee-info,
  .organization-info,
  .objective-info,
  .location-info {
    max-width: 150px;
    font-size: 0.9rem;
  }

  .date-info {
    min-width: 250px !important;
    max-width: none !important;
    font-size: 0.9rem;
  }

  :deep(.v-data-table) {
    .employee-cell {
      min-width: 160px;
      max-width: 170px;
    }

    .organization-cell {
      min-width: 130px;
      max-width: 150px;
    }

    .objective-cell {
      min-width: 180px;
      max-width: 200px;
    }

    .location-cell {
      min-width: 100px;
      max-width: 120px;
    }

    .date-cell {
      min-width: 250px !important;
      max-width: none !important;
    }
  }

  :deep(.v-table) {
    min-width: 1000px !important;
  }
}

@media (max-width: 959px) {
  .employee-info,
  .organization-info,
  .objective-info,
  .location-info {
    max-width: 120px;
    font-size: 0.85rem;
  }

  .date-info {
    min-width: 220px !important;
    max-width: none !important;
    font-size: 0.85rem;
  }

  :deep(.v-data-table) {
    .employee-cell {
      min-width: 140px;
      max-width: 150px;
    }

    .organization-cell {
      min-width: 110px;
      max-width: 130px;
    }

    .objective-cell {
      min-width: 160px;
      max-width: 180px;
    }

    .location-cell {
      min-width: 90px;
      max-width: 110px;
    }

    .date-cell {
      min-width: 220px !important;
      max-width: none !important;
    }
  }

  :deep(.v-table) {
    min-width: 900px !important;
  }
}

@media (max-width: 599px) {
  .employee-info,
  .organization-info,
  .objective-info,
  .location-info {
    max-width: 100px;
    font-size: 0.8rem;
  }

  .date-info {
    min-width: 180px !important;
    max-width: none !important;
    font-size: 0.8rem;
  }

  :deep(.v-data-table) {
    .employee-cell {
      min-width: 120px;
      max-width: 130px;
    }

    .organization-cell {
      min-width: 100px;
      max-width: 110px;
    }

    .objective-cell {
      min-width: 140px;
      max-width: 160px;
    }

    .location-cell {
      min-width: 80px;
      max-width: 100px;
    }

    .date-cell {
      min-width: 180px !important;
      max-width: none !important;
    }

    .action-cell {
      min-width: 40px;
      width: 40px;
    }
  }

  :deep(.v-table) {
    min-width: 800px !important; /* เพิ่มขนาดใน mobile */
  }
}

/* Table cell padding adjustments */
:deep(.v-data-table) {
  .employee-cell,
  .organization-cell,
  .objective-cell,
  .location-cell,
  .date-cell {
    padding: 12px 8px;
    vertical-align: middle;
  }

  .action-cell {
    padding: 8px;
    vertical-align: middle;
  }
}

@media (max-width: 959px) {
  :deep(.v-data-table) {
    .employee-cell,
    .organization-cell,
    .objective-cell,
    .location-cell,
    .date-cell {
      padding: 8px 6px;
    }

    .action-cell {
      padding: 6px;
      min-width: 50px;
      width: 50px;
    }
  }
}

/* Card table responsive */
.card-table {
  overflow-x: auto !important;
  border-radius: 12px;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.08);
  width: 100% !important;
}

@media (max-width: 959px) {
  .card-table {
    margin: 0 -4px;
    border-radius: 8px;
    overflow-x: auto !important;
  }
}

/* Filter row spacing */
@media (max-width: 959px) {
  .match-height.px-2 {
    margin-bottom: 16px !important;
  }
}

/* Dialog responsive */
:deep(.v-dialog) {
  @media (max-width: 599px) {
    margin: 0;
    max-height: 100vh;
  }
}

/* Table header styling */
:deep(.v-data-table-header) {
  background-color: #f8f9fa;
}

:deep(.v-data-table-header th) {
  font-weight: 600;
  color: #495057;
  white-space: nowrap;
  text-overflow: ellipsis;
  overflow: hidden;
}

/* Hover effect */
:deep(.v-data-table__tr:hover) {
  background-color: rgba(0, 0, 0, 0.04) !important;
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
