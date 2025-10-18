import type { App } from 'vue'
import { createRouter, createWebHistory } from 'vue-router'
import { routes } from './routes'
import MasterData from './MasterData'
import AppointmentPlan from './AppointmentPlan'
import { useAuthStore } from '@/stores'
import { roleGuard } from '@/router/guards'

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

  // Public routes that don't require authentication
  const publicRoutes = ['/login', '/register', '/report', '/not-authorized', '/']
  const isPublicRoute = publicRoutes.some(route => {
    // Exact match for homepage
    if (route === '/' && to.path === '/') return true
    // Prefix match for other routes
    if (route !== '/' && to.path.startsWith(route)) return true
    return false
  })

  // Get token from localStorage
  const token = localStorage.getItem('TOKEN_KEY')

  // If accessing a public route, allow access
  if (isPublicRoute) {
    // If already logged in and trying to access login/register, redirect to dashboard
    if ((to.path === '/login' || to.path === '/register') && token && auth.isLogged) {
      next('/dashboard')
      return
    }
    next()
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

      // Check role-based access after authentication
      roleGuard(to, from, next)
      return // Important: return after roleGuard to prevent double navigation
    } catch (error) {
      // Invalid token, logout and redirect to login
      console.error('Invalid token:', error)
      await auth.logout()
      next('/login')
      return
    }
  } else {
    next()
  }
})
export default function (app: App) {
  app.use(router)
  // Make router available globally for NavigationServices
  ;(window as any).__router = router
}

export { router }
