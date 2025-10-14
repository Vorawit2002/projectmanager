<template>
  <VCard>
    <VCardItem>
      <VCardTitle
        >วัตถุประสงค์ของการนัดหมาย {{ moreList.find((x: any) => x.value === request.type)?.title||'' }}</VCardTitle
      >

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
        v-if="chartSeries.length > 0 && chartSeries.every(val => Number.isFinite(val))"
        type="donut"
        :options="computedChartOptions"
        :series="chartSeries"
        :height="350"
      />

      <div
        v-else
        class="text-center text-subtitle-1"
      >
        ไม่มีข้อมูลแสดงผล
      </div>

      <!-- <VBtn class="mt-4" block>รายละเอียด</VBtn> -->
    </VCardText>
  </VCard>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import { Client, GetActivityPlanQueryForDashboard } from '@/client'
import { BACKEND_API_URL } from '@/constants'
const client = new Client(BACKEND_API_URL)
export default defineComponent({
  name: 'AppointmentPurpose',
  data() {
    return {
      moreList: [
        { title: 'ไตรมาสนี้', value: 'Quarter' },
        { title: 'ปีนี้', value: 'Years' },
        { title: '5 ปีย้อนหลัง', value: '5YearAgo' },
      ],
      Activity: [] as any,
      titleColors: {
        'ไปร่วมจัดอบรม / ตรวจรับงาน': '#4E71FF', // Vivid Blue
        'ประชุมงาน / โครงการ': '#56CA00', // Bright Green
        'ทานข้าว': '#FFB400', // Vibrant Orange
        'นำของไปฝาก': '#2b3086', // Vivid Purple
        'ไปสัญจร / ไปต่างจังหวัด': '#0F766E', // Bright Yellow
        'ไปต่างประเทศ / ไปดูงาน': '#EF4444', // Vivid Pink
        'อื่น ๆ': '#7E4EE6', // Bright Cyan
      } as { [key: string]: string },
      chartLabels: [] as string[],
      chartColors: [] as string[],
      chartSeries: [] as number[],
      request: new GetActivityPlanQueryForDashboard(),
    }
  },

  async mounted() {
    this.request.type = 'Quarter'
    await this.getActivity()
  },
  computed: {
    computedChartOptions() {
      return {
        chart: {
          type: 'donut',
        },
        labels: this.chartLabels, // ใช้ตัวแปรแยก
        colors: this.chartColors, // ใช้ตัวแปรแยก
        legend: {
          position: 'bottom',
        },
        dataLabels: {
          enabled: true,
        },
      }
    },
  },
  methods: {
    generateDonutData() {
      const labels: string[] = []
      const series: number[] = []
      const colors: string[] = []

      this.Activity.objectives?.forEach((item: any) => {
        const label = item.name ?? 'ไม่ระบุ'
        const rawCount = Number(item.count)
        const count = isFinite(rawCount) ? rawCount : 0

        labels.push(label)
        series.push(count)
        colors.push(this.titleColors[item.name] || '#ccc')
      })

      const total = series.reduce((sum, val) => sum + val, 0)

      this.chartLabels = total > 0 ? labels : ['ไม่มีข้อมูล']
      this.chartColors = total > 0 ? colors : ['#ccc']
      this.chartSeries = total > 0 ? series : [1]
    },
    async getActivity() {
      try {
        const response = await client.getActivityPlanForDashboardQuery(this.request)
        this.Activity = response
        if (!Array.isArray(response.objectives) || response.objectives.length === 0) {
          this.chartLabels = ['ไม่มีข้อมูล']
          this.chartColors = ['#ccc']
          this.chartSeries = [1] // ไม่ควรเป็น []
          return
        }
        this.generateDonutData()
      } catch (error) {
        console.error(error)
        this.chartLabels = ['ไม่มีข้อมูล']
        this.chartColors = ['#ccc']
        this.chartSeries = [1]
      }
    },
    async getNewData(value: any) {
      this.request.type = value
      await this.getActivity()
    },
  },
})
</script>

<style lang="scss" scoped>
.card-list {
  --v-card-list-gap: 1.5rem;
}
</style>
