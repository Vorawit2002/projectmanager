  <template>
  <VRow class="fill-width ma-0">
    <VCol
      v-if="filteredItems && filteredItems.length > 0"
      v-for="item in filteredItems"
      cols="12"
      md="4"
    >
      <VCard
        :class="[
          'd-flex flex-column fill-height px-2 py-2',
          String(item.id) === String(highlightedId) ? 'selected-row' : '',
        ]"
      >
        <VCardTitle>
          <VRow class="d-flex align-center">
            <VCol
              cols="auto"
              class="d-flex align-center pa-0"
            >
              <VAvatar
                color="primary"
                variant="tonal"
              >
                <template v-if="item.employees?.imageProfile">
                  <VImg :src="item.employees.imageProfile" />
                </template>
                <template v-else>
                  {{ getInitials(item.employees?.firstName, item.employees?.lastName) }}
                </template>
              </VAvatar>
            </VCol>
            <VCol
              class="mt-1"
              style="padding-left: 8px"
            >
              {{
                [item.employees?.titleName, item.employees?.firstName, item.employees?.lastName]
                  .filter(Boolean)
                  .join(' ') || item.objective
              }}
            </VCol>
            <VSpacer />
            <VCol
              cols="auto"
              class="text-end"
            >
              <v-btn
                v-if="item.eventTypes?.eventTypeCode == '001'"
                icon
                width="15"
                height="15"
                :color="item.haveCost === true || item.haveCost === false ? '#4caf50' : '#ff9800'"
              ></v-btn>
            </VCol>
          </VRow>
        </VCardTitle>

        <VCardText class="pt-1">
          <v-tooltip
            :content-class="'bg-secondary text-on-surface'"
            location="top"
          >
            <template #activator="{ props }">
              <div
                v-bind="props"
                class="text-truncate-multiline"
              >
                {{ item.detail }}
              </div>
              <div
                class="d-flex align-center mt-5 ml-3"
                style="gap: 6px"
                v-if="item.eventTypes?.eventTypeCode == '001'"
              >
                <VIcon
                  size="16"
                  icon="ri-price-tag-3-line"
                  class="text-primary"
                />
                <span class="text-body-2 text-medium-emphasis font-weight-bold">วัตถุประสงค์:</span>

                <span class="text-body-2">{{ item.objective || 'ไม่ระบุ' }}</span>
              </div>
            </template>
            <template #default>
              <div style="white-space: pre-wrap; max-width: 300px">
                {{ item.detail }}
              </div>
            </template>
          </v-tooltip>
        </VCardText>

        <VCardText class="pt-10">
          <!-- ระยะเวลา -->
          <VCol cols="12">
            <div class="d-flex align-center gap-x-3">
              <VAvatar
                color="info-darken-1"
                rounded
                size="40"
                class="elevation-2"
              >
                <VIcon
                  size="24"
                  icon="ri-calendar-schedule-line"
                  class="text-white"
                />
              </VAvatar>

              <div class="d-flex flex-column">
                <div class="text-body-1">ระยะเวลา</div>
                <h6 class="text-h6">
                  {{ formatDateforshow(item.startDate, item.endDate, item.allDay) }}
                </h6>
              </div>
            </div>
          </VCol>

          <!-- หน่วยงาน / ลูกค้า -->
          <VCol cols="12">
            <div class="d-flex align-center gap-x-3">
              <VAvatar
                color="primary-darken-1"
                rounded
                size="40"
                class="elevation-2"
              >
                <VIcon
                  size="24"
                  icon="ri-community-line"
                  class="text-white"
                />
              </VAvatar>

              <div class="d-flex flex-column">
                <div class="text-body-1">
                  {{ item.eventTypes?.eventTypeCode == '001' ? 'หน่วยงาน / ลูกค้า' : 'โครงการ' }}
                </div>
                <h5 class="text-h6">
                  {{
                    item.eventTypes?.eventTypeCode == '001'
                      ? item.organizations?.name
                      : item.projects?.projectName ?? 'ไม่ระบุ'
                  }}
                </h5>
              </div>
            </div>
          </VCol>

          <!-- สถานที่ -->
          <VCol
            cols="12"
            v-if="item.eventTypes?.eventTypeCode == '001'"
          >
            <div class="d-flex align-center gap-x-3">
              <VAvatar
                color="success-darken-1"
                rounded
                size="40"
                class="elevation-2"
              >
                <VIcon
                  size="24"
                  icon="ri-map-pin-2-line"
                  class="text-white"
                />
              </VAvatar>

              <div class="d-flex flex-column">
                <div class="text-body-1">สถานที่</div>
                <h5 class="text-h6">{{ item.location }}</h5>
              </div>
            </div>
          </VCol>
        </VCardText>

        <VCardActions class="mt-auto py-2">
          <div class="d-flex align-center justify-space-between w-100">
            <div>
              <VChip
                class="ml-2"
                size="small"
                :color="getEventTypeChipColor(item)"
                variant="tonal"
              >
                {{ getEventTypeChipLabel(item) }}
              </VChip>
            </div>
            <div class="d-flex align-center">
              <!-- <v-menu v-if="item.eventTypes?.eventTypeCode == '001'">
              <template v-slot:activator="{ props }">
                <VBtn
                  color="primary-darken-0"
                  v-bind="props"
                  ><v-icon
                    size="18"
                    class="mr-1"
                    >ri-eye-line</v-icon
                  >
                  @click="openDetailDialog(item.id)" 
                  รายละเอียด</VBtn
                >
              </template>
            </v-menu> -->

              <v-menu v-if="item.eventTypes?.eventTypeCode == '001'">
                <template v-slot:activator="{ props }">
                  <VBtn
                    color="primary-darken-0"
                    v-bind="props"
                    ><v-icon
                      size="18"
                      class="mr-1"
                      >ri-settings-5-line</v-icon
                    >
                    <!-- @click="openManageActivityDialog(item.id)" -->
                    จัดการนัดหมาย</VBtn
                  >
                </template>
                <v-list>
                  <v-list-item
                    v-for="Daily in getActivityActions(item)"
                    @click="Daily.action(item)"
                  >
                    <v-list-item-title class="text-primary font-weight-medium">{{ Daily.title }}</v-list-item-title>
                  </v-list-item>
                </v-list>
              </v-menu>

              <v-menu v-else>
                <template v-slot:activator="{ props }">
                  <VBtn
                    v-bind="props"
                    color="primary-darken-0"
                    ><v-icon
                      size="18"
                      class="mr-1"
                      >ri-settings-5-line</v-icon
                    >
                    <!--  @click="openDailyScheduleDialog(item.id)" -->
                    จัดการงานรายวัน</VBtn
                  >
                </template>
                <v-list>
                  <v-list-item
                    v-for="Daily in DailyActions"
                    @click="Daily.action(item.id)"
                  >
                    <v-list-item-title class="text-primary font-weight-medium">{{ Daily.title }}</v-list-item-title>
                  </v-list-item>
                </v-list>
              </v-menu>
            </div>
          </div>
        </VCardActions>
      </VCard>
    </VCol>

    <v-col
      v-else
      align="center"
      cols="12"
    >
      <h4 class="text-grey-600">-----------------ไม่มีข้อมูล-----------------</h4>
    </v-col>

    <!-- Pagination Controls for Mobile -->
    <v-col
      cols="12"
      class="text-center mt-4 px-10"
    >
      <!-- สำหรับ pageSize -->
      <v-autocomplete
        :items="[10, 25, 50, 100]"
        v-model="pageSize"
        label="จำนวนรายการต่อหน้า"
        dense
        hide-details
        class="mb-3 mx-auto"
        variant="outlined"
        density="compact"
        color="primary-darken-0"
        style="max-width: 180px"
        @update:modelValue="handlePageSizeChange"
      />

      <!-- สำหรับ pagination -->
      <v-pagination
        v-model="pageNumber"
        :length="Math.ceil(Itemlength / pageSize)"
        @update:modelValue="handlePageChange"
        color="primary"
        total-visible="4"
        size="small"
      />
    </v-col>
  </VRow>

  <!-- Full Page Overlay -->
  <div
    v-if="manageActivityDialog || detailDialog || dailyScheduleDialog || dailyScheduleDetail"
    class="fullpage-overlay"
    @click="closeAllDialogs"
  ></div>

  <v-navigation-drawer
    v-model="manageActivityDialog"
    scrollable
    transition="dialog-righ-transition"
    :width="$vuetify.display.xs ? '100vw' : '550'"
    class="z-indexDialog create-activity-drawer"
    location="right"
    close-on-back
  >
    <div v-if="selectedActivityId">
      <ManageActivityDetail
        :id="String(selectedActivityId)"
        :activitytab="activitytab"
        @close="closeManageActivityDialog"
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
    <div v-if="selectedDetailId">
      <ManageActivityDetailViews
        :id="String(selectedDetailId)"
        @close="closeDetailDialog"
      />
    </div>
  </v-navigation-drawer>

  <v-navigation-drawer
    v-model="dailyScheduleDialog"
    scrollable
    transition="dialog-righ-transition"
    :width="$vuetify.display.xs ? '100vw' : '550'"
    class="z-indexDialog create-activity-drawer"
    location="right"
    close-on-back
  >
    <div v-if="selectedDailyScheduleId">
      <UpdateCustomerDailySchedule
        :id="String(selectedDailyScheduleId)"
        :mode="mode"
        :CloseDialogUpdate="handleCloseDailyScheduleDialog"
      />
    </div>
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
    <div v-if="selectedDailyScheduleId">
      <CustomerDailyScheduleDetailView
        :id="String(selectedDailyScheduleId)"
        :mode="'edit'"
        :CloseDialogUpdate="handleCloseDailyScheduleDetail"
      />
    </div>
  </v-navigation-drawer>
</template>

  <script lang="ts">
import { Client } from '@/client'
import { BACKEND_API_URL } from '@/constants'
import { useSweetAlertStore } from '@/stores'
import { useAuthStore } from '@/stores/auth'
import CustomerDailyScheduleDetailView from '@/views/AppointmentPlan/CustomerDailyScheduleDetailView.vue'
import ManageActivityDetail from '@/views/AppointmentPlan/ManageActivityDetail.vue'
import ManageActivityDetailViews from '@/views/AppointmentPlan/ManageActivityDetailViews.vue'
import UpdateCustomerDailySchedule from '@/views/AppointmentPlan/UpdateCustomerDailySchedule.vue'
import { computed, defineComponent } from 'vue'
const client = new Client(BACKEND_API_URL)
export default defineComponent({
  name: 'CardComponent',
  components: {
    ManageActivityDetail,
    ManageActivityDetailViews,
    UpdateCustomerDailySchedule,
    CustomerDailyScheduleDetailView,
  },
  setup() {
    const authStore = useAuthStore()
    const auth = computed(() => {
      return {
        image: authStore.image ?? '',
      }
    })
    return { auth }
  },
  props: {
    local_data: {
      type: Array,
      required: true,
    },
    pagenumber: {
      type: Number,
      required: true,
    },
    pagesize: {
      type: Number,
      required: true,
    },
    itemlength: {
      type: Number,
      required: true,
    },
    activeTab: {
      type: String,
      default: 'Card',
    },
    highlightedId: {
      type: [String, Number, null],
      default: null,
    },
    filterType: {
      type: String,
      default: 'all', // 'all', 'summary', 'not-summary'
    },
  },
  data() {
    return {
      pageNumber: this.pagenumber,
      pageSize: this.pagesize,
      Itemlength: this.itemlength,
      manageActivityDialog: false,
      selectedActivityId: null as string | number | null,
      detailDialog: false,
      selectedDetailId: null as string | number | null,
      dailyScheduleDialog: false,
      dailyScheduleDetail: false,
      selectedDailyScheduleId: null as string | number | null,
      sweetAlert: useSweetAlertStore(),
      mode: 'edit' as any,
      DailyActions: [
        { title: 'แก้ไขการแจ้งงาน', action: (itemId: any) => this.openDailyScheduleDialog(itemId) },
        { title: 'สำเนาการแจ้งงาน', action: (itemId: any) => this.DuplicateActivity(itemId) },
        { title: 'รายละเอียดการแจ้งงาน', action: (itemId: any) => this.openDailyScheduleDetail(itemId) },
        { title: 'ลบการแจ้งงาน', action: (itemId: any) => this.deleteDailySchedule(itemId) },
      ],
      ActivityActions: [
        {
          title: 'แก้ไขนัดหมาย',
          action: (item: any) => {
            console.log('[CardAppointmentDetailView] Menu clicked: แก้ไขนัดหมาย', item)
            this.openManageActivityDialog(item, 'Activity')
          },
        },
        {
          title: 'รายละเอียดนัดหมาย',
          action: (item: any) => {
            console.log('[CardAppointmentDetailView] Menu clicked: รายละเอียดนัดหมาย', item)
            this.openDetailDialog(item)
          },
        },
        {
          title: 'ลบนัดหมาย',
          action: (item: any) => {
            console.log('[CardAppointmentDetailView] Menu clicked: ลบนัดหมาย', item)
            this.deleteActivity(item)
          },
        },
      ],
      activitytab: 'EditActivity' as any,
    }
  },
  mounted() {
    window.addEventListener('keydown', this.handleEscCloseDrawer)
  },

  beforeUnmount() {
    window.removeEventListener('keydown', this.handleEscCloseDrawer)
  },
  methods: {
    getEventTypeChipColor(item: any): string {
      const code = item?.eventTypes?.eventTypeCode
      // Harmonized palette: '001' (นัดหมาย) => primary, others (แจ้งงานรายวัน) => warning
      return code === '001' ? 'primary' : 'warning'
    },
    getEventTypeChipLabel(item: any): string {
      return item?.eventTypes?.eventTypeCode === '001' ? 'นัดหมาย' : 'แจ้งงานรายวัน'
    },
    handleCloseDailyScheduleDialog(...args: any[]): void {
      console.log('handleCloseDailyScheduleDialog called with args:', args)
      this.closeDailyScheduleDialog()
    },
    handleCloseDailyScheduleDetail() {
      this.dailyScheduleDetail = false
      this.selectedDailyScheduleId = null
      this.$emit('refresh-needed', true)
    },
    handleEscCloseDrawer(e: KeyboardEvent) {
      if (e.key === 'Escape' || e.key === 'Esc') {
        this.closeAllDialogs()
      }
    },
    closeAllDialogs() {
      if (this.manageActivityDialog) {
        this.manageActivityDialog = false
      }
      if (this.detailDialog) {
        this.detailDialog = false
      }
      if (this.dailyScheduleDialog) {
        this.dailyScheduleDialog = false
      }
      if (this.dailyScheduleDetail) {
        this.handleCloseDailyScheduleDetail()
      }
    },
    getInitials(firstName: string = '', lastName: string = ''): string {
      const firstInitial = firstName.trim().charAt(0) || ''
      const lastInitial = lastName.trim().charAt(0) || ''
      return (firstInitial + lastInitial).toUpperCase()
    },
    async handlePageChange(page: number) {
      this.pageNumber = page
      this.$emit('pagenumber', page)
    },

    async handlePageSizeChange(size: number) {
      this.pageSize = size
      this.$emit('pagesize', size)
    },

    openManageActivityDialog(item: any, mode: any): void {
      // Set highlight และ emit ไปให้ parent
      this.$emit('update:highlightedId', String(item.id))

      // บันทึกใน localStorage สำหรับการกลับมา
      localStorage.setItem('selectedAppointmentId', String(item.id))

      this.selectedActivityId = item.id
      if (mode === 'Report') {
        console.log('[CardAppointmentDetailView] Open manage dialog mode=Report, item:', item)
        if (item.hasPlanNote === true) {
          this.activitytab = 'EditReport'
        } else {
          this.activitytab = 'CrateReport'
        }
      } else {
        console.log('[CardAppointmentDetailView] Open manage dialog mode=Activity, item:', item)
        this.activitytab = 'EditActivity'
      }
      console.log('[CardAppointmentDetailView] Passing props to ManageActivityDetail:', {
        id: String(this.selectedActivityId),
        activitytab: this.activitytab,
      })
      this.manageActivityDialog = true
    },

    closeManageActivityDialog(shouldRefresh: boolean = false): void {
      console.log('🔧 CardAppointmentDetailView: closeManageActivityDialog called with shouldRefresh:', shouldRefresh)
      this.manageActivityDialog = false
      this.selectedActivityId = null

      // ส่งต่อ shouldRefresh ไปให้ parent
      if (shouldRefresh) {
        console.log('🔄 CardAppointmentDetailView: Emitting refresh request to parent')
        this.$emit('refresh-needed')
      }
    },

    openDetailDialog(item: any): void {
      // Set highlight และ emit ไปให้ parent
      this.$emit('update:highlightedId', String(item.id))

      // บันทึกใน localStorage สำหรับการกลับมา
      localStorage.setItem('selectedAppointmentId', String(item.id))

      this.selectedDetailId = item.id
      this.detailDialog = true
    },

    closeDetailDialog(): void {
      console.log('🔧 CardAppointmentDetailView: closeDetailDialog called')
      this.detailDialog = false
      this.selectedDetailId = null
      // ไม่ emit reload เพื่อป้องกันการ refresh ซ้ำ - ให้ parent จัดการเอง
    },

    openDailyScheduleDialog(id: string | number): void {
      console.log('🔧 CardAppointmentDetailView: openDailyScheduleDialog called with id:', id)
      this.mode = 'edit'
      // Set highlight และ emit ไปให้ parent
      this.$emit('update:highlightedId', String(id))

      // บันทึกใน localStorage สำหรับการกลับมา
      localStorage.setItem('selectedAppointmentId', String(id))

      this.selectedDailyScheduleId = id
      this.dailyScheduleDialog = true
    },
    openDailyScheduleDetail(id: any) {
      this.mode = 'edit'
      // Set highlight และ emit ไปให้ parent
      this.$emit('update:highlightedId', String(id))

      // บันทึกใน localStorage สำหรับการกลับมา
      localStorage.setItem('selectedAppointmentId', String(id))

      this.selectedDailyScheduleId = id
      this.dailyScheduleDetail = true
    },
    closeDailyScheduleDialog(): void {
      console.log('🔧 CardAppointmentDetailView: closeDailyScheduleDialog called')
      this.dailyScheduleDialog = false
      this.selectedDailyScheduleId = null
      this.$emit('refresh-needed', true) // Emit refresh event
      // ไม่ emit reload เพื่อป้องกันการ refresh ซ้ำ - ให้ parent จัดการเอง
    },

    Gosummarize(id: string | number): void {
      // Emit event เพื่อเปลี่ยน highlightedId
      this.$emit('update:highlightedId', id)
      // Emit navigate event ไปให้ parent component
      this.$emit('navigate', 'summary', id)
    },

    GoUpdate(id: string | number): void {
      // Set highlight ก่อน emit
      this.$emit('update:highlightedId', String(id))

      // รอให้ highlight update แล้วค่อย emit navigate
      this.$nextTick(() => {
        this.$emit('navigate', 'update', id)
      })
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
    async deleteActivity(item: any) {
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
          const del = await client.deleteActivityPlan(item.id)
          if (del) {
            this.sweetAlert.successDeleted('ลบข้อมูลสำเร็จ')
            this.$emit('refresh-needed', true)
          }
        } catch (error) {
          this.sweetAlert.error('เกิดข้อผิดพลาดในการลบข้อมูล !')
          console.error(error)
        }
      }
    },
    async deleteDailySchedule(id: any) {
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
          const del = await client.deleteActivityPlan(id)
          if (del) {
            this.sweetAlert.successDeleted('ลบข้อมูลสำเร็จ')
            this.$emit('refresh-needed', true)
          }
        } catch (error) {
          this.sweetAlert.error('เกิดข้อผิดพลาดในการลบข้อมูล !')
          console.error(error)
        }
      }
    },
    async DuplicateActivity(id: any) {
      try {
        this.mode = 'duplicate'
        this.selectedDailyScheduleId = id
        this.dailyScheduleDialog = true
      } catch (error) {
        this.sweetAlert.error('เกิดข้อผิดพลาดในการทำสำเนารายงาน !')
        console.error(error)
      }
    },

    getActivityActions(item: any) {
      const baseActions = [
        {
          title: 'แก้ไขนัดหมาย',
          action: (item: any) => {
            console.log('[CardAppointmentDetailView] Menu clicked: แก้ไขนัดหมาย', item)
            this.openManageActivityDialog(item, 'Activity')
          },
        },
        {
          title: 'รายละเอียดนัดหมาย',
          action: (item: any) => {
            console.log('[CardAppointmentDetailView] Menu clicked: รายละเอียดนัดหมาย', item)
            this.openDetailDialog(item)
          },
        },
      ]

      // เพิ่มเมนูสรุปผลนัดหมาย โดยเช็คว่ามีการสรุปแล้วหรือยัง
      const summaryAction = {
        title: item.hasPlanNote === true ? 'แก้ไขสรุปผลนัดหมาย' : 'สรุปผลนัดหมาย',
        action: (item: any) => {
          const actionName = item.hasPlanNote === true ? 'แก้ไขสรุปผลนัดหมาย' : 'สรุปผลนัดหมาย'
          console.log(`[CardAppointmentDetailView] Menu clicked: ${actionName}`, item)
          this.openManageActivityDialog(item, 'Report')
        },
      }

      const deleteAction = {
        title: 'ลบนัดหมาย',
        action: (item: any) => {
          console.log('[CardAppointmentDetailView] Menu clicked: ลบนัดหมาย', item)
          this.deleteActivity(item)
        },
      }

      return [...baseActions, summaryAction, deleteAction]
    },
  },

  watch: {
    local_data: {
      immediate: true,
      handler(newVal: any[]) {
        // ไม่ต้องเซ็ต Activity แล้ว ให้ใช้ prop local_data ตรง ๆ
      },
    },
    itemlength: {
      immediate: true,
      handler(newVal: number) {
        this.Itemlength = this.itemlength
      },
    },
    pagenumber: {
      immediate: true,
      handler(newVal: number) {
        this.pageNumber = newVal
      },
    },
    pagesize: {
      immediate: true,
      handler(newVal: number) {
        this.pageSize = newVal
      },
    },
    highlightedId(newVal) {
      // @ts-ignore
      console.log('[CardAppointmentDetailView] highlightedId changed:', newVal)
      // @ts-ignore
      const items = this.filteredItems || []
      console.log(
        '[CardAppointmentDetailView] filteredItems:',
        items.map((i: any) => i.id),
      )
      items.forEach((i: any) => {
        console.log(
          '[CardAppointmentDetailView] compare',
          'item.id:',
          i.id,
          'highlightedId:',
          newVal,
          '==',
          String(i.id) === String(newVal),
        )
      })
    },
  },

  computed: {
    filteredItems(): any[] {
      if (!Array.isArray(this.local_data)) return []

      if (this.filterType === 'summary') {
        return this.local_data.filter((x: any) => x.planNote || x.hasPlanNote)
      } else if (this.filterType === 'not-summary') {
        return this.local_data.filter((x: any) => !x.planNote && !x.hasPlanNote)
      }
      return this.local_data
    },
  },
})
</script>

  <style scoped>
/* Ensure full width */
.v-row {
  width: 100%;
  max-width: 100%;
}

/* Additional safety for drawer state */
.z-indexDialog {
  z-index: 9999;
}

@media (max-width: 599px) {
  .create-activity-drawer {
    border-radius: 0 !important;
  }
}

.text-truncate-multiline {
  overflow: hidden;
  display: -webkit-box;
  -webkit-line-clamp: 2; /* แสดง 2 บรรทัด */
  line-clamp: 2; /* Standard property for compatibility */
  -webkit-box-orient: vertical;
  white-space: normal;
}

.selected-row,
:deep(.selected-row),
.v-card.selected-row {
  background-color: rgba(var(--v-theme-info), 0.1) !important;
  border: 1px solid #9c9d9e !important;
  box-shadow: 0 0 8px #3d77b038 !important;
  transition: background-color 0.3s, border 0.3s, box-shadow 0.3s;
}

.v-card.selected-row {
  animation: pulse-border 2s infinite;
}

@keyframes pulse-border {
  0% {
    border-color: #9c9d9e;
    box-shadow: 0 0 8px #136dc833;
  }
  50% {
    border-color: #2196f3;
    box-shadow: 0 0 12px #2196f366;
  }
  100% {
    border-color: #9c9d9e;
    box-shadow: 0 0 8px #136dc833;
  }
}
/* ให้ scroll-wrapper มี border-radius เหมือน dialog */
.dialog-scroll-wrapper {
  max-height: 90vh;
  overflow-y: auto;
  overflow-x: hidden;
  -webkit-overflow-scrolling: touch;
  overscroll-behavior: contain;
  padding: 0;
  border-radius: 28px;
  background: transparent;
}

@media (max-width: 599px) {
  .dialog-scroll-wrapper {
    border-radius: 20px;
  }
}
@media (min-width: 600px) and (max-width: 959px) {
  .dialog-scroll-wrapper {
    border-radius: 24px;
  }
}
@media (min-width: 960px) and (max-width: 1263px) {
  .dialog-scroll-wrapper {
    border-radius: 24px;
  }
}
@media (min-width: 1264px) {
  .dialog-scroll-wrapper {
    border-radius: 28px;
  }
}
@media (max-width: 599px) {
  .custom-dialog-card {
    border-radius: 20px !important;
    max-width: 100vw !important;
    margin: 0 !important;
  }
  .dialog-scroll-wrapper {
    max-height: calc(100vh - 16px);
  }
}
@media (min-width: 600px) and (max-width: 959px) {
  .custom-dialog-card {
    border-radius: 24px !important;
    max-width: calc(100vw - 32px) !important;
    margin: 16px !important;
  }
  .dialog-scroll-wrapper {
    max-height: 92vh;
  }
}
@media (min-width: 960px) and (max-width: 1263px) {
  .custom-dialog-card {
    border-radius: 24px !important;
    max-width: 800px !important;
    margin: 20px auto !important;
  }
  .dialog-scroll-wrapper {
    max-height: 90vh;
  }
}
@media (min-width: 1264px) {
  .custom-dialog-card {
    border-radius: 28px !important;
    max-width: 900px !important;
    margin: 24px auto !important;
  }
  .dialog-scroll-wrapper {
    max-height: 90vh;
  }
}
</style>
