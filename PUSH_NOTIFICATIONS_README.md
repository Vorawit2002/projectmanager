# Push Notifications Setup Guide 🔔

คู่มือการติดตั้งและเชื่อมต่อ Native Push Notifications สำหรับระบบ CRM ที่รองรับ Windows Action Center และ macOS Notification Center

## 🎯 คุณสมบัติหลัก

- ✅ **Windows Action Center Integration** - แสดงการแจ้งเตือนใน Windows Action Center
- ✅ **macOS Notification Center Integration** - แสดงการแจ้งเตือนใน macOS Notification Center  
- ✅ **Service Worker Support** - รองรับ action buttons และ persistent notifications
- ✅ **Platform Detection** - ตรวจจับระบบปฏิบัติการอัตโนมัติ
- ✅ **Fallback System** - basic notifications เมื่อ Service Worker ไม่พร้อม
- ✅ **Native Sound Support** - ใช้เสียงแจ้งเตือนของระบบปฏิบัติการ

## 📋 สารบัญ

1. [การติดตั้ง Frontend](#การติดตั้ง-frontend)
2. [การเชื่อมต่อ Backend API](#การเชื่อมต่อ-backend-api)
3. [การใช้งาน](#การใช้งาน)
4. [การกำหนดค่า](#การกำหนดค่า)
5. [การทดสอบ](#การทดสอบ)
6. [การแก้ไขปัญหา](#การแก้ไขปัญหา)

## 🚀 การติดตั้ง Frontend

### 1. Component ที่ถูกเพิ่มแล้ว

```
src/
├── components/
│   └── PushNotification.vue          # Main notification component
├── services/
│   └── PushNotificationService.ts    # Service for managing notifications
└── public/
    └── sw.js                         # Service Worker for push notifications
```

### 2. การเพิ่ม Component ใน Layout

Component ได้ถูกเพิ่มใน `DefaultLayoutWithVerticalNav.vue` แล้ว:

```vue
<script setup>
import PushNotification from '@/components/PushNotification.vue'
</script>

<template>
  <!-- ไอคอนแจ้งเตือน native ใน navbar -->
  <PushNotification />
  <!-- ไอคอนแจ้งเตือนเดิมของระบบ -->
  <notification />
</template>
```

### 3. Native Notification Architecture

```
┌─────────────────────────────────────┐
│           Browser Layer             │
├─────────────────────────────────────┤
│    PushNotification.vue Component   │
│                 │                   │
│                 ▼                   │
│   PushNotificationService.ts        │
│                 │                   │
│                 ▼                   │
│     Platform Detection              │
│    ┌─────────────┬─────────────┐    │
│    │   macOS     │   Windows   │    │
│    │             │             │    │
│    ▼             ▼             ▼    │
│ Service Worker   Basic API     │    │
│ (with actions)  (fallback)     │    │
├─────────────────────────────────────┤
│            Service Worker           │
│              (sw.js)                │
├─────────────────────────────────────┤
│         Operating System            │
│  ┌─────────────┬─────────────────┐  │
│  │   macOS     │    Windows      │  │
│  │Notification │  Action Center  │  │
│  │   Center    │                 │  │
│  └─────────────┴─────────────────┘  │
└─────────────────────────────────────┘
```

### 4. Dependencies ที่ต้องการ

```json
{
  "dependencies": {
    "moment": "^2.30.1",
    "@vueuse/core": "^10.11.1"
  }
}
```

### 5. Platform-Specific Features

#### 🖥️ **Windows Features**
- แสดงใน **Windows Action Center**
- รองรับ **Action Buttons**: "ดูรายละเอียด", "ปิด"
- มี **Vibration Pattern**: [200, 100, 200]
- เสียงแจ้งเตือนของ Windows

#### 🍎 **macOS Features**  
- แสดงใน **macOS Notification Center**
- รองรับ **Action Button**: "เปิด"
- Banner notification ที่ด้านบนหน้าจอ
- เสียงแจ้งเตือนของ macOS
- Auto-dismiss หลัง 5 วินาที

## 🔗 การเชื่อมต่อ Backend API

### 1. สร้าง Push Notification Endpoints

#### A. DTO สำหรับ Push Notifications

```csharp
// src/Application/PushNotifications/Commands/SendNotification/SendPushNotificationCommand.cs
public class SendPushNotificationCommand : IRequest<bool>
{
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = "info"; // info, success, warning, error, activity, system
    public Dictionary<string, object>? Data { get; set; }
}
```

#### B. Subscription Management

```csharp
// src/Application/PushNotifications/Commands/SubscribeNotification/SubscribePushNotificationCommand.cs
public class SubscribePushNotificationCommand : IRequest<bool>
{
    public string UserId { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public string P256dh { get; set; } = string.Empty;
    public string Auth { get; set; } = string.Empty;
}
```

### 2. Entity สำหรับเก็บ Subscription

```csharp
// src/Domain/Entities/PushSubscription.cs
public class PushSubscription : BaseAuditableEntity
{
    public string UserId { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public string P256dh { get; set; } = string.Empty;
    public string Auth { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime LastUsed { get; set; }
}
```

### 3. Web API Endpoints

```csharp
// src/Web/Endpoints/PushNotificationEndpoint.cs
public class PushNotificationEndpoint : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(Subscribe, "subscribe")
            .MapPost(Unsubscribe, "unsubscribe") 
            .MapPost(Send, "send")
            .MapGet(GetSubscriptions, "subscriptions/{userId}");
    }

    public async Task<bool> Subscribe(ISender sender, SubscribePushNotificationCommand command)
    {
        return await sender.Send(command);
    }

    public async Task<bool> Send(ISender sender, SendPushNotificationCommand command)
    {
        return await sender.Send(command);
    }
}
```

### 4. Push Notification Service Implementation

```csharp
// src/Infrastructure/Services/WebPushService.cs
public interface IWebPushService
{
    Task<bool> SendNotificationAsync(string subscription, object payload);
    Task<bool> SendToUserAsync(string userId, string title, string message, string type = "info");
}

public class WebPushService : IWebPushService
{
    private readonly WebPushClient _webPushClient;
    private readonly IApplicationDbContext _context;

    public WebPushService(IApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        
        var vapidDetails = new VapidDetails(
            subject: configuration["PushNotifications:Subject"] ?? "mailto:admin@yoursite.com",
            publicKey: configuration["PushNotifications:PublicKey"] ?? "",
            privateKey: configuration["PushNotifications:PrivateKey"] ?? ""
        );
        
        _webPushClient = new WebPushClient();
        _webPushClient.SetVapidDetails(vapidDetails);
    }

    public async Task<bool> SendToUserAsync(string userId, string title, string message, string type = "info")
    {
        var subscriptions = await _context.PushSubscriptions
            .Where(s => s.UserId == userId && s.IsActive)
            .ToListAsync();

        var payload = new
        {
            title,
            message,
            type,
            timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };

        var tasks = subscriptions.Select(async subscription =>
        {
            try
            {
                var pushSubscription = new WebPush.PushSubscription(
                    subscription.Endpoint,
                    subscription.P256dh,
                    subscription.Auth
                );

                await _webPushClient.SendNotificationAsync(
                    pushSubscription,
                    JsonSerializer.Serialize(payload)
                );

                subscription.LastUsed = DateTime.UtcNow;
                return true;
            }
            catch (Exception ex)
            {
                // Log error and mark subscription as inactive if needed
                Console.WriteLine($"Failed to send push notification: {ex.Message}");
                return false;
            }
        });

        var results = await Task.WhenAll(tasks);
        await _context.SaveChangesAsync();
        
        return results.Any(r => r);
    }
}
```

### 5. การกำหนดค่าใน appsettings.json

```json
{
  "PushNotifications": {
    "Subject": "mailto:admin@yourcrm.com",
    "PublicKey": "YOUR_VAPID_PUBLIC_KEY",
    "PrivateKey": "YOUR_VAPID_PRIVATE_KEY"
  }
}
```

### 6. Package ที่ต้องติดตั้งใน Backend

```xml
<!-- src/Infrastructure/Infrastructure.csproj -->
<PackageReference Include="WebPush" Version="1.0.11" />
```

## ⚙️ การกำหนดค่า Frontend

### 1. อัพเดท PushNotificationService.ts

```typescript
// src/services/PushNotificationService.ts
const BACKEND_API_URL = 'http://localhost:5000/api'; // เปลี่ยนเป็น URL ของ backend

private async sendSubscriptionToServer(subscription: PushSubscription) {
  try {
    const response = await fetch(`${BACKEND_API_URL}/push-notifications/subscribe`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${getAuthToken()}` // เพิ่ม auth token
      },
      body: JSON.stringify({
        userId: getCurrentUserId(), // เพิ่ม function นี้
        endpoint: subscription.endpoint,
        p256dh: subscription.getKey('p256dh') ? btoa(String.fromCharCode(...new Uint8Array(subscription.getKey('p256dh')!))) : '',
        auth: subscription.getKey('auth') ? btoa(String.fromCharCode(...new Uint8Array(subscription.getKey('auth')!))) : ''
      })
    });

    if (!response.ok) {
      throw new Error('Failed to send subscription to server');
    }
  } catch (error) {
    console.error('Error sending subscription to server:', error);
  }
}
```

### 2. สร้าง VAPID Keys

```bash
# ติดตั้ง web-push CLI
npm install -g web-push

# สร้าง VAPID keys
web-push generate-vapid-keys

# Output:
# Public Key: BEl62iUYgUivxIkv69yViEuiBIa40HI8YlOU...
# Private Key: bdSiGdspy8ut0S98acLBd5...
```

### 3. อัพเดท VAPID Key ใน Service

```typescript
// src/services/PushNotificationService.ts
const vapidKey = 'BEl62iUYgUivxIkv69yViEuiBIa40HI8YlOU...' // ใส่ public key จริง
```

## 🎯 การใช้งาน

### 1. ส่งการแจ้งเตือนจาก Backend

```csharp
// ตัวอย่างการใช้งานใน Controller หรือ Service
public async Task NotifyUser(string userId, string message)
{
    var command = new SendPushNotificationCommand
    {
        UserId = userId,
        Title = "แจ้งเตือนใหม่",
        Message = message,
        Type = "info" // info, success, warning, error, activity, system
    };
    
    await _sender.Send(command);
}
```

### 2. ส่งการแจ้งเตือนจาก Frontend

```typescript
// ใน Vue component
import pushNotificationService from '@/services/PushNotificationService'

// ส่งการแจ้งเตือนแบบ local
pushNotificationService.addNotification({
  title: 'ข้อความใหม่',
  message: 'คุณมีข้อความใหม่',
  type: 'info',
  isRead: false,
  data: {
    url: '/dashboard', // optional: URL เมื่อคลิกที่การแจ้งเตือน
    userId: 'user123'
  }
})

// ทดสอบการแจ้งเตือน native
pushNotificationService.testNativeNotification()
```

### 3. ผสานกับระบบ Hangfire (สำหรับ scheduled notifications)

```csharp
// src/Infrastructure/HangfirePushNotificationSender.cs
public class HangfirePushNotificationSender
{
    private readonly IWebPushService _webPushService;

    [AutomaticRetry(Attempts = 3)]
    public async Task SendScheduledNotification(string userId, string title, string message)
    {
        await _webPushService.SendToUserAsync(userId, title, message, "activity");
    }
}
```

### 4. การใช้งาน Component

```vue
<template>
  <!-- ไอคอนกระดิ่งใน navbar -->
  <PushNotification />
</template>

<script>
import { pushNotificationService } from '@/services/PushNotificationService'

export default {
  methods: {
    async sendTestNotification() {
      // ทดสอบการแจ้งเตือน
      pushNotificationService.testNativeNotification()
    },
    
    async requestPermission() {
      // ขอ permission manually
      const success = await pushNotificationService.subscribeToPush()
      console.log('Permission granted:', success)
    }
  }
}
</script>
```

## 🧪 การทดสอบ

### 1. การทดสอบใน UI

#### 📱 **ทดสอบผ่าน Web Interface**
1. **เปิดเว็บไซต์** และรอ permission dialog
2. **คลิก "Allow"** เมื่อ browser ถาม permission
3. **คลิกไอคอนกระดิ่ง** ใน navbar (ถัดจาก notification เดิม)
4. **คลิก "ทดสอบแจ้งเตือน"** ในส่วนล่างของ dropdown
5. **ดูการแจ้งเตือน** ใน:
   - **macOS**: Notification Center (มุมขวาบน)
   - **Windows**: Action Center (มุมขวาล่าง)

#### 🔔 **สิ่งที่ควรเห็น**
- การแจ้งเตือนแสดงในระบบปฏิบัติการ (ไม่ใช่ใน browser)
- เสียงแจ้งเตือนของระบบ
- Action buttons (เปิด/ปิด)
- เมื่อคลิกจะกลับมาที่เว็บไซต์

### 2. การทดสอบผ่าน Console

```javascript
// ทดสอบพื้นฐาน
pushNotificationService.testNativeNotification()

// ทดสอบแจ้งเตือนแต่ละประเภท
pushNotificationService.addNotification({
  title: 'ทดสอบ Success',
  message: 'การแจ้งเตือนประเภท success',
  type: 'success',
  isRead: false
})

pushNotificationService.addNotification({
  title: 'ทดสอบ Warning', 
  message: 'การแจ้งเตือนประเภท warning',
  type: 'warning',
  isRead: false
})

// ตรวจสอบสถานะระบบ
console.log('Platform:', pushNotificationService.detectPlatform())
console.log('Permission:', Notification.permission)
```

### 3. การทดสอบ Service Worker

```javascript
// ตรวจสอบ Service Worker status
navigator.serviceWorker.getRegistrations().then(registrations => {
  console.log('Service Workers:', registrations)
})

// ทดสอบส่งข้อความไป Service Worker
navigator.serviceWorker.ready.then(registration => {
  registration.active.postMessage({
    type: 'SHOW_NOTIFICATION',
    title: 'ทดสอบจาก Console',
    body: 'ข้อความทดสอบผ่าน Service Worker',
    options: { type: 'info' }
  })
})
```

## 🐛 การแก้ไขปัญหา

### 1. ปัญหาที่พบบ่อย

#### ❌ **Permission ไม่ขึ้น**
```javascript
// ตรวจสอบสถานะ permission
console.log('Notification permission:', Notification.permission)

// ขอ permission ใหม่
Notification.requestPermission().then(permission => {
  console.log('Permission result:', permission)
})

// รีเซ็ต permission (ใน browser settings)
// Chrome: Settings > Privacy and security > Site Settings > Notifications
// Safari: Preferences > Websites > Notifications
```

#### ❌ **Service Worker ไม่ทำงาน**
```javascript
// ตรวจสอบ Service Worker registration
navigator.serviceWorker.getRegistrations().then(registrations => {
  console.log('Registered service workers:', registrations)
  if (registrations.length === 0) {
    console.error('No service worker registered!')
  }
})

// ลองลงทะเบียน Service Worker ใหม่
navigator.serviceWorker.register('/sw.js').then(registration => {
  console.log('Service Worker registered:', registration)
})
```

#### ❌ **การแจ้งเตือนไม่แสดงใน macOS**
```javascript
// ตรวจสอบ macOS notification settings
// System Preferences > Notifications & Focus > [Browser] > Allow Notifications

// ลองใช้ basic notification
new Notification('ทดสอบ Basic', {
  body: 'การแจ้งเตือนพื้นฐาน',
  icon: '/favicon.ico'
})
```

#### ❌ **VAPID Keys Error**
```bash
# สร้าง VAPID keys ใหม่
npm install -g web-push
web-push generate-vapid-keys

# ตรวจสอบ format
# Public key: ต้องเป็น base64url encoded
# Private key: ต้องเป็น base64url encoded
```

### 2. Debug Mode

```typescript
// เปิด debug mode ใน PushNotificationService
const DEBUG_MODE = true

if (DEBUG_MODE) {
  console.log('Push notification debug info:', {
    platform: navigator.platform,
    userAgent: navigator.userAgent,
    permission: Notification.permission,
    serviceWorkerSupport: 'serviceWorker' in navigator,
    pushManagerSupport: 'PushManager' in window,
    timestamp: new Date().toISOString()
  })
}
```

### 3. การตรวจสอบ Platform-Specific

#### 🍎 **macOS Troubleshooting**
```bash
# ตรวจสอบ notification settings
# System Preferences > Notifications & Focus
# หา browser ที่ใช้ (Chrome/Safari/Firefox)
# เปิด "Allow Notifications"

# ตรวจสอบ Do Not Disturb mode
# Control Center > Focus > Off
```

#### 🖥️ **Windows Troubleshooting**  
```bash
# ตรวจสอบ notification settings
# Settings > System > Notifications & actions
# เปิด "Get notifications from apps and other senders"
# หา browser ที่ใช้และเปิด notifications

# ตรวจสอบ Focus Assist
# Settings > System > Focus assist > Off
```

### 4. การทดสอบขั้นสูง

```javascript
// ทดสอบการทำงานของระบบทั้งหมด
async function fullSystemTest() {
  console.log('=== Full System Test ===')
  
  // 1. ตรวจสอบการรองรับ
  console.log('Browser support:', {
    notifications: 'Notification' in window,
    serviceWorker: 'serviceWorker' in navigator,
    pushManager: 'PushManager' in window
  })
  
  // 2. ตรวจสอบ platform
  const platform = pushNotificationService.detectPlatform()
  console.log('Platform detected:', platform)
  
  // 3. ตรวจสอบ permission
  console.log('Current permission:', Notification.permission)
  
  // 4. ทดสอบ Service Worker
  const registrations = await navigator.serviceWorker.getRegistrations()
  console.log('Service Workers:', registrations.length)
  
  // 5. ทดสอบการแจ้งเตือน
  console.log('Testing notification...')
  pushNotificationService.testNativeNotification()
  
  console.log('=== Test Complete ===')
}

// รันการทดสอบ
fullSystemTest()
```

## 📝 หมายเหตุสำคัญ

### 🔒 **ข้อกำหนดด้านความปลอดภัย**
1. **HTTPS Required**: Push notifications ต้องใช้ HTTPS ใน production
2. **VAPID Keys**: เก็บ private key ให้ปลอดภัย
3. **Authentication**: ใช้ JWT token สำหรับ API calls
4. **Rate Limiting**: จำกัดจำนวน requests ต่อ user
5. **Validation**: ตรวจสอบ input data ทุกครั้ง

### 🌐 **การรองรับ Browser**
| Browser | Windows | macOS | Linux | Mobile |
|---------|---------|-------|-------|---------|
| Chrome | ✅ Full | ✅ Full | ✅ Full | ✅ Basic |
| Firefox | ✅ Full | ✅ Full | ✅ Full | ✅ Basic |
| Safari | ❌ Limited | ✅ Full | ❌ N/A | ✅ Basic |
| Edge | ✅ Full | ✅ Full | ❌ N/A | ✅ Basic |

### 📱 **Platform-Specific Limitations**

#### 🍎 **macOS**
- ✅ Service Worker notifications พร้อม action buttons
- ✅ Auto-dismiss หลัง 5 วินาที  
- ✅ Banner notifications
- ❌ Vibration ไม่รองรับ

#### 🖥️ **Windows**  
- ✅ Action Center integration
- ✅ Action buttons (เปิด/ปิด)
- ✅ Vibration patterns
- ✅ Persistent notifications

#### 📱 **Mobile**
- ✅ Basic notifications
- ❌ Action buttons จำกัด
- ❌ Service Worker จำกัดในบาง browser

### ⚠️ **ข้อควรระวัง**
1. **Permission**: ต้องได้รับอนุญาตจากผู้ใช้ก่อน
2. **Rate Limiting**: ควรจำกัดจำนวนการแจ้งเตือนเพื่อไม่ให้รบกวนผู้ใช้
3. **Do Not Disturb**: ระบบปฏิบัติการอาจบล็อกการแจ้งเตือนใน DND mode
4. **Focus Assist**: Windows Focus Assist อาจซ่อนการแจ้งเตือน
5. **Battery Optimization**: Android อาจจำกัดการทำงานใน background

### 🔄 **Fallback Strategy**
```
Native Notifications (ลำดับความสำคัญ)
    ↓
1. Service Worker + Actions (สำหรับ Windows/macOS)
    ↓
2. Basic Notification API (สำหรับระบบอื่น)
    ↓
3. In-App Notifications (เมื่อไม่ได้รับ permission)
    ↓
4. Email/SMS Fallback (สำหรับการแจ้งเตือนสำคัญ)
```

## 🚀 Next Steps

### 📈 **การปรับปรุงในอนาคต**
1. **Real-time Push** - เชื่อมต่อกับ backend websocket
2. **Notification Scheduling** - กำหนดเวลาส่งการแจ้งเตือน
3. **User Preferences** - ให้ผู้ใช้เลือกประเภทการแจ้งเตือน
4. **Analytics** - ติดตาม engagement ของการแจ้งเตือน
5. **Rich Notifications** - เพิ่มรูปภาพ, การ์ด, interactive elements

### 🔧 **การขยายระบบ**
```typescript
// ตัวอย่างการขยายระบบ
interface ExtendedNotification extends PushNotification {
  priority: 'low' | 'normal' | 'high' | 'urgent'
  category: 'chat' | 'email' | 'system' | 'marketing'
  scheduledAt?: Date
  userId: string
  deviceTargets?: string[] // specific devices
}

// Notification preferences
interface NotificationPreferences {
  userId: string
  enabledTypes: string[]
  quietHours: { start: string, end: string }
  preferredDevices: string[]
}
```

---

## 📞 **ช่วยเหลือและการสนับสนุน**

หากมีปัญหาหรือต้องการความช่วยเหลือ:
1. ตรวจสอบ console logs ก่อน
2. ทดสอบด้วย `fullSystemTest()` 
3. ตรวจสอบ browser notification settings
4. ดู troubleshooting section ข้างต้น

**การแจ้งเตือน Native ทำงานได้แล้ว! 🎉**