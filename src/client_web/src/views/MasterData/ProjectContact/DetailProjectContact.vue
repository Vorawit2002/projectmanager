<template>
  <v-row class="d-flex align-center justify-center">
    <v-col
      cols="12"
      md="12"
    >
      <v-card class="page-container scroll-content">
        <v-card-text>
          <span class="text-sub-title">รายละเอียดข้อมูลหน่วยงาน</span>
          <v-form
            ref="form"
            class="text-black"
          >
            <v-row class="px-1 px-sm-3 px-md-5 mt-3">
              <v-col
                cols="12"
                md="12"
              >
                <label class="mb-2">โครงการ <span class="text-error">*</span></label>
                <v-autocomplete
                  v-model="UpdateCommand.projectId"
                  :items="ProjectList"
                  item-title="projectName"
                  item-value="id"
                  placeholder="เลือกโครงการ"
                  variant="outlined"
                  density="comfortable"
                  color="primary-darken-0"
                  dense
                  readonly
                  :rules="selectProjectRules"
                />
              </v-col>

              <v-col
                cols="12"
                md="12"
              >
                <label class="mb-2">ผู้ติดต่องาน <span class="text-error">*</span></label>
                <v-autocomplete
                  v-model="UpdateCommand.organizationContactId"
                  :items="OrganizationContactList"
                  item-title="fullName"
                  item-value="id"
                  placeholder="เลือกผู้ติดต่องาน"
                  variant="outlined"
                  density="comfortable"
                  color="primary-darken-0"
                  dense
                  readonly
                  :rules="selectContactRules"
                />
              </v-col>

              <v-col
                cols="12"
                class="pt-6"
              >
                <div
                  class="button-container"
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
import { Client, UpdateProjectContactCommand } from '@/client'
import { BACKEND_API_URL } from '@/constants'
import { useSweetAlertStore } from '@/stores'
import { selectContactRules, selectProjectRules } from '@/utils/RuleServices'
import { defineComponent } from 'vue'
import { id } from 'vuetify/locale'

const client = new Client(BACKEND_API_URL)

export default defineComponent({
  name: 'DetailProjectContact',
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
      UpdateCommand: new UpdateProjectContactCommand(),
      ProjectList: [] as any[],
      OrganizationContactList: [] as any,
      selectProjectRules,
      selectContactRules,
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
        // console.log(this.id)
        const response = await client.getProjectContactQueryByID(this.id)
        // console.log(response)
        if (response) {
          this.UpdateCommand = response as UpdateProjectContactCommand
        }
      } catch (error) {
        console.error(error)
      }

      try {
        this.ProjectList = await client.getProjectQuery()
        // console.log(this.ProjectList)
      } catch (error) {
        console.error(error)
      }

      try {
        const response = await client.getOrganizationContactQuery()
        this.OrganizationContactList = response.map(item => ({
          ...item,
          fullName: `${item.firstName || ''} ${item.lastName || ''}`.trim(),
        }))
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

/* Enhanced Form Row */
.v-row {
  min-height: 200px;
  align-items: flex-start;
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

  /* Stack form fields on mobile */
  .v-col[sm='6'],
  .v-col[md='6'] {
    flex-basis: 100% !important;
    max-width: 100% !important;
    padding-bottom: 12px;
  }

  .button-container {
    flex-direction: column;
    gap: 12px;
    width: 100%;
    max-width: 100%;
    margin: 0;
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

  /* Adjust field spacing */
  .v-col {
    padding-top: 8px;
    padding-bottom: 8px;
  }

  /* Card padding adjustment */
  .card-Dialog .v-card-text {
    padding: 16px !important;
  }

  /* Autocomplete mobile adjustments */
  .long-text-autocomplete .v-overlay__content {
    max-width: calc(100vw - 32px) !important;
  }

  .long-text-autocomplete .v-list-item {
    padding: 10px 16px !important;
    margin: 2px 4px !important;
  }

  /* Reduce row min-height on mobile */
  .v-row {
    min-height: 150px;
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

  /* Two columns for fields in landscape */
  .v-col[sm='6'] {
    flex-basis: 50% !important;
    max-width: 50% !important;
  }

  .v-row {
    min-height: 120px;
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

  /* Two columns for form fields */
  .v-col[sm='6'] {
    flex-basis: 50% !important;
    max-width: 50% !important;
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

  .card-Dialog .v-card-text {
    padding: 20px !important;
  }

  /* Autocomplete spacing for tablet */
  .v-col:has(.v-autocomplete) {
    margin-bottom: 12px;
  }

  .v-row {
    min-height: 180px;
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

  .card-Dialog .v-card-text {
    padding: 24px !important;
  }

  /* Autocomplete spacing for desktop */
  .v-col:has(.v-autocomplete) {
    margin-bottom: 16px;
  }

  .v-row {
    min-height: 200px;
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

  .card-Dialog .v-card-text {
    padding: 32px !important;
  }

  /* Enhanced autocomplete spacing */
  .v-col:has(.v-autocomplete) {
    margin-bottom: 16px;
  }

  .v-row {
    min-height: 220px;
  }
}

/* Ultra-wide screens (1920px+) */
@media (min-width: 1920px) {
  .text-sub-title {
    font-size: 32px;
  }

  .v-row {
    min-height: 240px;
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

  .v-row {
    min-height: 120px;
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

  .long-text-autocomplete .v-overlay__content {
    border: 3px solid #000 !important;
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

  .v-row {
    min-height: auto;
  }
}

/* Touch optimization */
@media (pointer: coarse) {
  .mobile-btn {
    min-height: 56px; /* Larger touch targets */
  }

  .long-text-autocomplete .v-list-item {
    min-height: 44px !important; /* Larger touch targets */
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

/* Enhanced Autocomplete Menu */
.long-text-autocomplete .v-list {
  padding: 8px !important;
}

.long-text-autocomplete .v-list-item--density-comfortable {
  min-height: auto !important;
}

/* Menu positioning improvements */
.long-text-autocomplete :deep(.v-overlay__content) {
  margin-top: 4px;
}
</style>
