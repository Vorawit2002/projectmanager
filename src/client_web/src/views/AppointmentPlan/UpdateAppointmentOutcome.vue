<template>
  <v-col cols="12">
    <v-card class="text-form card-form">
      <!-- card title -->
      <v-row class="mt-3 mb-4">
        <v-col
          cols="12"
          class="d-flex justify-center"
        >
          <span class="text-sub-title text-center">สรุปการนัดพบลูกค้า</span>
        </v-col>
      </v-row>

      <v-card-text>
        <v-form
          ref="form"
          @submit.prevent="UpdateActivityAndCreatePlan"
          class="text-black"
        >
          <v-row>
            <v-col
              cols="12"
              align="end"
            >
              <p class="mr-8 text-h5 text-primary">แผนก : {{ updateCommand.employees?.departments?.name }}</p>
              <p class="mr-8 text-h5 text-primary">
                โดย :
                {{
                  updateCommand.employees?.titleName +
                  ' ' +
                  updateCommand.employees?.firstName +
                  ' ' +
                  updateCommand.employees?.lastName
                }}
              </p>
            </v-col>
          </v-row>

          <v-row class="px-md-7">
            <VCol cols="12">
              <GetCustomerAppointmentPlan
                v-if="updateCommand.id"
                :ActivityPlan="updateCommand"
                :PlanNote="planNoteCommand"
              />
            </VCol>

            <v-col
              cols="12"
              md="5"
            >
              <label class="mb-2">ค่าใช้จ่าย <span class="text-error">*</span> </label>
              <v-row class="d-flex align-center">
                <v-col
                  cols="12"
                  md="4"
                >
                  <v-radio-group
                    v-model="updateCommand.haveCost"
                    :rules="appointmentCostRules"
                    row
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
                  md="8"
                  v-if="updateCommand.haveCost === true"
                >
                  <v-text-field
                    v-model="formattedCost"
                    type="text"
                    label="จำนวนเงิน"
                    placeholder="ระบุจำนวนเงิน"
                    suffix="บาท"
                    class="text-right"
                    @input="onInputCost"
                    @blur="formatCost"
                  />
                </v-col>
              </v-row>
            </v-col>

            <v-col
              cols="12"
              md="7"
              v-if="updateCommand.haveCost === true"
            >
              <label class="mb-3">รายละเอียดค่าใช้จ่าย <span class="text-error"></span> </label>
              <v-textarea
                v-model="updateCommand.costDetail"
                placeholder="ระบุรายละเอียดค่าใช้จ่าย"
                rows="3"
                class="mt-3"
              />
            </v-col>

            <v-col cols="12">
              <label class="mb-2">สรุปผลการนัดพบ <span class="text-error">*</span></label>
              <v-textarea
                v-model="planNoteCommand.summary"
                placeholder="ระบุรายละเอียดสรุปผลการนัดพบ"
                :rules="[(v:any)=> !!v||'กรุณาระบุรายละเอียดสรุปผลการนัดพบ']"
              />
            </v-col>

            <v-col cols="12">
              <label class="mb-2">สิ่งที่ต้องดำเนินการ <span class="text-error">*</span></label>
              <v-textarea
                v-model="planNoteCommand.toDoNext"
                placeholder="ระบุรายละเอียดสิ่งที่ต้องดำเนินการ"
                :rules="[(v:any)=> !!v||'กรุณาระบุรายละเอียดสิ่งที่ต้องดำเนินการ']"
              />
            </v-col>

            <v-col cols="12">
              <label class="mb-2">หมายเหตุ </label>
              <v-textarea
                v-model="planNoteCommand.remarks"
                placeholder="ระบุรายละเอียดหมายเหตุ"
              />
            </v-col>

            <!-- อัพโหลดภาพ -->
            <v-col cols="12">
              <v-card
                class="px-6 py-6 mb-6"
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
                    md="10"
                    style="margin-bottom: -50px"
                  >
                    <ImportFile
                      v-if="id"
                      :id="id"
                    />
                  </v-col>
                </v-row>
              </v-card>
            </v-col>

            <v-col
              cols="12"
              class="d-flex justify-end"
            >
              <v-btn
                class="me-4"
                color="error"
                @click="GoCancel"
              >
                <v-icon class="mr-2">ri-close-large-fill</v-icon>
                ยกเลิก
              </v-btn>

              <v-btn
                type="submit"
                color="success-darken-2"
              >
                <v-icon class="mr-2">ri-save-3-fill</v-icon>
                บันทึก
              </v-btn>
            </v-col>
          </v-row>
        </v-form>
      </v-card-text>
    </v-card>
  </v-col>
</template>

<script lang="ts">
import { Client, UpdateActivityPlanCommand, UpdatePlanNoteCommand } from '@/client'
import ImportFile from '@/components/Import/ImportFile.vue'
import { BACKEND_API_URL } from '@/constants'
import { useSweetAlertStore } from '@/stores'
import { appointmentCostRules } from '@/utils/RuleServices'
import { defineComponent } from 'vue'
import GetCustomerAppointmentPlan from './GetCustomerAppointmentPlan.vue'

const client = new Client(BACKEND_API_URL)
export default defineComponent({
  name: 'AppointmentOutcome',
  components: {
    ImportFile,
    GetCustomerAppointmentPlan,
  },
  data() {
    return {
      formattedCost: '',
      updateCommand: new UpdateActivityPlanCommand() as any,
      expensesOption: '',
      amount: null,
      expenseDetail: '',
      planNoteCommand: new UpdatePlanNoteCommand(),
      id: '' as any,
      sweetAlertStore: useSweetAlertStore(),
      appointmentCostRules,
    }
  },
  async mounted() {
    this.id = this.$route.params.id as any
    if (this.id) {
      await this.initialize()
    }
  },
  methods: {
    async initialize() {
      try {
        const response = await client.getActivityPlanQueryByID(this.id)
        this.updateCommand = { ...response } as UpdateActivityPlanCommand

        this.formattedCost = this.updateCommand.cost ? this.formatNumber(this.updateCommand.cost) : ''

        const result = await client.getPlanNoteQueryByActivityPlanId(this.id)
        this.planNoteCommand = { ...result } as UpdatePlanNoteCommand
      } catch (error) {
        console.error(error)
      }
    },
    GoCancel() {
      this.$router.push({
        name: 'CustomerAppointmentListView',
        query: { tab: this.$route.query.tab },
      })
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
                this.GoCancel()
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
        const numeric = parseFloat(rawValue)
        this.updateCommand.cost = numeric
        this.formattedCost = this.formatNumber(numeric) // ฟอร์แมตเลยตอนพิมพ์
      } else {
        this.formattedCost = ''
        this.updateCommand.cost = null
      }
    },
    formatCost() {
      this.formattedCost = this.formatNumber(this.updateCommand.cost)
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
  border-radius: 15px;
  /* box-shadow: 2px 2px 6px 6px rgba(43, 48, 134, 0.15); */
}
.dashed-border {
  border: 1px dashed grey;
}

.text-black {
  color: #1b1a1a;
}
</style>
