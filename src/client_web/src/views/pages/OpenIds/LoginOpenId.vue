<script lang="ts">
import {
  BACKEND_API_URL,
  ClientId,
  ClientSecret,
  code_verifier,
  GoLogin,
  PortalOpenId,
  RedirectUris,
  scope,
} from '@/constants'
import { defineComponent } from 'vue'
// import CardVueCalendar from '@/views/pages/Calendar/CardVueCalendar.vue'
import { CheckAndCreateEmployeeCommand, Client } from '@/client'
import { useAuthStore, useSweetAlertStore } from '@/stores'
const client = new Client(BACKEND_API_URL)
export default defineComponent({
  // components: {
  //   CardVueCalendar,
  // },
  name: 'LoginOpenId',
  data() {
    return {
      state: '' as any,
      code: '' as any,
      auth: useAuthStore(),
      CreateCommand: new CheckAndCreateEmployeeCommand(),
      sweetAlertStore: useSweetAlertStore(),
      GoLogin,
    }
  },
  mounted() {
    this.code = this.$route.query.code
    this.state = this.$route.query.state
    console.log(this.state)

    if (this.code && this.state) {
      this.initialize()
    } else {
      console.error('Missing code or state in query parameters')
    }
  },
  methods: {
    initialize() {
      const data = {
        grant_type: 'authorization_code',
        client_id: ClientId,
        Scope: scope,
        State: this.state,
        code: this.code,
        redirect_uri: RedirectUris,
        code_verifier: code_verifier,
        client_secret: ClientSecret,
        // refresh_token:REFRESHTOKEN ?? ''
      } as any
      const formBody = Object.keys(data)
        .map(key => encodeURIComponent(key) + '=' + encodeURIComponent(data[key]))
        .join('&')
      console.log(formBody)
      // ส่งคำขอด้วย fetch
      fetch(`${PortalOpenId}/token`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/x-www-form-urlencoded',
        },
        body: formBody,
      })
        .then(response => response.json())
        .then(async data => {
          console.log('Success:', data)
          if (data.error) {
            console.error('Error:', data.error_description)
            this.GoLogin()
            return
          } else if (data.access_token) {
            localStorage.setItem('TOKEN_KEY', data.access_token!)
            await this.auth.restoreData()
            try {
              if (!this.auth.userId || !this.auth.email) {
                console.log('User ID or email is not set, attempting to restore login...')
                const confirm = await this.sweetAlertStore.warning(
                  'ไม่สามารถเข้าสู่ระบบได้!!',
                  'เนื่องจากไม่มีข้อมูลของคุณในระบบ',
                )
                await this.auth.logout()
                this.$router.push('/')

                return
              }
              if (this.auth.roles === undefined || this.auth.roles.length === 0) {
                setTimeout(async () => {
                  const confirm = await this.sweetAlertStore.warning(
                    'ไม่สามารถเข้าสู่ระบบได้!!',
                    'เนื่องจากคุณไม่มีสิทธิ์เข้าใช้งานในระบบกรุณาติดต่อแอดมิน',
                  )
                  await this.auth.logout()
                  this.$router.push('/')
                }, 2000)
                return
              }
              if (this.auth.roles.length > 0 && !this.auth.roles.includes('CRM')) {
                setTimeout(async () => {
                  const confirm = await this.sweetAlertStore.warning(
                    'ไม่สามารถเข้าสู่ระบบได้!!',
                    'เนื่องจากคุณไม่มีสิทธิ์เข้าใช้งานในระบบ กรุณาติดต่อแอดมิน',
                  )
                  await this.auth.logout()
                  this.$router.push('/')
                }, 2000)
                return
              }
              if (
                !this.auth.email ||
                !this.auth.phone ||
                !this.auth.depart ||
                !this.auth.position ||
                !this.auth.titleName ||
                !this.auth.firstName ||
                !this.auth.lastName
              ) {
                setTimeout(async () => {
                  const confirm = await this.sweetAlertStore.showAlert({
                    title: 'ข้อมูลของท่านไม่ครบถ้วน',
                    text: 'เนื่องจากข้อมูลโปรไฟล์ของคุณไม่ครบ กรุณากรอกข้อมูล',
                    icon: 'warning',
                    confirmButtonText: 'ตกลง!',
                  })
                  if (confirm.isConfirmed) {
                    await this.auth.logout()
                    window.location.href = `https://ntiportal.nti.co.th/Identity/Account/Manage`
                  } else {
                    await this.auth.logout()
                    this.$router.push('/')
                  }
                }, 2000)
                return
              }
              this.CreateCommand.userId = this.auth.userId
              this.CreateCommand.email = this.auth.email
              this.CreateCommand.phone = this.auth.phone
              this.CreateCommand.departments = this.auth.depart
              this.CreateCommand.position = this.auth.position
              this.CreateCommand.titleName = this.auth.titleName
              this.CreateCommand.firstName = this.auth.firstName
              this.CreateCommand.lastName = this.auth.lastName
              this.CreateCommand.imageProfile = this.auth.image
              this.CreateCommand.group = this.auth.group
              this.CreateCommand.roles = this.auth.roleHR
              console.log(this.CreateCommand)
              const response = await client.checkAndCreateEmployee(this.CreateCommand)

              if (response) {
                if (this.state !== 'authencrm') {
                  const data = this.auth.decodeBase64Json(this.state)
                  console.log(data)
                  setTimeout(async () => {
                    this.$router.push(`${data.ru}`)
                  }, 1000)
                  return
                } else {
                  setTimeout(async () => {
                    this.$router.push('/dashboard')
                    //     //const confirm = await this.sweetAlertStore.showAlert({
                    //     //  title: 'ยินดีต้อนรับสู่ CRM',
                    //     //  text: 'เนื่องจากคุณเข้าสู่ระบบด้วย OpenID คุณอาจต้องกรอกข้อมูลส่วนตัวเพิ่มเติมในครั้งแรกที่เข้าสู่ระบบ',
                    //     //  icon: 'success',
                    //     //  confirmButtonText: 'ตกลง!',
                    //     //})
                    //     //if (confirm.isConfirmed) {
                    //     //  this.$router.push('/account-settings')
                    //     //}
                  }, 1000)
                }
              } else {
                setTimeout(async () => {
                  if (this.state !== 'authencrm') {
                    const data = this.auth.decodeBase64Json(this.state)
                    console.log(data)
                    setTimeout(async () => {
                      this.$router.push(`${data.ru}`)
                    }, 1000)
                    return
                  } else {
                    this.$router.push('/dashboard')
                  }
                }, 1000)
              }
            } catch (error) {
              console.error('Error restoring login:', error)
              this.sweetAlertStore.warning('ไม่สามารถเข้าสู่ระบบได้!!', 'เกิดข้อผิดพลาด')
              await this.auth.logout()
              this.$router.push('/')
            }
          }
        })
        .catch(error => {
          console.error('Error:', error)
          this.$router.push('/')
        })
    },
  },
})
</script>

<template>
  <div
    class="misc-wrapper"
    style="background: #e8f9ff !important"
  >
    <!-- 👉 Image -->
    <div class="misc-avatar w-100 text-center mb-16">
      <figure class="loader-connect">
        <div class="dot white"></div>
        <div class="dot"></div>
        <div class="dot"></div>
        <div class="dot"></div>
        <div class="dot"></div>
      </figure>
    </div>

    <p class="mt-10"></p>

    <center>
      <h2 class="mt-16 text-center">กรุณารอสักครู่กำลังเข้าสู่ระบบ CRM ...</h2>
    </center>

    <!-- 👉 Footer -->
    <!-- <VImg
      :src="tree"
      class="misc-footer-tree d-none d-md-block"
    /> -->
  </div>
</template>

<style lang="scss">
@use '@core/scss/template/pages/misc.scss';

.misc-footer-tree {
  inline-size: 15.625rem;
  inset-block-end: 3.5rem;
  inset-inline-start: 0.375rem;
}

.loader-connect {
  position: absolute;
  margin: auto;
  top: 0;
  bottom: 0;
  left: 0;
  right: 0;
  width: 7.25em;
  height: 7.25em;
  animation: rotate5123 2.4s linear infinite;
}

.white {
  top: 0;
  bottom: 0;
  left: 0;
  right: 0;
  background: white;
  animation: flash 2.4s linear infinite;
  opacity: 0;
}

.dot {
  position: absolute;
  margin: auto;
  width: 3.4em;
  height: 3.4em;
  border-radius: 100%;
  transition: all 1s ease;
}

.dot:nth-child(2) {
  top: 0;
  bottom: 0;
  left: 0;
  background: #ff4444;
  animation: dotsY 2.4s linear infinite;
}

.dot:nth-child(3) {
  left: 0;
  right: 0;
  top: 0;
  background: #ffbb33;
  animation: dotsX 2.4s linear infinite;
}

.dot:nth-child(4) {
  top: 0;
  bottom: 0;
  right: 0;
  background: #99cc00;
  animation: dotsY 2.4s linear infinite;
}

.dot:nth-child(5) {
  left: 0;
  right: 0;
  bottom: 0;
  background: #33b5e5;
  animation: dotsX 2.4s linear infinite;
}

@keyframes rotate5123 {
  0% {
    transform: rotate(0);
  }

  10% {
    width: 7.25em;
    height: 7.25em;
  }

  66% {
    width: 3.4em;
    height: 3.4em;
  }

  100% {
    transform: rotate(360deg);
    width: 7.25em;
    height: 7.25em;
  }
}

@keyframes dotsY {
  66% {
    opacity: 0.1;
    width: 3.4em;
  }

  77% {
    opacity: 1;
    width: 0;
  }
}

@keyframes dotsX {
  66% {
    opacity: 0.1;
    height: 3.4em;
  }

  77% {
    opacity: 1;
    height: 0;
  }
}

@keyframes flash {
  33% {
    opacity: 0;
    border-radius: 0%;
  }

  55% {
    opacity: 0.6;
    border-radius: 100%;
  }

  66% {
    opacity: 0;
  }
}
</style>
