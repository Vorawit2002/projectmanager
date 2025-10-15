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
        await this.logout()
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
        // Fetch current user data from backend using the new client method
        const userData = await client.getApiAuthMe()
        
        // Update state from backend response
        this.token = token
        this.isLogged = true
        this.userId = userData.userId || data.sub || ''
        this.email = userData.email || data.email || ''
        this.username = userData.username || data.unique_name || data.name || ''
        this.firstName = userData.firstName || data.given_name || ''
        this.lastName = userData.lastName || data.family_name || ''
        this.roles = userData.roles || (Array.isArray(data.role) ? data.role : [data.role]) || []
        
        // Optional fields
        this.image = userData.image || ''
        this.phone = userData.phone || ''
        this.depart = userData.department || ''
        this.position = userData.position || ''
        this.titleName = userData.titleName || ''
        this.group = userData.group || ''
        this.roleHR = userData.roleHR || ''
      } catch (error) {
        console.error('Error restoring session:', error)
        // If it's a 401 error, the BaseClass interceptor will handle the redirect
        if (!isAuthError(error)) {
          await this.logout()
        }
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
      
      // Clear localStorage
      localStorage.removeItem('TOKEN_KEY')
      
      // Redirect to login page
      this.router.push('/login')
      
      console.log('Logout complete')
    },

    async generateMenu() {
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
      return rolesToCheck.some(role => this.roles.includes(role))
    },
  },
})
