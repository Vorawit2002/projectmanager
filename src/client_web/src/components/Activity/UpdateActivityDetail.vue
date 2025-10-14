<template>
  <v-card
    class="text-form"
    elevation="0"
  >
    <v-form
      ref="form"
      @submit.prevent="UpdateForm"
      class="text-black"
    >
      <v-row class="mt-2 mb-0">
        <v-col
          cols="12"
          md="12"
        >
          <v-row
            class="d-flex"
            align="center"
          >
            <v-col
              cols="12"
              md="12"
            >
              <label class="mb-2">วันที่เริ่มต้น <span class="text-error">*</span> </label>
              <div class="form-field">
                <TextFieldDatepicker
                  v-if="localUpdateCommand.startDate"
                  placeholder="กรุณาระบุวันที่เริ่มต้นนัดหมาย"
                  :rules="appointmentStartDateRules"
                  :selectedDateTime="localUpdateCommand.startDate"
                  @selectedDateTime="changeTimestartDate"
                  :readonly="ConditionReadonly"
                  :AllDay="localUpdateCommand.allDay"
                />
                <TextFieldDatepicker
                  v-else
                  placeholder="กรุณาระบุวันที่เริ่มต้นนัดหมาย"
                  :rules="appointmentStartDateRules"
                  @selectedDateTime="changeTimestartDate"
                  :AllDay="localUpdateCommand.allDay"
                  :readonly="ConditionReadonly"
                />
              </div>
            </v-col>

            <v-col
              cols="12"
              md="12"
            >
              <label class="mb-2">วันที่สิ้นสุด <span class="text-error">*</span> </label>
              <div class="form-field">
                <TextFieldDatepicker
                  v-if="localUpdateCommand.endDate"
                  placeholder="กรุณาระบุวันที่สิ้นสุดนัดหมาย"
                  :rules="appointmentEndDateRules"
                  :selectedDateTime="localUpdateCommand.endDate"
                  @selectedDateTime="changeTimeendDate"
                  :AllDay="localUpdateCommand.allDay"
                  :readonly="ConditionReadonly"
                />
                <TextFieldDatepicker
                  v-else
                  placeholder="กรุณาระบุวันที่สิ้นสุดนัดหมาย"
                  :rules="appointmentEndDateRules"
                  @selectedDateTime="changeTimeendDate"
                  :AllDay="localUpdateCommand.allDay"
                  :readonly="ConditionReadonly"
                />
              </div>
            </v-col>

            <v-col
              cols="12"
              md="12"
              class="mt-md-4"
            >
              <v-checkbox
                label="ทั้งวัน"
                hide-details
                v-model="localUpdateCommand.allDay"
                :readonly="ConditionReadonly"
                class="form-field"
              />
            </v-col>
          </v-row>
        </v-col>

        <v-col
          cols="12"
          md="12"
        >
          <label class="mb-2">รหัสโครงการ </label>
          <v-autocomplete
            placeholder="กรุณาเลือกรหัสโครงการ"
            clearable
            v-model="localUpdateCommand.projectId"
            :items="ProjectList"
            item-title="projectCodeAndName"
            item-value="id"
            :readonly="ConditionReadonly"
            class="form-field"
          />
        </v-col>

        <v-col
          cols="12"
          md="12"
        >
          <label class="mb-2">หน่วยงาน / ลูกค้า <span class="text-error">*</span> </label>
          <v-autocomplete
            placeholder="กรุณาเลือกหน่วยงานหรือลูกค้า"
            clearable
            :items="OrganizationList"
            item-title="name"
            item-value="id"
            :rules="selectAgencyOrCustomerRules"
            v-model="localUpdateCommand.organizationId"
            :readonly="ConditionReadonly"
            class="form-field"
          />
        </v-col>

        <v-col
          cols="12"
          md="12"
        >
          <label class="mb-2">ลูกค้าที่นัดพบ <span class="text-error">*</span> </label>
          <v-autocomplete
            :key="`customers-${localUpdateCommand.organizationId}-${customersSelect.length}`"
            :items="OrganizationContactList"
            item-title="name"
            item-value="id"
            v-model="customersSelect"
            multiple
            :clearable="!ConditionReadonly"
            :rules="selectCustomerRules"
            placeholder="เลือกลูกค้าที่นัดพบ"
            :disabled="localUpdateCommand.organizationId === undefined || localUpdateCommand.organizationId === null"
            :readonly="ConditionReadonly"
            @update:model-value="onCustomersSelectChange"
            class="form-field"
          >
            <template v-slot:append-inner>
              <v-btn
                v-if="!ConditionReadonly"
                variant="text"
                class="cursor-pointer"
                icon="ri-add-circle-line"
                @click="addNewOrganizationContact"
              ></v-btn>
            </template>
          </v-autocomplete>
        </v-col>

        <v-col
          cols="12"
          md="12"
        >
          <label class="mb-2">สถานที่ <span class="text-error">*</span> </label>
          <v-text-field
            placeholder="อาคาร 2, ชั้น 12, ห้องประชุม A"
            :rules="appointmentLocationRules"
            v-model="localUpdateCommand.location"
            :readonly="ConditionReadonly"
            class="form-field"
          >
          </v-text-field>
        </v-col>

        <v-col
          cols="12"
          md="12"
        >
          <label class="mb-2">วัตถุประสงค์ <span class="text-error">*</span></label>
          <v-autocomplete
            placeholder="ระบุวัตถุประสงค์"
            :items="objectiveList"
            v-model="localUpdateCommand.objective"
            :rules="appointmentPurposeRules"
            clearable
            :readonly="ConditionReadonly"
            class="form-field"
          />
        </v-col>

        <v-col
          cols="12"
          md="12"
          v-if="localUpdateCommand.objective === 'อื่น ๆ'"
        >
          <label class="mb-2">วัตถุประสงค์อื่น ๆ <span class="text-error">*</span></label>
          <v-text-field
            placeholder="ระบุวัตถุประสงค์อื่น ๆ"
            :rules="otherPurposeRules"
            v-model="localUpdateCommand.objectiveDetail"
            :readonly="ConditionReadonly"
            class="form-field"
          />
        </v-col>

        <v-col cols="12">
          <label class="mb-2">รายละเอียดเพิ่มเติม </label>
          <v-textarea
            placeholder="ระบุรายละเอียดการนัดพบ"
            v-model="localUpdateCommand.detail"
            :readonly="ConditionReadonly"
            class="form-field"
          />
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
                  @click="CancelCreate"
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
                @click="CancelCreate"
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

  <v-dialog
    v-model="DialogCreateContact"
    transition="dialog-top-transition"
    class="z-indexDialog"
    :fullscreen="$vuetify.display.xs"
    max-width="900"
    content-class="custom-dialog-center"
  >
    <CreateOrganizationContact
      v-if="localUpdateCommand.organizationId"
      :CloseDialogCreate="CloseDialogCreate"
      :organization-id="localUpdateCommand.organizationId"
    />
  </v-dialog>
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
import CreateOrganizationContact from '@/components/OrganizationDialog/CreateOrganizationContact.vue'
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
  name: 'UpdateActivityDetail',
  components: {
    DemoFormLayoutVerticalFormWithIcons,
    TextFieldDatepicker,
    TextFieldTimepicker,
    CreateOrganizationContact,
  },
  props: {
    id: {
      type: [String, Number],
      default: null,
    },
    updateCommand: {
      type: Object,
      default: () => new UpdateActivityPlanCommand(),
    },
    isDialog: {
      type: Boolean,
      default: false,
    },
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
      localUpdateCommand: new UpdateActivityPlanCommand() as any, // ใช้ local copy
      startDate: undefined as any,
      endDate: undefined as any,
      selectedObjective: [] as any,
      otherObjective: '',
      customersSelect: [] as any,
      customers: [] as any,
      AllDay: false,
      loading: false,
      id: '' as any,
      DialogCreateContact: false,
      requestOrganizationContact: new GetOrganizationContactByOrganizationIdQuery(),
      ProjectList: [] as any,
      createContact: new CreateActivityPlanContactCommand(),
      sweetAlertStore: useSweetAlertStore(),
      auth: useAuthStore(),
      UserId: '' as any,
      appointmentStartDateRules,
      appointmentEndDateRules,
      selectAgencyOrCustomerRules,
      selectCustomerRules,
      appointmentLocationRules,
      appointmentPurposeRules,
      otherPurposeRules,
      // เพิ่ม flags เพื่อป้องกันการโหลดซ้ำ
      isLoadingOrganizationContact: false,
      isLoadingActivityContacts: false,
      lastLoadedOrganizationId: null as any,
      lastLoadedActivityPlanId: null as any,
      isUserInteracting: false, // flag เพื่อระบุว่า user กำลังแก้ไขข้อมูล
    }
  },
  methods: {
    async initialize() {
      try {
        // รีเซ็ตข้อมูล
        this.customersSelect = []
        this.customers = []

        // โหลดข้อมูลพื้นฐาน
        const result = await client.getEmployeeQueryByUserID(this.auth.userId)
        this.UserId = result.id

        // ถ้ามี id ให้โหลดข้อมูลกิจกรรม
        if (this.id) {
          const response = await client.getActivityPlanQueryByID(this.id)
          this.localUpdateCommand = Object.assign(new UpdateActivityPlanCommand(), response)
        } else if (this.updateCommand?.id) {
          this.localUpdateCommand = Object.assign(new UpdateActivityPlanCommand(), this.updateCommand)
        }

        // โหลดข้อมูลโครงการ
        this.ProjectList = await client.getProjectQuery()
        this.ProjectList.forEach((project: any) => {
          const truncatedName =
            project.projectName.length > 70 ? project.projectName.substring(0, 70) + '...' : project.projectName
          project.projectCodeAndName = `${project.projectCode} - ${truncatedName}`
        })

        // โหลด OrganizationContact หลังจากได้ organizationId แล้ว
        if (this.localUpdateCommand.organizationId) {
          await this.getOrganizationContact()

          // รอให้ OrganizationContact โหลดเสร็จก่อน แล้วค่อยโหลด ActivityPlanContact
          if (this.localUpdateCommand.id) {
            await this.loadActivityContacts()
          }
        }
      } catch (error) {
        console.error('เกิดข้อผิดพลาดใน initialize():', error)
      }
    },

    async loadActivityContacts() {
      try {
        if (!this.localUpdateCommand.id) {
          console.log('� ไม่มี activityPlanId')
          return
        }

        // ป้องกันการโหลดซ้ำ
        if (this.isLoadingActivityContacts || this.lastLoadedActivityPlanId === this.localUpdateCommand.id) {
          console.log('⏭️ ข้าม ActivityContact (กำลังโหลดหรือโหลดแล้ว)')
          return
        }

        this.isLoadingActivityContacts = true
        console.log('�🔍 เริ่มโหลด ActivityContact สำหรับ activityPlanId:', this.localUpdateCommand.id)

        let command = new GetActivityPlanContactByActivityPlanIdQuery()
        command.activityPlanId = this.localUpdateCommand.id
        const activityContact = await client.getActivityPlanContactQueryByActivityPlanId(command)

        console.log('📋 ActivityContact ที่โหลดได้:', activityContact.length, 'items')
        console.log(
          '📋 รายละเอียด:',
          activityContact.map((c: any) => ({
            id: c.id,
            organizationContactId: c.organizationContactId,
            activityPlanId: c.activityPlanId,
          })),
        )

        this.customers = activityContact

        // รีเซ็ตและอัพเดท customersSelect
        this.customersSelect = []

        // รอให้ Vue reactivity update
        await this.$nextTick()

        // ตั้งค่า customersSelect ใหม่
        if (activityContact && activityContact.length > 0) {
          // ลบ ID ที่ซ้ำออก
          const uniqueContactIds = [...new Set(activityContact.map((contact: any) => contact.organizationContactId))]

          // ตั้งค่าเฉพาะเมื่อ user ไม่ได้กำลังแก้ไข
          if (!this.isUserInteracting) {
            this.customersSelect = uniqueContactIds
            console.log('✅ ตั้งค่า customersSelect (ลบรายการซ้ำแล้ว):', [...this.customersSelect])
          } else {
            console.log('⏸️ ไม่ตั้งค่า customersSelect เพราะ user กำลังแก้ไข')
          }
        } else {
          console.log('⚠️ ไม่มี ActivityContact')
        }

        this.lastLoadedActivityPlanId = this.localUpdateCommand.id
        this.isLoadingActivityContacts = false

        // Force component re-render
        this.$forceUpdate()
      } catch (error) {
        console.error('❌ เกิดข้อผิดพลาดในการโหลด Activity Contacts:', error)
        this.isLoadingActivityContacts = false
      }
    },
    async UpdateForm() {
      const form = this.$refs.form as any
      const { valid } = await form.validate()
      if (valid) {
        this.loading = true
        try {
          // Debug: แสดงข้อมูลที่กำลังจะบันทึก
          console.log('💾 กำลังบันทึกข้อมูล localUpdateCommand:', JSON.stringify(this.localUpdateCommand, null, 2))
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
          const response = await client.updateActivityPlan(this.localUpdateCommand)
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
              this.createContact.activityPlanId = this.localUpdateCommand.id
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
              if (this.isDialog) {
                // ถ้าอยู่ใน dialog ให้ emit event กลับไป parent
                this.$emit('updated')
              } else {
                // ถ้าไม่ใช่ dialog ให้ navigate ตามปกติ
                this.navigateBackWithState()
              }
            }, 600)
          }
        } catch (error) {
          console.error(error)
          setTimeout(() => {
            this.loading = false
            this.sweetAlertStore.error('เกิดข้อผิดพลาดในการแก้ไขข้อมูล ล้มเหลว!')
            if (this.isDialog) {
              // ถ้าอยู่ใน dialog ให้ emit event กลับไป parent
              this.$emit('updated')
            } else {
              // ถ้าไม่ใช่ dialog ให้ navigate ตามปกติ
              this.navigateBackWithState()
            }
          }, 600)
        }
      }
    },
    async addNewOrganizationContact() {
      this.DialogCreateContact = true
    },
    async CloseDialogCreate(value: boolean, reload: boolean) {
      this.DialogCreateContact = value
      if (reload === true) {
        this.getOrganizationContact()
      }
    },
    changeTimestartDate(value: any) {
      this.localUpdateCommand.startDate = value
      this.$emit('update:update-command', this.localUpdateCommand)
    },
    changeTimeendDate(value: any) {
      this.localUpdateCommand.endDate = value
      this.$emit('update:update-command', this.localUpdateCommand)
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
        if (!this.localUpdateCommand.organizationId) {
          this.OrganizationContactList = []
          this.customersSelect = []
          this.lastLoadedOrganizationId = null
          console.log('🚫 ไม่มี organizationId')
          return
        }

        // ป้องกันการโหลดซ้ำ
        if (
          this.isLoadingOrganizationContact ||
          this.lastLoadedOrganizationId === this.localUpdateCommand.organizationId
        ) {
          console.log('⏭️ ข้าม OrganizationContact (กำลังโหลดหรือโหลดแล้ว)')
          return
        }

        this.isLoadingOrganizationContact = true
        console.log('🔍 โหลด OrganizationContact สำหรับ organizationId:', this.localUpdateCommand.organizationId)

        this.requestOrganizationContact.organizationId = this.localUpdateCommand.organizationId

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

        this.lastLoadedOrganizationId = this.localUpdateCommand.organizationId
        this.isLoadingOrganizationContact = false
      } catch (error) {
        console.error('❌ เกิดข้อผิดพลาดในการโหลด OrganizationContact:', error)
        this.OrganizationContactList = []
        this.isLoadingOrganizationContact = false
      }
    },

    // แก้ไข method CancelCreate ให้กลับไปพร้อมกับ state เดิม
    CancelCreate(): void {
      if (this.isDialog) {
        // ถ้าอยู่ใน dialog ให้ emit event กลับไป parent
        this.$emit('cancel')
      } else {
        // ถ้าไม่ใช่ dialog ให้ navigate ตามปกติ
        this.navigateBackWithState()
      }
    },

    // เพิ่ม method สำหรับ navigate กลับพร้อมกับ state
    navigateBackWithState(): void {
      // ดึง tab จาก route query (ถ้ามี)
      const currentTab = this.$route.query.tab as string

      // สร้าง query object สำหรับกลับไป
      const queryParams: any = {}
      if (currentTab) {
        queryParams.tab = currentTab
      }

      // ตรวจสอบว่ามี route history หรือไม่
      if (window.history.length > 1) {
        // ถ้ามี history ให้ back พร้อมกับ query
        this.$router.go(-1)
      } else {
        // ถ้าไม่มี history ให้ไปที่หน้า list โดยตรง
        this.$router.push({
          name: 'CustomerAppointmentPlanListView', // ชื่อ route ของหน้า list
          query: queryParams,
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
  watch: {
    id: {
      handler(newVal) {
        if (newVal) {
          this.id = newVal
          this.initialize()
        }
      },
      immediate: true,
    },
    updateCommand: {
      async handler(newVal) {
        if (newVal && Object.keys(newVal).length > 0) {
          // ตรวจสอบว่าเป็นการโหลดครั้งแรกหรือไม่
          const isInitialLoad = !this.localUpdateCommand.id || this.localUpdateCommand.id !== newVal.id

          if (isInitialLoad) {
            // กรณีโหลดครั้งแรก ให้ sync ข้อมูลทั้งหมด
            const oldOrganizationId = this.localUpdateCommand.organizationId
            this.localUpdateCommand = Object.assign(new UpdateActivityPlanCommand(), newVal)

            // ถ้า organizationId เปลี่ยนแปลง ให้โหลด OrganizationContact ใหม่
            if (oldOrganizationId !== this.localUpdateCommand.organizationId) {
              if (this.localUpdateCommand.organizationId) {
                await this.getOrganizationContact()
                // โหลด ActivityContact เฉพาะเมื่อเป็นการ sync ข้อมูลเริ่มต้น
                if (this.localUpdateCommand.id) {
                  await this.loadActivityContacts()
                }
              }
            }
          }
          // ถ้าไม่ใช่การโหลดครั้งแรก ไม่ต้องทำอะไร เพื่อไม่ให้ overwrite ข้อมูลที่ user กำลังแก้ไข
        }
      },
      immediate: true,
      deep: true,
    },
    localUpdateCommand: {
      handler(newVal) {
        // Emit เฉพาะเมื่อมีการเปลี่ยนแปลงจริงๆ และไม่ใช่การโหลดเริ่มต้น
        if (newVal && newVal.id) {
          this.$emit('update:update-command', newVal)
        }
      },
      deep: true,
    },
    'localUpdateCommand.organizationId': {
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

          // โหลด ActivityContact เฉพาะเมื่อมี id และไม่ได้เปลี่ยน organizationId
          if (this.localUpdateCommand.id && (oldVal === undefined || oldVal === newVal)) {
            console.log('โหลด ActivityContact สำหรับ organizationId:', newVal)
            await this.loadActivityContacts()
          }
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
  computed: {
    updateCommand(): any {
      return this.localUpdateCommand
    },
    ConditionReadonly() {
      if (this.UserId && this.localUpdateCommand.employeeId) {
        return this.UserId !== this.localUpdateCommand.employeeId
      }

      return false
    },
  },
  async mounted() {
    // ใช้ props id ถ้ามี หรือ route params ถ้าไม่มี props
    this.id = this.id || this.$route.params.id

    // โหลด Organization ก่อน
    await this.getOrganization()

    // โหลดข้อมูลหลัก
    await this.initialize()

    await this.$nextTick()
    ;(this.$refs.form as any).validate()
  },
})
</script>

<style scoped>
.text-sub-title {
  font-size: 30px;
  font-weight: bold;
  color: #2b3086;
  background-clip: text;
  -webkit-background-clip: text;
  text-shadow: 5px 5px 8px rgba(65, 69, 92, 0.5);
  padding: 0;
  margin: 0;
}

.card-form {
  background: #ffffff;
  border-radius: 15px;
  margin: 2px;
  /* box-shadow: 2px 2px 6px 6px rgba(43, 48, 134, 0.15); */
}

.dashed-border {
  border: 1px dashed grey;
}

/* Mobile: <= 599px */
@media (max-width: 599px) {
  .text-sub-title {
    font-size: 24px;
    padding: 8px 5px;
    text-align: center;
  }

  .card-form {
    margin: 5px 3px;
  }

  /* กำหนดให้ฟอร์มคอลัมน์เต็ม 12 คอลัมน์เสมอ */
  .v-col[md] {
    flex-basis: 100% !important;
    max-width: 100% !important;
  }

  /* เพิ่มระยะห่างในช่อง input */
  .v-col > label {
    font-size: 14px;
  }
  .action-btn-row {
    margin-bottom: 30% !important;
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
}

/* Tablet: 600px - 959px */
@media (min-width: 600px) and (max-width: 959px) {
  .text-sub-title {
    font-size: 28px;
    padding: 6px 10px;
    text-align: center;
  }

  .card-form {
    margin: 5px 8px;
  }

  /* ใช้คอลัมน์ md เท่าที่มี แต่วางตัวเรียงกัน */
  .v-col[md='4'] {
    flex-basis: 33.3333% !important;
    max-width: 33.3333% !important;
  }
  .v-col[md='8'] {
    flex-basis: 66.6666% !important;
    max-width: 66.6666% !important;
  }

  /* ปรับขนาด label และ padding */
  label {
    font-size: 15px;
  }
}

/* Desktop: >= 960px */
@media (min-width: 960px) {
  .text-sub-title {
    font-size: 30px;
    padding: 0;
    text-align: left;
  }

  .card-form {
    margin: 10px 20px;
  }

  label {
    font-size: 16px;
  }
}

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

.cancel-btn:focus,
.submit-btn:focus {
  outline: none !important;
  box-shadow: 0 0 0 3px rgba(255, 255, 255, 0.3), 0 4px 8px rgba(0, 0, 0, 0.15) !important;
}

.gap-2 {
  gap: 8px;
}

@media (max-width: 599px) {
  .mobile-btn {
    width: 100%;
  }

  .action-btn {
    width: 100%;
    min-width: unset;
    margin-bottom: 0;
    margin-top: 0;
    box-sizing: border-box;
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
/* จัด dialog CreateOrganizationContact ให้อยู่ตรงกลาง desktop/mobile และขยาย desktop ให้กว้างขึ้น */
.custom-dialog-center {
  display: flex !important;
  align-items: center;
  justify-content: center;
  min-height: 100vh;
  margin: 0 auto;
  padding: 0 !important;
  max-width: 900px !important;
}
@media (max-width: 599px) {
  .custom-dialog-center {
    min-height: 100vh;
    border-radius: 20px !important;
    margin: 0 !important;
    max-width: 100vw !important;
  }
}
</style>
