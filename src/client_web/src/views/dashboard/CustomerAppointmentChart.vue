<template>
  <VCard>
    <VCardItem>
      <VCardTitle
        >{{
          displayType === 'All'
            ? 'เปรียบเทียบรายการนัดหมายลูกค้า'
            : displayType === 'CurrentOnly'
            ? 'รายการนัดหมายลูกค้าในปีนี้'
            : 'รายการนัดหมายลูกค้าในปีก่อน'
        }}
      </VCardTitle>

      <template #append>
        <div class="me-n3">
          <MoreBtn
            :menu-list="moreList"
            @Types="getNewData"
          />
        </div>
      </template>
    </VCardItem>

    <VCardText>
      <VueApexCharts
        type="line"
        :options="chartOptions"
        :series="chartSeries"
        :height="350"
      />

      <div class="d-flex align-center justify-space-between mb-5">
        <div class="d-flex align-center gap-x-4">
          <h4
            v-if="displayType === 'All' || displayType === 'CurrentOnly'"
            class="text-h5"
          >
            ปีนี้: {{ totalCurrentYear }} รายการ
          </h4>
          <h4
            v-if="displayType === 'All' || displayType === 'OldOnly'"
            class="text-h5 text-grey"
          >
            ปีก่อน: {{ totalOldYear }} รายการ
          </h4>
        </div>

        <!-- Legend -->
        <div class="d-flex align-center gap-x-4">
          <div
            v-if="displayType === 'All' || displayType === 'CurrentOnly'"
            class="d-flex align-center gap-x-2"
          >
            <div class="legend-dot current-year"></div>
            <span class="text-sm">ปีนี้ (Column)</span>
          </div>
          <div
            v-if="displayType === 'All' || displayType === 'OldOnly'"
            class="d-flex align-center gap-x-2"
          >
            <div class="legend-dot old-year"></div>
            <span class="text-sm">ปีก่อน (Column)</span>
          </div>
          <!-- <div v-if="displayType === 'All'" class="d-flex align-center gap-x-2">
            <div class="legend-dot trend-line"></div>
            <span class="text-sm">เทรนด์ (Line)</span>
          </div> -->
        </div>
      </div>

      <!-- <VBtn block> ดูรายละเอียด </VBtn> -->
    </VCardText>
  </VCard>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import moment from 'moment'
import { useTheme } from 'vuetify'
import { hexToRgb } from '@layouts/utils'
import { BACKEND_API_URL } from '@/constants'
import { ActivityPlanYearsDto, Client } from '@/client'

const client = new Client(BACKEND_API_URL)

export default defineComponent({
  name: 'CustomerAppointmentChart',

  data() {
    const vuetifyTheme = useTheme()
    return {
      vuetifyTheme,
      months: ['ม.ค.', 'ก.พ.', 'มี.ค.', 'เม.ย.', 'พ.ค.', 'มิ.ย.', 'ก.ค.', 'ส.ค.', 'ก.ย.', 'ต.ค.', 'พ.ย.', 'ธ.ค.'],
      monthKeys: ['jan', 'feb', 'mar', 'apr', 'may', 'jun', 'jul', 'aug', 'sep', 'oct', 'nov', 'dec'] as any,
      moreList: [
        { title: 'เปรียบเทียบ', value: 'All' },
        { title: 'เฉพาะปีนี้', value: 'CurrentOnly' },
        { title: 'เฉพาะปีก่อน', value: 'OldOnly' },
      ],
      activityData: {} as any,
      displayType: 'All' as string,
      currentYearData: Array(12).fill(0) as number[],
      oldYearData: Array(12).fill(0) as number[],
      type: '' as any,
    }
  },

  computed: {
    totalCurrentYear(): number {
      return this.currentYearData.reduce((sum, val) => sum + val, 0)
    },

    totalOldYear(): number {
      return this.oldYearData.reduce((sum, val) => sum + val, 0)
    },

    // คำนวณเทรนด์ไลน์จากข้อมูลทั้งสองปี
    // trendLineData(): number[] {
    //   return this.currentYearData.map((current, index) => {
    //     const old = this.oldYearData[index] || 0
    //     return Math.round((current + old) / 2 * 1.2) // เพิ่ม multiplier เพื่อให้เห็นเทรนด์ชัดขึ้น
    //   })
    // },

    chartOptions() {
      const currentTheme = this.vuetifyTheme.current.colors
      const variableTheme = this.vuetifyTheme.current.variables as any

      const disabledColor = `rgba(${hexToRgb(currentTheme['on-surface'])}, 0.6)`
      const borderColor = `rgba(${hexToRgb(variableTheme['border-color'])}, 0.12)`

      return {
        chart: {
          height: 350,
          type: 'line',
          stacked: false,
          toolbar: { show: false },
          zoom: { enabled: false },
        },
        dataLabels: {
          enabled: true,
        },
        stroke: this.getStrokeConfig(),
        colors: this.getChartColors(),
        title: {
          text: 'รายการนัดหมายลูกค้า - การเปรียบเทียบรายเดือน',
          align: 'left',
          offsetX: 0,
          style: {
            fontSize: '16px',
            fontWeight: 600,
            color: disabledColor,
          },
        },
        legend: {
          show: true,
          horizontalAlign: 'left',
          offsetX: 0,
          offsetY: 10,
        },
        grid: {
          borderColor,
          strokeDashArray: 7,
          xaxis: { lines: { show: false } },
          yaxis: { lines: { show: true } },
          padding: {
            top: 0,
            right: 0,
            bottom: 0,
            left: 0,
          },
        },
        xaxis: {
          categories: this.months,
          tickPlacement: 'on',
          labels: {
            style: {
              colors: disabledColor,
              fontSize: '13px',
              fontFamily: 'inherit',
            },
          },
          crosshairs: { opacity: 0 },
          axisTicks: { show: false },
          axisBorder: { show: false },
        },
        yaxis: this.getYAxisConfig(disabledColor),
        tooltip: {
          fixed: {
            enabled: true,
            position: 'topLeft',
            offsetY: 30,
            offsetX: 60,
          },
          shared: true,
          intersect: false,
          y: {
            formatter: (val: number, { seriesIndex }: any) => {
              const suffixes = [
                ' รายการ (ปีนี้)',
                ' รายการ (ปีก่อน)',
                // ' รายการ (เทรนด์)'
              ]
              return `${val}${suffixes[seriesIndex] || ' รายการ'}`
            },
          },
        },
        markers: {
          size: [0, 0, 6], // ไม่แสดง marker สำหรับ column, แสดงสำหรับ line
          hover: {
            size: [6, 6, 8],
          },
        },
      }
    },

    chartSeries() {
      const sanitizeData = (arr: number[]) => {
        return arr.map(v => {
          if (v === null || v === undefined || !isFinite(v) || isNaN(v)) {
            return 0
          }
          return Number(v)
        })
      }

      const currentData = sanitizeData(this.currentYearData)
      const oldData = sanitizeData(this.oldYearData)

      // console.log('Sanitized currentYearData:', currentData)
      // console.log('Sanitized oldYearData:', oldData)

      const series = []

      switch (this.displayType) {
        case 'All':
          series.push(
            {
              name: 'ปีนี้',
              type: 'column',
              data: currentData,
              // ลบ yAxis ออก - ใช้ default axis
            },
            {
              name: 'ปีก่อน',
              type: 'column',
              data: oldData,
              // ลบ yAxis ออก - ใช้ default axis
            },
          )
          break

        case 'CurrentOnly':
          series.push({
            name: 'ปีนี้',
            type: 'column',
            data: currentData,
          })
          break

        case 'OldOnly':
          series.push({
            name: 'ปีก่อน',
            type: 'column',
            data: oldData,
          })
          break

        default:
          series.push(
            {
              name: 'ปีนี้',
              type: 'column',
              data: currentData,
            },
            {
              name: 'ปีก่อน',
              type: 'column',
              data: oldData,
            },
          )
      }

      return series
    },
  },

  methods: {
    getStrokeConfig() {
      switch (this.displayType) {
        case 'All':
          return { width: [1, 1, 4] } // column, column, line
        case 'CurrentOnly':
        case 'OldOnly':
          return { width: [1] } // column only
        default:
          return { width: [1, 1, 4] }
      }
    },

    getYAxisConfig(disabledColor: string) {
      const safeMax = (arr: number[]) => {
        const validNumbers = arr.filter(n => {
          return typeof n === 'number' && isFinite(n) && !isNaN(n) && n !== null && n !== undefined
        })

        if (validNumbers.length === 0) return 10

        const max = Math.max(...validNumbers)
        return isFinite(max) ? max : 10
      }

      const currentMax = safeMax(this.currentYearData)
      const oldMax = safeMax(this.oldYearData)
      const maxY = Math.max(currentMax, oldMax)

      // เพิ่ม buffer 20% และต้อง finite
      const finalMax = isFinite(maxY) && maxY > 0 ? Math.ceil(maxY * 1.2) : 10

      // console.log('Y-axis calculation:', {
      //   currentMax,
      //   oldMax,
      //   maxY,
      //   finalMax,
      //   currentData: this.currentYearData,
      //   oldData: this.oldYearData,
      // })

      // ใช้ Single Y-Axis เสมอเพื่อหลีกเลี่ยงปัญหา
      return {
        min: 0,
        max: finalMax,
        show: true,
        tickAmount: 5,
        forceNiceScale: true, // บังคับให้ ApexCharts ปรับ scale ให้สวย
        labels: {
          style: {
            colors: disabledColor,
            fontSize: '13px',
            fontFamily: 'inherit',
          },
          formatter: (val: number) => {
            if (!isFinite(val) || isNaN(val)) return '0'
            return Math.round(val).toString()
          },
        },
        title: {
          text: 'จำนวนรายการ',
          style: {
            color: disabledColor,
            fontSize: '12px',
            fontWeight: 600,
          },
        },
      }
    },

    getChartColors() {
      switch (this.displayType) {
        case 'CurrentOnly':
          return ['#2b3086']
        case 'OldOnly':
          return ['#94a3b8']
        case 'All':
        default:
          return ['#2b3086', '#94a3b8', '#FEB019'] // เพิ่มสีสำหรับ trend line
      }
    },
    convertDtoToArray(dto: any): number[] {
      if (!dto) return Array(12).fill(0)

      const arr = this.monthKeys.map((key: string) => {
        const value = dto[key]

        // ตรวจสอบและแปลงค่าอย่างละเอียด
        if (value === null || value === undefined) return 0

        const num = Number(value)

        if (isNaN(num) || !isFinite(num)) return 0

        return Math.max(0, num) // ป้องกันค่าติดลบ
      })

      // ตรวจสอบผลลัพธ์อีกครั้ง
      const validatedArr = arr.map((val: number) => (isFinite(val) ? val : 0))

      return validatedArr.length === 12 ? validatedArr : Array(12).fill(0)
    },
    async generateMonthlyActivityData() {
      try {
        const response = await client.getActivityPlanQueryWithYears()
        // console.log('API Response:', response)

        this.activityData = response

        // แปลง DTO เป็น Array สำหรับแต่ละปี พร้อมการตรวจสอบเพิ่มเติม
        this.currentYearData = this.convertDtoToArray(response?.currentYears)
        this.oldYearData = this.convertDtoToArray(response?.oldYears)

        // ตรวจสอบผลลัพธ์สุดท้าย
        // console.log('Final currentYearData:', this.currentYearData)
        // console.log('Final oldYearData:', this.oldYearData)

        // ตรวจสอบว่ามีค่าที่ไม่ valid หรือไม่
        const hasInvalidCurrent = this.currentYearData.some(val => !isFinite(val))
        const hasInvalidOld = this.oldYearData.some(val => !isFinite(val))

        if (hasInvalidCurrent || hasInvalidOld) {
          // console.warn('Found invalid data, using fallback')
          this.currentYearData = [2, 3, 2, 3, 1, 1, 1, 1, 1, 1, 1, 1]
          this.oldYearData = [1, 2, 1, 2, 2, 0, 2, 1, 0, 1, 0, 2]
        }
      } catch (error) {
        // console.error('Error fetching activity data:', error)

        // Fallback data สำหรับการทดสอบ
        this.currentYearData = [2, 3, 2, 3, 1, 1, 1, 1, 1, 1, 1, 1]
        this.oldYearData = [1, 2, 1, 2, 2, 0, 2, 1, 0, 1, 0, 2]
      }
    },

    async getNewData(value: string) {
      this.displayType = value

      // ถ้าต้องการโหลดข้อมูลใหม่ตาม type ที่เลือก
      // await this.generateMonthlyActivityData()
    },
  },

  mounted() {
    this.generateMonthlyActivityData()
  },
})
</script>

<style scoped>
.legend-dot {
  width: 12px;
  height: 12px;
  border-radius: 50%;
}

.legend-dot.current-year {
  background-color: #2b3086;
}

.legend-dot.old-year {
  background-color: #94a3b8;
}

.legend-dot.trend-line {
  background-color: #feb019;
}
</style>
