<template>
  <div class="onsite-form-container custom-scrollbar">
    <!-- Close Button (Fixed Top Right) -->
    <v-btn
      icon
      variant="text"
      size="small"
      @click="goBack"
      class="close-btn"
    >
      <v-icon>ri-close-line</v-icon>
    </v-btn>

    <!-- Card Title like CreateActivityDetail -->
    <v-row class="mt-2 mt-sm-3 mb-3 mb-sm-4">
      <v-col
        cols="12"
        class="d-flex justify-center"
      >
        <div class="d-flex flex-column align-center">
          <span class="text-sub-title text-center">{{ headerTitle }}</span>
        </div>
      </v-col>
    </v-row>

    <!-- Check-in/Check-out Content -->
    <v-container
      fluid
      class="checkin-checkout-content pa-0"
    >
      <v-form
        ref="form"
        @submit.prevent="handleSlideButtonClick"
      >
        <v-row class="ma-0">
          <v-col
            cols="12"
            class="pa-4"
          >
            <!-- Camera Section -->
            <v-row class="ma-0 mb-4">
              <v-col
                cols="12"
                class="pa-0"
              >
                <div class="camera-container">
                  <!-- Live Camera Video -->
                  <video
                    ref="video"
                    autoplay
                    playsinline
                    muted
                    class="camera-video"
                    v-show="cameraSupported && cameraActive && !showPhotoPreview"
                  ></video>

                  <!-- Photo Preview -->
                  <img
                    v-if="showPhotoPreview && capturedPhotoData"
                    :src="capturedPhotoData"
                    class="photo-preview"
                    alt="Captured Photo"
                  />

                  <!-- Camera Placeholder -->
                  <div
                    v-show="!cameraSupported || (!cameraActive && !showPhotoPreview)"
                    class="camera-placeholder"
                  >
                    <v-icon
                      size="100"
                      color="grey-lighten-2"
                      >ri-camera-off-line</v-icon
                    >
                    <p class="text-grey">{{ cameraError || 'กล้องไม่พร้อมใช้งาน' }}</p>
                    <v-btn
                      v-if="cameraSupported && !cameraActive"
                      color="primary"
                      @click="startCamera"
                      class="mt-3"
                    >
                      เปิดกล้อง
                    </v-btn>
                  </div>

                  <!-- Face detection status indicator (top right of camera) -->
                  <div
                    class="face-status-indicator"
                    v-if="cameraActive && !showPhotoPreview"
                  >
                    <div
                      class="status-badge"
                      :class="{
                        'status-detected': faceApiLoaded && detectedFace,
                        'status-not-detected': faceApiLoaded && !detectedFace,
                        'status-loading': !faceApiLoaded,
                      }"
                    >
                      <v-icon
                        size="16"
                        :color="getStatusIconColor()"
                      >
                        {{ getStatusIcon() }}
                      </v-icon>
                      <span class="status-text">{{ getStatusText() }}</span>
                    </div>
                  </div>

                  <!-- Photo Preview Controls -->
                  <div
                    v-if="showPhotoPreview"
                    class="photo-preview-controls"
                  >
                    <v-btn
                      color="primary"
                      variant="elevated"
                      @click="retakePhoto"
                      class="retake-btn"
                    >
                      <v-icon class="mr-2">ri-camera-line</v-icon>
                      ถ่ายใหม่
                    </v-btn>
                  </div>
                </div>
              </v-col>
            </v-row>

            <!-- Single Slide Button -->
            <v-row
              v-if="currentMode === 'checkin'"
              class="ma-0 mb-4 action-buttons-group"
              justify="center"
            >
              <v-col
                cols="12"
                sm="6"
                class="d-flex justify-center pa-1"
              >
                <div class="slide-button-container">
                  <!-- Sliding Button -->
                  <label class="mb-2">หน่วยงาน / ลูกค้า </label>
                  <v-autocomplete
                    placeholder="กรุณาเลือกหน่วยงานหรือลูกค้า"
                    :items="OrganizationList"
                    item-title="name"
                    item-value="id"
                    clearable
                    v-model="createCommand.organizationId"
                  >
                  </v-autocomplete>
                </div>
              </v-col>
            </v-row>
            <v-row
              class="ma-0 mb-4 action-buttons-group"
              justify="center"
            >
              <v-col
                cols="12"
                sm="6"
                class="d-flex justify-center pa-1"
              >
                <div class="slide-button-container">
                  <!-- Sliding Button -->
                  <v-btn
                    block
                    class="slide-btn onsite-action-btn-text"
                    :class="getSlideButtonClass()"
                    :color="getSlideButtonColor()"
                    variant="elevated"
                    size="x-large"
                    type="submit"
                    :disabled="getSlideButtonDisabled()"
                    :loading="isLoading"
                  >
                    <v-icon class="mr-2">{{ getSlideButtonIcon() }}</v-icon>
                    {{ getSlideButtonText() }}
                    <v-icon
                      v-if="getSlideButtonTrailingIcon()"
                      class="ml-2"
                      >{{ getSlideButtonTrailingIcon() }}</v-icon
                    >
                  </v-btn>
                </div>
              </v-col>
            </v-row>

            <!-- Date and Time Info / IP Address -->
            <v-row class="ma-0 mb-3 mt-10">
              <!-- Date and Time Info -->
              <v-col
                cols="12"
                md="6"
                class="pa-1 d-flex"
              >
                <v-card class="datetime-info flex-grow-1">
                  <v-card-text class="pa-4">
                    <v-row class="ma-0">
                      <v-col
                        cols="6"
                        class="text-center pa-1"
                      >
                        <div class="datetime-label">วันที่</div>
                        <div class="datetime-value">{{ currentDate }}</div>
                      </v-col>
                      <v-col
                        cols="6"
                        class="text-center pa-1"
                      >
                        <div class="datetime-label">เวลา</div>
                        <div class="datetime-value">{{ currentTime }}</div>
                      </v-col>
                    </v-row>
                  </v-card-text>
                </v-card>
              </v-col>

              <!-- Location Info -->
              <v-col
                cols="12"
                md="6"
                class="pa-1 d-flex"
              >
                <v-card class="location-info flex-grow-1">
                  <v-card-text class="pa-4">
                    <v-row class="ma-0">
                      <v-col
                        cols="12"
                        class="text-center pa-0"
                      >
                        <div class="location-label">IP Address</div>
                        <div class="location-value">{{ verificationType }}</div>
                      </v-col>
                    </v-row>
                  </v-card-text>
                </v-card>
              </v-col>
            </v-row>

            <!-- Work Status Info -->
            <v-row
              v-if="workStatus !== 'idle'"
              class="ma-0 mb-3"
            >
              <v-col
                cols="12"
                class="pa-0"
              >
                <v-card class="work-status-info">
                  <v-card-text class="pa-4">
                    <v-row
                      v-if="checkInTime"
                      class="ma-0 mb-2"
                    >
                      <v-col
                        cols="12"
                        class="d-flex justify-space-between align-center pa-0"
                      >
                        <span class="work-status-label">เวลาเข้างาน</span>
                        <span class="work-status-value text-success">{{ checkInTime }}</span>
                      </v-col>
                    </v-row>
                    <v-row
                      v-if="checkOutTime"
                      class="ma-0"
                    >
                      <v-col
                        cols="12"
                        class="d-flex justify-space-between align-center pa-0"
                      >
                        <span class="work-status-label">เวลาออกงาน</span>
                        <span class="work-status-value text-error">{{ checkOutTime }}</span>
                      </v-col>
                    </v-row>
                  </v-card-text>
                </v-card>
              </v-col>
            </v-row>

            <!-- Location -->
            <v-row
              v-if="location"
              class="ma-0"
            >
              <v-col
                cols="12"
                class="pa-0"
              >
                <v-card class="coordinates-info">
                  <v-card-text class="pa-4">
                    <h4 class="coordinates-title mb-3">ข้อมูลตำแหน่ง</h4>
                    <p class="coordinates-text mb-2">สถานที่: {{ location.address || 'กำลังโหลดชื่อสถานที่...' }}</p>
                    <p class="distance-text mb-0">ความแม่นยำ: {{ accuracy }} เมตร</p>
                  </v-card-text>
                </v-card>
              </v-col>
            </v-row>
          </v-col>
        </v-row>
      </v-form>
    </v-container>

    <!-- Check-in Confirmation Dialog -->
    <v-dialog
      v-model="checkinDialogVisible"
      max-width="500"
      @keydown.esc="checkinDialogVisible = false"
    >
      <v-card class="dialog-card">
        <v-card-title class="text-center pa-6 position-relative">
          <v-btn
            icon
            variant="text"
            size="small"
            @click="checkinDialogVisible = false"
            class="dialog-close-btn"
            :disabled="isLoading"
          >
            <v-icon>ri-close-line</v-icon>
          </v-btn>
          <v-icon
            color="success"
            size="48"
            class="mb-3"
          >
            ri-login-circle-line
          </v-icon>
          <div class="dialog-title">ยืนยันการบันทึกเวลาเข้างาน</div>
        </v-card-title>

        <v-card-text class="text-center pa-6">
          <div class="confirmation-details">
            <div class="detail-item">
              <span class="detail-label">วันที่:</span>
              <span class="detail-value">{{ currentDate }}</span>
            </div>
            <div class="detail-item">
              <span class="detail-label">เวลา:</span>
              <span class="detail-value">{{ currentTime }}</span>
            </div>
            <div class="detail-item">
              <span class="detail-label">IP Address:</span>
              <span class="detail-value">{{ userIPAddress }}</span>
            </div>
            <div
              class="detail-item"
              v-if="location && location.address"
            >
              <span class="detail-label">สถานที่:</span>
              <v-tooltip
                location="top"
                :content-class="'bg-secondary text-white pa-2 rounded'"
              >
                <template #activator="{ props }">
                  <span
                    v-bind="props"
                    class="detail-value location-text"
                  >
                    {{ location.address }}
                  </span>
                </template>
                <span>{{ location.address }}</span>
              </v-tooltip>
            </div>
          </div>
          <div class="confirmation-message mt-4">คุณต้องการบันทึกเวลาเข้างานใช่หรือไม่?</div>
        </v-card-text>

        <v-card-actions class="pa-6 pt-0">
          <v-btn
            color="success"
            variant="elevated"
            block
            @click="confirmCheckIn"
            :loading="isLoading"
            class="ml-3"
          >
            ยืนยันเข้างาน
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Check-out Confirmation Dialog -->
    <v-dialog
      v-model="checkoutDialogVisible"
      max-width="500"
      @keydown.esc="checkoutDialogVisible = false"
    >
      <v-card class="dialog-card">
        <v-card-title class="text-center pa-6 position-relative">
          <v-btn
            icon
            variant="text"
            size="small"
            @click="checkoutDialogVisible = false"
            class="dialog-close-btn"
            :disabled="isLoading"
          >
            <v-icon>ri-close-line</v-icon>
          </v-btn>
          <v-icon
            color="error"
            size="48"
            class="mb-3"
          >
            ri-logout-circle-line
          </v-icon>
          <div class="dialog-title">ยืนยันการบันทึกเวลาออกงาน</div>
        </v-card-title>

        <v-card-text class="text-center pa-6">
          <div class="confirmation-details">
            <div class="detail-item">
              <span class="detail-label">วันที่:</span>
              <span class="detail-value">{{ currentDate }}</span>
            </div>
            <div class="detail-item">
              <span class="detail-label">เวลา:</span>
              <span class="detail-value">{{ currentTime }}</span>
            </div>
            <div class="detail-item">
              <span class="detail-label">เวลาเข้างาน:</span>
              <span class="detail-value text-success">{{ checkInTime }}</span>
            </div>
            <div
              class="detail-item"
              v-if="location && location.address"
            >
              <span class="detail-label">สถานที่:</span>
              <v-tooltip
                location="top"
                :content-class="'bg-secondary text-white pa-2 rounded'"
              >
                <template #activator="{ props }">
                  <span
                    v-bind="props"
                    class="detail-value location-text"
                  >
                    {{ location.address }}
                  </span>
                </template>
                <span>{{ location.address }}</span>
              </v-tooltip>
            </div>
          </div>
          <div class="confirmation-message mt-4">คุณต้องการบันทึกเวลาออกงานใช่หรือไม่?</div>
        </v-card-text>

        <v-card-actions class="pa-6 pt-0">
          <v-btn
            color="error"
            variant="elevated"
            block
            @click="confirmCheckOut"
            :loading="isLoading"
            class="ml-3"
          >
            ยืนยันออกงาน
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<script lang="ts">
import { Client, CreateCheckInCheckOutCommand } from '@/client'
import { BACKEND_API_URL } from '@/constants'
import { useAuthStore, useSweetAlertStore } from '@/stores'
import { usePhotoCaptureStore } from '@/stores/photoCaptureStore'
import { selectAgencyOrCustomerRules } from '@/utils/RuleServices'
import * as faceapi from 'face-api.js'
import { defineComponent } from 'vue'

const client = new Client(BACKEND_API_URL)

export default defineComponent({
  name: 'OnsiteForm',
  props: {
    mode: { type: String, default: 'checkin' },
  },
  emits: ['close'],

  computed: {
    currentMode() {
      const routeMode = this.$route.params.mode
      const mode = this.mode || (Array.isArray(routeMode) ? routeMode[0] : routeMode) || 'checkin'
      return mode
    },
    headerTitle() {
      const titles: Record<string, string> = {
        checkin: 'บันทึกเวลาเข้างาน',
        checkout: 'บันทึกเวลาออกงาน',
      }
      return titles[this.currentMode] || 'ลงเวลาเข้า-ออกงาน'
    },
    isCheckinDisabled() {
      return ['checked-in', 'checked-out'].includes(this.workStatus)
    },
  },

  data() {
    return {
      // Stores
      sweetAlert: useSweetAlertStore(),
      authStore: useAuthStore(),
      photoCaptureStore: usePhotoCaptureStore(),

      // Employee & Organization
      employeeData: null as any,
      OrganizationList: [] as any,
      createCommand: new CreateCheckInCheckOutCommand(),
      selectAgencyOrCustomerRules,

      // Loading state
      isLoading: false,

      // Camera
      cameraSupported: false,
      cameraActive: false,
      cameraError: '',
      stream: null as any,
      faceApiLoaded: false,
      detectedFace: null as any,
      detectionRunning: false,
      detectionInterval: null as NodeJS.Timeout | null,

      // Photo
      photoData: null as any,
      showPhotoPreview: false,
      capturedPhotoData: null as string | null,

      // Location
      location: null as any,
      locationError: '',
      accuracy: 83204,

      // Time & IP
      currentTime: '',
      currentDate: '',
      verificationType: 'ตรวจสอบ IP Address',
      userIPAddress: '',
      timeInterval: null as NodeJS.Timeout | null,

      // Dialogs
      checkinDialogVisible: false,
      checkoutDialogVisible: false,

      // Work status
      workStatus: 'idle' as 'idle' | 'checked-in' | 'checked-out',
      checkInTime: null as string | null,
      checkOutTime: null as string | null,
    }
  },
  async mounted() {
    await this.initialize()
  },

  beforeUnmount() {
    this.cleanup()
  },

  methods: {
    // === INITIALIZATION ===
    async initialize() {
      try {
        // Load face-api in background - don't block camera setup
        this.loadFaceAPI().catch(error => {
          console.log('Face-api loading failed, continuing without face detection:', error)
        })

        await this.loadOrganizations()
        await this.loadEmployeeData()

        // รอให้ employee data โหลดเสร็จก่อนโหลด work status
        if (this.employeeData?.id) {
          await this.loadCurrentWorkStatus()
        }

        this.setupCamera()
        this.getLocation()
        this.getUserIPAddress()

        if (this.cameraSupported) {
          await this.startCamera()
        }

        this.startTimeUpdates()
      } catch (error) {
        console.error('Initialization error:', error)
        this.sweetAlert.error('เกิดข้อผิดพลาดในการเริ่มต้นระบบ')
      }
    },

    async loadOrganizations() {
      this.OrganizationList = await client.getOrganizationQuery()
    },

    async loadEmployeeData() {
      if (!this.authStore.userId) return

      try {
        this.employeeData = await client.getEmployeeQueryByUserID(this.authStore.userId)
        console.log('Employee data loaded:', this.employeeData)
      } catch (error) {
        console.error('Failed to load employee data:', error)
      }
    },

    setupCamera() {
      this.cameraSupported = !!(navigator.mediaDevices && navigator.mediaDevices.getUserMedia)
    },

    startTimeUpdates() {
      this.updateTime()
      this.timeInterval = setInterval(this.updateTime, 1000)
    },

    cleanup() {
      this.stopCamera()
      this.stopFaceDetection()
      if (this.timeInterval) {
        clearInterval(this.timeInterval)
      }
    },

    // === FACE API METHODS ===
    async loadFaceAPI() {
      try {
        console.log('Loading face-api.js models...')

        // Load face detection models from CDN with fallback
        const MODEL_URL = '/models'
        const CDN_URL = 'https://raw.githubusercontent.com/justadudewhohacks/face-api.js/master/weights'

        try {
          await faceapi.nets.tinyFaceDetector.loadFromUri(MODEL_URL)
        } catch {
          await faceapi.nets.tinyFaceDetector.loadFromUri(CDN_URL)
        }

        this.faceApiLoaded = true
        console.log('Face-api.js models loaded successfully')
      } catch (error) {
        console.error('Failed to load face-api.js models:', error)
        console.log('Face detection will be disabled, but camera should still work')
        this.faceApiLoaded = false
      }
    },

    startFaceDetection() {
      if (!this.faceApiLoaded || this.detectionRunning) return

      this.detectionRunning = true
      this.detectionInterval = setInterval(async () => {
        await this.detectFace()
      }, 100) // Run detection every 100ms
    },

    stopFaceDetection() {
      this.detectionRunning = false
      if (this.detectionInterval) {
        clearInterval(this.detectionInterval)
        this.detectionInterval = null
      }
      this.detectedFace = null
    },

    async detectFace() {
      if (!this.faceApiLoaded || !this.cameraActive) return

      const video = this.$refs.video as HTMLVideoElement
      if (!video || video.videoWidth === 0 || video.videoHeight === 0) return

      try {
        const detection = await faceapi.detectSingleFace(video, new faceapi.TinyFaceDetectorOptions({ inputSize: 320 }))

        if (detection) {
          // Scale detection box to video display size
          const displaySize = { width: video.offsetWidth, height: video.offsetHeight }
          const resizedDetection = faceapi.resizeResults(detection, displaySize)

          this.detectedFace = {
            box: resizedDetection.box,
            score: detection.score,
          }
        } else {
          this.detectedFace = null
        }
      } catch (error) {
        console.error('Face detection error:', error)
        this.detectedFace = null
      }
    },

    // === WORK STATUS ===
    async loadCurrentWorkStatus() {
      if (!this.employeeData?.id) {
        console.log('No employee data available for loading work status')
        return
      }

      try {
        console.log('Loading work status for employee:', this.employeeData.id)
        const records = await client.getCheckInCheckOutQueryByEmployeeId(this.employeeData.id)
        console.log('All records:', records)

        const todaysRecords = this.getTodaysRecords(records)
        console.log('Today records:', todaysRecords)

        const checkinRecord = todaysRecords.find((r: any) => r.checkIn && !r.checkOut)
        const checkoutRecord = todaysRecords.find((r: any) => r.checkOut)

        console.log('Check-in record:', checkinRecord)
        console.log('Check-out record:', checkoutRecord)

        this.updateWorkStatus(checkinRecord, checkoutRecord)
        console.log('Updated work status:', this.workStatus)
      } catch (error) {
        console.error('Failed to load current work status from API:', error)
      }
    },

    getTodaysRecords(records: any[]) {
      const today = new Date()
      today.setHours(0, 0, 0, 0)

      return records.filter((record: any) => {
        const checkDate = record.checkIn || record.checkOut
        if (!checkDate) return false
        const recordDate = new Date(checkDate)
        recordDate.setHours(0, 0, 0, 0)
        return recordDate.getTime() === today.getTime()
      })
    },

    updateWorkStatus(checkinRecord: any, checkoutRecord: any) {
      if (checkinRecord && checkoutRecord) {
        this.workStatus = 'checked-out'
        this.checkInTime = this.formatTime(checkinRecord.checkIn)
        this.checkOutTime = this.formatTime(checkoutRecord.checkOut)
      } else if (checkinRecord) {
        this.workStatus = 'checked-in'
        this.checkInTime = this.formatTime(checkinRecord.checkIn)
        this.checkOutTime = null
      } else {
        this.workStatus = 'idle'
        this.checkInTime = null
        this.checkOutTime = null
      }
    },

    formatTime(date: Date | undefined): string {
      if (!date) return '-'
      return new Date(date).toLocaleTimeString('th-TH', {
        hour: '2-digit',
        minute: '2-digit',
        second: '2-digit',
      })
    },

    updateTime() {
      const now = new Date()
      const timeOptions = { hour: '2-digit', minute: '2-digit', second: '2-digit' } as const
      const dateOptions = { day: 'numeric', month: 'long', year: 'numeric' } as const

      this.currentTime = now.toLocaleTimeString('th-TH', timeOptions)
      this.currentDate = now.toLocaleDateString('th-TH', dateOptions)
    },
    // === CAMERA METHODS ===
    async startCamera() {
      this.cameraError = ''

      if (!this.isCameraSupported()) {
        this.cameraSupported = false
        console.log('Camera not supported by browser')
        return
      }

      try {
        this.stream = await this.getCameraStream()
        this.setupVideoElement()
        this.cameraActive = true
        console.log('Camera started successfully')
      } catch (err) {
        this.handleCameraError(err)
      }
    },

    isCameraSupported() {
      return !!(navigator.mediaDevices && navigator.mediaDevices.getUserMedia)
    },

    getCameraStream() {
      return navigator.mediaDevices.getUserMedia({
        video: {
          facingMode: 'user',
          width: { ideal: 640 },
          height: { ideal: 480 },
        },
      })
    },

    setupVideoElement() {
      this.$nextTick(() => {
        const video = this.$refs.video as HTMLVideoElement
        if (video) {
          video.srcObject = this.stream
          video.onloadedmetadata = () => {
            video
              .play()
              .then(() => {
                if (this.faceApiLoaded) {
                  this.startFaceDetection()
                }
              })
              .catch(e => console.error('Error playing video:', e))
          }
        }
      })
    },

    handleCameraError(err: any) {
      console.error('Camera error:', err)
      this.cameraError = 'Camera access denied or not available.'
      this.cameraSupported = false
      this.sweetAlert.error('ไม่สามารถเข้าถึงกล้องได้: ' + (err as Error).message)
    },

    stopCamera() {
      this.stopFaceDetection()
      if (this.stream) {
        this.stream.getTracks().forEach((track: MediaStreamTrack) => track.stop())
        this.stream = null
      }
      this.cameraActive = false
    },
    // === LOCATION & IP ===
    async getLocation() {
      this.locationError = ''

      if (!navigator.geolocation) {
        this.locationError = 'Geolocation is not supported by your browser.'
        return
      }

      const options = { enableHighAccuracy: true }

      navigator.geolocation.getCurrentPosition(
        async pos => await this.handleLocationSuccess(pos),
        err => this.handleLocationError(err),
        options,
      )
    },

    async handleLocationSuccess(pos: GeolocationPosition) {
      this.location = {
        latitude: pos.coords.latitude.toFixed(6),
        longitude: pos.coords.longitude.toFixed(6),
      }
      this.accuracy = Math.round(pos.coords.accuracy)
      console.log(this.location)

      await this.getAddressFromCoordinates(pos.coords.latitude, pos.coords.longitude)
    },

    async getAddressFromCoordinates(lat: number, lon: number) {
      try {
        const url = `https://nominatim.openstreetmap.org/reverse?format=jsonv2&lat=${lat}&lon=${lon}&accept-language=th`
        const response = await fetch(url)
        const data = await response.json()

        this.location.address = data?.display_name || 'ไม่พบข้อมูลสถานที่'
      } catch (error) {
        this.location.address = 'เกิดข้อผิดพลาดในการเรียกข้อมูลสถานที่'
      }
    },

    handleLocationError(err: GeolocationPositionError) {
      this.locationError = 'Failed to get location: ' + err.message
    },

    async getUserIPAddress() {
      try {
        const response = await fetch('https://api.ipify.org?format=json')
        const data = await response.json()
        this.userIPAddress = data.ip
        this.verificationType = data.ip
      } catch (error) {
        console.error('Failed to get IP address:', error)
        this.userIPAddress = 'ไม่สามารถดึงข้อมูลได้'
        this.verificationType = 'ไม่สามารถตรวจสอบ IP ได้'
      }
    },
    // === PHOTO CAPTURE ===
    async manualCapturePhoto() {
      if (!this.cameraActive) {
        this.sweetAlert.error('กรุณาเปิดกล้องก่อนถ่ายรูป')
        return
      }

      // Check if face is detected when face-api is loaded
      if (this.faceApiLoaded && !this.detectedFace) {
        this.sweetAlert.error('กรุณาจัดตำแหน่งใบหน้าให้อยู่ในกรอบ')
        return
      }

      try {
        this.isLoading = true
        const type = this.currentMode === 'checkin' ? 'checkin' : 'checkout'

        await this.capturePhoto(type)
        this.sweetAlert.success('ถ่ายรูปสำเร็จ!')
      } catch (error) {
        console.error('Error capturing photo:', error)
        const errorMessage = error instanceof Error ? error.message : 'เกิดข้อผิดพลาดในการถ่ายรูป กรุณาลองใหม่อีกครั้ง'
        this.sweetAlert.error(errorMessage)
      } finally {
        this.isLoading = false
      }
    },

    async capturePhoto(type: 'checkin' | 'checkout') {
      return new Promise<void>((resolve, reject) => {
        try {
          const video = this.$refs.video as HTMLVideoElement

          if (!this.validateVideoElement(video)) {
            reject(new Error('ไม่พบกล้อง กรุณาเปิดกล้องก่อน'))
            return
          }

          const canvas = this.createCanvasFromVideo(video)
          if (!canvas) {
            reject(new Error('เกิดข้อผิดพลาดในการถ่ายรูป'))
            return
          }

          this.photoData = canvas.toDataURL('image/png')
          if (!this.photoData) {
            reject(new Error('ไม่สามารถถ่ายรูปได้'))
            return
          }

          this.storePhoto(type)
          resolve()
        } catch (error) {
          console.error('Error in capturePhoto:', error)
          reject(error)
        }
      })
    },

    validateVideoElement(video: HTMLVideoElement) {
      if (!video) {
        console.error('Video element not found for capture.')
        return false
      }
      if (!video.videoWidth || !video.videoHeight) {
        console.error('Video not ready for capture.')
        return false
      }
      return true
    },

    createCanvasFromVideo(video: HTMLVideoElement) {
      const canvas = document.createElement('canvas')
      canvas.width = video.videoWidth
      canvas.height = video.videoHeight
      const ctx = canvas.getContext('2d')

      if (!ctx) {
        console.error('Cannot get canvas context.')
        return null
      }

      // Flip horizontally to match mirrored camera view
      ctx.scale(-1, 1)
      ctx.translate(-canvas.width, 0)
      ctx.drawImage(video, 0, 0, canvas.width, canvas.height)

      return canvas
    },

    storePhoto(type: 'checkin' | 'checkout') {
      this.photoCaptureStore.setCapturedPhoto(this.photoData, type)
      this.capturedPhotoData = this.photoData
      this.showPhotoPreview = true
    },

    retakePhoto() {
      this.showPhotoPreview = false
      this.capturedPhotoData = null
      this.photoData = null
      this.photoCaptureStore.clearCapturedPhoto()
    },

    showPhotoRequiredError(mode: string) {
      const action = mode === 'checkin' ? 'เข้างาน' : 'ออกงาน'
      this.sweetAlert.error(`กรุณาถ่ายรูปก่อน${action}`)
    },

    showDuplicateWorkTimeError(mode: string) {
      const action = mode === 'checkin' ? 'เข้างาน' : 'ออกงาน'
      this.sweetAlert.info(`คุณได้ลงเวลา${action}วันนี้แล้ว`, `ไม่สามารถลงเวลา${action}ซ้ำได้`)
    },
    // === DIALOG METHODS ===
    openCheckinDialog() {
      if (!this.showPhotoPreview) {
        this.showPhotoRequiredError('checkin')
        return
      }
      this.checkinDialogVisible = true
    },

    openCheckoutDialog() {
      if (!this.showPhotoPreview) {
        this.showPhotoRequiredError('checkout')
        return
      }
      this.checkoutDialogVisible = true
    },

    async confirmCheckIn() {
      try {
        this.isLoading = true
        await this.checkIn()
        this.checkinDialogVisible = false
      } catch (error) {
        console.error('Check-in confirmation error:', error)
      } finally {
        this.isLoading = false
      }
    },

    async confirmCheckOut() {
      try {
        this.isLoading = true
        await this.checkOut()
        this.checkoutDialogVisible = false
      } catch (error) {
        console.error('Check-out confirmation error:', error)
      } finally {
        this.isLoading = false
      }
    },

    // === CHECK IN/OUT METHODS ===
    async checkIn() {
      try {
        const result = await this.saveCheckInData('checkin')
        if (result === false) return

        this.workStatus = 'checked-in'
        this.checkInTime = new Date().toLocaleTimeString('th-TH')
        this.showWorkTimeSuccess('checkin')
      } catch (error) {
        console.error('[OnsiteForm] เกิดข้อผิดพลาดในการลงเวลาเข้างาน:', error)
        this.showWorkTimeError('checkin')
        throw error
      }
    },

    async checkOut() {
      try {
        const result = await this.saveCheckInData('checkout')
        if (result === false) return

        this.workStatus = 'checked-out'
        this.checkOutTime = new Date().toLocaleTimeString('th-TH')
        this.showWorkTimeSuccess('checkout')
      } catch (error) {
        console.error('[OnsiteForm] เกิดข้อผิดพลาดในการลงเวลาออกงาน:', error)
        this.showWorkTimeError('checkout')
        throw error
      }
    },

    showWorkTimeError(mode: string) {
      const action = mode === 'checkin' ? 'เข้างาน' : 'ออกงาน'
      this.sweetAlert.error(`เกิดข้อผิดพลาดในการลงเวลา${action}!`)
    },

    showWorkTimeSuccess(mode: string) {
      const action = mode === 'checkin' ? 'เข้างาน' : 'ออกงาน'
      this.sweetAlert.successDeleted(`ลงเวลา${action}สำเร็จ!`)
    },

    // === SAVE DATA METHODS ===
    async saveCheckInData(type: 'checkin' | 'checkout') {
      try {
        if (type === 'checkin') {
          return await this.processCheckIn()
        } else {
          return await this.processCheckOut()
        }
      } catch (error) {
        this.handleSaveError(error, type)
        throw error
      }
    },

    async processCheckIn() {
      console.log('=== Starting processCheckIn ===')

      // ตรวจสอบสถานะล่าสุดก่อนส่ง API
      await this.loadCurrentWorkStatus()

      if (this.workStatus !== 'idle') {
        console.log('User already checked in today, workStatus:', this.workStatus)
        this.sweetAlert.warning('คุณได้ลงเวลาเข้างานวันนี้แล้ว', 'ไม่สามารถลงเวลาเข้างานซ้ำได้')
        return false
      }

      // ตรวจสอบข้อมูลที่จำเป็น
      if (!this.employeeData?.id) {
        console.error('Employee data not found:', this.employeeData)
        this.sweetAlert.error('ไม่พบข้อมูลพนักงาน กรุณาลองใหม่อีกครั้ง')
        return false
      }

      if (!this.location) {
        console.error('Location data not found:', this.location)
        this.sweetAlert.error('ไม่พบข้อมูลตำแหน่ง กรุณาลองใหม่อีกครั้ง')
        return false
      }

      const { base64Data, fileName } = this.getPhotoData('checkin')
      console.log('Photo data:', { hasBase64: !!base64Data, fileName })

      // ตั้งค่าข้อมูลสำหรับบันทึก
      this.createCommand.latLong = `${this.location?.latitude}, ${this.location?.longitude}`
      this.createCommand.employeeId = this.employeeData?.id
      this.createCommand.location = this.location?.address
      this.createCommand.checkIn = new Date()
      this.createCommand.ipAddress = this.userIPAddress
      this.createCommand.checkInCheckOutTypes = 0 // 0 = เข้างาน

      if (this.$route.params.projectId) {
        this.createCommand.projectId = this.$route.params.projectId as any
      }

      if (base64Data && fileName) {
        this.createCommand.fileName = fileName
        this.createCommand.base64 = base64Data
      }

      console.log('CreateCommand data:', {
        employeeId: this.createCommand.employeeId,
        organizationId: this.createCommand.organizationId,
        location: this.createCommand.location,
        latLong: this.createCommand.latLong,
        ipAddress: this.createCommand.ipAddress,
        checkInCheckOutTypes: this.createCommand.checkInCheckOutTypes,
        hasPhoto: !!(this.createCommand.fileName && this.createCommand.base64),
      })

      try {
        const success = await client.createCheckInCheckOut(this.createCommand)
        console.log('API Response:', success)

        if (success.status) {
          console.log('Check-in saved successfully to backend')
          await this.loadCurrentWorkStatus()
        } else {
          console.error('API returned false status:', success.message)
          // ถ้าเป็นข้อความที่บอกว่าลงเวลาซ้ำ ให้แสดง warning แทน error
          if (success.message && success.message.includes('ลงบันทึกเข้าไปแล้ว')) {
            this.sweetAlert.warning('คุณได้ลงเวลาเข้างานวันนี้แล้ว', 'ไม่สามารถลงเวลาเข้างานซ้ำได้')
            await this.loadCurrentWorkStatus() // อัพเดทสถานะให้ถูกต้อง
          } else {
            this.sweetAlert.error(success.message || 'เกิดข้อผิดพลาดในการบันทึกเข้างาน', 'กรุณาลองใหม่อีกครั้ง')
          }
          return false
        }
      } catch (apiError) {
        console.error('API call failed:', apiError)
        throw apiError
      }
    },

    async processCheckOut() {
      console.log('=== Starting processCheckOut ===')

      // ตรวจสอบสถานะล่าสุดก่อนส่ง API
      await this.loadCurrentWorkStatus()

      if (this.workStatus === 'checked-out') {
        console.log('User already checked out today, workStatus:', this.workStatus)
        this.sweetAlert.warning('คุณได้ลงเวลาออกงานวันนี้แล้ว', 'ไม่สามารถลงเวลาออกงานซ้ำได้')
        return false
      }

      if (this.workStatus === 'idle') {
        console.log('User has not checked in today, workStatus:', this.workStatus)
        this.sweetAlert.warning('คุณยังไม่ได้ลงเวลาเข้างานวันนี้', 'กรุณาลงเวลาเข้างานก่อน')
        return false
      }

      // ตรวจสอบข้อมูลที่จำเป็น
      if (!this.employeeData?.id) {
        console.error('Employee data not found:', this.employeeData)
        this.sweetAlert.error('ไม่พบข้อมูลพนักงาน กรุณาลองใหม่อีกครั้ง')
        return false
      }

      if (!this.location) {
        console.error('Location data not found:', this.location)
        this.sweetAlert.error('ไม่พบข้อมูลตำแหน่ง กรุณาลองใหม่อีกครั้ง')
        return false
      }

      const { base64Data, fileName } = this.getPhotoData('checkout')
      console.log('Photo data for checkout:', { hasBase64: !!base64Data, fileName })

      // ตั้งค่าข้อมูลสำหรับบันทึก checkout แบบ create
      this.createCommand.latLong = `${this.location?.latitude}, ${this.location?.longitude}`
      this.createCommand.employeeId = this.employeeData?.id
      this.createCommand.location = this.location?.address
      this.createCommand.checkIn = new Date() // ใช้ checkIn field แต่ระบุ type เป็น CheckOut
      this.createCommand.ipAddress = this.userIPAddress
      this.createCommand.checkInCheckOutTypes = 1 // 1 = ออกงาน

      if (this.$route.params.projectId) {
        this.createCommand.projectId = this.$route.params.projectId as any
      }

      if (base64Data && fileName) {
        this.createCommand.fileName = fileName
        this.createCommand.base64 = base64Data
      }

      console.log('CreateCommand data for checkout:', {
        employeeId: this.createCommand.employeeId,
        organizationId: this.createCommand.organizationId,
        location: this.createCommand.location,
        latLong: this.createCommand.latLong,
        ipAddress: this.createCommand.ipAddress,
        checkInCheckOutTypes: this.createCommand.checkInCheckOutTypes,
        hasPhoto: !!(this.createCommand.fileName && this.createCommand.base64),
      })

      try {
        console.log('Sending checkout API request...')
        const success = await client.createCheckInCheckOut(this.createCommand)
        console.log('Checkout API Response:', success)

        if (success.status) {
          console.log('Check-out saved successfully to backend')
          await this.loadCurrentWorkStatus()
        } else {
          console.error('API returned false status:', success.message)
          this.sweetAlert.error(success.message || 'เกิดข้อผิดพลาดในการบันทึกออกงาน', 'กรุณาลองใหม่อีกครั้ง')
          return false
        }
      } catch (apiError) {
        console.error('Checkout API call failed:', apiError)
        throw apiError
      }
    },

    getPhotoData(type: 'checkin' | 'checkout') {
      const photoData = type === 'checkin' ? this.photoCaptureStore.checkinImage : this.photoCaptureStore.checkoutImage

      if (photoData && typeof photoData === 'string' && photoData.includes(',')) {
        return {
          base64Data: photoData.split(',')[1],
          fileName: `${type}_photo_${Date.now()}.png`,
        }
      }

      return { base64Data: '', fileName: '' }
    },

    handleSaveError(error: any, type: string) {
      console.error(`[OnsiteForm] เกิดข้อผิดพลาดในการบันทึก ${type}:`, error)

      if (error?.status === 404) {
        this.sweetAlert.error('ระบบยังไม่พร้อมใช้งาน กรุณาติดต่อผู้ดูแลระบบ')
      } else {
        const action = type === 'checkin' ? 'เข้างาน' : 'ออกงาน'
        this.sweetAlert.error(`เกิดข้อผิดพลาดในการบันทึก${action} กรุณาลองใหม่อีกครั้ง`)
      }
    },
    // === BUTTON METHODS ===
    getSlideButtonClass() {
      const isWorkStep = !this.cameraActive || this.showPhotoPreview

      if (isWorkStep) {
        if (this.currentMode === 'checkin') {
          return this.isCheckinDisabled ? 'success-btn-disabled' : 'success-btn'
        } else {
          return this.workStatus === 'checked-out' ? 'error-btn-disabled' : 'error-btn'
        }
      }
      return 'photo-btn'
    },

    getSlideButtonColor() {
      const isWorkStep = !this.cameraActive || this.showPhotoPreview

      if (isWorkStep) {
        return this.currentMode === 'checkin' ? 'success' : 'error'
      }

      // For photo capture step - change color when face detection is required
      if (this.faceApiLoaded && !this.detectedFace) {
        return 'warning'
      }
      return 'primary'
    },

    getSlideButtonText() {
      const isWorkStep = !this.cameraActive || this.showPhotoPreview

      if (isWorkStep) {
        if (this.currentMode === 'checkin') {
          return this.isCheckinDisabled ? 'ลงเวลาเข้างานแล้ววันนี้' : 'เข้างาน'
        } else {
          return this.workStatus === 'checked-out' ? 'ลงเวลาออกงานแล้ววันนี้' : 'ออกงาน'
        }
      }

      // For photo capture step
      if (this.faceApiLoaded && !this.detectedFace) {
        return 'จัดตำแหน่งใบหน้า'
      }
      return 'ถ่ายรูป'
    },

    getSlideButtonIcon() {
      const isWorkStep = !this.cameraActive || this.showPhotoPreview

      if (isWorkStep) {
        return this.currentMode === 'checkin' ? 'ri-login-circle-line' : 'ri-logout-circle-line'
      }
      return 'ri-camera-line'
    },

    getSlideButtonTrailingIcon() {
      const isWorkStep = !this.cameraActive || this.showPhotoPreview

      if (isWorkStep) {
        const isDisabled =
          (this.currentMode === 'checkin' && this.isCheckinDisabled) ||
          (this.currentMode === 'checkout' && this.workStatus === 'checked-out')
        return isDisabled ? 'ri-check-line' : null
      }
      return null
    },

    getSlideButtonDisabled() {
      const isWorkStep = !this.cameraActive || this.showPhotoPreview

      if (isWorkStep) {
        if (this.currentMode === 'checkin') {
          return !this.showPhotoPreview || this.isCheckinDisabled
        } else {
          return this.workStatus === 'checked-out' || !this.showPhotoPreview
        }
      }

      // For photo capture step - disable if no face detected when face-api is loaded
      const faceDetectionRequired = this.faceApiLoaded && !this.detectedFace
      return !this.cameraActive || this.showPhotoPreview || this.isLoading || faceDetectionRequired
    },

    async handleSlideButtonClick() {
      const form = this.$refs.form as any
      const { valid } = await form.validate()
      const isWorkStep = !this.cameraActive || this.showPhotoPreview

      if (isWorkStep) {
        if (this.currentMode === 'checkin') {
          if (this.isCheckinDisabled) {
            this.showDuplicateWorkTimeError('checkin')
            return
          }
          if (valid) this.openCheckinDialog()
        } else {
          if (this.workStatus === 'checked-out') {
            this.showDuplicateWorkTimeError('checkout')
            return
          }
          this.openCheckoutDialog()
        }
      } else {
        this.manualCapturePhoto()
      }
    },

    goBack() {
      this.$emit('close')
    },

    // === STATUS INDICATOR METHODS ===
    getStatusIcon() {
      if (!this.faceApiLoaded) return 'ri-loader-4-line'
      return this.detectedFace ? 'ri-checkbox-circle-line' : 'ri-error-warning-line'
    },

    getStatusIconColor() {
      if (!this.faceApiLoaded) return 'grey'
      return this.detectedFace ? 'success' : 'warning'
    },

    getStatusText() {
      if (!this.faceApiLoaded) return 'กำลังโหลด...'
      return this.detectedFace ? 'ตรวจพบใบหน้า' : 'ไม่พบใบหน้า'
    },
  },
})
</script>

<style scoped>
.onsite-form-container {
  height: 100%;
  background-color: #f5f5f5;
}

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

/* Check-in/Check-out Content */
.checkin-checkout-content {
  background-color: #f5f5f5;
  min-height: calc(100vh - 220px);
}

/* Camera Section */
.camera-container {
  position: relative;
  width: 50%;
  height: 450px;
  background-color: #000;
  border-radius: 12px;
  overflow: hidden;
  margin-bottom: 16px;
  justify-self: center;
}

.camera-video {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transform: scaleX(-1);
}

.camera-placeholder {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 100%;
  color: #666;
}

/* Face Detection Status Indicator */
.face-status-indicator {
  position: absolute;
  top: 12px;
  right: 12px;
  z-index: 10;
}

.status-badge {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 8px 12px;
  border-radius: 20px;
  backdrop-filter: blur(10px);
  font-size: 12px;
  font-weight: 600;
  transition: all 0.3s ease;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.2);
}

.status-detected {
  background-color: rgba(76, 175, 80, 0.9);
  color: white;
  border: 1px solid rgba(76, 175, 80, 0.5);
}

.status-not-detected {
  background-color: rgba(255, 152, 0, 0.9);
  color: white;
  border: 1px solid rgba(255, 152, 0, 0.5);
  animation: status-pulse 2s infinite;
}

.status-loading {
  background-color: rgba(158, 158, 158, 0.9);
  color: white;
  border: 1px solid rgba(158, 158, 158, 0.5);
}

.status-loading .v-icon {
  animation: spin 1s linear infinite;
}

.status-text {
  white-space: nowrap;
  font-size: 11px;
}

@keyframes status-pulse {
  0% {
    opacity: 0.8;
    transform: scale(1);
  }
  50% {
    opacity: 1;
    transform: scale(1.05);
  }
  100% {
    opacity: 0.8;
    transform: scale(1);
  }
}

@keyframes spin {
  from {
    transform: rotate(0deg);
  }
  to {
    transform: rotate(360deg);
  }
}

/* Action Buttons */
.onsite-action-btn-text {
  font-size: 16px;
  letter-spacing: 0.5px;
}

@media (max-width: 599px) {
  .onsite-action-btn-text {
    font-size: 14px;
  }
}

.success-btn,
.error-btn,
.photo-btn,
.slide-btn {
  border-radius: 12px;
  font-weight: 600;
  padding: 14px;
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
}

.photo-btn:disabled {
  background: linear-gradient(135deg, #9e9e9e 0%, #757575 100%) !important;
  opacity: 0.6 !important;
}

.v-btn--variant-elevated.bg-warning:disabled {
  background: linear-gradient(135deg, #ff9800 0%, #f57c00 100%) !important;
  opacity: 0.7 !important;
  color: white !important;
}

.success-btn {
  background: linear-gradient(135deg, #4caf50 0%, #45a049 100%) !important;
}

.success-btn:disabled {
  background: linear-gradient(135deg, #81c784 0%, #66bb6a 100%) !important;
  opacity: 0.8 !important;
}

.error-btn {
  background: linear-gradient(135deg, #f44336 0%, #d32f2f 100%) !important;
}

.error-btn:disabled {
  background: linear-gradient(135deg, #e57373 0%, #ef5350 100%) !important;
  opacity: 0.6 !important;
}

/* Slide Button Container */
.slide-button-container {
  width: 100%;
}

/* Success Button Disabled State */
.success-btn-disabled {
  background: linear-gradient(135deg, #81c784 0%, #66bb6a 100%) !important;
  color: white !important;
  opacity: 0.8 !important;
}

/* Error Button Disabled State */
.error-btn-disabled {
  background: linear-gradient(135deg, #e57373 0%, #ef5350 100%) !important;
  color: white !important;
  opacity: 0.6 !important;
}

/* Info Cards */
.datetime-info,
.location-info,
.work-status-info,
.coordinates-info {
  background-color: white;
  border-radius: 12px;
  padding: 16px;
  margin-bottom: 16px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.work-status-info {
  border-left: 4px solid #4caf50;
}

.datetime-label,
.location-label,
.work-status-label {
  display: block;
  font-size: 14px;
  color: #666;
  margin-bottom: 8px;
}

.datetime-value,
.location-value,
.work-status-value {
  display: block;
  font-size: 16px;
  font-weight: 600;
  color: #333;
}

.work-status-value.text-success {
  color: #4caf50 !important;
}

.work-status-value.text-error {
  color: #f44336 !important;
}

.coordinates-title {
  font-size: 16px;
  font-weight: 600;
  color: #333;
  margin-bottom: 12px;
}

.coordinates-text,
.distance-text {
  font-size: 12px;
  color: #666;
  margin: 4px 0;
  line-height: 1.4;
}

/* Dialog Styles */
.dialog-card {
  border-radius: 16px !important;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.15) !important;
}

.dialog-title {
  font-size: 20px;
  font-weight: 600;
  color: #333;
  margin-top: 8px;
}

.confirmation-details {
  background-color: #f8f9fa;
  border-radius: 12px;
  padding: 16px;
  margin: 16px 0;
}

.detail-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 8px;
}

.detail-item:last-child {
  margin-bottom: 0;
}

.detail-label {
  font-size: 14px;
  color: #666;
  font-weight: 500;
}

.detail-value {
  font-size: 14px;
  color: #333;
  font-weight: 600;
}

.confirmation-message {
  font-size: 16px;
  color: #333;
  font-weight: 500;
  text-align: center;
}

/* Close Button - Fixed top right */
.close-btn {
  position: absolute !important;
  top: 16px !important;
  right: 16px !important;
  z-index: 2010 !important;
  background: rgba(255, 255, 255, 0.9) !important;
  backdrop-filter: blur(10px);
  border: none !important;
  color: #2b3086 !important;
  transition: all 0.2s ease;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.close-btn:hover {
  background: rgba(43, 48, 134, 0.1) !important;
  transform: scale(1.05);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

/* Photo Preview Styles */
.photo-preview {
  width: 100%;
  height: 100%;
  object-fit: cover;
  border-radius: 12px;
}

.photo-preview-controls {
  position: absolute;
  bottom: 16px;
  left: 50%;
  transform: translateX(-50%);
  z-index: 10;
}

.retake-btn {
  color: white !important;
  border-radius: 25px !important;
  padding: 12px 24px !important;
  font-weight: 600;
  box-shadow: 0 4px 12px rgba(43, 48, 134, 0.3);
  transition: all 0.3s ease;
  display: flex !important;
  align-items: center !important;
  justify-content: center !important;
}

.retake-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 16px rgba(43, 48, 134, 0.4);
}

/* Responsive Design */
@media (max-width: 599px) {
  .text-sub-title {
    font-size: 18px;
    text-align: center;
    margin-bottom: 10px;
  }

  .camera-container {
    height: 220px;
  }

  .success-btn,
  .error-btn,
  .slide-btn {
    font-size: 14px;
    padding: 12px;
  }

  .dialog-title {
    font-size: 18px;
  }

  .confirmation-message {
    font-size: 14px;
  }

  .close-btn {
    top: 12px !important;
    right: 12px !important;
  }

  .dialog-close-btn {
    top: 8px !important;
    right: 8px !important;
  }

  .photo-preview-controls {
    bottom: 12px;
  }

  .retake-btn {
    padding: 10px 20px !important;
    font-size: 14px;
    display: flex !important;
    align-items: center !important;
    justify-content: center !important;
  }

  /* Location section margin for mobile */
  .coordinates-info {
    margin-bottom: 27% !important;
  }

  /* Status indicator mobile adjustments */
  .face-status-indicator {
    top: 8px;
    right: 8px;
  }

  .status-badge {
    padding: 6px 10px;
    font-size: 10px;
  }

  .status-text {
    font-size: 9px;
  }
}

/* Dialog Close Button */
.dialog-close-btn {
  position: absolute !important;
  top: 16px;
  right: 16px;
  z-index: 1;
  background-color: rgba(255, 255, 255, 0.9) !important;
  border-radius: 50% !important;
}

.dialog-close-btn:hover {
  background-color: rgba(255, 255, 255, 1) !important;
}

/* Dialog Card Styles */
.dialog-card {
  border-radius: 16px !important;
  overflow: hidden;
}

.dialog-title {
  font-size: 20px;
  font-weight: 600;
  color: #333;
  margin-top: 8px;
}

.confirmation-details {
  background-color: #f8f9fa;
  border-radius: 8px;
  padding: 16px;
  margin: 16px 0;
}

.detail-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 8px;
}

.detail-item:last-child {
  margin-bottom: 0;
}

.detail-label {
  font-size: 14px;
  color: #666;
  font-weight: 500;
}

.detail-value {
  font-size: 14px;
  color: #333;
  font-weight: 600;
}

.confirmation-message {
  font-size: 16px;
  color: #333;
  font-weight: 500;
}

.location-text {
  max-width: 250px;
  overflow: hidden;
  white-space: nowrap;
  text-overflow: ellipsis;
  display: inline-block;
}
</style>
