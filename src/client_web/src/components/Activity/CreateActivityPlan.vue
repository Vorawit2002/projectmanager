<template>
  <v-card
    class="text-form"
    elevation="0"
  >
    <v-form
      ref="form"
      @submit.prevent="UpdateActivityAndCreatePlan"
      class="text-black"
    >
      <v-row class="mt-2 mb-2">
        <v-col
          cols="12"
          md="12"
        >
          <label class="mb-2">ค่าใช้จ่าย <span class="text-error">*</span> </label>
          <v-row class="d-flex align-center">
            <v-col
              cols="12"
              md="6"
            >
              <v-radio-group
                v-model="currentUpdateCommand.haveCost"
                :rules="appointmentCostRules"
                row
                :readonly="ConditionReadonly"
              >
                <v-radio
                  label="มีค่าใช้จ่าย"
                  :value="true"
                />
                <v-radio
                  label="ไม่มีค่าใช้จ่าย"
                  :value="false"
                />
              </v-radio-group>
            </v-col>

            <v-col
              cols="12"
              md="12"
              v-if="currentUpdateCommand.haveCost === true"
            >
              <v-text-field
                v-model="formattedCost"
                type="text"
                label="จำนวนเงิน"
                placeholder="ระบุจำนวนเงิน"
                suffix="บาท"
                class="text-right from-field"
                @input="onInputCost"
                @blur="formatCost"
                :rules="amountRules"
              />
            </v-col>
          </v-row>
        </v-col>

        <v-col
          cols="12"
          md="12"
          v-if="currentUpdateCommand.haveCost === true"
        >
          <label class="mb-3">รายละเอียดค่าใช้จ่าย <span class="text-error">*</span> </label>
          <v-textarea
            v-model="currentUpdateCommand.costDetail"
            placeholder="ระบุรายละเอียดค่าใช้จ่าย"
            rows="3"
            class="mt-3 form-field"
            :rules="CostDetailRules"
            :readonly="ConditionReadonly"
          />
        </v-col>

        <v-col cols="12">
          <label class="mb-2">สรุปผลการนัดพบ <span class="text-error">*</span></label>
          <v-textarea
            v-model="currentPlanNoteCommand.summary"
            placeholder="ระบุรายละเอียดสรุปผลการนัดพบ"
            :rules="appointmentSummaryRules"
            :readonly="ConditionReadonly"
            class="form-field"
          />
        </v-col>

        <v-col cols="12">
          <label class="mb-2">สิ่งที่ต้องดำเนินการ <span class="text-error">*</span></label>
          <v-textarea
            v-model="currentPlanNoteCommand.toDoNext"
            placeholder="ระบุรายละเอียดสิ่งที่ต้องดำเนินการ"
            :rules="actionItemsRules"
            :readonly="ConditionReadonly"
            class="form-field"
          />
        </v-col>

        <v-col cols="12">
          <label class="mb-2">หมายเหตุ </label>
          <v-textarea
            v-model="currentPlanNoteCommand.remarks"
            placeholder="ระบุรายละเอียดหมายเหตุ"
            :readonly="ConditionReadonly"
            class="form-field"
          />
        </v-col>

        <!-- อัพโหลดภาพ -->
        <v-col cols="12">
          <v-card
            class="upload-container pa-6 mb-6"
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
                <h2 class="mb-0">รูปภาพอ้างอิง</h2>
              </v-col>
            </v-row>

            <v-row
              class="mt-0 mb-0 px-0 text-form"
              align="center"
              justify="center"
            >
              <v-col
                clos="12"
                md="12"
                style="margin-bottom: -50px"
              >
                <ImportFile
                  :readonly="ConditionReadonly"
                  v-if="effectiveId"
                  :id="effectiveId"
                />
              </v-col>
            </v-row>
          </v-card>
        </v-col>

        <v-col
          cols="12"
          class="pt-6 action-btn-row"
        >
          <!-- Mobile: ปุ่มบันทึก/ยกเลิก 50/50 -->
          <template v-if="$vuetify.display.mobile">
            <v-row
              class="d-flex action-btn-row"
              no-gutters
            >
              <v-col
                cols="6"
                class="pr-1"
              >
                <v-btn
                  class="mobile-btn"
                  color="error"
                  @click="GoCancel"
                  :disabled="loading"
                  :size="$vuetify.display.mobile ? 'default' : 'large'"
                  block
                >
                  <v-icon class="mr-2">ri-close-large-fill</v-icon>
                  ยกเลิก
                </v-btn>
              </v-col>
              <v-col
                cols="6"
                class="pl-1"
              >
                <v-btn
                  v-if="!ConditionReadonly"
                  class="mobile-btn"
                  color="success-darken-2"
                  type="submit"
                  :loading="loading"
                  :disabled="loading"
                  :size="$vuetify.display.mobile ? 'default' : 'large'"
                  block
                >
                  <v-icon class="mr-2">ri-save-3-fill</v-icon>
                  บันทึก
                </v-btn>
              </v-col>
            </v-row>
          </template>
          <!-- Desktop: ปุ่มปกติ -->
          <template v-else>
            <div class="d-flex justify-center flex-wrap gap-2">
              <v-btn
                class="me-2 mb-2 mb-sm-0 mobile-btn"
                color="error"
                @click="GoCancel"
                :disabled="loading"
                :size="$vuetify.display.mobile ? 'default' : 'large'"
              >
                <v-icon class="mr-2">ri-close-large-fill</v-icon>
                ยกเลิก
              </v-btn>
              <v-btn
                v-if="!ConditionReadonly"
                class="mobile-btn"
                color="success-darken-2"
                type="submit"
                :loading="loading"
                :disabled="loading"
                :size="$vuetify.display.mobile ? 'default' : 'large'"
              >
                <v-icon class="mr-2">ri-save-3-fill</v-icon>
                บันทึก
              </v-btn>
            </div>
          </template>
        </v-col>
      </v-row>
    </v-form>
  </v-card>
</template>

<script lang="ts">
import { defineComponent } from 'vue'

import { Client, CreatePlanNoteCommand, UpdateActivityPlanCommand } from '@/client'
import ImportFile from '@/components/Import/ImportFile.vue'
import { BACKEND_API_URL } from '@/constants'
import { useAuthStore, useSweetAlertStore } from '@/stores'
import {
  CostDetailRules,
  actionItemsRules,
  amountRules,
  appointmentCostRules,
  appointmentSummaryRules,
} from '@/utils/RuleServices'
import GetCustomerAppointmentPlan from '@/views/AppointmentPlan/GetCustomerAppointmentPlan.vue'

const client = new Client(BACKEND_API_URL)
export default defineComponent({
  name: 'CreateActivityPlan',
  components: {
    ImportFile,
    GetCustomerAppointmentPlan,
  },
  props: {
    updateCommand: {
      type: Object as any,
      required: false,
      default: null,
    },
    planNoteCommand: {
      type: Object as any,
      required: false,
      default: null,
    },
    isDialog: {
      type: Boolean,
      required: false,
      default: false,
    },
    activityPlanId: {
      type: String,
      required: false,
      default: null,
    },
  },
  emits: ['updated', 'cancel', 'update:update-command', 'update:plan-note-command'],
  data() {
    return {
      loading: false,
      formattedCost: '',
      localUpdateCommand: new UpdateActivityPlanCommand() as any,
      expensesOption: '',
      amount: null,
      expenseDetail: '',
      localPlanNoteCommand: new CreatePlanNoteCommand(),
      id: '' as any,
      sweetAlertStore: useSweetAlertStore(),
      User: {} as any,
      auth: useAuthStore(),
      appointmentCostRules,
      amountRules,
      appointmentSummaryRules,
      actionItemsRules,
      CostDetailRules,
    }
  },
  watch: {
    'currentUpdateCommand.cost'(newVal: any) {
      // เมื่อ updateCommand.cost เปลี่ยนจากภายนอก ให้ sync กับ formattedCost
      this.formattedCost = this.formatNumber(newVal)
    },
  },
  async mounted() {
    this.id = this.$route.params.id as any

    // ถ้าเป็น dialog และมี activityPlanId จาก props ให้ใช้แทน
    if (this.isDialog && this.activityPlanId) {
      this.id = this.activityPlanId
    }

    console.log('🔧 CreateActivityPlan mounted:', {
      isDialog: this.isDialog,
      routeId: this.$route.params.id,
      activityPlanId: this.activityPlanId,
      finalId: this.id,
      effectiveId: this.effectiveId,
    })

    if (this.id && !this.isDialog) {
      // ถ้าเป็น standalone page ให้โหลดข้อมูลเอง
      await this.initialize()
    } else if (this.isDialog) {
      // ถ้าเป็น dialog ใช้ข้อมูลจาก props
      this.formattedCost = this.currentUpdateCommand.cost ? this.formatNumber(this.currentUpdateCommand.cost) : ''

      // ถ้ามี id ให้ setup ค่าเริ่มต้นสำหรับ ImportFile
      if (this.id) {
        const result = await client.getEmployeeQueryByUserID(this.auth.userId)
        this.User = result
        this.localPlanNoteCommand.activityPlanId = this.id
      }
    }
  },
  methods: {
    async initialize() {
      try {
        const response = await client.getActivityPlanQueryByID(this.id)
        this.localUpdateCommand = { ...response } as UpdateActivityPlanCommand
        const result = await client.getEmployeeQueryByUserID(this.auth.userId)
        this.User = result
        // const result = await client.getPlanNoteQueryByActivityPlanId(this.id)
        this.localPlanNoteCommand.activityPlanId = this.id
      } catch (error) {
        console.error(error)
      }
    },
    GoCancel() {
      console.log('🔴 CreateActivityPlan: GoCancel called, isDialog:', this.isDialog)
      if (this.isDialog) {
        // ถ้าเป็น dialog ให้ emit cancel event
        this.$emit('cancel')
      } else {
        // ถ้าเป็น standalone page ให้ใช้ router
        this.$router.back()
      }
    },
    async UpdateActivityAndCreatePlan() {
      const form = this.$refs.form as any
      const { valid } = await form.validate()
      if (valid) {
        this.loading = true
        try {
          console.log('🟢 CreateActivityPlan: Saving...', {
            isDialog: this.isDialog,
            updateCommand: this.currentUpdateCommand,
            planNoteCommand: this.currentPlanNoteCommand,
          })

          const response = await client.updateActivityPlan(this.currentUpdateCommand)
          if (response) {
            const result = await client.createPlanNote(this.currentPlanNoteCommand)
            if (result) {
              this.sweetAlertStore.successDeleted('สร้างแผนสรุปสำเร็จ')

              if (this.isDialog) {
                // ถ้าเป็น dialog ให้ emit updated event และปิด dialog
                this.$emit('updated', {
                  updateCommand: this.currentUpdateCommand,
                  planNoteCommand: this.currentPlanNoteCommand,
                })
              } else {
                // ถ้าเป็น standalone page ให้กลับไปหน้าเดิม
                setTimeout(() => {
                  this.GoCancel()
                }, 1500)
              }
            }
          }
        } catch (error) {
          console.error('❌ CreateActivityPlan: Error saving:', error)
          this.sweetAlertStore.error('เกิดข้อผิดพลาดในการบันทึกข้อมูล')
        } finally {
          this.loading = false
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
        const numeric = parseFloat(rawValue)

        // อัปเดตค่าในโครงสร้างข้อมูลที่เหมาะสม
        if (this.isDialog && this.updateCommand) {
          // ถ้าเป็น dialog ให้ emit การเปลี่ยนแปลง
          const updatedCommand = { ...this.currentUpdateCommand, cost: numeric }
          this.$emit('update:update-command', updatedCommand)
        } else {
          // ถ้าเป็น standalone ให้อัปเดต local data
          this.localUpdateCommand.cost = numeric
        }

        this.formattedCost = rawValue
      }
    },
    formatCost() {
      this.formattedCost = this.formatNumber(this.currentUpdateCommand.cost)
    },
  },
  computed: {
    // ใช้ props ถ้ามี หรือใช้ local data ถ้าเป็น standalone
    currentUpdateCommand(): any {
      return this.updateCommand || this.localUpdateCommand
    },
    currentPlanNoteCommand(): any {
      return this.planNoteCommand || this.localPlanNoteCommand
    },
    ConditionReadonly() {
      if (this.User.id === this.currentUpdateCommand.employeeId) {
        return false
      } else {
        return true
      }
    },
    // ID ที่ใช้สำหรับ ImportFile
    effectiveId(): string {
      const id = this.id || this.activityPlanId || ''
      console.log('🎯 CreateActivityPlan effectiveId:', {
        id: this.id,
        activityPlanId: this.activityPlanId,
        effectiveId: id,
      })
      return id
    },
  },
})
</script>

<style scoped>
.text-sub-title {
  font-size: 30px;
  font-weight: bold;
  color: #2b3086;
  /* background-image: linear-gradient( 135deg, #7e4ee6b6 10%, #8C57FF 100%); */
  background-clip: text;
  -webkit-background-clip: text; /* สำหรับเว็บเบราว์เซอร์ที่รองรับ */
  text-shadow: 5px 5px 8px rgba(65, 69, 92, 0.447);
}

.card-form {
  background: #ffffff;
  border-radius: 10px;
}
.dashed-border {
  border: 1px dashed grey;
}

.text-black {
  color: #1b1a1a;
}

/* ปุ่มและ layout เหมือน UpdateActivityDetail.vue */
.button-container {
  display: flex;
  gap: 16px;
  justify-content: center;
  flex-wrap: wrap;
}

.mobile-btn {
  min-width: 120px;
  height: 44px;
  font-size: 0.9375rem;
  font-weight: 500;
  letter-spacing: 0.025em;
  text-transform: none;
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.15);
  transition: all 0.3s cubic-bezier(0.25, 0.8, 0.5, 1);
}

.mobile-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 12px rgba(0, 0, 0, 0.2);
}

.action-btn:focus {
  outline: none !important;
  box-shadow: 0 0 0 3px rgba(255, 255, 255, 0.3), 0 4px 8px rgba(0, 0, 0, 0.15) !important;
}

@media (max-width: 599px) {
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
  .action-btn-row {
    margin-bottom: 30% !important;
  }
}

/* Enhanced form fields */
.form-field {
  transition: all 0.2s ease;
}

.form-field:deep(.v-field) {
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.04);
  transition: all 0.2s ease;
}

.form-field:deep(.v-field--focused) {
  box-shadow: 0 4px 16px rgba(43, 48, 134, 0.12);
}

.textarea-field:deep(.v-field) {
  border-radius: 16px;
}

.checkbox-field:deep(.v-selection-control) {
  min-height: 32px;
}

.add-btn {
  opacity: 0.7;
  transition: all 0.2s ease;
}

.add-btn:hover {
  opacity: 1;
  transform: scale(1.1);
}

.upload-container {
  background-color: #fff; /* ต้องมีพื้นหลังสีขาว */
  border: 1px solid rgba(0, 0, 0, 0.12); /* สร้างเส้นขอบบางๆ สีเทาอ่อน */
  border-radius: 12px; /* ทำให้ขอบมนเหมือน form-field */
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.04); /* เพิ่มเงาเหมือน form-field */
  transition: all 0.2s ease;
}

/* สร้างเอฟเฟกต์ตอน focus */
.upload-container:focus-within {
  border-color: rgba(43, 48, 134, 0.6); /* เปลี่ยนสีขอบเมื่อ focus */
  box-shadow: 0 4px 16px rgba(43, 48, 134, 0.12); /* เพิ่มเงาตอน focus */
}
</style>

