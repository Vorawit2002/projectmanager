<template>
  <v-row class="d-flex align-center justify-center">
    <v-col
      cols="12"
      md="12"
    >
      <v-card class="page-container scroll-content dialog-scrollbar">
        <v-card-text>
          <span class="text-sub-title"> รายละเอียดข้อมูลหน่วยงาน</span>

          <v-form
            ref="form"
            class="text-black"
          >
            <v-row class="px-1 px-sm-3 px-md-5 mt-3">
              <v-col
                cols="12"
                md="6"
              >
                <label class="mb-2">คำนำหน้า <span class="text-error">*</span></label>
                <v-text-field
                  v-model="UpdateCommand.titleName"
                  placeholder="ระบุคำนำหน้าชื่อ"
                  variant="outlined"
                  density="comfortable"
                  color="primary-darken-0"
                  dense
                  readonly
                ></v-text-field>
              </v-col>
              <v-col
                cols="12"
                md="6"
              />
              <v-col
                cols="12"
                md="6"
              >
                <label class="mb-2">ชื่อ <span class="text-error">*</span></label>
                <v-text-field
                  v-model="UpdateCommand.firstName"
                  placeholder="ระบุชื่อ"
                  variant="outlined"
                  density="comfortable"
                  color="primary-darken-0"
                  dense
                  readonly
                  :rules="nameRules"
                ></v-text-field>
              </v-col>

              <v-col
                cols="12"
                md="6"
              >
                <label class="mb-2">นามสกุล <span class="text-error">*</span></label>
                <v-text-field
                  v-model="UpdateCommand.lastName"
                  placeholder="ระบุนามสกุล"
                  variant="outlined"
                  density="comfortable"
                  color="primary-darken-0"
                  dense
                  readonly
                  :rules="surnameRules"
                ></v-text-field>
              </v-col>

              <v-col
                cols="12"
                md="12"
              >
                <label class="mb-2">หน่วยงาน <span class="text-error">*</span></label>
                <v-autocomplete
                  v-model="UpdateCommand.organizationId"
                  :items="OrganizationList"
                  item-title="name"
                  item-value="id"
                  placeholder="ระบุตำแหน่งงาน"
                  variant="outlined"
                  density="comfortable"
                  color="primary-darken-0"
                  dense
                  readonly
                  :rules="selectAgencyRules"
                ></v-autocomplete>
              </v-col>

              <v-col
                cols="12"
                md="12"
              >
                <label class="mb-2">ตำแหน่งงาน <span class="text-error">*</span></label>
                <v-text-field
                  v-model="UpdateCommand.position"
                  placeholder="ระบุตำแหน่งงาน"
                  variant="outlined"
                  density="comfortable"
                  color="primary-darken-0"
                  dense
                  readonly
                  :rules="positionRules"
                ></v-text-field>
              </v-col>

              <v-col
                cols="12"
                md="12"
              >
                <label class="mb-2">อีเมล <span class="text-error">*</span></label>
                <v-text-field
                  v-model="UpdateCommand.email"
                  placeholder="ระบุอีเมล"
                  variant="outlined"
                  density="comfortable"
                  color="primary-darken-0"
                  dense
                  readonly
                  :rules="emailRules"
                ></v-text-field>
              </v-col>

              <v-col
                cols="12"
                md="6"
              >
                <label class="mb-2">เบอร์โทร <span class="text-error">*</span></label>
                <v-text-field
                  v-model="UpdateCommand.phone"
                  placeholder="ระบุเบอร์โทร"
                  variant="outlined"
                  density="comfortable"
                  color="primary-darken-0"
                  dense
                  readonly
                  :rules="phoneRules"
                ></v-text-field>
              </v-col>

              <v-col
                cols="12"
                md="6"
              >
                <label class="mb-2">แฟกซ์</label>
                <v-text-field
                  v-model="UpdateCommand.fax"
                  placeholder="ระบุแฟกซ์"
                  variant="outlined"
                  density="comfortable"
                  color="primary-darken-0"
                  dense
                  readonly
                ></v-text-field>
              </v-col>

              <v-col
                cols="12"
                md="6"
              >
                <label class="mb-2">LineID</label>
                <v-text-field
                  v-model="UpdateCommand.lineId"
                  placeholder="ระบุLineID"
                  variant="outlined"
                  density="comfortable"
                  color="primary-darken-0"
                  dense
                  readonly
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
                  readonly
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
                ></v-file-input>
              </v-col>

              <v-col
                cols="0"
                md="6"
              ></v-col>

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
                  :src="
                    UpdateCommand.base64
                      ? 'data:image/jpeg;base64,' + UpdateCommand.base64
                      : UpdateCommand.imageProfile
                      ? 'data:image/jpeg;base64,' + UpdateCommand.imageProfile
                      : ''
                  "
                ></v-img>
              </v-col>

              <v-col
                cols="12"
                class="pt-6"
              >
                <div
                  class="button-container action-btn-row"
                  :style="{
                    justifyContent: 'center',
                    width: $vuetify.display.xs ? '100%' : 'auto',
                  }"
                >
                  <v-btn
                    class="mobile-btn cancel-btn"
                    rounded="lg"
                    color="error"
                    @click="closeDialog"
                    :style="{
                      width: $vuetify.display.xs ? '100%' : '50%',
                      maxWidth: $vuetify.display.xs ? 'none' : '220px',
                      margin: $vuetify.display.xs ? '0' : '0 auto',
                    }"
                  >
                    <v-icon
                      icon="ri-close-line"
                      class="mr-2"
                    ></v-icon>
                    ปิด
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
import { Client, UpdateOrganizationContactCommand } from '@/client'
import { BACKEND_API_URL } from '@/constants'
import {
  emailRules,
  faxRules,
  lineIdRules,
  nameRules,
  phoneRules,
  positionRules,
  prefixRules,
  selectAgencyRules,
  surnameRules,
} from '@/utils/RuleServices'
import { defineComponent } from 'vue'
const client = new Client(BACKEND_API_URL)

export default defineComponent({
  name: 'OrganizationContactDetailView',
  props: {
    id: {
      type: String,
      required: true,
    },
    CloseDialogDetail: {
      type: Function,
      required: true,
    },
  },
  data() {
    return {
      UpdateCommand: new UpdateOrganizationContactCommand() as any,
      OrganizationList: [] as any,
      prefixRules,
      nameRules,
      surnameRules,
      phoneRules,
      faxRules,
      emailRules,
      selectAgencyRules,
      positionRules,
      lineIdRules,
      files: [] as any,
    }
  },
  mounted() {
    console.log(this.id)
    this.initialize()
  },
  watch: {
    id() {
      this.initialize()
    },
  },
  methods: {
    async initialize() {
      try {
        this.OrganizationList = await client.getOrganizationQuery()
        console.log(this.OrganizationList)
      } catch (error) {
        console.error(error)
      }

      try {
        const response = await client.getOrganizationContactQueryByID(this.id)
        console.log(response)
        if (response) {
          this.UpdateCommand = response as UpdateOrganizationContactCommand
          if (this.UpdateCommand.attachmentId) {
            const result = await client.getAttachmentQueryById(this.UpdateCommand.attachmentId)
            this.files.name = result.nameFile
          }
        }
      } catch (error) {
        console.error(error)
      }
    },
    closeDialog(reload: boolean = false) {
      this.CloseDialogDetail(false, reload) // ปิด dialog
    },
  },
})
</script>

<style scoped>
/* Enhanced Image Card */
.imgCard {
  border-radius: 20px;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1), 0 2px 8px rgba(0, 0, 0, 0.05);
  border: 2px solid rgba(255, 255, 255, 0.2);
  overflow: hidden;
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

/* Enhanced Upload Container */
.upload-container {
  background: rgba(255, 255, 255, 0.9);
  border: 2px solid #e8eaed;
  border-radius: 16px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.04);
  backdrop-filter: blur(5px);
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

/* ===== RESPONSIVE BREAKPOINTS ===== */

/* Mobile Portrait (320px - 599px) */
@media (max-width: 599px) {
  .card-Dialog {
    margin: 0px;
    border-radius: 20px;
    /* max-height: calc(100vh - 16px); */
  }

  .text-sub-title {
    font-size: 30px;
    margin-bottom: 20px;
  }

  .form-label {
    font-size: 14px;
    margin-bottom: 6px;
  }

  .button-container {
    flex-direction: column;
    gap: 12px;
    width: 100%;
    max-width: 100%;
    margin: 0;
  }

  /* Margin below action button row on mobile */
  .action-btn-row {
    margin-bottom: 20% !important;
  }

  .mobile-btn {
    width: 100% !important;
    min-width: 100% !important;
    min-height: 56px;
    font-size: 16px;
    border-radius: 14px !important;
    margin: 0 !important;
  }

  .mobile-btn .v-icon {
    font-size: 18px;
  }

  /* Reorder buttons on mobile */
  .submit-btn {
    order: 1;
  }

  .cancel-btn {
    order: 2;
  }

  /* Stack form fields on mobile */
  .v-col[sm='4'],
  .v-col[sm='6'],
  .v-col[md='3'],
  .v-col[md='4'],
  .v-col[md='5'],
  .v-col[md='6'] {
    flex-basis: 100% !important;
    max-width: 100% !important;
    padding-bottom: 8px;
  }

  /* Image responsive */
  .imgCard {
    border-radius: 16px;
  }

  .v-img {
    max-width: 60% !important;
  }

  /* Adjust field spacing */
  .v-col {
    padding-top: 8px;
    padding-bottom: 8px;
  }

  /* Card padding adjustment */
  .card-Dialog .v-card-text {
    padding: 16px !important;
  }

  /* File input adjustments */
  .upload-container {
    border-radius: 14px;
  }
}

/* Mobile Landscape (480px - 767px) */
@media (min-width: 480px) and (max-width: 767px) and (orientation: landscape) {
  .card-Dialog {
    max-height: 98vh;
    margin: 4px;
  }

  .text-sub-title {
    font-size: 22px;
    padding: 12px 8px 0;
  }

  /* Allow horizontal button layout in landscape */
  .button-container {
    flex-direction: row !important;
    justify-content: center !important;
    gap: 12px !important;
  }

  .mobile-btn {
    width: auto !important;
    min-width: 140px !important;
    flex: 1;
    max-width: 200px;
  }

  /* Two columns for some fields in landscape */
  .v-col[sm='6'] {
    flex-basis: 50% !important;
    max-width: 50% !important;
  }

  .v-img {
    max-width: 50% !important;
  }
}

/* Tablet Portrait (600px - 959px) */
@media (min-width: 600px) and (max-width: 959px) {
  .card-Dialog {
    margin: 16px;
    border-radius: 24px;
    max-width: calc(100vw - 32px);
  }

  .text-sub-title {
    font-size: 24px;
    margin-bottom: 20px;
    padding: 20px 16px 0;
  }

  .form-label {
    font-size: 15px;
  }

  /* Responsive columns for tablet */
  .v-col[sm='4'] {
    flex-basis: 33.333% !important;
    max-width: 33.333% !important;
  }

  .v-col[sm='6'] {
    flex-basis: 50% !important;
    max-width: 50% !important;
  }

  .v-col[md='3'] {
    flex-basis: 33.333% !important;
    max-width: 33.333% !important;
  }

  .v-col[md='4'] {
    flex-basis: 33.333% !important;
    max-width: 33.333% !important;
  }

  .v-col[md='5'] {
    flex-basis: 41.666% !important;
    max-width: 41.666% !important;
  }

  .v-col[md='6'] {
    flex-basis: 50% !important;
    max-width: 50% !important;
  }

  .button-container {
    gap: 16px;
    max-width: 400px;
  }

  .mobile-btn {
    min-width: 160px;
    min-height: 48px;
    font-size: 15px;
  }

  .mobile-btn .v-icon {
    font-size: 19px;
  }

  .v-img {
    max-width: 45% !important;
  }

  .imgCard {
    border-radius: 18px;
  }

  .card-Dialog .v-card-text {
    padding: 20px !important;
  }
}

/* Tablet Landscape / Small Desktop (960px - 1263px) */
@media (min-width: 960px) and (max-width: 1263px) {
  .card-Dialog {
    margin: 20px auto;
    border-radius: 24px;
  }

  .text-sub-title {
    font-size: 26px;
    margin-bottom: 24px;
    padding: 24px 20px 0;
  }

  .button-container {
    gap: 18px;
    max-width: 450px;
  }

  .mobile-btn {
    min-width: 170px;
    min-height: 50px;
    font-size: 16px;
  }

  .mobile-btn .v-icon {
    font-size: 20px;
  }

  .v-img {
    max-width: 40% !important;
  }

  .card-Dialog .v-card-text {
    padding: 24px !important;
  }
}

/* Large Desktop (1264px+) */
@media (min-width: 1264px) {
  .card-Dialog {
    margin: 24px auto;
    border-radius: 28px;
  }

  .text-sub-title {
    font-size: 28px;
    margin-bottom: 24px;
    padding: 28px 24px 0;
  }

  .form-label {
    font-size: 16px;
  }

  .button-container {
    gap: 20px;
    max-width: 500px;
  }

  .mobile-btn {
    min-width: 180px;
    min-height: 52px;
    font-size: 16px;
  }

  .mobile-btn .v-icon {
    font-size: 20px;
  }

  .v-img {
    max-width: 35% !important;
  }

  .card-Dialog .v-card-text {
    padding: 32px !important;
  }
}

/* Ultra-wide screens (1920px+) */
@media (min-width: 1920px) {
  .text-sub-title {
    font-size: 32px;
  }

  .v-img {
    max-width: 30% !important;
  }
}

/* Extra small devices adjustments */
@media (max-width: 375px) {
  .text-sub-title {
    font-size: 18px;
    padding: 12px 6px 0;
  }

  .card-Dialog {
    margin: 4px;
    border-radius: 16px;
  }

  .card-Dialog .v-card-text {
    padding: 12px !important;
  }

  .form-label {
    font-size: 13px;
  }

  .mobile-btn {
    min-height: 52px;
    font-size: 15px;
  }

  .mobile-btn .v-icon {
    font-size: 16px;
  }

  .v-img {
    max-width: 70% !important;
  }
}

/* ===== NAME FIELD RESPONSIVE LAYOUT ===== */

/* Mobile: Stack all name fields */
@media (max-width: 599px) {
  .v-col[sm='4'][md='3'],
  .v-col[sm='4'][md='5'],
  .v-col[sm='4'][md='4'] {
    flex-basis: 100% !important;
    max-width: 100% !important;
  }
}

/* Tablet: Show name fields in row */
@media (min-width: 600px) and (max-width: 959px) {
  .v-col[sm='4'][md='3'] {
    flex-basis: 25% !important;
    max-width: 25% !important;
  }

  .v-col[sm='4'][md='5'] {
    flex-basis: 40% !important;
    max-width: 40% !important;
  }

  .v-col[sm='4'][md='4'] {
    flex-basis: 35% !important;
    max-width: 35% !important;
  }
}

/* Desktop: Use original layout */
@media (min-width: 960px) {
  .v-col[md='3'] {
    flex-basis: 25% !important;
    max-width: 25% !important;
  }

  .v-col[md='5'] {
    flex-basis: 41.666% !important;
    max-width: 41.666% !important;
  }

  .v-col[md='4'] {
    flex-basis: 33.333% !important;
    max-width: 33.333% !important;
  }
}

/* ===== ACCESSIBILITY & PERFORMANCE ===== */

/* High contrast mode support */
@media (prefers-contrast: high) {
  .card-Dialog {
    border: 2px solid #2b3086;
    background: white;
  }

  .mobile-btn {
    border: 2px solid currentColor !important;
  }

  .text-sub-title {
    background: none !important;
    -webkit-text-fill-color: initial !important;
    color: #000 !important;
    text-shadow: none;
  }

  .imgCard {
    border: 3px solid #000;
  }
}

/* Print styles */
@media print {
  .card-Dialog {
    box-shadow: none;
    border: 1px solid #ccc;
    max-height: none;
    overflow: visible;
    margin: 0;
    padding: 20px;
  }

  .button-container {
    display: none !important;
  }

  .text-sub-title {
    color: #000 !important;
    background: none !important;
    -webkit-text-fill-color: initial !important;
  }

  .imgCard {
    border: 1px solid #ccc;
  }
}

/* Touch optimization */
@media (pointer: coarse) {
  .mobile-btn {
    min-height: 56px; /* Larger touch targets */
  }
}

/* iOS Safari specific fixes */
@supports (-webkit-touch-callout: none) {
  .card-Dialog {
    -webkit-overflow-scrolling: touch;
  }

  .mobile-btn {
    appearance: none;
    -webkit-appearance: none;
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

.mobile-btn.v-btn--loading {
  animation: pulse 1.5s infinite;
}
</style>
