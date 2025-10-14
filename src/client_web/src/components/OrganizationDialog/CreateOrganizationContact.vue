<template>
  <v-card class="text-form page-container">
    <v-card-text class="scroll-content page-container dialog-scrollbar">
      <v-form
        ref="form"
        @submit.prevent="CreateContact"
        class="text-black"
      >
        <v-row>
          <v-col
            cols="12"
            class="d-flex justify-center px-3 px-md-0"
          >
            <span class="text-sub-title">เพิ่มข้อมูลผู้ติดต่อ ของ {{ organizationName }}</span>
          </v-col>
        </v-row>

        <v-row class="px-1 px-sm-2 px-md-4">
          <v-col
            cols="12"
            md="3"
          >
            <label class="mb-2">คำนำหน้า <span class="text-error">*</span></label>
            <v-text-field
              v-model="createCommand.titleName"
              placeholder="ระบุคำนำหน้าชื่อ"
              variant="outlined"
              density="comfortable"
              color="primary-darken-0"
              dense
              :rules="[
                (v: string) => !!v || 'กรุณาระบุคำนำหน้า',
              ]"
            ></v-text-field>
          </v-col>

          <v-col
            cols="12"
            md="5"
          >
            <label class="mb-2">ชื่อ <span class="text-error">*</span></label>
            <v-text-field
              v-model="createCommand.firstName"
              placeholder="ระบุชื่อ"
              variant="outlined"
              density="comfortable"
              color="primary-darken-0"
              dense
              :rules="[
                (v: string) => !!v || 'กรุณาระบุชื่อ',
              ]"
            ></v-text-field>
          </v-col>

          <v-col
            cols="12"
            md="4"
          >
            <label class="mb-2">นามสกุล <span class="text-error">*</span></label>
            <v-text-field
              v-model="createCommand.lastName"
              placeholder="ระบุนามสกุล"
              variant="outlined"
              density="comfortable"
              color="primary-darken-0"
              dense
              :rules="[
                (v: string) => !!v || 'กรุณาระบุนามสกุล',
              ]"
            ></v-text-field>
          </v-col>

          <v-col
            cols="12"
            md="6"
          >
            <label class="mb-2">ตำแหน่งงาน <span class="text-error">*</span></label>
            <v-text-field
              v-model="createCommand.position"
              placeholder="ระบุตำแหน่งงาน"
              variant="outlined"
              density="comfortable"
              color="primary-darken-0"
              dense
              :rules="positionRules"
            ></v-text-field>
          </v-col>

          <v-col
            cols="12"
            md="6"
          >
            <label class="mb-2">อีเมล <span class="text-error">*</span></label>
            <v-text-field
              v-model="createCommand.email"
              placeholder="ระบุอีเมล"
              variant="outlined"
              density="comfortable"
              color="primary-darken-0"
              dense
              :rules="emailRules"
            ></v-text-field>
          </v-col>

          <v-col
            cols="12"
            md="6"
          >
            <label class="mb-2">เบอร์โทร <span class="text-error">*</span></label>
            <v-text-field
              v-model="createCommand.phone"
              placeholder="ระบุเบอร์โทร"
              variant="outlined"
              density="comfortable"
              color="primary-darken-0"
              dense
              :rules="phoneRules"
            ></v-text-field>
          </v-col>

          <v-col
            cols="12"
            md="6"
          >
            <label class="mb-2">แฟกซ์ </label>
            <v-text-field
              v-model="createCommand.fax"
              placeholder="ระบุแฟกซ์"
              variant="outlined"
              density="comfortable"
              color="primary-darken-0"
              dense
            ></v-text-field>
          </v-col>

          <v-col
            cols="12"
            md="6"
          >
            <label class="mb-2">LineID <span class="text-error"></span></label>
            <v-text-field
              v-model="createCommand.lineId"
              placeholder="ระบุLineID"
              variant="outlined"
              density="comfortable"
              color="primary-darken-0"
              dense
            ></v-text-field>
          </v-col>
          <v-col
            cols="12"
            md="6"
          >
            <label class="mb-2">รูปภาพรูปติดต่อ</label>
            <v-text-field
              v-model="files.name"
              placeholder="โปรดเพิ่มไฟล์รูปภาพ"
              variant="outlined"
              density="comfortable"
              color="primary-darken-0"
              dense
              prepend-inner-icon="ri-attachment-2"
              @click="triggerFileInput"
              @click:clear="onClearFile"
              clearable
            ></v-text-field>
            <v-file-input
              ref="fileInput"
              hide-input
              placeholder="โปรดเพิ่มไฟล์รูปภาพ"
              variant="outlined"
              density="comfortable"
              color="primary-darken-0"
              dense
              prepend-icon=""
              prepend-inner-icon="ri-attachment-2"
              accept="image/*"
              @change="UploadFile"
            ></v-file-input>
          </v-col>

          <v-col
            cols="12"
            md="12"
            class="d-flex justify-center align-center"
            align="center"
            v-if="files && files.name"
          >
            <v-img
              max-width="35%"
              class="mt-5 imgCard"
              :src="createCommand.base64 ? 'data:image/jpeg;base64,' + createCommand.base64 : ''"
            ></v-img>
          </v-col>

          <!-- Enhanced Action Buttons -->
          <v-col
            cols="12"
            class="pt-4 pt-md-6"
          >
            <div v-if="$vuetify.display.mobile">
              <v-row
                class="d-flex action-row"
                no-gutters
              >
                <v-col
                  cols="6"
                  class="pr-1"
                >
                  <v-btn
                    class="action-btn cancel-btn"
                    rounded="lg"
                    color="error"
                    @click="closeDialog"
                    :loading="loading"
                    :disabled="loading"
                    block
                  >
                    <v-icon
                      class="btn-icon"
                      icon="ri-close-line"
                    ></v-icon>
                    <span class="btn-text">ยกเลิก</span>
                  </v-btn>
                </v-col>
                <v-col
                  cols="6"
                  class="pl-1"
                >
                  <v-btn
                    class="action-btn submit-btn"
                    rounded="lg"
                    color="success-darken-2"
                    type="submit"
                    :loading="loading"
                    :disabled="loading"
                    block
                  >
                    <v-icon
                      class="btn-icon"
                      icon="ri-save-3-fill"
                    ></v-icon>
                    <span class="btn-text">บันทึก</span>
                  </v-btn>
                </v-col>
              </v-row>
            </div>
            <div
              v-else
              class="d-flex justify-center flex-wrap gap-2"
            >
              <v-btn
                class="action-btn cancel-btn me-2 mb-2 mb-sm-0"
                rounded="lg"
                color="error"
                @click="closeDialog"
                :loading="loading"
                :disabled="loading"
              >
                <v-icon
                  class="btn-icon"
                  icon="ri-close-line"
                ></v-icon>
                <span class="btn-text">ยกเลิก</span>
              </v-btn>
              <v-btn
                class="action-btn submit-btn"
                rounded="lg"
                color="success-darken-2"
                type="submit"
                :loading="loading"
                :disabled="loading"
              >
                <v-icon
                  class="btn-icon"
                  icon="ri-save-3-fill"
                ></v-icon>
                <span class="btn-text">บันทึก</span>
              </v-btn>
            </div>
          </v-col>
        </v-row>
      </v-form>
    </v-card-text>
  </v-card>
</template>

<script lang="ts">
import { Client, CreateOrganizationContactCommand } from '@/client'
import { BACKEND_API_URL } from '@/constants'
import { useSweetAlertStore } from '@/stores'
import { emailRules, phoneRules, positionRules } from '@/utils/RuleServices'
import { defineComponent } from 'vue'
const client = new Client(BACKEND_API_URL)

export default defineComponent({
  name: 'CreateContact',
  props: {
    CloseDialogCreate: {
      type: Function,
      required: true,
    },
    organizationId: {
      type: String,
      required: true,
    },
  },
  data() {
    return {
      createCommand: new CreateOrganizationContactCommand(),
      loading: false,
      sweetAlertStore: useSweetAlertStore(),
      organizationName: '',
      files: [] as any,
      positionRules,
      emailRules,
      phoneRules,
    }
  },
  mounted() {
    this.organizationId
    this.initialize()
    console.log(this.organizationId)
  },
  methods: {
    triggerFileInput() {
      const fileInput = this.$refs.fileInput as HTMLInputElement
      if (fileInput) {
        fileInput.click() // เปิด dialog สำหรับเลือกไฟล์
      }
    },
    UploadFile(event: any) {
      const file = event.target.files[0]
      this.files = file // เก็บไฟล์ที่เลือก
      if (file) {
        const reader = new FileReader()
        reader.onload = (e: any) => {
          this.createCommand.fileName = file.name // เก็บข้อมูล Base64 ของรูปภาพ
          this.createCommand.base64 = e.target.result.split(',')[1] // เก็บชื่อไฟล์
        }
        reader.readAsDataURL(file) // อ่านไฟล์เป็น Base64
      }
    },
    onClearFile() {
      this.files = []
      this.createCommand.fileName = ''
      this.createCommand.base64 = ''
      console.log('File cleared:', this.createCommand)
    },
    async initialize() {
      try {
        const response = await client.getOrganizationQueryByID(this.organizationId)
        this.organizationName = response.name ?? '' // เก็บชื่อองค์กร
      } catch (error) {
        console.error(error)
      }
    },
    closeDialog(reload: boolean = false) {
      this.CloseDialogCreate(false, reload) // ปิด dialog
    },
    async CreateContact() {
      const form = this.$refs.form as any
      const { valid } = await form.validate()
      if (valid) {
        try {
          this.loading = true
          this.createCommand.organizationId = this.organizationId
          const response = await client.createOrganizationContact(this.createCommand)
          if (response) {
            setTimeout(() => {
              this.loading = false
              this.sweetAlertStore.success('เพิ่มข้อมูลผู้ติดต่อสำเร็จ!')
              this.closeDialog(true)
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
  },
})
</script>

<style scoped>
/* Enhanced container styles */
.scroll-wrapper {
  max-height: 90vh;
  overflow: hidden;
  padding: 0;
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

.scroll-content {
  overflow-y: auto;
  overflow-x: hidden;
  max-height: calc(90vh - 140px);
  flex: 1;
  scrollbar-width: thin;
  scrollbar-color: #2b3086 #f8f9fa;
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
  line-height: 1.3;
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

.imgCard {
  border-radius: 20px;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1), 0 2px 8px rgba(0, 0, 0, 0.05);
  border: 2px solid rgba(255, 255, 255, 0.2);
  overflow: hidden;
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
    font-size: 20px;
    padding: 16px 8px 8px;
    line-height: 1.4;
  }

  .pr-1 {
    padding-right: 6px !important;
  }
  .pl-1 {
    padding-left: 6px !important;
  }
  .action-row {
    margin-bottom: 28% !important;
  }

  .action-btn {
    width: 100% !important;
    min-width: 100% !important;
    min-height: 56px;
    font-size: 16px;
    border-radius: 14px !important;
    margin: 0 !important;
  }

  .btn-icon {
    font-size: 18px;
    margin-right: 6px;
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

/* Firefox scrollbar */
.scroll-content {
  scrollbar-width: thin;
  scrollbar-color: #667eea rgba(0, 0, 0, 0.05);
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
</style>
