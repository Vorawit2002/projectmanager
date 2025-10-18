<template>
  <VRow class="d-flex justify-space-between align-center px-md-1">
    <VCol
      cols="12"
      class="d-flex align-center"
    >
      <span class="text-sub-title d-flex align-center">
        <v-icon
          icon="ri-id-card-line"
          class="mr-2"
        />
        รายการนัดหมาย
      </span>
    </VCol>
  </VRow>

  <VRow class="px-md-1">
    <!-- Filters Row -->
    <VCol cols="12">
      <VRow class="align-end">
        <!-- Event Type Filter -->
        <VCol
          :cols="auth.roles.includes('Admin') || auth.roles.includes('Manager') ? '12' : '12'"
          :sm="auth.roles.includes('Admin') || auth.roles.includes('Manager') ? '12' : '12'"
          :md="auth.roles.includes('Admin') || auth.roles.includes('Manager') ? '12' : '10'"
          class="d-flex justify-end align-end gap-2 flex-wrap"
        >
          <!-- Create Daily Schedule Dropdown -->
          <v-menu offset-y>
            <template v-slot:activator="{ props }">
              <v-btn
                color="success-darken-2"
                v-bind="props"
                class="w-auto"
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
              min-width="280"
              style="border-radius: 17px"
            >
              <v-list-item @click="CreateAppointmentPlan">
                <template v-slot:prepend>
                  <v-icon>ri-contract-line</v-icon>
                </template>
                <v-list-item-title>นัดหมายใหม่</v-list-item-title>
                <v-list-item-subtitle class="text-wrap">การนัดหมายลูกค้า ที่ต้องการบันทึกรายงานผล</v-list-item-subtitle>
              </v-list-item>
              <v-divider></v-divider>
              <v-list-item @click="goCreateActivity">
                <template v-slot:prepend>
                  <v-icon>ri-save-3-fill</v-icon>
                </template>
                <v-list-item-title>แจ้งงานรายวันใหม่</v-list-item-title>
                <v-list-item-subtitle class="text-wrap">แผนงานในแต่วัน ( กรณีที่อยู่ office )</v-list-item-subtitle>
              </v-list-item>
              <v-divider></v-divider>
              <v-list-item @click="DuplicateActivity">
                <template v-slot:prepend>
                  <v-icon>ri-file-copy-line</v-icon>
                </template>
                <v-list-item-title>สำเนาแจ้งงานรายวันล่าสุด</v-list-item-title>
                <v-list-item-subtitle class="text-wrap">สำเนาแจ้งงานรายวันจากวันล่าสุด</v-list-item-subtitle>
              </v-list-item>
            </v-list>
          </v-menu>
        </VCol>
        <VCol
          cols="12"
          sm="6"
          md="3"
        >
          <label class="mb-2 text-black">ประเภท</label>
          <v-select
            :items="EventTypeList"
            item-title="name"
            item-value="id"
            v-model="request.eventTypeId"
            @update:model-value="onEventTypeChange"
            density="comfortable"
            multiple
            variant="outlined"
            class="form-field"
          ></v-select>
        </VCol>

        <!-- Department Filter (Admin/Manager only) -->
        <VCol
          cols="12"
          sm="6"
          md="2"
          v-if="auth.roles.includes('Admin') || auth.roles.includes('Manager')"
        >
          <label class="mb-2 text-black">แผนก</label>
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
        </VCol>

        <!-- Employee Filter (Admin/Manager only) -->
        <VCol
          cols="12"
          sm="6"
          md="4"
          v-if="auth.roles.includes('Admin') || auth.roles.includes('Manager')"
        >
          <label class="mb-2 text-black">พนักงาน</label>
          <v-autocomplete
            :items="EmployeeList"
            item-title="name"
            item-value="id"
            placeholder="กรุณาเลือกพนักงาน"
            multiple
            clearable
            v-model="selectedEmployeeIds"
            :disabled="selectedDepartmentIds.length === 0"
            @update:model-value="onEmployeeChange"
            density="comfortable"
            variant="outlined"
            class="form-field"
          />
        </VCol>

        <!-- Action Buttons -->
        <VCol
          cols="12"
          sm="6"
          md="3"
        >
          <label class="mb-2 text-black">ช่วงเวลา</label>
          <v-select
            :items="SelectTimeList"
            item-title="title"
            item-value="value"
            placeholder="กรุณาเลือกช่วงเวลา"
            v-model="request.date"
            @update:model-value="onEmployeeChange"
            density="comfortable"
            variant="outlined"
            class="form-field"
          />
        </VCol>
      </VRow>
    </VCol>
  </VRow>

  <Card class="px-3 mt-0">
    <VTabs
      v-model="activeTab"
      show-arrows
      class="v-tabs-pill"
      color="secondary"
    >
      <VTab
        v-for="item in tabs"
        :key="item.icon"
        :value="item.tab"
        class="text-primary-darken-0 font-weight-black"
      >
        {{ item.title }}
      </VTab>
    </VTabs>
    <VRow
      class="mt-5 mb-4 px-md-6"
      v-if="EventTypeIsActivityPlan"
    >
      <VCol
        cols="12"
        sm="6"
        md="4"
      >
        <VCard
          class="cursor-pointer card-with-left-border card-blue position-relative"
          :class="{ 'card-active': currentFilter === 'all' }"
          @click="handleSelectAll"
        >
          <div class="status-indicator bg-primary"></div>
          <VCardText class="text-black"> แสดงข้อมูลทั้งหมด {{ totalAllItems }} รายการ </VCardText>
        </VCard>
      </VCol>

      <VCol
        cols="12"
        md="4"
      >
        <VCard
          class="cursor-pointer card-with-left-border card-green position-relative"
          :class="{ 'card-active': currentFilter === 'summary' }"
          @click="handleSelectSummary"
        >
          <div class="status-indicator bg-success"></div>
          <VCardText class="text-black"> บันทึกรายงานแล้ว {{ ActivitySummary }} รายการ </VCardText>
        </VCard>
      </VCol>
      <VCol
        cols="12"
        md="4"
      >
        <VCard
          class="cursor-pointer card-with-left-border card-orange position-relative"
          :class="{ 'card-active': currentFilter === 'not-summary' }"
          @click="handleSelectNotSummary"
        >
          <div class="status-indicator bg-warning"></div>
          <VCardText class="text-black"> ยังไม่บันทึกรายงาน {{ ActivityNotSummary }} รายการ </VCardText>
        </VCard>
      </VCol>
    </VRow>

    <VWindow
      v-model="activeTab"
      class="mt-5 disable-tab-transition px-md-6"
      :touch="false"
      :item-class="getRowClass"
    >
      <!-- Card -->
      <VWindowItem value="Card">
        <CardAppointmentDetailView
          :local_data="filteredActivity"
          :itemlength="Itemlength"
          :pagenumber="pageNumber"
          :pagesize="pageSize"
          :activeTab="activeTab"
          :highlightedId="String(highlightedId)"
          :filterType="currentFilter"
          @pagesize="handlePageSizeChange"
          @pagenumber="handlePageChange"
          @update:highlightedId="highlightedId = String($event)"
          @refresh-needed="handleRefreshNeeded"
        />
        <!-- @navigate="handleNavigateFromCard" -->
      </VWindowItem>

      <!-- Table -->
      <VWindowItem value="Table">
        <VCard class="card-table mb-6">
          <v-data-table-server
            v-model:page="pageNumber"
            v-model:items-per-page="pageSize"
            :headers="headertable"
            :items="filteredActivity"
            :items-length="Itemlength"
            :loading="isLoading"
            class="text-no-wrap"
            @update:page="handlePageChange"
            @update:items-per-page="handlePageSizeChange"
            @update:options="loadItems"
            :item-class="getRowClass"
          >
            <template v-slot:header.employeeFullName>
              <div
                @click="handleSortemployee"
                style="cursor: pointer"
                class="d-flex align-center"
              >
                ผู้รับผิดชอบ
                <v-icon
                  small
                  class="ml-1"
                >
                  {{ sortDescemployee ? 'ri-arrow-down-line' : 'ri-arrow-up-line' }}
                </v-icon>
              </div>
            </template>
            <template v-slot:header.date>
              <div
                @click="handleSortDate"
                style="cursor: pointer"
                class="d-flex align-center"
              >
                วันที่
                <v-icon
                  small
                  class="ml-1"
                >
                  {{ sortDescDate ? 'ri-arrow-down-line' : 'ri-arrow-up-line' }}
                </v-icon>
              </div>
            </template>
            <template v-slot:item="{ item }">
              <tr
                :class="{ 'selected-row': item.id && highlightedId && item.id.toString() === highlightedId.toString() }"
                :data-row-id="item.id"
              >
                <!-- จัดการ -->
                <td class="text-center">
                  <div class="flex justify-start align-center">
                    <v-menu v-if="item.eventTypes?.eventTypeCode == '001'">
                      <template v-slot:activator="{ props }">
                        <v-btn
                          v-bind="props"
                          color="primary-darken-0"
                          icon
                          density="compact"
                          rounded="lg"
                          v-tooltip="{
                            text: 'จัดการนัดหมาย',
                            contentClass: 'bg-primary text-white',
                            location: 'start',
                          }"
                        >
                          <v-icon size="18">ri-settings-5-line</v-icon>
                        </v-btn>
                      </template>
                      <v-list>
                        <v-list-item
                          v-for="Activity in ActivityActions"
                          @click="Activity.action(item)"
                        >
                          <v-list-item-title class="text-primary font-weight-medium">{{
                            Activity.title
                          }}</v-list-item-title>
                        </v-list-item>
                      </v-list>
                    </v-menu>
                    <v-menu v-else>
                      <template v-slot:activator="{ props }">
                        <v-btn
                          v-bind="props"
                          color="primary-darken-0"
                          icon
                          density="compact"
                          rounded="lg"
                          v-tooltip="{
                            text: 'จัดการแจ้งงานรายวัน',
                            contentClass: 'bg-primary text-white pa-2 rounded',
                            location: 'start',
                          }"
                        >
                          <v-icon size="18">ri-settings-5-line</v-icon>
                        </v-btn>
                      </template>
                      <v-list>
                        <v-list-item
                          v-for="Daily in DailyActions"
                          @click="Daily.action(item.id)"
                        >
                          <v-list-item-title class="text-primary font-weight-medium">{{
                            Daily.title
                          }}</v-list-item-title>
                        </v-list-item>
                      </v-list>
                    </v-menu>
                  </div>
                </td>
                <!-- ผู้รับผิดชอบ พร้อม Avatar และสถานะ -->
                <td class="d-flex align-center">
                  <div class="position-relative mr-2">
                    <v-avatar
                      size="40"
                      color="primary"
                      variant="tonal"
                    >
                      <template v-if="item.employees?.imageProfile">
                        <VImg :src="getImageUrl(item.employees.imageProfile)" />
                      </template>
                      <template v-else>
                        {{ getInitials(item.employees?.firstName, item.employees?.lastName) }}
                      </template>
                    </v-avatar>
                    <!-- Status indicator -->
                    <div
                      v-if="item.eventTypes?.eventTypeCode == '001'"
                      class="status-dot position-absolute"
                      :class="getEmployeeStatusClass(item)"
                    ></div>
                  </div>
                  <span>
                    {{ item.employees?.titleName || '' }} {{ item.employees?.firstName || '' }}
                    {{ item.employees?.lastName || '' }}
                  </span>
                </td>

                <!-- วัตถุประสงค์ -->
                <td>
                  <div
                    v-if="item.eventTypes?.eventTypeCode == '001'"
                    style="white-space: pre-wrap; word-break: break-word"
                  >
                    {{ item.objective }}
                  </div>

                  <div v-else>
                    <v-tooltip
                      location="top"
                      :content-class="'bg-secondary text-on-surface'"
                    >
                      <template v-slot:activator="{ props }">
                        <span
                          v-bind="props"
                          class="truncate-cell"
                        >
                          {{ item.detail }}
                        </span>
                      </template>
                      <div style="max-width: 250px; white-space: pre-wrap">
                        {{ item.detail }}
                      </div>
                    </v-tooltip>
                  </div>
                </td>

                <!-- หน่วยงาน / ลูกค้า -->
                <td v-if="item.eventTypes?.eventTypeCode == '001'">
                  {{ item.organizations?.name }}
                </td>
                <td v-else-if="item.eventTypes?.eventTypeCode == '002' && checkEventType"></td>

                <!-- สถานที่ -->
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
                        {{ item.eventTypes?.eventTypeCode == '001' ? item.location : item.projects?.projectName || '' }}
                      </div>
                    </template>
                    <template #default>
                      <div style="white-space: pre-wrap; max-width: 300px">
                        {{ item.eventTypes?.eventTypeCode == '001' ? item.location : item.projects?.projectName || '' }}
                      </div>
                    </template>
                  </v-tooltip>
                </td>

                <!-- วันที่ -->
                <td>
                  {{ formatDateforshow(item.startDate, item.endDate, item.allDay) }}
                </td>

                <td v-if="checkEventType">
                  <v-chip
                    class="ml-2"
                    size="small"
                    variant="tonal"
                    :color="item.eventTypes?.eventTypeCode == '001' ? 'primary' : 'warning'"
                  >
                    {{ item.eventTypes?.eventTypeCode == '001' ? 'นัดหมาย' : 'แจ้งงานรายวัน' }}
                  </v-chip>
                </td>
              </tr>
            </template>
          </v-data-table-server>
        </VCard>
      </VWindowItem>

      <!-- DailySchedule -->
      <VWindowItem value="DailySchedule"> 000 </VWindowItem>
    </VWindow>
  </Card>
  <!-- เลือกวันที่ -->
  <v-dialog
    v-model="DialogSelectedTime"
    :max-width="$vuetify.display.xs ? '100%' : '800px'"
    :max-height="$vuetify.display.xs ? '100vh' : '90vh'"
    transition="dialog-top-transition"
    :fullscreen="$vuetify.display.xs"
  >
    <v-card class="">
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

  <v-navigation-drawer
    v-model="DialogCreate"
    :width="$vuetify.display.xs ? '100vw' : '550'"
    class="z-indexDialog create-activity-drawer"
    close-on-back
    transition="dialog-right-transition"
    temporary
    location="right"
    scrollable
    :permanent="false"
  >
    <CreateCustomerDailySchedule :CloseDialogCreate="CloseDialogCreate" />
  </v-navigation-drawer>

  <v-navigation-drawer
    v-model="DialogUpdate"
    class="z-indexDialog create-activity-drawer"
    :width="$vuetify.display.xs ? '100vw' : '550'"
    :id="id"
    transition="dialog-right-transition"
    close-on-back
    temporary
    location="right"
    scrollable
    :permanent="false"
  >
    <UpdateCustomerDailySchedule
      :id="id"
      :CloseDialogUpdate="CloseDialogUpdate"
      :mode="mode"
    />
  </v-navigation-drawer>

  <!-- Manage Activity Dialog -->
  <v-navigation-drawer
    v-model="manageActivityDialog"
    scrollable
    :id="id"
    transition="dialog-right-transition"
    :width="$vuetify.display.xs ? '100vw' : '550'"
    class="z-indexDialog create-activity-drawer"
    close-on-back
    location="right"
    temporary
    :permanent="false"
  >
    <ManageActivityDetail
      :id="(selectedActivityId as string | number)"
      :activitytab="activitytab"
      @close="closeManageActivityDialog"
    />
  </v-navigation-drawer>

  <!-- Create Activity Plan Dialog -->
  <v-navigation-drawer
    v-model="createActivityPlanDialog"
    :id="id"
    :width="$vuetify.display.xs ? '100vw' : '550'"
    scrollable
    class="z-indexDialog create-activity-drawer"
    location="right"
    transition="dialog-right-transition"
    close-on-back
    temporary
    :permanent="false"
  >
    <CreateCustomerAppointmentPlan
      @close="closeCreateActivityPlanDialog"
      @created="handleActivityPlanCreated"
    />
  </v-navigation-drawer>

  <v-navigation-drawer
    v-model="dailyScheduleDetail"
    scrollable
    transition="dialog-righ-transition"
    :width="$vuetify.display.xs ? '100vw' : '550'"
    class="z-indexDialog create-activity-drawer"
    location="right"
    close-on-back
  >
    <div v-if="id">
      <CustomerDailyScheduleDetailView
        :id="String(id)"
        :mode="'edit'"
        :CloseDialogUpdate="handleCloseDailyScheduleDetail"
      />
    </div>
  </v-navigation-drawer>

  <v-navigation-drawer
    v-model="detailDialog"
    scrollable
    transition="dialog-righ-transition"
    :width="$vuetify.display.xs ? '100vw' : '550'"
    class="z-indexDialog create-activity-drawer"
    location="right"
    close-on-back
  >
    <div v-if="selectedActivityId">
      <ManageActivityDetailViews
        :id="String(selectedActivityId)"
        @close="closeDetailDialog"
      />
    </div>
  </v-navigation-drawer>
</template>

<script lang="ts">
import '@/assets/styles/CustomerAppointmentPlan.css'
import {
  ActivityPlanStatus,
  Client,
  GetActivityPlanLatestQuery,
  GetActivityPlanWithPaginationQuery,
  GetEmployeeByDepartmentIdQuery,
} from '@/client'
import CardAppointmentDetailView from '@/components/CardAppointmentDetailView.vue'
import TextFieldDatepicker from '@/components/Datepicker/TextFieldDatepicker.vue'
import TextFieldTimepicker from '@/components/Datepicker/TextFieldTimepicker.vue'
import { BACKEND_API_URL } from '@/constants'
import { useAuthStore, useSweetAlertStore } from '@/stores'
import { appointmentEndDateRules, appointmentStartDateRules } from '@/utils/RuleServices'
import { defineComponent } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import CreateCustomerAppointmentPlan from './CreateCustomerAppointmentPlan.vue'
import CreateCustomerDailySchedule from './CreateCustomerDailySchedule.vue'
import CustomerDailyScheduleDetailView from './CustomerDailyScheduleDetailView.vue'
import ManageActivityDetail from './ManageActivityDetail.vue'
import ManageActivityDetailViews from './ManageActivityDetailViews.vue'
import UpdateCustomerDailySchedule from './UpdateCustomerDailySchedule.vue'

interface TabItem {
  title: string
  icon: string
  tab: string
}

type FilterType = 'all' | 'summary' | 'not-summary'

const client = new Client(BACKEND_API_URL)

export default defineComponent({
  name: 'CustomerAppointmentPlanListView',
  setup() {
    const router = useRouter()
    const route = useRoute()

    return { router, route }
  },
  components: {
    TextFieldDatepicker,
    TextFieldTimepicker,
    CardAppointmentDetailView,
    CreateCustomerDailySchedule,
    CreateCustomerAppointmentPlan,
    UpdateCustomerDailySchedule,
    ManageActivityDetail,
    CustomerDailyScheduleDetailView,
    ManageActivityDetailViews,
  },
  data() {
    return {
      activeTab: 'Card' as string,
      DialogCreate: false as boolean,
      tabs: [
        { title: 'มุมมองการ์ด', icon: 'ri-info-card-line', tab: 'Card' },
        { title: 'มุมมองตาราง', icon: 'ri-table-line', tab: 'Table' },
      ] as TabItem[],
      headertable: [] as any[],
      search: '',
      // Filter data from CardVueCalendar
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
      SelectTimeList: [
        { title: 'วันนี้', value: 'วันนี้' },
        { title: 'สัปดาห์นี้', value: 'สัปดาห์นี้' },
        { title: 'เดือนนี้', value: 'เดือนนี้' },
        { title: 'ระบุช่วงเวลา', value: 'ระบุ' },
      ] as any,
      DepartmentList: [] as any[],
      selectedDepartmentIds: [] as string[],
      EmployeeList: [] as any[],
      selectedYear: '2025' as string,
      selectedEmployeeIds: [] as string[],
      employeeAllSelected: false as boolean, // sentinel state for ALL employees
      lastEmployeeAllSelected: false as boolean, // track last state to allow directional toggle
      defaultDepartmentId: '' as string | null,
      defaultEmployeeId: '' as string | null,
      Activity: [] as any[],
      ActivitySummary: 0 as number,
      ActivityNotSummary: 0 as number,
      isLoading: false as boolean,
      Itemlength: 0 as number,
      pageNumber: 1 as number,
      pageSize: 10 as number,
      request: new GetActivityPlanWithPaginationQuery(),
      auth: useAuthStore(),
      sweetAlertStore: useSweetAlertStore(),
      highlightedId: null as string | null,
      sortDescemployee: false as boolean,
      sortDescDate: false as boolean,
      currentFilter: 'all' as FilterType,
      EventTypeList: [] as any,
      EventTypeIsActivityPlan: false,
      DialogUpdate: false,
      id: '' as any,
      DialogCreateSelect: false,
      mode: 'edit' as 'edit' | 'duplicate',
      isNavigating: false as boolean,
      manageActivityDialog: false as boolean,
      selectedActivityId: null as string | number | null,
      createActivityPlanDialog: false as boolean,
      dailyScheduleDetail: false,
      detailDialog: false,
      DailyActions: [
        { title: 'แก้ไขการแจ้งงาน', action: (itemId: any) => this.GoUpdate(itemId) },
        { title: 'สำเนาการแจ้งงาน', action: (itemId: any) => this.DuplicateActivityBySelect(itemId) },
        { title: 'รายละเอียดการแจ้งงาน', action: (itemId: any) => this.openDailyScheduleDetail(itemId) },
        { title: 'ลบการแจ้งงาน', action: (itemId: any) => this.deleteDailySchedule(itemId) },
      ],
      ActivityActions: [
        { title: 'แก้ไขนัดหมาย', action: (itemId: any) => this.Gosummarize(itemId, 'Activity') },
        { title: 'รายละเอียดนัดหมาย', action: (itemId: any) => this.openDetailDialog(itemId) },
        { title: 'สรุปผลนัดหมาย', action: (itemId: any) => this.Gosummarize(itemId, 'Report') },
        { title: 'ลบนัดหมาย', action: (itemId: any) => this.deleteActivity(itemId) },
      ],
      activitytab: 'EditReport' as string,
      checkEventType: false,
      DialogSelectedTime: false,
      appointmentStartDateRules,
      appointmentEndDateRules,
      lastSelectionIsAll: false as boolean,
    }
  },
  computed: {
    departmentItems(): any[] {
      return [{ id: 'ALL', name: 'ทั้งหมด' }, ...this.DepartmentList]
    },
    multiDeptMode(): any {
      return this.selectedDepartmentIds.length > 1 && !this.selectedDepartmentIds.includes('ALL')
    },
    effectiveDepartmentIds(): any[] {
      // Only real ids (exclude ALL sentinel)
      return this.selectedDepartmentIds
    },
    filteredActivity(): any[] {
      if (this.currentFilter === 'all') {
        return this.Activity
      } else if (this.currentFilter === 'summary') {
        return this.Activity.filter((item: any) => item.hasPlanNote === true)
      } else if (this.currentFilter === 'not-summary') {
        return this.Activity.filter((item: any) => item.hasPlanNote === false)
      }
      return this.Activity
    },
    filteredItemLength(): number {
      return this.filteredActivity.length
    },
    paginatedActivity(): any[] {
      if (this.pageSize === -1) {
        return this.filteredActivity
      }
      const start = (this.pageNumber - 1) * this.pageSize
      const end = start + this.pageSize
      return this.filteredActivity.slice(start, end)
    },
    totalAllItems(): number {
      return this.Itemlength
    },
  },
  created() {
    const tab = this.$route.query.tab as string
    if (tab === 'Card' || tab === 'Table') {
      this.activeTab = tab
    }
    if (!tab) {
      const savedState = localStorage.getItem('appointmentListState')
      if (savedState) {
        try {
          const state = JSON.parse(savedState)
          if (state.tab) {
            this.activeTab = state.tab
          }
          // if ( state.eventTypeId) {
          //   this.request.eventTypeId = state.eventTypeId
          // }
        } catch (error) {
          console.error('Error parsing saved state in created:', error)
        }
      }
    }
  },
  watch: {
    activeTab(newTab: string) {
      this.$router.replace({
        query: {
          ...this.$route.query,
          tab: newTab,
        },
      })
    },
    currentFilter() {
      this.pageNumber = 1
    },
  },
  async mounted() {
    // เพิ่มการปิด drawer บังคับก่อนโหลดหน้า
    this.forceCloseAllDrawers()
    this.loadSavedState()
    const isPageRefresh = this.isActualRefresh()

    await this.getEventTypeList()
    await this.getDepartmentList()
    if (
      (!this.request.employeeId || this.request.employeeId?.length === 0) &&
      (!this.request.departmentId)
    ) {
      this.request.employeeId = [] // กำหนด array ใหม่ถ้ายังไม่มี
      this.selectedEmployeeIds = []
      const result = await client.getEmployeeQueryByUserID(this.auth.userId)
      if (!this.request.employeeId?.includes(result.id!)) {
     
        this.selectedDepartmentIds = result.departmentId ? [result.departmentId] : []
        // store defaults
           this.request.departmentId =  this.selectedDepartmentIds
        this.defaultDepartmentId = result.departmentId || ''
        this.defaultEmployeeId = result.id || ''
        await this.getEmployeeByDepartment()
        this.selectedEmployeeIds?.push(result.id!)
      }
    }
    this.request.date = 'สัปดาห์นี้'
    this.request.sortDesc = true
    this.request.sortColumn = 'StartDate'
    // this.sortDescDate = true
    await this.initialize()
    // เพิ่ม event listener สำหรับ reloadAppointmentPlan
    window.addEventListener('reloadAppointmentPlan', this.handleReloadAppointmentPlan)

    // Add fallback: ESC key closes drawer if open
    window.addEventListener('keydown', this.handleEscCloseDrawer)

    // เพิ่ม listener สำหรับ back button บนมือถือ
    window.addEventListener('popstate', this.handleBackButton)

    // หากเป็น page refresh ให้เคลียร์ highlight และ localStorage ทันที
    if (isPageRefresh) {
      this.highlightedId = null
      localStorage.removeItem('selectedAppointmentId')
    } else {
      // Restore highlight from localStorage ONLY if NOT a page refresh
      const storedId = localStorage.getItem('selectedAppointmentId')

      if (storedId) {
        setTimeout(() => {
          this.highlightedId = String(storedId)
          localStorage.removeItem('selectedAppointmentId')

          setTimeout(() => {
            this.scrollToHighlightedRow()
          }, 300)
        }, 100)
      }

      if (this.highlightedId) {
        setTimeout(() => {
          this.scrollToHighlightedRow()
        }, 500)
      }
    }

    setTimeout(() => {
      localStorage.removeItem('appointmentListState')
    }, 1000)
  },

  beforeUnmount() {
    // ลบ event listeners เพื่อป้องกัน memory leaks
    window.removeEventListener('reloadAppointmentPlan', this.handleReloadAppointmentPlan)
    window.removeEventListener('keydown', this.handleEscCloseDrawer)
    window.removeEventListener('popstate', this.handleBackButton)
  },

  methods: {
    async onYearChange() {
      this.pageNumber = 1
      await this.initialize()
    },
    async getDatatime() {
      const form = this.$refs.form as any
      const { valid } = await form.validate()
      if (!valid) return
      try {
        this.request.date = 'ระบุช่วงเวลา'
        await this.initialize()
        this.CloseDialogSelectTime()
      } catch (error) {
        console.error(error)
      }
    },
    async onDepartmentChange() {
      await this.getEmployeeByDepartment()
      this.pageNumber = 1
      await this.initialize()
    },
    async onDepartmentMultiChange(newValue?: any[]) {
      let value = Array.isArray(newValue) ? [...newValue] : [...this.selectedDepartmentIds]
            this.selectedEmployeeIds = []
      if (value.length === 0) {
        // เมื่อกด clearable ให้เคลียร์ทั้งหมด ไม่กู้คืนค่า default
        // ไม่ทำอะไร ให้ value เป็น array ว่าง
                value = []
  
      }
      if (value.includes('ALL') ) {
        if (value.length === 1) {
          this.lastSelectionIsAll = true
          value = []
        } else {
          if (this.lastSelectionIsAll) {
            value = value.filter(v => v !== 'ALL')
            this.lastSelectionIsAll = false
          } else {
            value = []
            this.lastSelectionIsAll = true
          }
        }
      } else {
        this.lastSelectionIsAll = false
      }

      this.selectedDepartmentIds = value

      if (value.length === 1 && value[0] !== 'ALL') {
        this.request.departmentId = value[0] as any
      } else {
        this.request.departmentId = undefined as any
      }

      await this.getEmployeeByDepartment()

      // ไม่เลือกตัวเองอัตโนมัติเมื่อเปลี่ยนแผนก ให้ผู้ใช้เลือกเอง
      this.pageNumber = 1
      await this.initialize()
    },

    async onEmployeeChange() {
      // const ALL_EMP = 'ALL_EMP'
      // let value = [...this.selectedEmployeeIds]

      // if (value.includes(ALL_EMP)) {
      //   if (value.length === 1) {
      //     this.lastEmployeeAllSelected = true
      //     this.employeeAllSelected = true
      //     value = ['ALL_EMP']
      //     this.selectedEmployeeIds = value
      //     // เมื่อเลือก "ทั้งหมด" ให้ส่งรายการพนักงานทั้งหมดในแผนกที่เลือก แทนที่จะเป็น undefined
      //     this.request.employeeId = this.getAllEmployeeIdsInSelectedDepartments()
      //   } else {
      //     if (this.lastEmployeeAllSelected) {
      //       value = value.filter(v => v !== ALL_EMP)
      //       this.selectedEmployeeIds = value
      //       this.lastEmployeeAllSelected = false
      //       this.employeeAllSelected = false
      //       this.request.employeeId = value.length > 0 ? [...value] : undefined
      //     } else {
      //       value = ['ALL_EMP']
      //       this.selectedEmployeeIds = value
      //       this.lastEmployeeAllSelected = true
      //       this.employeeAllSelected = true
      //       // เมื่อเลือก "ทั้งหมด" ให้ส่งรายการพนักงานทั้งหมดในแผนกที่เลือก แทนที่จะเป็น undefined
      //       this.request.employeeId = this.getAllEmployeeIdsInSelectedDepartments()
      //     }
      //   }
      // } else {
      //   this.lastEmployeeAllSelected = false
      //   this.employeeAllSelected = false
      //   this.selectedEmployeeIds = value
      //   if (value.length > 0) {
      //     this.request.employeeId = [...value]
      //   } else {
      //     // cleared -> ไม่กู้คืนค่า default ให้เคลียร์ทั้งหมด
      //     this.request.employeeId = undefined
      //   }
      // }

      if (!this.request.date?.includes('ระบุ')) {
        this.pageNumber = 1
        await this.initialize()
      } else {
        this.DialogSelectedTime = true
      }
    },
    async getEmployeeByDepartment() {
      try {
        const prevHadAll = this.selectedEmployeeIds.includes('ALL_EMP')
        // this.selectedEmployeeIds = []
        if (this.multiDeptMode) {
          const uniqueMap: Record<string, any> = {}
          for (const deptId of this.effectiveDepartmentIds) {
            const reqEmp = new GetEmployeeByDepartmentIdQuery()
            reqEmp.departmentId = deptId
            try {
              const list = await client.getEmployeeQueryByDepartmentId(reqEmp)
              list.forEach((emp: any) => {
                emp.name = emp.firstName + ' ' + emp.lastName
                uniqueMap[emp.id] = emp
              })
            } catch (e) {
              console.warn('Failed to load employees for department', deptId, e)
            }
          }
          // กรองชื่อตัวเองออกถ้าเลือกแผนกอื่นที่ไม่ใช่แผนกตัวเอง
          const filteredEmployees = this.filterCurrentUserFromOtherDepartments(Object.values(uniqueMap))
          this.EmployeeList = this.injectAllEmployeeOption(filteredEmployees)
          return
        }
        if (!this.multiDeptMode && this.selectedDepartmentIds.length === 1 && this.selectedDepartmentIds[0] === 'ALL') {
          const uniqueAll: Record<string, any> = {}
          for (const dept of this.DepartmentList) {
            if (!dept.id) continue
            const reqEmp = new GetEmployeeByDepartmentIdQuery()
            reqEmp.departmentId = dept.id
            try {
              const list = await client.getEmployeeQueryByDepartmentId(reqEmp)
              list.forEach((emp: any) => {
                emp.name = emp.firstName + ' ' + emp.lastName
                uniqueAll[emp.id] = emp
              })
            } catch (e) {
              console.warn('Failed to load employees for department (ALL mode)', dept.id, e)
            }
          }
          this.EmployeeList = this.injectAllEmployeeOption(Object.values(uniqueAll))
          return
        }
        // Single department mode
        if (
          !this.request.departmentId ||
          this.selectedDepartmentIds.length === 0
        ) {
          this.EmployeeList = this.injectAllEmployeeOption([])
          return
        }
        const requestEmployee = new GetEmployeeByDepartmentIdQuery()
        // ใช้ selectedDepartmentIds[0] แทน this.request.departmentId เพื่อให้แน่ใจว่าเป็น string
        requestEmployee.departmentId = this.selectedDepartmentIds[0]
        const list = await client.getEmployeeQueryByDepartmentId(requestEmployee)
        list.forEach((x: any) => (x.name = x.firstName + ' ' + x.lastName))

        // กรองชื่อตัวเองออกถ้าเลือกแผนกอื่นที่ไม่ใช่แผนกตัวเอง
        const filteredList = this.filterCurrentUserFromOtherDepartments(list)
        this.EmployeeList = this.injectAllEmployeeOption(filteredList)

        if (prevHadAll) {
          this.selectedEmployeeIds = ['ALL_EMP']
          this.employeeAllSelected = true
        }
      } catch (error) {
        console.error(error)
      }
    },
    getAllEmployeeIdsInSelectedDepartments(): string[] {
      // ดึง ID ของพนักงานทั้งหมดในรายการปัจจุบัน (ยกเว้น ALL_EMP)
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
      const has = list.some(l => l.id === ALL_EMP)
      const enriched = [{ id: ALL_EMP, name: 'ทั้งหมด' }, ...list]
      return has ? list : enriched
    },
    async getDepartmentList() {
      try {
        const response = await client.getDepartmentQuery()
        this.DepartmentList = response || []
      } catch (error) {
        console.error('Error fetching department list:', error)
      }
    },

    async resetFilters() {
      this.isLoading = true
      try {
        this.selectedYear = '2025'
        this.request.departmentId = undefined
        this.selectedEmployeeIds = []
        this.request.eventTypeId = undefined
        ;(this.request as any).years = '2025'
        await this.getEmployeeByDepartment()
        await this.initialize()
      } catch (error) {
        console.error('Error resetting filters:', error)
      } finally {
        this.isLoading = false
      }
    },
    openDailyScheduleDetail(id: any) {
      this.mode = 'edit'
      // Set highlight และ emit ไปให้ parent
      this.highlightedId = String(id)

      // บันทึกใน localStorage สำหรับการกลับมา
      localStorage.setItem('selectedAppointmentId', String(id))
      this.id = id
      this.dailyScheduleDetail = true
    },
    openDetailDialog(item: any) {
      this.highlightedId = String(item.id)
      // บันทึกใน localStorage สำหรับการกลับมา
      localStorage.setItem('selectedAppointmentId', String(item.id))
      this.selectedActivityId = item.id
      this.detailDialog = true
    },
    handleCloseDailyScheduleDetail() {
      this.dailyScheduleDetail = false
    },
    closeDetailDialog() {
      this.detailDialog = false
    },
    forceCloseAllDrawers() {
      this.DialogCreate = false
      this.DialogUpdate = false
      this.manageActivityDialog = false
      this.createActivityPlanDialog = false
      this.DialogCreateSelect = false
    },
    handleBackButton() {
      if (this.DialogCreate || this.DialogUpdate || this.manageActivityDialog || this.createActivityPlanDialog) {
        this.forceCloseAllDrawers()
      }
    },
    handleEscCloseDrawer(e: KeyboardEvent) {
      if (e.key === 'Escape' || e.key === 'Esc') {
        let closed = false

        if (this.createActivityPlanDialog) {
          this.createActivityPlanDialog = false
          closed = true
        } else if (this.DialogCreate) {
          this.DialogCreate = false
          closed = true
        } else if (this.DialogUpdate) {
          this.DialogUpdate = false
          closed = true
        } else if (this.manageActivityDialog) {
          this.manageActivityDialog = false
          closed = true
        }

        if (closed) {
          e.preventDefault()
          e.stopPropagation()
        }
      }
    },
    getInitials(firstName: string = '', lastName: string = ''): string {
      const firstInitial = firstName.trim().charAt(0) || ''
      const lastInitial = lastName.trim().charAt(0) || ''
      return (firstInitial + lastInitial).toUpperCase()
    },
    getImageUrl(imageProfile: string): string {
      if (!imageProfile) return ''
      
      // If it's already a full URL (http/https) or data URL (data:), use as is
      if (imageProfile.startsWith('http') || imageProfile.startsWith('data:')) {
        return imageProfile
      }
      
      // If it's a relative path, prepend BACKEND_API_URL
      const BACKEND_API_URL = 'https://localhost:5001'
      return `${BACKEND_API_URL}${imageProfile.startsWith('/') ? '' : '/'}${imageProfile}`
    },
    getEmployeeStatusClass(item: any): string {
      if (item.haveCost === true || item.haveCost === false) {
        return 'status-online' // สีเขียว - บันทึกรายงานแล้ว
      } else {
        return 'status-away' // สีส้ม - ยังไม่บันทึกรายงาน
      }
    },
    loadItems({ page, itemsPerPage, sortBy }: { page: number; itemsPerPage: number; sortBy: any[] }) {
      this.pageNumber = page
      this.pageSize = itemsPerPage

      this.initialize()
    },

    isActualRefresh(): boolean {
      const navigation = window.performance.getEntriesByType('navigation')[0] as any

      if (navigation && navigation.type === 'reload') {
        const hasStoredId = localStorage.getItem('selectedAppointmentId')
        if (hasStoredId) {
          localStorage.removeItem('selectedAppointmentId')
        }
        return true
      }

      if (window.performance.navigation && window.performance.navigation.type === 1) {
        const hasStoredId = localStorage.getItem('selectedAppointmentId')
        if (hasStoredId) {
          localStorage.removeItem('selectedAppointmentId')
        }
        return true
      }

      const hasStoredId = localStorage.getItem('selectedAppointmentId')

      if (hasStoredId) {
        return false
      }

      return false
    },

    handleSelectAll(): void {
      this.currentFilter = 'all'
    },
    handleSelectSummary(): void {
      this.currentFilter = 'summary'
    },
    handleSelectNotSummary(): void {
      this.currentFilter = 'not-summary'
    },

    async initialize(): Promise<void> {
      this.Activity = []
      try {
        this.isLoading = true

  
        if (this.selectedEmployeeIds.includes('ALL_EMP') || this.employeeAllSelected) {
          // เมื่อเลือก "ทั้งหมด" ให้ใช้รายการพนักงานทั้งหมดในแผนกที่เลือก
          // แทนที่จะเป็น undefined ซึ่งจะดึงข้อมูลจากทุกแผนก
          this.request.employeeId = this.getAllEmployeeIdsInSelectedDepartments()
        } else if (this.selectedEmployeeIds.length > 0) {
          this.request.employeeId = [...this.selectedEmployeeIds]
        } else {
          if (!this.selectedEmployeeIds || this.selectedEmployeeIds.length === 0) {
            this.request.employeeId = undefined
          }
        }
        if (this.request.eventTypeId && this.request.eventTypeId.length === 1) {
          this.checkEventType = false
          this.EventTypeList.forEach((item: any) => {
            if (item.name === 'นัดหมาย') {
              if (this.request.eventTypeId?.includes(item.id)) {
                this.EventTypeIsActivityPlan = true
                this.headertable = [
                  { title: 'จัดการ', value: 'actions', align: 'center' },
                  { title: 'ผู้รับผิดชอบ', value: 'employeeFullName' },
                  { title: 'วัตถุประสงค์', value: 'objective' },
                  { title: 'หน่วยงาน / ลูกค้า', value: 'organizations.name' },
                  { title: 'สถานที่', value: 'location' },
                  { title: 'วันที่', value: 'date' },
                ]
              }
            } else {
              if (this.request.eventTypeId?.includes(item.id)) {
                this.EventTypeIsActivityPlan = false
                this.headertable = [
                  { title: 'จัดการ', value: 'actions', align: 'center' },
                  { title: 'ผู้รับผิดชอบ', value: 'employeeFullName' },
                  { title: 'รายละเอียด', value: 'detail' },
                  { title: 'โครงการ', value: 'projects' },
                  { title: 'วันที่', value: 'date' },
                ]
              }
            }
          })
        } else if (!this.request.eventTypeId || this.request.eventTypeId.length > 1) {
          this.checkEventType = true
          this.EventTypeIsActivityPlan = false
          this.headertable = [
            { title: 'จัดการ', value: 'actions', align: 'center' },
            { title: 'ผู้รับผิดชอบ', value: 'employeeFullName' },
            { title: 'วัตถุประสงค์', value: 'objective' },
            { title: 'หน่วยงาน / ลูกค้า', value: 'organizations.name' },
            { title: 'สถานที่', value: 'location' },
            { title: 'วันที่', value: 'date' },
            { title: 'ประเภท', value: 'eventtype' },
          ]
        }
        // Remove search filter since we removed the search field
        this.request.search = undefined

        // Apply year filter (using years property like in CardVueCalendar)
        if (this.selectedYear) {
          ;(this.request as any).years = this.selectedYear
        } else {
          ;(this.request as any).years = undefined
        }

        this.request.pageNumber = this.pageNumber
        this.request.pageSize = this.pageSize === -1 ? 9999 : this.pageSize
        this.request.activityPlanStatus = ActivityPlanStatus.All
        this.request.departmentId = this.selectedDepartmentIds.length > 0 ? [...this.selectedDepartmentIds] : undefined
        console.log(this.request)
          const response = await client.getActivityPlanWithPagination(this.request)
          this.Activity = response.items || []
          this.Itemlength = response.totalCount || 0
          
          // Debug: Check if imageProfile is in response
          if (this.Activity.length > 0) {
            console.log('First activity item:', this.Activity[0])
            console.log('Employee data:', this.Activity[0].employees)
            console.log('ImageProfile:', this.Activity[0].employees?.imageProfile)
          }
        // }

        if (this.Activity.length > 0) {
          const planNotePromises = this.Activity.map(async (item: any) => {
            try {
              if (!item.id) return item
              const activityPlanId = String(item.id)
              const result = await client.getPlanNoteQueryByActivityPlanId(activityPlanId)

              if (result && result.id !== '00000000-0000-0000-0000-000000000000') {
                item.planNote = result
                item.hasPlanNote = true
              } else {
                item.planNote = null
                item.hasPlanNote = false
              }
              return item
            } catch (error) {
              console.warn(`Failed to get plan note for item ${item.id}:`, error)
              item.planNote = null
              item.hasPlanNote = false
              return item
            }
          })

          await Promise.all(planNotePromises)
        }

        this.calculateCounts()
      } catch (error) {
        console.error('Error in initialize:', error)
      } finally {
        this.isLoading = false
      }
    },

    async getEventTypeList() {
      try {
        const response = await client.getEventTypeQuery()
        this.EventTypeList = response

        if (!this.request.eventTypeId) {
          this.request.eventTypeId = []
          this.EventTypeList.forEach((item: any) => {
            this.request.eventTypeId?.push(item.id)
          })
        } else {
          const found = this.EventTypeList.find((item: any) => item.id === this.request.eventTypeId)
          if (found && found.name === 'นัดหมาย') {
            this.EventTypeIsActivityPlan = true
          } else {
            this.EventTypeIsActivityPlan = false
          }
        }
      } catch (error) {
        console.error('Error fetching event type list:', error)
      }
    },

    calculateCounts(): void {
      const summaryCount = this.Activity.filter((item: any) => item.hasPlanNote === true).length
      const notSummaryCount = this.Activity.filter((item: any) => item.hasPlanNote === false).length

      this.ActivitySummary = summaryCount
      this.ActivityNotSummary = notSummaryCount
    },
    async handleSortemployee(): Promise<void> {
      this.sortDescemployee = !this.sortDescemployee
      this.request.sortDesc = this.sortDescemployee
      this.request.sortColumn = 'Employee'
      await this.initialize()
    },

    async handleSortDate(): Promise<void> {
      this.sortDescDate = !this.sortDescDate
      this.request.sortDesc = this.sortDescDate
      this.request.sortColumn = 'created'
      await this.initialize()
    },
    async onEventTypeChange(): Promise<void> {
      const currentTab = this.activeTab
      this.pageNumber = 1
      // this.currentFilter = 'all'
      await this.initialize()

      if (this.EventTypeIsActivityPlan) {
        this.activeTab = currentTab
      }
    },
    CreateAppointmentPlan(): void {
      this.createActivityPlanDialog = true
    },
    CreateDailyScheduleDialog(): void {
      this.DialogCreateSelect = true
    },
    goCreateActivity() {
      this.DialogCreateSelect = false
      this.DialogCreate = true
    },
    async DuplicateActivity() {
      try {
        if (this.auth.userId) {
          const result = await client.getEmployeeQueryByUserID(this.auth.userId)

          if (!result || !result.id) {
            this.sweetAlertStore.error('ไม่พบข้อมูลพนักงาน')
            return
          }

          const commandCreate = new GetActivityPlanLatestQuery()
          commandCreate.employeeId = result.id

          const response = await client.getActivityPlanQueryLatest(commandCreate)

          if (response) {
            this.mode = 'duplicate'
            this.DialogCreateSelect = false
            setTimeout(() => {
              this.id = response
              this.DialogUpdate = true
            }, 1000)
          } else {
            this.DialogCreateSelect = true
            this.sweetAlertStore.warning('ไม่พบตารางงานล่าสุด กรุณาสร้างใหม่')
            this.mode = 'duplicate'
            this.DialogCreate = true
          }
        }
      } catch (error) {
        console.error('❌ Error in DuplicateActivity:', error)
        this.sweetAlertStore.error('เกิดข้อผิดพลาดในการโหลดข้อมูล')
      }
    },
    async CloseDialogUpdate(value: boolean) {
      this.DialogUpdate = value
    },

    async CloseDialogCreate(value: boolean) {
      this.DialogCreate = value
    },
    CloseDialogSelectTime() {
      this.DialogSelectedTime = false
      this.request.startDate = undefined
      this.request.endDate = undefined
    },
    changeTimestartDate(value: any) {
      this.request.startDate = value
    },
    changeTimeendDate(value: any) {
      this.request.endDate = value
    },
    UpdateAppointmentPlan(id: string | number): void {
      this.isNavigating = true
      this.saveCurrentState()
      localStorage.setItem('selectedAppointmentId', String(id))
      this.$router.push({
        name: 'AppointmentOutcomeDetailview',
        params: { id: id },
        query: { tab: this.activeTab },
      })
    },

    GoEditsummarize(id: string | number): void {
      this.isNavigating = true
      this.saveCurrentState()
      localStorage.setItem('selectedAppointmentId', String(id))
      this.$router.push({
        name: 'UpdateAppointmentOutcomeDetailview',
        params: { id: id },
        query: { tab: this.activeTab },
      })
    },

    Gosummarize(item: any, mode: any): void {
      // Set highlight และเปิด dialog แทนการ navigate
      this.highlightedId = String(item.id)
      this.selectedActivityId = item.id
      if (mode === 'Report') {
        if (item.hasPlanNote === true) {
          this.activitytab = 'EditReport'
        } else {
          this.activitytab = 'CrateReport'
        }
      } else {
        this.activitytab = 'EditActivity'
      }
      // บันทึกใน localStorage สำหรับการกลับมา
      localStorage.setItem('selectedAppointmentId', String(item.id))
      this.manageActivityDialog = true
    },

    GoUpdate(id: any) {
      this.mode = 'edit'
      if (id) {
        // Set highlight ทันที
        this.highlightedId = String(id)

        // บันทึกลง localStorage เฉพาะสำหรับหน้านัดหมาย
        // หน้าตารางงานประจำวันไม่บันทึกลง localStorage เพื่อให้ refresh ครั้งเดียวหาย
        if (this.EventTypeIsActivityPlan === true) {
          this.saveCurrentState()
          localStorage.setItem('selectedAppointmentId', String(id))
        }

        this.DialogUpdate = true
        this.id = id
      }
    },
    async DuplicateActivityBySelect(id: any) {
      try {
        this.mode = 'duplicate'
        this.highlightedId = id
        this.id = id
        this.DialogUpdate = true
      } catch (error) {
        this.sweetAlertStore.error('เกิดข้อผิดพลาดในการทำสำเนารายงาน !')
        console.error(error)
      }
    },
    closeManageActivityDialog(shouldRefresh: boolean = false): void {
      this.manageActivityDialog = false
      this.selectedActivityId = null

      // รีเฟรชข้อมูลเฉพาะเมื่อมีการบันทึกสำเร็จ
      if (shouldRefresh) {
        // เก็บค่า highlightedId ก่อน refresh
        const currentHighlightedId = this.highlightedId

        // Force refresh โดยเพิ่ม timestamp เพื่อป้องกัน cache
        const timestamp = Date.now()

        // เรียก initialize() เฉพาะที่นี่ รอบเดียว
        this.initialize().then(() => {
          // กู้คืน highlightedId หลัง refresh
          if (currentHighlightedId) {
            this.highlightedId = currentHighlightedId

            // Scroll ไปยัง highlighted row
            setTimeout(() => {
              this.scrollToHighlightedRow()
            }, 100)
          }
        })
      }
    },

    closeCreateActivityPlanDialog(): void {
      this.createActivityPlanDialog = false
      // ไม่ refresh ที่นี่ - จะ refresh เฉพาะเมื่อสร้างสำเร็จ
    },

    handleActivityPlanCreated(activityPlanId: string | number): void {
      this.createActivityPlanDialog = false
      // Set highlight และบันทึกใน localStorage
      this.highlightedId = String(activityPlanId)
      localStorage.setItem('selectedAppointmentId', String(activityPlanId))
      // Refresh data และกู้คืน highlight
      this.initialize().then(() => {
        // ตรวจสอบว่า highlightedId ยังคงอยู่
        if (this.highlightedId === String(activityPlanId)) {
          setTimeout(() => {
            // Scroll ไปยัง highlighted row (ใช้ data-row-id ถ้ามี)
            const row = document.querySelector(`tr.selected-row[data-row-id='${activityPlanId}']`)
            if (row) {
              row.scrollIntoView({ behavior: 'smooth', block: 'center' })
              row.classList.add('pulse-animation')
              setTimeout(() => row.classList.remove('pulse-animation'), 2400)
            } else {
              // fallback: scroll by class
              this.scrollToHighlightedRow()
            }
          }, 300)
        }
      })
    },

    // handleNavigateFromCard(action: string, id: string | number): void {
    //   if (action === 'create') {
    //     this.CreateAppointmentPlan()
    //   } else if (action === 'update') {
    //     // แก้ไข: set highlightedId ทันทีเมื่อกดปุ่ม "จัดการงานประจำวัน"
    //     this.highlightedId = String(id)
    //     this.UpdateAppointmentPlan(id)
    //   } else if (action === 'edit-summary') {
    //     this.GoEditsummarize(id)
    //   } else if (action === 'summary') {
    //     this.Gosummarize(id)
    //   }
    // },

    saveCurrentState(): void {
      const currentState = {
        filter: this.currentFilter,
        tab: this.activeTab,
        pageNumber: this.pageNumber,
        pageSize: this.pageSize,
        departmentId: this.request.departmentId,
        employeeId: this.request.employeeId,
        eventTypeId: this.request.eventTypeId,
        highlightedId: this.highlightedId, // บันทึก highlightedId ด้วย
        timestamp: Date.now(),
      }
      localStorage.setItem('appointmentListState', JSON.stringify(currentState))
    },

    loadSavedState(): void {
      const savedState = localStorage.getItem('appointmentListState')
      if (savedState) {
        try {
          const state = JSON.parse(savedState)

          const oneHour = 60 * 60 * 1000
          if (Date.now() - state.timestamp < oneHour) {
            this.currentFilter = state.filter || 'all'
            this.activeTab = state.tab || 'Card'
            this.pageNumber = state.pageNumber || 1
            this.pageSize = state.pageSize || 10
            if (state.eventTypeId) {
              this.request.eventTypeId = state.eventTypeId
            }
            // กู้คืน highlightedId ด้วย
            if (state.highlightedId) {
              this.highlightedId = state.highlightedId
            }
            if (state.departmentId) {
              this.request.departmentId = Array.isArray(state.departmentId) ? state.departmentId[0] : state.departmentId
              this.selectedDepartmentIds = this.request.departmentId ? [this.request.departmentId as any] : []
            }
            if (state.employeeId) {
              this.request.employeeId = state.employeeId
            }
          } else {
            localStorage.removeItem('appointmentListState')
          }
        } catch (error) {
          console.error('Error parsing saved state:', error)
          localStorage.removeItem('appointmentListState')
        }
      }
    },

    handlePageChange(page: number): void {
      this.pageNumber = page
      this.initialize()
    },

    handlePageSizeChange(newPageSize: number): void {
      this.pageSize = newPageSize
      this.pageNumber = 1 // Reset to the first page

      if (newPageSize === -1) {
      }

      this.initialize() // Reload data with the new page size
    },

    scrollToHighlightedRow(): void {
      const highlightedElement = document.querySelector('.selected-row')

      if (highlightedElement) {
        highlightedElement.scrollIntoView({
          behavior: 'smooth',
          block: 'center',
        })

        highlightedElement.classList.add('pulse-animation')
        setTimeout(() => {
          highlightedElement.classList.remove('pulse-animation')
        }, 2400)
      }
    },

    getRowClass(item: any): string {
      return item.id && this.highlightedId && item.id.toString() === this.highlightedId.toString() ? 'selected-row' : ''
    },

    formatDateforshow(startdate: string, enddate: string, allday: boolean): string {
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
    formatDateforshowNotHaveYears(startdate: string, enddate: string, allday: boolean): string {
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

      const startText = `${start.getDate()} ${monthShortThai[start.getMonth() + 1]}`
      const endText = `${end.getDate()} ${monthShortThai[end.getMonth() + 1]}`

      return `${startText} ถึง ${endText}`
    },
    handleRefreshNeeded(): void {
      // เก็บค่า highlightedId ก่อน refresh
      const currentHighlightedId = this.highlightedId

      // รีเฟรชข้อมูล
      this.initialize().then(() => {
        // กู้คืน highlightedId หลัง refresh
        if (currentHighlightedId) {
          this.highlightedId = currentHighlightedId

          // Scroll ไปยัง highlighted row
          setTimeout(() => {
            this.scrollToHighlightedRow()
          }, 100)
        }
      })
    },

    async handleReloadAppointmentPlan(event: any): Promise<void> {
      if (!event.detail?.newAppointmentId) {
        console.warn('Received reloadAppointmentPlan event but no newAppointmentId was provided.')
        return
      }

      const newId = String(event.detail.newAppointmentId)
      const eventTypeToSelect = event.detail.eventTypeToSelect
      this.isLoading = true

      try {
        // --- ขั้นตอนที่ 1: ตั้งค่า State ใหม่ทั้งหมด ---

        // 1.1. เปลี่ยน Filter (v-select) ไปยังประเภทที่ถูกต้องเสมอ
        if (eventTypeToSelect && this.EventTypeList && this.EventTypeList.length > 0) {
          this.request.eventTypeId = []
          this.EventTypeList.forEach((item: any) => {
            this.request.eventTypeId?.push(item.id)
          })
          // const targetEventType = this.EventTypeList.find((item: any) => item.eventTypeCode === eventTypeToSelect)
          // if (targetEventType) {
          //   this.request.eventTypeId = targetEventType.id
          // }
        }

        // 1.2. ตั้งค่า ID ที่จะไฮไลท์
        this.highlightedId = newId

        // 1.3. บังคับให้กลับไปที่หน้า 1 เสมอ
        this.pageNumber = 1

        // --- ขั้นตอนที่ 2: โหลดข้อมูลพื้นฐานของหน้า 1 ---
        await this.initialize()

        // --- ขั้นตอนที่ 3: ตรวจสอบและแทรกข้อมูลใหม่ ---

        // 3.1. ตรวจสอบว่าข้อมูลใหม่ที่เราสร้าง อยู่ในรายการที่เพิ่งโหลดมา (หน้า 1) หรือไม่
        const isAlreadyInList = this.Activity.some(item => item.id === newId)

        if (isAlreadyInList) {
          // ถ้าเจออยู่แล้ว (เช่น ข้อมูลใหม่ถูกเรียงมาอยู่บนสุดพอดี) ก็ไม่ต้องทำอะไรเพิ่ม
        } else {
          // ถ้าไม่เจอ (ซึ่งเป็นกรณีส่วนใหญ่) ให้ดึงข้อมูลใหม่มาแทรกไว้บนสุด
          const newItem = await client.getActivityPlanQueryByID(newId)
          if (newItem) {
            this.Activity.unshift(newItem)
            this.Itemlength += 1 // อัปเดตจำนวนรายการทั้งหมด
            this.calculateCounts()
          } else {
            console.warn(`Could not fetch the new item with ID ${newId} for prepending.`)
          }
        }

        // --- ขั้นตอนที่ 4: เลื่อนหน้าจอ ---
        this.$nextTick(() => {
          setTimeout(() => {
            this.scrollToHighlightedRow()
          }, 300)
        })
      } catch (error) {
        console.error('Error handling reload event:', error)
        // หากเกิดข้อผิดพลาดร้ายแรง ให้ลองโหลดหน้าใหม่อีกครั้งเพื่อความปลอดภัย
        await this.initialize()
      } finally {
        this.isLoading = false
      }
    },
    async deleteActivity(item: any) {
      const confirmDelete = await this.sweetAlertStore.showAlert({
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
          const del = await client.deleteActivityPlan(item.id)
          if (del) {
            this.sweetAlertStore.successDeleted('ลบข้อมูลสำเร็จ')
            await this.initialize() // รีเฟรชข้อมูลหลังลบ
          }
        } catch (error) {
          this.sweetAlertStore.error('เกิดข้อผิดพลาดในการลบข้อมูล !')
          console.error(error)
        }
      }
    },
    async deleteDailySchedule(id: any) {
      const confirmDelete = await this.sweetAlertStore.showAlert({
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
          const del = await client.deleteActivityPlan(id)
          if (del) {
            this.sweetAlertStore.successDeleted('ลบข้อมูลสำเร็จ')
            await this.initialize() // รีเฟรชข้อมูลหลังลบ
          }
        } catch (error) {
          this.sweetAlertStore.error('เกิดข้อผิดพลาดในการลบข้อมูล !')
          console.error(error)
        }
      }
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

@media (max-width: 599px) {
  .create-activity-drawer {
    border-radius: 0 !important;
  }
}

/* สีสำหรับแต่ละ card */
.card-blue {
  border-left-color: #2196f3 !important;
}

.card-green {
  border-left-color: #4caf50 !important;
}

.card-orange {
  border-left-color: #ff9800 !important;
}

/* สไตล์สำหรับ card ที่ถูกเลือก - แยกตามสี */
.card-blue.card-active {
  background-color: #2195f321;
  /* สีฟ้าอ่อน */
}

.card-green.card-active {
  background-color: #4caf4f23;
  /* สีเขียวอ่อน */
}

.card-orange.card-active {
  background-color: #ff990033;
  /* สีส้มอ่อน */
}

.text-black {
  color: #000 !important;
}

/* Class สำหรับตัดข้อความและแสดง ... */
.truncate-cell {
  display: block; /* ทำให้คุณสมบัติข้างล่างทำงานได้ */
  white-space: nowrap; /* บังคับให้ข้อความเป็นบรรทัดเดียว */
  overflow: hidden; /* ซ่อนข้อความส่วนที่เกิน */
  text-overflow: ellipsis; /* แสดง ... แทนข้อความที่ถูกซ่อน */
  max-width: 250px; /* กำหนดความกว้างสูงสุด (ปรับค่าได้ตามความเหมาะสม) */
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
/* Status Indicator */
.status-indicator {
  width: 6px;
  height: 100%;
  border-radius: 0;
  flex-shrink: 0;
  position: absolute;
  left: 0;
  top: 0;
}

/* Card text positioning */
.v-card-text {
  position: relative;
  padding-left: 20px !important;
}

/* Employee Status Dot */
.status-dot {
  width: 14px;
  height: 14px;
  border-radius: 50%;
  border: 1px solid #fff;
  bottom: -2px;
  right: -2px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
}

.status-online {
  background-color: #4caf50;
}

.status-busy {
  background-color: #f44336;
}

.status-away {
  background-color: #ff9800;
}

.selected-row {
  background-color: #e0f2f7 !important; /* สีพื้นหลังสำหรับแถวที่ถูกไฮไลต์ */
  transition: background-color 0.3s ease;
}

.v-data-table .selected-row td {
  background-color: #e0f2f7 !important; /* บังคับสีพื้นหลังของ cell ในแถว */
}

@media (max-width: 767.98px) {
  .create-activity-drawer:not(.v-navigation-drawer--active) {
    transform: translateX(100%) !important;
    visibility: hidden !important;
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
