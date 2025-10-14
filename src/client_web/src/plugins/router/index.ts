import type { App } from 'vue'
import { createRouter, createWebHistory } from 'vue-router'
import { routes } from './routes'
import OpenId from './OpenId'
import MasterData from './MasterData'
import AppointmentPlan from './AppointmentPlan'
import { useAuthStore } from '@/stores'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
   routes: [
    ...routes,
    ...OpenId,
    ...MasterData,
    ...AppointmentPlan
  ],
})

router.beforeEach(async (to: any, from: any, next: any) => {
  const auth = await useAuthStore()

  // redirect to login page if not logged in and trying to access a restricted page
  // i18n.global.locale = localStorage.getItem('language') == 'en-US' ? 'en-US' : 'th-TH'

  const token = await localStorage.getItem('TOKEN_KEY')
  if (token) {
    if (to.fullPath != '/Homepage' && !auth.isLogged) {
      await auth.restorelogin()
    }
  } else {
    if (to.matched.some((record: any) => record.meta.requiresAuth)) {
      await auth.logout()
    }
  }

  next()
})
export default function (app: App) {
  app.use(router)
}

export { router }
