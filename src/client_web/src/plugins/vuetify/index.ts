import type { App } from 'vue'

import { createVuetify } from 'vuetify'
import { VBtn } from 'vuetify/components/VBtn'
import defaults from './defaults'
import { icons } from './icons'
import { themes } from './theme'
// import { VTimePicker } from 'vuetify/labs/VTimePicker'
import { VCalendar } from 'vuetify/labs/VCalendar'
import { pl, zhHans, th } from 'vuetify/locale'
// ข้อความภาษาไทยที่ต้องการเปลี่ยน
const myThaiMessages = {
  $vuetify: {
    timePicker: {
      title: 'เลือกเวลา',          // เปลี่ยนหัวเรื่อง (Title)
      cancel: 'ยกเลิก',             // ปุ่มยกเลิก
      ok: 'ตกลง',                   // ปุ่มตกลง
      am: 'ก่อนเที่ยง',             // AM
      pm: 'หลังเที่ยง',             // PM
    },
  },
}

// Styles

import '@core/scss/template/libs/vuetify/index.scss'
import 'vuetify/styles'

export default function (app: App) {
  const vuetify = createVuetify({
    aliases: {
      IconBtn: VBtn,
      // VTimePicker,
      VCalendar,
    },
    defaults,
    icons,
    theme: {
      defaultTheme: 'light',
      themes,
    },
    locale: {
    locale: 'th',
    fallback: 'th',
    messages: {
      th: {
        ...th,
        ...myThaiMessages,
      },
    },
  },
  })

  app.use(vuetify)
}
