<template>
  <v-col cols="12">
    <v-card class="page-container scroll-content elevation-0 dialog-scrollbar">
      <!-- card title -->
      <v-row class="mt-3 mb-4">
        <v-col
          cols="12"
          class="d-flex justify-center"
        >
          <span class="text-sub-title text-center">รายละเอียดรายงานสรุปผล</span>
        </v-col>
      </v-row>

      <v-card-text>
        <v-form
          ref="form"
          @submit.prevent="UpdateActivityAndCreatePlan"
          class="text-black"
        >
          <v-row class="px-2 px-md-7">
             <v-col
              cols="12"
              align="end"
              class="d-flex justify-end"
            >
              <span class="mr-2 mr-md-8 text-h6 text-md-h5">แผนก : {{ User.departments?.name }}</span>
            </v-col>

            <!-- BTN Share -->
            <!-- <v-col
              cols="12"
              class="d-flex justify-center"
            >
              <v-btn
                rounded="xl"
                color="error"
                class="mr-2 ml-12"
                @click="openShareDialog"
              >
                <v-icon icon="ri-reply-fill"></v-icon>แชร์
                <v-tooltip
                  activator="parent"
                  location="bottom"
                >
                  แชร์
                </v-tooltip>
              </v-btn>
            </v-col> -->

            <VCol cols="12">
              <GetCustomerAppointmentPlan
                v-if="updateCommand.id"
                :ActivityPlan="updateCommand"
                :PlanNote="planNoteCommand"
              />
            </VCol>

            <!-- อัพโหลดภาพ -->
            <v-col cols="12">
              <v-card
                class="document-card px-4 px-md-6 py-4 py-md-6 mb-4 mb-md-6"
                border="grey-500 opacity-50 sm"
                elevation="0"
              >
                <v-row class="align-center mb-0">
                  <!-- Title -->
                  <v-col
                    cols="12"
                    md="6"
                    class="d-flex align-center justify-center justify-md-start"
                  >
                    <h2 class="document-title mb-0">เอกสารอ้างอิงอ้างอิง</h2>
                  </v-col>
                </v-row>

                <v-row
                  class="mt-0 mb-0 px-0 text-form carousel-row"
                  align="center"
                  justify="center"
                >
                  <v-col
                    cols="12"
                    class="carousel-col"
                  >
                    <CarouselsImage
                      v-if="currentId"
                      :id="String(currentId)"
                    />
                  </v-col>
                </v-row>
              </v-card>
            </v-col>

            <v-col
              cols="12"
              class="pt-4 pt-md-6 action-btn-row"
            >
              <!-- <div class="button-container">
                <v-btn
                  class="mobile-btn cancel-btn"
                  rounded="lg"
                  color="error"
                  @click="closeDialog"
                  block
                >
                  <v-icon
                    icon="ri-close-line"
                    class="mr-2"
                  ></v-icon>
                  ปิด
                </v-btn>
              </div> -->
            </v-col>
          </v-row>
        </v-form>
      </v-card-text>
    </v-card>

    <!-- Share Dialog -->
    <ShareDialog
      :open="shareDialog"
      :report-title="updateCommand.objective + ' ('+employee.titleName+employee.firstName +' '+ employee.lastName+')'"
      :report-id="currentId"
      @close="closeShareDialog"
      @share="handleShare"
    />
  </v-col>
</template>

<script lang="ts">
import { Client, UpdateActivityPlanCommand, UpdatePlanNoteCommand } from '@/client'
import CarouselsImage from '@/components/CarouselsImage.vue'
import ImportFile from '@/components/Import/ImportFile.vue'
import { BACKEND_API_URL } from '@/constants'
import { useAuthStore, useSweetAlertStore } from '@/stores'
import { appointmentCostRules } from '@/utils/RuleServices'
import { defineComponent } from 'vue'
import GetCustomerAppointmentPlan from './GetCustomerAppointmentPlan.vue'
import ShareDialog from './ShareDialog.vue'
const client = new Client(BACKEND_API_URL)

export default defineComponent({
  name: 'CustomerAppointmentByIdDetailView',
  props: {
    id: {
      type: [String, Number],
      required: false,
    },
  },
  emits: ['close'],
  components: {
    ImportFile,
    GetCustomerAppointmentPlan,
    CarouselsImage,
    ShareDialog,
  },
  data() {
    return {
      formattedCost: '',
      updateCommand: new UpdateActivityPlanCommand(),
      expensesOption: '',
      amount: null,
      expenseDetail: '',
      planNoteCommand: new UpdatePlanNoteCommand(),
      currentId: '' as any,
      sweetAlertStore: useSweetAlertStore(),
      appointmentCostRules,
      auth: useAuthStore(),
      User: {} as any, // Will be populated with EmployeeDto
      shareDialog: false,
      employee:{} as any,
    }
  },
  async mounted() {
    // ใช้ props id ถ้ามี หรือใช้ route params
    this.currentId = this.id || (this.$route.params.id as any)
    if (this.currentId) {
      await this.initialize()
    }
  },
  methods: {
    async initialize() {
      try {
        const response = await client.getActivityPlanQueryByID(this.currentId)
        this.updateCommand = { ...response } as UpdateActivityPlanCommand
        this.employee = response.employees
        this.formattedCost = this.updateCommand.cost ? this.formatNumber(this.updateCommand.cost) : ''

        const result = await client.getPlanNoteQueryByActivityPlanId(String(this.currentId))
        if (result) {
          this.planNoteCommand = { ...result } as UpdatePlanNoteCommand
        } else {
          this.planNoteCommand = new UpdatePlanNoteCommand()
        }

        // Fetch User data using auth.userId
        if (this.auth.userId) {
          const result = await client.getEmployeeQueryByUserID(this.auth.userId)
          if (result) {
            this.User = result
          } else {
            console.error('Failed to fetch employee data')
          }
        } else {
          console.error('userId is missing in auth store')
        }
      } catch (error) {
        console.error(error)
      }
    },
    closeDialog() {
      this.$emit('close')
    },
    GoCancel() {
      // เมื่อใช้ใน dialog ให้ปิด dialog แทน
      if (this.id) {
        this.$emit('close')
      } else {
        this.$router.push({ name: 'ReportCustomerAppointmentPlanListView' })
      }
    },
    async UpdateActivityAndCreatePlan() {
      const form = this.$refs.form as any
      const { valid } = await form.validate()
      if (valid) {
        try {
          const response = await client.updateActivityPlan(this.updateCommand)
          if (response) {
            const result = await client.updatePlanNote(this.planNoteCommand)
            if (result) {
              this.sweetAlertStore.successDeleted('สร้างแผนสรุปสำเร็จ')
              setTimeout(() => {
                this.$router.push({ name: 'CustomerAppointmentListView' })
              }, 1500)
            }
          }
        } catch (error) {
          console.error(error)
        }
      }
    },
    formatNumber(value: any) {
      if (value == null || value === '') return ''
      const number = parseFloat(value)
      return isNaN(number) ? '' : number.toLocaleString('en-US')
    },
    onInputCost(event: any) {
      const rawValue = event.target.value.replace(/,/g, '')
      if (!isNaN(rawValue)) {
        this.updateCommand.cost = parseFloat(rawValue)
        this.formattedCost = rawValue
      }
    },
    formatCost() {
      this.formattedCost = this.formatNumber(this.updateCommand.cost)
    },
    openShareDialog() {
      this.shareDialog = true
    },
    closeShareDialog() {
      this.shareDialog = false
    },
    handleShare(shareData: any) {
      // Handle share logic here or pass to API
      console.log('Share data received from dialog:', shareData)
    },
  },
})
</script>

<style scoped>
.scroll-wrapper {
  max-height: 90vh;
  overflow: hidden;
}
.page-container {
  width: 100%;
  min-height: 100vh;
  overflow-y: auto;
  overflow-x: hidden;
  scroll-behavior: smooth;
  position: relative;
}

/* Scrollable area with fixed max-height, no flex grow */
.scroll-content {
  overflow-y: auto;
  overflow-x: hidden;
  max-height: 70vh;
  -webkit-overflow-scrolling: touch;
  overscroll-behavior: contain;
}

.scroll-content {
  overflow-y: auto;
  overflow-x: hidden;
  max-height: calc(90vh - 120px);
}

.text-sub-title {
  font-size: clamp(20px, 5vw, 35px);
  font-weight: bold;
  color: #2b3086;
  background-clip: text;
  text-shadow: 5px 5px 8px rgba(65, 69, 92, 0.447);
  line-height: 1.2;
}

.text-black {
  color: #1b1a1a;
}

.document-card {
  border-radius: 12px;
  transition: all 0.3s ease;
}

.document-title {
  font-size: clamp(16px, 3vw, 20px);
  color: #2b3086;
  font-weight: 600;
}

.carousel-row {
  margin-top: 16px;
}

.carousel-col {
  margin-bottom: 0;
}

/* Button Styles */
.button-container {
  display: flex;
  justify-content: center;
  width: 100%;
  max-width: 300px;
  margin: 0 auto;
}

.mobile-btn {
  min-width: 120px;
  height: 44px;
  font-size: 16px;
  font-weight: 500;
  border: none !important;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.15);
  transition: all 0.3s ease;
}

.mobile-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 12px rgba(0, 0, 0, 0.2);
}

/* Mobile: <= 599px */
@media (max-width: 599px) {
  .scroll-wrapper {
    max-height: 85vh;
    padding: 0;
  }

  .scroll-content {
    max-height: calc(85vh - 100px);
  }

  .text-sub-title {
    font-size: 30px;
    text-align: center;
  }

  .document-card {
    padding: 12px !important;
    margin-bottom: 16px !important;
    border-radius: 8px;
  }

  .document-title {
    font-size: 16px;
    text-align: center;
  }

  .carousel-row {
    margin-top: 12px;
  }

  .mobile-btn {
    width: 100% !important;
    min-width: 100% !important;
    min-height: 56px;
    font-size: 16px;
    border-radius: 14px;
    margin-bottom: 0 !important;
    font-weight: 600;
    letter-spacing: 0.025em;
    text-transform: none;
    box-shadow: 0 4px 16px rgba(0, 0, 0, 0.1), 0 1px 4px rgba(0, 0, 0, 0.05);
    position: relative;
    overflow: hidden;
  }
  .mobile-btn::before {
    content: '';
    position: absolute;
    top: 0;
    left: -100%;
    width: 100%;
    height: 100%;
    background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.2), transparent);
  }
  .mobile-btn:active {
    transform: translateY(-1px);
    transition-duration: 0.1s;
  }

  .button-container {
    max-width: 100%;
  }

  .action-btn-row {
    margin-bottom: 30% !important;
  }

  /* ปรับขนาดฟอนต์ข้อมูลแผนก */
  .text-h6 {
    font-size: 14px !important;
  }
}

/* Tablet: 600px - 959px */
@media (min-width: 600px) and (max-width: 959px) {
  .scroll-wrapper {
    max-height: 88vh;
    padding: 0;
  }

  .scroll-content {
    max-height: calc(88vh - 110px);
  }

  .card-form {
    margin: 5px 8px;
    border-radius: 12px;
  }

  .text-sub-title {
    font-size: 28px;
    padding: 6px 10px;
    text-align: center;
  }

  .document-card {
    padding: 20px !important;
    margin-bottom: 20px !important;
  }

  .document-title {
    font-size: 18px;
  }

  .mobile-btn {
    min-width: 140px;
    width: 70%;
    max-width: 250px;
  }

  .button-container {
    max-width: 350px;
  }
}

/* Desktop: >= 960px */
@media (min-width: 960px) {
  .scroll-wrapper {
    max-height: 90vh;
    padding: 0;
  }

  .scroll-content {
    max-height: calc(90vh - 120px);
  }

  .card-form {
    margin: 10px 20px;
    border-radius: 15px;
  }

  .text-sub-title {
    font-size: 35px;
    padding: 0;
    text-align: left;
  }

  .document-card {
    padding: 24px !important;
    margin-bottom: 24px !important;
  }

  .document-title {
    font-size: 20px;
  }

  .mobile-btn {
    min-width: 150px;
    width: 50%;
    max-width: 220px;
  }

  .button-container {
    max-width: 400px;
  }
}

/* Large Desktop: >= 1200px */
@media (min-width: 1200px) {
  .card-form {
    margin: 15px 30px;
    max-width: 1200px;
    margin-left: auto;
    margin-right: auto;
  }

  .document-card {
    max-width: 1000px;
    margin-left: auto;
    margin-right: auto;
  }
}

/* Extra small devices adjustments */
@media (max-width: 375px) {
  .text-sub-title {
    font-size: 18px;
  }

  .card-form {
    margin: 3px 2px;
    border-radius: 8px;
  }

  .document-card {
    padding: 8px !important;
    margin-bottom: 12px !important;
  }

  .document-title {
    font-size: 14px;
  }

  .mobile-btn {
    min-height: 44px;
    font-size: 15px;
  }
}

/* Landscape mobile orientation */
@media (max-width: 959px) and (orientation: landscape) {
  .scroll-wrapper {
    max-height: 80vh;
  }

  .scroll-content {
    max-height: calc(80vh - 80px);
  }

  .text-sub-title {
    font-size: clamp(16px, 4vw, 24px);
  }
}
</style>
