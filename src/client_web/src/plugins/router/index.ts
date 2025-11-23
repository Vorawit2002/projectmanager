import type { App } from 'vue'
import { createRouter, createWebHistory } from 'vue-router'
import Swal from 'sweetalert2'
import { routes } from './routes'
import MasterData from './MasterData'
import AppointmentPlan from './AppointmentPlan'
import { useAuthStore } from '@/stores'
import { roleGuard } from '@/router/guards'

// Helper function to check if user profile is complete
function isProfileComplete(auth: any): boolean {
  // Check required fields
  const requiredFields = [
    auth.firstName,
    auth.lastName,
    auth.email,
    auth.phone,
    auth.depart,    // แผนก
    auth.position,  // ตำแหน่ง
  ]

  return requiredFields.every(field => field && field.trim() !== '')
}

// Track if we've already shown the dialog in this session
let profileDialogShown = false

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    ...routes,
    ...MasterData,
    ...AppointmentPlan
  ],
})

router.beforeEach(async (to: any, from: any, next: any) => {
  const auth = useAuthStore()

  // Get token from localStorage
  const token = localStorage.getItem('TOKEN_KEY')

  // Public routes accessible without authentication
  const publicRoutes = ['/', '/login', '/register']
  const isPublicRoute = publicRoutes.includes(to.path)

  // If not logged in, only allow access to public routes
  if (!token) {
    if (isPublicRoute) {
      next()
      return
    }
    // Redirect to homepage for any other route
    console.log('Not logged in, redirecting to homepage')
    next('/')
    return
  }

  // If logged in and accessing login/register, redirect to dashboard
  if ((to.path === '/login' || to.path === '/register') && token && auth.isLogged) {
    next('/dashboard')
    return
  }

  // Check if route requires authentication
  const requiresAuth = to.matched.some((record: any) => record.meta.requiresAuth)

  if (requiresAuth || to.path !== '/login') {
    if (!token) {
      // No token, redirect to login
      console.log('No token found, redirecting to login')
      next('/login')
      return
    }

    // Check token expiration
    try {
      const decoded = auth.decodeJWT(token)
      const currentTime = Math.floor(Date.now() / 1000)

      if (decoded.exp && decoded.exp < currentTime) {
        // Token expired, logout and redirect to login
        console.log('Token expired, redirecting to login')
        await auth.logout()
        next('/login')
        return
      }

      // Token is valid, restore session if not already logged in
      if (!auth.isLogged) {
        console.log('Restoring session before navigation')
        await auth.restoreSession()

        // After restore, check if still logged in
        if (!auth.isLogged) {
          console.log('Session restore failed, redirecting to login')
          next('/login')
          return
        }
      }

      // Check if user is Viewer role - block access and show message
      if (auth.roles.includes('Viewer')) {
        console.log('Viewer role detected, showing access denied message')
        await Swal.fire({
          title: 'ไม่มีสิทธิ์เข้าถึงระบบ',
          text: 'กรุณาติดต่อผู้ดูแลระบบเพื่อขอสิทธิ์ใช้งานระบบ',
          icon: 'warning',
          confirmButtonText: '<span style="color: white;">ตกลง</span>',
          confirmButtonColor: '#41B06E',
          allowOutsideClick: false,
          allowEscapeKey: false,
        })
        await auth.logout()
        next('/login')
        return
      }

      // Check if profile is complete only when accessing Homepage
      if (to.path === '/Homepage' && !profileDialogShown) {
        // Fetch fresh user data from database
        try {
          const client = new (await import('@/client')).Client((await import('@/constants')).BACKEND_API_URL)
          const userData = await client.getCurrentUser()
          
          // Check required fields from database
          const requiredFields = [
            userData.firstName,
            userData.lastName,
            userData.email,
            userData.phone,
            userData.department,  // แผนก
            userData.position,    // ตำแหน่ง
          ]
          
          const isComplete = requiredFields.every(field => field && field.trim() !== '')
          
          // Only show dialog if profile is incomplete
          if (!isComplete) {
            profileDialogShown = true

            // Allow navigation first
            next()

            // Show dialog after a delay to let the success dialog close first
            setTimeout(() => {
              Swal.fire({
                title: 'กรุณากรอกข้อมูลส่วนตัว',
                text: 'คุณยังกรอกข้อมูลส่วนตัวไม่ครบถ้วน กรุณากรอกข้อมูลให้ครบเพื่อใช้งานระบบ',
                icon: 'warning',
                confirmButtonText: '<span style="color: white;">ไปกรอกข้อมูล</span>',
                confirmButtonColor: '#41B06E',
                allowOutsideClick: false,
                allowEscapeKey: false,
              }).then(() => {
                router.push('/account-settings')
              })
            }, 500)
            return
          }
        } catch (error) {
          console.error('Error checking profile completeness:', error)
        }
      }

      // Check role-based access after authentication
      roleGuard(to, from, next)
      return // Important: return after roleGuard to prevent double navigation
    }
 catch (error) {
      // Invalid token, logout and redirect to login
      console.error('Invalid token:', error)
      await auth.logout()
      next('/login')
      return
    }
  }
 else {
    next()
  }
})
export default function (app: App) {
  app.use(router)
    // Make router available globally for NavigationServices
    ; (window as any).__router = router
}

export { router }
