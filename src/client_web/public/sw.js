// Service Worker สำหรับ Native Push Notifications (Windows & macOS)
const CACHE_NAME = 'crm-native-push-v1'

// Install event
self.addEventListener('install', (event) => {
  console.log('Service Worker installing for native notifications...')
  self.skipWaiting()
})

// Activate event
self.addEventListener('activate', (event) => {
  console.log('Service Worker activating for native notifications...')
  event.waitUntil(self.clients.claim())
})

// ตรวจสอบระบบปฏิบัติการ
function detectPlatform() {
  const userAgent = (self.navigator?.userAgent || '').toLowerCase()
  
  if (userAgent.includes('win')) return 'Windows'
  if (userAgent.includes('mac')) return 'macOS'
  if (userAgent.includes('linux')) return 'Linux'
  if (userAgent.includes('android')) return 'Android'
  if (userAgent.includes('iphone') || userAgent.includes('ipad')) return 'iOS'
  
  return 'Unknown'
}

// กำหนด notification options ตามระบบปฏิบัติการ
function getNotificationOptions(data, platform) {
  const baseOptions = {
    body: data.message || data.body || 'คุณมีการแจ้งเตือนใหม่',
    icon: data.icon || '/favicon.ico',
    badge: data.badge || '/favicon.ico',
    tag: data.tag || `crm-${Date.now()}`,
    data: data.data || {},
    timestamp: data.timestamp || Date.now(),
    requireInteraction: data.type === 'error' || data.type === 'warning',
    silent: false, // เปิดเสียงแจ้งเตือน
    renotify: true
  }

  // ปรับแต่งสำหรับ Windows
  if (platform === 'Windows') {
    return {
      ...baseOptions,
      vibrate: [200, 100, 200], // Windows notification vibration pattern
      actions: [
        {
          action: 'open',
          title: 'เปิด',
          icon: '/favicon.ico'
        },
        {
          action: 'dismiss',
          title: 'ปิด',
          icon: '/favicon.ico'
        }
      ]
    }
  }

  // ปรับแต่งสำหรับ macOS
  if (platform === 'macOS') {
    return {
      ...baseOptions,
      actions: [
        {
          action: 'open',
          title: 'เปิด',
          icon: '/favicon.ico'
        }
      ]
    }
  }

  // สำหรับระบบอื่นๆ
  return baseOptions
}

// Push event - จัดการเมื่อได้รับ push notification
self.addEventListener('push', (event) => {
  console.log('Push event received:', event)
  
  const platform = detectPlatform()
  console.log(`Processing push notification on ${platform}`)
  
  let notificationData = {
    title: 'แจ้งเตือนใหม่',
    message: 'คุณมีข้อความใหม่',
    type: 'info',
    icon: '/favicon.ico',
    badge: '/favicon.ico',
    tag: 'default',
    data: {},
    timestamp: Date.now()
  }

  // ถ้ามีข้อมูลจาก server
  if (event.data) {
    try {
      const data = event.data.json()
      notificationData = {
        title: data.title || notificationData.title,
        message: data.message || data.body || notificationData.message,
        type: data.type || notificationData.type,
        icon: data.icon || notificationData.icon,
        badge: data.badge || notificationData.badge,
        tag: data.tag || notificationData.tag,
        data: data.data || notificationData.data,
        timestamp: data.timestamp || notificationData.timestamp
      }
    } catch (error) {
      console.error('Error parsing push data:', error)
    }
  }

  const options = getNotificationOptions(notificationData, platform)

  event.waitUntil(
    self.registration.showNotification(notificationData.title, options)
      .then(() => {
        console.log(`Native notification shown successfully on ${platform}`)
        
        // Log for debugging
        console.log('Notification details:', {
          title: notificationData.title,
          platform: platform,
          timestamp: new Date().toISOString(),
          options: options
        })
      })
      .catch((error) => {
        console.error(`Failed to show notification on ${platform}:`, error)
      })
  )
})

// Notification click event
self.addEventListener('notificationclick', (event) => {
  console.log('Notification clicked:', event)
  const platform = detectPlatform()
  
  event.notification.close()

  // จัดการ action buttons
  if (event.action === 'dismiss') {
    console.log(`Notification dismissed on ${platform}`)
    return
  }

  // เปิดหน้าแอปพลิเคชัน
  event.waitUntil(
    self.clients.matchAll({
      type: 'window',
      includeUncontrolled: true
    }).then((clientList) => {
      // ถ้ามีหน้าต่างเปิดอยู่แล้ว ให้ focus
      for (let i = 0; i < clientList.length; i++) {
        const client = clientList[i]
        if (client.url.includes(self.location.origin) && 'focus' in client) {
          console.log(`Focusing existing window on ${platform}`)
          return client.focus()
        }
      }
      
      // ถ้าไม่มี ให้เปิดหน้าต่างใหม่
      if (self.clients.openWindow) {
        console.log(`Opening new window on ${platform}`)
        return self.clients.openWindow('/')
      }
    }).catch((error) => {
      console.error(`Error handling notification click on ${platform}:`, error)
    })
  )
})

// Notification close event
self.addEventListener('notificationclose', (event) => {
  const platform = detectPlatform()
  console.log(`Notification closed on ${platform}:`, event.notification.tag)
})

// Background sync สำหรับ offline support
self.addEventListener('sync', (event) => {
  const platform = detectPlatform()
  
  if (event.tag === 'background-sync') {
    console.log(`Background sync triggered on ${platform}`)
    
    event.waitUntil(
      // สามารถเพิ่มการ sync ข้อมูลเมื่อกลับมา online
      fetch('/api/sync/notifications')
        .then(response => {
          if (response.ok) {
            console.log(`Background sync completed on ${platform}`)
          }
        })
        .catch(error => {
          console.log(`Background sync failed on ${platform}:`, error)
        })
    )
  }
})

// Message event - รับข้อความจาก main thread
self.addEventListener('message', (event) => {
  const platform = detectPlatform()
  console.log(`Service Worker received message on ${platform}:`, event.data)
  
  if (event.data && event.data.type === 'SHOW_NOTIFICATION') {
    const { title, body, options } = event.data
    const notificationOptions = getNotificationOptions(
      { ...options, message: body }, 
      platform
    )
    
    self.registration.showNotification(title, notificationOptions)
      .then(() => {
        console.log(`Manual notification shown on ${platform}`)
      })
      .catch((error) => {
        console.error(`Failed to show manual notification on ${platform}:`, error)
      })
  }
  
  // ส่งข้อมูลกลับไปยัง client
  if (event.data && event.data.type === 'GET_PLATFORM') {
    event.ports[0].postMessage({
      type: 'PLATFORM_INFO',
      platform: platform,
      timestamp: Date.now()
    })
  }
})

// Error handling
self.addEventListener('error', (event) => {
  const platform = detectPlatform()
  console.error(`Service Worker error on ${platform}:`, event.error)
})

// Unhandled promise rejection
self.addEventListener('unhandledrejection', (event) => {
  const platform = detectPlatform()
  console.error(`Service Worker unhandled rejection on ${platform}:`, event.reason)
})

// เพิ่ม fetch event สำหรับ caching (optional)
self.addEventListener('fetch', (event) => {
  // สามารถเพิ่ม caching strategy ได้ตามต้องการ
  // ตัวอย่าง: cache API responses หรือ static files
})

console.log(`Service Worker loaded for platform: ${detectPlatform()}`)