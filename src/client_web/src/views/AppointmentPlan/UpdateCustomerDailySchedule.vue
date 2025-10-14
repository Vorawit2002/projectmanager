<template>
  <v-row class="justify-center">
    <v-col
      cols="12"
      md="12"
    >
      <v-card class="scroll-content page-container dialog-scrollbar">
        <!-- card title -->
        <v-row class="mt-3 mb-4">
          <v-col
            cols="12"
            class="d-flex justify-center"
          >
            <span class="text-sub-title text-center">
              {{ mode === 'duplicate' ? 'สร้างตารางแจ้งงานรายวันจากวันที่ล่าสุด' : 'แก้ไขตารางแจ้งงานรายวัน' }}
            </span>
          </v-col>
        </v-row>

        <v-card-text>
          <v-form
            ref="form"
            @submit.prevent="UpdateActivityPlan"
            class="text-black"
          >
            <v-row>
              <v-col
                cols="12"
                align="end"
              >
                <p class="mr-2 mr-md-8 text-h5 text-primary">แผนก : {{ User.departments?.name }}</p>
                <p class="mr-2 mr-md-8 text-h5 text-primary">
                  โดย : {{ User.titleName + ' ' + User.firstName + ' ' + User.lastName }}
                </p>
              </v-col>
            </v-row>

            <v-row class="px-2 px-sm-4 px-md-5">
              <v-col
                cols="0"
                md="9"
              ></v-col>

              <!-- <v-col
                cols="12"
                md="3"
                class="d-flex justify-end"
              >
                <v-switch
                  v-model="updateCommand.outSide"
                  :label="switchLabel"
                  hide-details
                  color="primary"
                  inset
                  :readonly="ConditionReadonly"
                />
              </v-col> -->

              <v-col
                cols="12"
                md="12"
              >
                <v-row>
                  <v-col
                    cols="12"
                    sm="12"
                    md="12"
                  >
                    <label class="mb-2">วันที่เริ่มต้น <span class="text-error">*</span> </label>
                    <div>
                      <TextFieldDatepicker
                        v-if="updateCommand.startDate"
                        placeholder="กรุณาระบุวันที่เริ่มต้นนัดหมาย"
                        :rules="appointmentStartDateRules"
                        :AllDay="updateCommand.allDay"
                        :selectedDateTime="updateCommand.startDate"
                        @selectedDateTime="changeTimestartDate"
                        :readonly="ConditionReadonly"
                      />
                      <TextFieldDatepicker
                        v-else
                        placeholder="กรุณาระบุวันที่เริ่มต้นนัดหมาย"
                        :rules="appointmentStartDateRules"
                        :AllDay="updateCommand.allDay"
                        @selectedDateTime="changeTimestartDate"
                        :readonly="ConditionReadonly"
                      />
                    </div>
                  </v-col>

                  <v-col
                    cols="12"
                    sm="12"
                    md="12"
                  >
                    <label class="mb-2">วันที่สิ้นสุด <span class="text-error">*</span> </label>
                    <div>
                      <TextFieldDatepicker
                        v-if="updateCommand.endDate"
                        placeholder="กรุณาระบุวันที่สิ้นสุดนัดหมาย"
                        :rules="appointmentEndDateRules"
                        :AllDay="updateCommand.allDay"
                        :selectedDateTime="updateCommand.endDate"
                        @selectedDateTime="changeTimeendDate"
                        :readonly="ConditionReadonly"
                      />
                      <TextFieldDatepicker
                        v-else
                        placeholder="กรุณาระบุวันที่สิ้นสุดนัดหมาย"
                        :minDate="updateCommand.startDate"
                        :rules="appointmentEndDateRules"
                        :AllDay="updateCommand.allDay"
                        :selectedDateTime="updateCommand.endDate"
                        @selectedDateTime="changeTimeendDate"
                        :readonly="ConditionReadonly"
                      />
                    </div>
                  </v-col>

                  <v-col
                    cols="12"
                    sm="12"
                    md="12"
                    class="mt-0 mt-md-7 d-flex align-center"
                  >
                    <v-checkbox
                      label="ทั้งวัน"
                      hide-details
                      v-model="updateCommand.allDay"
                      :readonly="ConditionReadonly"
                    />
                  </v-col>
                </v-row>
              </v-col>

              <v-col cols="12">
                <label class="mb-2">รหัสโครงการ </label>
                <v-autocomplete
                  placeholder="กรุณาเลือกรหัสโครงการ"
                  clearable
                  v-model="updateCommand.projectId"
                  :items="ProjectList"
                  item-title="projectCodeAndName"
                  item-value="id"
                  :readonly="ConditionReadonly"
                />
              </v-col>

              <v-col cols="12">
                <label class="mb-2">รายละเอียดเพิ่มเติม <span class="text-error">*</span></label>
                <v-textarea
                  v-model="updateCommand.detail"
                  :rules="[(v:any) => !!v || 'กรุณาระบุรายละเอียด',
                    (v:any) => v.length >= 10 || 'กรุณาระบุรายละเอียดอย่างน้อย 10 ตัวอักษร'
                  ]"
                  placeholder="ระบุรายละเอียดการนัดพบ"
                  :readonly="ConditionReadonly"
                />
              </v-col>

              <!-- Check-in Section -->
              <v-col
                cols="12"
                v-if="updateCommand.outSide"
              >
                <v-row class="align-center mt-5">
                  <v-col
                    cols="12"
                    class="text-center"
                  >
                    <h5 class="text-h5 font-weight-bold text-primary mb-3">
                      <v-icon class="mr-2">ri-map-pin-line</v-icon>
                      Check in / Check out
                    </h5>
                  </v-col>
                </v-row>
                <v-divider class="mb-4"></v-divider>

                <v-row class="justify-center">
                  <!-- Location Section -->
                  <v-col
                    cols="12"
                    md="12"
                  >
                    <v-alert
                      v-if="locationError"
                      type="error"
                      variant="tonal"
                      class="mt-2 pa-2"
                    >
                      {{ locationError }}
                    </v-alert>
                  </v-col>

                  <!-- Camera Section -->
                  <v-col
                    cols="12"
                    md="10"
                  >
                    <div class="mb-3">
                      <label class="mb-4">แนบรูปถ่าย</label>
                      <div v-if="cameraSupported">
                        <video
                          ref="video"
                          autoplay
                          playsinline
                          class="mb-4 mt-2 video-full-width"
                          style="
                            border-radius: 8px;
                            border: 1px solid #ccc;
                            transform: none;
                            width: 100%;
                            height: 350px !important;
                            min-height: 350px;
                            object-fit: cover;
                            display: block;
                          "
                        ></video>

                        <div class="d-flex flex-wrap gap-3 justify-center mb-2">
                          <div class="checkin-btn-group">
                            <!-- <v-btn
                              color="primary"
                              @click="getLocation"
                              :loading="loadingLocation"
                              :disabled="loadingLocation || ConditionReadonly"
                              class="checkin-btn"
                              prepend-icon="ri-map-pin-line"
                            >
                              {{ loadingLocation ? 'กำลังดึงตำแหน่ง...' : 'ดึงตำแหน่งปัจจุบัน' }}
                            </v-btn> -->
                            <v-btn
                              color="success-darken-2"
                              @click="capturePhoto"
                              :disabled="!cameraActive || ConditionReadonly"
                              class="checkin-btn"
                              prepend-icon="ri-camera-lens-line"
                            >
                              ถ่ายรูป
                            </v-btn>
                            <v-btn
                              color="error"
                              @click="stopCamera"
                              v-if="cameraActive"
                              class="checkin-btn"
                              prepend-icon="ri-camera-off-fill"
                            >
                              ปิดกล้อง
                            </v-btn>
                            <v-btn
                              color="success"
                              @click="startCamera"
                              v-if="!cameraActive"
                              class="checkin-btn"
                              prepend-icon="ri-camera-fill"
                              :disabled="ConditionReadonly"
                            >
                              เปิดกล้อง
                            </v-btn>
                          </div>
                        </div>

                        <div
                          v-if="location"
                          class="mt-4"
                        >
                          <v-alert
                            variant="tonal"
                            color="primary"
                            class="pa-2 pl-4"
                          >
                            <div><strong>สถานที่:</strong> {{ location.address || 'กำลังโหลด...' }}</div>
                          </v-alert>
                        </div>
                      </div>

                      <div v-else>
                        <v-file-input
                          accept="image/*"
                          label="เลือกไฟล์รูปภาพ"
                          prepend-icon="ri-image-line"
                          @change="onFileChange"
                          hide-details
                          dense
                          :disabled="ConditionReadonly"
                        ></v-file-input>
                      </div>
                    </div>
                  </v-col>
                </v-row>

                <!-- Photo Preview -->
                <v-col
                  cols="12"
                  class="text-center"
                  v-if="imageSrcList && imageSrcList.length > 0"
                >
                  <div class="d-flex justify-center flex-wrap gap-3 mt-2">
                    <div
                      v-for="(img, idx) in imageSrcList"
                      :key="idx"
                      class="image-container"
                      style="
                        position: relative;
                        display: block;
                        width: calc(50% - 12px);
                        max-width: 300px;
                        overflow: hidden;
                        border-radius: 12px;
                      "
                    >
                      <v-img
                        :src="img.src"
                        alt="รูปที่ถ่าย"
                        width="100%"
                        height="200"
                        class="rounded border"
                        style="width: 100%; position: relative"
                      >
                        <template v-slot:default>
                          <div style="position: absolute; top: 0; left: 0; right: 0; bottom: 0; pointer-events: none">
                            <v-btn
                              v-if="!ConditionReadonly"
                              icon
                              size="small"
                              @click="removeImage(idx)"
                              :disabled="loading"
                              color="white"
                              elevation="2"
                              style="
                                position: absolute;
                                top: 8px;
                                right: 8px;
                                pointer-events: auto;
                                z-index: 100;
                                background: #fff;
                              "
                            >
                              <v-icon
                                size="18"
                                color="error"
                                >ri-delete-bin-line</v-icon
                              >
                            </v-btn>

                            <v-chip
                              :color="img.create ? 'success' : 'warning'"
                              size="small"
                              class="image-status-chip-inside"
                              variant="flat"
                              style="position: absolute; bottom: 8px; left: 8px; pointer-events: none; z-index: 100"
                            >
                              {{ img.create ? 'บันทึกแล้ว' : 'ใหม่' }}
                            </v-chip>
                          </div>
                        </template>
                      </v-img>
                    </div>
                  </div>
                </v-col>

                <!-- Map Section -->
                <!-- <v-row class="mt-3">
                  <v-col cols="12">
                    <div class="d-flex align-center mb-3 ml-4">
                      <v-icon
                        icon="ri-map-pin-line"
                        class="section-icon mr-2"
                      />
                      <h3 class="section-title">ตำแหน่ง</h3>
                    </div>
                    <div
                      class="map-container"
                      style="height: 350px"
                    >
                      <template v-if="isValidCoordinates">
                        <iframe
                          :src="googleMapsUrl"
                          class="map-iframe"
                          style="width: 100%; height: 100%; border-radius: 8px; border: 1px solid #ccc"
                          loading="lazy"
                          referrerpolicy="no-referrer-when-downgrade"
                        ></iframe>
                      </template>
                      <template v-else>
                        <v-card
                          height="200"
                          class="d-flex align-center justify-center"
                          color="grey-lighten-4"
                          rounded="xl"
                        >
                          <div class="text-center">
                            <v-icon
                              size="48"
                              color="grey-darken-1"
                              class="mb-3"
                            >
                              mdi-map-marker-outline
                            </v-icon>
                            <p class="text-grey-darken-1 text-body-2">
                              {{ coordinatesMessage }}
                            </p>
                          </div>
                        </v-card>
                      </template>
                    </div>
                  </v-col>
                </v-row> -->
              </v-col>

              <v-col
                cols="12"
                class="pa-0"
              >
                <!-- Responsive action buttons: mobile = 50/50, desktop = together -->
                <template v-if="$vuetify.display.mobile">
                  <v-row class="mt-2 action-row">
                    <v-col
                      cols="6"
                      class="pr-1"
                    >
                      <v-btn
                        class="mobile-btn"
                        color="error"
                        @click="closeDialog"
                        :disabled="loading"
                        size="default"
                        block
                      >
                        <v-icon class="mr-2">ri-close-large-fill</v-icon>
                        ยกเลิก
                      </v-btn>
                    </v-col>
                    <v-col
                      cols="6"
                      class="pl-1"
                    >
                      <v-btn
                        v-if="shouldShowSaveButton"
                        type="submit"
                        color="success-darken-2"
                        :loading="loading"
                        :disabled="loading || !canSubmit"
                        size="default"
                        block
                        class="mobile-btn"
                      >
                        <v-icon class="mr-2">ri-save-3-fill</v-icon>
                        บันทึก
                      </v-btn>
                    </v-col>
                  </v-row>
                </template>
                <template v-else>
                  <div class="d-flex justify-center flex-wrap gap-2 mt-2 mb-2">
                    <v-btn
                      class="me-2 mb-2 mb-sm-0 mobile-btn"
                      color="error"
                      @click="closeDialog"
                      :disabled="loading"
                      size="large"
                    >
                      <v-icon class="mr-2">ri-close-large-fill</v-icon>
                      ยกเลิก
                    </v-btn>
                    <v-btn
                      v-if="shouldShowSaveButton"
                      type="submit"
                      color="success-darken-2"
                      :loading="loading"
                      :disabled="loading || !canSubmit"
                      size="large"
                      class="mobile-btn"
                    >
                      <v-icon class="mr-2">ri-save-3-fill</v-icon>
                      บันทึก
                    </v-btn>
                  </div>
                </template>
              </v-col>
            </v-row>
          </v-form>
        </v-card-text>
      </v-card>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import {
  Client,
  CreateActivityPlanAttachmentCommand,
  CreateActivityPlanCommand,
  GetActivityPlanAttachmentsByActivityPlanIdQuery,
  UpdateActivityPlanCommand,
} from '@/client'
import TextFieldDatepicker from '@/components/Datepicker/TextFieldDatepicker.vue'
import TextFieldTimepicker from '@/components/Datepicker/TextFieldTimepicker.vue'
import { BACKEND_API_URL } from '@/constants'
import { useAuthStore, useSweetAlertStore } from '@/stores'
import { appointmentEndDateRules, appointmentStartDateRules } from '@/utils/RuleServices'
import { defineComponent, PropType } from 'vue'

const client = new Client(BACKEND_API_URL)
export default defineComponent({
  name: 'UpdateCustomerDailySchedule',
  components: {
    TextFieldDatepicker,
    TextFieldTimepicker,
  },
  props: {
    CloseDialogUpdate: {
      type: Function,
      required: true,
    },
    id: {
      type: String,
      required: true,
    },
    mode: {
      type: String as PropType<'edit' | 'duplicate'>,
    },
    selectedDate: {
      type: Date,
      required: false,
    },
  },
  data() {
    return {
      ProjectList: [] as any,
      createCommand: new CreateActivityPlanCommand(),
      updateCommand: new UpdateActivityPlanCommand(),
      auth: useAuthStore(),
      sweetAlertStore: useSweetAlertStore(),
      loading: false,
      User: {} as any,
      UserId: '' as any,
      attachments: [] as any[],
      imageSrcList: [] as any,
      appointmentStartDateRules,
      appointmentEndDateRules,
      location: null as any,
      locationError: '',
      loadingLocation: false,
      cameraSupported: false,
      cameraActive: false,
      cameraError: '',
      photoData: null as any,
      stream: null as any,
    }
  },
  computed: {
    switchLabel() {
      return this.updateCommand.outSide ? 'ออกนอกสถานที่' : 'เข้าออฟฟิศ'
    },
    canSubmit() {
      if (this.updateCommand.outSide) {
        return this.location && this.updateCommand.detail && this.updateCommand.detail.length >= 10
      } else {
        return this.updateCommand.detail && this.updateCommand.detail.length >= 10
      }
    },
    ConditionReadonly() {
      if (this.UserId === this.updateCommand.employeeId) {
        return false
      } else {
        return true
      }
    },
    // ตรวจสอบว่าวันที่ที่เลือกเป็นวันนี้หรือไม่
    isToday() {
      if (!this.selectedDate) return true // ถ้าไม่มี selectedDate ให้แสดงปุ่มได้ (กรณี default)

      const today = new Date()
      const selected = new Date(this.selectedDate)

      // เปรียบเทียบเฉพาะวัน เดือน ปี (ไม่สนใจเวลา)
      const isToday =
        today.getFullYear() === selected.getFullYear() &&
        today.getMonth() === selected.getMonth() &&
        today.getDate() === selected.getDate()

      // Debug log เพื่อตรวจสอบ
      console.log('Date comparison:', {
        today: today.toDateString(),
        selected: selected.toDateString(),
        isToday: isToday,
      })

      return isToday
    },
    // ตรวจสอบว่าควรแสดงปุ่มบันทึกหรือไม่
    shouldShowSaveButton() {
      return !this.ConditionReadonly
    },
  },
  watch: {
    id: {
      immediate: true,
      handler(newVal) {
        if (newVal) {
          this.initialize()
        }
      },
    },
    'updateCommand.outSide'(val: boolean) {
      if (val) {
        this.cameraSupported = !!(navigator.mediaDevices && navigator.mediaDevices.getUserMedia)
        if (this.cameraSupported) {
          this.startCamera()
        }
        this.getLocation()
      } else {
        this.stopCamera()
        this.photoData = null
        this.location = null
        this.updateCommand.location = ''
      }
    },
  },
  async mounted() {
    console.log('DEBUG: UpdateCustomerDailySchedule mounted with selectedDate:', this.selectedDate)
    console.log('DEBUG: isToday computed:', this.isToday)
    console.log('DEBUG: shouldShowSaveButton computed:', this.shouldShowSaveButton)

    if (this.id) {
      await this.initialize()
      if (this.updateCommand.outSide) {
        this.cameraSupported = !!(navigator.mediaDevices && navigator.mediaDevices.getUserMedia)
      }
    } else {
      this.closeDialog()
    }
  },
  beforeUnmount() {
    this.stopCamera()
  },
  methods: {
    closeDialog(reload: boolean = false) {
      this.stopCamera()
      if (!reload) {
        localStorage.removeItem('selectedAppointmentId')
      }
      this.CloseDialogUpdate(false, reload)
    },
    async initialize() {
      try {
        if (this.auth.userId) {
          const result = await client.getEmployeeQueryByUserID(this.auth.userId)
          this.UserId = result.id
        }

        this.ProjectList = await client.getProjectQuery()
        this.ProjectList.forEach((project: any) => {
          const truncatedName =
            project.projectName.length > 70 ? project.projectName.substring(0, 70) + '...' : project.projectName
          project.projectCodeAndName = `${project.projectCode} - ${truncatedName}`
        })

        const response = await client.getActivityPlanQueryByID(this.id)
        this.updateCommand = { ...response } as UpdateActivityPlanCommand

        if (!this.updateCommand.location || typeof this.updateCommand.location !== 'string') {
          this.updateCommand.location = ''
        } else {
          const [lat, lng] = this.updateCommand.location.split(',').map(Number)
          if (!isNaN(lat) && !isNaN(lng)) {
            this.location = { latitude: lat, longitude: lng }
            this.fetchAddress(lat, lng)
          }
        }

        // 👉 ปรับตรงนี้: กรณี duplicate ให้ใช้วันที่วันนี้
        if (this.mode === 'duplicate') {
          const today = new Date()
          today.setHours(0, 0, 0, 0) // ตั้งเวลาให้เป็นเที่ยงคืนของวันปัจจุบัน
          this.updateCommand.startDate = new Date(today)
          this.updateCommand.endDate = new Date(today)
        }

        const result = await client.getEmployeeQueryByID(this.updateCommand.employeeId as any)
        this.User = result

        const attachmentQuery = new GetActivityPlanAttachmentsByActivityPlanIdQuery()
        attachmentQuery.activityPlanId = this.id
        const attachments = await client.getActivityPlanAttachmentsQueryByActivityPlanId(attachmentQuery)
        this.attachments = attachments
        this.imageSrcList = attachments.map((x: any) => ({
          ...x,
          src: `data:image/png;base64,${x.thumbnailBase64}`,
          create: true,
        }))
      } catch (error) {
        console.error(error)
      }
    },
    async fetchAddress(lat: number, lon: number) {
      try {
        const response = await fetch(`https://nominatim.openstreetmap.org/reverse?format=jsonv2&lat=${lat}&lon=${lon}`)
        const data = await response.json()
        if (this.location) {
          if (data && data.display_name) {
            this.location.address = data.display_name
          } else {
            this.location.address = 'ไม่พบข้อมูลสถานที่'
          }
        }
      } catch (error) {
        if (this.location) {
          this.location.address = 'เกิดข้อผิดพลาดในการเรียกข้อมูลสถานที่'
        }
      }
    },
    async getLocation() {
      this.locationError = ''
      this.loadingLocation = true

      if (!navigator.geolocation) {
        this.locationError = 'Geolocation is not supported by your browser.'
        this.loadingLocation = false
        return
      }

      navigator.geolocation.getCurrentPosition(
        async pos => {
          this.location = {
            latitude: pos.coords.latitude,
            longitude: pos.coords.longitude,
          }
          this.updateCommand.location = `${pos.coords.latitude},${pos.coords.longitude}`
          await this.fetchAddress(pos.coords.latitude, pos.coords.longitude)
          this.loadingLocation = false
        },
        err => {
          this.locationError = 'Failed to get location: ' + err.message
          this.loadingLocation = false
        },
        {
          enableHighAccuracy: true,
        },
      )
    },
    async startCamera() {
      this.cameraError = ''
      if (!navigator.mediaDevices || !navigator.mediaDevices.getUserMedia) {
        this.cameraSupported = false
        return
      }
      try {
        this.stream = await navigator.mediaDevices.getUserMedia({
          video: { facingMode: 'user' },
        })
        if (this.$refs.video) {
          const video = this.$refs.video as HTMLVideoElement
          video.srcObject = this.stream
          video.style.transform = 'none'
        }
        this.cameraActive = true
      } catch (err) {
        this.cameraError = 'Camera access denied or not available.'
        this.cameraSupported = false
      }
    },
    stopCamera() {
      if (this.stream) {
        this.stream.getTracks().forEach((track: MediaStreamTrack) => track.stop())
        this.stream = null
      }
      if (this.$refs.video) {
        const video = this.$refs.video as HTMLVideoElement
        video.srcObject = null
      }
      this.cameraActive = false
    },
    capturePhoto() {
      const video = this.$refs.video as HTMLVideoElement
      if (!video) return
      const canvas = document.createElement('canvas')
      canvas.width = video.videoWidth || 320
      canvas.height = video.videoHeight || 240
      const ctx = canvas.getContext('2d')

      if (ctx) {
        ctx.drawImage(video, 0, 0, canvas.width, canvas.height)
      }
      const photoDataUrl = canvas.toDataURL('image/png')
      this.imageSrcList.push({
        src: photoDataUrl,
        create: false,
        id: null,
      })
      this.stopCamera()
    },
    onFileChange(e: Event) {
      const target = e.target as HTMLInputElement
      if (target && target.files && target.files.length > 0) {
        const file = target.files[0]
        const reader = new FileReader()
        reader.onload = ev => {
          if (ev.target) {
            this.imageSrcList.push({
              src: ev.target.result,
              create: false,
              id: null,
            })
          }
        }
        reader.readAsDataURL(file)
      }
    },
    removeImage(index: number) {
      this.imageSrcList.splice(index, 1)
    },
    changeTimestartDate(value: any) {
      this.updateCommand.startDate = value
    },
    changeTimeendDate(value: any) {
      this.updateCommand.endDate = value
    },
    async UpdateActivityPlan() {
      const form = this.$refs.form as any
      const { valid } = await form.validate()

      if (valid && this.canSubmit) {
        this.loading = true
        try {
          let itemId: string | undefined

          if (this.mode === 'duplicate') {
            this.createCommand = { ...this.updateCommand } as CreateActivityPlanCommand
            this.createCommand.eventType = '002'
            const result = await client.createActivityPlan(this.createCommand)
            itemId = result
            this.sweetAlertStore.successDeleted('บันทึกข้อมูลสำเร็จ!')
            this.closeDialog(true)
          } else {
            const originalAttachmentIds = new Set(this.attachments.map((a: any) => a.id))
            const currentImageIds = new Set(
              this.imageSrcList.filter((img: any) => img.create).map((img: any) => img.id),
            )
            const imagesToDelete = [...originalAttachmentIds].filter(id => !currentImageIds.has(id))
            const imagesToAdd = this.imageSrcList.filter((img: any) => !img.create)

            const response = await client.updateActivityPlan(this.updateCommand)
            if (response) {
              if (imagesToDelete.length > 0) {
                for (const id of imagesToDelete) {
                  try {
                    await client.deleteActivityPlanAttachment(id)
                  } catch (deleteError) {
                    console.error('Failed to delete attachment:', id, deleteError)
                  }
                }
              }

              if (imagesToAdd.length > 0) {
                for (const [i, img] of imagesToAdd.entries()) {
                  try {
                    const createAttachmentCommand = new CreateActivityPlanAttachmentCommand()
                    createAttachmentCommand.activityPlanId = this.updateCommand.id
                    createAttachmentCommand.fileName = `photo_${Date.now()}_${i}.png`
                    let base64Data = ''
                    if (typeof img.src === 'string' && img.src.includes(',')) {
                      base64Data = img.src.split(',')[1]
                    }
                    createAttachmentCommand.base64 = base64Data
                    await client.createActivityPlanAttachment(createAttachmentCommand)
                  } catch (createError) {
                    console.error('Failed to add attachment:', i, createError)
                  }
                }
              }
            }
            this.sweetAlertStore.successDeleted('บันทึกข้อมูลสำเร็จ!')
            this.closeDialog(true)
          }
        } catch (error) {
          console.error('Error updating plan:', error)
          this.sweetAlertStore.error('เกิดข้อผิดพลาดในการบันทึก')
        } finally {
          this.loading = false
        }
      }
    },
  },
})
</script>

<style scoped>
.video-full-width {
  width: 100% !important;
  height: 300px !important;
  min-height: 300px !important;
  object-fit: cover !important;
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

/* Scrollable content area with custom scrollbar */
.scroll-content {
  overflow-y: auto;
  overflow-x: hidden;
  max-height: 70vh;
  scrollbar-width: thin;
  scrollbar-color: #667eea #f8f9fa;
  -webkit-overflow-scrolling: touch;
  overscroll-behavior: contain;
}

.scroll-content::-webkit-scrollbar {
  width: 8px;
  background: transparent;
}

.scroll-content::-webkit-scrollbar-thumb {
  background: linear-gradient(135deg, #667eea, #764ba2);
  border-radius: 4px;
}

.scroll-content::-webkit-scrollbar-track {
  background: rgba(0, 0, 0, 0.05);
  border-radius: 4px;
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

/* Title Styling */
.text-sub-title {
  font-size: 10px;
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
  white-space: nowrap;
}

/* Enhanced Buttons */
.mobile-btn {
  min-height: 52px;
  border-radius: 16px;
  font-size: 16px;
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

/* Enhanced Map Container */
.map-container {
  border-radius: 20px;
  overflow: hidden;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1), 0 2px 8px rgba(0, 0, 0, 0.05);
  border: 2px solid rgba(255, 255, 255, 0.2);
  background: #f8fafc;
  position: relative;
  height: 450px; /* เพิ่มความสูงตรงนี้ */
}

.map-iframe {
  width: 100%;
  border: none;
}

/* Check-in Section Styling */
.v-divider {
  background: linear-gradient(90deg, transparent, #e8eaed, transparent);
  height: 2px;
  border-radius: 1px;
}

/* Section Icons and Titles */
.section-icon {
  color: #667eea;
  font-size: 24px;
}

.section-title {
  color: #2b3086;
  font-weight: 600;
}

/* Camera Video Styling */
video {
  border-radius: 16px;
  border: 3px solid #e8eaed;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.1);
}

/* Alert Styling */
.v-alert {
  border-radius: 12px;
  border: none;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
}

/* Enhanced Checkbox */
.v-checkbox :deep(.v-selection-control__wrapper) {
  border-radius: 8px;
}

/* Textarea Enhancement */
.v-textarea :deep(.v-field) {
  border-radius: 20px;
}

.v-textarea :deep(.v-field__input) {
  padding: 20px;
}

/* Image Preview Enhancement */
.v-img {
  border-radius: 16px;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.12);
}

.v-img:hover {
  transform: none !important;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.12) !important;
}

.v-img :deep(.v-img__img) {
  transition: none !important;
}

.v-img :deep(.v-img__img):hover {
  transform: none !important;
  scale: 1 !important;
}

/* ===== RESPONSIVE BREAKPOINTS ===== */

/* Mobile Portrait (320px - 599px) */
@media (max-width: 599px) {
  .card-Dialog {
    margin: 0px;
  }

  .text-sub-title {
    font-size: 20px;
    line-height: 1.4;
  }

  .v-card-text {
    padding: 16px !important;
  }

  /* Stack form columns vertically */
  .v-row .v-col[sm='6'],
  .v-row .v-col[md='5'] {
    flex-basis: 100% !important;
    max-width: 100% !important;
    padding-bottom: 16px;
  }

  /* Mobile button styling */
  .mobile-btn {
    width: 100% !important;
    min-width: 100% !important;
    min-height: 56px;
    font-size: 16px;
    margin-bottom: 0px !important;
    border-radius: 14px;
  }

  /* Margin below action button row on mobile */
  .action-row {
    margin-bottom: 17% !important;
  }

  .mobile-btn.me-2 {
    margin-right: 0 !important;
  }

  /* Button container - stack vertically */
  .d-flex.justify-center.flex-wrap {
    flex-direction: column !important;
    align-items: stretch !important;
    gap: 0 !important;
  }

  /* Map responsive */
  .map-container {
    height: 450px !important;
  }

  .map-iframe {
    height: 450px !important;
    border-radius: 16px;
  }

  /* Video responsive */
  video {
    width: 100% !important;
    max-width: 100% !important;
    height: 200px !important;
    border-radius: 12px;
  }

  /* Image preview responsive */
  .v-img {
    width: 100% !important;
    max-width: 100% !important;
    height: 200px !important;
    border-radius: 12px;
  }

  /* Camera controls stack */
  .d-flex.flex-wrap.gap-2 {
    flex-direction: column !important;
    gap: 8px !important;
  }

  .d-flex.flex-wrap.gap-2 .v-btn {
    width: 100% !important;
    justify-content: center !important;
  }

  /* Reduce padding and margins */
  .px-2 {
    padding-left: 8px !important;
    padding-right: 8px !important;
  }

  .px-sm-4 {
    padding-left: 8px !important;
    padding-right: 8px !important;
  }

  .v-col {
    padding: 8px !important;
  }

  /* Section spacing */
  .mb-4 {
    margin-bottom: 16px !important;
  }

  .mt-3 {
    margin-top: 16px !important;
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
    padding: 12px 8px 8px;
  }

  /* Allow horizontal button layout in landscape */
  .d-flex.justify-center.flex-wrap {
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
    height: 420px !important;
  }

  .map-iframe {
    height: 420px !important;
  }
}

/* Tablet Portrait (600px - 959px) */
@media (min-width: 600px) and (max-width: 959px) {
  .card-Dialog {
    margin: 16px;
    max-width: calc(100vw - 32px);
  }

  .text-sub-title {
    font-size: 26px;
    padding: 20px 16px 12px;
  }

  .v-card-text {
    padding: 24px !important;
  }

  /* Two columns for date fields */
  .v-row .v-col[sm='6'] {
    flex-basis: 50% !important;
    max-width: 50% !important;
  }

  .v-row .v-col[md='5'] {
    flex-basis: 48% !important;
    max-width: 48% !important;
  }

  .mobile-btn {
    min-width: 160px;
    min-height: 48px;
    font-size: 15px;
  }

  .map-container {
    height: 650px !important;
  }

  .map-iframe {
    height: 650px !important;
  }

  video,
  .v-img {
    height: 220px !important;
  }

  /* Button layout */
  .d-flex.justify-center.flex-wrap {
    flex-direction: row !important;
    gap: 16px !important;
  }
}

/* Tablet Landscape / Small Desktop (960px - 1263px) */
@media (min-width: 960px) and (max-width: 1263px) {
  .card-Dialog {
    margin: 20px auto;
    max-width: 800px;
  }

  .text-sub-title {
    font-size: 28px;
    padding: 24px 20px 16px;
  }

  .v-card-text {
    padding: 32px !important;
  }

  .mobile-btn {
    min-width: 180px;
    min-height: 50px;
    font-size: 16px;
  }

  .map-container {
    height: 600px !important;
  }

  .map-iframe {
    height: 600px !important;
  }

  video,
  .v-img {
    height: 240px !important;
  }
}

/* Large Desktop (1264px+) */
@media (min-width: 1264px) {
  .card-Dialog {
    margin: 24px auto;
    max-width: 900px;
  }

  .text-sub-title {
    font-size: 32px;
    padding: 28px 24px 20px;
  }

  .v-card-text {
    padding: 40px !important;
  }

  .mobile-btn {
    min-width: 200px;
    min-height: 52px;
    font-size: 16px;
  }

  .map-iframe {
    height: 400px !important;
  }

  video,
  .v-img {
    height: 260px !important;
  }

  /* Ultra-wide screens (1920px+) */
  @media (min-width: 1920px) {
    .card-Dialog {
      max-width: 1000px;
    }

    .text-sub-title {
      .checkin-btn {
        width: 100%;
        border-radius: 22px;
        min-height: 44px;
        margin-bottom: 12px;
        box-shadow: 0 2px 8px rgba(0, 0, 0, 0.13);
        letter-spacing: 0.01em;
      }
      .checkin-btn:last-child {
        margin-bottom: 0;
      }
    }
  }

  /* ===== ACCESSIBILITY & PERFORMANCE ===== */

  /* High contrast mode support */
  @media (prefers-contrast: high) {
    .text-sub-title {
      background: none;
      -webkit-text-fill-color: initial;
      color: #000;
      text-shadow: none;
    }

    .mobile-btn {
      border: 2px solid currentColor;
    }
  }

  /* Reduced motion preferences */
  @media (prefers-reduced-motion: reduce) {
    * {
      animation-duration: 0.01ms !important;
      animation-iteration-count: 1 !important;
      transition-duration: 0.01ms !important;
    }

    .mobile-btn:hover {
      transform: none;
    }

    .map-container:hover {
      transform: none;
    }
  }

  /* ===== PRINT STYLES ===== */
  @media print {
    .card-Dialog {
      box-shadow: none;
      border: 1px solid #ccc;
      max-height: none;
      overflow: visible;
      margin: 0;
      padding: 20px;
    }

    .mobile-btn {
      display: none;
    }

    .map-container,
    video {
      border: 1px solid #ccc;
      background: #f5f5f5;
    }

    .text-sub-title {
      color: #000 !important;
      background: none !important;
      -webkit-text-fill-color: initial !important;
    }
  }

  /* ===== TOUCH OPTIMIZATION ===== */
  @media (pointer: coarse) {
    .mobile-btn {
      min-height: 56px; /* Larger touch targets */
    }

    .v-checkbox :deep(.v-selection-control__wrapper) {
      min-width: 44px;
      min-height: 44px;
    }
  }

  /* ===== LOADING STATES ===== */
  .mobile-btn[loading] {
    pointer-events: none;
    opacity: 0.7;
  }

  .mobile-btn[disabled] {
    pointer-events: none;
    opacity: 0.5;
    transform: none !important;
  }

  /* ===== ANIMATION ENHANCEMENTS ===== */
  @keyframes slideInUp {
    from {
      opacity: 0;
      transform: translateY(20px);
    }
    to {
      opacity: 1;
      transform: translateY(0);
    }
  }

  @keyframes pulse {
    0%,
    100% {
      opacity: 1;
    }
    50% {
      opacity: 0.7;
    }
  }

  .mobile-btn[loading] {
    animation: pulse 1.5s infinite;
  }
}

/* Responsive check-in button group */
.checkin-btn-group {
  display: flex;
  flex-direction: row;
  gap: 12px;
  justify-content: center;
  align-items: stretch;
  width: 100%;
}
/* ปุ่ม check-in ทั่วไป */
.checkin-btn {
  flex: 1 1 0;
  min-width: 0;
  margin: 0;
  border-radius: 18px;
  font-weight: 500;
  font-size: 1.08rem;
  min-height: 48px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
  transition: box-shadow 0.2s;
}
@media (max-width: 600px) {
  .checkin-btn-group {
    flex-direction: column;
    gap: 10px;
    align-items: stretch;
  }
  .checkin-btn {
    width: 100%;
    border-radius: 22px;
    font-size: 1.13rem;
    min-height: 44px;
    margin-bottom: 0;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.13);
    letter-spacing: 0.01em;
    font-weight: 500;
  }
}
@media (max-width: 600px) {
  .checkin-btn-group {
    flex-direction: column;
    gap: 10px;
    align-items: stretch;
  }
  .checkin-btn {
    width: 100%;
    border-radius: 15px;
  }
}
</style>
