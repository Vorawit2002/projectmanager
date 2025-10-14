<template>
  <v-col cols="12">
    <v-card class="text-form card-form scrollable-card">
      <!-- card title -->

      <v-card-text class="scrollable-content dialog-scrollbar">
        <v-form
          ref="form"
          @submit.prevent="UpdateForm"
          class="text-black"
        >
          <v-row>
            <v-col
          cols="12"
          class="d-flex justify-center px-3"
        >
          <span class="text-sub-title text-center">แก้ไขแผนการนัดพบลูกค้า</span>
        </v-col>
            <v-col
              cols="12"
              align="end"
            >
              <p class="mr-2 mr-md-8 text-h5 text-primary">แผนก : {{ updateCommand.employees?.departments?.name }}</p>
              <p class="mr-2 mr-md-8 text-h5 text-primary">
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

          <v-row class="px-2 px-sm-4 px-md-7">
            <!-- Empty spacer column for desktop layout -->
            <v-col
              cols="12"
              md="4"
              class="d-none d-md-block"
            ></v-col>

            <!-- Date section -->
            <v-col
              cols="12"
              md="8"
            >
              <v-row
                class="d-flex"
                align="center"
              >
                <v-col
                  cols="12"
                  sm="6"
                  md="5"
                >
                  <label class="mb-2">วันที่เริ่มต้น <span class="text-error">*</span> </label>
                  <TextFieldDatepicker
                    v-if="updateCommand.startDate"
                    :readonly="ConditionReadonly"
                    placeholder="กรุณาระบุวันที่เริ่มต้นนัดหมาย"
                    :rules="appointmentStartDateRules"
                    :selectedDateTime="updateCommand.startDate"
                    @selectedDateTime="changeTimestartDate"
                    :AllDay="updateCommand.allDay"
                  />
                  <TextFieldDatepicker
                    v-else
                    :readonly="ConditionReadonly"
                    placeholder="กรุณาระบุวันที่เริ่มต้นนัดหมาย"
                    :rules="appointmentStartDateRules"
                    @selectedDateTime="changeTimestartDate"
                    :AllDay="updateCommand.allDay"
                  />
                </v-col>

                <v-col
                  cols="12"
                  sm="6"
                  md="5"
                >
                  <label class="mb-2">วันที่สิ้นสุด <span class="text-error">*</span> </label>
                  <TextFieldDatepicker
                    v-if="updateCommand.endDate"
                    :readonly="ConditionReadonly"
                    placeholder="กรุณาระบุวันที่สิ้นสุดนัดหมาย"
                    :rules="appointmentEndDateRules"
                    :selectedDateTime="updateCommand.endDate"
                    @selectedDateTime="changeTimeendDate"
                    :AllDay="updateCommand.allDay"
                  />
                  <TextFieldDatepicker
                    v-else
                    :readonly="ConditionReadonly"
                    placeholder="กรุณาระบุวันที่สิ้นสุดนัดหมาย"
                    :rules="appointmentEndDateRules"
                    @selectedDateTime="changeTimeendDate"
                    :AllDay="updateCommand.allDay"
                  />
                </v-col>

                <v-col
                  cols="12"
                  sm="12"
                  md="2"
                  class="mt-0 mt-md-4 d-flex align-center"
                >
                  <v-checkbox
                    :readonly="ConditionReadonly"
                    label="ทั้งวัน"
                    hide-details
                    v-model="updateCommand.allDay"
                  />
                </v-col>
              </v-row>
            </v-col>

            <!-- Form fields -->
            <v-col
              cols="12"
              sm="6"
              md="4"
            >
              <label class="mb-2">รหัสโครงการ </label>
              <v-autocomplete
                :readonly="ConditionReadonly"
                placeholder="กรุณาเลือกรหัสโครงการ"
                clearable
                v-model="updateCommand.projectId"
                :items="ProjectList"
                item-title="projectCodeAndName"
                item-value="id"
              />
            </v-col>

            <v-col
              cols="12"
              sm="6"
              md="4"
            >
              <label class="mb-2">หน่วยงาน / ลูกค้า <span class="text-error">*</span> </label>
              <v-autocomplete
                :readonly="ConditionReadonly"
                placeholder="กรุณาเลือกหน่วยงานหรือลูกค้า"
                clearable
                :items="OrganizationList"
                item-title="name"
                item-value="id"
                :rules="selectAgencyOrCustomerRules"
                v-model="updateCommand.organizationId"
              />
            </v-col>

            <v-col
              cols="12"
              sm="6"
              md="4"
            >
              <label class="mb-2">ลูกค้าที่นัดพบ <span class="text-error">*</span> </label>
              <v-autocomplete
                :key="`customers-${updateCommand.organizationId}-${customersSelect.length}`"
                :readonly="ConditionReadonly"
                :items="OrganizationContactList"
                item-title="name"
                item-value="id"
                v-model="customersSelect"
                multiple
                clearable
                :rules="selectCustomerRules"
                placeholder="เลือกลูกค้าที่นัดพบ"
                :disabled="updateCommand.organizationId === undefined || updateCommand.organizationId === null"
                @update:model-value="onCustomersSelectChange"
              >
              </v-autocomplete>
            </v-col>

            <v-col
              cols="12"
              sm="6"
              md="4"
            >
              <label class="mb-2">สถานที่ <span class="text-error">*</span> </label>
              <v-text-field
                :readonly="ConditionReadonly"
                placeholder="อาคาร 2, ชั้น 12, ห้องประชุม A"
                :rules="appointmentLocationRules"
                v-model="updateCommand.location"
              >
              </v-text-field>
            </v-col>

            <v-col
              cols="12"
              sm="6"
              md="4"
            >
              <label class="mb-2">วัตถุประสงค์ <span class="text-error">*</span></label>
              <v-autocomplete
                :readonly="ConditionReadonly"
                placeholder="ระบุวัตถุประสงค์"
                :items="objectiveList"
                v-model="updateCommand.objective"
                :rules="appointmentPurposeRules"
                clearable
              />
            </v-col>

            <v-col
              cols="12"
              sm="6"
              md="4"
              v-if="updateCommand.objective === 'อื่น ๆ'"
            >
              <label class="mb-2">วัตถุประสงค์อื่น ๆ <span class="text-error">*</span></label>
              <v-text-field
                :readonly="ConditionReadonly"
                placeholder="ระบุวัตถุประสงค์อื่น ๆ"
                :rules="otherPurposeRules"
                v-model="updateCommand.objectiveDetail"
              />
            </v-col>

            <v-col cols="12">
              <label class="mb-2">รายละเอียดเพิ่มเติม </label>
              <v-textarea
                :readonly="ConditionReadonly"
                placeholder="ระบุรายละเอียดการนัดพบ"
                v-model="updateCommand.detail"
              />
            </v-col>

            <v-col
              cols="12"
              class="pt-6"
            >
              <div class="button-row">
                <v-btn
                  rounded="lg"
                  color="error"
                  @click="CancelCreate"
                  class="btn-flex me-2 mb-2 mb-sm-0 mobile-btn"
                  type="button"
                  :size="$vuetify.display.mobile ? 'default' : 'large'"
                  :block="$vuetify.display.mobile"
                >
                  <v-icon
                    icon="ri-close-line"
                    class="mr-2"
                  ></v-icon>
                  ยกเลิก
                </v-btn>

                <v-btn
                  v-if="!ConditionReadonly"
                  rounded="lg"
                  color="success-darken-2"
                  type="submit"
                  class="btn-flex mobile-btn"
                  :loading="loading"
                  :disabled="loading"
                  :size="$vuetify.display.mobile ? 'default' : 'large'"
                  :block="$vuetify.display.mobile"
                >
                  <v-icon
                    icon="ri-save-3-fill"
                    class="mr-2"
                  ></v-icon>
                  บันทึก
                </v-btn>
              </div>
            </v-col>
          </v-row>
        </v-form>
      </v-card-text>
    </v-card>
  </v-col>
</template>

<script lang="ts">
import {
  Client,
  CreateActivityPlanContactCommand,
  GetActivityPlanContactByActivityPlanIdQuery,
  GetOrganizationContactByOrganizationIdQuery,
  UpdateActivityPlanCommand,
} from '@/client'
import TextFieldDatepicker from '@/components/Datepicker/TextFieldDatepicker.vue'
import TextFieldTimepicker from '@/components/Datepicker/TextFieldTimepicker.vue'
import { BACKEND_API_URL } from '@/constants'
import { useAuthStore, useSweetAlertStore } from '@/stores'
import {
  appointmentEndDateRules,
  appointmentLocationRules,
  appointmentPurposeRules,
  appointmentStartDateRules,
  otherPurposeRules,
  selectAgencyOrCustomerRules,
  selectCustomerRules,
} from '@/utils/RuleServices'
import DemoFormLayoutVerticalFormWithIcons from '@/views/pages/form-layouts/DemoFormLayoutVerticalFormWithIcons.vue'
import { defineComponent } from 'vue'
const client = new Client(BACKEND_API_URL)
export default defineComponent({
  name: 'UpdateCustomerAppointmentPlan',
  props: {
    id: {
      type: [String, Number],
      required: false,
    },
  },
  emits: ['close', 'updated'],
  components: {
    DemoFormLayoutVerticalFormWithIcons,
    TextFieldDatepicker,
    TextFieldTimepicker,
  },
  data() {
    return {
      DepartmentList: ['Sale', 'Product', 'Services', 'Development', 'MD Office'] as any,
      OrganizationList: [] as any,
      CustomerTypeList: ['เดิม', 'ใหม่', 'ส่งต่อ'] as any,
      objectiveList: [
        'ประชุมงาน / โครงการ',
        'ทานข้าว',
        'นำของไปฝาก',
        'ไปร่วมจัดอบรม / ตรวจรับงาน',
        'ไปสัญจร / ไปต่างจังหวัด',
        'ไปต่างประเทศ / ไปดูงาน',
        'อื่น ๆ',
      ] as any,
      OrganizationContactList: [] as any,
      updateCommand: new UpdateActivityPlanCommand() as any,
      startDate: undefined as any,
      endDate: undefined as any,
      selectedObjective: [] as any, // เก็บวัตถุประสงค์ที่เลือก
      otherObjective: '', // เก็บวัตถุประสงค์อื่น ๆ ที่กรอก
      customersSelect: [] as any,
      customers: [] as any,
      AllDay: false,
      currentId: '' as any,
      requestOrganizationContact: new GetOrganizationContactByOrganizationIdQuery(),
      ProjectList: [] as any,
      createContact: new CreateActivityPlanContactCommand(),
      sweetAlertStore: useSweetAlertStore(),
      auth: useAuthStore(),
      loading: false,
      UserId: '' as any, // สำหรับเช็ค readonly
      appointmentStartDateRules,
      appointmentEndDateRules,
      selectAgencyOrCustomerRules,
      selectCustomerRules,
      appointmentLocationRules,
      appointmentPurposeRules,
      otherPurposeRules,
      isUserInteracting: false, // flag เพื่อระบุว่า user กำลังแก้ไขข้อมูล
    }
  },
  async mounted() {
    // ใช้ props id ถ้ามี หรือใช้ route params
    this.currentId = this.id || (this.$route.params.id as any)
    if (this.currentId) {
      await this.initialize()
    }
    await this.getOrganization()
    // ไม่ต้องเรียก getOrganizationContact() เพราะ watcher จะจัดการให้
    await this.$nextTick()
    ;(this.$refs.form as any).validate()
  },
  methods: {
    async initialize() {
      try {
        this.customersSelect = []
        const result = await client.getEmployeeQueryByUserID(this.auth.userId)
        this.UserId = result.id
        const response = await client.getActivityPlanQueryByID(String(this.currentId))
        this.updateCommand = Object.assign(new UpdateActivityPlanCommand(), response)
        this.ProjectList = await client.getProjectQuery()
        this.ProjectList.forEach((project: any) => {
          const truncatedName =
            project.projectName.length > 70 ? project.projectName.substring(0, 70) + '...' : project.projectName
          project.projectCodeAndName = `${project.projectCode} - ${truncatedName}`
        })
        let command = new GetActivityPlanContactByActivityPlanIdQuery()
        command.activityPlanId = this.updateCommand.id
        const activityContact = await client.getActivityPlanContactQueryByActivityPlanId(command)
        this.customers = activityContact
        activityContact.forEach((x: any) => {
          this.customersSelect.push(x.organizationContactId)
        })
      } catch (error) {
        console.error(error)
      }
    },
    async UpdateForm() {
      const form = this.$refs.form as any
      const { valid } = await form.validate()
      if (valid) {
        this.loading = true
        try {
          // Debug: แสดงข้อมูลที่กำลังจะบันทึก
          console.log('💾 กำลังบันทึกข้อมูล updateCommand:', JSON.stringify(this.updateCommand, null, 2))
          console.log('👥 ลูกค้าปัจจุบัน (customers):', this.customers)
          console.log('✅ ลูกค้าที่เลือก (customersSelect):', this.customersSelect)

          // คำนวณ contacts ที่ต้องลบ - เอาลูกค้าที่มีในฐานข้อมูลแต่ไม่ได้เลือกในการแก้ไขครั้งนี้
          const dontselected = this.customers
            .filter((customer: any) => !this.customersSelect.includes(customer.organizationContactId))
            .map((customer: any) => customer.id) // ใช้ id ของ ActivityPlanContact โดยตรง

          // คำนวณ contacts ที่ต้องเพิ่ม - เอาลูกค้าที่เลือกใหม่แต่ยังไม่มีในฐานข้อมูล
          const Updateselected = this.customersSelect.filter(
            (id: any) => !this.customers.some((customer: any) => customer.organizationContactId === id),
          )

          console.log('🗑️ จะลบ ActivityPlanContact:', dontselected)
          console.log('➕ จะเพิ่ม OrganizationContact:', Updateselected)

          // บันทึกข้อมูลหลักก่อน
          const response = await client.updateActivityPlan(this.updateCommand)
          if (response) {
            // ลบ contacts ที่ไม่ต้องการก่อน
            if (dontselected.length > 0) {
              console.log('🗑️ เริ่มลบ contacts:', dontselected)
              for (let index = 0; index < dontselected.length; index++) {
                try {
                  await client.deleteActivityPlanContact(dontselected[index])
                  console.log('✅ ลบ contact สำเร็จ:', dontselected[index])
                } catch (deleteError) {
                  console.error('❌ ลบ contact ล้มเหลว:', dontselected[index], deleteError)
                }
              }
            }

            // เพิ่ม contacts ใหม่
            if (Updateselected.length > 0) {
              console.log('➕ เริ่มเพิ่ม contacts:', Updateselected)
              this.createContact.activityPlanId = this.updateCommand.id
              for (let i = 0; i < Updateselected.length; i++) {
                try {
                  this.createContact.organizationContactId = Updateselected[i]
                  await client.createActivityPlanContact(this.createContact)
                  console.log('✅ เพิ่ม contact สำเร็จ:', Updateselected[i])
                } catch (createError) {
                  console.error('❌ เพิ่ม contact ล้มเหลว:', Updateselected[i], createError)
                }
              }
            }

            setTimeout(() => {
              this.loading = false
              this.sweetAlertStore.success('แก้ไขข้อมูลสำเร็จ')
              if (this.id) {
                // เมื่อใช้ใน dialog ให้ emit updated event
                this.$emit('updated')
                this.$emit('close')
              } else {
                this.CancelCreate()
              }
            }, 600)
          }
        } catch (error) {
          console.error(error)
          setTimeout(() => {
            this.loading = false
            this.sweetAlertStore.error('เกิดข้อผิดพลาดในการแก้ไขข้อมูล ล้มเหลว!')
            if (this.id) {
              // เมื่อใช้ใน dialog ให้ emit updated event แม้จะล้มเหลว
              this.$emit('updated')
            }
          }, 600)
        }
      }
    },
    changeTimestartDate(value: any) {
      this.updateCommand.startDate = value
    },
    changeTimeendDate(value: any) {
      this.updateCommand.endDate = value
    },
    async getOrganization() {
      try {
        this.OrganizationList = await client.getOrganizationQuery()
      } catch (error) {
        console.error(error)
      }
    },
    async getOrganizationContact() {
      try {
        if (!this.updateCommand.organizationId) {
          this.OrganizationContactList = []
          this.customersSelect = []
          console.log('🚫 ไม่มี organizationId')
          return
        }

        console.log('🔍 โหลด OrganizationContact สำหรับ organizationId:', this.updateCommand.organizationId)

        this.requestOrganizationContact.organizationId = this.updateCommand.organizationId

        this.OrganizationContactList = await client.getOrganizationContactQueryByOrganizationId(
          this.requestOrganizationContact,
        )

        console.log('📋 OrganizationContact ที่โหลดได้:', this.OrganizationContactList.length, 'items')
        console.log(
          '📋 รายชื่อ:',
          this.OrganizationContactList.map((c: any) => ({ id: c.id, name: c.name })),
        )

        // จัดรูปแบบชื่อ contact
        this.OrganizationContactList.forEach((OC: any) => {
          OC.name = `${OC.firstName || ''}   ${OC.lastName || ''}`.trim()
        })
      } catch (error) {
        console.error('❌ เกิดข้อผิดพลาดในการโหลด OrganizationContact:', error)
        this.OrganizationContactList = []
      }
    },
    CancelCreate() {
      // เมื่อใช้ใน dialog ให้ปิด dialog แทน
      if (this.id) {
        this.$emit('close')
      } else {
        this.$router.push({
          name: 'CustomerAppointmentPlanListView',
          query: { tab: this.$route.query.tab },
        })
      }
    },
    removeCustomer(index: number) {
      this.customers.splice(index, 1)
    },
    onCustomersSelectChange(newValue: any) {
      this.isUserInteracting = true
      console.log('👤 User เปลี่ยน customersSelect เป็น:', newValue)
      // ไม่ต้องทำอะไรเพิ่ม เพราะ v-model จะจัดการให้แล้ว
      setTimeout(() => {
        this.isUserInteracting = false
      }, 100)
    },
  },
  computed: {
    ConditionReadonly() {
      if (this.UserId === this.updateCommand.employeeId) {
        return false
      } else {
        return true
      }
    },
  },
  watch: {
    'updateCommand.organizationId': {
      async handler(newVal, oldVal) {
        if (newVal) {
          // ถ้าเป็นการเปลี่ยนแปลง organizationId ในระหว่างการแก้ไข (ไม่ใช่การโหลดครั้งแรก)
          if (oldVal !== undefined && oldVal !== newVal) {
            // รีเซ็ตข้อมูลลูกค้าที่เลือกเฉพาะเมื่อเปลี่ยน organization
            this.customersSelect = []
            this.customers = []
            console.log('เปลี่ยน organizationId จาก', oldVal, 'เป็น', newVal, '- รีเซ็ตลูกค้า')
          }

          // โหลด OrganizationContact ใหม่
          await this.getOrganizationContact()
        } else {
          // ถ้าไม่มี organizationId ให้เคลียร์ข้อมูล
          this.OrganizationContactList = []
          this.customersSelect = []
          this.customers = []
          console.log('ไม่มี organizationId - เคลียร์ข้อมูลทั้งหมด')
        }
      },
      immediate: true,
    },
    customersSelect: {
      handler(newVal, oldVal) {
        console.log('🔄 customersSelect เปลี่ยนจาก:', oldVal, 'เป็น:', newVal)
      },
      deep: true,
    },
  },
})
</script>

<style scoped>
.text-sub-title {
  font-size: clamp(20px, 4vw, 30px);
  font-weight: bold;
  color: #2b3086;
  background-clip: text;
  -webkit-background-clip: text;
  text-shadow: 5px 5px 8px rgba(65, 69, 92, 0.5);
  padding: 0;
  margin: 0;
  line-height: 1.2;
}

.card-form {
  background: #ffffff;
  border-radius: 15px;
  margin: 2px;
  /* box-shadow: 2px 2px 6px 6px rgba(43, 48, 134, 0.15); */
}

/* Global overflow prevention */
.v-application {
  overflow-x: hidden !important;
}

.v-main {
  overflow-x: hidden !important;
}

.v-container {
  overflow-x: hidden !important;
}

.v-row {
  overflow-x: hidden !important;
}

.v-col {
  overflow-x: hidden !important;
}

.scrollable-card {
  max-height: 90vh;
  overflow: hidden;
  overflow-x: hidden;
  display: flex;
  flex-direction: column;
}

.scrollable-content {
  overflow-y: auto;
  overflow-x: hidden;
  flex: 1;
  padding-right: 8px;
}

/* Form elements overflow prevention */
.v-text-field,
.v-select,
.v-autocomplete,
.v-textarea,
.v-btn {
  overflow-x: hidden !important;
}

.v-text-field .v-input__control,
.v-select .v-input__control,
.v-autocomplete .v-input__control,
.v-textarea .v-input__control {
  overflow-x: hidden !important;
}

.dashed-border {
  border: 1px dashed grey;
}

/* Enhanced responsive breakpoints */
/* Extra small devices (phones, 600px and down) */
@media (max-width: 599px) {
  .v-application,
  .v-main,
  .v-container,
  .v-row,
  .v-col {
    overflow-x: hidden !important;
  }

  .text-sub-title {
    font-size: 20px;
    padding: 8px 5px;
    text-align: center;
  }

  .card-form {
    margin: 5px 3px;
  }

  .scrollable-card {
    max-height: 85vh;
  }

  .scrollable-content {
    padding: 12px !important;
  }

  .px-2 {
    padding-left: 8px !important;
    padding-right: 8px !important;
  }

  /* All columns full width on mobile */
  .v-col[sm] {
    flex-basis: 100% !important;
    max-width: 100% !important;
  }

  .v-col[md] {
    flex-basis: 100% !important;
    max-width: 100% !important;
  }

  /* Hide empty columns on mobile */
  .d-none.d-md-block {
    display: none !important;
  }

  /* Adjust label sizes */
  label {
    font-size: 14px;
    font-weight: 500;
  }

  /* Mobile button styling - full width */
  .mobile-btn {
    width: 100% !important;
    min-width: 100% !important;
    margin-bottom: 8px !important;
    font-size: 16px !important;
    padding: 12px 16px !important;
  }

  /* Stack buttons vertically on mobile */
  .button-row {
    display: flex;
    flex-direction: column !important;
    gap: 8px;
  }

  /* Remove horizontal margins for mobile buttons */
  .mobile-btn.me-2 {
    margin-right: 0 !important;
  }

  /* Form field spacing */
  .v-col {
    padding-bottom: 12px;
  }
}

/* Small devices (portrait tablets and large phones, 600px and up) */
@media (min-width: 600px) and (max-width: 959px) {
  .v-application,
  .v-main,
  .v-container,
  .v-row,
  .v-col {
    overflow-x: hidden !important;
  }

  .text-sub-title {
    font-size: 24px;
    padding: 6px 10px;
    text-align: center;
  }

  .card-form {
    margin: 5px 8px;
  }

  .scrollable-card {
    max-height: 88vh;
  }

  .scrollable-content {
    padding: 16px !important;
  }

  /* Two columns on tablets */
  .v-col[sm="6"] {
    flex-basis: 50% !important;
    max-width: 50% !important;
  }

  .v-col[md="8"] {
    flex-basis: 100% !important;
    max-width: 100% !important;
  }

  /* Hide empty columns on tablets */
  .d-none.d-md-block {
    display: none !important;
  }

  label {
    font-size: 15px;
    font-weight: 500;
  }

  .v-btn {
    min-width: 140px;
    font-size: 15px;
  }

  .button-row {
    display: flex;
    gap: 12px;
  }
}

/* Medium devices (landscape tablets, 960px and up) */
@media (min-width: 960px) and (max-width: 1263px) {
  .v-application,
  .v-main,
  .v-container,
  .v-row,
  .v-col {
    overflow-x: hidden !important;
  }

  .text-sub-title {
    font-size: 28px;
    text-align: center;
  }

  .card-form {
    margin: 8px 16px;
  }

  .scrollable-card {
    max-height: 90vh;
  }

  .scrollable-content {
    padding: 20px !important;
  }

  label {
    font-size: 16px;
    font-weight: 500;
  }

  .v-btn {
    min-width: 160px;
    font-size: 16px;
  }

  .button-row {
    display: flex;
    gap: 16px;
  }
}

/* Large devices (laptops/desktops, 1264px and up) */
@media (min-width: 1264px) {
  .v-application,
  .v-main,
  .v-container,
  .v-row,
  .v-col {
    overflow-x: hidden !important;
  }

  .text-sub-title {
    font-size: 30px;
    text-align: center;
  }

  .card-form {
    margin: 10px 20px;
  }

  .scrollable-card {
    max-height: 90vh;
  }

  .scrollable-content {
    padding: 24px !important;
  }

  label {
    font-size: 16px;
    font-weight: 500;
  }

  .v-btn {
    min-width: 180px;
    font-size: 16px;
  }

  .button-row {
    display: flex;
    gap: 20px;
  }
}

.button-row > .v-btn {
  flex: 1;
}

/* Enhanced touch support */
.scrollable-card {
  touch-action: pan-y;
  overscroll-behavior: contain;
}

/* Improved iOS Safari support */
@supports (-webkit-touch-callout: none) {
  .scrollable-content {
    -webkit-overflow-scrolling: touch;
  }

  .v-btn {
    appearance: none;
    -webkit-appearance: none;
  }
}

/* Print styles */
@media print {
  .scrollable-card {
    max-height: none;
    overflow: visible;
  }

  .v-btn {
    display: none;
  }
}

/* High contrast mode support */
@media (prefers-contrast: high) {
  .text-sub-title {
    text-shadow: none;
  }
}

/* Reduce motion for accessibility */
@media (prefers-reduced-motion: reduce) {
  * {
    animation-duration: 0.01ms !important;
    animation-iteration-count: 1 !important;
    transition-duration: 0.01ms !important;
  }
}
</style>
