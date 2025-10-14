<template>
  <v-dialog
    v-model="dialogShow"
    max-width="300px"
    class="text-blue"
  >
    <template v-slot:activator="{ on, atts }: any">
      <h3 v-if="bold">{{ Message }}</h3>
      <span v-else>{{ Message }}</span>

      <!-- TextField to display formatted date in Thai with placeholder -->
      <v-textarea
        v-if="readonly"
        :readonly
        class="text-blue"
        variant="outlined"
        density="comfortable"
        dense
        color="purple"
        bg-color="white"
        rounded="md"
        v-bind="atts"
        v-on="on"
          :value="Array.isArray(DateFormatThai) ? DateFormatThai.join(', ') : DateFormatThai || ''"
        v-model="selectedDate"
        :placeholder="placeholder"
         rows="1"
      >
      </v-textarea>

      <v-textarea
        v-else
        class="text-blue"
        v-bind="atts"
        v-on="on"
        :rules="rules || [(v: any) => !!v || `กรุณาระบุวันที่`]"
          :value="Array.isArray(DateFormatThai) ? DateFormatThai.join(', ') : DateFormatThai || ''"
        v-model="selectedDate"
        @click="dialogShow = true"
        variant="outlined"
        density="comfortable"
        dense
        color="purple"
        bg-color="white"
        rounded="md"
        :placeholder="placeholder"
        rows="1"
      >
      </v-textarea>
    </template>

    <!-- Date Picker dialog -->
    <v-col
      cols="auto"
      align="center"
      class="text-blue"
    >
      <v-date-picker
        class="text-blue"
        title="ระบุวันที่"
        header="เลือกวันที่"
        color="DarkInfo"
        v-model="selectedDate"
        :min="calculatedMinDate"
        :max="calculatedMaxDate"
        multiple
      >
        <!-- ปุ่ม Reset อยู่ใน actions -->
        <template v-slot:actions>
          <v-btn
            color="red"
            variant="outlined"
            class="mt-2 bg-error"
            @click.stop="resetDate"
            >รีเซ็ต</v-btn
          >
             <v-btn
            color="red"
            variant="outlined"
            class="mt-2 bg-success"
            @click.stop="changeTimetoThaiStart() , dialogShow = false"
            >บันทึก</v-btn
          >
        </template>
      </v-date-picker>
    </v-col>
  </v-dialog>
  <!-- <v-dialog
    v-model="dialogTime"
    max-width="300px"
    class="text-blue"
    persistent
  >
    <v-col
      cols="6"
      align="center"
      class="text-blue"
    >
      <v-time-picker
        v-model="selectedTime"
        elevation="15"
        format="24hr"
      >
        <template v-slot:actions>
          <v-btn
            color="error"
            variant="tonal"
            class="mt-2 mr-2"
            @click.stop="dialogTime = false"
            >ย้อนกลับ</v-btn
          >
          <v-btn
            color="success"
            variant="tonal"
            class="mt-2"
            @click.stop="changeTimetoThaiTime"
            >ตกลง</v-btn
          >
        </template></v-time-picker
      >
    </v-col>
  </v-dialog> -->
</template>

<script lang="ts">
import moment from 'moment'
import { defineComponent } from 'vue'
moment.locale('th')
import 'moment/locale/th'
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
    rules: {
      type: Array,
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
    }
  },
  computed: {
    calculatedMinDate() {
      return moment(this.minDate).add(1, 'days').format('YYYY-MM-DD')
    },
    calculatedMaxDate() {
      return moment(this.maxDate).subtract(1, 'days').format('YYYY-MM-DD')
    },
  },
  async mounted() {
    if (this.selectedDateTime) {
      this.changeTimetoThaiStart()
    } else if (!this.selectedDate) {
      this.DateFormatThai = '' as any
    }
  },
  methods: {
   changeTimetoThaiStart() {
  moment.locale('th')
  
  if (Array.isArray(this.selectedDate)) {
    const thaiDates = this.selectedDate.map(dateStr => {
      const date = moment(dateStr)
      const buddhistYear = date.year() + 543
      return {
        display: date.format(`วันที่ D MMM ${buddhistYear}`),
        iso: date.format('YYYY-MM-DD'),
        dateObject: new Date(date.format('YYYY-MM-DD'))
      }
    })

    this.DateFormatThai = thaiDates.map(d => d.display) as any

    // ส่ง array ของ Date objects กลับไป
    this.$emit('selectedDateTime', thaiDates.map(d => d.dateObject))

  } else if (this.selectedDate) {
    // กรณีไม่ได้เป็น array (เช่นเปลี่ยนกลับไปแบบเดี่ยว)
    const date = moment(this.selectedDate)
    const buddhistYear = date.year() + 543
    const display = date.format(`วันที่ D MMMM ${buddhistYear}`)
    this.DateFormatThai = [display] as any
    this.$emit('selectedDateTime', [new Date(date.format('YYYY-MM-DD'))])
  } else {
    this.DateFormatThai = [] as any
    this.$emit('selectedDateTime', [])
  }
},
    // changeTimetoThaiTime() {
    //   moment.locale('th')
    //   // ตรวจสอบว่าเวลาใน v-time-picker ถูกเลือกแล้ว
    //   if (this.selectedTime) {
    //     // แปลงเวลาจาก v-time-picker เป็น moment object
    //     let time = moment(this.selectedTime, 'HH:mm')
    //     // เพิ่มเวลาลงในวันที่ที่มีอยู่แล้ว
    //     let fullDate = moment(this.SenddateBack).set({
    //       hour: time.hour(),
    //       minute: time.minute(),
    //       second: 0, // สามารถเพิ่มการตั้งค่าค่า second ได้ตามต้องการ
    //     })
    //     let date = moment(this.SenddateBack)
    //     let buddhistYear = date.year() + 543

    //     // แปลงเป็น string ในรูปแบบ 'YYYY-MM-DD HH:mm'
    //     const fullDateTime = fullDate.format('YYYY-MM-DD HH:mm')
    //     // Emit ค่าที่เลือกเวลาไป
    //     this.DateFormatThai = moment(fullDate).format(`วันที่ D MMMM ${buddhistYear} เวลา HH:mm น.`)
    //     this.$emit('selectedDateTime', fullDateTime)
    //     this.SenddateBack = fullDateTime // ถ้าต้องการเก็บแค่วันที่โดยไม่มีเวลา
    //     console.log('Selected Date and Time:', this.SenddateBack)
    //     this.dialogTime = false // ปิด dialog time picker
    //     this.dialogShow = false
    //   }
    // },
    // closeDialog() {
    //   // this.dialogShow = false
    //   this.dialogTime = true
    // },
    resetDate() {
      this.selectedDate = undefined
      this.DateFormatThai = ''
      this.dialogShow = false
      this.$emit('selectedDateTime', undefined)
    },
  },
})
</script>

<style scoped>
.text-blue {
  color: #000 !important;
}
</style>
