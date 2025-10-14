  <template>
  <div class="onsite-container">
    <!-- Header Title with CRM styling -->
    <div class="text-center text-sub-title mb-15">
      ระบบการบันทึกเวลาเข้า - ออกงาน<br />
      สำหรับพนักงานที่ประจำไซต์ (Onsite)
    </div>

    <!-- Cards Grid with CRM styling -->
    <v-row class="d-flex justify-center px-0 equal-height-row">
      <!-- Check-in Card -->
      <v-col
        cols="12"
        md="6"
        lg="4"
        class="px-5 py-5 px-md-8 d-flex align-stretch"
      >
        <v-card
          width="100%"
          class="mx-auto mb-8 Card-Radius d-flex flex-column equal-height-card hover-effect animate__animated animate__bounceInUp cursor-pointer"
          @click="navigateToCheckIn"
        >
          <!-- Icon -->
          <div class="text-center mt-8">
            <v-icon
              size="80"
              color="#4CAF50"
              class="IMGLogo flex-shrink-0 z-10"
            >
              ri-login-circle-line
            </v-icon>
          </div>

          <!-- Title -->
          <v-card-text class="text-center mt-5 font-weight-bold flex-shrink-0 z-10"> บันทึกเวลาเข้างาน </v-card-text>

          <!-- Description -->
          <v-card-title class="text-center flex-grow-1 d-flex justify-center card-detail-full z-10">
            <div class="text-center">ลงเวลาเข้างานพร้อมถ่ายภาพและบันทึกตำแหน่งสำหรับพนักงานที่ประจำไซต์</div>
          </v-card-title>
        </v-card>
      </v-col>

      <!-- Check-out Card -->
      <v-col
        cols="12"
        md="6"
        lg="4"
        class="px-5 py-5 px-md-8 d-flex align-stretch"
      >
        <v-card
          width="100%"
          class="mx-auto mb-8 Card-Radius d-flex flex-column equal-height-card hover-effect animate__animated animate__bounceInUp cursor-pointer"
          @click="navigateToCheckOut"
        >
          <!-- Icon -->
          <div class="text-center mt-8">
            <v-icon
              size="80"
              color="#f44336"
              class="IMGLogo flex-shrink-0 z-10"
            >
              ri-logout-circle-line
            </v-icon>
          </div>

          <!-- Title -->
          <v-card-text class="text-center mt-5 font-weight-bold flex-shrink-0 z-10"> บันทึกเวลาออกงาน </v-card-text>

          <!-- Description -->
          <v-card-title class="text-center flex-grow-1 d-flex justify-center card-detail-full z-10">
            <div class="text-center">ลงเวลาออกงานพร้อมถ่ายภาพและบันทึกตำแหน่งเพื่อสรุปการทำงานประจำวัน</div>
          </v-card-title>
        </v-card>
      </v-col>

      <!-- History Card -->
      <v-col
        cols="12"
        md="6"
        lg="4"
        class="px-5 py-5 px-md-8 d-flex align-stretch"
      >
        <v-card
          width="100%"
          class="mx-auto mb-8 Card-Radius d-flex flex-column equal-height-card hover-effect animate__animated animate__bounceInUp cursor-pointer"
          @click="navigateToHistory"
        >
          <!-- Icon -->
          <div class="text-center mt-8">
            <v-icon
              size="80"
              color="#FF9800"
              class="IMGLogo flex-shrink-0 z-10"
            >
              ri-history-line
            </v-icon>
          </div>

          <!-- Title -->
          <v-card-text class="text-center mt-5 font-weight-bold flex-shrink-0 z-10"> ประวัติการลงเวลา </v-card-text>

          <!-- Description -->
          <v-card-title class="text-center flex-grow-1 d-flex justify-center card-detail-full z-10">
            <div class="text-center">ดูประวัติการลงเวลาเข้า-ออกงาน สถิติการทำงาน และรายงานการเข้างาน</div>
          </v-card-title>
        </v-card>
      </v-col>
    </v-row>

    <!-- OnsiteForm Dialog -->
    <v-dialog
      v-model="dialogVisible"
      :max-width="$vuetify.display.xs ? '100%' : '1200px'"
      :max-height="$vuetify.display.xs ? '100vh' : '90vh'"
      transition="dialog-top-transition"
      scrollable
      :fullscreen="$vuetify.display.xs"
    >
      <v-card class="dialog-card">
        <OnsiteForm
          v-if="dialogVisible"
          :mode="currentMode"
          @close="closeDialog"
        />
      </v-card>
    </v-dialog>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import OnsiteForm from './OnsiteForm.vue'

export default defineComponent({
  name: 'OnsiteViews',
  components: {
    OnsiteForm,
  },
  data() {
    return {
      dialogVisible: false,
      currentMode: 'checkin' as 'checkin' | 'checkout',
      handleEscKey: null as any,
    }
  },
  mounted() {
    // Add ESC key handler
    this.handleEscKey = (e: KeyboardEvent) => {
      if (e.key === 'Escape' || e.key === 'Esc') {
        if (this.dialogVisible) {
          this.closeDialog()
        }
      }
    }
    window.addEventListener('keydown', this.handleEscKey)
  },
  beforeUnmount() {
    if (this.handleEscKey) {
      window.removeEventListener('keydown', this.handleEscKey)
    }
  },
  methods: {
    navigateToCheckIn() {
      this.currentMode = 'checkin'
      this.dialogVisible = true
    },
    navigateToCheckOut() {
      this.currentMode = 'checkout'
      this.dialogVisible = true
    },
    navigateToHistory() {
      this.$router.push({ name: 'OnsiteDetailViews' })
    },
    closeDialog() {
      this.dialogVisible = false
    },
  },
})
</script>

<style scoped>
/* Base container */
/* .onsite-container {
  background-color: #f5f5f5;
} */

/* Header Styles matching CreateActivityDetail */
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

/* Card Styling from CRMUltraFeature */
.Card-Radius {
  border-radius: 12px;
  --font-color-sub: #666;

  --main-color: #222566;
  --main-focus: #2d8cf0;
  background: var(--bg-color);
  border: 2px solid var(--main-color);
  box-shadow: 4px 4px var(--main-color);
  transition: transform 0.2s ease;
}

.Card-Radius:hover {
  transform: translateY(-4px);
}

/* Card detail text styling */
.card-detail-full {
  white-space: normal !important;
  font-size: 16px !important;
  font-weight: normal !important;
  line-height: 1.6 !important;
}

/* Hover effect from CRMUltraFeature */
.hover-effect {
  position: relative;
  overflow: hidden;
  color: #333 !important;
}

.hover-effect::before {
  content: '';
  position: absolute;
  top: -100%;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgb(var(--v-theme-primary));
  transition: top 0.3s ease;
  z-index: 1;
}

.hover-effect:hover::before {
  top: 0;
}

/* Text color changes on hover */
.hover-effect:hover,
.hover-effect:hover .card-detail-full,
.hover-effect:hover .v-card-title,
.hover-effect:hover .v-card-text,
.hover-effect:hover .IMGLogo,
.hover-effect:hover .v-icon {
  color: #fff !important;
  z-index: 10;
}

/* Icon styling */
.IMGLogo {
  transition: color 0.3s ease;
}

/* Z-index utility */
.z-10 {
  z-index: 10;
  position: relative;
  font-size: 1.2rem;
}

/* Row styling */
.equal-height-row {
  align-items: stretch;
}

.equal-height-card {
  height: 100%;
  min-height: 350px;
}

/* Cursor pointer */
.cursor-pointer {
  cursor: pointer;
}

/* Drawer Styles */
.z-indexDialog {
  z-index: 2100 !important;
}

/* Responsive Design */
@media (max-width: 960px) {
  .text-Crm {
    font-size: 32px;
  }

  .equal-height-card {
    min-height: 300px;
  }
}

@media (max-width: 600px) {
  .text-Crm {
    font-size: 24px;
  }

  .onsite-container {
    padding: 15px;
  }

  .equal-height-card {
    min-height: 250px;
  }

  .IMGLogo .v-icon {
    font-size: 60px !important;
  }
}

/* Animation delay for bounceInUp effect */
.animate__animated.animate__bounceInUp:nth-child(1) {
  animation-delay: 0.1s;
}

.animate__animated.animate__bounceInUp:nth-child(2) {
  animation-delay: 0.2s;
}

.animate__animated.animate__bounceInUp:nth-child(3) {
  animation-delay: 0.3s;
}

/* Dialog Styles */
.dialog-card {
  border-radius: 16px !important;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.15) !important;
  overflow: hidden;
  border: none !important;
}

/* Remove default dialog border */
.v-dialog > .v-card {
  border: none !important;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.15) !important;
}

/* Remove all possible borders from dialog */
.v-dialog .v-card,
.v-dialog .v-sheet,
.v-overlay__content .v-card {
  border: none !important;
  outline: none !important;
}

/* Responsive Dialog */
@media (max-width: 768px) {
  .v-dialog {
    margin: 12px !important;
  }

  .dialog-card {
    border-radius: 12px !important;
  }
}

/* Fullscreen Dialog for Mobile */
@media (max-width: 599.98px) {
  .v-dialog--fullscreen {
    margin: 0 !important;
    max-height: 100vh !important;
    height: 100vh !important;
  }

  .v-dialog--fullscreen .dialog-card {
    border-radius: 0 !important;
    height: 100vh !important;
    max-height: 100vh !important;
    display: flex !important;
    flex-direction: column !important;
  }

  .v-dialog--fullscreen .v-card {
    height: 100vh !important;
    max-height: 100vh !important;
    overflow-y: auto !important;
  }
}

/* Drawer responsive */
@media (max-width: 767.98px) {
  .onsite-drawer:not(.v-navigation-drawer--active) {
    transform: translateX(100%) !important;
    visibility: hidden !important;
  }
}
</style>
