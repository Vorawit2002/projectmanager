<template>
  <VRow>
    <VCol cols="12">
      <v-card>
        <v-row class="pa-4">
          <v-col
            cols="12"
            md="3"
          >
            <v-btn
              width="100vw"
              @click="dialogAdd = true"
              >+ Add EVENT</v-btn
            >
            <!-- <v-btn @click="gotoTest"></v-btn> -->
            <v-divider
              class="mt-3 opacity-25"
              :thickness="2"
            ></v-divider>

            <VRow class="d-flex mt-0 justify-center">
              <v-col
                cols="12"
                md="12"
              >
                <label class="mb-3 text-black">เลือกปี </label>
                <v-autocomplete
                  :items="YearList"
                  placeholder="กรุณาเลือกเลือกปี"
                  clearable
                />
              </v-col>

              <v-col
                cols="12"
                md="12"
              >
                <label class="mb-3 text-black">แผนก </label>
                <v-autocomplete
                  :items="DepartmentList"
                  placeholder="กรุณาเลือกแผนก"
                  clearable
                />
              </v-col>

              <v-col
                cols="12"
                md="12"
              >
                <label class="mb-3 text-black">พนักงาน </label>
                <v-autocomplete
                  :items="EmployeeList"
                  placeholder="กรุณาเลือกพนักงาน"
                  clearable
                />
              </v-col>
            </VRow>
          </v-col>

          <v-col
            cols="12"
            sm="9"
          >
            <v-calendar
              ref="calendar"
              v-model="value"
              :events="events"
              @click:event="onEventClick"
              @click:date="selectDate"
              event-ripple
              :view-mode="type"
              :weekdays="weekday"
              locale="th"
            >
              <template v-slot:header="{ title }">
                <v-row class="pa-4">
                  <v-col
                    cols="12"
                    md="8"
                  >
                    <v-btn
                      @click="prev"
                      class="mr-4"
                      variant="outlined"
                      ><i
                        class="ri-arrow-left-s-line"
                        style="font-size: 20px; padding: 0px -10px"
                      ></i>
                    </v-btn>

                    <v-btn
                      @click="next"
                      class="mr-4"
                      variant="outlined"
                      ><i
                        class="ri-arrow-right-s-line"
                        style="font-size: 20px; padding: 0px -10px"
                      ></i>
                    </v-btn>

                    <span style="font-size: 20px; color: rgb(var(--v-theme-primary)); font-weight: 400">
                      {{ formatDateThai(title) }}</span
                    >
                  </v-col>

                  <!-- BTN -->
                  <v-col
                    cols="12"
                    md="4"
                    class="d-flex justify-end"
                  >
                    <v-btn
                      v-for="(item, index) in ListCalendar"
                      :key="index"
                      color="primary-darken-0 rounded-0"
                      :class="[
                        index === 0
                          ? 'rounded-left'
                          : index === ListCalendar.length - 1
                          ? 'rounded-right'
                          : 'no-rounded',
                      ]"
                      border
                      variant="outlined"
                      :value="type"
                      @click="type = item.value"
                    >
                      {{ item.title }}
                    </v-btn>
                  </v-col>
                </v-row>
              </template>
            </v-calendar>
          </v-col>
        </v-row>
      </v-card>
    </VCol>
  </VRow>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import { useDate } from 'vuetify'
import moment from 'moment'
import 'moment/locale/th'
import { Employee } from '@/client'
import { useSweetAlertStore } from '@/stores'
moment.locale('th')
moment.updateLocale('th', {
  months: [
    'มกราคม',
    'กุมภาพันธ์',
    'มีนาคม',
    'เมษายน',
    'พฤษภาคม',
    'มิถุนายน',
    'กรกฎาคม',
    'สิงหาคม',
    'กันยายน',
    'ตุลาคม',
    'พฤศจิกายน',
    'ธันวาคม',
  ],
})
export default defineComponent({
  name: 'CardCalendar',
  data() {
    return {
      type: 'month' as any,
      sweetAlertStore: useSweetAlertStore(),
      YearList: [
        { title: '2023', value: '2023' },
        { title: '2024', value: '2024' },
        { title: '2025', value: '2025' },
      ],
      EmployeeList: [
        { title: 'อานนท์ คุ้มภัย', value: '0' },
        { title: 'ธนากรณ์ แสงอนันต์', value: '1' },
        { title: 'หฤษฏิ์ อ่อนน้อม', value: '2' },
      ],
      ListCalendar: [
        { title: 'Month', value: 'month' },
        { title: 'Week', value: 'week' },
        { title: 'Day', value: 'day' },
        // { title: 'List', value: 'list' },
      ],
      DepartmentList: [
        { title: 'Develop', value: 'Develop' },
        { title: 'Sale', value: 'sale' },
        { title: 'Product', value: 'product' },
        { title: 'System', value: 'system' },
      ],
      weekday: [0, 1, 2, 3, 4, 5, 6] as any,
      weekdays: [
        { title: 'Sun - Sat', value: [0, 1, 2, 3, 4, 5, 6] },
        { title: 'Mon - Sun', value: [1, 2, 3, 4, 5, 6, 0] },
        { title: 'Mon - Fri', value: [1, 2, 3, 4, 5] },
        { title: 'Mon, Wed, Fri', value: [1, 3, 5] },
      ],
      value: [new Date()] as any,
      events: [] as any,
      colors: ['blue', 'indigo', 'deep-purple', 'cyan', 'green', 'orange', 'grey darken-1'],
      names: ['Meeting', 'Holiday', 'PTO', 'Travel', 'Event', 'Birthday', 'Conference'],
      dialogAdd: false,
      selectedEvent: null as any, // หรือใส่ {} ก็ได้
      isUserClick: false, // ตัวแปรเพื่อเช็คว่าผู้ใช้คลิกหรือไม่
    }
  },
  mounted() {
    const adapter = useDate()
    this.fetchEvents(
      adapter.startOfDay(adapter.startOfMonth(new Date())),
      adapter.endOfDay(adapter.endOfMonth(new Date())),
    )
  },
  methods: {
    gotoTest() {
      this.$router.push({ name: 'ProfileBackoffice' })
    },
    onEventClick({ event }: { event: any }) {
      // ตัวอย่าง: แสดงชื่ออีเวนต์ผ่าน alert
      console.log('Event clicked:', event)
      alert(`คุณคลิกที่อีเวนต์: ${event || 'ไม่พบชื่ออีเวนต์'}`)

      // หรือถ้าคุณต้องการเปิด dialog แสดงรายละเอียด:
      this.selectedEvent = event
      this.dialogAdd = true
    },
    selectDate(dateInfo: any) {
      console.log('raw dateInfo:', dateInfo)

      let dateString = ''

      // วิธีที่ 1: ตรวจสอบว่ามี property date หรือไม่
      if (dateInfo && typeof dateInfo.date === 'string') {
        dateString = dateInfo.date
      }
      // วิธีที่ 2: ตรวจสอบว่าเป็น string โดยตรง
      else if (typeof dateInfo === 'string') {
        dateString = dateInfo
      }
      // วิธีที่ 3: ถ้าเป็น PointerEvent ให้หาข้อมูลจาก target
      else if (dateInfo && dateInfo.target) {
        // ลองหาจาก data attributes
        const target = dateInfo.target as HTMLElement

        // ลองหาจาก data-date attribute
        dateString =
          target.getAttribute('data-date') ||
          target.getAttribute('data-value') ||
          target.closest('[data-date]')?.getAttribute('data-date') ||
          target.closest('[data-value]')?.getAttribute('data-value') ||
          ''

        // ถ้าไม่เจอ ลองหาจาก class หรือ id
        if (!dateString) {
          const classList = Array.from(target.classList)
          const dateClass = classList.find(cls => cls.includes('date-') || cls.includes('day-'))
          if (dateClass) {
            const match = dateClass.match(/(\d{4}-\d{2}-\d{2})/)
            if (match) {
              dateString = match[1]
            }
          }
        }

        // ถ้ายังไม่เจอ ลองดูจาก text content
        if (!dateString && target.textContent) {
          const text = target.textContent.trim()
          // ถ้าเป็นตัวเลขวันที่ ให้สร้าง date string
          if (/^\d{1,2}$/.test(text)) {
            const currentDate = new Date()
            const day = parseInt(text)
            const year = currentDate.getFullYear()
            const month = currentDate.getMonth()
            dateString = new Date(year, month, day).toISOString().split('T')[0]
          }
        }
      }

      console.log('extracted dateString:', dateString)

      if (dateString) {
        const selectedDate = new Date(dateString)

        // แปลงให้เป็นรูปแบบไทย
        const formatted = selectedDate.toLocaleDateString('th-TH', {
          weekday: 'long',
          year: 'numeric',
          month: 'long',
          day: 'numeric',
        })

        console.log(`คุณคลิกวันที่: ${formatted}`)
        // alert(`คุณคลิกวันที่: ${formatted}`)
        this.sweetAlertStore.warning(`คุณคลิกวันที่: ${formatted}`)

        // เปิด dialog เพิ่มกิจกรรม
        this.dialogAdd = true
      } else {
        console.warn('ไม่สามารถหาข้อมูลวันที่ได้')
        console.log('event target:', dateInfo?.target)
        console.log(
          'available attributes:',
          dateInfo?.target ? Object.getOwnPropertyNames(dateInfo.target) : 'no target',
        )
      }
    },

    // วิธีที่ 2: ใช้ @click:day แทน @click:date
    // ใน template เปลี่ยนจาก @click:date="selectDate" เป็น @click:day="selectDay"
    selectDay(dayInfo: any) {
      console.log('day info:', dayInfo)

      if (dayInfo && dayInfo.date) {
        const selectedDate = new Date(dayInfo.date)
        const formatted = selectedDate.toLocaleDateString('th-TH', {
          weekday: 'long',
          year: 'numeric',
          month: 'long',
          day: 'numeric',
        })

        alert(`คุณคลิกวันที่: ${formatted}`)
        this.dialogAdd = true
      }
    },

    fetchEvents(start: any, end: any) {
      const events = []

      const min = start
      const max = end
      const days = (max.getTime() - min.getTime()) / 86400000
      const eventCount = this.rnd(days, days + 20)

      for (let i = 0; i < 10; i++) {
        const allDay = this.rnd(0, 3) === 0
        // สุ่มเวลาเริ่มต้นและสิ้นสุดของกิจกรรม
        // 15 นาที = 900000 มิลลิวินาที
        const firstTimestamp = this.rnd(min.getTime(), max.getTime())
        const first = new Date(firstTimestamp - (firstTimestamp % 900000))
        const secondTimestamp = this.rnd(2, allDay ? 288 : 8) * 900000
        const second = new Date(first.getTime() + secondTimestamp)
        const today = new Date()
        const tomorrow = new Date(today)
        tomorrow.setDate(tomorrow.getDate() + 1)
        events.push({
          title: this.names[this.rnd(0, this.names.length - 1)],
          start: today,
          end: tomorrow,
          color: this.colors[this.rnd(0, this.colors.length - 1)],
          allDay: !allDay,
        })
      }

      this.events = events
    },

    formatDateThai(date: any) {
      return moment(date, 'MMMM YYYY').locale('th').format('MMMM YYYY')
    },
    rnd(a: any, b: any) {
      return Math.floor((b - a + 1) * Math.random()) + a
    },
    next() {
      const current = new Date(this.value[0])
      if (this.type === 'month') {
        current.setMonth(current.getMonth() + 1)
      } else if (this.type === 'day') {
        current.setDate(current.getDate() + 1)
      } else {
        current.setDate(current.getDate() + 7)
      }
      this.value = [current] // ต้องเซ็ตกลับเป็น array
    },
    prev() {
      const current = new Date(this.value[0])
      if (this.type === 'month') {
        current.setMonth(current.getMonth() - 1)
      } else if (this.type === 'day') {
        current.setDate(current.getDate() - 1)
      } else {
        current.setDate(current.getDate() - 7)
      }
      this.value = [current]
    },
    today() {
      this.value = [new Date()]
    },
    // @click="handleCalendarClick"
    handleCalendarClick(event: any) {
      this.isUserClick = true
      console.log('Calendar clicked:', event)
    },
  },
})
</script>

<style></style>
