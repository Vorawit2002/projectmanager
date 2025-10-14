<template>
  <VCard class="page-container scroll-content dialog-scrollbar">
    <VCardItem class="elevation-0 mb-2 mt-2 pa-2 pa-md-3">
      <div>
        <h5 class="text-sub-title">รายละเอียดนัดหมาย</h5>
      </div>
    </VCardItem>

    <VCardText>
      <VRow>
        <VCol
          cols="12"
          md="12"
        >
          <div class="activity-details-section">
            <GetActivityById
              v-if="updateCommand.id"
              :ActivityPlan="updateCommand"
              :PlanNote="planNoteCommand"
              :CloseDialogDetail="closeDialog"
              @close="closeDialog"
              class="activity-plan-card"
            />
          </div>
        </VCol>

        <!-- <VCol
          cols="12"
          md="12"
        >
          <div class="management-panel">
            <VCard
              class="management-card"
              elevation="0"
            >
              Tabs Section
              <VCardText class="px-0 py-4">
                <VTabs
                  v-model="activeTab"
                  slider-color="primary"
                >
                  <VTab
                    v-for="item in tabs.filter(t => !t.hidden)"
                    :key="item.tab"
                    :value="item.tab"
                  >
                    <VIcon
                      size="20"
                      :icon="item.icon"
                      class="me-2"
                    />
                    {{ item.title }}
                  </VTab>
                </VTabs>

                Tab Content
                <VWindow
                  v-model="activeTab"
                  :touch="false"
                  class="mt-5"
                >
                  <VWindowItem value="EditActivity">
                    <UpdateActivityDetail
                      :update-command="updateCommand"
                      :condition-readonly="ConditionReadonly"
                      @update:update-command="Object.assign(updateCommand, $event)"
                    />
                  </VWindowItem>
                  <VWindowItem value="EditReport">
                    <UpdateAppointmentOutcome
                      :update-command="updateCommand"
                      :plan-note-command="planNoteCommand"
                      :condition-readonly="ConditionReadonly"
                      :is-dialog="true"
                      :activity-plan-id="String(id)"
                      @updated="handleAppointmentOutcomeUpdated"
                      @cancel="closeDialog"
                      @update:update-command="Object.assign(updateCommand, $event)"
                      @update:plan-note-command="Object.assign(planNoteCommand, $event)"
                    />
                  </VWindowItem>
                  <VWindowItem value="CrateReport">
                    <CreateActivityPlan
                      :update-command="updateCommand"
                      :plan-note-command="planNoteCommand"
                      :condition-readonly="ConditionReadonly"
                      :is-dialog="true"
                      :activity-plan-id="String(id)"
                      @updated="handleCreateActivityPlanUpdated"
                      @cancel="closeDialog"
                      @update:update-command="Object.assign(updateCommand, $event)"
                      @update:plan-note-command="Object.assign(planNoteCommand, $event)"
                    />
                  </VWindowItem>
                </VWindow>
              </VCardText>
            </VCard>
          </div>
        </VCol> -->
      </VRow>
    </VCardText>
  </VCard>
</template>

<script lang="ts">
import { Client, CreatePlanNoteCommand, UpdateActivityPlanCommand } from '@/client'
import CreateActivityPlan from '@/components/Activity/CreateActivityPlan.vue'
import GetActivityById from '@/components/Activity/GetActivityById.vue'
import UpdateActivityDetail from '@/components/Activity/UpdateActivityDetail.vue'
import UpdateAppointmentOutcome from '@/components/Activity/UpdateAppointmentOutcome.vue'
import ImportFile from '@/components/Import/ImportFile.vue'
import { BACKEND_API_URL } from '@/constants'
import { useAuthStore, useSweetAlertStore } from '@/stores'
import { actionItemsRules, amountRules, appointmentCostRules, appointmentSummaryRules } from '@/utils/RuleServices'
import { defineComponent } from 'vue'

const client = new Client(BACKEND_API_URL)
export default defineComponent({
  name: 'ManageActivityDetail',
  components: {
    ImportFile,
    GetActivityById,
    UpdateActivityDetail,
    CreateActivityPlan,
    UpdateAppointmentOutcome,
  },
  props: {
    id: {
      type: [String, Number],
      required: true,
    },
  },
  emits: ['close'],
  data() {
    return {
      activeTab: 'EditActivity',
      ConditionReadonly: false,
      tabs: [
        {
          title: 'แก้ไขนัดหมาย',
          subtitle: 'จัดการข้อมูลนัดหมาย',
          icon: 'ri-edit-2-line',
          tab: 'EditActivity',
          hidden: false,
        },
        {
          title: 'แก้ไขรายงานสรุปผลนัดหมาย',
          subtitle: 'ปรับปรุงรายงานสรุปนัดหมาย',
          icon: 'ri-edit-box-line',
          tab: 'EditReport',
          hidden: false,
        },
        {
          title: 'บันทึกรายงานสรุปผลนัดหมาย',
          subtitle: 'เขียนรายงานสรุปนัดหมาย',
          icon: 'ri-draft-line',
          tab: 'CrateReport',
          hidden: false,
        },
      ],
      formattedCost: '',
      updateCommand: new UpdateActivityPlanCommand() as any,
      expensesOption: '',
      amount: null,
      expenseDetail: '',
      planNoteCommand: new CreatePlanNoteCommand(),
      sweetAlertStore: useSweetAlertStore(),
      User: {} as any,
      auth: useAuthStore(),
      appointmentCostRules,
      amountRules,
      appointmentSummaryRules,
      actionItemsRules,
    }
  },
  watch: {
    'updateCommand.cost'(newVal: any) {
      this.formattedCost = this.formatNumber(newVal)
    },
    id: {
      handler(newVal) {
        if (newVal) {
          this.initialize()
        }
      },
      immediate: true,
    },
  },
  async mounted() {
    // The initialize method is now called via the watcher
  },
  methods: {
    async initialize() {
      try {
        const response = await client.getActivityPlanQueryByID(this.id as any)
        this.updateCommand = { ...response } as UpdateActivityPlanCommand
        const result = await client.getEmployeeQueryByUserID(this.auth.userId)
        this.User = result

        // ดึง PlanNote จาก API
        const planNoteResult = await client.getPlanNoteQueryByActivityPlanId(String(this.id))
        if (planNoteResult) {
          this.planNoteCommand = new CreatePlanNoteCommand()
          Object.assign(this.planNoteCommand, planNoteResult)
        } else {
          // ถ้าไม่เจอข้อมูล PlanNote ให้สร้าง object เปล่า
          this.planNoteCommand = new CreatePlanNoteCommand()
        }
        this.planNoteCommand.activityPlanId = String(this.id)

        if (this.updateCommand.haveCost !== null && this.updateCommand.haveCost !== undefined) {
          this.tabs.forEach(tab => {
            if (tab.tab === 'CrateReport') {
              tab.hidden = true
            }
          })
        } else {
          this.tabs.forEach(tab => {
            if (tab.tab === 'EditReport') {
              tab.hidden = true
            }
          })
        }
      } catch (error) {
        console.error(error)
      }
    },
    closeDialog(shouldClose: boolean = true, reload: boolean = false) {
      console.log('🟢 ManageActivityDetailViews.closeDialog called with:', { shouldClose, reload })
      if (shouldClose === false || shouldClose === true) {
        console.log('🟢 Emitting close event')
        this.$emit('close')
      }
    },
    GoCancel() {
      this.closeDialog()
    },
    handleAppointmentOutcomeUpdated(data: any) {
      console.log('🟢 ManageActivityDetailViews: AppointmentOutcome updated successfully', data)
      // อัปเดตข้อมูลใน parent
      Object.assign(this.updateCommand, data.updateCommand)
      Object.assign(this.planNoteCommand, data.planNoteCommand)
      // ปิด dialog
      this.closeDialog()
    },
    handleCreateActivityPlanUpdated(data: any) {
      console.log('🟢 ManageActivityDetailViews: CreateActivityPlan updated successfully', data)
      // อัปเดตข้อมูลใน parent
      Object.assign(this.updateCommand, data.updateCommand)
      Object.assign(this.planNoteCommand, data.planNoteCommand)
      // ปิด dialog
      this.closeDialog()
    },
    async UpdateActivityAndCreatePlan() {
      const form = this.$refs.form as any
      const { valid } = await form.validate()
      if (valid) {
        try {
          const response = await client.updateActivityPlan(this.updateCommand)
          if (response) {
            const result = await client.createPlanNote(this.planNoteCommand)
            if (result) {
              this.sweetAlertStore.successDeleted('สร้างแผนสรุปสำเร็จ')
              this.closeDialog()
            }
          }
        } catch (error) {
          console.error(error)
        }
      }
    },
    formatNumber(value: any) {
      if (value !== null && value !== undefined) {
        const numberValue = Number(value)
        return numberValue.toLocaleString('en-US', {
          style: 'currency',
          currency: 'USD',
          minimumFractionDigits: 0,
          maximumFractionDigits: 2,
        })
      }
      return ''
    },
  },
})
</script>

<style scoped>
.page-container {
  width: 100% !important;
  max-width: 100% !important;
  min-height: 100vh;
  overflow-y: auto;
  overflow-x: hidden;
  padding: 16px 0 32px 0;
  scroll-behavior: smooth;
  position: relative;
  box-sizing: border-box;
}

.management-panel {
  max-width: 800px;
  margin: 0 auto;
}

.management-card {
  border-radius: 8px;
}

.header-section {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 16px;
  background: #ffffff;
  border-bottom: 1px solid #f3f4f6;
  padding: 16px;
}

.activity-title {
  margin: 0;
  font-size: 18px;
  font-weight: 500;
}

.management-tabs {
  border-bottom: 1px solid #e0e0e0;
}

.tab-content {
  display: flex;
  align-items: center;
}

.tab-icon-wrapper {
  width: 24px;
  height: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-right: 8px;
}

.tab-icon {
  font-size: 18px;
}

.tab-text {
  display: flex;
  flex-direction: column;
}

.tab-title {
  font-size: 14px;
  font-weight: 500;
}

.tab-subtitle {
  font-size: 12px;
  color: #888;
}

.content-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 16px;
}

.header-text {
  margin: 0;
  font-size: 16px;
  font-weight: 500;
}

.v-window {
  border-top: 1px solid #e0e0e0;
  padding-top: 16px;
}

.text-sub-title {
  font-size: clamp(18px, 4vw, 25px);
  font-weight: bold;
  color: #2b3086;
  background-clip: text;
  -webkit-background-clip: text;
  text-shadow: 5px 5px 8px rgba(65, 69, 92, 0.447);
  text-align: center;
  display: block;
  margin-bottom: 20px;
}
.card-Dialog {
  background-size: cover;
  background-position: center;
  background-repeat: no-repeat;
  max-height: 90vh;
  overflow-y: auto;
  -webkit-overflow-scrolling: touch;
}

/* Ensure full width for all containers */
.v-card {
  width: 100% !important;
  max-width: 100% !important;
}

.v-row {
  width: 100% !important;
  max-width: 100% !important;
  margin: 0 !important;
}

.v-col {
  width: 100% !important;
  max-width: 100% !important;
}

.activity-details-section {
  width: 100% !important;
  max-width: 100% !important;
}

.activity-plan-card {
  width: 100% !important;
  max-width: 100% !important;
}

@media (max-width: 600px) {
  .text-sub-title {
    font-size: 30px;
  }
}
</style>
