import { defineStore } from 'pinia'
import { useRouter } from 'vue-router'
import { useSweetAlertStore } from './sweetalert2'
import { Client } from '@/client'
import { BACKEND_API_URL, ClientId, ClientSecret, PortalOpenId } from '@/constants'
// import { RoleService } from '@/utils/RoleService'
// import { AuthenticatedApiClient } from '@/AuthenticatedApiClient'
// import {
//   AdministratorMenu,
//   NATEmployeeMenu,
//   NATMemberMenu,
//   NoneMenu,
// } from '@/utils/NavigationGenerator'
import { Administrator, NATEmployee, NATMember } from '@/utils/ApplicationRoles'
import { RoleService } from '@/utils/RoleService'
let client = new Client(BACKEND_API_URL)
// let authClient = new AuthenticatedApiClient(BACKEND_API_URL)
let roleService = new RoleService()
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
    // async login(account: AuthenticateUserCommand) {
    //   console.log('AuthenticateUserCommand = ', account)
    //   try {
    //     const response = await Client.authenticate(account)

    //     // console.log(response)
    //     if (response.succeeded) {
    //       let token = response.token as string
    //       this.token = token
    //       this.isLogged = true
    //       localStorage.setItem('TOKEN_KEY', token)
    //       console.log('TOKEN_KEY = ', token)

    //       await this.restorelogin()

    //       if (this.isLogged) {
    //         this.SweetAlert.success('เข้าสู่ระบบสำเร็จ')
    //       }

    //       this.handleFirstLoginNavigation()
    //     } else {
    //       // แสดง error จาก server
    //       if (response.errors && response.errors.length > 0) {
    //         console.log('Error response:', response.errors)
    //         this.SweetAlert.warning('ชื่อผู้ใช้หรือรหัสผ่านไม่ถูกต้อง') // เปลี่ยนจาก error เป็น warning
    //       } else {
    //         this.SweetAlert.warning('ชื่อผู้ใช้หรือรหัสผ่านไม่ถูกต้อง')
    //       }
    //     }
    //   } catch (error: any) {
    //     console.error('Unexpected error during login:', error)
    //     this.SweetAlert.error('เกิดข้อผิดพลาด กรุณาลองใหม่ในภายหลัง')
    //   }
    // },

    // async restorelogin() {
    //   try {
    //     //api restore data
    //     // let response: any = await client.getApplicationProfile()
    //     // console.log('restore login response = ', response)
    //     this.username = response.userName
    //     this.firstName = response.firstName
    //     this.lastName = response.lastName
    //     this.roles = response.roles
    //     this.email = response.email
    //     this.displayName = response.displayName
    //     // this.userId = response.userId
    //     // this.generateMenu()
    //     this.isLogged = true
    //   } catch (error: any) {
    //     // this.SweetAlert.error(error.message)
    //     // this.isLogged = false
    //     // this.logout()
    //     console.error('Restore login failed:', error)
    //     // await this.logout()
    //     this.SweetAlert.error('เกิดข้อผิดพลาด กรุณาเข้าสู่ระบบอีกครั้ง')
    //   }
    // },
    async restorelogin() {
      console.log('restore')
      let token = localStorage.getItem('TOKEN_KEY') || ''
      if (!token) {
        await this.logout()
        return
      }
      const data = this.decodeJWT(token)
      console.log('Decoded JWT data:', data)
      const currentTime = Math.floor(Date.now() / 1000) // เวลาปัจจุบันในหน่วยวินาที
      if (data.exp && data.exp < currentTime) {
        console.warn('Token has expired, logging out')
        await this.logout()
        return
      } else {
        try {
          // const res = await client.getEmployeeQueryByUserID(data.sub)
                  const res = await this.objEmployeeByUsernameAD(data.email)
          this.token = token
          this.username = data.name || null
          this.isLogged = true
          this.email = data.email || null
          this.userId = data.sub
          this.roles = data.role
          this.image = data.profile_image !== 'No Assign' ? data.profile_image : null
          this.phone = data.phone_number !== 'No Assign' ? data.phone_number : null
          this.depart = data.depart !== 'No Assign' ? data.depart : null
          this.position = data.position !== 'No Assign' ? data.position : null
          this.titleName = data.TitleName !== 'No Assign' ? data.TitleName : null
          this.firstName = data.FistNameTH !== 'No Assign' ? data.FistNameTH : null
          this.lastName = data.LastNameTH !== 'No Assign' ? data.LastNameTH : null
            this.group = res.group || ''
        this.roleHR = res.roles || ''
        } catch (error) {
          console.error(error)
          await this.logout()
        }
      }
    },
    async restoreData() {
      console.log('restore')
      let token = localStorage.getItem('TOKEN_KEY') || ''
      if (!token) {
        await this.logout()
        return
      }
      const data = this.decodeJWT(token)
      console.log('Decoded JWT data:', data)
      const currentTime = Math.floor(Date.now() / 1000) // เวลาปัจจุบันในหน่วยวินาที
      if (data.exp && data.exp < currentTime) {
        console.warn('Token has expired, logging out')
        await this.logout()
        return
      } else {
                const res = await this.objEmployeeByUsernameAD(data.email)
        this.token = token
        this.username = data.name || ''
        this.isLogged = true
        this.email = data.email || ''
        this.userId = data.sub
        this.roles = data.role
        this.image = data.profile_image !== 'No Assign' ? data.profile_image : ''
        this.phone = data.phone_number !== 'No Assign' ? data.phone_number : ''
        this.depart = data.depart !== 'No Assign' ? data.depart : ''
        this.position = data.position !== 'No Assign' ? data.position : ''
        this.titleName = data.TitleName !== 'No Assign' ? data.TitleName : ''
        this.firstName = data.FistNameTH !== 'No Assign' ? data.FistNameTH : ''
        this.lastName = data.LastNameTH !== 'No Assign' ? data.LastNameTH : ''
          this.group = res.group || ''
        this.roleHR = res.roles || ''
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
      console.log('ก่อน logout username = ', this.username)
      const data = {
        token: localStorage.getItem('TOKEN_KEY') || '',
        token_type_hint: 'aceess_token',
        client_id: ClientId,
        client_secret: ClientSecret,
      } as any
      const formBody = Object.keys(data)
        .map(key => encodeURIComponent(key) + '=' + encodeURIComponent(data[key]))
        .join('&')
      console.log(formBody)
      // ส่งคำขอด้วย fetch
      if (data.token) {
        fetch(`${PortalOpenId}/revocation`, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
          },
          body: formBody,
        })
          .then(response => response.json())
          .then(async data => {
            console.log(data)
          })
      }
      this.username = ''
      this.isLogged = false
      this.token = ''
      this.roles = []
      this.returnUrl = ''
      this.image = ''
      localStorage.removeItem('TOKEN_KEY')
      this.router.push('/')
      console.log('หลัง logout ', this.username)
    },
  async objEmployeeByUsernameAD(username: string) {
      let command = { usernameAD: username.toLowerCase() }
      const response = await fetch('https://hr.nti.co.th/api/EmployeeEndpoint/ObjEmployeeByUsernameAD', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(command),
      })

      if (!response.ok) {
        throw new Error(`HTTP error! Status: ${response.status}`)
      }

      const data = await response.json()
      return data
    },
    // async generateMenu() {
    //   switch (true) {
    //     case this.hasRole(Administrator):
    //       this.menu = AdministratorMenu()
    //       break
    //     case this.hasRole(NATMember):
    //       this.menu = NATMemberMenu()
    //       break
    //     case roleService.isNATEmployee(this.roles):
    //       this.menu = NATEmployeeMenu()
    //       break
    //     default:
    //       this.menu = NoneMenu()
    //       break
    //   }
    // },

    hasRole(...rolesToCheck: string[]) {
      return rolesToCheck.some(role => this.roles.includes(role))
    },
  },
})
