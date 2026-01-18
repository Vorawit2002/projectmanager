<template>
  <div class="page-container dialog-scrollbar">
    <v-card
      class="text-form card-form px-3 px-sm-5 px-md-6"
      elevation="0"
    >
      <!-- Card Title -->
      <v-row class="mt-2 mt-sm-3 mb-3 mb-sm-4">
        <v-col
          cols="12"
          class="d-flex justify-center"
        >
          <span class="text-sub-title text-center">เพิ่มนัดหมายใหม่</span>
        </v-col>
      </v-row>

      <v-card-text class="px-0 px-sm-3">
        <v-form
          ref="form"
          @submit.prevent="createActivityPlan"
          class="text-black"
        >
          <!-- Date Section -->
          <v-card
            class="form-section mb-4 mb-sm-6"
            elevation="1"
          >
            <v-card-title class="section-title py-3">
              <v-icon
                class="mr-2"
                color="primary"
                >ri-calendar-line</v-icon
              >
              ข้อมูลวันเวลา
            </v-card-title>
            <v-card-text class="pt-2">
              <v-row>
                <v-col
                  cols="12"
                  sm="12"
                  md="12"
                >
                  <label class="field-label mb-2">วันที่เริ่มต้น <span class="text-error">*</span></label>
                  <div class="form-field">
                    <TextFieldDatepicker
                      placeholder="กรุณาระบุวันที่เริ่มต้นนัดหมาย"
                      :rules="appointmentStartDateRules"
                      :AllDay="createCommand.allDay"
                      @selectedDateTime="changeTimestartDate"
                    />
                  </div>
                </v-col>

                <v-col
                  cols="12"
                  sm="12"
                  md="12"
                >
                  <label class="field-label mb-2">วันที่สิ้นสุด <span class="text-error">*</span></label>
                  <div class="form-field">
                    <TextFieldDatepicker
                      placeholder="กรุณาระบุวันที่สิ้นสุดนัดหมาย"
                      :rules="appointmentEndDateRules"
                      :AllDay="createCommand.allDay"
                      @selectedDateTime="changeTimeendDate"
                    />
                  </div>
                </v-col>

                <v-col
                  cols="12"
                  class="mt-2"
                >
                  <v-checkbox
                    label="ทั้งวัน"
                    hide-details
                    v-model="createCommand.allDay"
                    color="primary"
                    class="all-day-checkbox form-field"
                  />
                </v-col>
              </v-row>
            </v-card-text>
          </v-card>

          <!-- Project & Organization Section -->
          <v-card
            class="form-section mb-4 mb-sm-6"
            elevation="1"
          >
            <v-card-title class="section-title py-3">
              <v-icon
                class="mr-2"
                color="primary"
                >ri-building-line</v-icon
              >
              ข้อมูลโครงการและหน่วยงาน
            </v-card-title>
            <v-card-text class="pt-2">
              <v-row>
                <v-col
                  cols="12"
                  sm="12"
                  lg="12"
                >
                  <label class="field-label mb-2">รหัสโครงการ</label>
                  <v-autocomplete
                    placeholder="กรุณาเลือกรหัสโครงการ"
                    clearable
                    v-model="createCommand.projectId"
                    :items="ProjectList"
                    item-title="projectCodeAndName"
                    item-value="id"
                    variant="outlined"
                    density="comfortable"
                    class="form-field"
                  />
                </v-col>

                <v-col
                  cols="12"
                  sm="12"
                  lg="12"
                >
                  <label class="field-label mb-2">หน่วยงาน / ลูกค้า <span class="text-error">*</span></label>
                  <v-autocomplete
                    placeholder="กรุณาเลือกหน่วยงานหรือลูกค้า"
                    :items="OrganizationList"
                    item-title="name"
                    item-value="id"
                    clearable
                    :rules="selectAgencyOrCustomerRules"
                    v-model="createCommand.organizationId"
                    @update:model-value="getOrganizationContact"
                    @update:search="handleSearchUpdate"
                    variant="outlined"
                    density="comfortable"
                    class="form-field"
                  >
                    <template v-slot:append-inner>
                      <v-btn
                        v-if="showAddButton"
                        variant="text"
                        class="cursor-pointer add-btn"
                        icon="ri-add-circle-line"
                        size="small"
                        color="primary"
                        @click="addNewOrganization"
                      ></v-btn>
                    </template>
                  </v-autocomplete>
                </v-col>

                <v-col
                  cols="12"
                  sm="12"
                  lg="12"
                >
                  <label class="field-label mb-2">ลูกค้าที่นัดพบ <span class="text-error">*</span></label>
                  <v-autocomplete
                    :items="OrganizationContactList"
                    item-title="name"
                    item-value="id"
                    multiple
                    :disabled="createCommand.organizationId === undefined || createCommand.organizationId === null"
                    :rules="selectCustomerRules"
                    placeholder="เลือกลูกค้าที่นัดพบ"
                    @update:search="handleSearchContactUpdate"
                    v-model="customers"
                    variant="outlined"
                    density="comfortable"
                    class="form-field"
                  >
                    <template v-slot:append-inner>
                      <v-btn
                        variant="text"
                        class="cursor-pointer add-btn"
                        icon="ri-add-circle-line"
                        size="small"
                        color="primary"
                        @click="addNewOrganizationContact"
                      ></v-btn>
                    </template>
                  </v-autocomplete>
                </v-col>
              </v-row>
            </v-card-text>
          </v-card>

          <!-- Meeting Details Section -->
          <v-card
            class="form-section mb-4 mb-sm-6"
            elevation="1"
          >
            <v-card-title class="section-title py-3">
              <v-icon
                class="mr-2"
                color="primary"
                >ri-information-line</v-icon
              >
              รายละเอียดการประชุม
            </v-card-title>
            <v-card-text class="pt-2">
              <v-row>
                <v-col
                  cols="12"
                  sm="6"
                  md="12"
                >
                  <label class="field-label mb-2">สถานที่ <span class="text-error">*</span></label>
                  <v-text-field
                    placeholder="อาคาร 2, ชั้น 12, ห้องประชุม A"
                    :rules="appointmentLocationRules"
                    v-model="createCommand.location"
                    variant="outlined"
                    density="comfortable"
                    prepend-inner-icon="ri-map-pin-line"
                    class="form-field"
                  />
                </v-col>

                <v-col
                  cols="12"
                  sm="6"
                  md="12"
                >
                  <label class="field-label mb-2">วัตถุประสงค์ <span class="text-error">*</span></label>
                  <v-autocomplete
                    placeholder="ระบุวัตถุประสงค์"
                    :items="objectiveList"
                    :rules="appointmentPurposeRules"
                    v-model="createCommand.objective"
                    clearable
                    variant="outlined"
                    density="comfortable"
                    class="form-field"
                  />
                </v-col>

                <v-col
                  cols="12"
                  sm="12"
                  md="4"
                  v-if="createCommand.objective === 'อื่น ๆ'"
                >
                  <label class="field-label mb-2">วัตถุประสงค์อื่น ๆ</label>
                  <v-text-field
                    placeholder="ระบุวัตถุประสงค์อื่น ๆ"
                    :rules="otherPurposeRules"
                    v-model="createCommand.objectiveDetail"
                    variant="outlined"
                    density="comfortable"
                    class="form-field"
                  />
                </v-col>

                <v-col cols="12">
                  <label class="field-label mb-2">รายละเอียดเพิ่มเติม</label>
                  <v-textarea
                    v-model="createCommand.detail"
                    placeholder="ระบุรายละเอียดการนัดพบ"
                    variant="outlined"
                    rows="4"
                    auto-grow
                    density="comfortable"
                  />
                </v-col>
              </v-row>
            </v-card-text>
          </v-card>

          <!-- Action Buttons -->
          <v-row class="mt-4 mt-sm-6">
            <v-col
              cols="12"
              class="action-btn-row"
            >
              <div v-if="$vuetify.display.mobile">
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
              </div>
              <div
                v-else
                class="d-flex justify-center flex-wrap gap-2"
              >
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
            </v-col>
          </v-row>
        </v-form>
      </v-card-text>
    </v-card>
  </div>

  <v-navigation-drawer
    v-model="DialogCreateContact"
    :width="$vuetify.display.xs ? '100vw' : '550'"
    class="z-indexDialog create-activity-drawer"
    close-on-back
    temporary
    transition="dialog-righ-transition"
    location="right"
    scrollable
    :permanent="false"
    :scrim="true"
    :scrim-opacity="0.5"
    :persistent="false"
    @update:model-value="handleDrawerModelUpdate"
  >
    <CreateOrganizationContact
      v-if="DialogCreateContact && createCommand.organizationId"
      :CloseDialogCreate="CloseDialogCreate"
      :organization-id="createCommand.organizationId"
    />
  </v-navigation-drawer>

  <!-- <v-dialog
    v-model="DialogCreateOrganization"
    max-width="600px"
    persistent
    class="custom-dialog"
  >
    <v-card class="dialog-card">
      <v-card-title class="dialog-header d-flex justify-space-between align-center">
        <span class="dialog-title">เพิ่มหน่วยงาน / ลูกค้าใหม่</span>
        <v-btn
          icon
          variant="text"
          size="small"
          @click="CloseDialogOrganiz(false, false)"
          class="close-btn"
        >
          <v-icon>ri-close-line</v-icon>
        </v-btn>
      </v-card-title>
      <v-card-text class="dialog-content">
        <div class="scrollable-content dialog-scrollbar">
          <CreateOrganization :CloseDialogOrganiz="CloseDialogOrganiz" />
        </div>
      </v-card-text>
    </v-card>
  </v-dialog> -->
</template>

<script lang="ts">
import {
  Client,
  CreateActivityPlanCommand,
  CreateActivityPlanContactCommand,
  CreateOrganizationContactJustNameCommand,
  CreateOrganizationJustNameCommand,
  GetOrganizationContactByOrganizationIdQuery,
} from '@/client'
import TextFieldDatepicker from '@/components/Datepicker/TextFieldDatepicker.vue'
import TextFieldTimepicker from '@/components/Datepicker/TextFieldTimepicker.vue'
import CreateOrganization from '@/components/OrganizationDialog/CreateOrganization.vue' // Import component
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
import { defineComponent } from 'vue'

const client = new Client(BACKEND_API_URL)
export default defineComponent({
  name: 'CreateActivityDetail',
  components: {
    TextFieldDatepicker,
    TextFieldTimepicker,
    CreateOrganizationContact,
    CreateOrganization, // Register component
  },
  props: {
    CloseDialogCreated: {
      type: Function,
      required: true,
    },
  },
  data() {
    return {
      OrganizationList: [] as any,
      ProjectList: [] as any,
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
      startDate: undefined as any,
      endDate: undefined as any,
      selectedObjective: [] as any, // เก็บวัตถุประสงค์ที่เลือก
      otherObjective: '', // เก็บวัตถุประสงค์อื่น ๆ ที่กรอก
      searchText: '' as any,
      searchTextContact: '' as any,
      customers: [] as any,
      AllDay: false,
      createCommand: new CreateActivityPlanCommand({ allDay: false }),
      requestOrganizationContact: new GetOrganizationContactByOrganizationIdQuery(),
      createOrganization: new CreateOrganizationJustNameCommand(),
      auth: useAuthStore(),
      createOrganizationContact: new CreateOrganizationContactJustNameCommand(),
      sweetAlertStore: useSweetAlertStore(),
      loading: false,
      DialogCreateContact: false, // State สำหรับ Drawer เพิ่ม Contact
      DialogCreateOrganization: false, // State สำหรับ Dialog เพิ่ม Organization
      createContact: new CreateActivityPlanContactCommand(),
      User: {} as any,
      appointmentStartDateRules,
      appointmentEndDateRules,
      selectAgencyOrCustomerRules,
      selectCustomerRules,
      appointmentLocationRules,
      appointmentPurposeRules,
      otherPurposeRules,
    }
  },
  async mounted() {
    await this.initialize()
    // Add ESC key event listener
    document.addEventListener('keydown', this.handleEscKey)
  },
  beforeUnmount() {
    // Remove ESC key event listener
    document.removeEventListener('keydown', this.handleEscKey)
  },
  computed: {
    showAddButton() {
      // แสดงปุ่มเฉพาะเมื่อ: มีการพิมพ์ค้นหา และไม่พบรายการที่ตรงกัน
      return this.searchText && this.searchText.trim() !== '' && !this.hasMatchingItems
    },
    hasMatchingItems() {
      if (!this.searchText || this.searchText.trim() === '') return true
      return this.OrganizationList.some((item: any) => item.name.toLowerCase().includes(this.searchText.toLowerCase()))
    },
    showAddButtonContact() {
      // แสดงปุ่มบวกเฉพาะตอนผู้ใช้พิมพ์ค้นหา แล้วไม่เจอ contact ที่ตรงกันเท่านั้น
      return this.searchTextContact && this.searchTextContact.trim() !== '' && !this.hasMatchingItemsContact
    },
    hasMatchingItemsContact() {
      if (!this.searchTextContact || this.searchTextContact.trim() === '') return true
      return this.OrganizationContactList.some((item: any) =>
        item.name.toLowerCase().includes(this.searchTextContact.toLowerCase()),
      )
    },
  },
  methods: {
    /**
     * @description จัดการการกดปุ่ม ESC เพื่อปิด Dialog/Drawer ที่เปิดอยู่
     * @param {KeyboardEvent} event - เหตุการณ์ Keyboard
     */
    handleEscKey(event: KeyboardEvent) {
      if (event.key === 'Escape' || event.keyCode === 27) {
        if (this.DialogCreateContact) {
          this.DialogCreateContact = false
          event.stopPropagation() // ไม่ให้ event bubbling
          return // หยุดการทำงานต่อ
        }

        if (this.DialogCreateOrganization) {
          this.DialogCreateOrganization = false
          event.stopPropagation()
          return
        }

        this.CancelCreate(false)
      }
    },
    /**
     * @description ดักจับการเปลี่ยนแปลงของ v-model ของ v-navigation-drawer
     *              เมื่อผู้ใช้คลิกนอก Drawer หรือกด ESC, v-model จะเปลี่ยนเป็น false
     *              ฟังก์ชันนี้จะช่วยให้ state ใน data() ของเราตรงกับสถานะจริงของ Drawer
     * @param {boolean} value - ค่าใหม่ของ v-model (true = เปิด, false = ปิด)
     */
    handleDrawerModelUpdate(value: boolean) {
      this.DialogCreateContact = value
      // ถ้า Drawer ปิด และต้องการ reload ข้อมูล (เช่น หลังจากเพิ่ม Contact สำเร็จ)
      if (!value) {
        // สามารถเรียก getOrganizationContact() ที่นี่ได้ หากต้องการให้ข้อมูล Contact อัปเดตทันทีที่ Drawer ปิด
        // this.getOrganizationContact();
      }
    },
    handleSearchUpdate(value: any) {
      this.searchText = value
    },
    handleSearchContactUpdate(value: any) {
      this.searchTextContact = value
    },

    async addNewOrganization() {
      this.DialogCreateOrganization = true
    },

    async CloseDialogOrganiz(value: boolean, reload: boolean, newOrgId: string | null = null) {
      this.DialogCreateOrganization = value

      if (reload === true) {
        await this.initialize()
        if (newOrgId) {
          this.createCommand.organizationId = newOrgId
        }
      }
    },

    async addNewOrganizationContact() {
      if (!this.createCommand.organizationId) {
        this.sweetAlertStore.warning('กรุณาเลือกหน่วยงาน / ลูกค้าก่อนเพิ่มลูกค้าที่นัดพบ')
        return
      }
      if (this.DialogCreateContact) return // ป้องกันเปิดซ้ำ
      console.log('addNewOrganizationContact called')
      this.DialogCreateContact = true
    },
    /**
     * @description ปิด Drawer เพิ่ม Contact และจัดการการโหลดข้อมูล Contact ใหม่
     * @param {boolean} value - สถานะของ Drawer (true = เปิด, false = ปิด)
     * @param {boolean} reload - ระบุว่าควรโหลดข้อมูล OrganizationContactList ใหม่หรือไม่
     */
    async CloseDialogCreate(value: boolean, reload: boolean) {
      this.DialogCreateContact = value
      if (reload === true) {
        await this.getOrganizationContact() // โหลด OrganizationContactList ใหม่
      }
    },
    /**
     * @description โหลดข้อมูลเริ่มต้นสำหรับหน้าจอ (OrganizationList, ProjectList, User Info)
     */
    async initialize() {
      try {
        this.OrganizationList = await client.getOrganizationQuery()
        if (this.auth.userId) {
          console.log(this.auth.userId)
          const result = await client.getEmployeeQueryByUserID(this.auth.userId)
          this.createCommand.employeeId = result.id
          this.User = result
        }

        this.ProjectList = await client.getProjectQuery()
        this.ProjectList.forEach((project: any) => {
          const truncatedName =
            project.projectName.length > 70 ? project.projectName.substring(0, 70) + '...' : project.projectName
          project.projectCodeAndName = `${project.projectCode} - ${truncatedName}`
        })
      } catch (error) {
        console.error('Error initializing data:', error)
        this.sweetAlertStore.error('เกิดข้อผิดพลาดในการโหลดข้อมูลเริ่มต้น')
      }
    },
    /**
     * @description โหลดข้อมูล Contact ของ Organization ที่เลือก
     */
    async getOrganizationContact() {
      try {
        if (!this.createCommand.organizationId) {
          this.OrganizationContactList = [] // เคลียร์รายการถ้าไม่มี organizationId
          this.customers = [] // เคลียร์ลูกค้าที่เลือกไว้ด้วย
          return
        }
        this.requestOrganizationContact.organizationId = this.createCommand.organizationId
        const contacts = await client.getOrganizationContactQueryByOrganizationId(this.requestOrganizationContact)
        this.OrganizationContactList = contacts.map((oc: any) => ({
          ...oc,
          name: `${oc.firstName || ''}   ${oc.lastName || ''}`.trim(),
        }))
        console.log(this.OrganizationContactList)
      } catch (error) {
        console.error('Error fetching organization contacts:', error)
        this.sweetAlertStore.error('เกิดข้อผิดพลาดในการโหลดข้อมูลลูกค้าที่นัดพบ')
      }
    },
    /**
     * @description ยกเลิกการสร้างกิจกรรมและปิด Component หลัก
     * @param {boolean} reload - ระบุว่าควรโหลดข้อมูลหน้าหลักใหม่หรือไม่
     */
    CancelCreate(reload: boolean = false) {
      this.CloseDialogCreated(false, reload)
    },
    removeCustomer(index: number) {
      this.customers.splice(index, 1)
    },
    changeTimestartDate(value: any) {
      this.createCommand.startDate = value
    },
    changeTimeendDate(value: any) {
      this.createCommand.endDate = value
    },
    /**
     * @description สร้างแผนกิจกรรมใหม่
     */
    async createActivityPlan() {
      const form = this.$refs.form as any
      const { valid } = await form.validate()
      if (valid) {
        this.loading = true
        try {
          this.createCommand.eventType = '001'
          const response = await client.createActivityPlan(this.createCommand)
          if (this.customers.length > 0) {
            // สร้าง Activity Plan Contact สำหรับลูกค้าที่เลือกไว้
            for (const customerId of this.customers) {
              this.createContact.activityPlanId = response
              this.createContact.organizationContactId = customerId
              const result = await client.createActivityPlanContact(this.createContact)
              if (result) console.log(`Activity Plan Contact created for ${customerId}`)
            }
          }

          if (response) {
            this.sweetAlertStore.successDeleted('สร้างแผนการนัดพบลูกค้าสำเร็จ!!')
            setTimeout(() => {
              this.CancelCreate(true) // ปิด Component และสั่งให้หน้าหลักโหลดข้อมูลใหม่
            }, 1500)
          }
        } catch (error) {
          console.error('Error creating activity plan:', error)
          this.sweetAlertStore.error('เกิดข้อผิดพลาดในการสร้างแผนการนัดพบลูกค้า')
        } finally {
          this.loading = false
        }
      }
    },
  },
})
</script>

<style scoped>
/* Page Container for Scrolling */
.page-container {
  width: 100%;
  min-height: 100vh;
  overflow-y: auto;
  overflow-x: hidden;
  padding: 16px 0 32px 0;
  scroll-behavior: smooth;
  position: relative;
}

/* Ensure body and html allow scrolling */
body,
html {
  height: 100%;
  overflow: auto;
}

/* Title Styling */
.text-sub-title {
  font-size: clamp(20px, 4vw, 30px);
  font-weight: 700;
  color: #2b3086;
  background: linear-gradient(135deg, #2b3086 0%, #4a6cf7 100%);
  background-clip: text;
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  text-shadow: 0 2px 4px rgba(43, 48, 134, 0.2);
  letter-spacing: -0.5px;
}

/* Card Styling */
/* .card-form {
  background: linear-gradient(145deg, #ffffff 0%, #f8f9ff 100%);
  border-radius: 20px;
  box-shadow: 0 8px 32px rgba(43, 48, 134, 0.1) !important;
  border: 1px solid rgba(255, 255, 255, 0.2);
  backdrop-filter: blur(10px);
  max-width: 1200px;
  width: 100%;
  margin: 0 auto;
  min-height: auto;
  position: relative;
  flex-shrink: 0;
} */

/* Form Section Cards */
.form-section {
  background: rgba(255, 255, 255, 0.9);
  border-radius: 12px;
  border: 1px solid rgba(43, 48, 134, 0.1);
  transition: all 0.3s ease;
}

/* Section Titles */
.section-title {
  font-size: 16px;
  font-weight: 600;
  color: #2b3086;
  background: linear-gradient(90deg, rgba(43, 48, 134, 0.1) 0%, rgba(74, 108, 247, 0.05) 100%);
  border-bottom: 1px solid rgba(43, 48, 134, 0.1);
}

/* Field Labels */
.field-label {
  font-size: 14px;
  font-weight: 500;
  color: #374151;
  display: block;
}

/* Form Fields */
.v-text-field,
.v-autocomplete,
.v-textarea {
  margin-bottom: 8px;
}

:deep(.v-field--variant-outlined) {
  border-radius: 8px;
}

:deep(.v-field--variant-outlined .v-field__outline) {
  border-color: rgba(43, 48, 134, 0.2);
}

:deep(.v-field--variant-outlined:hover .v-field__outline) {
  border-color: rgba(43, 48, 134, 0.4);
}

:deep(.v-field--variant-outlined.v-field--focused .v-field__outline) {
  border-color: #2b3086;
  border-width: 2px;
}

/* Checkbox Styling */
.all-day-checkbox {
  background: rgba(43, 48, 134, 0.05);
  border-radius: 8px;
  padding: 8px 12px;
  margin-top: 8px;
}

/* Add Button Styling */
.add-btn {
  opacity: 0.7;
  transition: all 0.2s ease;
}

.add-btn:hover {
  opacity: 1;
  transform: scale(1.1);
}

/* Action Buttons */
.action-btn {
  min-width: 120px;
  font-weight: 500;
  text-transform: none;
  border-radius: 8px;
  transition: all 0.3s ease;
}

.action-btn:hover {
  transform: translateY(-1px);
}

/* Error Text */
.text-error {
  color: #dc2626;
  font-weight: 500;
}

/* Mobile Responsive Adjustments */
@media (max-width: 599px) {
  .page-container {
    padding: 0px;
    height: 100vh;
  }

  .card-form {
    margin: 0 8px;
    border-radius: 16px;
  }

  .text-sub-title {
    font-size: 25px;
    line-height: 2;
    margin-bottom: 0px;
  }

  .section-title {
    font-size: 14px;
    padding: 12px 16px;
  }

  .form-section {
    margin-bottom: 16px;
  }

  .action-btn-row {
    margin-bottom: 13% !important;
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

/* Tablet Responsive Adjustments */
@media (min-width: 600px) and (max-width: 959px) {
  .page-container {
    padding: 12px 0 28px 0;
    height: 100vh;
  }

  .card-form {
    margin: 0 16px;
  }

  .text-sub-title {
    font-size: 26px;
  }
}

/* Desktop Responsive Adjustments */
@media (min-width: 960px) {
  .page-container {
    padding: 20px 0 40px 0;
    height: 100vh;
  }

  .card-form {
    margin: 0 auto;
  }
}

/* Additional content to test scrolling */
/* .card-form::after {
  content: '';
  display: block;
  height: 50px;
  width: 100%;
} */

/* Loading State */
:deep(.v-btn--loading) {
  pointer-events: none;
}

/* Focus States for Accessibility */
:deep(.v-field--focused) {
  box-shadow: 0 0 0 2px rgba(43, 48, 134, 0.2);
}

/* Dialog Responsive */
@media (max-width: 599px) {
  :deep(.v-dialog) {
    margin: 0;
  }
}

/* Animation for form sections */
.form-section {
  animation: slideInUp 0.6s ease-out;
}

@keyframes slideInUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* Z-index for dialogs */
.z-indexDialog {
  z-index: 9999;
}

/* Mobile drawer fix - ensure drawer is closed on page load */
.create-activity-drawer {
  z-index: 9999;
  background: #fff !important;
  overflow: hidden;
}

/* Custom Dialog Styling */
.custom-dialog {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
}

:deep(.custom-dialog .v-overlay__content) {
  margin: 5vh auto;
  max-height: 90vh;
  display: flex;
  flex-direction: column;
  animation: slideDown 0.4s cubic-bezier(0.25, 0.8, 0.25, 1);
}

@keyframes slideDown {
  from {
    opacity: 0;
    transform: translateY(-50px) scale(0.95);
  }
  to {
    opacity: 1;
    transform: translateY(0) scale(1);
  }
}

/* Dialog Card Styling */
.dialog-card {
  display: flex;
  flex-direction: column;
  height: 100%;
  max-height: 90vh;
  border-radius: 16px !important;
  box-shadow: 0 24px 48px rgba(0, 0, 0, 0.2) !important;
  background: linear-gradient(145deg, #ffffff 0%, #f8f9ff 100%);
  overflow: hidden;
}

/* Dialog Header */
.dialog-header {
  background: linear-gradient(90deg, #2b3086 0%, #4a6cf7 100%);
  color: white;
  padding: 16px 24px;
  position: sticky;
  top: 0;
  z-index: 10;
  border-radius: 16px 16px 0 0 !important;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.dialog-title {
  font-size: 18px;
  font-weight: 600;
  letter-spacing: -0.3px;
}

.close-btn {
  color: white !important;
  transition: all 0.2s ease;
}

.close-btn:hover {
  background: rgba(255, 255, 255, 0.1) !important;
  transform: scale(1.1);
}

/* Dialog Content */
.dialog-content {
  flex: 1;
  overflow: hidden;
  display: flex;
  flex-direction: column;
}

.scrollable-content {
  flex: 1;
  overflow-y: auto;
  overflow-x: hidden;
  padding: 24px;
  scroll-behavior: smooth;
}

/* Dialog Responsive Design */
@media (max-width: 959px) {
  :deep(.custom-dialog .v-overlay__content) {
    margin: 2vh 16px;
    max-height: 96vh;
    width: calc(100% - 32px);
  }

  .dialog-header {
    padding: 12px 16px;
  }

  .dialog-title {
    font-size: 16px;
  }

  .scrollable-content {
    padding: 16px;
  }
}

@media (max-width: 599px) {
  :deep(.custom-dialog .v-overlay__content) {
    margin: 1vh 8px;
    max-height: 98vh;
    width: calc(100% - 16px);
  }

  .dialog-card {
    border-radius: 12px !important;
  }

  .dialog-header {
    border-radius: 12px 12px 0 0 !important;
    padding: 12px 16px;
  }

  .scrollable-content {
    padding: 16px 12px;
  }
}

/* Enhanced slide transition */
:deep(.v-dialog-transition-enter-active),
:deep(.v-dialog-transition-leave-active) {
  transition: all 0.4s cubic-bezier(0.25, 0.8, 0.25, 1) !important;
}

:deep(.v-dialog-transition-enter-from) {
  opacity: 0 !important;
  transform: translateY(-50px) scale(0.9) !important;
}

:deep(.v-dialog-transition-leave-to) {
  opacity: 0 !important;
  transform: translateY(-30px) scale(0.95) !important;
}

/* Backdrop blur effect */
:deep(.v-overlay__scrim) {
  backdrop-filter: blur(8px);
  background: rgba(0, 0, 0, 0.6) !important;
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
@media (max-width: 767.98px) {
  .create-activity-drawer:not(.v-navigation-drawer--active) {
    transform: translateX(100%) !important;
    visibility: hidden !important;
  }
}
</style>
