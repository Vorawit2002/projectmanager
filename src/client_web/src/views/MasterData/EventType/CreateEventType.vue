<template>
  <v-row class="d-flex align-center justify-center">
    <v-col
      cols="12"
      md="12"
    >
      <v-card class="page-container scroll-content dialog-scrollbar">
        <v-card-text>
          <span class="text-sub-title"> สร้างประเภทกิจกรรม</span>

          <v-form
            ref="form"
            @submit.prevent="CreateEventType"
            class="text-black"
          >
            <v-row class="px-1 px-sm-3 px-md-5 mt-3">
              <v-col
                cols="12"
                md="12"
              >
                <label class="mb-2">ชื่อ<span class="text-error"> *</span></label>
                <v-text-field
                  v-model="form.name"
                  placeholder="ระบุชื่อประเภทกิจกรรม"
                  variant="outlined"
                  density="comfortable"
                  color="primary-darken-0"
                  :rules="eventTypeNameRules"
                  dense
                ></v-text-field>
              </v-col>

              <v-col
                cols="0"
                md="6"
              ></v-col>

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
                    block
                    :loading="loading"
                    :disabled="loading"
                  >
                    <v-icon class="mr-2">ri-save-3-fill</v-icon>
                    บันทึก
                  </v-btn>
                  <v-btn
                    class="mobile-btn cancel-btn"
                    rounded="lg"
                    color="error"
                    @click="CancelCreate"
                    block
                  >
                    <v-icon
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
import { Client, CreateEventTypeCommand } from '@/client'
import { defineComponent } from 'vue'
import { BACKEND_API_URL } from '@/constants'
import { useSweetAlertStore } from '@/stores'
import { eventTypeNameRules } from '@/utils/RuleServices'
const client = new Client(BACKEND_API_URL)

export default defineComponent({
  name: 'CreateEventType',
  data() {
    return {
      form: {
        name: '',
      },
      loading: false,
      sweetAlertStore: useSweetAlertStore(),
      eventTypeNameRules,
    }
  },
  methods: {
    async CreateEventType() {
      const formRef = this.$refs.form as any
      const { valid } = await formRef.validate()
      if (!valid) return
      this.loading = true
      try {
        const command = new CreateEventTypeCommand()
        command.name = this.form.name
        const response = await client.createEventType(command)
        if (response) {
          this.sweetAlertStore.success('สร้างประเภทกิจกรรมสำเร็จ')
          this.$emit('created')
          this.$emit('close')
        }
      } catch (error) {
        console.error(error)
        this.sweetAlertStore.error('เกิดข้อผิดพลาดในการสร้างประเภทกิจกรรม!')
      } finally {
        this.loading = false
      }
    },
    CancelCreate() {
      this.$emit('close')
    },
  },
})
</script>

<style scoped>
/* Enhanced Image Card */
.imgCard {
  border-radius: 20px;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.12), 0 2px 8px rgba(0, 0, 0, 0.08);
  border: 3px solid rgba(255, 255, 255, 0.2);
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



/* Enhanced Typography */
.text-title {
  font-size: 25px;
  font-weight: 700;
  color: #2b3086;
  text-align: center;
  margin-bottom: 16px;
}

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

/* Enhanced Labels */
label {
  font-size: 15px;
  font-weight: 500;
  color: #333;
  margin-bottom: 8px;
  display: block;
  letter-spacing: 0.025em;
}

.textarea-field :deep(.v-field) {
  border-radius: 20px;
}

.textarea-field :deep(.v-field__input) {
  padding: 20px;
}

.checkbox-field :deep(.v-selection-control) {
  min-height: 32px;
}

.checkbox-field :deep(.v-selection-control__wrapper) {
  border-radius: 8px;
}

.add-btn {
  opacity: 0.7;
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
  .text-title {
    font-size: 20px;
    margin-bottom: 12px;
  }

  .text-sub-title {
    font-size: 30px !important;
    margin-bottom: 20px !important;
  }

  label {
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

  /* Hide empty columns */
  .v-col[cols='0'] {
    display: none !important;
  }


/* Mobile Landscape (480px - 767px) */
  .text-title {
    font-size: 22px;
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

  /* Hide empty columns */
  .v-col[cols='0'] {
    display: none !important;
  }
}

/* Tablet Portrait (600px - 959px) */
@media (min-width: 600px) and (max-width: 959px) {

  .text-title {
    font-size: 24px;
  }

  .text-sub-title {
    font-size: 24px;
    margin-bottom: 20px;
    padding: 20px 16px 0;
  }

  label {
    font-size: 15px;
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

  /* Show empty columns as spacers */
  .v-col[cols='0'][md='6'] {
    display: block !important;
    flex-basis: 50% !important;
    max-width: 50% !important;
  }
}

/* Tablet Landscape / Small Desktop (960px - 1263px) */
@media (min-width: 960px) and (max-width: 1263px) {
  .card-Dialog {
    margin: 20px auto;
    border-radius: 24px;
  }

  .text-title {
    font-size: 25px;
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

  /* Show empty columns as spacers */
  .v-col[cols='0'][md='6'] {
    display: block !important;
    flex-basis: 50% !important;
    max-width: 50% !important;
  }
}

/* Large Desktop (1264px+) */
@media (min-width: 1264px) {
  .card-Dialog {
    margin: 24px auto;
    border-radius: 28px;
  }

  .text-title {
    font-size: 25px;
  }

  .text-sub-title {
    font-size: 28px;
    margin-bottom: 24px;
    padding: 28px 24px 0;
  }

  label {
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

  /* Show empty columns as spacers */
  .v-col[cols='0'][md='6'] {
    display: block !important;
    flex-basis: 50% !important;
    max-width: 50% !important;
  }
}

/* Ultra-wide screens (1920px+) */
@media (min-width: 1920px) {
  .text-title {
    font-size: 28px;
  }

  .text-sub-title {
    font-size: 32px;
  }
}

/* Extra small devices adjustments */
@media (max-width: 375px) {
  .text-title {
    font-size: 18px;
    margin-bottom: 10px;
  }

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

  label {
    font-size: 13px;
  }

  .mobile-btn {
    min-height: 52px;
    font-size: 15px;
  }

  .mobile-btn .v-icon {
    font-size: 16px;
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

  .text-sub-title,
  .text-title {
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

  .text-sub-title,
  .text-title {
    color: #000 !important;
    background: none !important;
    -webkit-text-fill-color: initial !important;
  }

  .imgCard {
    border: 1px solid #ccc;
  }

  /* Hide empty columns in print */
  .v-col[cols='0'] {
    display: none !important;
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

/* Enhanced single field form styling */
.v-row {
  justify-content: center;
}

.v-col[md='12'] {
  max-width: 600px;
}

/* Ensure proper spacing for simple forms */
@media (min-width: 960px) {
  .v-row .v-col[md='12']:only-of-type {
    flex-basis: 66.666%;
    max-width: 66.666%;
  } 
}

/* Enhanced empty column handling */
.v-col[cols='0'] {
  display: none;
}

@media (min-width: 600px) {
  .v-col[cols='0'][md='6'] {
    display: block;
    flex-basis: 50%;
    max-width: 50%;
  }
}
</style>
