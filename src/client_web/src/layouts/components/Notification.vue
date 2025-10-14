<template>
  <!-- Combined Notification Menu -->
  <v-menu
    v-model="isMenuOpen"
    max-height="600"
    class="overflow-y-auto"
    offset-y
    :close-on-content-click="false"
  >
    <template v-slot:activator="{ props }">
      <v-btn
        variant="text"
        icon
        :ripple="false"
        class="mr-3"
        v-bind="props"
        :class="{
          'animate__animated animate__swing animate__repeat-2': totalActivityNotifications > 0,
          'animate__animated animate__ring animate__repeat-2': hasNewPushNotifications,
        }"
      >
        <v-badge
          color="error"
          :content="totalActivityNotifications"
          :model-value="totalActivityNotifications > 0"
        >
          <v-icon
            icon="ri-notification-line"
            size="large"
          ></v-icon>
        </v-badge>
      </v-btn>
    </template>

    <v-card
      style="border-radius: 15px"
      width="400"
      max-height="700"
    >
      <!-- Header with Tabs -->
      <v-card-title class="d-flex justify-space-between align-center pa-4 bg-primary">
        <span class="text-white font-weight-bold">การแจ้งเตือน</span>
        <!-- <v-btn
          v-if="currentTab === 'push' && pushNotifications.length > 0"
          variant="text"
          size="small"
          class="text-white"
          @click="markAllPushAsRead"
        >
          อ่านทั้งหมด
        </v-btn> -->
      </v-card-title>

      <!-- Tabs -->
      <!-- <v-tabs
        v-model="currentTab"
        bg-color="grey-lighten-4"
        color="primary"
        grow
      >
        <v-tab value="activity">
          กิจกรรม
          <v-badge
            v-if="totalActivityNotifications > 0"
            color="error"
            :content="totalActivityNotifications"
            inline
            class="ml-2"
          ></v-badge>
        </v-tab>
        <v-tab value="push">
          ระบบ
          <v-badge
            v-if="unreadPushCount > 0"
            color="success"
            :content="unreadPushCount"
            inline
            class="ml-2"
          ></v-badge>
        </v-tab>
      </v-tabs> -->

      <!-- Tab Content -->
      <v-card-text class="pa-0">
        <!-- <v-window v-model="currentTab"> -->
        <!-- Activity Notifications Tab -->
        <!-- <v-window-item value="activity"> -->
        <v-list
          v-if="Activity.length > 0"
          lines="three"
          class="pa-2"
        >
          <v-list-item
            v-for="(noti, index) in Activity"
            :key="noti.id"
            rounded="md"
            class="mb-2"
          >
            <v-list-item-title
              class="font-weight-bold mb-1"
              :class="[noti.title === 'แจ้งเตือนการสรุปผล' ? 'text-warning' : 'text-primary']"
            >
              {{ noti.title }}
            </v-list-item-title>
            <v-list-item-subtitle class="font-weight-medium mb-1">
              หน่วยงาน : <span class="text-primary">{{ noti.organizations?.name }}</span>
            </v-list-item-subtitle>
            <v-list-item-subtitle class="font-weight-medium mb-1">
              วัตถุประสงค์ : <span class="text-primary">{{ noti.objective }}</span>
            </v-list-item-subtitle>
            <v-list-item-subtitle class="font-weight-medium mb-2">
              วันที่ <span class="text-primary">{{ formatDateforshow(noti.startDate, noti.endDate) }}</span>
            </v-list-item-subtitle>
            <v-list-item-subtitle>
              <v-btn
                @click="UpdateAppointmentPlan(noti.id, noti.title)"
                variant="outlined"
                size="small"
                color="info"
                class="text-decoration-underline"
              >
                บันทึกรายงาน
              </v-btn>
            </v-list-item-subtitle>
            <v-divider
              v-if="index < Activity.length - 1"
              :thickness="1"
              class="mt-3"
            ></v-divider>
          </v-list-item>
        </v-list>
        <div
          v-else
          class="text-center pa-8"
        >
          <v-icon
            icon="ri-calendar-line"
            size="64"
            class="text-grey-lighten-1 mb-4"
          ></v-icon>
          <p class="text-grey-darken-1">ไม่มีกิจกรรมที่ต้องบันทึกรายงาน</p>
        </div>
        <!-- </v-window-item> -->

        <!-- Push Notifications Tab -->
        <!-- <v-window-item value="push">
            <v-list
              v-if="pushNotifications.length > 0"
              lines="three"
            >
              <template
                v-for="(notification, index) in pushNotifications"
                :key="notification.id"
              >
                <v-list-item
                  :class="{ 'bg-blue-lighten-5': !notification.isRead }"
                  @click="markPushAsRead(notification.id)"
                  class="notification-item"
                >
                  <template v-slot:prepend>
                    <v-avatar
                      :color="getNotificationColor(notification.type)"
                      size="40"
                    >
                      <v-icon
                        :icon="getNotificationIcon(notification.type)"
                        color="white"
                      ></v-icon>
                    </v-avatar>
                  </template>

                  <v-list-item-title class="font-weight-medium">
                    {{ notification.title }}
                  </v-list-item-title>

                  <v-list-item-subtitle class="text-wrap">
                    {{ notification.message }}
                  </v-list-item-subtitle>

                  <v-list-item-subtitle class="mt-1">
                    <v-chip
                      size="x-small"
                      :color="getNotificationColor(notification.type)"
                      variant="outlined"
                    >
                      {{ getTypeLabel(notification.type) }}
                    </v-chip>
                    <span class="text-caption ml-2">{{ formatTime(notification.createdAt) }}</span>
                  </v-list-item-subtitle>

                  <template v-slot:append>
                    <v-btn
                      icon="ri-close-line"
                      variant="text"
                      size="small"
                      @click.stop="removePushNotification(notification.id)"
                    ></v-btn>
                  </template>
                </v-list-item>

                <v-divider
                  v-if="index < pushNotifications.length - 1"
                  :thickness="1"
                ></v-divider>
              </template>
            </v-list>
            <div
              v-else
              class="text-center pa-8"
            >
              <v-icon
                icon="ri-notification-off-line"
                size="64"
                class="text-grey-lighten-1 mb-4"
              ></v-icon>
              <p class="text-grey-darken-1">ไม่มีการแจ้งเตือนใหม่</p>
            </div>
          </v-window-item> -->
        <!-- </v-window> -->
      </v-card-text>

      <!-- Footer -->
      <v-card-actions class="pa-3 bg-grey-lighten-4">
        <template v-if="currentTab === 'push'">
          <!-- ปุ่มขอ permission เมื่อยังไม่ได้รับอนุญาต -->
          <v-btn
            v-if="notificationPermission === 'default'"
            variant="outlined"
            size="small"
            color="success"
            @click="requestPermission"
          >
            <v-icon left>ri-notification-line</v-icon>
            {{ detectBrowser() === 'Edge' ? 'เปิดการแจ้งเตือน (Edge)' : 'เปิดการแจ้งเตือน' }}
          </v-btn>

          <!-- ปุ่มทดสอบเมื่อได้รับอนุญาตแล้ว -->
          <v-btn
            v-else-if="notificationPermission === 'granted'"
            variant="text"
            size="small"
            color="primary"
            @click="testNativeNotification"
          >
            ทดสอบแจ้งเตือน
          </v-btn>

          <!-- ข้อความเมื่อถูกปฏิเสธ -->
          <v-chip
            v-else-if="notificationPermission === 'denied'"
            size="small"
            color="error"
            variant="outlined"
          >
            <v-icon
              left
              size="small"
              >ri-close-circle-line</v-icon
            >
            การแจ้งเตือนถูกปิด
          </v-chip>

          <v-spacer></v-spacer>
          <v-btn
            v-if="pushNotifications.length > 0"
            variant="text"
            size="small"
            @click="clearAllPushNotifications"
          >
            ลบทั้งหมด
          </v-btn>
        </template>
      </v-card-actions>
    </v-card>
  </v-menu>

  <!-- Update Appointment Plan Drawer -->
  <v-navigation-drawer
    v-model="updateAppointmentDialog"
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
      v-if="selectedAppointmentId"
      :id="selectedAppointmentId"
      :activitytab="activitytab"
      @close="closeUpdateAppointmentDialog"
    />
  </v-navigation-drawer>
</template>
<script lang="ts">
import { Client, GetNotificationActivityPlanByEmployeeIdQuery } from '@/client'
import { BACKEND_API_URL } from '@/constants'
import createPushNotificationService, { type PushNotification } from '@/services/PushNotificationService'
import { useAuthStore } from '@/stores'
import ManageActivityDetail from '@/views/AppointmentPlan/ManageActivityDetail.vue'
import moment from 'moment'
import { defineComponent } from 'vue'
import Logo from '@/assets/images/logos/LogoTitle.jpg'

const client = new Client(BACKEND_API_URL)

export default defineComponent({
  name: 'notification',
  components: {
    ManageActivityDetail,
  },
  data() {
    return {
      // Activity notifications data
      Activity: [] as any,
      auth: useAuthStore(),
      totalActivityNotifications: 0 as number,
      updateAppointmentDialog: false as boolean,
      selectedAppointmentId: null as string | number | null,
      selectedTitle: '' as string,
      activitytab: 'CrateReport' as string,

      // Push notifications data
      pushNotificationService: null as any,
      isMenuOpen: false as boolean,
      hasNewPushNotifications: false as boolean,
      notificationPermission: 'default' as NotificationPermission,
      currentTab: 'activity' as string,
      unsubscribeFromService: null as (() => void) | null,
    }
  },
  computed: {
    pushNotifications() {
      return this.pushNotificationService?.getNotifications()?.value || []
    },
    unreadPushCount() {
      return this.pushNotifications?.filter((n: PushNotification) => !n.isRead)?.length || 0
    },
    totalNotificationCount() {
      return this.totalActivityNotifications + this.unreadPushCount
    },
  },
  created() {
    // Initialize push notification service as early as possible
    this.pushNotificationService = createPushNotificationService()
  },
  async mounted() {
    await this.getNotification()
    await this.initializePushNotifications()

    // Add ESC key event listener
    document.addEventListener('keydown', this.handleEscKey)
  },
  beforeUnmount() {
    // Remove ESC key event listener
    document.removeEventListener('keydown', this.handleEscKey)

    // Unsubscribe from push notification service
    if (this.unsubscribeFromService) {
      this.unsubscribeFromService()
    }
  },
  methods: {
    /**
     * @description จัดการการกดปุ่ม ESC เพื่อปิด Drawer ที่เปิดอยู่
     * @param {KeyboardEvent} event - เหตุการณ์ Keyboard
     */
    handleEscKey(event: KeyboardEvent) {
      if (event.key === 'Escape') {
        if (this.updateAppointmentDialog) {
          this.updateAppointmentDialog = false
          event.stopPropagation()
        }
      }
    },

    // Activity notification methods
    async getNotification() {
      try {
        const result = await client.getEmployeeQueryByUserID(this.auth.userId)
        let command = new GetNotificationActivityPlanByEmployeeIdQuery()
        command.employeeId = result.id
        const response = await client.getNotificationActivityPlanQueryByEmployeeId(command)
        this.Activity = response
        this.totalActivityNotifications = response.length
        console.log(response)
      } catch (error) {
        console.log(error)
      }
    },
    formatDateforshow(startdate: any, enddate: any) {
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

      const startText = `${start.getDate()} ${monthShortThai[start.getMonth() + 1]} ${start.getFullYear() + 543}`
      const endText = `${end.getDate()} ${monthShortThai[end.getMonth() + 1]} ${end.getFullYear() + 543}`

      return `${startText} ถึง ${endText}`
    },
    UpdateAppointmentPlan(id: any, title: any) {
      this.selectedAppointmentId = id
      this.selectedTitle = title
      this.updateAppointmentDialog = true
      // ปิดเมนู notification เมื่อเปิด drawer
      this.isMenuOpen = false
    },
    closeUpdateAppointmentDialog() {
      this.updateAppointmentDialog = false
      this.selectedAppointmentId = null
      this.selectedTitle = ''
      this.getNotification()
    },
    onAppointmentUpdated() {
      this.getNotification()
    },

    // Push notification methods
    async initializePushNotifications() {
      const browser = this.detectBrowser()
      console.log('Browser detected:', browser)

      if ('Notification' in window) {
        this.notificationPermission = Notification.permission

        if (Notification.permission === 'default') {
          try {
            console.log(`Requesting permission automatically on ${browser}`)
            let permission: NotificationPermission

            if (browser === 'Edge') {
              try {
                permission = await Notification.requestPermission()
                console.log('Edge: Promise-based permission request successful:', permission)
              } catch (promiseError) {
                console.log('Edge: Promise-based failed, trying callback style')
                permission = await new Promise<NotificationPermission>(resolve => {
                  const result = Notification.requestPermission(perm => {
                    console.log('Edge: Callback permission result:', perm)
                    resolve(perm as NotificationPermission)
                  })
                  if (result && typeof result.then === 'function') {
                    result.then(resolve)
                  }
                })
              }
            } else {
              permission = await Notification.requestPermission()
            }

            this.notificationPermission = permission
            console.log(`Final permission result for ${browser}:`, permission)

            if (permission === 'granted') {
              const success = await this.pushNotificationService.subscribeToPush()
              if (success) {
                console.log(`Notification permission granted and subscribed automatically on ${browser}`)
              }
            } else {
              console.log(`Permission denied or dismissed on ${browser}`)
            }
          } catch (error) {
            console.error(`Error requesting notification permission on ${browser}:`, error)
          }
        } else if (Notification.permission === 'granted') {
          const success = await this.pushNotificationService.subscribeToPush()
          if (success) {
            console.log(`Push notification service initialized successfully on ${browser}`)
            // แสดงการแจ้งเตือนเข้าสู่ระบบแบบ native ทุกครั้งที่ login (ไม่ใช่ refresh)
            const loginSessionKey = `login_session_${this.auth.userId}`
            const hasShownInThisSession = sessionStorage.getItem(loginSessionKey)

            // ถ้ายังไม่ได้แจ้งเตือนใน session นี้ แสดงว่าเป็นการ login ใหม่
            if (!hasShownInThisSession) {
              setTimeout(() => {
                if (Notification.permission === 'granted') {
                  new Notification('เข้าสู่ระบบสำเร็จ', {
                    body: 'ยินดีต้อนรับเข้าสู่ระบบ CRM',
                    icon: Logo,
                    badge: Logo,
                  })
                  // บันทึกว่าได้แจ้งเตือนใน session นี้แล้ว
                  sessionStorage.setItem(loginSessionKey, 'true')
                }
              }, 1000)
            }
          }
        }
      }

      // Subscribe to new notifications from service
      this.unsubscribeFromService = this.pushNotificationService.subscribe((_notification: PushNotification) => {
        this.hasNewPushNotifications = true
      })
    },

    getNotificationColor(type: PushNotification['type']) {
      const colors = {
        info: 'info',
        success: 'success',
        warning: 'warning',
        error: 'error',
        activity: 'primary',
        system: 'secondary',
      }
      return colors[type] || 'info'
    },

    getNotificationIcon(type: PushNotification['type']) {
      const icons = {
        info: 'ri-information-line',
        success: 'ri-check-line',
        warning: 'ri-alert-line',
        error: 'ri-error-warning-line',
        activity: 'ri-calendar-line',
        system: 'ri-settings-line',
      }
      return icons[type] || 'ri-information-line'
    },

    getTypeLabel(type: PushNotification['type']) {
      const labels = {
        info: 'ข้อมูล',
        success: 'สำเร็จ',
        warning: 'คำเตือน',
        error: 'ข้อผิดพลาด',
        activity: 'กิจกรรม',
        system: 'ระบบ',
      }
      return labels[type] || 'ข้อมูล'
    },

    formatTime(date: Date) {
      return moment(date).locale('th').fromNow()
    },

    markPushAsRead(notificationId: string) {
      this.pushNotificationService.markAsRead(notificationId)
    },

    markAllPushAsRead() {
      this.pushNotificationService.markAllAsRead()
      this.hasNewPushNotifications = false
    },

    removePushNotification(notificationId: string) {
      this.pushNotificationService.removeNotification(notificationId)
    },

    clearAllPushNotifications() {
      this.pushNotificationService.clearAllNotifications()
      this.hasNewPushNotifications = false
    },

    addNotification(notification: Omit<PushNotification, 'id' | 'createdAt'>) {
      this.pushNotificationService.addNotification(notification)
    },

    async testNativeNotification() {
      if (Notification.permission === 'default') {
        const success = await this.requestPermission()
        if (!success) {
          alert('กรุณาเปิดการแจ้งเตือนในการตั้งค่าเบราว์เซอร์เพื่อรับการแจ้งเตือน')
          return
        }
      }

      if (Notification.permission === 'denied') {
        alert('การแจ้งเตือนถูกปิดใช้งาน กรุณาเปิดในการตั้งค่าเบราว์เซอร์')
        return
      }

      this.pushNotificationService.testNativeNotification()
    },

    async requestPermission(): Promise<boolean> {
      const browser = this.detectBrowser()
      console.log(`Requesting permission on ${browser}`)

      try {
        if ('Notification' in window) {
          if (browser === 'Edge') {
            const permission = await new Promise(resolve => {
              if (typeof Notification.requestPermission === 'function') {
                const result = Notification.requestPermission(permission => {
                  resolve(permission)
                })
                if (result && typeof result.then === 'function') {
                  result.then(resolve)
                }
              }
            })
            this.notificationPermission = permission as NotificationPermission
          } else {
            const permission = await Notification.requestPermission()
            this.notificationPermission = permission
          }

          if (this.notificationPermission === 'granted') {
            const success = await this.pushNotificationService.subscribeToPush()
            if (success) {
              console.log(`Push notification permission granted and subscribed on ${browser}`)
              return true
            }
          } else {
            console.log(`Push notification permission denied on ${browser}`)
            if (browser === 'Edge') {
              alert(
                `สำหรับ Microsoft Edge: กรุณาไปที่ Settings > Cookies and site permissions > Notifications เพื่อเปิดการแจ้งเตือนสำหรับเว็บไซต์นี้`,
              )
            }
          }
        }
      } catch (error) {
        console.error(`Error requesting notification permission on ${browser}:`, error)
        if (browser === 'Edge') {
          try {
            const permission = await (Notification as any).requestPermission()
            this.notificationPermission = permission
            console.log(`Fallback permission request successful on Edge: ${permission}`)
            if (permission === 'granted') {
              const success = await this.pushNotificationService.subscribeToPush()
              return success
            }
          } catch (fallbackError) {
            console.error('Fallback permission request failed on Edge:', fallbackError)
          }
        }
      }
      return false
    },

    detectBrowser() {
      const userAgent = navigator.userAgent.toLowerCase()
      if (userAgent.includes('edg')) return 'Edge'
      if (userAgent.includes('chrome')) return 'Chrome'
      if (userAgent.includes('firefox')) return 'Firefox'
      if (userAgent.includes('safari') && !userAgent.includes('chrome')) return 'Safari'
      return 'Unknown'
    },

    // Method สำหรับล้างข้อมูล notification เมื่อ logout
    clearLoginNotificationData() {
      const loginSessionKey = `login_session_${this.auth.userId}`
      sessionStorage.removeItem(loginSessionKey)
      // ล้าง session ทั้งหมดที่เกี่ยวข้องกับ login notification
      Object.keys(sessionStorage).forEach(key => {
        if (key.startsWith('login_session_')) {
          sessionStorage.removeItem(key)
        }
      })
    },
  },
})
</script>
<style scoped>
.notification-item {
  cursor: pointer;
  transition: background-color 0.2s ease;
}

.notification-item:hover {
  background-color: rgba(0, 0, 0, 0.04);
}

@keyframes ring {
  0% {
    transform: rotate(0deg);
  }
  10% {
    transform: rotate(10deg);
  }
  20% {
    transform: rotate(-10deg);
  }
  30% {
    transform: rotate(10deg);
  }
  40% {
    transform: rotate(-10deg);
  }
  50% {
    transform: rotate(0deg);
  }
  100% {
    transform: rotate(0deg);
  }
}

.animate__ring {
  animation: ring 0.8s ease-in-out;
}
</style>
