<script lang="ts">
import trophy from '@images/misc/trophy.png'
import coin from '@/assets/images/logos/coin.png'
import { Client, GetActivityPlanQueryForDashboard } from '@/client'
import { BACKEND_API_URL } from '@/constants'
const client = new Client(BACKEND_API_URL)
export default defineComponent({
  name: 'AnalyticsAward',
  data() {
    return {
      trophy,
      request: new GetActivityPlanQueryForDashboard(),
      activity: {} as any,
      coin,
    }
  },
  async mounted() {
    this.request.type = 'Years'
    await this.initialize()
  },
  methods: {
    async initialize() {
      try {
        const response = await client.getActivityPlanForDashboardQuery(this.request)
        this.activity = response
      } catch (error) {
        console.error('Error during initialization:', error)
      }
    },
  },
})
</script>

<template>
  <VCard class="position-relative">
    <VCardText>
      <div class="mb-2">
        <h5 class="text-h5">
          ยอดค่าใช้จ่ายที่ออกไปพบลูกค้าของปีนี้
          <!-- <span class="text-high-emphasis">🎉</span> -->
        </h5>
        <div class="text-body-1">ยอดทั้งหมด</div>
      </div>
      <h4 class="text-h4 text-primary">
        {{
          activity.cost
            ? activity.cost.toLocaleString(undefined, {
                maximumFractionDigits: 3,
              })
            : '0'
        }}
        บาท
      </h4>
      <!-- <div class="text-body-1 mb-2">78% ของงบประมาณ <span class="text-high-emphasis">🚀</span></div> -->
      <!-- <VBtn size="small" class="mt-5"> ดูรายละเอียด </VBtn> -->
    </VCardText>

    <!-- Trophy -->
    <VImg
      :src="coin"
      class="trophy"
    />
  </VCard>
</template>

<style lang="scss">
.v-card .trophy {
  position: absolute;
  inline-size: 7.188rem;
  inset-block-end: 1.25rem;
  inset-inline-end: 1.25rem;
  pointer-events: none;
}

@media (max-width: 600px) {
  .v-card .trophy {
    inline-size: 6rem;
    inset-block-end: -20px;
    inset-inline-end: 1.5rem;
    padding-right: 3px;
  }
}
</style>
