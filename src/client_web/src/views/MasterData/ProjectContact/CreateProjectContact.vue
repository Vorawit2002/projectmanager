<template>
  <v-row class="d-flex align-center justify-center">
    <v-col
      cols="12"
      md="12"
    >
      <v-card class="page-container scroll-content dialog-scrollbar">
        <v-card-text>
          <span class="text-sub-title">แก้ไขผู้ติดต่อโครงการ</span>
          <v-form
            ref="form"
            @submit.prevent="CreateProjectContact"
            class="text-black"
          >
            <v-row class="px-1 px-sm-3 px-md-5 mt-3">
              <!-- โครงการ -->
              <v-col
                cols="12"
                md="12"
              >
                <label class="form-label">โครงการ <span class="text-error">*</span></label>
                <v-autocomplete
                  v-model="createCommand.projectId"
                  :items="ProjectList"
                  item-title="projectName"
                  item-value="id"
                  placeholder="เลือกหรือเพิ่มชื่อโครงการ"
                  variant="outlined"
                  density="comfortable"
                  color="primary-darken-0"
                  :rules="selectProjectRules"
                  clearable
                >
                  <template v-slot:item="{ props, item }">
                    <v-list-item
                      v-bind="props"
                      :title="item.raw.projectName"
                    />
                  </template>
                </v-autocomplete>
              </v-col>
              <!-- ผู้ติดต่อหน่วยงาน -->
              <v-col
                cols="12"
                md="12"
              >
                <label class="form-label">ผู้ติดต่อหน่วยงาน <span class="text-error">*</span></label>
                <v-autocomplete
                  v-model="createCommand.organizationContactId"
                  :items="OrganizationContactList"
                  item-title="fullName"
                  item-value="id"
                  placeholder="เลือกหรือเพิ่มผู้ติดต่อหน่วยงาน"
                  variant="outlined"
                  density="comfortable"
                  color="primary-darken-0"
                  :rules="selectContactRules"
                  clearable
                >
                  <template v-slot:item="{ props, item }">
                    <v-list-item
                      v-bind="props"
                      :title="(item.raw as any)?.fullName"
                    />
                  </template>
                </v-autocomplete>
              </v-col>
              <!-- ปุ่มควบคุม -->
              <v-col
                cols="12"
                class="pt-6"
              >
                <div class="button-container">
                  <v-btn
                    class="mobile-btn submit-btn"
                    rounded="lg"
                    color="success-darken-2"
                    type="submit"
                    :loading="loading"
                    :disabled="loading"
                    block
                  >
                    <v-icon class="mr-2">ri-save-3-fill</v-icon>
                    บันทึก
                  </v-btn>
                  <v-btn
                    class="mobile-btn cancel-btn"
                    rounded="lg"
                    color="error"
                    @click="closeDialog"
                    block
                  ><v-icon
                      icon="ri-close-line"
                      class="mr-2"
                    ></v-icon>
                    ยกเลิก
                  </v-btn>
                </div>
              </v-col>
            </v-row>
          </v-form>
        </v-card-text>
      </v-card>
    </v-col>
  </v-row>
</template>
<script lang="ts">
import { Client, CreateProjectContactCommand, GetOrganizationContactByOrganizationIdQuery } from '@/client'
import { BACKEND_API_URL } from '@/constants'
import { useSweetAlertStore } from '@/stores'
import { selectContactRules, selectProjectRules } from '@/utils/RuleServices'
import { defineComponent } from 'vue'
const client = new Client(BACKEND_API_URL)

export default defineComponent({
  name: 'CreateProjectContact',
  props: {
    CloseDialogCreate: {
      type: Function,
      required: true,
    },
  },
  data() {
    return {
      createCommand: new CreateProjectContactCommand(),
      requestOrganizationContact: new GetOrganizationContactByOrganizationIdQuery(),
      loading: false,
      sweetAlertStore: useSweetAlertStore(),
      ProjectList: [] as any[],
      OrganizationContactList: [] as any,
      OrganizationList: [] as any,
      selectContactRules,
      selectProjectRules,
    }
  },
  mounted() {
    this.initialize() // เรียกใช้ฟังก์ชัน initialize เมื่อคอมโพเนนต์ถูกเมานต์
  },
  watch: {
    'createCommand.projectId'(newProjectId) {
      const selectedProject = this.ProjectList.find(p => p.id === newProjectId)
      if (selectedProject && selectedProject.organizationId) {
        this.requestOrganizationContact.organizationId = selectedProject.organizationId
        this.loadOrganizationContactList()
      } else {
        this.OrganizationContactList = [] // reset ถ้าไม่พบ
      }
    },
  },
  methods: {
    closeDialog(reload: boolean = false, newId?: string) {
      this.CloseDialogCreate(false, reload, newId) // ปิด dialog และส่ง ID ไปด้วย
    },
    async CreateProjectContact() {
      const form = this.$refs.form as any
      const { valid } = await form.validate()
      if (valid) {
        try {
          this.loading = true
          const response = await client.createProjectContact(this.createCommand)
          console.log('Create response:', response) // Debug log
          if (response) {
            setTimeout(() => {
              this.loading = false
              this.sweetAlertStore.success('เพิ่มข้อมูลผู้ติดต่อโครงการสำเร็จ!')
              // ส่งข้อมูลที่ใช้สร้างไปด้วย เพื่อหา ID ที่ตรงกัน
              const searchData = {
                projectId: this.createCommand.projectId,
                organizationContactId: this.createCommand.organizationContactId,
              }
              console.log('Sending search data:', searchData) // Debug log
              this.closeDialog(true, JSON.stringify(searchData))
            }, 600)
          }
        } catch (error) {
          console.error(error)
          this.closeDialog(true)
          setTimeout(() => {
            this.loading = false
            this.sweetAlertStore.error('เกิดข้อผิดพลาดในการเพิ่มข้อมูล ล้มเหลว!')
          }, 600)
        }
      }
    },
    async initialize() {
      //ดึงข้อมูลโครงการ
      try {
        this.ProjectList = await client.getProjectQuery()
        console.log('ProjectList : ', this.ProjectList)
      } catch (error) {
        console.error(error)
      }

      //ดึงข้อมูลหน่วยงาน
      try {
        this.OrganizationList = await client.getOrganizationQuery()
        console.log('OrganizationList : ', this.OrganizationList)
      } catch (error) {
        console.error(error)
      }
    },
    async loadOrganizationContactList() {
      try {
        this.OrganizationContactList = await client.getOrganizationContactQueryByOrganizationId(
          this.requestOrganizationContact,
        )
        this.OrganizationContactList.forEach((data: any) => {
          data.fullName = `${data.firstName || ''} ${data.lastName || ''}`.trim()
        })
      } catch (error) {
        console.error('Error loading organization contacts:', error)
      }
    },
  },
})
</script>

<style scoped>
/* Enhanced Autocomplete Dropdown Styling */
.long-text-autocomplete .v-overlay__content {
  max-width: 600px !important;
  border-radius: 16px !important;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.15) !important;
  border: 2px solid rgba(102, 126, 234, 0.1) !important;
}

.long-text-autocomplete .v-list-item__title {
  white-space: normal !important;
  overflow: visible !important;
  line-height: 1.4 !important;
  word-break: break-word !important;
  font-weight: 400 !important;
  color: #2c3e50 !important;
}

.long-text-autocomplete .v-list-item {
  min-height: auto !important;
  padding: 12px 20px !important;
  border-radius: 8px !important;
  margin: 4px 8px !important;
}

.long-text-autocomplete .v-list-item--active {
  background: rgba(102, 126, 234, 0.08) !important;
}

.long-text-autocomplete .v-field__input {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.page-container {
  width: 100%;
  min-height: 100vh;
  overflow-y: auto;
  overflow-x: hidden;
  padding: 16px 0 32px 0;
  scroll-behavior: smooth;
  position: relative;
}

/* Enhanced Typography */
.text-sub-title {
  font-size: clamp(20px, 4vw, 28px);
  font-weight: 700;
  color: #2b3086;
  background: linear-gradient(135deg, #2b3086 0%, #667eea 100%);
  background-clip: text;
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  text-align: center;
  display: block;
  margin: 0 0 24px 0;
  padding: 0;
  line-height: 1.5;
  letter-spacing: -0.02em;
}

.text-black {
  color: #000 !important;
}

.form-label {
  font-size: 15px;
  font-weight: 500;
  color: #333;
  margin-bottom: 8px;
  display: block;
  letter-spacing: 0.025em;
}

/* Enhanced Button Container */
.button-container {
  display: flex;
  gap: 20px;
  justify-content: center;
  flex-wrap: wrap;
  max-width: 500px;
  margin: 0 auto;
}

.mobile-btn {
  min-width: 200px;
  min-height: 52px;
  font-size: 16px;
  font-weight: 600;
  border: none !important;
  border-radius: 16px;
  box-shadow: 
    0 4px 16px rgba(0, 0, 0, 0.1),
    0 1px 4px rgba(0, 0, 0, 0.05);
  text-transform: none;
  letter-spacing: 0.025em;
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

.mobile-btn .v-icon {
  font-size: 20px;
}


/* Loading and Disabled States */
.mobile-btn.v-btn--loading {
  pointer-events: none;
  opacity: 0.7;
}

.mobile-btn:disabled {
  opacity: 0.5 !important;
  pointer-events: none !important;
  transform: none !important;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1) !important;
}

/* Enhanced Scrollbar */
.card-Dialog::-webkit-scrollbar {
  width: 8px;
}

.card-Dialog::-webkit-scrollbar-track {
  background: rgba(0, 0, 0, 0.05);
  border-radius: 4px;
}

.card-Dialog::-webkit-scrollbar-thumb {
  background: linear-gradient(135deg, #667eea, #764ba2);
  border-radius: 4px;
}

/* Firefox scrollbar */
.card-Dialog {
  scrollbar-width: thin;
  scrollbar-color: #667eea rgba(0, 0, 0, 0.05);
}

/* Enhanced Form Row */
.v-row {
  min-height: 200px;
  align-items: flex-start;
}

@media (max-width: 600px) {
  .text-sub-title {
    font-size: 35px;
    margin-bottom: 20px;
  }
  
}
</style>
