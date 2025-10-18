<template>
  <VRow>
    <VCol cols="12">
      <v-card class="cardCalendar">
        <v-row class="pa-2 pa-sm-4">
          <v-col
            cols="12"
            md="3"
            class="mb-2 mb-md-4"
          >
            <!-- Create Dropdown (Menu) -->
            <v-menu offset-y>
              <template v-slot:activator="{ props }">
                <v-btn
                  block
                  v-bind="props"
                  class="add-activity-btn"
                  color="primary"
                  size="default"
                >
                  <v-icon
                    class="mr-2"
                    size="small"
                    >ri-add-circle-line</v-icon
                  >
                  <span class="d-none d-sm-inline">สร้าง</span>
                  <span class="d-inline d-sm-none">สร้าง</span>
                  <v-icon
                    class="ml-2"
                    size="small"
                    >ri-arrow-down-s-line</v-icon
                  >
                </v-btn>
              </template>
              <v-list
                min-width="240"
                style="border-radius: 17px"
              >
                <v-list-item @click="openCreateActivityDetail()">
                  <template v-slot:prepend>
                    <v-icon>ri-contract-line</v-icon>
                  </template>
                  <v-list-item-title>นัดหมายใหม่</v-list-item-title>
                  <v-list-item-subtitle class="text-wrap"
                    >การนัดหมายลูกค้าที่ต้องการบันทึกรายงานผล</v-list-item-subtitle
                  >
                </v-list-item>
                <v-divider></v-divider>
                <v-list-item @click="openCreateCustomerDailySchedule()">
                  <template v-slot:prepend>
                    <v-icon>ri-save-3-fill</v-icon>
                  </template>
                  <v-list-item-title>แจ้งงานรายวันใหม่</v-list-item-title>
                  <v-list-item-subtitle class="text-wrap">แผนงานในแต่ละวัน (กรณีที่อยู่ office)</v-list-item-subtitle>
                </v-list-item>
                <v-divider></v-divider>
                <v-list-item @click="openUpdateCustomerDailySchedule()">
                  <template v-slot:prepend>
                    <v-icon>ri-file-copy-line</v-icon>
                  </template>
                  <v-list-item-title>สำเนาแจ้งงานรายวันล่าสุด</v-list-item-title>
                  <v-list-item-subtitle class="text-wrap">สำเนาแจ้งงานรายวันจากวันล่าสุด</v-list-item-subtitle>
                </v-list-item>
              </v-list>
            </v-menu>
            <v-divider
              class="mt-3 opacity-25"
              :thickness="2"
            ></v-divider>

            <VRow class="d-flex mt-0 justify-center">
              <v-col
                cols="12"
                md="12"
              >
                <label class="mb-3 text-black">เลือกปี </label>
                <v-autocomplete
                  :items="YearList"
                  item-title="title"
                  item-value="value"
                  placeholder="กรุณาเลือกเลือกปี"
                  clearable
                  v-model="request.years"
                  @update:model-value="onYearChange"
                  density="comfortable"
                  variant="outlined"
                  class="form-field"
                />
              </v-col>

              <v-col
                cols="12"
                md="12"
                v-if="auth.roles.includes('Admin') || auth.roles.includes('Manager')"
              >
                <label class="mb-3 text-black">แผนก </label>
                <v-autocomplete
                  :items="departmentItems"
                  item-title="name"
                  item-value="id"
                  placeholder="กรุณาเลือกแผนก"
                  multiple
                  chips
                  clearable
                  v-model="selectedDepartmentIds"
                  @update:model-value="onDepartmentMultiChange"
                  density="comfortable"
                  variant="outlined"
                  class="form-field"
                />
              </v-col>

              <v-col
                cols="12"
                md="12"
                v-if="auth.roles.includes('Admin') || auth.roles.includes('Manager')"
              >
                <label class="mb-3 text-black">พนักงาน </label>
                <v-autocomplete
                  :items="EmployeeList"
                  item-title="name"
                  item-value="id"
                  placeholder="กรุณาเลือกพนักงาน"
                  multiple
                  clearable
                  chips
                  v-model="selectedEmployeeIds"
                  :disabled="selectedDepartmentIds.length === 0"
                  @update:model-value="onEmployeeChange"
                  density="comfortable"
                  variant="outlined"
                  class="form-field"
                />
              </v-col>
            </VRow>
            <v-row>
              <v-col
                cols="12"
                style="margin-bottom: -10px"
              >
                <label class="mb-0 text-black">{{ 'ประเภท' }}</label>
              </v-col>
              <v-col
                cols="12"
                md="6"
                v-for="(type, index) in EventTypeList"
              >
                <!-- <v-checkbox-group
                  v-model="request.eventTypeId"
                  class="form-field"
                  column
                > -->
                <v-checkbox
                  :key="type.id"
                  :label="type.name"
                  :value="type.id"
                  v-model="request.eventTypeId"
                />
                <!-- </v-checkbox-group> -->
              </v-col>
            </v-row>
            <!-- Button Section with improved layout -->
            <v-row class="button-section mt-4 justify-end">
              <v-col
                cols="auto"
                class="pa-1"
              >
                <v-btn
                  color="grey-darken-1"
                  class="reset-btn"
                  @click="resetFilters()"
                  variant="outlined"
                  :loading="isLoading"
                  :disabled="isLoading"
                  size="default"
                >
                  <v-icon
                    icon="ri-refresh-line"
                    size="18"
                    class="mr-1"
                  />
                  รีเซ็ต
                </v-btn>
              </v-col>
              <v-col
                cols="auto"
                class="pa-1"
              >
                <v-btn
                  color="primary"
                  class="search-btn"
                  @click="initialize()"
                  variant="elevated"
                  :loading="isLoading"
                  :disabled="isLoading"
                  size="default"
                >
                  <v-icon
                    icon="ri-search-line"
                    size="18"
                    class="mr-1"
                  />
                  ค้นหา
                </v-btn>
              </v-col>
            </v-row>
          </v-col>

          <v-col
            cols="12"
            md="9"
          >
            <div
              class="calendarWidth custom-scrollbar"
              ref="calendar"
            ></div>
          </v-col>
        </v-row>
      </v-card>
    </VCol>
  </VRow>

  <!-- Enhanced responsive dialog -->
  <v-dialog
    v-model="dialog"
    :max-width="$vuetify.display.xs ? '100vw' : $vuetify.display.sm ? '90vw' : '65vw'"
    transition="dialog-top-transition"
    :fullscreen="$vuetify.display.xs"
    scrollable
  >
    <v-card
      class="premium-appointment-card"
      elevation="0"
    >
      <!-- Enhanced responsive hero header -->
      <div class="hero-header">
        <div class="glass-overlay"></div>
        <div class="hero-content">
          <div class="hero-badge">
            <i class="ri-calendar-check-line hero-icon"></i>
          </div>
          <div class="hero-text">
            <h1 class="hero-title">
              {{ !request.employeeId ? 'แผนการนัดหมายลูกค้าของ' : 'แผนการนัดหมายลูกค้าเรื่อง' }}
            </h1>
            <h2 class="hero-subtitle">{{ selectedEvent.title }}</h2>
            <div class="hero-date">
              <i class="ri-time-line date-icon"></i>
              <span class="date-text">{{ selectedEvent.date }}</span>
            </div>
          </div>
        </div>
        <v-btn
          icon
          variant="text"
          color="#37474f"
          size="large"
          class="close-button"
          @click="dialog = false"
        >
          <i class="ri-close-line close-icon"></i>
        </v-btn>
      </div>

      <!-- Enhanced scrollable content -->
      <v-card-text class="premium-content">
        <v-container
          fluid
          class="content-container"
        >
          <!-- Enhanced responsive info cards -->
          <v-row class="info-row">
            <v-col
              cols="12"
              sm="6"
              lg="4"
            >
              <div class="modern-info-card primary-card">
                <div class="card-header">
                  <div class="card-avatar primary-avatar">
                    <i class="ri-building-line avatar-icon"></i>
                  </div>
                  <div class="card-header-text">
                    <span class="card-category">หน่วยงาน</span>
                    <h3 class="card-title">{{ selectedEvent.organizations?.name || 'ไม่ระบุ' }}</h3>
                  </div>
                </div>
              </div>
            </v-col>
            <v-col
              cols="12"
              sm="6"
              lg="4"
            >
              <div class="modern-info-card success-card">
                <div class="card-header">
                  <div class="card-avatar success-avatar">
                    <i class="ri-map-pin-line avatar-icon"></i>
                  </div>
                  <div class="card-header-text">
                    <span class="card-category">สถานที่</span>
                    <h3 class="card-title">{{ selectedEvent.location || 'ไม่ระบุ' }}</h3>
                  </div>
                </div>
              </div>
            </v-col>
            <v-col
              cols="12"
              sm="12"
              lg="4"
            >
              <div class="modern-info-card warning-card">
                <div class="card-header">
                  <div class="card-avatar warning-avatar">
                    <i class="ri-user-3-line avatar-icon"></i>
                  </div>
                  <div class="card-header-text">
                    <span class="card-category">ผู้รับผิดชอบ</span>
                    <h3 class="card-title">{{ selectedEvent.employees?.employeeFullName || 'ไม่ระบุ' }}</h3>
                  </div>
                </div>
              </div>
            </v-col>
          </v-row>

          <!-- Enhanced detail section -->
          <v-row class="detail-row">
            <v-col
              cols="12"
              :lg="selectedEvent.cost !== null ? 8 : 12"
            >
              <div class="detail-panel">
                <div class="panel-content">
                  <div
                    class="objective-section"
                    v-if="selectedEvent?.eventType?.eventTypeCode == '001'"
                  >
                    <h5 class="details-label">
                      <i class="ri-target-line details-icon"></i>
                      วัตถุประสงค์
                    </h5>
                    <p class="details-text">{{ selectedEvent.objective }}</p>
                  </div>
                  <v-divider
                    class="my-4"
                    v-if="selectedEvent.detail"
                  />
                  <div
                    class="additional-details"
                    v-if="selectedEvent.detail"
                  >
                    <h5 class="details-label">
                      <i class="ri-file-text-line details-icon"></i>
                      รายละเอียดเพิ่มเติม
                    </h5>
                    <p class="details-text">{{ selectedEvent.detail }}</p>
                  </div>
                </div>
              </div>
            </v-col>
            <v-col
              cols="12"
              lg="4"
              v-if="selectedEvent.cost !== null"
            >
              <div class="cost-panel">
                <div class="cost-header">
                  <span class="cost-label">ค่าใช้จ่าย</span>
                </div>
                <div class="cost-amount">
                  <span class="amount">{{ selectedEvent.cost?.toLocaleString() || '0' }}</span>
                  <span class="currency">บาท</span>
                </div>
              </div>
            </v-col>
          </v-row>

          <!-- Enhanced map section -->
          <v-row>
            <v-col
              cols="12"
              class="mt-2 mt-md-4"
            >
              <h5 class="details-label mb-3">
                <i class="ri-map-pin-line details-icon"></i>
                พิกัดหน่วยงาน
              </h5>

              <!-- กรณีมีพิกัด -->
              <div
                v-if="isValidCoordinates"
                class="map-container"
              >
                <iframe
                  :src="googleMapsUrl"
                  width="100%"
                  height="300"
                  style="border: 0; border-radius: 12px"
                  loading="lazy"
                  referrerpolicy="no-referrer-when-downgrade"
                ></iframe>
              </div>

              <!-- กรณีไม่มีพิกัด -->
              <div
                v-else
                class="map-placeholder"
              >
                <v-card
                  height="200"
                  class="d-flex align-center justify-center"
                  color="grey-lighten-4"
                  rounded="xl"
                >
                  <div class="text-center">
                    <v-icon
                      size="48"
                      color="grey-darken-1"
                      class="mb-3"
                    >
                      mdi-map-marker-outline
                    </v-icon>
                    <p class="text-grey-darken-1 text-body-2">
                      {{ coordinatesMessage }}
                    </p>
                  </div>
                </v-card>
              </div>
            </v-col>
          </v-row>
        </v-container>
      </v-card-text>

      <!-- Enhanced responsive action bar -->
      <v-card-actions class="premium-actions">
        <v-container
          fluid
          class="action-container"
        >
          <div class="action-bar">
            <!-- ปุ่มสำหรับ นัดหมาย -->
            <v-menu>
              <template v-slot:activator="{ props }">
                <v-btn
                  v-if="!isSelectedEventTypeDailySchedule()"
                  variant="elevated"
                  size="large"
                  color="primary"
                  class="action-button primary-button"
                  :block="$vuetify.display.xs"
                  v-bind="props"
                >
                  <i class="ri-settings-5-line btn-icon"></i>
                  <span class="btn-text ml-2">จัดการการนัดหมาย</span>
                </v-btn>
              </template>
              <v-list>
                <v-list-item rounded="md">
                  <v-list-item-title
                    class="text-primary font-weight-medium cursor-pointer"
                    @click="goSettingActivityPlan('activity')"
                    >แก้ไขนัดหมาย</v-list-item-title
                  >
                </v-list-item>
                <v-list-item rounded="md">
                  <v-list-item-title
                    class="text-primary font-weight-medium cursor-pointer"
                    @click="goSettingActivityPlan('report')"
                    >{{ selectedEvent?.hasPlanNote ? 'แก้ไขสรุปผลนัดหมาย' : 'บันทึกสรุปผลนัดหมาย' }}</v-list-item-title
                  >
                </v-list-item>
              </v-list>
            </v-menu>
          </div>
        </v-container>
      </v-card-actions>
    </v-card>
  </v-dialog>

  <v-navigation-drawer
    v-model="manageActivityDialog"
    :width="$vuetify.display.xs ? '100vw' : '550'"
    class="z-indexDialog create-activity-drawer"
    close-on-back
    transition="dialog-right-transition"
    temporary
    location="right"
    scrollable
    :permanent="false"
  >
    <ManageActivityDetail
      v-if="manageActivityDialog"
      :activitytab="activitytab"
      :id="selectedActivityId != null ? String(selectedActivityId) : ''"
      @close="closeManageActivityDialog"
    />
  </v-navigation-drawer>
  <v-navigation-drawer
    v-model="showCreateActivityDetail"
    :width="$vuetify.display.xs ? '100vw' : '550'"
    class="z-indexDialog create-activity-drawer"
    close-on-back
    transition="dialog-right-transition"
    temporary
    location="right"
    scrollable
    :permanent="false"
  >
    <CreateActivityDetail
      v-if="showCreateActivityDetail"
      :CloseDialogCreated="CloseDialogCreated"
    />
  </v-navigation-drawer>
  <v-navigation-drawer
    v-model="showCreateCustomerDailySchedule"
    :width="$vuetify.display.xs ? '100vw' : '550'"
    class="z-indexDialog create-activity-drawer"
    close-on-back
    transition="dialog-right-transition"
    temporary
    location="right"
    scrollable
    :permanent="false"
  >
    <CreateCustomerDailySchedule
      v-if="showCreateCustomerDailySchedule"
      :CloseDialogCreate="CloseDialogCreate"
    />
  </v-navigation-drawer>
  <v-navigation-drawer
    v-model="showUpdateCustomerDailySchedule"
    :width="$vuetify.display.xs ? '100vw' : '550'"
    class="z-indexDialog create-activity-drawer"
    close-on-back
    transition="dialog-right-transition"
    temporary
    location="right"
    scrollable
    :permanent="false"
  >
    <UpdateCustomerDailySchedule
      v-if="showUpdateCustomerDailySchedule"
      :id="String(selectedDailyScheduleId)"
      :mode="mode"
      :selectedDate="selectedEvent.startDate"
      :CloseDialogUpdate="closeUpdateDailyScheduleDialog"
    />
  </v-navigation-drawer>
</template>

<script lang="ts">
import {
  Client,
  GetActivityPlanByEmployeeIdQuery,
  GetActivityPlanLatestQuery,
  GetEmployeeByDepartmentIdQuery,
} from '@/client'

import CreateActivityDetail from '@/components/CreateActivityDetail.vue'
import { BACKEND_API_URL } from '@/constants'
import { useAuthStore, useSweetAlertStore } from '@/stores'
import CreateCustomerDailySchedule from '@/views/AppointmentPlan/CreateCustomerDailySchedule.vue'
import ManageActivityDetail from '@/views/AppointmentPlan/ManageActivityDetail.vue'
import UpdateCustomerDailySchedule from '@/views/AppointmentPlan/UpdateCustomerDailySchedule.vue'
import { Calendar } from '@fullcalendar/core'
import thLocale from '@fullcalendar/core/locales/th'
import dayGridPlugin from '@fullcalendar/daygrid'
import interactionPlugin from '@fullcalendar/interaction'
import timeGridPlugin from '@fullcalendar/timegrid'
import moment from 'moment'
import 'moment/locale/th'
import tippy from 'tippy.js'

const client = new Client(BACKEND_API_URL)
moment.locale('th')

export default {
  components: {
    CreateActivityDetail,
    CreateCustomerDailySchedule,
    ManageActivityDetail,
    UpdateCustomerDailySchedule,
  },
  props: {
    activeTab: {
      type: String,
      default: 'Card',
    },
  },
  // ล้างฟิลเตอร์เมื่อออกจากหน้า (ไม่ใช่รีเฟรช)
  beforeRouteLeave(to, from, next) {
    // ล้างค่าฟิลเตอร์ใน localStorage เมื่อออกจากหน้าไปยังหน้าอื่น
    console.log('🚪 Leaving calendar page, clearing filters')
    localStorage.removeItem('calendar-filters')
    next()
  },
  beforeUnmount() {
    // ลบ event listener esc
    if (this.handleEscKey) window.removeEventListener('keydown', this.handleEscKey)
    // บังคับปิด drawer ก่อน unmount
    this.DialogCreate = false
    this.dialog = false
    this.manageActivityDialog = false
    this.updateDailyScheduleDialog = false
    console.log('🔧 Before unmount, all dialogs closed')
  },
  created() {
    // บังคับปิด drawer เมื่อ component ถูกสร้าง
    this.DialogCreate = false
    this.dialog = false
    this.manageActivityDialog = false
    this.updateDailyScheduleDialog = false
    console.log('🔧 Component created, all dialogs initialized as closed')
  },
  beforeMount() {
    // บังคับปิด drawer ก่อน mount
    this.DialogCreate = false
    this.dialog = false
    this.manageActivityDialog = false
    this.updateDailyScheduleDialog = false
    console.log('🔧 Before mount, all dialogs closed')
  },
  data() {
    return {
      handleEscKey: null as any,
      DepartmentList: [] as any,
      EmployeeList: [] as any,
      selectedDepartmentIds: [] as string[],
      selectedEmployeeIds: [] as string[],
      lastSelectionIsAllDept: false as boolean,
      lastSelectionIsAllEmp: false as boolean,
      employeeAllSelected: false as boolean,
      defaultDepartmentId: '' as string | null,
      defaultEmployeeId: '' as string | null,
      calendar: null as any,
      dateTime: new Date(),
      dialog: false,
      DialogCreate: false,
      DialogUpdate: false,
      showCreateActivityDetail: false,
      showCreateCustomerDailySchedule: false,
      showUpdateCustomerDailySchedule: false,
      id: '' as any,
      manageActivityDialog: false,
      updateDailyScheduleDialog: false,
      selectedActivityId: null as string | number | null,
      selectedDailyScheduleId: null as string | number | null,
      selectedEvent: {} as any,
      activityquery: {} as any,
      mode: '' as any, // เพิ่ม property mode เพื่อใช้สำหรับโหมดต่างๆ เช่น 'duplicate'
      employeeColors: {} as Record<string, { bg: string; border: string }>,
      nextColorIndex: 0, // ลำดับสีที่กำลังจะใช้
      isLoading: false, // สถานะการโหลด
      colorPalette: [
        { bg: '#3B82F6', border: '#3B82F6' },
        { bg: '#EF4444', border: '#EF4444' },
        { bg: '#22C55E', border: '#22C55E' },
        { bg: '#F97A00', border: '#F97A00' },
        { bg: '#EAB308', border: '#EAB308' },
        { bg: '#8B5CF6', border: '#8B5CF6' },
        { bg: '#EA5B6F', border: '#EA5B6F' },
        { bg: '#615DEC', border: '#615DEC' },
        { bg: '#2b3086', border: '#2b3086' },
        { bg: '#FB7185', border: '#FB7185' },
        { bg: '#10B981', border: '#10B981' },
        { bg: '#F59E0B', border: '#F59E0B' },
        { bg: '#6366F1', border: '#6366F1' },
        { bg: '#14B8A6', border: '#14B8A6' },
        { bg: '#F43F5E', border: '#F43F5E' },
        { bg: '#BF9264', border: '#BF9264' },
        { bg: '#DC2626', border: '#DC2626' },
        { bg: '#7C3AED', border: '#7C3AED' },
        { bg: '#BE185D', border: '#BE185D' },
        { bg: '#0F766E', border: '#0F766E' },
        { bg: '#0EA5E9', border: '#0EA5E9' },
        { bg: '#16A34A', border: '#16A34A' },
        { bg: '#0369A1', border: '#0369A1' },
        { bg: '#7E4EE6', border: '#7E4EE6' },
        { bg: '#56CA00', border: '#56CA00' },
        { bg: '#D97706', border: '#D97706' },
        { bg: '#7C7F84', border: '#7C7F84' },
        { bg: '#15803D', border: '#15803D' },
      ] as any,
      YearList: [
        { title: '2568', value: '2025' },
        { title: '2567', value: '2024' },
        { title: '2566', value: '2023' },
        { title: '2565', value: '2022' },
        { title: '2564', value: '2021' },
        { title: '2563', value: '2020' },
        { title: '2562', value: '2019' },
        { title: '2561', value: '2018' },
        { title: '2560', value: '2017' },
        { title: '2559', value: '2016' },
        { title: '2558', value: '2015' },
      ],
      holidayDates: [
        // '2025-01-01',
        // '2025-02-12',
        // '2025-04-06',
        // '2025-04-13',
        // '2025-04-14',
        // '2025-04-15',
        // '2025-05-01',
        // '2025-05-05',
        // '2025-05-12',
        // '2025-06-03',
        // '2025-07-10',
        // '2025-07-28',
        // '2025-08-12',
        // '2025-10-13',
        // '2025-10-23',
        // '2025-12-05',
        // '2025-12-10',
        // '2025-12-31',
      ] as any,
      auth: useAuthStore(),
      sweetAlertStore: useSweetAlertStore(),
      request: new GetActivityPlanByEmployeeIdQuery(),
      activity: [] as any,
      calendarObject: {} as any,
      requestEmployee: new GetEmployeeByDepartmentIdQuery(),
      EventTypeList: [] as any,
      activitytab: 'EditActivity' as any,
    }
  },
  async mounted() {
    // บังคับปิด drawer เมื่อโหลดหน้า
    this.DialogCreate = false
    this.dialog = false
    this.manageActivityDialog = false
    this.updateDailyScheduleDialog = false

    // เพิ่ม event listener สำหรับ keydown (esc) เพื่อปิด drawer ทุกตัว
    this.handleEscKey = (e: KeyboardEvent) => {
      if (e.key === 'Escape' || e.key === 'Esc') {
        if (this.showCreateActivityDetail) this.showCreateActivityDetail = false
        if (this.showCreateCustomerDailySchedule) this.showCreateCustomerDailySchedule = false
        if (this.showUpdateCustomerDailySchedule) this.showUpdateCustomerDailySchedule = false
        if (this.DialogCreate) this.DialogCreate = false
        if (this.manageActivityDialog) this.manageActivityDialog = false
        if (this.updateDailyScheduleDialog) this.updateDailyScheduleDialog = false
      }
    }
    window.addEventListener('keydown', this.handleEscKey)

    this.calendarObject = {
      locale: thLocale, // หรือใช้ 'th' ถ้าโหลด locale มาแล้ว
      plugins: [dayGridPlugin, interactionPlugin, timeGridPlugin],
      // businessHours: [{ daysOfWeek: [1, 2, 3, 4, 5] }],
      firstDay: 0,

      initialView: 'dayGridMonth',
      dayMaxEvents: 3,
      customButtons: {
        myPrev: {
          text: '◀',
          click: () => {
            this.calendar.prev()
          },
        },
        myNext: {
          text: '▶',
          click: () => {
            this.calendar.next()
          },
        },
        myToday: {
          text: 'วันนี้',
          click: () => {
            this.calendar.today()
          },
        },
      },
      headerToolbar: {
        right: 'myPrev,myNext myToday',
        center: 'title',
        left: 'dayGridMonth,timeGridWeek,timeGridDay',
      },
      dayCellDidMount: (arg: any) => {
        const year = arg.date.getFullYear()
        const month = String(arg.date.getMonth() + 1).padStart(2, '0')
        const day = String(arg.date.getDate()).padStart(2, '0')
        const dateStr = `${year}-${month}-${day}`
        const dayOfWeek = arg.date.getDay()

        // ตรวจสอบวันหยุดประจำปี
        if (this.holidayDates.includes(dateStr)) {
          const holidayText = this.getHolidayName(dateStr)
          const textEl = document.createElement('div')
          textEl.innerHTML = holidayText
          textEl.style.fontSize = '12px'
          textEl.style.color = '#EA2F14'
          textEl.style.fontWeight = 'bold'
          textEl.style.textAlign = 'center'

          arg.el.style.backgroundColor = '#ff413328'
          arg.el.style.color = '#EA2F14'
          // arg.el.style.border = '1px solid #EA2F14'

          const dayNumber = arg.el.querySelector('.fc-daygrid-day-number') as any
          if (dayNumber) {
            dayNumber.style.color = '#EA2F14'
            dayNumber.style.fontWeight = 'bold'
            dayNumber.parentNode.insertBefore(textEl, dayNumber.nextSibling)
          }
        }
      },
      eventContent: function (arg: any) {
        const event = arg.event
        const start = event.start
        const end = event.end

        if (event.allDay) {
          return {
            html: `<div>${event.title}</div>`,
          }
        }

        const formatTime = (date: any) => {
          if (!date) return ''
          const hours = date.getHours().toString().padStart(2, '0')
          const minutes = date.getMinutes().toString().padStart(2, '0')
          return `${hours}:${minutes} น.`
        }

        const startTime = formatTime(start)
        const endTime = formatTime(end)
        const timeRange = startTime && endTime ? `${startTime} - ${endTime}` : startTime

        return {
          html: `<div>${timeRange} ${event.title}</div>`,
        }
      },
      eventDidMount(info: any) {
        tippy(info.el, {
          content: info.event.extendedProps.description || info.event.title,
          placement: 'top',
          theme: 'light-border', // เปลี่ยนจาก 'white' เป็น 'light-border'
        })
      },
      eventClick: this.handleEventClick, // เมื่อคลิก event
    }

    // โหลดฟิลเตอร์ที่บันทึกไว้ก่อนโหลดข้อมูลพื้นฐาน
    // this.loadSavedFilters()
    this.request.eventTypeId = []
    this.request.years = '2025'
    if (this.auth.userId) {
      this.request.employeeId = [] as string[]
      try {
        const result = await client.getEmployeeQueryByUserID(this.auth.userId)
        // Store defaults (use once for clear/reset fallback)
        this.defaultDepartmentId = result.departmentId || ''
        this.defaultEmployeeId = result.id || ''

        // Apply defaults to current filters (selected arrays used by multi-select)
        if (result.departmentId) {
          this.selectedDepartmentIds = [result.departmentId]
          this.request.departmentId = result.departmentId
        }
        if (result.id) {
          this.selectedEmployeeIds = [result.id]
          this.request.employeeId?.push(result.id as any)
        }
      } catch (error) {
        console.warn('No employee record found for current user. Calendar will show all data based on role permissions.')
        // If no employee record, don't set any default filters
        // Role-based filtering in backend will handle access control
      }
    }
    // โหลดข้อมูลพื้นฐาน
    await this.getDepartment()
    await this.getEventType()
    await this.getEmployee()

    // ถ้ามี departmentId ที่บันทึกไว้ ให้โหลด employee list ของแผนกนั้น (แต่ไม่รีเซ็ต employeeId)
    if (this.request.departmentId) {
      await this.loadEmployeeListForSavedDepartment()
    }

    await this.initialize()

    // บังคับตรวจสอบสถานะ drawer หลังจากทุกอย่างโหลดเสร็จ
    await this.$nextTick(() => {
      this.DialogCreate = false
      console.log('🔧 Final check: DialogCreate set to false after all mounted operations')
    })

    // await this.getActivebyid() // Removed because it requires an argument
  },
  computed: {
    departmentItems(): any[] {
      return [{ id: 'ALL', name: 'ทั้งหมด' }, ...(this.DepartmentList || [])]
    },
    // ตรวจสอบว่าพิกัดถูกต้องหรือไม่
    isValidCoordinates(): boolean {
      if (!this.selectedEvent?.coordinates) {
        // console.log('No coordinates found in selectedEvent')
        return false
      }

      // console.log('Raw coordinates:', this.selectedEvent.coordinates)
      const coords = this.parseCoordinates(this.selectedEvent.coordinates)
      // console.log('Parsed coordinates:', coords)

      return coords !== null
    },

    // สร้าง URL สำหรับ Google Maps
    googleMapsUrl(): string {
      if (!this.isValidCoordinates) return ''

      // เพิ่มการตรวจสอบเพื่อแก้ TypeScript error
      const coordinatesString = this.selectedEvent?.coordinates
      if (!coordinatesString) return ''

      const coords = this.parseCoordinates(coordinatesString)
      if (!coords) return ''

      // ใช้ Google Maps โดยไม่ต้อง API Key (สำหรับการทดสอบ)
      return `https://maps.google.com/maps?q=${coords.lat},${coords.lng}&hl=th&z=15&output=embed`

      // หรือใช้ Google Maps Embed API (ต้อง API Key)
      // return `https://www.google.com/maps/embed/v1/place?key=YOUR_API_KEY&q=${coords.lat},${coords.lng}&zoom=15`
    },

    // ข้อความที่แสดงเมื่อไม่มีพิกัดหรือพิกัดไม่ถูกต้อง
    coordinatesMessage(): string {
      const coords = this.selectedEvent?.coordinates

      if (!coords) {
        return 'กรุณาระบุพิกัดในรูปแบบตัวเลขเพื่อแสดงแผนที่'
      } else if (coords.match(/[ก-ฮ]/)) {
        return 'กรุณาระบุพิกัดเป็นตัวเลข\nตัวอย่าง: 13.7563,100.5018'
      } else {
        return 'รูปแบบพิกัดไม่ถูกต้อง\nตัวอย่าง: 13.7563,100.5018 (ละเว้นช่องว่าง)'
      }
    },
  },
  watch: {
    'request.eventTypeId'(newId) {
      // 1. บันทึกฟิลเตอร์ลง localStorage (เหมือนเดิม)
      // this.saveFilters()

      // 2. อัปเดต URL
      const query = { ...this.$route.query }
      if (!newId) {
        delete query.type
      } else {
        const selectedType = this.EventTypeList.find((type: any) => type.id === newId)
        if (selectedType) {
          query.type = selectedType.name
        }
      }
      if (JSON.stringify(query) !== JSON.stringify(this.$route.query)) {
        this.$router.push({ query })
      }
    },

    // เฝ้าดูการเปลี่ยนแปลงของฟิลเตอร์และบันทึกอัตโนมัติ
    'request.years'() {
      // this.saveFilters()
    },
    'request.departmentId'() {
      // this.saveFilters()
    },
    'request.employeeId': {
      handler() {
        // this.saveFilters()
      },
      deep: true,
    },
    // ไม่จำเป็นต้องมี 'request.eventTypeId' watcher แยกต่างหากแล้ว
    // DialogCreate(newValue) {
    //   console.log('🔍 DialogCreate changed to:', newValue)
    //   if (newValue) {
    //     console.log('⚠️ DialogCreate opened unexpectedly')
    //   }
    // },
  },
  methods: {
    async onDepartmentMultiChange(newValue?: string[]) {
      let value = Array.isArray(newValue) ? [...newValue] : [...this.selectedDepartmentIds]

      if (value.length === 0) {
      }
      if (value.includes('ALL')) {
        if (value.length === 1) {
          this.lastSelectionIsAllDept = true
          value = ['ALL']
        } else {
          if (this.lastSelectionIsAllDept) {
            value = value.filter(v => v !== 'ALL')
            this.lastSelectionIsAllDept = false
          } else {
            value = ['ALL']
            this.lastSelectionIsAllDept = true
          }
        }
      } else {
        this.lastSelectionIsAllDept = false
      }

      this.selectedDepartmentIds = value

      if (value.length === 1 && value[0] !== 'ALL') {
        this.request.departmentId = value[0] as any
      } else {
        this.request.departmentId = undefined as any
      }

      await this.getEmployeeByMultiDepartment()

      await this.initialize()
    },
    async onEmployeeChange() {
      const ALL_EMP = 'ALL_EMP'
      let value = [...this.selectedEmployeeIds]

      if (value.includes(ALL_EMP)) {
        if (value.length === 1) {
          this.lastSelectionIsAllEmp = true
          this.employeeAllSelected = true
          value = ['ALL_EMP']
          this.selectedEmployeeIds = value

          this.request.employeeId = this.getAllEmployeeIdsInSelectedDepartments() as any
        } else {
          if (this.lastSelectionIsAllEmp) {
            value = value.filter(v => v !== ALL_EMP)
            this.selectedEmployeeIds = value
            this.lastSelectionIsAllEmp = false
            this.employeeAllSelected = false
            this.request.employeeId = value.length > 0 ? [...value] : undefined
          } else {
            value = ['ALL_EMP']
            this.selectedEmployeeIds = value
            this.lastSelectionIsAllEmp = true
            this.employeeAllSelected = true

            this.request.employeeId = this.getAllEmployeeIdsInSelectedDepartments() as any
          }
        }
      } else {
        this.lastSelectionIsAllEmp = false
        this.employeeAllSelected = false
        this.selectedEmployeeIds = value
        if (value.length > 0) {
          this.request.employeeId = [...value] as any
        } else {

          this.request.employeeId = undefined as any
        }
      }
      await this.initialize()
    },
    async getEmployeeByMultiDepartment() {
      try {
        const prevHadAll = this.selectedEmployeeIds.includes('ALL_EMP')
        this.selectedEmployeeIds = []

        // Multi dept aggregation
        if (this.selectedDepartmentIds.length > 1 && !this.selectedDepartmentIds.includes('ALL')) {
          const unique: Record<string, any> = {}
          for (const deptId of this.selectedDepartmentIds) {
            if (deptId === 'ALL') continue
            const req = new GetEmployeeByDepartmentIdQuery()
            req.departmentId = deptId as any
            try {
              const list = await client.getEmployeeQueryByDepartmentId(req)
              list.forEach((e: any) => (unique[e.id] = { ...e, name: e.firstName + ' ' + e.lastName }))
            } catch (e) {
              console.warn('dept load fail', deptId, e)
            }
          }
          // กรองชื่อตัวเองออกถ้าเลือกแผนกอื่นที่ไม่ใช่แผนกตัวเอง
          const filteredEmployees = this.filterCurrentUserFromOtherDepartments(Object.values(unique))
          this.EmployeeList = this.injectAllEmployeeOption(filteredEmployees)
          return
        }
        // ALL sentinel across all departments
        if (this.selectedDepartmentIds.length === 1 && this.selectedDepartmentIds[0] === 'ALL') {
          const uniqueAll: Record<string, any> = {}
          for (const dept of this.DepartmentList) {
            if (!dept.id) continue
            const req = new GetEmployeeByDepartmentIdQuery()
            req.departmentId = dept.id
            try {
              const list = await client.getEmployeeQueryByDepartmentId(req)
              list.forEach((e: any) => (uniqueAll[e.id] = { ...e, name: e.firstName + ' ' + e.lastName }))
            } catch (e) {
              console.warn('dept all load fail', dept.id, e)
            }
          }
          this.EmployeeList = this.injectAllEmployeeOption(Object.values(uniqueAll))
          return
        }
        // Single dept
        if (this.request.departmentId) {
          const req = new GetEmployeeByDepartmentIdQuery()
          req.departmentId = this.request.departmentId
          const list = await client.getEmployeeQueryByDepartmentId(req)
          list.forEach((e: any) => (e.name = e.firstName + ' ' + e.lastName))

          // กรองชื่อตัวเองออกถ้าเลือกแผนกอื่นที่ไม่ใช่แผนกตัวเอง
          const filteredList = this.filterCurrentUserFromOtherDepartments(list)
          this.EmployeeList = this.injectAllEmployeeOption(filteredList)
        } else {
          this.EmployeeList = this.injectAllEmployeeOption([])
        }

        if (prevHadAll) {
          this.selectedEmployeeIds = ['ALL_EMP']
          this.employeeAllSelected = true
        }
      } catch (err) {
        console.error('getEmployeeByMultiDepartment error', err)
      }
    },
    getAllEmployeeIdsInSelectedDepartments(): string[] {

      return this.EmployeeList.filter((emp: any) => emp.id !== 'ALL_EMP').map((emp: any) => emp.id)
    },
    filterCurrentUserFromOtherDepartments(list: any[]): any[] {
      // ถ้าเลือกแผนกตัวเอง หรือเลือก "ทั้งหมด" ให้แสดงชื่อตัวเองได้
      if (
        this.selectedDepartmentIds.includes('ALL') ||
        this.selectedDepartmentIds.includes(this.defaultDepartmentId || '')
      ) {
        return list
      }

      // ถ้าเลือกแผนกอื่นที่ไม่ใช่แผนกตัวเอง ให้เอาชื่อตัวเองออก
      return list.filter((emp: any) => emp.id !== this.defaultEmployeeId)
    },
    injectAllEmployeeOption(list: any[]): any[] {
      const ALL_EMP = 'ALL_EMP'
      if (!list.some(l => l.id === ALL_EMP)) {
        return [{ id: ALL_EMP, name: 'ทั้งหมด' }, ...list]
      }
      return list
    },
    openCreateActivityDetail() {
      this.showCreateActivityDetail = true
      this.showCreateCustomerDailySchedule = false
      this.showUpdateCustomerDailySchedule = false
    },
    openCreateCustomerDailySchedule() {
      this.showCreateActivityDetail = false
      this.showCreateCustomerDailySchedule = true
      this.showUpdateCustomerDailySchedule = false
    },
    async openUpdateCustomerDailySchedule() {
      // ดึง activity ล่าสุดของ user
      try {
        if (this.auth && this.auth.userId) {
          const result = await client.getEmployeeQueryByUserID(this.auth.userId)
          if (!result || !result.id) {
            if (this.sweetAlertStore && this.sweetAlertStore.error) {
              this.sweetAlertStore.error('ไม่พบข้อมูลพนักงาน')
            }
            return
          }
          const commandCreate = new GetActivityPlanLatestQuery()
          commandCreate.employeeId = result.id
          const response = await client.getActivityPlanQueryLatest(commandCreate)
          if (response) {
            this.mode = 'duplicate'
            this.selectedDailyScheduleId = response
            this.showUpdateCustomerDailySchedule = true
            this.showCreateActivityDetail = false
            this.showCreateCustomerDailySchedule = false
          } else {
            if (this.sweetAlertStore && this.sweetAlertStore.warning) {
              this.sweetAlertStore.warning('ไม่พบตารางงานล่าสุด กรุณาสร้างใหม่')
            }
            this.showUpdateCustomerDailySchedule = false
            this.showCreateActivityDetail = false
            this.showCreateCustomerDailySchedule = true
          }
        }
      } catch (error) {
        if (this.sweetAlertStore && this.sweetAlertStore.error) {
          this.sweetAlertStore.error('เกิดข้อผิดพลาดในการโหลดข้อมูล')
        }
        this.showUpdateCustomerDailySchedule = false
        this.showCreateActivityDetail = false
        this.showCreateCustomerDailySchedule = false
      }
    },
    async CloseDialogCreate(value: boolean, reload?: any) {
      this.DialogCreate = value
      if (!value) {
        this.showCreateCustomerDailySchedule = false
      }
      if (reload === true) {
        this.initialize()
      }
    },
    // บันทึกฟิลเตอร์ที่เลือกไว้ใน localStorage เท่านั้น
    // saveFilters() {
    //   const filters = {
    //     years: this.request.years,
    //     departmentId: this.request.departmentId,
    //     employeeId: this.request.employeeId,
    //     eventTypeId: this.request.eventTypeId,
    //   }
    //
    //   console.log('💾 Saving filters to localStorage:', filters)
    //   // บันทึกใน localStorage เท่านั้น (ไม่ใช้ URL query parameters)
    //   localStorage.setItem('calendar-filters', JSON.stringify(filters))
    // },

    // โหลดฟิลเตอร์ที่บันทึกไว้จาก localStorage
    loadSavedFilters() {
      const savedFilters = localStorage.getItem('calendar-filters')

      if (savedFilters) {
        try {
          const filters = JSON.parse(savedFilters)
          console.log('🔄 Loading saved filters from localStorage:', filters)

          this.request.years = filters.years || '2025'
          this.request.departmentId = filters.departmentId
          this.request.employeeId = filters.employeeId
          this.request.eventTypeId = filters.eventTypeId

          console.log('✅ Filters loaded successfully:', this.request)
        } catch (error) {
          console.error('❌ Error loading saved filters:', error)
          this.setDefaultFilters()
        }
      } else {
        console.log('🆕 No saved filters found, setting defaults')
        this.setDefaultFilters()
      }
    },

    // ตั้งค่าฟิลเตอร์เริ่มต้น
    setDefaultFilters() {
      // Keep year default
      this.request.years = '2025'

      // Restore department & employee to user defaults (match list view behavior)
      if (this.defaultDepartmentId) {
        this.request.departmentId = this.defaultDepartmentId
        this.selectedDepartmentIds = [this.defaultDepartmentId]
      } else {
        this.request.departmentId = undefined
        this.selectedDepartmentIds = []
      }

      if (this.defaultEmployeeId) {
        this.request.employeeId = [this.defaultEmployeeId]
        this.selectedEmployeeIds = [this.defaultEmployeeId]
      } else {
        this.request.employeeId = []
        this.selectedEmployeeIds = []
      }

      // Ensure event types default to all (consistent with list view auto-select)
      this.request.eventTypeId = []
      if (Array.isArray(this.EventTypeList) && this.EventTypeList.length > 0) {
        this.EventTypeList.forEach((et: any) => this.request.eventTypeId?.push(et.id))
      }

      // // 1. ค้นหา ID ของ "ตารางงานประจำวัน" จาก EventTypeList
      // const dailyScheduleType = this.EventTypeList.find((type: any) => type.eventTypeCode == '002')

      // // 2. ถ้าเจอ ให้ตั้งค่า ID นั้นเป็นค่าเริ่มต้น, ถ้าไม่เจอให้เป็น undefined (ป้องกัน error)
      // this.request.eventTypeId = dailyScheduleType ? dailyScheduleType.id : undefined
    },

    // รีเซ็ตฟิลเตอร์ทั้งหมด
    async resetFilters() {
      this.isLoading = true

      try {
        console.log('🔄 Resetting all filters')

        // รีเซ็ตค่าฟิลเตอร์
        this.setDefaultFilters()

        // ล้างค่าใน localStorage
        localStorage.removeItem('calendar-filters')

        // โหลดข้อมูลใหม่
        await this.getEmployee()
        await this.initialize()

        console.log('✅ Filters reset successfully')
      } catch (error) {
        console.error('❌ Error resetting filters:', error)
      } finally {
        this.isLoading = false
      }
    },

    OpenDialogCreate() {
      console.log('🎯 OpenDialogCreate called - User clicked button')
      this.DialogCreate = true
    },
    CloseDialogCreated(value: boolean, reload?: boolean) {
      console.log('🔒 CloseDialogCreated called with value:', value, reload)
      this.DialogCreate = value
      if (!value) {
        this.showCreateActivityDetail = false
      }
      if (reload === true) {
        this.initialize()
        this.getDepartment()
        this.getEmployee()
      }
    },
    async initialize() {
      // บันทึกฟิลเตอร์ที่เลือกไว้
      // this.saveFilters()
      // แสดงสถานะการโหลด
      this.isLoading = true

      try {
        // if (!this.auth.roles.includes('Admin') && !this.auth.roles.includes('Manager')) {

        // }
        console.log('Request:', this.request)
        const response = await client.getActivityPlanQueryByEmployeeId(this.request)
        this.activity = response
        console.log('Activity data:', this.activity)
      } catch (error) {
        console.error(error)
        this.activity = [] // ตั้งค่าเป็น array ว่างเมื่อมี error
      } finally {
        // ปิดสถานะการโหลดเมื่อเสร็จแล้ว ไม่ว่าจะสำเร็จหรือมี error
        this.isLoading = false
      }

      const activityEvents = this.activity.map((x: any) => {
        let title = x.objective ?? x.detail ?? 'ไม่มีหัวเรื่อง'
        let colors = this.getEmployeeColor(x.eventTypeId)

        if (this.request.employeeId === undefined || this.request.employeeId.length !== 1) {
          colors = this.getEmployeeColor(x.employeeId) as any

          const titlename = x.employees?.titleName ?? ''
          const firstName = x.employees?.firstName ?? ''
          const lastName = x.employees?.lastName ?? ''
          title = titlename + ' ' + firstName + ' ' + lastName
        }

        // 🔧 ตัดความยาว title ไม่เกิน 15 ตัวอักษร
        if (title.length > 15) {
          title = title.substring(0, 15) + '...'
        }

        let objective = x.objective
        if (objective === 'อื่น ๆ') {
          objective = x.objectiveDetail
        }
        if (this.request.employeeId) {
          objective = x.detail ?? ''
        }
        if (objective === null) {
          objective = x.detail ?? ''
        }

        // แก้ไขส่วนการจัดการวันที่และเวลา
        let startDate, endDate

        if (x.allDay) {
          startDate = moment(x.startDate).format('YYYY-MM-DD')
          // ✅ สำหรับ allDay events ให้บวก 1 วันใน endDate
          endDate = moment(x.endDate).add(1, 'day').format('YYYY-MM-DD')
        } else {
          startDate = x.startDate
          endDate = x.endDate

          if (moment(startDate).isSame(moment(endDate))) {
            endDate = moment(endDate).add(1, 'hour').toISOString()
          }

          if (
            moment(startDate).format('YYYY-MM-DD') === moment(endDate).format('YYYY-MM-DD') &&
            moment(startDate).format('HH:mm') === moment(endDate).format('HH:mm')
          ) {
            endDate = moment(startDate).add(30, 'minutes').toISOString()
          }
        }

        return {
          id: x.id,
          title: title,
          start: startDate,
          end: endDate,
          allDay: x.allDay,
          backgroundColor: colors.bg,
          borderColor: colors.border,
          textColor: '#fff',
          extendedProps: {
            description: objective ?? 'ไม่มีรายละเอียด',
            eventType: x.eventTypes, // เพิ่ม eventType เข้าไปใน extendedProps
          },
        }
      })

      console.log('Processed events:', activityEvents)

      this.calendar = new Calendar(this.$refs.calendar as any, {
        ...this.calendarObject,
        events: [...activityEvents],
        eventDisplay: 'block',
        displayEventTime: true,
        displayEventEnd: true,
        eventMinHeight: 15,
        eventShortHeight: 20,
      })

      this.calendar.render()
      this.calendar.gotoDate(this.dateTime)
    },
    async getActivebyid(id: string, eventTypeFromExtendedProps: any) {
      if (!id) {
        console.error('❌ ID is undefined!')
        return
      }

      const activity = await client.getActivityPlanQueryByID(id)
      const result = await client.getPlanNoteQueryByActivityPlanId(id)
      console.log('📦 Full Activity Data:', activity) // Log full object

      this.selectedEvent = {
        id: activity.id,
        title: activity.objectiveDetail || activity.objective || '-', // หัวเรื่อง
        date: this.formatDateforshow(activity.startDate, activity.endDate, activity.allDay),
        location: activity.location || '-',
        coordinates: activity.organizations?.coordinates || activity.location || '',
        organizations: activity.organizations || { name: '-' },
        detail: activity.detail || '-',
        objective: activity.objective || '-',
        objectiveDetail: activity.objectiveDetail || '-',
        startDate: activity.startDate,
        endDate: activity.endDate,
        cost: activity.cost,
        eventType: eventTypeFromExtendedProps || activity.eventTypes, // ใช้ eventType ที่ส่งมา หรือจาก activity ถ้ามี
        // planNote: activity.planNote ?? null, // ถ้ามีการโหลดไว้ก่อน
        planNote: result && result.id !== '00000000-0000-0000-0000-000000000000' ? result : null, // ใช้ข้อมูลจาก query planNote
        hasPlanNote: result && result.id !== '00000000-0000-0000-0000-000000000000' ? true : false, // เช็คว่ามี planNote หรือไม่
        employees: activity.employees
          ? {
              titleName: activity.employees.titleName || '',
              firstName: activity.employees.firstName || '',
              lastName: activity.employees.lastName || '',
              employeeFullName: `${activity.employees.titleName || ''}${activity.employees.firstName || ''} ${
                activity.employees.lastName || ''
              }`,
            }
          : {
              titleName: '',
              firstName: '',
              lastName: '',
              employeeFullName: '-',
            },
      }

      this.dialog = true
    },
    async getDepartment() {
      try {
        this.DepartmentList = await client.getDepartmentQuery()
        // If defaults exist and user hasn't chosen yet, ensure selection reflects default
        if (this.defaultDepartmentId && this.selectedDepartmentIds.length === 0) {
          this.selectedDepartmentIds = [this.defaultDepartmentId]
          this.request.departmentId = this.defaultDepartmentId
        }
      } catch (error) {
        console.error(error)
      }
    },
    goCreateActivity() {
      this.DialogCreate = true
    },

    async DuplicateActivity() {
      try {
        if (this.auth && this.auth.userId) {
          // ดึงข้อมูล employee ตาม userId
          const result = await client.getEmployeeQueryByUserID(this.auth.userId)
          if (!result || !result.id) {
            if (this.sweetAlertStore && this.sweetAlertStore.error) {
              this.sweetAlertStore.error('ไม่พบข้อมูลพนักงาน')
            }
            return
          }
          // สร้าง command สำหรับดึง activity ล่าสุด
          const commandCreate = new GetActivityPlanLatestQuery()
          commandCreate.employeeId = result.id
          const response = await client.getActivityPlanQueryLatest(commandCreate)
          if (response) {
            this.mode = 'duplicate'
            setTimeout(() => {
              this.id = response
              this.DialogUpdate = true
            }, 1000)
          } else {
            if (this.sweetAlertStore && this.sweetAlertStore.warning) {
              this.sweetAlertStore.warning('ไม่พบตารางงานล่าสุด กรุณาสร้างใหม่')
            }
            this.mode = 'duplicate'
            this.DialogCreate = true
          }
        }
      } catch (error) {
        if (this.sweetAlertStore && this.sweetAlertStore.error) {
          this.sweetAlertStore.error('เกิดข้อผิดพลาดในการโคลนข้อมูล')
        }
        console.error('❌ Error in DuplicateActivity:', error)
      }
    },
    async getEmployee() {
      try {
        const list = await client.getEmployeeQuery()
        list.forEach((x: any) => {
          x.name = x.firstName + ' ' + x.lastName
        })
        this.EmployeeList = this.injectAllEmployeeOption(list)
        // Apply default employee if not set
        if (this.defaultEmployeeId && this.selectedEmployeeIds.length === 0) {
          this.selectedEmployeeIds = [this.defaultEmployeeId]
          this.request.employeeId = [this.defaultEmployeeId]
        }
      } catch (error) {
        console.error(error)
      }
    },
    async getEventType() {
      try {
        const result = await client.getEventTypeQuery()

        result.sort((a, b) => {
          if (a.eventTypeCode === '002') return -1
          if (b.eventTypeCode === '002') return 1
          return 0
        })

        this.EventTypeList = result

        // ถ้ายังไม่ได้เลือกประเภท (eventTypeId) ให้ตั้งค่าเริ่มต้นเป็น "แจ้งงานรายวัน"
        if (!this.request.eventTypeId || this.request.eventTypeId.length === 0) {
          // const dailyScheduleType = this.EventTypeList.find((type: any) => type.eventTypeCode == '002')
          // if (dailyScheduleType) {
          // this.request.eventTypeId =  [dailyScheduleType.id]}
          this.request.eventTypeId = []
          this.EventTypeList.forEach((item: any) => {
            this.request.eventTypeId?.push(item.id)
          })
        }
      } catch (error) {
        console.error(error)
      }
    },
    async getEmployeeByDepartment() {
      try {
        this.request.employeeId = undefined
        this.requestEmployee.departmentId = this.request.departmentId
        this.EmployeeList = await client.getEmployeeQueryByDepartmentId(this.requestEmployee)
        this.EmployeeList.forEach((x: any) => {
          x.name = x.firstName + ' ' + x.lastName
        })
      } catch (error) {
        console.error(error)
      }
    },
    // โหลดรายชื่อพนักงานตามแผนกที่บันทึกไว้ โดยไม่รีเซ็ต employeeId
    async loadEmployeeListForSavedDepartment() {
      try {
        // เก็บ employeeId ที่บันทึกไว้
        const savedEmployeeId = this.request.employeeId

        this.requestEmployee.departmentId = this.request.departmentId
        this.EmployeeList = await client.getEmployeeQueryByDepartmentId(this.requestEmployee)
        this.EmployeeList.forEach((x: any) => {
          x.name = x.firstName + ' ' + x.lastName
        })

        // เก็บ employeeId ที่บันทึกไว้กลับคืน
        this.request.employeeId = savedEmployeeId

        console.log('✅ Employee list loaded for saved department, employeeId preserved:', savedEmployeeId)
      } catch (error) {
        console.error('❌ Error loading employee list for saved department:', error)
      }
    },
    goTocreateActivityPlan() {
      this.$router.push('/CreateCustomerAppointmentPlanDetailview')
    },
    async handleEventClick(info: any) {
      this.selectedEvent = info.event
      this.selectedEvent.date = info.event.start.toLocaleDateString()

      const result = await client.getPlanNoteQueryByActivityPlanId(this.selectedEvent.id)
      if (result.id !== '00000000-0000-0000-0000-000000000000') {
        this.selectedEvent.planNote = result
      }

      // ✅ ดึง publicId จาก _def
      const publicId = info?.event?._def?.publicId

      console.log('✅ publicId:', publicId)

      if (publicId) {
        await this.getActivebyid(publicId, info.event.extendedProps.eventType)
      } else {
        console.warn('⚠️ ไม่พบ publicId จาก event:', info.event)
      }

      console.log('DEBUG: selectedEvent after getActivebyid:', this.selectedEvent)
      console.log('DEBUG: selectedEvent.eventType:', this.selectedEvent.eventType)
      console.log('DEBUG: selectedEvent.eventType.eventTypeCode:', this.selectedEvent.eventType?.eventTypeCode)

      // ถ้าเป็นประเภท "แจ้งงานรายวัน" ให้เปิด dialog แก้ไขแจ้งงานรายวัน
      const isDailySchedule = this.isSelectedEventTypeDailySchedule()
      console.log('DEBUG: isSelectedEventTypeDailySchedule() result:', isDailySchedule)

      if (isDailySchedule) {
        this.openUpdateDailyScheduleDialog()
        return
      }

      this.dialog = true
    },
    goToEditActivityPlan() {
      this.$router.push({ name: 'UpdateCustomerAppointmentPlanDetailview', params: { id: this.selectedEvent.id } })
    },
    goSettingActivityPlan(mode: any) {
      if (mode === 'activity') {
        this.activitytab = 'EditActivity'
      } else if (mode === 'report') {
        if (this.selectedEvent.hasPlanNote) {
          this.activitytab = 'EditReport'
        } else {
          this.activitytab = 'CrateReport'
        }
      }
      this.selectedActivityId = this.selectedEvent.id
      this.manageActivityDialog = true
      this.dialog = false // ปิด dialog รายละเอียดก่อน
    },
    closeManageActivityDialog() {
      this.manageActivityDialog = false
      this.selectedActivityId = null
    },
    openUpdateDailyScheduleDialog() {
      this.selectedDailyScheduleId = this.selectedEvent.id
      this.mode = 'edit' // กำหนดโหมดเป็น edit สำหรับการแก้ไข

      this.showUpdateCustomerDailySchedule = true
      this.dialog = false // ปิด dialog รายละเอียดก่อน
    },
    closeUpdateDailyScheduleDialog(value: any, reload?: any) {
      this.updateDailyScheduleDialog = false
      this.selectedDailyScheduleId = null
      this.showUpdateCustomerDailySchedule = false
      if (reload) {
        this.initialize()
      }
    },
    goToCreatePlan() {
      this.$router.push({ name: 'AppointmentOutcomeDetailview', params: { id: this.selectedEvent.id } })
    },
    async onYearChange(year: any) {
      if (!year || !this.calendar) return
      const currentDate = this.calendar.getDate()
      const newDate = new Date(currentDate)
      newDate.setFullYear(parseInt(year))
      this.dateTime = newDate as any
    },
    getHolidayName(dateStr: any) {
      const holidayNames = {
        '2025-01-01': 'วันปีใหม่',
        '2025-02-12': 'วันมาฆบูชา',
        '2025-04-06': 'วันจักรี',
        '2025-04-13': 'วันสงกรานต์',
        '2025-04-14': 'วันสงกรานต์',
        '2025-04-15': 'วันสงกรานต์',
        '2025-05-01': 'วันแรงงาน',
        '2025-05-05': 'วันฉัตรมงคล',
        '2025-05-12': 'วันวิสาขบูชา',
        '2025-06-03': 'วันเฉลิมฯ',
        '2025-07-10': 'วันอาสาฬหบูชา',
        '2025-07-28': 'วันเฉลิมฯ',
        '2025-08-12': 'วันแม่',
        '2025-10-13': 'วันคล้ายฯ',
        '2025-10-23': 'วันปิยมหาราช',
        '2025-12-05': 'วันพ่อ',
        '2025-12-10': 'วันรัฐธรรมนูญ',
        '2025-12-31': 'วันสิ้นปี',
      } as any
      return holidayNames[dateStr] || ''
    },
    // 🔄 ฟังก์ชันวนลูปสีแบบเรียงลำดับ
    generateColorFromIndex(index: number) {
      const safeIndex = index % this.colorPalette.length
      return this.colorPalette[safeIndex]
    },
    // ✅ ได้สีแบบวนตามลำดับจาก employeeId (เป็นตัวเลขล้วนจะดีที่สุด)
    getEmployeeColor(employeeId: any) {
      if (!employeeId) return { bg: '#758694', border: '#758694' }

      // ถ้ามีอยู่แล้ว ให้ใช้สีเดิม
      if (this.employeeColors[employeeId]) {
        return this.employeeColors[employeeId]
      }

      // แจกสีตามลำดับ แล้ววนกลับเมื่อใช้หมด
      const color = this.colorPalette[this.nextColorIndex % this.colorPalette.length]

      // บันทึกให้จำสีของพนักงานนี้ไว้
      this.employeeColors[employeeId] = color

      // เตรียมลำดับสีถัดไปสำหรับคนถัดไป
      this.nextColorIndex++

      return color
    },
    // 🔧 ฟังก์ชันเสริม (กรณี employeeId เป็น string เช่น "emp001")
    stringToIndex(str: string): number {
      let total = 0
      for (let i = 0; i < str.length; i++) {
        total += str.charCodeAt(i)
      }
      return total
    },
    // แปลงพิกัดจาก string เป็น object
    parseCoordinates(coordinatesString: string): { lat: number; lng: number } | null {
      if (!coordinatesString) return null

      const cleanCoords = coordinatesString.trim()
      const patterns = [
        /^(-?\d+\.?\d*),\s*(-?\d+\.?\d*)$/, // lat,lng หรือ lat, lng
        /^(-?\d+\.?\d*)\s+(-?\d+\.?\d*)$/, // lat lng
      ]

      for (const pattern of patterns) {
        const match = cleanCoords.match(pattern)
        if (match) {
          const lat = parseFloat(match[1])
          const lng = parseFloat(match[2])

          // ตรวจสอบว่าเป็นพิกัดที่ถูกต้อง
          if (isFinite(lat) && isFinite(lng) && lat >= -90 && lat <= 90 && lng >= -180 && lng <= 180) {
            return { lat, lng }
          }
        }
      }

      return null
    },
    eventDidMount: function (info: any) {
      const start = info.event.start
      const end = info.event.end

      const options = {
        hour: '2-digit',
        minute: '2-digit',
        hour12: false,
      }

      const startStr = start.toLocaleTimeString('th-TH', options).replace(':', '.')
      const endStr = end.toLocaleTimeString('th-TH', options).replace(':', '.')

      const originalTitle = info.event.title

      // เฉพาะในมุมมอง month (dayGrid)
      if (info.view.type === 'dayGridMonth') {
        const timeText = `${startStr} น. - ${endStr} น.`
        info.el.querySelector('.fc-event-title').innerText = `${timeText} ${originalTitle}`
      }
      tippy(info.el, {
        content: info.event.title, // หรือใช้ info.event.extendedProps.xxx
        placement: 'top',
        theme: 'light',
      })
    },
    isSelectedEventTypeDailySchedule() {
      // ตรวจสอบจาก selectedEvent eventType
      if (this.selectedEvent.eventType && this.selectedEvent.eventType.eventTypeCode == '002') {
        return true
      }

      // ตรวจสอบจาก request.eventTypeId ที่เลือกไว้ (fallback)
      const selectedEventType = this.EventTypeList.find((type: any) => type.id === this.request.eventTypeId)
      if (selectedEventType && selectedEventType.eventTypeCode == '002') {
        return true
      }

      return false
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
  },
}
</script>

<style scoped>
.fc-event-title,
.fc-event {
  white-space: nowrap; /* ไม่ขึ้นบรรทัดใหม่ */
  overflow: hidden; /* ซ่อนข้อความที่ล้น */
  text-overflow: ellipsis; /* ใส่ ... */
  font-size: 13px;
}

::v-deep .fc-event-title,
::v-deep .fc-event {
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

/* Enhanced responsive base styles */
.cardCalendar {
  position: relative;
  border-radius: 16px;
}

.add-activity-btn {
  font-weight: 600;
  border-radius: 12px;
  text-transform: none;
}

.search-btn {
  font-weight: 600;
  border-radius: 8px;
  text-transform: none;
  min-width: 120px;
  box-shadow: 0 2px 8px rgba(25, 118, 210, 0.2);
  transition: all 0.2s ease;
  letter-spacing: 0.025em;
}

.search-btn:hover {
  box-shadow: 0 4px 12px rgba(25, 118, 210, 0.3);
  transform: translateY(-1px);
}

.reset-btn {
  font-weight: 600;
  border-radius: 8px;
  text-transform: none;
  min-width: 100px;
  transition: all 0.2s ease;
  letter-spacing: 0.025em;
}

.reset-btn:hover {
  background-color: #f5f5f5;
  transform: translateY(-1px);
}

/* Mobile responsive button adjustments */
@media (max-width: 599.98px) {
  .button-section {
    margin-top: 1rem !important;
  }

  .button-section .v-col {
    padding-top: 0.5rem !important;
    padding-bottom: 0.5rem !important;
  }

  .button-section .pr-1 {
    padding-right: 0.25rem !important;
  }

  .button-section .pl-1 {
    padding-left: 0.25rem !important;
  }

  .search-btn,
  .reset-btn {
    height: 44px;
    font-size: 14px;
    min-width: unset;
    border-radius: 8px;
    font-weight: 600;
  }

  .search-btn .v-icon,
  .reset-btn .v-icon {
    font-size: 16px;
  }
}

/* Tablet responsive button adjustments */
@media (min-width: 600px) and (max-width: 959.98px) {
  .button-section {
    justify-content: center;
    margin-top: 1.5rem !important;
  }

  .button-section .v-col {
    padding: 0 0.5rem !important;
  }

  .search-btn,
  .reset-btn {
    height: 46px;
    font-size: 15px;
    min-width: 140px;
  }
}

/* Desktop button layout */
@media (min-width: 960px) {
  .button-section {
    justify-content: flex-end;
    margin-top: 2rem !important;
  }

  .button-section .v-col {
    flex: 0 0 auto;
    max-width: none;
    padding: 0 0.25rem !important;
  }

  .search-btn,
  .reset-btn {
    min-width: 120px;
  }
}

::v-deep(.fc-popover) {
  background-color: #f3f4f6 !important;
  border-radius: 8px;
}
::v-deep(.fc-popover-body) {
  background-color: #f3f4f6 !important;
}
::v-deep(.fc-popover-header) {
  background-color: #e5e7eb !important;
  color: #1a237e;
}
/* Enhanced calendar button styles */
:deep(.fc .fc-button) {
  background-color: #fff;
  color: #2b3086;
  text-transform: none;
  border: 1.6px solid #2b3086;
  font-size: 14px;
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
  transition: all 0.2s ease;
  border-radius: 8px;
}

:deep(.fc .fc-button.fc-button-active) {
  background-color: #2b3086 !important;
  color: #fff !important;
  border: 1.5px solid #2b3086;
}

:deep(.fc .fc-button:hover) {
  background-color: #f5f5f5;
  transform: translateY(-1px);
}

:deep(.fc .fc-day-today),
:deep(.fc .fc-timegrid-col.fc-day-today) {
  background-color: #2b30862b !important;
}

:deep(.fc-event.event-training) {
  background-color: #4da8da;
  border: none;
  color: white;
  font-weight: bold;
  border-radius: 6px;
}

.calendarWidth {
  width: 100%;
  max-width: 100%;
  overflow-x: auto;
  border-radius: 12px;
}

/* Enhanced dialog responsive styles */
.premium-appointment-card {
  border-radius: 24px !important;
  overflow: hidden;
  max-height: 100vh;
  display: flex;
  flex-direction: column;
}

/* Enhanced hero header */
.hero-header {
  position: relative;
  padding: 1.5rem 2rem;
  background: linear-gradient(135deg, #e3f2fd 0%, #f3e5f5 100%);
  display: flex;
  align-items: center;
  justify-content: space-between;
  min-height: 120px;
  flex-shrink: 0;
}

.glass-overlay {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(255, 255, 255, 0.3);
  backdrop-filter: blur(10px);
}

.hero-content {
  display: flex;
  align-items: center;
  gap: 1.5rem;
  z-index: 2;
  position: relative;
  flex: 1;
  min-width: 0;
}

.hero-badge {
  width: 56px;
  height: 56px;
  background: rgba(255, 255, 255, 0.8);
  border-radius: 16px;
  display: flex;
  align-items: center;
  justify-content: center;
  backdrop-filter: blur(20px);
  border: 1px solid rgba(255, 255, 255, 0.5);
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.1);
  flex-shrink: 0;
}

.hero-icon {
  font-size: 28px;
  color: #1976d2;
}

.hero-text {
  color: #2c3e50;
  flex: 1;
  min-width: 0;
}

.hero-title {
  font-size: 1rem;
  font-weight: 500;
  opacity: 0.8;
  margin-bottom: 0.5rem;
  color: #546e7a;
}

.hero-subtitle {
  font-size: 1.6rem;
  font-weight: 700;
  margin-bottom: 0.75rem;
  line-height: 1.2;
  color: #1a237e;
  word-wrap: break-word;
}

.hero-date {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.95rem;
  color: #37474f;
  background: rgba(255, 255, 255, 0.8);
  padding: 0.5rem 1rem;
  border-radius: 20px;
  backdrop-filter: blur(10px);
  width: fit-content;
  border: 1px solid rgba(255, 255, 255, 0.8);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
}

.date-icon {
  font-size: 16px;
  color: #1976d2;
  flex-shrink: 0;
}

.date-text {
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.close-button {
  z-index: 3;
  position: relative;
  background: rgba(255, 255, 255, 0.8) !important;
  backdrop-filter: blur(10px);
  flex-shrink: 0;
  border-radius: 12px !important;
}

.close-icon {
  font-size: 20px;
  color: #37474f;
}

/* Enhanced content area */
.premium-content {
  background: white;
  flex: 1;
  overflow-y: auto;
  padding: 0 !important;
}

.content-container {
  padding: 1.5rem !important;
  min-height: 0;
}

.info-row {
  margin-bottom: 1.5rem;
}

.detail-row {
  margin-bottom: 0;
}

/* Enhanced info cards */
.modern-info-card {
  background: white;
  border-radius: 16px;
  padding: 1.25rem;
  height: 100%;
  border: 1px solid #f0f0f0;
  transition: all 0.3s ease;
  position: relative;
  overflow: hidden;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.08);
}

.modern-info-card::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 3px;
  background: linear-gradient(90deg, var(--accent-color), var(--accent-light));
}

.primary-card {
  --accent-color: #42a5f5;
  --accent-light: #90caf9;
}

.success-card {
  --accent-color: #66bb6a;
  --accent-light: #a5d6a7;
}

.warning-card {
  --accent-color: #ffb74d;
  --accent-light: #ffcc02;
}

.card-header {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.card-avatar {
  width: 44px;
  height: 44px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 3px 12px rgba(0, 0, 0, 0.15);
  flex-shrink: 0;
}

.primary-avatar {
  background: linear-gradient(135deg, #42a5f5, #90caf9);
}

.success-avatar {
  background: linear-gradient(135deg, #66bb6a, #a5d6a7);
}

.warning-avatar {
  background: linear-gradient(135deg, #ffb74d, #ffcc02);
}

.avatar-icon {
  font-size: 20px;
  color: white;
}

.card-header-text {
  flex: 1;
  min-width: 0;
}

.card-category {
  font-size: 0.8rem;
  color: #666;
  font-weight: 500;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  display: block;
}

.card-title {
  font-size: 1rem;
  font-weight: 600;
  color: #333;
  margin-top: 0.25rem;
  line-height: 1.3;
  word-wrap: break-word;
}

/* Enhanced detail panel */
.detail-panel {
  background: white;
  border-radius: 16px;
  border: 1px solid #f0f0f0;
  overflow: hidden;
  height: 100%;
  display: flex;
  flex-direction: column;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.08);
}

.panel-content {
  padding: 1.25rem;
  flex-grow: 1;
  overflow-y: auto;
  -webkit-overflow-scrolling: touch;
}

.objective-section {
  margin-bottom: 0;
}

.details-label {
  font-size: 1rem;
  font-weight: 600;
  color: #333;
  margin-bottom: 0.75rem;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.details-icon {
  font-size: 18px;
  color: #795548;
  flex-shrink: 0;
}

.details-text {
  color: #666;
  line-height: 1.6;
  margin-bottom: 0;
  word-wrap: break-word;
  font-size: 0.95rem;
}

/* Enhanced cost panel */
.cost-panel {
  background: linear-gradient(135deg, #e8f5e8 0%, #f1f8e9 100%);
  border-radius: 16px;
  padding: 1.25rem;
  text-align: center;
  border: 1px solid #e8f5e8;
  height: 100%;
  display: flex;
  flex-direction: column;
  justify-content: center;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.08);
}

.cost-header {
  margin-bottom: 1rem;
}

.cost-label {
  font-size: 0.8rem;
  font-weight: 600;
  color: #2e7d32;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.cost-amount {
  display: flex;
  align-items: baseline;
  justify-content: center;
  gap: 0.25rem;
  flex-wrap: wrap;
}

.currency {
  font-size: 1.25rem;
  font-weight: 600;
  color: #2e7d32;
}

.amount {
  font-size: 2rem;
  font-weight: 700;
  color: #1b5e20;
  word-break: break-all;
}

/* Enhanced action bar */
.premium-actions {
  background: #fafafa;
  border-top: 1px solid #e0e0e0;
  padding: 1.25rem 0;
  flex-shrink: 0;
}

.action-container {
  padding: 0 1.5rem !important;
}

.action-bar {
  display: flex;
  justify-content: center;
  gap: 1rem;
  flex-wrap: wrap;
}

.action-button {
  min-width: 180px;
  height: 48px;
  border-radius: 12px;
  font-weight: 600;
  text-transform: none;
  letter-spacing: 0.25px;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  justify-content: center;
}

.btn-icon {
  font-size: 18px;
  flex-shrink: 0;
}

.btn-text {
  flex: 1;
  text-align: center;
}

.primary-button {
  box-shadow: 0 4px 16px rgba(25, 118, 210, 0.3);
}

/* Mobile responsive styles (XS) */
@media (max-width: 599.98px) {
  .cardCalendar {
    border-radius: 12px;
  }

  .premium-appointment-card {
    border-radius: 0 !important;
    height: 100vh;
    max-height: 100vh;
  }

  .premium-actions {
    background: #fafafa;
    border-top: 1px solid #e0e0e0;
    padding: 1.25rem 0;
    flex-shrink: 0;
    margin-bottom: 25% !important; /* ปรับให้ไม่ทับกับปุ่ม action */
  }

  /* Mobile hero header */
  .hero-header {
    padding: 1rem;
    min-height: 90px;
    flex-direction: row;
    flex-wrap: nowrap;
  }

  .hero-content {
    gap: 0.75rem;
    flex-direction: row;
  }

  .hero-badge {
    width: 40px;
    height: 40px;
    border-radius: 12px;
  }

  .hero-icon {
    font-size: 20px;
  }

  .hero-title {
    font-size: 0.8rem;
    margin-bottom: 0.25rem;
  }

  .hero-subtitle {
    font-size: 1.1rem;
    margin-bottom: 0.5rem;
    line-height: 1.1;
  }

  .hero-date {
    font-size: 0.8rem;
    padding: 0.375rem 0.75rem;
    border-radius: 16px;
    flex-wrap: wrap;
    width: 100%;
    max-width: 100%;
    justify-content: flex-start;
  }

  .date-icon {
    font-size: 14px;
  }

  .date-text {
    white-space: normal;
    overflow: visible;
    text-overflow: clip;
    word-wrap: break-word;
  }

  .close-button {
    width: 36px !important;
    height: 36px !important;
    min-width: 36px !important;
  }

  .close-icon {
    font-size: 16px;
  }

  /* Mobile content */
  .content-container {
    padding: 1rem !important;
  }

  .info-row {
    margin-bottom: 0.75rem;
  }

  /* Mobile info cards */
  .modern-info-card {
    padding: 1rem;
    border-radius: 12px;
    margin-bottom: 0.75rem;
  }

  .card-avatar {
    width: 36px;
    height: 36px;
    border-radius: 10px;
  }

  .avatar-icon {
    font-size: 18px;
  }

  .card-category {
    font-size: 0.7rem;
  }

  .card-title {
    font-size: 0.9rem;
  }

  /* Mobile detail panel */
  .detail-panel {
    border-radius: 12px;
    margin-bottom: 0.75rem;
  }

  .panel-content {
    padding: 1rem;
  }

  .details-label {
    font-size: 0.9rem;
    gap: 0.375rem;
  }

  .details-icon {
    font-size: 16px;
  }

  .details-text {
    font-size: 0.85rem;
  }

  /* Mobile cost panel */
  .cost-panel {
    padding: 1rem;
    border-radius: 12px;
  }

  .cost-label {
    font-size: 0.7rem;
  }

  .currency {
    font-size: 1rem;
  }

  .amount {
    font-size: 1.5rem;
  }
}

/* Tablet responsive styles (SM-MD) */
@media (min-width: 600px) and (max-width: 959.98px) {
  .hero-header {
    padding: 1.25rem 1.5rem;
    min-height: 100px;
  }

  .hero-content {
    gap: 1rem;
  }

  .hero-badge {
    width: 48px;
    height: 48px;
    border-radius: 14px;
  }

  .hero-icon {
    font-size: 24px;
  }

  .hero-subtitle {
    font-size: 1.4rem;
  }

  .content-container {
    padding: 1.25rem !important;
  }

  .modern-info-card {
    padding: 1.125rem;
  }

  .card-avatar {
    width: 40px;
    height: 40px;
  }

  .avatar-icon {
    font-size: 19px;
  }

  .panel-content {
    padding: 1.125rem;
  }

  .cost-panel {
    padding: 1.125rem;
  }

  .amount {
    font-size: 1.875rem;
  }

  .action-bar {
    gap: 0.875rem;
  }

  .action-button {
    min-width: 160px;
    height: 46px;
  }
}

/* Desktop responsive styles (LG+) */
@media (min-width: 960px) {
  .hero-header {
    padding: 1.5rem 2rem;
    min-height: 120px;
  }

  .content-container {
    padding: 1.5rem !important;
  }

  .info-row {
    margin-bottom: 1.5rem;
  }

  /* Hover effects for desktop */
  .modern-info-card:hover {
    transform: translateY(-2px);
    box-shadow: 0 6px 24px rgba(0, 0, 0, 0.12);
  }

  :deep(.fc-event):hover {
    transform: translateY(-1px);
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
  }

  :deep(.fc-button):hover:not(.fc-button-active) {
    background-color: #f0f4ff !important;
    border-color: #2b3086 !important;
    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.15) !important;
  }
}

/* Calendar responsive enhancements */
@media (max-width: 479.98px) {
  :deep(.fc-toolbar) {
    flex-direction: column;
    gap: 6px;
    padding: 8px 4px;
  }

  :deep(.fc-toolbar-chunk) {
    display: flex;
    justify-content: space-between;
    flex-wrap: wrap;
    gap: 4px;
  }

  :deep(.fc-prev-button),
  :deep(.fc-next-button) {
    min-width: 32px !important;
    padding: 6px 8px !important;
    font-size: 16px !important;
    margin: 0 2px;
  }

  :deep(.fc-today-button) {
    font-size: 11px !important;
    padding: 6px 10px !important;
    margin: 0 4px;
  }

  :deep(.fc-toolbar-title) {
    font-size: 18px !important;
    margin: 4px 0 !important;
    order: -1;
    width: 100%;
    text-align: center;
  }

  :deep(.fc-dayGridMonth-button),
  :deep(.fc-dayGridWeek-button),
  :deep(.fc-listWeek-button) {
    font-size: 10px !important;
    padding: 5px 6px !important;
    margin: 0 1px;
    min-width: 35px;
  }

  :deep(.fc-listWeek-button) {
    display: none;
  }
}

@media (min-width: 480px) and (max-width: 575.98px) {
  :deep(.fc-toolbar) {
    flex-direction: column;
    gap: 8px;
    padding: 10px 6px;
  }

  :deep(.fc-toolbar-chunk) {
    display: flex;
    justify-content: center;
    flex-wrap: wrap;
    gap: 6px;
  }

  :deep(.fc-toolbar-title) {
    font-size: 20px !important;
    margin: 6px 0 !important;
    order: -1;
    width: 100%;
    text-align: center;
  }
}

@media (max-width: 575.98px) {
  .calendarWidth {
    font-size: 12px;
    overflow-x: auto;
    -webkit-overflow-scrolling: touch;
  }

  :deep(.fc-col-header-cell) {
    font-size: 11px;
    padding: 4px 2px;
  }

  :deep(.fc-daygrid-day-number) {
    font-size: 12px;
    padding: 2px;
  }

  :deep(.fc-event-title) {
    font-size: 10px;
    line-height: 1.2;
  }
}

/* Enhanced scroll for mobile calendar */
@media (max-width: 767.98px) {
  .calendarWidth::-webkit-scrollbar {
    height: 4px;
  }

  .calendarWidth::-webkit-scrollbar-track {
    background: #f1f1f1;
    border-radius: 2px;
  }

  .calendarWidth::-webkit-scrollbar-thumb {
    background: #2b3086;
    border-radius: 2px;
  }
}

/* Loading and transition improvements for mobile */
@media (max-width: 599.98px) {
  .premium-appointment-card {
    transition: none;
  }

  .modern-info-card {
    transition: box-shadow 0.2s ease;
  }

  .action-button {
    transition: background-color 0.2s ease;
  }
}

/* Map container responsive */
.map-container {
  border-radius: 12px;
  overflow: hidden;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.08);
}

@media (max-width: 599.98px) {
  .map-container iframe {
    height: 200px !important;
    border-radius: 8px !important;
  }

  .map-placeholder .v-card {
    height: 150px !important;
  }

  .map-placeholder .v-icon {
    font-size: 36px !important;
  }

  .map-placeholder .text-body-2 {
    font-size: 0.8rem !important;
  }
}

/* Print styles */
@media print {
  .premium-appointment-card {
    max-height: none;
    overflow: visible;
  }

  .hero-header {
    background: #f5f5f5 !important;
  }

  .glass-overlay {
    display: none;
  }

  .close-button,
  .premium-actions {
    display: none !important;
  }
}

/* High contrast mode support */
@media (prefers-contrast: high) {
  .hero-subtitle {
    text-shadow: none;
  }

  .modern-info-card {
    border: 2px solid #333;
  }
}

/* Reduce motion for accessibility */
@media (prefers-reduced-motion: reduce) {
  * {
    animation-duration: 0.01ms !important;
    animation-iteration-count: 1 !important;
    transition-duration: 0.01ms !important;
  }

  .modern-info-card:hover {
    transform: none;
  }
}

/* Mobile drawer fix - ensure drawer is closed on page load */
.create-activity-drawer {
  z-index: 9999;
}

/* Fix for mobile drawer auto-opening */
@media (max-width: 767.98px) {
  .create-activity-drawer:not(.v-navigation-drawer--active) {
    transform: translateX(100%) !important;
    visibility: hidden !important;
  }
}

/* Additional safety for drawer state */
.z-indexDialog {
  z-index: 9999;
}

/* ปรับปรุง Button Layout */
.button-section {
  margin-top: 1.5rem !important;
}

.search-btn,
.reset-btn {
  font-weight: 600;
  border-radius: 8px;
  text-transform: none;
  letter-spacing: 0.025em;
  height: 44px;
  font-size: 14px;
  min-width: 110px;
  transition: all 0.2s ease;
}

.search-btn {
  box-shadow: 0 2px 8px rgba(25, 118, 210, 0.2);
}

.search-btn:hover {
  box-shadow: 0 4px 12px rgba(25, 118, 210, 0.3);
  transform: translateY(-1px);
}

.reset-btn:hover {
  background-color: #f5f5f5;
  transform: translateY(-1px);
}

/* Mobile responsive (≤599px) */
@media (max-width: 599.98px) {
  .button-section {
    margin-top: 1rem !important;
    justify-content: center !important;
  }

  .search-btn,
  .reset-btn {
    height: 42px;
    font-size: 13px;
    min-width: 90px;
  }

  .search-btn .v-icon,
  .reset-btn .v-icon {
    font-size: 16px;
  }
}

/* Small tablet (600px - 767px) */
@media (min-width: 600px) and (max-width: 767.98px) {
  .button-section {
    margin-top: 1.25rem !important;
    justify-content: center !important;
  }

  .search-btn,
  .reset-btn {
    height: 44px;
    font-size: 14px;
    min-width: 120px;
  }
}

/* Large tablet (768px - 959px) */
@media (min-width: 768px) and (max-width: 959.98px) {
  .button-section {
    margin-top: 1.5rem !important;
    justify-content: flex-end !important;
  }

  .search-btn,
  .reset-btn {
    height: 44px;
    font-size: 14px;
    min-width: 120px;
  }
}

/* Desktop (≥960px) */
@media (min-width: 960px) {
  .button-section {
    margin-top: 2rem !important;
    justify-content: flex-end !important;
  }

  .search-btn,
  .reset-btn {
    height: 44px;
    font-size: 14px;
    min-width: 120px;
  }
}

/* Focus states */
.search-btn:focus,
.reset-btn:focus {
  outline: 2px solid #2b3086;
  outline-offset: 2px;
}

/* Loading states */
.search-btn:disabled,
.reset-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
  transform: none;
}

/* Accessibility - reduced motion */
@media (prefers-reduced-motion: reduce) {
  .search-btn:hover,
  .reset-btn:hover {
    transform: none;
  }
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
