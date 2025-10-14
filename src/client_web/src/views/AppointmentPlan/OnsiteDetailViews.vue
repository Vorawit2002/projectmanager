<template>
  <div class="onsite-detail-views-container custom-scrollbar">
    <!-- Card Title like CreateActivityDetail -->
    <v-row class="mt-2 mt-sm-3 mb-3 mb-sm-4">
      <v-col
        cols="12"
        class="d-flex justify-center"
      >
        <div class="d-flex flex-column align-center">
          <span class="text-sub-title text-center">ประวัติการลงเวลา</span>
        </div>
      </v-col>
    </v-row>
    <!-- History Content -->
    <div class="history-wrapper dialog-scrollbar">
      <div class="history-content">
        <!-- Statistics Summary -->
        <div class="stats-summary">
          <v-row class="mb-4">
            <v-col
              cols="12"
              sm="12"
            >
              <v-card class="text-center stats-card">
                <v-card-text class="pa-3">
                  <div class="stats-number text-primary">{{ totalWorkingHours }}</div>
                  <div class="stats-label">ชั่วโมงการทำงานรวม</div>
                </v-card-text>
              </v-card>
            </v-col>
          </v-row>
        </div>

        <!-- Search Box (Date, Department, Name) -->
        <v-row
          class="mb-4"
          justify="end"
        >
          <!-- Department Filter -->
          <v-col
            cols="12"
            sm="6"
            md="3"
          >
            <v-select
              v-model="selectedDepartmentIds"
              :items="departmentItems"
              item-title="name"
              item-value="id"
              label="แผนก"
              variant="outlined"
              multiple
              clearable
              hide-details
              prepend-inner-icon="ri-team-line"
              @update:model-value="onDepartmentChange"
            >
              <template #selection="{ index, item }">
                <v-chip
                  v-if="index === 0"
                  size="small"
                  label
                  class="mr-1"
                >
                  {{ multiDeptMode ? `เลือก ${selectedDepartmentIds.length} แผนก` : item.raw.name }}
                </v-chip>
                <span v-if="index === 1 && selectedDepartmentIds.length > 1">...</span>
              </template>
            </v-select>
          </v-col>
          <!-- Employee Filter (UI only) -->
          <v-col
            cols="12"
            sm="6"
            md="3"
          >
            <v-select
              v-model="selectedEmployeeIds"
              :items="EmployeeList"
              item-title="firstName"
              item-value="id"
              label="พนักงาน"
              variant="outlined"
              multiple
              clearable
              hide-details
              prepend-inner-icon="ri-user-3-line"
              @update:model-value="fetchPageData"
            >
              <!-- <template #selection="{ index, item }">
                <v-chip
                  v-if="index === 0"
                  size="small"
                  label
                  class="mr-1"
                >
                  {{ selectedEmployeeIds.length > 1 ? `เลือก ${selectedEmployeeIds.length} คน` : item.raw.name }}
                </v-chip>
                <span v-if="index === 1 && selectedEmployeeIds.length > 1">...</span>
              </template> -->
            </v-select>
          </v-col>
          <v-col
            cols="12"
            sm="6"
            md="3"
          >
            <v-select
              v-model="selectedRange"
              :items="rangeOptions"
              item-title="title"
              item-value="value"
              label="ช่วงเวลา"
              variant="outlined"
              hide-details
              clearable
              @update:model-value="onRangeChange"
              prepend-inner-icon="ri-time-line"
            />
          </v-col>
        </v-row>

        <!-- History Table -->
        <v-card class="card-table">
          <div class="table-responsive">
            <v-data-table-server
              :headers="historyHeaders"
              :items="attendanceHistory"
              :items-length="totalItems"
              :loading="isLoading"
              class="text-no-wrap"
              v-model:page="pageNumber"
              v-model:items-per-page="pageSize"
              :items-per-page-options="itemsPerPageOptions"
              @update:page="handlePageChange"
              @update:items-per-page="handlePageSizeChange"
            >
              <template v-slot:item="{ item }: any">
                <tr :class="{ 'highlighted-row': item.id === highlightedItemId }">
                  <td>
                    <v-btn
                      color="info"
                      icon
                      density="compact"
                      rounded="lg"
                      @click="viewPhoto(item)"
                      v-tooltip="{
                        text: 'ดูรายละเอียด',
                        contentClass: 'bg-info text-white',
                        location: 'start',
                      }"
                    >
                      <v-icon size="18">ri-article-line</v-icon>
                    </v-btn>
                  </td>
                  <td class="d-flex align-center">
                    <div class="position-relative mr-2">
                      <v-avatar
                        size="40"
                        color="primary"
                        variant="tonal"
                      >
                        <template v-if="item.employees?.imageProfile">
                          <VImg :src="item.employees.imageProfile" />
                        </template>
                        <template v-else>
                          {{ getInitials(item.employees?.firstName, item.employees?.lastName) }}
                        </template>
                      </v-avatar>
                      <!-- Status indicator -->
                      <div
                        class="status-dot position-absolute"
                        :class="getEmployeeStatusClass(item)"
                      ></div>
                    </div>
                    <span>
                      {{ item.employees?.titleName || '' }} {{ item.employees?.firstName || '' }}
                      {{ item.employees?.lastName || '' }}
                    </span>
                  </td>
                  <td>
                    <span>{{ item.date }}</span>
                  </td>
                  <td>
                    <span>{{ item.time }}</span>
                  </td>
                  <td>
                    <v-chip
                      :color="getCheckInCheckOutTypeColor(item.checkInCheckOutTypes)"
                      size="small"
                      variant="flat"
                    >
                      {{ formatCheckInCheckOutType(item.checkInCheckOutTypes) }}
                    </v-chip>
                  </td>
                  <td>
                    <v-tooltip
                      location="top"
                      :content-class="'bg-secondary text-white pa-2 rounded'"
                    >
                      <template #activator="{ props }">
                        <div
                          v-bind="props"
                          class="text-truncate"
                          style="max-width: 300px; overflow: hidden; white-space: nowrap; text-overflow: ellipsis"
                        >
                          {{ item.location || 'ไม่ระบุสถานที่' }}
                        </div>
                      </template>
                      <span>{{ item.location || 'ไม่ระบุสถานที่' }}</span>
                    </v-tooltip>
                  </td>
                  <!-- Working Hours Column - Commented Out -->
                  <!-- <td>
                    <div class="d-flex flex-column">
                      <span
                        v-if="item.workingHours"
                        class="font-weight-bold"
                        >{{
                          typeof item.workingHours === 'object' && item.workingHours.workingHours
                            ? item.workingHours.workingHours
                            : item.workingHours
                        }}</span
                      >
                      <span
                        v-else
                        class="text-grey"
                        >-</span
                      >
                      <span
                        v-if="item.overtime && item.overtime !== ''"
                        class="text-error"
                        style="font-size: 12px"
                        >OT {{ item.overtime }}</span
                      >
                    </div>
                  </td> -->
                </tr>
              </template>
            </v-data-table-server>
          </div>
        </v-card>
      </div>
    </div>

    <!-- OnsiteDetail Drawer -->
    <v-navigation-drawer
      v-model="drawerVisible"
      scrollable
      transition="dialog-right-transition"
      :width="$vuetify.display.xs ? '100vw' : '550'"
      class="z-indexDialog create-activity-drawer"
      close-on-back
      location="right"
      temporary
      :permanent="false"
    >
      <OnsiteDetail
        v-if="drawerVisible"
        :item="selectedItem"
        @close="closeOnsiteDetailDrawer"
      />
    </v-navigation-drawer>

    <!-- Floating Back Button (Bottom Right) -->
    <v-btn
      fab
      fixed
      bottom
      right
      color="error"
      class="floating-back-btn"
      @click="goBack"
    >
      <v-icon>ri-arrow-go-back-line</v-icon>
      <div class="ml-2">ย้อนกลับ</div>
    </v-btn>

    <!-- เลือกวันที่ -->
    <v-dialog
      v-model="DialogSelectedTime"
      :max-width="$vuetify.display.xs ? '100%' : '800px'"
      :max-height="$vuetify.display.xs ? '100vh' : '90vh'"
      transition="dialog-top-transition"
      :fullscreen="$vuetify.display.xs"
    >
      <v-card class="dialog-card">
        <div class="onsite-form-container pa-1 px-4">
          <v-btn
            icon
            variant="text"
            size="small"
            class="close-btn"
            @click="CloseDialogSelectTime"
          >
            <v-icon>ri-close-line</v-icon>
          </v-btn>

          <v-row class="mt-2 mt-sm-3 mb-3 mb-sm-4">
            <v-col
              cols="12"
              class="d-flex align-start justify-start"
            >
              <div class="d-flex flex-column align-start">
                <span class="text-sub-title-dialog text-start">ระบุช่วงเวลา</span>
              </div>
            </v-col>
          </v-row>

          <!-- Check-in/Check-out Content -->
          <v-container
            fluid
            class="checkin-checkout-content pa-0"
          >
            <v-form
              ref="form"
              @submit.prevent="getDatatime"
            >
              <v-row>
                <v-col
                  cols="12"
                  sm="12"
                  md="12"
                >
                  <label class="mb-2">วันที่เริ่มต้น <span class="text-error">*</span> </label>
                  <div>
                    <TextFieldDatepicker
                      placeholder="กรุณาระบุวันที่เริ่มต้นนัดหมาย"
                      :selectedDateTime="request.startDate"
                      :rules="appointmentStartDateRules"
                      :AllDay="true"
                      @selectedDateTime="changeTimestartDate"
                    />
                  </div>
                </v-col>

                <v-col
                  cols="12"
                  sm="12"
                  md="12"
                >
                  <label class="mb-2">วันที่สิ้นสุด <span class="text-error">*</span> </label>
                  <div>
                    <TextFieldDatepicker
                      :selectedDateTime="request.endDate"
                      placeholder="กรุณาระบุวันที่สิ้นสุดนัดหมาย"
                      :rules="appointmentEndDateRules"
                      :minDate="request.startDate"
                      :AllDay="true"
                      @selectedDateTime="changeTimeendDate"
                    />
                  </div>
                </v-col>
                <v-col
                  cols="12"
                  sm="12"
                  md="12"
                  class="align-center mb-4"
                  align="center"
                >
                  <v-btn
                    class=""
                    type="submit"
                    >แสดงข้อมูล</v-btn
                  >
                </v-col>
              </v-row>
            </v-form>
          </v-container>
        </div>
      </v-card>
    </v-dialog>
  </div>
</template>

<script lang="ts">
import {
  CheckInCheckOutType,
  Client,
  GetCheckInCheckOutWithPaginationQuery,
  GetEmployeeByDepartmentIdQuery,
} from '@/client'
import { BACKEND_API_URL } from '@/constants'
import { useSweetAlertStore } from '@/stores'
import { defineComponent } from 'vue'
import OnsiteDetail from './OnsiteDetail.vue'

const client = new Client(BACKEND_API_URL)

export default defineComponent({
  name: 'OnsiteDetailViews',
  components: {
    OnsiteDetail,
  },
  data() {
    return {
      sweetAlert: useSweetAlertStore(),
      isLoading: false,
      searchQuery: '',
      pageNumber: 1,
      pageSize: 10,
      hasFetchedAll: false, // fetched all records (used when search or stats)
      statsInitialized: false,
      activeFilter: null as string | null,
      // Date picker
      datePickerMenu: false,
      selectedDate: null as Date | null,
      selectedDateDisplay: '',
      totalWorkingHours: '00:00:00',
      totalItems: 0 as any,
      // Drawer state
      drawerVisible: false,
      selectedItem: null as any,
      highlightedItemId: null as string | number | null,
      handleEscKey: null as any,
      // Real API data
      attendanceHistory: [] as any,
      // Raw data for statistics calculation
      rawAttendanceData: [] as any,
      // History table headers
      historyHeaders: [
        { title: 'จัดการ', value: 'actions', align: 'center', sortable: false },
        { title: 'ชื่อ', value: 'employees.firstName', sortable: true },
        { title: 'วันที่', value: 'date', sortable: true },
        { title: 'เวลา', value: 'time', sortable: true },
        { title: 'เข้างาน/ออกงาน', value: 'checkInCheckOutTypes', sortable: true },
        { title: 'สถานที่', value: 'location', sortable: false },
        // { title: 'ชั่วโมงทำงาน', value: 'workingHours', sortable: true, width: '150px' },
      ] as any[],
      // Department filter state
      DepartmentList: [] as any[],
      selectedDepartmentIds: [] as string[],
      // Employee filter state (UI only)
      EmployeeList: [] as any[],
      selectedEmployeeIds: [] as string[],
      // Time range selector
      selectedRange: null as any,
      rangeOptions: [
        { title: 'วันนี้', value: 'วันนี้' },
        { title: 'สัปดาห์นี้', value: 'สัปดาห์นี้' },
        { title: 'เดือนนี้', value: 'เดือนนี้' },
        { title: 'ระบุช่วงเวลา', value: 'ระบุ' },
      ] as any[],
      DialogSelectedTime: false,
      // small request placeholder for custom range dialog
      request: {
        startDate: null as Date | null,
        endDate: null as Date | null,
      },
    }
  },
  computed: {
    itemsPerPageOptions() {
      return [
        { value: 10, title: '10' },
        { value: 25, title: '25' },
        { value: 50, title: '50' },
        { value: 100, title: '100' },
        { value: -1, title: 'ทั้งหมด' },
      ]
    },
    departmentItems(): any[] {
      return [{ id: 'ALL', name: 'ทั้งหมด' }, ...this.DepartmentList]
    },
    multiDeptMode(): boolean {
      return this.selectedDepartmentIds.length > 1 && !this.selectedDepartmentIds.includes('ALL')
    },
    effectiveDepartmentIds(): string[] {
      if (this.selectedDepartmentIds.includes('ALL')) return []
      return this.selectedDepartmentIds
    },
    employeeItems(): any[] {
      // UI placeholder list (could be populated later)
      return this.EmployeeList
    },
    appointmentStartDateRules(): any[] {
      return []
    },
    appointmentEndDateRules(): any[] {
      return []
    },
  },
  async mounted() {
    await this.initialize()
  },
  watch: {
    searchQuery: {
      handler() {
        this.pageNumber = 1
        // this.loadAttendanceHistory()
      },
      deep: true,
    },
  },
  beforeUnmount() {
    if (this.handleEscKey) {
      window.removeEventListener('keydown', this.handleEscKey)
    }
  },
  methods: {
    async initialize() {
      try {
        await this.getDepartmentList()
        const emp = await client.getEmployeeQuery()
        this.EmployeeList = emp
        // Load attendance history
        // await this.loadAttendanceHistory()
        await this.fetchPageData()
        // Add ESC key handler
        this.handleEscKey = (e: KeyboardEvent) => {
          if (e.key === 'Escape' || e.key === 'Esc') {
            if (this.drawerVisible) {
              this.closeOnsiteDetailDrawer()
            }
          }
        }
        window.addEventListener('keydown', this.handleEscKey)
      } catch (error) {
        console.error('Initialization error:', error)
        this.sweetAlert.error('เกิดข้อผิดพลาดในการเริ่มต้นระบบ')
      }
    },
    // async loadAttendanceHistory() {
    //   // If user is searching by name -> need full dataset to filter by fullName (API lacks search param)
    //   if (this.searchQuery) {
    //     await this.fetchAllForSearchAndStats()
    //     this.applyFiltersAndPagination()
    //     return
    //   }

    //   // When not searching, use real server-side pagination
    //   await this.fetchPageData()

    //   // Trigger statistics load (once) in background if not yet done
    //   if (!this.statsInitialized) {
    //     this.statsInitialized = true
    //     this.fetchAllForStatistics()
    //   }
    // },
    async fetchPageData() {
      this.isLoading = true
      try {
        const query = new GetCheckInCheckOutWithPaginationQuery({
          pageNumber: this.pageNumber,
          pageSize: this.pageSize === -1 ? 1000 : this.pageSize, // cap to reasonable size
          dateCheck: this.selectedDate ? new Date(this.selectedDate) : undefined,
          departmentId: this.effectiveDepartmentIds.length ? [...this.effectiveDepartmentIds] : undefined,
        })
        query.employeeId = this.selectedEmployeeIds.length ? [...this.selectedEmployeeIds] : undefined
        query.date = this.selectedRange
        query.startDate = this.request.startDate as any
        query.endDate = this.request.endDate as any
        const resp = await client.getCheckInCheckOutQueryWithPagination(query)

        this.attendanceHistory = resp.items || []
        this.totalItems = resp.totalCount || this.attendanceHistory.length
        // Keep a slim copy in rawAttendanceData only for current page to reuse display processing
        this.rawAttendanceData = [...this.attendanceHistory]
        this.processDisplayData(this.attendanceHistory)
      } catch (error) {
        console.error('Failed to load (page) attendance history:', error)
        this.sweetAlert.error('โหลดหน้าข้อมูลไม่สำเร็จ')
      } finally {
        this.isLoading = false
      }
    },

    async handlePageChange(page: number) {
      this.pageNumber = page

      await this.fetchPageData()
    },
    async handlePageSizeChange(size: number) {
      this.pageSize = size
      // Reset to first page when page size changes
      this.pageNumber = 1

      await this.fetchPageData()
    },

    calculateWorkingHours(
      checkinDate: Date | undefined,
      checkoutDate: Date | undefined,
    ): { workingHours: string; overtime: string } | null {
      if (!checkinDate || !checkoutDate) return null

      const checkin = new Date(checkinDate)
      const checkout = new Date(checkoutDate)

      const workStart = new Date(checkin)
      workStart.setHours(8, 30, 0, 0)

      const workEnd = new Date(checkin)
      workEnd.setHours(18, 0, 0, 0)

      const breakStart = new Date(checkin)
      breakStart.setHours(12, 0, 0, 0)

      const breakEnd = new Date(checkin)
      breakEnd.setHours(13, 0, 0, 0)

      const otStart = new Date(checkin)
      otStart.setHours(20, 0, 0, 0)

      // ปรับเวลาเริ่มต้นและสิ้นสุดให้อยู่ในช่วงเวลางาน
      const actualStart = checkin < workStart ? workStart : checkin > workEnd ? workEnd : checkin
      const actualEnd = checkout > workEnd ? workEnd : checkout < workStart ? workStart : checkout

      let workingMs = actualEnd > actualStart ? actualEnd.getTime() - actualStart.getTime() : 0

      // หักช่วงพักเที่ยงถ้าทับกัน
      if (actualStart < breakEnd && actualEnd > breakStart) {
        const breakOverlapStart = actualStart > breakStart ? actualStart : breakStart
        const breakOverlapEnd = actualEnd < breakEnd ? actualEnd : breakEnd
        const breakDuration = breakOverlapEnd.getTime() - breakOverlapStart.getTime()
        workingMs -= breakDuration
      }

      // คำนวณ OT เฉพาะหลัง 20:00
      let overtimeMs = 0
      if (checkout > otStart) {
        overtimeMs = checkout.getTime() - otStart.getTime()
      }

      function msToHMS(ms: number) {
        const hours = Math.floor(ms / (1000 * 60 * 60))
        const minutes = Math.floor((ms % (1000 * 60 * 60)) / (1000 * 60))
        const seconds = Math.floor((ms % (1000 * 60)) / 1000)
        return `${String(hours).padStart(2, '0')}:${String(minutes).padStart(2, '0')}:${String(seconds).padStart(
          2,
          '0',
        )}`
      }

      return {
        workingHours: msToHMS(Math.max(0, workingMs)),
        overtime: overtimeMs > 0 ? msToHMS(overtimeMs) : '',
      }
    },
    processRawData(dataArray: any[]) {
      dataArray.forEach((x: any) => {
        x.status = x.checkOut ? 'complete' : 'incomplete'
        const workingResult = this.calculateWorkingHours(x?.checkIn, x?.checkOut)
        x.workingHours = null
        x.overtime = ''
        if (workingResult && typeof workingResult === 'object') {
          x.workingHours = workingResult.workingHours
          x.overtime = workingResult.overtime
        } else if (typeof workingResult === 'string') {
          x.workingHours = workingResult
        }
      })
    },
    processDisplayData(dataArray: any[]) {
      // Group data by employee and date to calculate working hours properly
      const groupedData = this.groupByEmployeeAndDate(dataArray)

      dataArray.forEach((x: any, index: number) => {
        // Debug: Log first few items to see data structure
        if (index < 3) {
          console.log('Item data:', {
            id: x.id,
            checkIn: x.checkIn,
            checkOut: x.checkOut,
            checkInCheckOutTypes: x.checkInCheckOutTypes,
            employeeId: x.employeeId,
          })
        }

        const date = x.checkIn || x.checkOut
        const rawDate = date ? new Date(date) : new Date()
        x.date = date
          ? rawDate.toLocaleDateString('th-TH', {
              day: 'numeric',
              month: 'long',
              year: 'numeric',
            })
          : '-'
        x.status = x.checkOut ? 'complete' : 'incomplete'

        // Initialize working hours
        x.workingHours = null
        x.overtime = ''

        // Calculate working hours only for the last CheckOut record of the day
        // if (x.checkInCheckOutTypes === CheckInCheckOutType.CheckOut) {
        //   const employeeId = x.employeeId
        //   const dateKey = this.getDateKey(date)
        //   const dayRecords = groupedData[employeeId]?.[dateKey] || []

        //   // Check if this is the last CheckOut of the day
        //   if (this.isLastCheckOutOfDay(x, dayRecords)) {
        //     // Find the first CheckIn of the same day
        //     const firstCheckIn = dayRecords.find((record: any) => record.checkInCheckOutTypes === CheckInCheckOutType.CheckIn)

        //     if (firstCheckIn && firstCheckIn.checkIn && x.checkIn) {
        //       console.log('Calculating working hours from:', firstCheckIn.checkIn, 'to:', x.checkIn)
        //       const workingResult = this.calculateWorkingHours(firstCheckIn.checkIn, x.checkIn)
        //       console.log('Working result:', workingResult)
        //       if (workingResult && typeof workingResult === 'object') {
        //         x.workingHours = workingResult.workingHours
        //         x.overtime = workingResult.overtime
        //       } else if (typeof workingResult === 'string') {
        //         x.workingHours = workingResult
        //       }
        //     }
        //   }
        // }

        // Set time - show any available time without checking type
        x.time = x.checkIn ? this.formatTime(x.checkIn) : x.checkOut ? this.formatTime(x.checkOut) : '-'
        x.created = x.created ? this.formatTime(x.created) : '-'
        x.checkinPhoto = x.checkInImage || null
        x.checkoutPhoto = x.checkOutImage || null
      })
    },

    formatTime(date: Date | undefined): string {
      if (!date) return '-'
      return new Date(date).toLocaleTimeString('th-TH', {
        hour: '2-digit',
        minute: '2-digit',
        second: '2-digit',
      })
    },
    viewPhoto(item: any) {
      this.selectedItem = item
      this.highlightedItemId = item.id
      this.drawerVisible = true
    },
    closeOnsiteDetailDrawer() {
      this.drawerVisible = false
      this.selectedItem = null
    },
    getInitials(firstName: string, lastName: string): string {
      const first = firstName ? firstName.charAt(0).toUpperCase() : ''
      const last = lastName ? lastName.charAt(0).toUpperCase() : ''

      if (first && last) {
        return `${first}${last}`
      } else if (first) {
        return first
      } else if (last) {
        return last
      } else {
        return 'UN'
      }
    },
    getEmployeeStatusClass(item: any): string {
      if (item.status === 'complete') {
        return 'status-complete'
      } else {
        return 'status-incomplete'
      }
    },
    onDateSelected(date: Date | null) {
      this.selectedDate = date
      if (date) {
        this.selectedDateDisplay = new Date(date).toLocaleDateString('th-TH', {
          day: 'numeric',
          month: 'long',
          year: 'numeric',
        })
        this.datePickerMenu = false
        this.pageNumber = 1
        // this.loadAttendanceHistory()
      }
    },
    clearDateFilter() {
      this.selectedDate = null
      this.selectedDateDisplay = ''
      this.pageNumber = 1
      // this.loadAttendanceHistory()
    },
    async getDepartmentList() {
      try {
        this.DepartmentList = await client.getDepartmentQuery()
      } catch (error) {
        console.error('Error fetching departments:', error)
      }
    },
    // Range selector handlers
    async onRangeChange() {
      if (!this.selectedRange?.includes('ระบุ')) {
        this.pageNumber = 1
        await this.fetchPageData()
      } else {
        this.DialogSelectedTime = true
      }
    },
    CloseDialogSelectTime() {
      this.DialogSelectedTime = false
    },
    async getDatatime() {
      // Form submit from dialog: assume request.startDate/endDate have been set by child component

      // Prepare display text
      this.selectedRange = 'ระบุช่วงเวลา'
      if (this.request && this.request.startDate && this.request.endDate) {
        const s = new Date(this.request.startDate).toLocaleDateString('th-TH')
        const e = new Date(this.request.endDate).toLocaleDateString('th-TH')
        this.selectedDateDisplay = `${s} - ${e}`
      }
      this.pageNumber = 1
      await this.fetchPageData()
      this.DialogSelectedTime = false
      this.request.startDate = null
      this.request.endDate = null
      // await this.loadAttendanceHistory()
    },
    changeTimestartDate(date: Date) {
      this.request.startDate = date
    },
    changeTimeendDate(date: Date) {
      this.request.endDate = date
    },
    async onDepartmentChange() {
      console.log(this.selectedDepartmentIds)
      if (this.selectedDepartmentIds.includes('All')) {
        const emp = await client.getEmployeeQuery()
        this.EmployeeList = emp
      } else if (this.selectedDepartmentIds.length > 0) {
        for (let i = 0; i < this.selectedDepartmentIds.length; i++) {
          let query = new GetEmployeeByDepartmentIdQuery()
          query.departmentId = this.selectedDepartmentIds[i]
          if (i == 0) {
            const emp = await client.getEmployeeQueryByDepartmentId(query)
            this.EmployeeList = emp
          } else {
            const emp = await client.getEmployeeQueryByDepartmentId(query)
            this.EmployeeList = [...this.EmployeeList, ...emp]
          }
        }
      }
      await this.fetchPageData()
      // // Handle ALL sentinel
      // if (this.selectedDepartmentIds.includes('ALL')) {
      //   this.selectedDepartmentIds = ['ALL']
      // }
      // this.pageNumber = 1
      // if (this.searchQuery) {
      //   // refresh full dataset with new department filter then apply
      //   await this.fetchAllForSearchAndStats()
      //   this.applyFiltersAndPagination()
      // } else {
      //   // await this.loadAttendanceHistory()
      // }
    },
    goBack() {
      this.$router.push({ name: 'OnsiteViews' })
    },

    formatCheckInCheckOutType(type: CheckInCheckOutType | undefined): string {
      if (type === CheckInCheckOutType.CheckIn) {
        return 'เข้างาน'
      } else if (type === CheckInCheckOutType.CheckOut) {
        return 'ออกงาน'
      } else {
        return '-'
      }
    },
    getCheckInCheckOutTypeClass(type: CheckInCheckOutType | undefined): string {
      if (type === CheckInCheckOutType.CheckIn) {
        return 'text-success'
      } else if (type === CheckInCheckOutType.CheckOut) {
        return 'text-error'
      } else {
        return 'text-grey'
      }
    },
    getCheckInCheckOutTypeColor(type: CheckInCheckOutType | undefined): string {
      if (type === CheckInCheckOutType.CheckIn) {
        return 'success'
      } else if (type === CheckInCheckOutType.CheckOut) {
        return 'error'
      } else {
        return 'grey'
      }
    },
    groupByEmployeeAndDate(dataArray: any[]): any {
      const grouped: any = {}

      dataArray.forEach(item => {
        const employeeId = item.employeeId
        const date = item.checkIn || item.checkOut
        const dateKey = this.getDateKey(date)

        if (!grouped[employeeId]) {
          grouped[employeeId] = {}
        }
        if (!grouped[employeeId][dateKey]) {
          grouped[employeeId][dateKey] = []
        }

        grouped[employeeId][dateKey].push(item)
      })

      return grouped
    },
    getDateKey(date: Date | undefined): string {
      if (!date) return 'unknown'
      const d = new Date(date)
      return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`
    },
    isLastCheckOutOfDay(currentItem: any, dayRecords: any[]): boolean {
      const checkOutRecords = dayRecords.filter(
        record => record.checkInCheckOutTypes === CheckInCheckOutType.CheckOut && record.checkIn,
      )

      if (checkOutRecords.length === 0) return false

      // Find the latest checkout time (using checkIn field for CheckOut records)
      const latestCheckOut = checkOutRecords.reduce((latest, current) => {
        const currentTime = new Date(current.checkIn).getTime()
        const latestTime = new Date(latest.checkIn).getTime()
        return currentTime > latestTime ? current : latest
      })

      return currentItem.id === latestCheckOut.id
    },
    getFirstCheckInOfDay(dayRecords: any[]): any {
      const checkInRecords = dayRecords.filter(
        record => record.checkInCheckOutTypes === CheckInCheckOutType.CheckIn && record.checkIn,
      )

      if (checkInRecords.length === 0) return null

      // Find the earliest checkin time
      return checkInRecords.reduce((earliest, current) => {
        const currentTime = new Date(current.checkIn).getTime()
        const earliestTime = new Date(earliest.checkIn).getTime()
        return currentTime < earliestTime ? current : earliest
      })
    },
  },
})
</script>

<style scoped>
.onsite-detail-views-container {
  height: 100%;
}

/* Header Styles matching CreateActivityDetail */
.text-sub-title {
  font-size: clamp(20px, 4vw, 30px);
  font-weight: 700;
  color: #2b3086;
  background: linear-gradient(135deg, #2b3086 0%, #4a6cf7 100%);
  background-clip: text;
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  text-shadow: 0 2px 4px rgba(43, 48, 134, 0.2);
  letter-spacing: -0.5px;
}

/* History Wrapper */
.history-wrapper {
  min-height: calc(100vh - 220px);
}

/* History Content */
.history-content {
  padding: 20px 16px;
}

.stats-summary {
  margin-bottom: 20px;
}

.stats-card {
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  transition: transform 0.2s ease;
}

.clickable-stats {
  cursor: pointer;
  transition: all 0.3s ease;
}

.clickable-stats:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.active-filter-green {
  border: 2px solid #4caf50;
  background-color: #e8f5e8;
}

.active-filter-yellow {
  border: 2px solid #ff9800;
  background-color: #fff8e1;
}

.stats-number {
  font-size: 24px;
  font-weight: 700;
  line-height: 1;
  margin-bottom: 8px;
}

.stats-label {
  font-size: 12px;
  color: #666;
  font-weight: 500;
}

.highlighted-row {
  background-color: #e0f2f7 !important;
  transition: background-color 0.3s ease;
}

.v-data-table .highlighted-row td {
  background-color: #e0f2f7 !important;
}

/* Status dot styling */
.status-dot {
  width: 14px;
  height: 14px;
  border-radius: 50%;
  border: 1px solid #fff;
  bottom: -2px;
  right: -2px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
}

.status-complete {
  background-color: #4caf50;
}

.status-incomplete {
  background-color: #ff9800;
}

/* Drawer CSS Classes */
.z-indexDialog {
  z-index: 9999;
}

.create-activity-drawer {
  z-index: 9999;
  background: #fff !important;
}

/* Floating Back Button */
.floating-back-btn {
  z-index: 1000 !important;
  position: fixed !important;
  bottom: 55px !important;
  right: 50px !important;
  left: auto !important;
}

@media (max-width: 599px) {
  .floating-back-btn {
    bottom: 20px !important;
    right: 20px !important;
    width: 56px;
    height: 56px;
  }

  .floating-back-btn .ml-2 {
    display: none;
  }
}

@media (max-width: 959px) {
  .floating-back-btn {
    bottom: 30px !important;
    right: 30px !important;
  }
}

/* Responsive Design */
@media (max-width: 767.98px) {
  .create-activity-drawer:not(.v-navigation-drawer--active) {
    transform: translateX(100%) !important;
    visibility: hidden !important;
  }
}

@media (max-width: 599px) {
  .create-activity-drawer {
    border-radius: 0 !important;
  }
}

@media (max-width: 959px) {
  .text-sub-title {
    font-size: 20px;
  }
}

/* Table Responsive */
.table-responsive {
  overflow-x: auto;
  -webkit-overflow-scrolling: touch;
}

@media (max-width: 1023px) {
  .table-responsive {
    margin: 0 -16px;
    padding: 0 16px;
  }
}

@media (max-width: 599px) {
  .text-sub-title {
    font-size: 18px;
    text-align: center;
    margin-bottom: 10px;
  }

  .history-content {
    padding: 16px 8px;
  }

  .stats-summary .v-col {
    padding: 6px;
  }

  .stats-card {
    margin-bottom: 8px;
  }

  .stats-number {
    font-size: 20px;
  }

  .stats-label {
    font-size: 11px;
  }

  .table-responsive {
    margin: 0 -8px;
    padding: 0 8px;
  }
}

@media (max-width: 479px) {
  .history-content {
    padding: 12px 4px;
  }

  .stats-number {
    font-size: 18px;
  }

  .stats-label {
    font-size: 10px;
  }

  .table-responsive {
    margin: 0 -4px;
    padding: 0 4px;
  }
}

.dialog-card {
  border-radius: 16px !important;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.15) !important;
  overflow: hidden;
  border: none !important;
}

.onsite-form-container {
  height: 100%;
  background-color: #f5f5f5;
}

/* Close Button - Fixed top right */
.close-btn {
  position: absolute !important;
  top: 16px !important;
  right: 16px !important;
  z-index: 2010 !important;
  background: rgba(255, 255, 255, 0.9) !important;
  backdrop-filter: blur(10px);
  border: none !important;
  color: #2b3086 !important;
  transition: all 0.2s ease;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.close-btn:hover {
  background: rgba(43, 48, 134, 0.1) !important;
  transform: scale(1.05);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

/* Header Styles matching CreateActivityDetail */
.text-sub-title-dialog {
  font-size: clamp(20px, 4vw, 20px);
  font-weight: 600;
  color: #2b3086;
  background: linear-gradient(135deg, #2b3086 0%, #4a6cf7 100%);
  background-clip: text;
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  text-shadow: 0 2px 4px rgba(43, 48, 134, 0.2);
  letter-spacing: -0.5px;
}
</style>
