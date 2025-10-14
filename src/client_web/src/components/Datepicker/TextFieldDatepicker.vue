<template>
  <v-menu
    v-model="dialogShow"
    :close-on-content-click="false"
  >
    <template v-slot:activator="{ props }">
      <h3 v-if="bold">{{ Message }}</h3>
      <span v-else>{{ Message }}</span>
      <!-- TextField to display formatted date in Thai with placeholder -->
      <VTextField
        v-if="readonly"
        v-bind="$attrs"
        :readonly="readonly"
        class="text-blue"
        variant="outlined"
        density="comfortable"
        dense
        color="primary-darken-0"
        bg-color="white"
        rounded="md"
        :value="DateFormatThai || ''"
        v-model="selectedDate"
        :placeholder="placeholder"
        :rules="rules || [(v: any) => !!v || `กรุณาระบุวันที่`]"
      >
      </VTextField>

      <VTextField
        v-else
        v-bind="{ ...props, ...$attrs }"
        class="text-blue"
        :value="DateFormatThai || ''"
        v-model="selectedDate"
        @click="dialogShow = true"
        variant="outlined"
        density="comfortable"
        dense
        color="primary-darken-0"
        bg-color="white"
        rounded="md"
        :placeholder="placeholder"
        :rules="rules || [(v: any) => !!v || `กรุณาระบุวันที่`]"
      >
      </VTextField>
    </template>

    <!-- Date Picker dialog -->
    <v-col
      cols="12"
      align="start"
      class="text-blue"
    >
      <v-date-picker
        class="text-blue card_Date"
        title="ระบุวันที่"
        header="เลือกวันที่"
        color="DarkInfo"
        v-model="selectedDate"
        :min="calculatedMinDate"
        :max="calculatedMaxDate"
      >
        <!-- ปุ่ม Reset อยู่ใน actions -->
        <template v-slot:actions>
          <v-row class="text-primary">
            <v-row
              class="mt-2"
              align="center"
              justify="center"
              v-if="!AllDay"
            >
              <v-col cols="4">
                <label class="mb-2 d-block">ชั่วโมง</label>
                <v-text-field
                  v-model="HourTime"
                  type="text"
                  inputmode="numeric"
                  maxlength="2"
                  placeholder="00 - 23"
                  @input="onHourInput"
                  @blur="formatHour"
                  density="comfortable"
                  dense
                  class="text-center"
                />
              </v-col>

              <v-col
                cols="1"
                class="text-center"
              >
                <div class="text-h4 mt-6">:</div>
              </v-col>

              <v-col cols="4">
                <label class="mb-2 d-block">นาที</label>
                <v-text-field
                  ref="minuteInput"
                  v-model="MinuteTime"
                  type="text"
                  inputmode="numeric"
                  maxlength="2"
                  placeholder="00 - 59"
                  @input="onMinuteInput"
                  @blur="formatMinute"
                  class="text-center"
                  density="comfortable"
                />
              </v-col>

              <v-col
                cols="1"
                class="text-center"
              >
                <div class="text-h5 mt-6">น.</div>
              </v-col>
            </v-row>

            <v-col
              cols="12"
              align="end"
            >
              <v-btn
                color="red"
                variant="outlined"
                class="mt-0 bg-error-darken-1"
                @click.stop="resetDate"
                >รีเซ็ต</v-btn
              >
              <v-btn
                color="red"
                variant="outlined"
                class="mt-0 bg-success-darken-2"
                @click.stop="changeTimetoThaiStart(), (dialogShow = false)"
                >ยืนยัน</v-btn
              >
            </v-col>
          </v-row>
        </template>
      </v-date-picker>
    </v-col>
  </v-menu>
</template>

<script lang="ts">
import moment from 'moment'
import 'moment/locale/th'
import { defineComponent, PropType } from 'vue'
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
  name: 'TextFieldDatepicker',
  inheritAttrs: false,
  props: {
    Message: {
      type: String,
    },
    selectedDateTime: {
      type: String as any,
    },
    placeholder: {
      type: String as any,
    },
    variant: {
      type: String,
      default: 'outlined',
    },
    bold: {
      type: Boolean,
    },
    readonly: {
      type: Boolean,
    },
    bg: {
      type: String,
    },
    minDate: {
      type: String as any,
      default: '',
    },
    maxDate: {
      type: String as any,
      default: '',
    },
    AllDay: {
      type: Boolean,
      default: false,
    },
    rules: {
      type: Array as PropType<((v: any) => string | boolean)[]>,
      default: () => [],
    },
  },
  data() {
    return {
      selectedDate: this.selectedDateTime,
      dialogShow: false,
      DateFormatThai: '',
      firstDate: new Date('0001-01-01'),
      dialogTime: false,
      SenddateBack: undefined as any,
      SendTimeBack: undefined as any,
      selectedTime: this.selectedDateTime,
      HourTime: '' as number | string,
      MinuteTime: '' as number | string,
    }
  },
  computed: {
    calculatedMinDate() {
      return moment(this.minDate).format('YYYY-MM-DD')
    },
    calculatedMaxDate() {
      return moment(this.maxDate).subtract(1, 'days').format('YYYY-MM-DD')
    },
  },
  async mounted() {
    if (this.selectedDateTime) {
      this.extractTimeFromDateTime()
      this.changeTimetoThaiStart()
    } else if (!this.selectedDate) {
      this.DateFormatThai = '' as any
    }
  },
  methods: {
    // ฟังก์ชันใหม่: แยกเวลาจาก selectedDateTime
    extractTimeFromDateTime() {
      if (this.selectedDateTime) {
        const dateTime = moment(this.selectedDateTime)
        if (dateTime.isValid()) {
          this.HourTime = dateTime.format('HH')
          this.MinuteTime = dateTime.format('mm')
          // ตั้งค่า selectedDate เป็นวันที่เฉพาะ (ไม่รวมเวลา)
          this.selectedDate = dateTime.format('YYYY-MM-DD')
        }
      }
    },
    changeTimetoThaiStart() {
      moment.locale('th')

      const date = moment(this.selectedDate)

      // แปลงให้เป็นตัวเลข
      const hour = Number(this.HourTime) || 0
      const minute = Number(this.MinuteTime) || 0
      // copy วันแล้ว set เวลาแบบชัดเจน
      let combinedDateTime
      if (this.AllDay) {
        // ถ้าเป็น AllDay ให้ใช้เวลา 00:00
        this.HourTime = '00'
        this.MinuteTime = '00'
        combinedDateTime = moment(date).set('hour', 0).set('minute', 0).set('second', 0)
      } else {
        // ถ้าไม่ใช่ AllDay ให้ใช้เวลาที่ผู้ใช้เลือก
        combinedDateTime = moment(date).set('hour', hour).set('minute', minute).set('second', 0)
      }

      // ใช้ พ.ศ. (ปี +543)
      const buddhistYear = combinedDateTime.year() + 543
      const dateStartThai = this.AllDay
        ? combinedDateTime.format(`วันที่ D MMMM ${buddhistYear}`)
        : combinedDateTime.format(`วันที่ D MMMM ${buddhistYear} เวลา HH:mm น.`)

      if (this.DateFormatThai !== dateStartThai) {
        this.DateFormatThai = dateStartThai

        // ส่ง date ที่รวมเวลาถูกต้องแล้ว
        this.$emit('selectedDateTime', combinedDateTime.toDate())
      }
    },
    onHourInput(e: Event) {
      let val = (e.target as HTMLInputElement).value.replace(/\D/g, '') // ลบ non-digit
      if (val.length > 2) val = val.slice(0, 2)
      this.HourTime = val

      // ถ้าครบ 2 หลักให้ focus ไปช่องนาที
      if (val.length === 2) {
        this.$nextTick(() => {
          ;(this.$refs.minuteInput as HTMLInputElement)?.focus()
        })
      }
    },
    onMinuteInput(e: Event) {
      let val = (e.target as HTMLInputElement).value.replace(/\D/g, '')
      if (val.length > 2) val = val.slice(0, 2)
      this.MinuteTime = val
    },
    formatHour() {
      let h = parseInt(String(this.HourTime))
      if (isNaN(h) || h < 0) h = 0
      if (h > 23) h = 23
      this.HourTime = h.toString().padStart(2, '0')
    },
    formatMinute() {
      let m = parseInt(String(this.MinuteTime))
      if (isNaN(m) || m < 0) m = 0
      if (m > 59) m = 59
      this.MinuteTime = m.toString().padStart(2, '0')
    },
    resetDate() {
      this.selectedDate = undefined
      this.DateFormatThai = ''
      this.dialogShow = false
      this.$emit('selectedDateTime', undefined)
    },
  },
  watch: {
    AllDay(newValue) {
      if (this.selectedDateTime || this.DateFormatThai !== '') {
        this.changeTimetoThaiStart()
      }
    },
    // เพิ่ม watcher สำหรับ selectedDateTime
    selectedDateTime: {
      handler(newValue) {
        if (newValue) {
          this.extractTimeFromDateTime()
          this.changeTimetoThaiStart()
        }
      },
      immediate: true,
    },
  },
})
</script>

<style scoped>
.card_Date {
  background-color: #fbfbfb !important;
  border-radius: 12px;
  color: #2b3086 !important;
  border: 1px solid #a6aebf;
  box-shadow: 2px 2px 8px 4px rgba(0, 0, 0, 0.1);
}
</style>
