<template>
  <v-card class="text-form page-container">
    <!-- Enhanced title section -->

    <v-card-text class="scroll-content page-container dialog-scrollbar">
      <v-form
        ref="form"
        @submit.prevent="createActivityPlan"
        class="text-black"
      >
        <!-- Department info section -->
        <v-row>
          <v-col
            cols="12"
            class="d-flex justify-center px-3 px-md-0"
          >
            <span class="text-sub-title text-center">สร้างนัดหมาย</span>
          </v-col>
          <v-col
            cols="12"
            class="text-right pb-2 pb-md-3"
          >
            <p class="mr-2 mr-md-8 text-subtitle-1 text-md-h6 text-lg-h5 text-primary mb-1">
              แผนก : {{ User.departments?.name }}
            </p>
            <p class="mr-2 mr-md-8 text-subtitle-1 text-md-h6 text-lg-h5 text-primary">
              โดย : {{ User.titleName + ' ' + User.firstName + ' ' + User.lastName }}
            </p>
          </v-col>
        </v-row>

        <v-row class="px-1 px-sm-2 px-md-4">
          <!-- Enhanced Date Section -->
          <v-col
            cols="12"
            class="pb-1 pb-md-2"
          >
            <v-row class="d-flex">
              <v-col
                cols="12"
                md="12"
              >
                <label class="mb-2">วันที่เริ่มต้น <span class="text-error">*</span></label>

                <TextFieldDatepicker
                  placeholder="กรุณาระบุวันที่เริ่มต้นนัดหมาย"
                  :rules="appointmentStartDateRules"
                  :AllDay="createCommand.allDay"
                  @selectedDateTime="changeTimestartDate"
                />
              </v-col>
              <v-col
                cols="12"
                md="12"
              >
                <label class="mb-2">วันที่สิ้นสุด <span class="text-error">*</span></label>
                <TextFieldDatepicker
                  placeholder="กรุณาระบุวันที่สิ้นสุดนัดหมาย"
                  :rules="appointmentEndDateRules"
                  :AllDay="createCommand.allDay"
                  @selectedDateTime="changeTimeendDate"
                />
              </v-col>

              <v-col
                cols="12"
                md="4"
                class="mt-1 mt-sm-2 mt-md-4 d-flex align-center"
              >
                <v-checkbox
                  label="ทั้งวัน"
                  hide-details
                  v-model="createCommand.allDay"
                  class="checkbox-field"
                />
              </v-col>
            </v-row>
          </v-col>

          <!-- Enhanced form fields grid -->
          <v-col
            cols="12"
            sm="12"
            lg="12"
          >
            <label class="mb-2">รหัสโครงการ</label>
            <v-autocomplete
              placeholder="กรุณาเลือกรหัสโครงการ"
              clearable
              v-model="createCommand.projectId"
              :items="ProjectList"
              item-title="projectCodeAndName"
              item-value="id"
            />
          </v-col>

          <v-col
            cols="12"
            sm="12"
            lg="12"
          >
            <label class="mb-2">หน่วยงาน / ลูกค้า <span class="text-error">*</span></label>
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
            >
              <template v-slot:append-inner>
                <v-tooltip
                  text="เพิ่มหน่วยงานหรือลูกค้า (เพิ่มเติม)"
                  location="top"
                  contentClass="bg-secondary text-white"
                >
                  <template #activator="{ props }">
                    <v-btn
                      v-bind="props"
                      variant="text"
                      class="cursor-pointer add-btn"
                      icon="ri-add-circle-line"
                      size="small"
                      @click="addNewOrganization"
                    ></v-btn>
                  </template>
                </v-tooltip>
              </template>
            </v-autocomplete>
          </v-col>

          <v-col
            cols="12"
            sm="12"
            lg="12"
          >
            <label class="mb-2">ลูกค้าที่นัดพบ <span class="text-error">*</span></label>
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
            >
              <template v-slot:append-inner>
                <v-tooltip
                  text="เพิ่มลูกค้าที่นัดพบ (เพิ่มเติม)"
                  location="top"
                  content-class="bg-secondary text-white"
                >
                  <template #activator="{ props }">
                    <v-btn
                      v-bind="props"
                      variant="text"
                      class="cursor-pointer add-btn"
                      icon="ri-add-circle-line"
                      size="small"
                      @click="addNewOrganizationContact"
                    ></v-btn>
                  </template>
                </v-tooltip>
              </template>
            </v-autocomplete>
          </v-col>

          <v-col
            cols="12"
            sm="12"
            lg="12"
          >
            <label class="mb-2">สถานที่ <span class="text-error">*</span></label>
            <v-text-field
              placeholder="อาคาร 2, ชั้น 12, ห้องประชุม A"
              :rules="appointmentLocationRules"
              v-model="createCommand.location"
            />
          </v-col>

          <v-col
            cols="12"
            sm="12"
            lg="12"
          >
            <label class="mb-2">วัตถุประสงค์ <span class="text-error">*</span></label>
            <v-autocomplete
              placeholder="ระบุวัตถุประสงค์"
              :items="objectiveList"
              :rules="appointmentPurposeRules"
              v-model="createCommand.objective"
              clearable
            />
          </v-col>

          <v-col
            cols="12"
            sm="12"
            lg="12"
            v-if="createCommand.objective === 'อื่น ๆ'"
          >
            <label class="mb-2">วัตถุประสงค์อื่น ๆ</label>
            <v-text-field
              placeholder="ระบุวัตถุประสงค์อื่น ๆ"
              :rules="otherPurposeRules"
              v-model="createCommand.objectiveDetail"
            />
          </v-col>

          <!-- Enhanced detail section -->
          <v-col cols="12">
            <label class="mb-2">รายละเอียดเพิ่มเติม</label>
            <v-textarea
              v-model="createCommand.detail"
              placeholder="ระบุรายละเอียดการนัดพบ"
              rows="3"
            />
          </v-col>

          <!-- Enhanced Action Buttons -->
          <v-col
            cols="12"
            class="pt-4 pt-md-6 action-row"
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
                    :loading="loading"
                    :disabled="loading"
                    :size="$vuetify.display.mobile ? 'default' : 'large'"
                    block
                  >
                    <v-icon class="mr-2">ri-close-line</v-icon>
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
                :loading="loading"
                :disabled="loading"
                :size="$vuetify.display.mobile ? 'default' : 'large'"
              >
                <v-icon class="mr-2">ri-close-line</v-icon>
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

  <!-- Enhanced responsive dialogs -->
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
  >
    <CreateOrganizationContact
      v-if="createCommand.organizationId"
      :CloseDialogCreate="CloseDialogCreateContact"
      :organization-id="createCommand.organizationId"
    />
  </v-navigation-drawer>

  <v-navigation-drawer
    v-model="DialogCreateOrganization"
    :width="$vuetify.display.xs ? '100vw' : '550'"
    class="z-indexDialog create-activity-drawer"
    close-on-back
    temporary
    transition="dialog-righ-transition"
    location="right"
    scrollable
    :permanent="false"
  >
    <CreateOrganizationDetailView :CloseDialogCreate="CloseDialogCreate" />
  </v-navigation-drawer>
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
import CreateOrganizationDetailView from '@/views/MasterData/Organizations/CreateOrganizationDetailView.vue'
import DemoFormLayoutVerticalFormWithIcons from '@/views/pages/form-layouts/DemoFormLayoutVerticalFormWithIcons.vue'
import { defineComponent } from 'vue'

const client = new Client(BACKEND_API_URL)
export default defineComponent({
  name: 'CreateCustomerAppointmentPlan',
  emits: ['close'],
  components: {
    DemoFormLayoutVerticalFormWithIcons,
    TextFieldDatepicker,
    TextFieldTimepicker,
    CreateOrganizationContact,
    CreateOrganizationDetailView,
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
      DialogCreateContact: false,
      DialogCreateOrganization: false,
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

    document.addEventListener('keydown', this.handleEscKey)
  },
  beforeUnmount() {
    document.removeEventListener('keydown', this.handleEscKey)
  },
  computed: {
    // showAddButton() {
    //   // แสดงปุ่มเฉพาะเมื่อ: มีการพิมพ์ค้นหา และไม่พบรายการที่ตรงกัน
    //   return this.searchText && this.searchText.trim() !== '' && !this.hasMatchingItems
    // },
    hasMatchingItems() {
      if (!this.searchText || this.searchText.trim() === '') return true
      return this.OrganizationList.some((item: any) => item.name.toLowerCase().includes(this.searchText.toLowerCase()))
    },
    showAddButtonContact() {
      const noContact =
        this.createCommand.organizationId &&
        (!this.OrganizationContactList || this.OrganizationContactList.length === 0)

      const noSearchMatch =
        this.searchTextContact && this.searchTextContact.trim() !== '' && !this.hasMatchingItemsContact

      return noContact || noSearchMatch
    },
    hasMatchingItemsContact() {
      if (!this.searchTextContact || this.searchTextContact.trim() === '') return true
      return this.OrganizationContactList.some((item: any) =>
        item.name.toLowerCase().includes(this.searchTextContact.toLowerCase()),
      )
    },
  },
  methods: {
    handleEscKey(e: KeyboardEvent) {
      if (e.key === 'Escape' || e.keyCode === 27) {
        // ถ้า Drawer ใดกำลังเปิด ให้ปิดอย่างเดียว
        if (this.DialogCreateContact) {
          this.DialogCreateContact = false
          e.stopPropagation()
          return
        }
        if (this.DialogCreateOrganization) {
          this.DialogCreateOrganization = false
          e.stopPropagation()
          return
        }
        // ถ้าไม่มี Drawer ไหนเปิด ก็ปิดหน้าหลักได้
        this.$emit('close')
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
    async CloseDialogCreate(value: boolean, reload: boolean, newOrgId: string | null = null) {
      this.DialogCreateOrganization = value

      if (reload === true) {
        await this.initialize()

        if (newOrgId) {
          // ตั้งค่าให้ v-model
          this.createCommand.organizationId = newOrgId
        }
      }
    },
    async addNewOrganizationContact() {
      this.DialogCreateContact = true
    },
    async CloseDialogCreateContact(value: boolean, reload: boolean) {
      this.DialogCreateContact = value
      if (reload === true) {
        this.getOrganizationContact()
      }
    },
    async initialize() {
      try {
        this.OrganizationList = await client.getOrganizationQuery()
        if (this.auth.userId) {
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
        console.error(error)
      }
    },
    async getOrganizationContact() {
      try {
        this.requestOrganizationContact.organizationId = this.createCommand.organizationId
        this.OrganizationContactList = await client.getOrganizationContactQueryByOrganizationId(
          this.requestOrganizationContact,
        )
        this.OrganizationContactList.forEach((OC: any) => {
          OC.name = `${OC.firstName || ''}   ${OC.lastName || ''}`.trim()
        })
      } catch (error) {
        console.error(error)
      }
    },
    CancelCreate() {
      this.$emit('close')
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
    async createActivityPlan() {
      const form = this.$refs.form as any
      const { valid } = await form.validate()
      if (valid) {
        this.loading = true
        try {
          this.createCommand.eventType = '001'
          const response = await client.createActivityPlan(this.createCommand)

          if (this.customers.length > 0) {
            try {
              this.createContact.activityPlanId = response
              for (let i = 0; i < this.customers.length; i++) {
                this.createContact.organizationContactId = this.customers[i]
                await client.createActivityPlanContact(this.createContact)
              }
            } catch (error) {
              console.error('เกิดข้อผิดพลาดในการสร้าง Contact:', error)
            }
          }

          if (response) {
            this.sweetAlertStore.successDeleted('สร้างนัดหมายสำเร็จ!')

            localStorage.setItem('selectedAppointmentId', String(response))
            console.log('Saved to localStorage - selectedAppointmentId:', response)

            const reloadEvent = new CustomEvent('reloadAppointmentPlan', {
              detail: {
                newAppointmentId: String(response),
                eventTypeToSelect: this.createCommand.eventType,
              },
            })
            window.dispatchEvent(reloadEvent)
            console.log('🔥 Dispatched reloadAppointmentPlan event for:', response)

            await this.initialize()

            this.$emit('close')
          }
        } catch (error) {
          console.error('เกิดข้อผิดพลาดในการสร้างนัดหมาย:', error)
        } finally {
          this.loading = false
        }
      }
    },
  },
})
</script>

<style scoped>
.page-container {
  width: 100%;
  min-height: 100vh;
  overflow-y: auto;
  overflow-x: hidden;
  padding: 16px 0 32px 0;
  scroll-behavior: smooth;
  position: relative;
}

/* Enhanced container styles */
.scroll-wrapper {
  max-height: 90vh;
  overflow: hidden;
  padding: 0;
}

.card-form::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 4px;
  /* background: linear-gradient(90deg, #667eea 0%, #764ba2 50%, #667eea 100%); */
  border-radius: 24px 24px 0 0;
  z-index: 1;
}

/* Scrollable area with fixed max-height, no flex grow */
.scroll-content {
  overflow-y: auto;
  overflow-x: hidden;
  max-height: 70vh;
  scrollbar-width: thin;
  scrollbar-color: #667eea #f8f9fa;
  -webkit-overflow-scrolling: touch;
  overscroll-behavior: contain;
}

/* Enhanced scrollbar */
.scroll-content::-webkit-scrollbar {
  width: 8px;
}

.scroll-content::-webkit-scrollbar-track {
  background: rgba(0, 0, 0, 0.05);
  border-radius: 4px;
}

.scroll-content::-webkit-scrollbar-thumb {
  background: linear-gradient(135deg, #667eea, #764ba2);
  border-radius: 4px;
}

/* Enhanced typography */
.text-sub-title {
  font-size: clamp(20px, 4vw, 32px);
  font-weight: 700;
  color: #2b3086;
  background: linear-gradient(135deg, #2b3086 0%, #667eea 100%);
  background-clip: text;
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  text-align: center;
  margin: 0;
  padding: 0;
  line-height: 1.5;
  letter-spacing: -0.02em;
}

.add-btn {
  opacity: 0.7;
}

.action-btn {
  min-width: 180px;
  min-height: 52px;
  font-size: 16px;
  font-weight: 600;
  border: none !important;
  border-radius: 16px;
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.1), 0 1px 4px rgba(0, 0, 0, 0.05);
  text-transform: none;
  letter-spacing: 0.025em;
  position: relative;
  overflow: hidden;
}

.action-btn::before {
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.2), transparent);
}

.btn-icon {
  font-size: 20px;
  margin-right: 8px;
}

.btn-text {
  font-weight: 600;
}

/* ===== RESPONSIVE BREAKPOINTS ===== */

/* Mobile Portrait (320px - 599px) */
@media (max-width: 599px) {
  .scroll-wrapper {
    margin: 0px;
  }

  .card-form {
    margin: 0px;
    border-radius: 20px;
    max-height: 100vh;
  }

  .scroll-content {
    max-height: calc(100vh - 120px);
    padding: 16px !important;
  }

  .text-sub-title {
    font-size: 30px;
  }

  .pr-1 {
    padding-right: 6px !important;
  }
  .pl-1 {
    padding-left: 6px !important;
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

/* Mobile Landscape (480px - 767px) */
@media (min-width: 480px) and (max-width: 767px) and (orientation: landscape) {
  .scroll-wrapper {
    max-height: 98vh;
  }

  .card-form {
    margin: 0px;
    max-height: 98vh;
  }

  .scroll-content {
    max-height: calc(98vh - 120px);
  }

  /* Allow horizontal button layout in landscape */
  .button-container {
    flex-direction: row !important;
    justify-content: center !important;
    gap: 12px !important;
  }

  .action-btn {
    width: auto !important;
    min-width: 140px !important;
    flex: 1;
    max-width: 200px;
  }
}

/* Tablet Portrait (600px - 959px) */
@media (min-width: 600px) and (max-width: 959px) {
  .scroll-wrapper {
    max-height: 92vh;
    padding: 0;
  }

  .card-form {
    margin: 16px;
    border-radius: 24px;
  }

  .scroll-content {
    max-height: calc(92vh - 130px);
    padding: 24px !important;
  }

  /* Two columns for some fields */
  .v-col[sm='6'] {
    flex-basis: 50% !important;
    max-width: 50% !important;
  }

  .v-col[md='5'] {
    flex-basis: 48% !important;
    max-width: 48% !important;
  }

  .button-container {
    gap: 16px;
    max-width: 400px;
  }

  .action-btn {
    min-width: 160px;
    min-height: 48px;
    font-size: 15px;
  }

  .btn-icon {
    font-size: 19px;
  }
}

/* Tablet Landscape / Small Desktop (960px - 1263px) */
@media (min-width: 960px) and (max-width: 1263px) {
  .scroll-wrapper {
    max-height: 90vh;
    padding: 0;
  }

  .card-form {
    margin: 20px auto;
    max-width: 800px;
    border-radius: 24px;
  }

  .scroll-content {
    max-height: calc(90vh - 140px);
    padding: 32px !important;
  }

  .text-sub-title {
    font-size: 28px;
    padding: 24px 20px 16px;
  }

  .button-container {
    gap: 18px;
    max-width: 450px;
  }

  .action-btn {
    min-width: 180px;
    min-height: 50px;
    font-size: 16px;
  }

  .btn-icon {
    font-size: 20px;
  }
}

/* Large Desktop (1264px+) */
@media (min-width: 1264px) {
  .scroll-wrapper {
    max-height: 90vh;
    padding: 0;
  }

  .card-form {
    margin: 24px auto;
    max-width: 900px;
    border-radius: 28px;
  }

  .scroll-content {
    max-height: calc(90vh - 140px);
    padding: 40px !important;
  }

  .text-sub-title {
    font-size: 32px;
    padding: 28px 24px 20px;
  }

  .button-container {
    gap: 20px;
    max-width: 500px;
  }

  .action-btn {
    min-width: 200px;
    min-height: 52px;
    font-size: 16px;
  }

  .btn-icon {
    font-size: 20px;
    margin-right: 8px;
  }
}

/* Ultra-wide screens (1920px+) */
@media (min-width: 1920px) {
  .card-form {
    max-width: 1000px;
  }

  .text-sub-title {
    font-size: 36px;
  }
}

/* Extra small devices adjustments */
@media (max-width: 375px) {
  .text-sub-title {
    font-size: 18px;
    padding: 12px 6px 6px;
  }

  .card-form {
    margin: 4px;
    border-radius: 16px;
  }

  .scroll-content {
    padding: 12px !important;
  }

  .action-btn {
    min-height: 52px;
    font-size: 15px;
  }

  .btn-icon {
    font-size: 16px;
    margin-right: 4px;
  }
}

/* ===== ACCESSIBILITY & PERFORMANCE ===== */

/* Enhanced focus states */
.action-btn:focus-visible {
  outline: 3px solid rgba(102, 126, 234, 0.5);
  outline-offset: 2px;
}

/* Loading state enhancements */
.action-btn.v-btn--loading {
  pointer-events: none;
  opacity: 0.7;
}

.action-btn.v-btn--loading .v-btn__content {
  opacity: 0.6;
}

.action-btn[loading] {
  pointer-events: none;
  opacity: 0.7;
}

/* Disabled state */
.action-btn:disabled {
  opacity: 0.5 !important;
  pointer-events: none !important;
  transform: none !important;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1) !important;
}

.action-btn[disabled] {
  pointer-events: none;
  opacity: 0.5;
  transform: none !important;
}

/* High contrast mode support */
@media (prefers-contrast: high) {
  .card-form {
    border: 2px solid #2b3086;
    background: white;
  }

  .action-btn {
    border: 2px solid currentColor !important;
  }

  .text-sub-title {
    background: none !important;
    -webkit-text-fill-color: initial !important;
    color: #000 !important;
    text-shadow: none;
  }
}

/* Print styles */
@media print {
  .scroll-wrapper {
    max-height: none;
    overflow: visible;
  }

  .card-form {
    box-shadow: none;
    border: 1px solid #ccc;
    max-height: none;
    overflow: visible;
    margin: 0;
    padding: 20px;
  }

  .scroll-content {
    overflow: visible;
    max-height: none;
  }

  .button-container {
    display: none !important;
  }

  .text-sub-title {
    color: #000 !important;
    background: none !important;
    -webkit-text-fill-color: initial !important;
  }
}

/* Touch optimization */
@media (pointer: coarse) {
  .action-btn {
    min-height: 56px; /* Larger touch targets */
  }
}

/* Loading animations */
@keyframes pulse {
  0%,
  100% {
    opacity: 1;
  }
  50% {
    opacity: 0.7;
  }
}

.action-btn[loading] {
  animation: pulse 1.5s infinite;
}

@media (max-width: 767.98px) {
  .create-activity-drawer:not(.v-navigation-drawer--active) {
    transform: translateX(100%) !important;
    visibility: hidden !important;
  }
}
</style>

