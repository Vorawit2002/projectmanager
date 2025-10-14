import { Client, SubcriptionsCommand } from '@/client'
import { BACKEND_API_URL } from '@/constants'
import { useAuthStore } from '@/stores'
import { ref } from 'vue'
import Logo from '@/assets/images/logos/LogoTitle.jpg'

export interface PushNotification {
  id: string
  title: string
  message: string
  type: 'info' | 'success' | 'warning' | 'error' | 'activity' | 'system'
  isRead: boolean
  createdAt: Date
  data?: any
}

class PushNotificationService {
  private notifications = ref<PushNotification[]>([])
  private subscribers: Array<(notification: PushNotification) => void> = []
  private client: Client | null = null
  private authStore: any = null

  constructor() {
    // Ensure notifications array is properly initialized
    if (!this.notifications.value) {
      this.notifications.value = []
    }

    // Lazy initialization to avoid circular dependency issues
    this.initializeService()
  }

  private getClient(): Client {
    if (!this.client) {
      this.client = new Client(BACKEND_API_URL)
    }
    return this.client
  }

  private getAuthStore(): any {
    if (!this.authStore) {
      this.authStore = useAuthStore()
    }
    return this.authStore
  }

  private initializeService() {
    // ตรวจสอบการรองรับ Service Worker
    if ('serviceWorker' in navigator) {
      this.registerServiceWorker()
    } else {
      console.warn('Service Worker is not supported in this browser')
    }

    // ตรวจสอบการรองรับ Push API
    if ('PushManager' in window) {
      this.checkPushSubscription()
    } else {
      console.warn('Push messaging is not supported in this browser')
    }
  }

  private async registerServiceWorker() {
    try {
      const registration = await navigator.serviceWorker.register('/sw.js')
      console.log('Service Worker registered successfully:', registration)
    } catch (error) {
      console.warn('Service Worker registration failed (this is normal if sw.js does not exist):', error)
      // ไม่ throw error เพราะ service worker ไม่จำเป็นสำหรับ basic push notifications
    }
  }

  private async checkPushSubscription() {
    try {
      const registration = await navigator.serviceWorker.ready
      const subscription = await registration.pushManager.getSubscription()

      if (subscription) {
        console.log('User is already subscribed:', subscription)
        // ส่ง subscription ไปยัง server
        this.sendSubscriptionToServer(subscription)
      }
    } catch (error) {
      console.error('Error checking push subscription:', error)
    }
  }

  async requestPermission(): Promise<boolean> {
    if (!('Notification' in window)) {
      console.warn('This browser does not support notifications')
      return false
    }

    if (Notification.permission === 'granted') {
      return true
    }

    if (Notification.permission === 'denied') {
      return false
    }

    const permission = await Notification.requestPermission()
    return permission === 'granted'
  }

  async subscribeToPush(): Promise<boolean> {
    try {
      const hasPermission = await this.requestPermission()
      if (!hasPermission) {
        return false
      }

      // ตรวจสอบว่า browser รองรับ Push API
      if (!('PushManager' in window)) {
        console.warn('Push messaging is not supported')
        return false
      }

      // ตรวจสอบว่า Service Worker พร้อมใช้งาน
      if (!('serviceWorker' in navigator)) {
        console.warn('Service Worker is not supported')
        return false
      }

      const registration = await navigator.serviceWorker.ready

      // VAPID key จาก server
      const vapidKey = import.meta.env.VITE_VAPID_PUBLIC_KEY || 'BFmlPwOqjUUETUkEMmuO7pQAKIE10efqnetnTHFSHZVKuGYTqCuP01L91nbbj4Hgq82gbSpVRmrYR44M4-K6jLk'

      let applicationServerKey: ArrayBuffer | null = null

      // เตรียม applicationServerKey เมื่อมี VAPID key
      if (vapidKey) {
        try {
          applicationServerKey = this.urlBase64ToUint8Array(vapidKey).buffer
          console.log('Using VAPID key for push subscription')
        } catch (error) {
          console.warn('Invalid VAPID key format, subscribing without applicationServerKey:', error)
        }
      }

      const subscribeOptions: PushSubscriptionOptions = {
        userVisibleOnly: true,
        applicationServerKey: applicationServerKey
      }

      const subscription = await registration.pushManager.subscribe(subscribeOptions)

      // ส่ง subscription ไปยัง server
      await this.sendSubscriptionToServer(subscription)

      return true
    } catch (error) {
      console.error('Error subscribing to push notifications:', error)

      return false
    }
  }

  private async sendSubscriptionToServer(subscription: PushSubscription) {
    try {
      // ดึง employeeId จาก auth store
      const authStore = this.getAuthStore()
      const employeeId = authStore.userId

      if (!employeeId) {
        console.warn('No employee ID found, cannot send subscription to server')
        return
      }

      // แปลง subscription เป็น format ที่ backend ต้องการ
      const subscriptionData = subscription.toJSON()

      const command = new SubcriptionsCommand({
        employeeId: employeeId,
        endpoint: subscriptionData.endpoint,
        p256dh: subscriptionData.keys?.p256dh,
        auth: subscriptionData.keys?.auth
      })

      console.log('Sending subscription to server:', command)

      // ส่ง subscription ไปยัง backend API ผ่าน generated client
      const client = this.getClient()
      const success = await client.subcriptions(command)

      if (success) {
        console.log('Subscription sent to server successfully')
      } else {
        throw new Error('Server returned false for subscription')
      }
    } catch (error) {
      console.error('Error sending subscription to server:', error)
    }
  }

  private urlBase64ToUint8Array(base64String: string): Uint8Array {
    try {
      // ตรวจสอบว่า base64String เป็น string ที่ถูกต้อง
      if (!base64String || typeof base64String !== 'string') {
        throw new Error('Invalid base64 string')
      }

      const padding = '='.repeat((4 - base64String.length % 4) % 4)
      const base64 = (base64String + padding)
        .replace(/-/g, '+')
        .replace(/_/g, '/')

      const rawData = window.atob(base64)
      const outputArray = new Uint8Array(rawData.length)

      for (let i = 0; i < rawData.length; ++i) {
        outputArray[i] = rawData.charCodeAt(i)
      }
      return outputArray
    } catch (error) {
      console.error('Error converting base64 to Uint8Array:', error)
      throw new Error(`Invalid VAPID key format: ${error}`)
    }
  }

  addNotification(notification: Omit<PushNotification, 'id' | 'createdAt'>): void {
    const newNotification: PushNotification = {
      ...notification,
      id: Date.now().toString() + Math.random().toString(36).substring(2, 11),
      createdAt: new Date()
    }

    // Ensure notifications array is initialized
    if (!this.notifications.value) {
      this.notifications.value = []
    }

    this.notifications.value.unshift(newNotification)

    // แจ้งผู้สมัครสมาชิก
    this.subscribers.forEach(callback => callback(newNotification))

    // แสดง native notification สำหรับ Windows และ macOS
    this.showNativeNotification(newNotification)
  }

  private async showNativeNotification(notification: PushNotification): Promise<void> {
    if (Notification.permission === 'granted') {
      const platform = this.detectPlatform()
      console.log(`Showing native notification on ${platform}:`, notification.title)

      // กำหนด icon ตามประเภทการแจ้งเตือน
      const getNotificationIcon = () => {
        return Logo
      }

      // ใช้ Service Worker สำหรับ macOS และ Windows เพื่อรองรับ actions
      if ('serviceWorker' in navigator && (platform === 'macOS' || platform === 'Windows')) {
        try {
          const registration = await navigator.serviceWorker.ready

          const options: NotificationOptions = {
            body: notification.message,
            icon: getNotificationIcon(),
            badge: Logo,
            tag: `crm-notification-${notification.id}`,
            data: {
              ...notification.data,
              notificationId: notification.id,
              timestamp: notification.createdAt.getTime(),
              platform: platform,
              type: notification.type
            },
            requireInteraction: notification.type === 'error' || notification.type === 'warning',
            silent: false,
            timestamp: notification.createdAt.getTime(),
            image: notification.type === 'activity' ? Logo : undefined,
            actions: this.getActionsForPlatform(platform) as NotificationAction[]
          }

          await registration.showNotification(notification.title, options)
          console.log(`Service Worker notification shown successfully on ${platform}`)

        } catch (error) {
          console.error(`Failed to show Service Worker notification on ${platform}:`, error)
          // Fallback to basic notification
          this.showBasicNotification(notification, platform)
        }
      } else {
        // ใช้ basic notification สำหรับระบบอื่นๆ หรือเมื่อ Service Worker ไม่พร้อม
        this.showBasicNotification(notification, platform)
      }
    } else {
      console.warn('Notification permission not granted')
    }
  }

  private showBasicNotification(notification: PushNotification, platform: string): void {
    const getNotificationIcon = () => {
      return Logo
    }

    // Basic notification options (ไม่มี actions)
    const options: NotificationOptions = {
      body: notification.message,
      icon: getNotificationIcon(),
      badge: Logo,
      tag: `crm-notification-${notification.id}`,
      data: {
        ...notification.data,
        notificationId: notification.id,
        timestamp: notification.createdAt.getTime(),
        platform: platform
      },
      requireInteraction: notification.type === 'error' || notification.type === 'warning',
      silent: false,
      timestamp: notification.createdAt.getTime(),
      image: notification.type === 'activity' ? Logo : undefined,
      vibrate: platform === 'Windows' ? [200, 100, 200] : undefined
      // ไม่ใส่ actions เพื่อหลีกเลี่ยง error
    }

    try {
      // สร้าง basic notification
      const nativeNotification = new Notification(notification.title, options)

      // จัดการเมื่อคลิกที่การแจ้งเตือน
      nativeNotification.onclick = (event) => {
        event.preventDefault()
        console.log(`Basic notification clicked on ${platform}`)

        // Focus window
        if (window.focus) {
          window.focus()
        }

        // Mark as read
        this.markAsRead(notification.id)
        nativeNotification.close()

        // นำทางไปยังหน้าที่เกี่ยวข้อง (ถ้ามี)
        if (notification.data && notification.data.url) {
          window.location.href = notification.data.url
        }
      }

      // จัดการข้อผิดพลาด
      nativeNotification.onerror = (error) => {
        console.error(`Basic notification error on ${platform}:`, error)
      }

      // จัดการเมื่อแสดงสำเร็จ
      nativeNotification.onshow = () => {
        console.log(`Basic notification shown successfully on ${platform}`)
      }

      // จัดการเมื่อปิดการแจ้งเตือน
      nativeNotification.onclose = () => {
        console.log(`Basic notification closed on ${platform}`)
      }

      // ปิดการแจ้งเตือนอัตโนมัติ (เว้นแต่เป็น error หรือ warning)
      if (notification.type !== 'error' && notification.type !== 'warning') {
        setTimeout(() => {
          nativeNotification.close()
        }, platform === 'macOS' ? 5000 : 8000)
      }

      // Log สำหรับ debug
      console.log(`Basic notification created for ${platform}:`, {
        title: notification.title,
        type: notification.type,
        permission: Notification.permission,
        platform: platform,
        timestamp: new Date().toISOString()
      })

    } catch (error) {
      console.error(`Failed to create basic notification on ${platform}:`, error)
    }
  }

  private detectPlatform(): string {
    const userAgent = navigator.userAgent.toLowerCase()

    if (userAgent.includes('win')) {
      return 'Windows'
    }
    if (userAgent.includes('mac')) {
      return 'macOS'
    }
    if (userAgent.includes('linux')) {
      return 'Linux'
    }
    if (userAgent.includes('android')) {
      return 'Android'
    }
    if (userAgent.includes('iphone') || userAgent.includes('ipad')) {
      return 'iOS'
    }

    return 'Unknown'
  }

  private getActionsForPlatform(platform: string): NotificationAction[] {
    const baseUrl = window.location.origin

    // Windows รองรับ actions ได้ดี
    if (platform === 'Windows') {
      return [
        {
          action: 'view',
          title: 'ดูรายละเอียด',
          icon: Logo
        },
        {
          action: 'dismiss',
          title: 'ปิด',
          icon: Logo
        }
      ]
    }

    // macOS รองรับ actions จำกัด
    if (platform === 'macOS') {
      return [
        {
          action: 'view',
          title: 'เปิด',
          icon: Logo
        }
      ]
    }

    // ระบบอื่นๆ
    return []
  }

  markAsRead(notificationId: string): void {
    if (!this.notifications.value) {
      return
    }
    const notification = this.notifications.value.find(n => n.id === notificationId)
    if (notification) {
      notification.isRead = true
    }
  }

  markAllAsRead(): void {
    if (!this.notifications.value) {
      return
    }
    this.notifications.value.forEach(n => n.isRead = true)
  }

  removeNotification(notificationId: string): void {
    if (!this.notifications.value) {
      return
    }
    const index = this.notifications.value.findIndex(n => n.id === notificationId)
    if (index > -1) {
      this.notifications.value.splice(index, 1)
    }
  }

  clearAllNotifications(): void {
    if (!this.notifications.value) {
      this.notifications.value = []
    } else {
      this.notifications.value = []
    }
  }

  getNotifications() {
    return this.notifications
  }

  getUnreadCount(): number {
    if (!this.notifications.value) {
      return 0
    }
    return this.notifications.value.filter(n => !n.isRead).length
  }

  subscribe(callback: (notification: PushNotification) => void): () => void {
    this.subscribers.push(callback)

    // Return unsubscribe function
    return () => {
      const index = this.subscribers.indexOf(callback)
      if (index > -1) {
        this.subscribers.splice(index, 1)
      }
    }
  }

  // ทดสอบการแจ้งเตือนสำหรับแต่ละระบบ
  testNativeNotification(): void {
    const platform = this.detectPlatform()

    this.addNotification({
      title: `ทดสอบการแจ้งเตือนบน ${platform}`,
      message: `การแจ้งเตือน native สำหรับ ${platform} ทำงานได้แล้ว! 🎉`,
      type: 'success',
      isRead: false,
      data: {
        testMode: true,
        platform: platform,
        timestamp: new Date().toISOString()
      }
    })
  }


}

// Export the class and a factory function for lazy initialization
export { PushNotificationService }

// Lazy singleton factory
export const createPushNotificationService = (() => {
  let instance: PushNotificationService | null = null
  return () => {
    if (!instance) {
      instance = new PushNotificationService()
    }
    return instance
  }
})()

// Default export for convenience
export default createPushNotificationService
