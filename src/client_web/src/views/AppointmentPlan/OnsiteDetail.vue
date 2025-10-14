<template>
  <div class="onsite-detail-container scroll-content dialog-scrollbar">
    <!-- Close Button (Fixed Top Right) -->
    <v-btn
      icon
      variant="text"
      size="small"
      @click="closeDrawer"
      class="close-btn"
    >
      <v-icon>ri-close-line</v-icon>
    </v-btn>

    <div
      v-if="selectedItem"
      class="content-wrapper"
    >
      <!-- Header Section like other pages -->
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
            <span class="text-sub-title text-center">รายละเอียดการลงเวลา</span>
          </v-col>
        </v-row>

        <v-card-text class="px-0 px-sm-3">
          <!-- Employee Information -->
          <v-card
            class="mb-4"
            elevation="0"
          >
            <v-card-text class="pa-4">
              <div class="d-flex align-center mb-5">
                <v-avatar
                  size="60"
                  color="primary"
                  variant="tonal"
                  class="mr-3"
                >
                  <template v-if="employeeImageProfile">
                    <v-img :src="employeeImageProfile" />
                  </template>
                  <template v-else>
                    {{ getInitials(employeeDisplayName) }}
                  </template>
                </v-avatar>
                <div>
                  <h3 class="text-h6 mb-1 ml-5">{{ employeeDisplayName }}</h3>
                  <div class="text-body-2 text-grey-darken-1 ml-5">{{ selectedItem.date }}</div>
                </div>
              </div>

              <!-- Status Badge -->
              <!-- <v-chip
                :color="getCheckInCheckOutTypeColor(selectedItem.checkInCheckOutTypes)"
                size="small"
                class="mb-2"
              >
                {{ formatCheckInCheckOutType(selectedItem.checkInCheckOutTypes) }}
              </v-chip> -->
            </v-card-text>
          </v-card>

          <!-- Time Information -->
          <v-card
            class="mb-4"
            elevation="0"
          >
            <v-card-title class="text-h5 pb-2">รายละเอียด</v-card-title>
            <v-card-text class="pa-4">
              <v-row>
                <v-col cols="6">
                  <div class="text-body-2 text-grey-darken-1 mb-1">ลงเวลา/เวลาจริง</div>
                  <div class="text-h6">{{ selectedItem.time + " / " + selectedItem.created }}</div>
                </v-col>
                <v-col cols="6">
                  <div class="text-body-2 text-grey-darken-1 mb-1">ประเภท</div>
                  <v-chip
                    :color="getCheckInCheckOutTypeColor(selectedItem.checkInCheckOutTypes)"
                    size="small"
                    variant="flat"
                  >
                    {{ formatCheckInCheckOutType(selectedItem.checkInCheckOutTypes) }}
                  </v-chip>
                </v-col>
              </v-row>

              <v-divider class="my-3"></v-divider>

              <v-row>
                <v-col cols="6">
                  <div class="text-body-2 text-grey-darken-1 mb-1">ชั่วโมงทำงาน</div>
                  <div
                    v-if="selectedItem.workingHours"
                    class="text-h6 font-weight-bold"
                  >
                    {{ selectedItem.workingHours }}
                  </div>
                  <div
                    v-else
                    class="text-h6 text-grey"
                  >
                    -
                  </div>
                </v-col>
                <v-col cols="6">
                  <div class="text-body-2 text-grey-darken-1 mb-1">ทำงานล่วงเวลา OT</div>
                  <div
                    v-if="selectedItem.overtime"
                    class="text-h6 text-warning"
                  >
                    {{ selectedItem.overtime }}
                  </div>
                  <div
                    v-else
                    class="text-h6 text-grey"
                  >
                    -
                  </div>
                </v-col>
              </v-row>
            </v-card-text>
          </v-card>

          <!-- Location Information -->
          <v-card
            class=""
            elevation="0"
          >
            <!-- <v-card-title class="text-h5 pb-2">สถานที่</v-card-title> -->
            <v-card-text class="pa-4">
              <v-col>
                <div class="align-start mb-3">
                  <div>
                    <v-icon
                      color="primary"
                      class="mr-2 mb-2"
                    >
                      ri-map-pin-line </v-icon
                    ><span class="text-body-1 font-weight-bold">สถานที่ {{ (selectedItem.lat ? "(พิกัด :"+ selectedItem.lat +","+selectedItem.long+")" : "") }}</span>
                  </div>
                  <div class="text-body-1 mt-2">{{ selectedItem.location }}</div>
                </div>
              </v-col>

              <!-- <v-col>
                <div class="align-start mb-3">
                  <div>
                    <v-icon
                      color="error"
                      class="mr-2 mb-2"
                    >
                      ri-map-pin-line </v-icon
                    ><span class="text-body-1 font-weight-bold">สถานที่ CheckOut</span>
                  </div>
                  <div class="text-body-1 mt-2">{{ selectedItem.locationCheckOut || '-' }}</div>
                </div>
              </v-col> -->
              <v-col>
                <!-- IP Address Information -->
                <div
                  v-if="selectedItem.ipAddress"
                  class="d-flex align-start"
                >
                  <v-icon
                    color="info"
                    class="mr-2 mb-2"
                  >
                    ri-global-line
                  </v-icon>
                  <div>
                    <div class="text-body-2 text-grey-darken-1 mb-1">IP Address</div>
                    <div class="text-body-1">{{ selectedItem.ipAddress }}</div>
                  </div>
                </div>
              </v-col>
            </v-card-text>
          </v-card>

          <!-- Photo Section -->
          <v-card
            class="mb-4"
            elevation="0"
          >
            <v-card-title class="text-h5 pb-2">รูปถ่าย</v-card-title>
            <v-card-text class="pa-4">
              <div
                v-if="hasPhotos"
                class="photos-grid"
              >
                <!-- Check-in Photo (แบบ OrganizationContactDetailView) -->
                <div
                  v-if="selectedItem?.checkinPhoto"
                  class="photo-item mb-4"
                >
                  <div class="photo-header d-flex align-center justify-space-between mb-2">
                    <v-chip
                      color="success"
                      size="small"
                      prepend-icon="ri-login-circle-line"
                      >รูปเข้างาน</v-chip
                    >
                  </div>
                  <div class="photo-container">
                    <v-img
                      :src="'data:image/png;base64,' + selectedItem?.checkinPhoto"
                      alt="รูปถ่ายเข้างาน"
                      class="photo-image rounded-lg"
                      cover
                      @click="openPhotoModal('data:image/png;base64,' + selectedItem?.checkinPhoto, 'checkin')"
                    >
                      <template #placeholder>
                        <div class="d-flex align-center justify-center fill-height">
                          <v-progress-circular
                            indeterminate
                            color="primary"
                          ></v-progress-circular>
                        </div>
                      </template>
                    </v-img>
                  </div>
                </div>

                <!-- Check-out Photo (แบบ OrganizationContactDetailView) -->
                <div
                  v-if="selectedItem?.checkoutPhoto"
                  class="photo-item mb-4"
                >
                  <div class="photo-header d-flex align-center justify-space-between mb-2">
                    <v-chip
                      color="error"
                      size="small"
                      prepend-icon="ri-logout-circle-line"
                      >รูปออกงาน</v-chip
                    >
                  </div>
                  <div class="photo-container">
                    <v-img
                      :src="'data:image/png;base64,' + selectedItem?.checkoutPhoto"
                      alt="รูปถ่ายออกงาน"
                      class="photo-image rounded-lg"
                      cover
                      @click="openPhotoModal('data:image/png;base64,' + selectedItem?.checkoutPhoto, 'checkout')"
                    >
                      <template #placeholder>
                        <div class="d-flex align-center justify-center fill-height">
                          <v-progress-circular
                            indeterminate
                            color="primary"
                          ></v-progress-circular>
                        </div>
                      </template>
                    </v-img>
                  </div>
                </div>

                <div class="text-caption text-center mt-2 text-grey-darken-1">คลิกเพื่อดูรูปขนาดใหญ่</div>
              </div>

              <div
                v-else
                class="no-photo-placeholder"
              >
                <v-icon
                  size="64"
                  color="grey-lighten-2"
                >
                  ri-image-line
                </v-icon>
                <div class="text-grey mt-2">ไม่มีรูปถ่าย</div>
              </div>
            </v-card-text>
          </v-card>
        </v-card-text>
      </v-card>

      <!-- Photo Modal -->
      <v-dialog
        v-model="photoModalVisible"
        max-width="90vw"
        max-height="90vh"
      >
        <v-card elevation="0">
          <v-card-title class="d-flex justify-space-between align-center">
            <div class="d-flex align-center">
              <v-chip
                v-if="currentPhotoType === 'checkin'"
                color="success"
                size="small"
                prepend-icon="ri-login-circle-line"
                class="mr-2"
              >
                รูปเข้างาน
              </v-chip>
              <v-chip
                v-else-if="currentPhotoType === 'checkout'"
                color="error"
                size="small"
                prepend-icon="ri-logout-circle-line"
                class="mr-2"
              >
                รูปออกงาน
              </v-chip>
              <v-chip
                v-else
                color="primary"
                size="small"
                prepend-icon="ri-camera-line"
                class="mr-2"
              >
                รูปถ่าย
              </v-chip>
              <span>{{ getPhotoModalTitle() }}</span>
            </div>
            <v-btn
              icon
              @click="photoModalVisible = false"
            >
              <v-icon>ri-close-line</v-icon>
            </v-btn>
          </v-card-title>
          <v-card-text class="pa-0">
            <v-img
              v-if="currentPhotoSrc"
              :src="currentPhotoSrc"
              :alt="getPhotoAlt()"
              contain
              class="modal-photo"
            />
          </v-card-text>
        </v-card>
      </v-dialog>
    </div>
  </div>
</template>

<script lang="ts">
import { CheckInCheckOutType } from '@/client'
import { usePhotoCaptureStore } from '@/stores/photoCaptureStore'
import { defineComponent } from 'vue'

export default defineComponent({
  name: 'OnsiteDetail',
  props: {
    item: {
      type: Object as () => any,
      default: null,
    },
  },
  emits: ['close'],
  setup() {
    const photoCaptureStore = usePhotoCaptureStore()
    console.log('OnsiteDetail: photoCaptureStore in setup()', photoCaptureStore)
    return { photoCaptureStore }
  },
  data() {
    return {
      photoModalVisible: false,
      currentPhotoSrc: '',
      currentPhotoType: 'general' as 'checkin' | 'checkout' | 'general',
    }
  },
  computed: {
    selectedItem() {
      console.log('selectedItem data:', this.item)
      if (this.item) {
        console.log('LocationCheckOut:', this.item.locationCheckOut)
      }
      return this.item
    },
    capturedCheckinImage() {
      return this.photoCaptureStore.checkinImage
    },
    capturedCheckoutImage() {
      return this.photoCaptureStore.checkoutImage
    },
    hasPhotos() {
      return !!(
        this.selectedItem?.photo ||
        this.selectedItem?.checkinPhoto ||
        this.selectedItem?.checkoutPhoto ||
        this.capturedCheckinImage ||
        this.capturedCheckoutImage
      )
    },
    employeeDisplayName() {
      if (this.selectedItem?.employees) {
        const emp = this.selectedItem.employees
        const titleName = emp.titleName || ''
        const firstName = emp.firstName || ''
        const lastName = emp.lastName || ''
        return `${titleName} ${firstName} ${lastName}`.trim()
      }
      return this.selectedItem?.employeeName || 'ไม่ระบุชื่อ'
    },
    employeeImageProfile() {
      if (this.selectedItem?.employees?.imageProfile) {
        return this.selectedItem.employees.imageProfile
      }
      return this.selectedItem?.imageProfile || null
    },
  },
  methods: {
    closeDrawer() {
      this.photoCaptureStore.clearCapturedPhoto()
      this.$emit('close')
    },
    openPhotoModal(photoSrc: string, photoType: 'checkin' | 'checkout' | 'general') {
      this.currentPhotoSrc = photoSrc
      this.currentPhotoType = photoType
      this.photoModalVisible = true
    },
    getPhotoModalTitle() {
      const employeeName = this.employeeDisplayName
      if (this.currentPhotoType === 'checkin') {
        return `รูปเข้างาน - ${employeeName}`
      } else if (this.currentPhotoType === 'checkout') {
        return `รูปออกงาน - ${employeeName}`
      }
      return `รูปถ่ายการลงเวลา - ${employeeName}`
    },
    getPhotoAlt() {
      if (this.currentPhotoType === 'checkin') {
        return 'รูปถ่ายเข้างาน'
      } else if (this.currentPhotoType === 'checkout') {
        return 'รูปถ่ายออกงาน'
      }
      return 'รูปถ่ายการลงเวลา'
    },
    getInitials(employeeName: string) {
      if (!employeeName) return 'UN'
      const names = employeeName.trim().split(' ')
      if (names.length === 1) {
        return names[0].charAt(0).toUpperCase()
      }
      return (names[0].charAt(0) + names[names.length - 1].charAt(0)).toUpperCase()
    },
    formatCheckInCheckOutType(type: CheckInCheckOutType | undefined): string {
      if (type === CheckInCheckOutType.CheckIn) {
        return 'เข้างาน'
      } else if (type === CheckInCheckOutType.CheckOut) {
        return 'ออกงาน'
      } else {
        return '-'
      }
    },
    getCheckInCheckOutTypeColor(type: CheckInCheckOutType | undefined): string {
      if (type === CheckInCheckOutType.CheckIn) {
        return 'success'
      } else if (type === CheckInCheckOutType.CheckOut) {
        return 'error'
      } else {
        return 'grey'
      }
    },
  },
})
</script>

<style scoped>
/* Main container for scrolling - matching CreateCustomerAppointmentPlan exactly */
.onsite-detail-container {
  width: 100%;
  min-height: 100vh;
  max-height: 100vh;
  overflow-y: auto;
  overflow-x: hidden;
  padding: 16px 0 32px 0;
  scroll-behavior: smooth;
  position: relative;
  /* Custom scrollbar for better visibility */
  scrollbar-width: thin;
  scrollbar-color: #4a6cf7 #e0e0e0;
}

.onsite-detail-container::-webkit-scrollbar {
  width: 8px;
}
.onsite-detail-container::-webkit-scrollbar-thumb {
  background: #4a6cf7;
  border-radius: 8px;
}
.onsite-detail-container::-webkit-scrollbar-track {
  background: #e0e0e0;
  border-radius: 8px;
}

/* Header Title - matching CreateActivityDetail */
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
  text-align: center;
  margin: 0;
  padding: 0;
  line-height: 1.5;
}

.scroll-content {
  overflow-y: auto;
  overflow-x: hidden;
  flex: 1;
  -webkit-overflow-scrolling: touch;
  overscroll-behavior: contain;
}

/* Basic container to ensure content flows properly */
/* .v-card {
  margin-bottom: 16px;
} */

/* .card-form {
  border: 1px solid rgba(0, 0, 0, 0.05);
} */

/* Photo Styles */
.photo-container {
  max-width: 100%;
}

.photo-image {
  max-height: 300px;
  cursor: pointer;
  transition: transform 0.2s ease;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.photo-image:hover {
  transform: scale(1.02);
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.15);
}

.photo-header {
  margin-bottom: 12px;
}

.photo-item {
  background: white;
  border-radius: 12px;
  padding: 16px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.06);
  transition: all 0.2s ease;
}

.photo-item:hover {
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.1);
  transform: translateY(-1px);
}

.no-photo-placeholder {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 200px;
  background-color: #f8f9fa;
  border-radius: 12px;
  border: 2px dashed #e0e0e0;
}

/* Close Button - Fixed top right */
.close-btn {
  position: fixed !important;
  top: 16px !important;
  right: 16px !important;
  z-index: 2010 !important;
  background: rgba(255, 255, 255, 0.9) !important;
  backdrop-filter: blur(10px);
  border: 1px solid rgba(43, 48, 134, 0.1) !important;
  color: #2b3086 !important;
  transition: all 0.2s ease;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.close-btn:hover {
  background: rgba(43, 48, 134, 0.1) !important;
  transform: scale(1.05);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

/* Photo Styles */
.photo-container {
  max-width: 100%;
}

.photo-image {
  max-height: 300px;
  cursor: pointer;
  transition: transform 0.2s ease;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.photo-image:hover {
  transform: scale(1.02);
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.15);
}

.photo-header {
  margin-bottom: 12px;
}

.photo-item {
  background: white;
  border-radius: 12px;
  padding: 16px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.06);
  transition: all 0.2s ease;
}

.photo-item:hover {
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.1);
  transform: translateY(-1px);
}

.no-photo-placeholder {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 200px;
  background-color: #f8f9fa;
  border-radius: 12px;
  border: 2px dashed #e0e0e0;
}

.v-card-title {
  font-size: 16px;
  font-weight: 600;
  color: #2b3086;
  padding-bottom: 8px;
}

/* Modal Styles */
.modal-photo {
  max-height: 80vh;
}

.modal-photo:hover {
  transform: none !important;
  box-shadow: none !important;
}

/* Responsive Design */
@media (max-width: 599px) {
  .text-sub-title {
    font-size: 20px;
  }

  .close-btn {
    top: 12px !important;
    right: 12px !important;
  }
  .photo-item {
    margin-bottom: 20%;
  }
}
</style>
