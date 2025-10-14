<template>
  <v-row class="d-flex align-center justify-center">
    <v-col
      cols="12"
      md="12"
    >
      <VCard class="scroll-content page-container dialog-scrollbar">
        <v-card-text>
          <span class="text-sub-title"> รายละเอียดข้อมูลหน่วยงาน</span>

          <v-form
            ref="form"
            class="text-black"
          >
            <v-row class="px-1 px-sm-3 px-md-5 mt-3">
              <v-col
                cols="12"
                sm="6"
                md="6"
              >
                <label class="mb-2">ชื่อหน่วยงาน <span class="text-error">*</span></label>
                <v-text-field
                  v-model="UpdateCommand.name"
                  placeholder="ระบุชื่อหน่วยงาน"
                  variant="outlined"
                  density="comfortable"
                  color="primary-darken-0"
                  dense
                  readonly
                  :rules="agencyRules"
                ></v-text-field>
              </v-col>

              <v-col
                cols="12"
                sm="6"
                md="6"
              >
                <label class="mb-2">ประเภทหน่วยงาน <span class="text-error">*</span></label>
                <v-autocomplete
                  v-model="UpdateCommand.typeOrganization"
                  :items="TypeOrganizationEnum"
                  item-title="title"
                  item-value="value"
                  placeholder="เลือกประเภทหน่วยงาน"
                  variant="outlined"
                  density="comfortable"
                  color="primary-darken-0"
                  dense
                  readonly
                  :rules="agencyTypeRules"
                />
              </v-col>

              <v-col
                cols="12"
                md="12"
              >
                <label class="mb-2">ที่อยู่ <span class="text-error">*</span></label>
                <v-text-field
                  v-model="UpdateCommand.address"
                  placeholder="ระบุที่อยู่หน่วยงาน"
                  variant="outlined"
                  density="comfortable"
                  color="primary-darken-0"
                  dense
                  readonly
                  :rules="addressRules"
                ></v-text-field>
              </v-col>

              <v-col
                cols="12"
                sm="6"
                md="6"
              >
                <label class="mb-2">เว็บไซต์ <span class="text-error">*</span></label>
                <v-text-field
                  v-model="UpdateCommand.webSite"
                  placeholder="ระบุเว็บไซต์หน่วยงาน"
                  variant="outlined"
                  density="comfortable"
                  color="primary-darken-0"
                  dense
                  readonly
                  :rules="websiteRules"
                ></v-text-field>
              </v-col>

              <v-col
                cols="12"
                sm="6"
                md="6"
              >
                <label class="mb-2">เบอร์โทร <span class="text-error">*</span></label>
                <v-text-field
                  v-model="UpdateCommand.phone"
                  placeholder="ระบุเบอร์โทรหน่วยงาน"
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
                sm="6"
                md="6"
              >
                <label class="mb-2">แฟกซ์ <span class="text-error">*</span></label>
                <v-text-field
                  v-model="UpdateCommand.fax"
                  placeholder="ระบุแฟกซ์หน่วยงาน"
                  variant="outlined"
                  density="comfortable"
                  color="primary-darken-0"
                  dense
                  readonly
                  :rules="faxRules"
                ></v-text-field>
              </v-col>

              <v-col
                cols="12"
                sm="6"
                md="6"
              >
                <label class="mb-2">พิกัด <span class="text-error">*</span></label>
                <v-text-field
                  v-model="UpdateCommand.coordinates"
                  placeholder="ระบุพิกัดหน่วยงาน"
                  variant="outlined"
                  density="comfortable"
                  color="primary-darken-0"
                  dense
                  readonly
                  :rules="coordinateRules"
                ></v-text-field>
              </v-col>

              <!-- แสดง map ตามพิกัดที่ระบุ -->
              <v-col
                cols="12"
                md="12"
              >
                <label class="mb-2">Google Map</label>

                <!-- แสดง map เมื่อมีพิกัด -->
                <div
                  v-if="isValidCoordinates"
                  class="map-container"
                >
                  <iframe
                    :src="googleMapsUrl"
                    width="100%"
                    height="300"
                    style="border: 0; border-radius: 8px"
                    loading="lazy"
                    referrerpolicy="no-referrer-when-downgrade"
                  >
                  </iframe>
                </div>

                <!-- แสดงข้อความเมื่อยังไม่มีพิกัดหรือพิกัดไม่ถูกต้อง -->
                <div
                  v-else
                  class="map-placeholder"
                >
                  <v-card
                    height="auto"
                    class="d-flex align-center justify-center"
                    color="grey-lighten-4"
                  >
                    <div class="text-center">
                      <v-icon
                        size="64"
                        color="grey-darken-1"
                        class="mb-4"
                      >
                        mdi-map-marker-outline
                      </v-icon>
                      <p class="text-grey-darken-1">
                        {{ coordinatesMessage }}
                      </p>
                    </div>
                  </v-card>
                </div>
              </v-col>

              <v-col
                cols="12"
              >
                <div class="button-container action-btn-row">
                  <v-btn
                    class="mobile-btn cancel-btn"
                    rounded="lg"
                    color="error"
                    @click="closeDialog"
                    block
                  >
                    <v-icon
                      icon="ri-close-line"
                      class="mr-2"
                    ></v-icon>
                    ปิด
                  </v-btn>
                  <!-- <v-btn
                    class="mobile-btn submit-btn"
                    rounded="lg"
                    color="success-darken-2"
                    type="submit"
                    block
                  >
                    <v-icon class="mr-2">ri-save-3-fill</v-icon>
                    บันทึก
                  </v-btn> -->
                </div>
              </v-col>
            </v-row>
          </v-form>
        </v-card-text>
      </VCard>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { TypeOrganizationEnum } from '@/@layouts/enums'
import { Client, UpdateOrganizationCommand } from '@/client'
import { BACKEND_API_URL } from '@/constants'
import { useSweetAlertStore } from '@/stores'
import {
  addressRules,
  agencyRules,
  agencyTypeRules,
  coordinateRules,
  faxRules,
  phoneRules,
  websiteRules,
} from '@/utils/RuleServices'
import { defineComponent } from 'vue'

const client = new Client(BACKEND_API_URL)

export default defineComponent({
  name: 'OrganizationDetail',
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
      UpdateCommand: new UpdateOrganizationCommand(),
      loading: false,
      sweetAlertStore: useSweetAlertStore(),
      TypeOrganizationEnum,
      phoneRules,
      agencyRules,
      agencyTypeRules,
      addressRules,
      coordinateRules,
      websiteRules,
      faxRules,
    }
  },
  mounted() {
    console.log(this.id)
    this.initialize()
  },
  watch: {
    id: {
      immediate: true,
      handler() {
        this.initialize()
      },
    },
  },
  methods: {
    async initialize() {
      try {
        const response = await client.getOrganizationQueryByID(this.id)
        console.log(response)
        if (response) {
          this.UpdateCommand = response as UpdateOrganizationCommand
        }
      } catch (error) {
        console.error(error)
      }
    },
    closeDialog(reload: boolean = false) {
      this.CloseDialogDetail(false, reload) // ปิด dialog
    },
    // แปลงพิกัดจาก string เป็น object
    parseCoordinates(coordinatesString: string): { lat: number; lng: number } | null {
      if (!coordinatesString) return null

      const cleanCoords = coordinatesString.trim()
      const patterns = [
        /^(-?\d+\.?\d*),\s*(-?\d+\.?\d*)$/, // lat,lng หรือ lat, lng
        /^(-?\d+\.?\d*)\s+(-?\d+\.?\d*)$/, // lat lng
      ]

      for (const pattern of patterns) {
        const match = cleanCoords.match(pattern)
        if (match) {
          const lat = parseFloat(match[1])
          const lng = parseFloat(match[2])

          // ตรวจสอบว่าเป็นพิกัดที่ถูกต้อง
          if (isFinite(lat) && isFinite(lng) && lat >= -90 && lat <= 90 && lng >= -180 && lng <= 180) {
            return { lat, lng }
          }
        }
      }

      return null
    },
  },
  computed: {
    // ตรวจสอบว่าพิกัดถูกต้องหรือไม่
    isValidCoordinates(): boolean {
      if (!this.UpdateCommand?.coordinates) return false

      const coords = this.parseCoordinates(this.UpdateCommand.coordinates)
      return coords !== null
    },

    // สร้าง URL สำหรับ Google Maps
    googleMapsUrl(): string {
      if (!this.isValidCoordinates) return ''

      // เพิ่มการตรวจสอบเพื่อแก้ TypeScript error
      const coordinatesString = this.UpdateCommand?.coordinates
      if (!coordinatesString) return ''

      const coords = this.parseCoordinates(coordinatesString)
      if (!coords) return ''

      // ใช้ Google Maps โดยไม่ต้อง API Key (สำหรับการทดสอบ)
      return `https://maps.google.com/maps?q=${coords.lat},${coords.lng}&hl=th&z=15&output=embed`

      // หรือใช้ Google Maps Embed API (ต้อง API Key)
      // return `https://www.google.com/maps/embed/v1/place?key=YOUR_API_KEY&q=${coords.lat},${coords.lng}&zoom=15`
    },

    // ข้อความที่แสดงเมื่อไม่มีพิกัดหรือพิกัดไม่ถูกต้อง
    coordinatesMessage(): string {
      const coords = this.UpdateCommand?.coordinates

      if (!coords) {
        return 'กรุณาระบุพิกัดเพื่อแสดงแผนที่'
      } else {
        return 'รูปแบบพิกัดไม่ถูกต้อง\nตัวอย่าง: 13.7563, 100.5018'
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

/* Enhanced Map Container */
.map-container {
  border-radius: 20px;
  overflow: hidden;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1), 0 2px 8px rgba(0, 0, 0, 0.05);
  border: 2px solid rgba(255, 255, 255, 0.2);
  background: #f8fafc;
  position: relative;
  height: 350px;
}

.map-container iframe {
  width: 100%;
  height: 350px;
  border: none;
  border-radius: 20px;
}

.map-placeholder {
  border-radius: 20px;
  overflow: hidden;
  height: 300px;
}

.map-placeholder .v-card {
  height: 300px !important;
  border-radius: 20px;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1), 0 2px 8px rgba(0, 0, 0, 0.05);
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
    margin-bottom: 25px !important;
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

  /* Margin below action button row on mobile */
  .action-btn-row {
    margin-bottom: 20% !important;
  }

  /* Reorder buttons on mobile */
  .submit-btn {
    order: 1;
  }

  .cancel-btn {
    order: 2;
  }

  /* Stack form fields on mobile */
  .v-col[sm='6'] {
    flex-basis: 100% !important;
    max-width: 100% !important;
    padding-bottom: 8px;
  }

  /* Map responsive */
  .map-container {
    height: 300px;
    border-radius: 16px;
  }

  .map-container iframe {
    height: 300px;
    border-radius: 16px;
  }

  .map-placeholder {
    height: 250px;
    border-radius: 16px;
  }

  .map-placeholder .v-card {
    height: 250px !important;
    border-radius: 16px;
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

  .map-container {
    height: 280px;
  }

  .map-container iframe {
    height: 280px;
  }

  .map-placeholder {
    height: 220px;
  }

  .map-placeholder .v-card {
    height: 220px !important;
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

  .map-container {
    height: 320px;
  }

  .map-container iframe {
    height: 320px;
  }

  .map-placeholder {
    height: 280px;
  }

  .map-placeholder .v-card {
    height: 280px !important;
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

  .map-container {
    height: 350px;
  }

  .map-container iframe {
    height: 350px;
  }

  .map-placeholder {
    height: 300px;
  }

  .map-placeholder .v-card {
    height: 300px !important;
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

  .map-container {
    height: 400px;
  }

  .map-container iframe {
    height: 400px;
  }

  .map-placeholder {
    height: 350px;
  }

  .map-placeholder .v-card {
    height: 350px !important;
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

  .map-container {
    height: 450px;
  }

  .map-container iframe {
    height: 450px;
  }

  .map-placeholder {
    height: 400px;
  }

  .map-placeholder .v-card {
    height: 400px !important;
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

  .map-container {
    height: 250px;
  }

  .map-container iframe {
    height: 250px;
  }

  .map-placeholder {
    height: 200px;
  }

  .map-placeholder .v-card {
    height: 200px !important;
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

  .map-container,
  .map-placeholder {
    border: 1px solid #ccc;
    background: #f5f5f5;
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
