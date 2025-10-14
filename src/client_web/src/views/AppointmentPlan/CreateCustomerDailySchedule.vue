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
            <span class="text-sub-title text-center">สร้างตารางแจ้งงานรายวัน</span>
          </v-col>
        </v-row>

        <v-card-text>
          <v-form
            ref="form"
            @submit.prevent="createActivityPlan"
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
                  v-model="createCommand.outSide"
                  :label="switchLabel"
                  hide-details
                  color="primary"
                  inset
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
                        v-if="createCommand.startDate"
                        placeholder="กรุณาระบุวันที่เริ่มต้นนัดหมาย"
                        :rules="appointmentStartDateRules"
                        :AllDay="createCommand.allDay"
                        :selectedDateTime="createCommand.startDate"
                        @selectedDateTime="changeTimestartDate"
                      />
                      <TextFieldDatepicker
                        v-else
                        placeholder="กรุณาระบุวันที่เริ่มต้นนัดหมาย"
                        :rules="appointmentStartDateRules"
                        :AllDay="createCommand.allDay"
                        @selectedDateTime="changeTimestartDate"
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
                        v-if="createCommand.endDate"
                        placeholder="กรุณาระบุวันที่สิ้นสุดนัดหมาย"
                        :rules="appointmentEndDateRules"
                        :AllDay="createCommand.allDay"
                        :selectedDateTime="createCommand.endDate"
                        @selectedDateTime="changeTimeendDate"
                      />
                      <TextFieldDatepicker
                        v-else
                        placeholder="กรุณาระบุวันที่สิ้นสุดนัดหมาย"
                        :minDate="createCommand.startDate"
                        :rules="appointmentEndDateRules"
                        :AllDay="createCommand.allDay"
                        :selectedDateTime="createCommand.endDate"
                        @selectedDateTime="changeTimeendDate"
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
                      v-model="createCommand.allDay"
                    />
                  </v-col>
                </v-row>
              </v-col>

              <v-col cols="12">
                <label class="mb-2">รหัสโครงการ </label>
                <v-autocomplete
                  placeholder="กรุณาเลือกรหัสโครงการ"
                  clearable
                  v-model="createCommand.projectId"
                  :items="ProjectList"
                  item-title="projectCodeAndName"
                  item-value="id"
                />
              </v-col>

              <v-col cols="12">
                <label class="mb-2">รายละเอียดเพิ่มเติม <span class="text-error">*</span></label>
                <v-textarea
                  v-model="createCommand.detail"
                  :rules="[(v:any) => !!v || 'กรุณาระบุรายละเอียด',
                    (v:any) => v.length >= 10 || 'กรุณาระบุรายละเอียดอย่างน้อย 10 ตัวอักษร'
                  ]"
                  placeholder="ระบุรายละเอียดการนัดพบ"
                />
              </v-col>

              <!-- Check-in Section -->
              <v-col
                cols="12"
                v-if="createCommand.outSide"
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
                    <!-- <label class="mb-2">ตำแหน่งปัจจุบัน</label>
                    <v-btn
                      color="primary"
                      @click="getLocation"
                      :loading="loadingLocation"
                      :disabled="loadingLocation"
                      class="mb-2 ml-3"
                      prepend-icon="ri-map-pin-line"
                    >
                      {{ loadingLocation ? 'กำลังดึงตำแหน่ง...' : 'ดึงตำแหน่งปัจจุบัน' }}
                    </v-btn> -->

                    <!-- <div
                      v-if="location"
                      class="mt-2"
                    >
                      <v-alert
                        type="info"
                        variant="tonal"
                        border="start"
                        color="primary"
                        class="pa-2 pl-4"
                      >
                        <div>
                          <strong>ละติจูด:</strong> {{ location.latitude }}<br />
                          <strong>ลองจิจูด:</strong> {{ location.longitude }}
                        </div>
                      </v-alert>
                    </div> -->

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

                        <div class="d-flex flex-wrap gap-3 justify-center mb-4">
                          <div class="checkin-btn-group">
                            <!-- <v-btn
                            color="primary"
                            @click="getLocation"
                            :loading="loadingLocation"
                            :disabled="loadingLocation"
                            class="checkin-btn"
                            prepend-icon="ri-map-pin-line"
                          >
                            {{ loadingLocation ? 'กำลังดึงตำแหน่ง...' : 'ดึงตำแหน่งปัจจุบัน' }}
                          </v-btn> -->

                            <v-btn
                              color="success-darken-2"
                              @click="capturePhoto"
                              :disabled="!cameraActive"
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
                            >
                              เปิดกล้อง
                            </v-btn>
                          </div>
                        </div>

                        <div
                          v-if="location"
                          class="mt-5"
                        >
                          <v-alert
                            variant="tonal"
                            color="primary"
                            class="pa-2 pl-4 mt-2"
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
                class="pt-4 pt-md-6"
              >
                <div v-if="$vuetify.display.mobile">
                  <v-row
                    class="d-flex action-btn-row"
                    no-gutters
                  >
                    <v-col
                      cols="6"
                      class="pr-1"
                    >
                      <v-btn
                        class="mobile-btn"
                        color="error"
                        @click="closeDialog"
                        :disabled="loading"
                        :size="$vuetify.display.mobile ? 'default' : 'large'"
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
                        type="submit"
                        color="success-darken-2"
                        :loading="loading"
                        :disabled="loading || !canSubmit"
                        :size="$vuetify.display.mobile ? 'default' : 'large'"
                        class="mobile-btn"
                        block
                      >
                        <v-icon class="mr-2">ri-save-3-fill</v-icon>
                        บันทึก
                      </v-btn>
                    </v-col>
                  </v-row>
                </div>
                <div
                  v-else
                  class="d-flex justify-center flex-wrap gap-2"
                >
                  <v-btn
                    class="me-2 mb-2 mb-sm-0 mobile-btn"
                    color="error"
                    @click="closeDialog"
                    :disabled="loading"
                    :size="$vuetify.display.mobile ? 'default' : 'large'"
                  >
                    <v-icon class="mr-2">ri-close-large-fill</v-icon>
                    ยกเลิก
                  </v-btn>
                  <v-btn
                    type="submit"
                    color="success-darken-2"
                    :loading="loading"
                    :disabled="loading || !canSubmit"
                    :size="$vuetify.display.mobile ? 'default' : 'large'"
                    class="mobile-btn"
                  >
                    <v-icon class="mr-2">ri-save-3-fill</v-icon>
                    บันทึก
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
import {
  Client,
  CreateActivityPlanAttachmentCommand,
  CreateActivityPlanCommand,
  CreateActivityPlanContactCommand,
  GetOrganizationContactByOrganizationIdQuery,
} from '@/client'
import TextFieldDatepicker from '@/components/Datepicker/TextFieldDatepicker.vue'
import TextFieldTimepicker from '@/components/Datepicker/TextFieldTimepicker.vue'
import { BACKEND_API_URL } from '@/constants'
import { useAuthStore, useSweetAlertStore } from '@/stores'
import {
  appointmentEndDateRules,
  appointmentLocationRules,
  appointmentPurposeRules,
  appointmentStartDateRules,
  otherPurposeRules,
  selectAgencyOrCustomerRules,
  selectCustomerRules,
} from '@/utils/RuleServices'
import DemoFormLayoutVerticalFormWithIcons from '@/views/pages/form-layouts/DemoFormLayoutVerticalFormWithIcons.vue'
import { LMap, LMarker, LTileLayer } from '@vue-leaflet/vue-leaflet'
import L from 'leaflet'
import 'leaflet/dist/leaflet.css'
import { defineComponent } from 'vue'

// FIX for Vite: Leaflet's default marker icon issue
L.Icon.Default.mergeOptions({
  iconRetinaUrl: new URL('leaflet/dist/images/marker-icon-2x.png', import.meta.url).href,
  iconUrl: new URL('leaflet/dist/images/marker-icon.png', import.meta.url).href,
  shadowUrl: new URL('leaflet/dist/images/marker-shadow.png', import.meta.url).href,
})

const client = new Client(BACKEND_API_URL)
export default defineComponent({
  name: 'CreateCustomerDailySchedule',
  components: {
    DemoFormLayoutVerticalFormWithIcons,
    TextFieldDatepicker,
    TextFieldTimepicker,
    LMap,
    LMarker,
    LTileLayer,
  },
  props: {
    CloseDialogCreate: {
      type: Function,
      required: true,
    },
  },
  data() {
    return {
      OrganizationList: [] as any,
      ProjectList: [] as any,
      objectiveList: [
        'ประชุมงาน / โครงการ',
        'ทานข้าว',
        'นำของไปฝาก',
        'ไปร่วมจัดอบรม / ตรวจรับงาน',
        'ไปสัญจร / ไปต่างจังหวัด',
        'ไปต่างประเทศ / ไปดูงาน',
        'อื่น ๆ',
      ] as any,
      OrganizationContactList: [] as any,
      startDate: undefined as any,
      endDate: undefined as any,
      selectedObjective: [] as any,
      otherObjective: '',
      searchText: '' as any,
      searchTextContact: '' as any,
      customers: [] as any,
      AllDay: false,
      createCommand: new CreateActivityPlanCommand({
        startDate: new Date(),
        endDate: new Date(),
        allDay: true,
        outSide: false,
      }),
      requestOrganizationContact: new GetOrganizationContactByOrganizationIdQuery(),
      auth: useAuthStore(),
      sweetAlertStore: useSweetAlertStore(),
      loading: false,
      createContact: new CreateActivityPlanContactCommand(),
      User: {} as any,
      appointmentStartDateRules,
      appointmentEndDateRules,
      selectAgencyOrCustomerRules,
      selectCustomerRules,
      appointmentLocationRules,
      appointmentPurposeRules,
      otherPurposeRules,
      // Check-in related data
      location: null as any,
      locationError: '',
      locationName: '',
      loadingLocation: false,
      cameraSupported: false,
      cameraActive: false,
      cameraError: '',
      photoData: null as any,
      stream: null as any,
      imageSrcList: [] as any,
    }
  },
  computed: {
    switchLabel() {
      return this.createCommand.outSide ? 'ออกนอกสถานที่' : 'เข้าออฟฟิศ'
    },
    canSubmit() {
      if (this.createCommand.outSide) {
        return this.location && this.createCommand.detail && this.createCommand.detail.length >= 10
      } else {
        return this.createCommand.detail && this.createCommand.detail.length >= 10
      }
    },
    showAddButton() {
      return this.searchText && this.searchText.trim() !== '' && !this.hasMatchingItems
    },
    hasMatchingItems() {
      if (!this.searchText || this.searchText.trim() === '') return true
      return this.OrganizationList.some((item: any) => item.name.toLowerCase().includes(this.searchText.toLowerCase()))
    },
    showAddButtonContact() {
      const noContact =
        this.createCommand.organizationId &&
        (!this.OrganizationContactList || this.OrganizationContactList.length === 0)

      const noSearchMatch =
        this.searchTextContact && this.searchTextContact.trim() !== '' && !this.hasMatchingItemsContact

      return noContact || noSearchMatch
    },
    hasMatchingItemsContact() {
      if (!this.searchTextContact || this.searchTextContact.trim() === '') return true
      return this.OrganizationContactList.some((item: any) =>
        item.name.toLowerCase().includes(this.searchTextContact.toLowerCase()),
      )
    },
    coordinatesMessage(): string {
      if (this.locationError) return this.locationError
      return 'ยังไม่ได้ดึงตำแหน่งปัจจุบัน'
    },
    googleMapsUrl(): string {
      if (!this.location) return ''
      return `https://maps.google.com/maps?q=${this.location.latitude},${this.location.longitude}&z=16&output=embed`
    },
    isValidCoordinates(): boolean {
      return this.location && typeof this.location.latitude === 'number' && typeof this.location.longitude === 'number'
    },
  },
  watch: {
    'createCommand.outSide'(val: boolean) {
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
        this.createCommand.location = ''
      }
    },
  },
  async mounted() {
    await this.initialize()
  },
  beforeUnmount() {
    this.stopCamera()
  },
  methods: {
    removeImage(index: number) {
      this.imageSrcList.splice(index, 1)
    },
    closeDialog(reload: boolean = false, searchData?: string) {
      if (!reload) {
        localStorage.removeItem('selectedAppointmentId')
      }
      this.stopCamera()
      this.CloseDialogCreate(false, reload, searchData)
    },
    handleSearchUpdate(value: any) {
      this.searchText = value
    },
    handleSearchContactUpdate(value: any) {
      this.searchTextContact = value
    },
    async initialize() {
      try {
        this.OrganizationList = await client.getOrganizationQuery()
        if (this.auth.userId) {
          console.log(this.auth.userId)
          const result = await client.getEmployeeQueryByUserID(this.auth.userId)
          this.createCommand.employeeId = result.id
          this.User = result
        }

        this.ProjectList = await client.getProjectQuery()
        this.ProjectList.forEach((project: any) => {
          const truncatedName =
            project.projectName.length > 70 ? project.projectName.substring(0, 70) + '...' : project.projectName
          project.projectCodeAndName = `${project.projectCode} - ${truncatedName}`
        })
      } catch (error) {
        console.error(error)
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
          this.createCommand.location = `${pos.coords.latitude},${pos.coords.longitude}`

          try {
            const response = await fetch(
              `https://nominatim.openstreetmap.org/reverse?format=jsonv2&lat=${pos.coords.latitude}&lon=${pos.coords.longitude}`,
            )
            const data = await response.json()
            if (data && data.display_name) {
              this.location.address = data.display_name
            } else {
              this.location.address = 'ไม่พบข้อมูลสถานที่'
            }
          } catch (error) {
            this.location.address = 'เกิดข้อผิดพลาดในการเรียกข้อมูลสถานที่'
          }

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
      this.photoData = canvas.toDataURL('image/png')
      if (this.photoData) {
        this.imageSrcList.push({ src: this.photoData, create: false, id: null })
      }
      this.stopCamera()
    },
    onFileChange(e: Event) {
      const target = e.target as HTMLInputElement
      if (target && target.files && target.files.length > 0) {
        const file = target.files[0]
        const reader = new FileReader()
        reader.onload = ev => {
          if (ev.target) {
            this.photoData = ev.target.result
            if (this.photoData) {
              this.imageSrcList.push({ src: this.photoData, create: false, id: null })
            }
          }
        }
        reader.readAsDataURL(file)
      }
    },
    async getOrganizationContact() {
      try {
        this.requestOrganizationContact.organizationId = this.createCommand.organizationId
        this.OrganizationContactList = await client.getOrganizationContactQueryByOrganizationId(
          this.requestOrganizationContact,
        )
        this.OrganizationContactList.forEach((OC: any) => {
          OC.name = `${OC.firstName || ''}   ${OC.lastName || ''}`.trim()
        })
      } catch (error) {
        console.error(error)
      }
    },
    CancelCreate() {
      this.$router.push({ name: 'CustomerAppointmentListView' })
    },
    removeCustomer(index: number) {
      this.customers.splice(index, 1)
    },
    changeTimestartDate(value: any) {
      this.createCommand.startDate = value
    },
    changeTimeendDate(value: any) {
      this.createCommand.endDate = value
    },
    async createActivityPlan() {
      const form = this.$refs.form as any
      const { valid } = await form.validate()

      if (valid && this.canSubmit) {
        this.loading = true
        try {
          this.createCommand.eventType = '002'
          if (this.location) {
            this.createCommand.location = `${this.location.latitude},${this.location.longitude}`
          }

          const response = await client.createActivityPlan(this.createCommand)

          this.stopCamera()

          if (response && this.imageSrcList && this.imageSrcList.length > 0) {
            for (let i = 0; i < this.imageSrcList.length; i++) {
              let base64Data = ''
              if (typeof this.imageSrcList[i].src === 'string' && this.imageSrcList[i].src.includes(',')) {
                base64Data = this.imageSrcList[i].src.split(',')[1]
              }
              if (base64Data) {
                const attachmentCommand = new CreateActivityPlanAttachmentCommand()
                attachmentCommand.activityPlanId = response
                attachmentCommand.fileName = `photo_${Date.now()}_${i}.png`
                attachmentCommand.base64 = base64Data
                try {
                  await client.createActivityPlanAttachment(attachmentCommand)
                } catch (err) {
                  console.error('[CreateCustomerDailySchedule] อัพโหลดรูปไม่สำเร็จ:', err)
                }
              } else {
                console.warn('[CreateCustomerDailySchedule] ไม่พบข้อมูล base64 สำหรับอัพโหลดรูป index:', i)
              }
            }
          }

          if (this.customers.length > 0) {
            try {
              this.createContact.activityPlanId = response
              for (let i = 0; i < this.customers.length; i++) {
                this.createContact.organizationContactId = this.customers[i]
                const result = await client.createActivityPlanContact(this.createContact)
                if (result) console.log('[CreateCustomerDailySchedule] ----สร้าง Contact แล้ว----- ')
              }
            } catch (error) {
              console.error('[CreateCustomerDailySchedule] เกิดข้อผิดพลาดในการสร้าง Contact:', error)
            }
          }

          if (response) {
            localStorage.setItem('selectedAppointmentId', String(response))

            const reloadEvent = new CustomEvent('reloadAppointmentPlan', {
              detail: {
                newAppointmentId: String(response),
                eventTypeToSelect: this.createCommand.eventType,
              },
            })
            window.dispatchEvent(reloadEvent)

            this.sweetAlertStore.successDeleted('สร้างตารางแจ้งงานรายวันสำเร็จ!')

            // Reset form data after successful create
            this.createCommand = new CreateActivityPlanCommand({
              startDate: new Date(),
              endDate: new Date(),
              allDay: true,
              outSide: false,
            })
            this.customers = []
            this.imageSrcList = []
            this.location = null
            this.locationError = ''
            this.photoData = null

            this.stopCamera()
            this.closeDialog()
          }
        } catch (error) {
          console.error('[CreateCustomerDailySchedule] เกิดข้อผิดพลาดในการสร้างตารางแจ้งงานรายวัน :', error)
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

/* Base Styles */
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

/* Title Styling */
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
  line-height: 1.5;
  letter-spacing: -0.02em;
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
    /* max-width: calc(100vw - 16px); */
    max-height: 95vh;
  }

  .text-sub-title {
    font-size: 30px;
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
    border-radius: 14px;
    margin-bottom: 0 !important;
  }

  .pr-1 {
    padding-right: 6px !important;
  }
  .pl-1 {
    padding-left: 6px !important;
  }

    /* Margin below action button row on mobile */
  .action-btn-row {
    margin-bottom: 20% !important;
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
      font-size: 36px;
    }

    .map-container {
      height: 500px !important;
    }

    .map-iframe {
      height: 500px !important;
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
