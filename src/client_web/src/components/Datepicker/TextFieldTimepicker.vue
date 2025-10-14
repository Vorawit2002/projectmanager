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
      <v-text-field
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
        :value="DateFormatThai || ''"
        v-model="selectedDate"
        :placeholder="placeholder"
        :rules="rules || [(v: any) => !!v || `กรุณาระบุวันที่`]"
      >
      </v-text-field>

      <v-text-field
        v-else
        class="text-blue"
        v-bind="atts"
        v-on="on"
        :rules="rules || [(v: any) => !!v || `กรุณาระบุวันที่`]"
        :value="DateFormatThai || ''"
        v-model="selectedDate"
        @click="dialogShow = true"
        variant="outlined"
        density="comfortable"
        dense
        color="purple"
        bg-color="white"
        rounded="md"
        :placeholder="placeholder"
      >
      </v-text-field>
    </template>

    <!-- Date Picker dialog -->
    <v-col
      cols="auto"
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
  </v-dialog>
  <v-dialog
    v-model="dialogTime"
    max-width="300px"
    class="text-blue"
    persistent
  >
    <v-col
      cols="auto"
      align="center"
      class="text-blue"
    >
    </v-col>
  </v-dialog>
</template>

<script lang="ts">
import moment from 'moment'
import 'moment/locale/th'
import { defineComponent } from 'vue'
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
  props: {
    Message: {
      type: String,
    },
    selectedDateTime: {
      type: String as any,
    },
    // selectedDateTime: {
    //   type: String as any,
    // },
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
  computed: {},
  async mounted() {
    if (this.selectedDateTime) {
      // this.changeTimetoThaiStart()
    } else if (!this.selectedDate) {
      this.DateFormatThai = '' as any
    }
  },

  // async mounted() {
  //   if (this.selectedDateTime) {
  //     // Initialize time display format
  //     moment.locale('th')
  //     const dateStartThai = moment(this.selectedDateTime, 'YYYY-MM-DD HH:mm').format(`เวลา HH:mm น.`)
  //     this.DateFormatThai = dateStartThai
  //     this.SenddateBack = this.selectedDateTime
  //   } else if (!this.selectedDate) {
  //     this.DateFormatThai = '' as any
  //   }
  // },
  methods: {
    changeTimetoThaiStart() {
      moment.locale('th')
      const dateStartThai = moment(this.selectedDate).format(`เวลา HH:mm น.`)
      if (this.DateFormatThai !== dateStartThai) {
        this.DateFormatThai = dateStartThai
        const SenddateBack = moment(this.selectedDate).format('YYYY-MM-DD')
        this.$emit('selectedDateTime', new Date(SenddateBack))
        this.SenddateBack = SenddateBack
      }
    },
    changeTimetoThaiTime() {
      moment.locale('th')
      // ตรวจสอบว่าเวลาใน v-time-picker ถูกเลือกแล้ว
      if (this.selectedTime) {
        // แปลงเวลาจาก v-time-picker เป็น moment object
        let time = moment(this.selectedTime, 'HH:mm')
        // เพิ่มเวลาลงในวันที่ที่มีอยู่แล้ว
        let fullDate = moment(this.SenddateBack).set({
          hour: time.hour(),
          minute: time.minute(),
          second: 0, // สามารถเพิ่มการตั้งค่าค่า second ได้ตามต้องการ
        })
        let date = moment(this.SenddateBack)
        let buddhistYear = date.year() + 543

        // แปลงเป็น string ในรูปแบบ 'YYYY-MM-DD HH:mm'
        const fullDateTime = fullDate.format('YYYY-MM-DD HH:mm')
        // Emit ค่าที่เลือกเวลาไป
        this.DateFormatThai = moment(fullDate).format(` เวลา HH:mm น.`)
        this.$emit('selectedDateTime', fullDateTime)
        this.SenddateBack = fullDateTime // ถ้าต้องการเก็บแค่วันที่โดยไม่มีเวลา
        console.log('Selected Date and Time:', this.SenddateBack)
        this.dialogTime = false // ปิด dialog time picker
        this.dialogShow = false
      }
    },
    closeDialog() {
      // this.dialogShow = false
      this.dialogTime = true
    },
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
