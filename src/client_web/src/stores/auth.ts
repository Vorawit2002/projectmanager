import { defineStore } from 'pinia'
import { useRouter } from 'vue-router'
import { useSweetAlertStore } from './sweetalert2'
import { Client } from '@/client'
import { BACKEND_API_URL } from '@/constants'
import { Administrator, NATEmployee, NATMember } from '@/utils/ApplicationRoles'
import { RoleService } from '@/utils/RoleService'
import { parseAuthError, isAuthError, isValidationError } from '@/utils/authErrors'
import { BaseClass } from '@/BaseClass'
import { NoneMenu } from '@/utils/NavigationGenerator'
import { NATEmployeeMenu } from '@/utils/NavigationGenerator'
import { NATMemberMenu } from '@/utils/NavigationGenerator'
import { AdministratorMenu } from '@/utils/NavigationGenerator'

let client = new Client(BACKEND_API_URL)
let roleService = new RoleService()

// Register unauthorized callback to handle 401 responses globally
BaseClass.onUnauthorized(() => {
  console.log('Unauthorized callback triggered - clearing auth state')
})
export const useAuthStore = defineStore('auth', {
  state: () => ({
    token: '',
    returnUrl: '',
    isLogged: false,
    username: '',
    roles: [] as string[],
    titleName: '',
    firstName: '',
    lastName: '',
    email: '',
    displayName: '',
    userId: '',
    menu: [] as any,
    SweetAlert: useSweetAlertStore(),
    router: useRouter(),
    image: '' as any,
    phone: '' as any,
    depart: '' as any,
    position: '' as any,
        group: '',
    roleHR: '',
    employeeId: '' as string,
    departmentId: '' as string,
  }),
  getters: {
    IsLogged(state) {
      return state.isLogged
    },
    UserName(state) {
      return state.username
    },
    MenuList(state) {
      return state.menu
    },
  },
  actions: {
    async login(emailOrUsername: string, password: string): Promise<boolean> {
      try {
        const command = {
          emailOrUsername,
          password,
        }
        
        const data = await client.login(command)
        
        // Store token
        this.token = data.token || ''
        localStorage.setItem('TOKEN_KEY', data.token || '')
        
        // Update state
        this.isLogged = true
        this.userId = data.userId || ''
        this.email = data.email || ''
        this.username = data.username || ''
        this.firstName = data.firstName || ''
        this.lastName = data.lastName || ''
        this.roles = data.roles || []
        
        // Fetch full user profile after login to get all details including image
        try {
          const userData = await client.getCurrentUser()
          
          // Update additional user details
          this.titleName = userData.titleName || ''
          this.position = userData.position || ''
          this.phone = userData.phone || ''
          this.depart = userData.department || ''
          this.group = userData.group || ''
          this.roleHR = userData.roleHR || ''
          this.employeeId = userData.employeeId || ''
          this.departmentId = userData.departmentId || ''
          
          // Handle image URL - support both data URLs (Base64) and relative paths
          const imageProfile = userData.imageProfile || ''
          if (imageProfile && !imageProfile.startsWith('http') && !imageProfile.startsWith('data:')) {
            // It's a relative path, prepend BACKEND_API_URL
            this.image = `${BACKEND_API_URL}${imageProfile.startsWith('/') ? '' : '/'}${imageProfile}`
          } else {
            // It's either a full URL (http/https) or a data URL (data:)
            this.image = imageProfile
          }
        } catch (error) {
          console.warn('Failed to fetch full user profile after login:', error)
        }
        
        // Generate menu based on roles
        await this.generateMenu()
        
        this.SweetAlert.success('เข้าสู่ระบบสำเร็จ')
        
        // Redirect to homepage
        this.router.push('/Homepage')
        
        return true
      } catch (error: any) {
        console.error('Login error:', error)
        const errorMessage = parseAuthError(error)
        this.SweetAlert.error(errorMessage)
        return false
      }
    },
    async register(registerData: {
      email: string
      username: string
      password: string
      confirmPassword: string
      firstName: string
      lastName: string
    }): Promise<{ success: boolean; message?: string }> {
      try {
        const command = registerData
        
        await client.register(command)

        this.SweetAlert.success('สมัครสมาชิกสำเร็จ กรุณาเข้าสู่ระบบ')
        
        return { success: true }
      } catch (error: any) {
        console.error('Register error:', error)
        const errorMessage = parseAuthError(error)
        this.SweetAlert.error(errorMessage)
        return { success: false, message: errorMessage }
      }
    },
    async restoreSession() {
      console.log('Restoring session from token')
      let token = localStorage.getItem('TOKEN_KEY') || ''
      if (!token) {
        console.log('No token found in localStorage')
        this.isLogged = false
        return
      }
      
      const data = this.decodeJWT(token)
      console.log('Decoded JWT data:', data)
      
      const currentTime = Math.floor(Date.now() / 1000)
      if (data.exp && data.exp < currentTime) {
        console.warn('Token has expired, logging out')
        await this.logout()
        return
      }
      
      try {
        // Fetch current user data from backend
        const userData = await client.getCurrentUser()
        
        console.log('Full userData from backend:', userData)
        console.log('userData type:', typeof userData)
        console.log('userData.roles:', userData.roles)
        console.log('userData.roleNames:', userData.roleNames)
        
        // Update state from backend response
        this.token = token
        this.isLogged = true
        this.userId = userData.userId || data.sub || ''
        this.email = userData.email || data.email || ''
        this.username = userData.userName || userData.username || data.unique_name || data.name || ''
        this.firstName = userData.firstName || data.given_name || ''
        this.lastName = userData.lastName || data.family_name || ''
        
        // Handle roles - ensure it's always an array
        // Try both 'roles' and 'roleNames' properties
        let userRoles = userData.roles || userData.roleNames || []
        console.log('userRoles after first check:', userRoles, 'isArray:', Array.isArray(userRoles))
        
        if (!Array.isArray(userRoles) || userRoles.length === 0) {
          // If roles come from JWT token
          console.log('No roles from backend, trying JWT token')
          const jwtRole = data.role
          userRoles = Array.isArray(jwtRole) ? jwtRole : (jwtRole ? [jwtRole] : [])
        }
        this.roles = userRoles
        
        console.log('Final user roles restored:', this.roles)
        
        // Optional fields
        // Handle image URL - support both data URLs (Base64) and relative paths
        const imageProfile = userData.imageProfile || ''
        if (imageProfile && !imageProfile.startsWith('http') && !imageProfile.startsWith('data:')) {
          // It's a relative path, prepend BACKEND_API_URL
          this.image = `${BACKEND_API_URL}${imageProfile.startsWith('/') ? '' : '/'}${imageProfile}`
        } else {
          // It's either a full URL (http/https) or a data URL (data:)
          this.image = imageProfile
        }
        
        this.phone = userData.phone || ''
        this.depart = userData.department || ''
        this.position = userData.position || ''
        this.titleName = userData.titleName || ''
        this.group = userData.group || ''
        this.roleHR = userData.roleHR || ''
        this.employeeId = userData.employeeId || ''
        this.departmentId = userData.departmentId || ''
        
        // Generate menu based on restored roles
        await this.generateMenu()
        
        console.log('Session restored successfully for user:', this.username)
      } catch (error: any) {
        console.error('Error restoring session:', error)
        
        // Check if it's a 401 Unauthorized error
        if (error?.status === 401 || isAuthError(error)) {
          console.log('Unauthorized error during session restore, logging out')
          await this.logout()
          return
        }
        
        // For other errors, try to use JWT data as fallback
        console.warn('Failed to fetch user data from backend, using JWT data as fallback')
        
        // Use JWT data as fallback
        this.token = token
        this.isLogged = true
        this.userId = data.sub || ''
        this.email = data.email || ''
        this.username = data.unique_name || data.name || ''
        this.firstName = data.given_name || ''
        this.lastName = data.family_name || ''
        
        // Handle roles from JWT
        const jwtRole = data.role
        this.roles = Array.isArray(jwtRole) ? jwtRole : (jwtRole ? [jwtRole] : [])
        
        console.log('Using JWT fallback data, roles:', this.roles)
        
        // Generate menu based on JWT roles
        await this.generateMenu()
      }
    },

    decodeJWT(token: string) {
      try {
        if (!token || typeof token !== 'string' || token.split('.').length !== 3) {
          throw new Error('Invalid JWT format')
        }

        const payload = token.split('.')[1]

        // padding base64 ให้ครบตามมาตรฐาน
        const base64 = payload.replace(/-/g, '+').replace(/_/g, '/')
        const padded = base64.padEnd(base64.length + ((4 - (base64.length % 4)) % 4), '=')

        const binary = atob(padded)
        const bytes = Uint8Array.from(binary, c => c.charCodeAt(0))
        const decoded = new TextDecoder('utf-8').decode(bytes)
        return JSON.parse(decoded)
      } catch (error) {
        console.error('decodeJWT error:', error)
        return {}
      }
    },
    decodeBase64Json(base64String: string) {
      try {
        const base64 = base64String.replace(/-/g, '+').replace(/_/g, '/')
        const padded = base64.padEnd(base64.length + ((4 - (base64.length % 4)) % 4), '=')
        const decoded = atob(padded)
        return JSON.parse(decoded)
      } catch (e) {
        console.error('Failed to decode:', e)
        return null
      }
    },
    handleFirstLoginNavigation() {
      console.log('Roles =', this.roles)

      const hasRole = (targetRoles: string[]) => {
        return this.roles?.some((role: string) => targetRoles.includes(role))
      }

      // if (hasRole(['Employee'])) {
      //   this.router.push('/ReportEtracking')
      // } else if (hasRole(['Administrator', 'MasterWeb', 'ManagementImport'])) {
      //   this.router.push('/MasterEtracking')
      // } else {
      //   console.warn('No matching role found. Please verify roles:', this.roles)
      //   // เพิ่ม router.push ไปหน้า default หรือ error page ถ้าจำเป็น
      // }
    },

    async logout() {
      console.log('Logging out user:', this.username)
      
      // Clear state
      this.username = ''
      this.isLogged = false
      this.token = ''
      this.roles = []
      this.returnUrl = ''
      this.email = ''
      this.userId = ''
      this.firstName = ''
      this.lastName = ''
      this.image = ''
      this.phone = ''
      this.depart = ''
      this.position = ''
      this.titleName = ''
      this.displayName = ''
      this.group = ''
      this.roleHR = ''
      this.employeeId = ''
      this.departmentId = ''
      
      // Clear localStorage
      localStorage.removeItem('TOKEN_KEY')
      
      // Redirect to login page
      this.router.push('/login')
      
      console.log('Logout complete')
    },

    async generateMenu() {
      // Check for new role management roles first (Admin, Manager, User, Viewer)
      if (roleService.isAdmin(this.roles)) {
        this.menu = AdministratorMenu()
        return
      }
      
      if (roleService.isManager(this.roles)) {
        this.menu = AdministratorMenu() // Manager has same menu as Admin
        return
      }
      
      if (roleService.isUser(this.roles)) {
        this.menu = NATEmployeeMenu() // User has employee menu without Master Data
        return
      }
      
      if (roleService.isViewer(this.roles)) {
        this.menu = NATEmployeeMenu() // Viewer has employee menu (read-only)
        return
      }
      
      // Fallback to legacy role checking
      switch (true) {
        case this.hasRole(Administrator):
          this.menu = AdministratorMenu()
          break
        case this.hasRole(NATMember):
          this.menu = NATMemberMenu()
          break
        case roleService.isNATEmployee(this.roles):
          this.menu = NATEmployeeMenu()
          break
        default:
          this.menu = NoneMenu()
          break
      }
    },

    hasRole(...rolesToCheck: string[]) {
      if (!this.roles || this.roles.length === 0) {
        return false
      }
      return rolesToCheck.some(role => this.roles.includes(role))
    },
  },
})
