<script lang="ts">
import { defineComponent, nextTick, onMounted, ref } from 'vue'
import { Client, GetActivityPlanQueryForDashboard } from '@/client'
import { BACKEND_API_URL } from '@/constants'
const client = new Client(BACKEND_API_URL)
export default defineComponent({
  name: 'DashboardActivity',

  data() {
    return {
      statistics: [
        {
          title: 'Sales',
          stats: '',
          icon: 'ri-store-3-line',
          color: 'primary',
        },
        {
          title: 'Development',
          stats: '',
          icon: 'ri-code-s-slash-line',
          color: 'success',
        },
        {
          title: 'Product',
          stats: '',
          icon: 'ri-box-3-line',
          color: 'warning',
        },
        {
          title: 'System&Service',
          stats: '',
          icon: 'ri-computer-line',
          color: 'info',
        },
      ],
      moreList: [
        { title: 'ไตรมาสนี้', value: 'Quarter' },
        { title: 'ปีนี้', value: 'Years' },
        { title: 'ปีก่อนหน้านี้', value: 'YearAgo' },
      ],
      request: new GetActivityPlanQueryForDashboard(),
      activity: {} as any,
    }
  },
  async mounted() {
    this.request.type = 'Quarter'
    await this.getActivity()
  },
  methods: {
    async getActivity() {
      try {
        const response = await client.getActivityPlanForDashboardQuery(this.request)
        console.log(response)
        this.activity = response
        this.statistics.forEach((x: any) => {
          switch (x.title) {
            case 'Sales':
              x.stats = response.countSale
              break
            case 'Development':
              x.stats = response.countDevelopment
              break
            case 'Product':
              x.stats = response.countProduct
              break
            case 'System&Service':
              x.stats = response.countSystemService
              break
          }
        })
      } catch (error) {
        console.error(error)
      }
    },
    async getNewData(value: any) {
      this.request.type = value
      await this.getActivity()
    },
  },
})
</script>

<template>
  <VCard :title="'จำนวนการนัดหมายแต่ละแผนกใน'+ moreList.find((x: any) => x.value === request.type)?.title||''">
    <template #subtitle>
      <p class="text-body-1 mb-0">
        <span class="d-inline-block font-weight-medium text-high-emphasis"
          >เพิ่มมากขึ้น {{ activity.percent }} %
          <v-icon
            class="mr-0"
            color="success"
            icon="ri-arrow-up-line"
          />
          ไตรมาสนี้
        </span>
      </p>
    </template>

    <template #append>
      <MoreBtn
        :menu-list="moreList"
        @Types="getNewData"
      />
    </template>

    <VCardText class="pt-10">
      <VRow>
        <VCol
          v-for="item in statistics"
          :key="item.title"
          cols="12"
          sm="6"
          md="3"
        >
          <div class="d-flex align-center gap-x-3">
            <VAvatar
              :color="item.color"
              rounded
              size="40"
              class="elevation-2"
            >
              <VIcon
                size="24"
                :icon="item.icon"
              />
            </VAvatar>

            <div class="d-flex flex-column">
              <div class="text-body-1">
                {{ item.title }}
              </div>
              <h5 class="text-h5">
                {{ item.stats }}
              </h5>
            </div>
          </div>
        </VCol>
      </VRow>
    </VCardText>
  </VCard>
</template>
